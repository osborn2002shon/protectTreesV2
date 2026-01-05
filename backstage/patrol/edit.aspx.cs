using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;
using protectTreesV2.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using static protectTreesV2.Patrol.Patrol;

namespace protectTreesV2.backstage.patrol
{
    public partial class edit : BasePage
    {
        private readonly protectTreesV2.Patrol.Patrol system_patrol = new protectTreesV2.Patrol.Patrol();

        public int CurrentPatrolID
        {
            get { return (int)(ViewState["CurrentPatrolID"] ?? 0); }
            set { ViewState["CurrentPatrolID"] = value; }
        }

        public int CurrentTreeID
        {
            get { return (int)(ViewState["CurrentTreeID"] ?? 0); }
            set { ViewState["CurrentTreeID"] = value; }
        }

        public string Action => CurrentPatrolID > 0 ? "edit" : "add";

        protected void Page_Load(object sender, EventArgs e)
        {
            LinkButton_save.OnClientClick = "return confirmPatrolRisk();";

            if (!IsPostBack)
            {
                InitIdsFromSession();

                if (CurrentPatrolID == 0 && CurrentTreeID == 0)
                {
                    ShowMessage("錯誤", "無效的參數，請由列表頁進入。");
                    Response.Redirect("list.aspx");
                    return;
                }

                LoadRecord();
                SetPageTitle();
                BindTreeInfo();
                BindPhotoData();

                Literal_btnSaveText.Text = Action == "add" ? "新增" : "儲存";
                HiddenField_treeId.Value = CurrentTreeID.ToString();
                HiddenField_patrolId.Value = CurrentPatrolID.ToString();
            }
        }

        private void InitIdsFromSession()
        {
            if (!string.IsNullOrEmpty(this.setPatrolID))
            {
                if (int.TryParse(this.setPatrolID, out int patrolId))
                {
                    CurrentPatrolID = patrolId;
                }
                this.setPatrolID = null;
            }

            if (!string.IsNullOrEmpty(this.setTreeID))
            {
                if (int.TryParse(this.setTreeID, out int treeId))
                {
                    CurrentTreeID = treeId;
                }
                this.setTreeID = null;
            }
        }

        private void SetPageTitle()
        {
            if (this.Action == "add")
            {
                Literal_pathAction.Text = "新增";
                Literal_title.Text = "巡查紀錄新增";
                Label_recordStatus.CssClass = "badge bg-secondary";
                Label_recordStatus.Text = "新增";
            }
            else
            {
                Literal_pathAction.Text = "編輯";
                Literal_title.Text = "巡查紀錄編輯";
            }
        }

        private void LoadRecord()
        {
            if (this.Action == "edit")
            {
                var record = system_patrol.GetPatrolRecord(this.CurrentPatrolID);
                if (record == null)
                {
                    ShowMessage("錯誤", "找不到巡查資料");
                    Response.Redirect("list.aspx");
                    return;
                }

                CurrentTreeID = record.treeID;

                TextBox_patrolDate.Text = record.patrolDate?.ToString("yyyy-MM-dd");
                TextBox_patroller.Text = record.patroller;
                TextBox_memo.Text = record.memo;
                CheckBox_isFinal.Checked = record.dataStatus == (int)PatrolRecordStatus.完稿;
                CheckBox_hasPublicSafetyRisk.Checked = record.hasPublicSafetyRisk;

                Label_recordStatus.CssClass = record.dataStatus == (int)PatrolRecordStatus.完稿 ? "badge bg-success" : "badge bg-warning text-dark";
                Label_recordStatus.Text = record.dataStatus == (int)PatrolRecordStatus.完稿 ? "定稿" : "草稿";
            }
            else
            {
                Label_recordStatus.CssClass = "badge bg-secondary";
                Label_recordStatus.Text = "新增";
            }
        }

        private void BindTreeInfo()
        {
            var tree = TreeService.GetTree(CurrentTreeID);
            if (tree == null)
            {
                ShowMessage("錯誤", "無法取得樹籍資料");
                Response.Redirect("list.aspx");
                return;
            }

            Label_systemTreeNo.Text = tree.SystemTreeNo ?? "--";
            Label_speciesName.Text = tree.SpeciesDisplayName ?? "--";
            Label_cityName.Text = tree.CityName ?? "--";
            Label_areaName.Text = tree.AreaName;
            Label_manager.Text = tree.Manager ?? "--";
        }

        private void BindPhotoData()
        {
            var serializer = new JavaScriptSerializer();
            if (CurrentPatrolID <= 0)
            {
                HiddenField_existingPhotosData.Value = serializer.Serialize(new List<object>());
                return;
            }

            var photos = system_patrol.GetPatrolPhotos(CurrentPatrolID)
                .Select(p => new
                {
                    key = p.PhotoID,
                    fileName = p.FileName,
                    filePath = p.FilePath,
                    caption = p.Caption
                }).ToList();

            HiddenField_existingPhotosData.Value = serializer.Serialize(photos);
        }

        protected void LinkButton_save_Click(object sender, EventArgs e)
        {
            if (!ValidateForm(out DateTime? patrolDate))
            {
                return;
            }

            bool isFinal = CheckBox_isFinal.Checked;
            bool hasRisk = CheckBox_hasPublicSafetyRisk.Checked;

            var record = this.Action == "edit"
                ? system_patrol.GetPatrolRecord(this.CurrentPatrolID) ?? new PatrolRecord()
                : new PatrolRecord();

            record.treeID = this.CurrentTreeID;
            record.patrolDate = patrolDate;
            record.patroller = TextBox_patroller.Text.Trim();
            record.memo = TextBox_memo.Text.Trim();
            record.hasPublicSafetyRisk = hasRisk;
            record.dataStatus = isFinal ? (int)PatrolRecordStatus.完稿 : (int)PatrolRecordStatus.草稿;

            var tree = TreeService.GetTree(CurrentTreeID);
            if (record.patrolID == 0 && tree != null)
            {
                record.sourceUnit = tree.SourceUnit;
                record.sourceUnitID = tree.SourceUnitID;
            }

            var user = UserService.GetCurrentUser();
            int accountId = user?.userID ?? 0;

            if (record.patrolID > 0)
            {
                system_patrol.UpdatePatrolRecord(record, accountId);
            }
            else
            {
                int newId = system_patrol.InsertPatrolRecord(record, accountId);
                CurrentPatrolID = newId;
                record.patrolID = newId;
            }

            if (!ProcessPhotos(record.patrolID, accountId, isFinal))
            {
                return;
            }

            if (isFinal && hasRisk)
            {
                TrySendRiskNotification(record, tree);
            }

            Response.Redirect("list.aspx");
        }

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("list.aspx");
        }

        private bool ValidateForm(out DateTime? patrolDate)
        {
            patrolDate = null;
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(TextBox_patrolDate.Text) || !DateTime.TryParse(TextBox_patrolDate.Text, out DateTime parsed))
            {
                missing.Add("巡查日期");
            }
            else
            {
                patrolDate = parsed;
            }

            if (string.IsNullOrWhiteSpace(TextBox_patroller.Text))
            {
                missing.Add("巡查人姓名");
            }

            if (missing.Any())
            {
                ShowMessage("驗證", $"請填寫：{string.Join("、", missing)}", "warning");
                return false;
            }

            bool isFinal = CheckBox_isFinal.Checked;
            var deleted = ParseDeletedPhotoIds();
            var existing = CurrentPatrolID > 0 ? system_patrol.GetPatrolPhotos(CurrentPatrolID) : new List<PatrolPhoto>();
            int remainingExisting = existing.Count(p => !deleted.Contains(p.PhotoID));
            var files = GetPostedFiles();

            if (files.Any(f => f.ContentLength > 10 * 1024 * 1024))
            {
                ShowMessage("限制", "單張照片大小不可超過 10MB", "warning");
                return false;
            }

            if (isFinal && (remainingExisting + files.Count == 0))
            {
                ShowMessage("限制", "定稿至少需要一張巡查照片", "warning");
                return false;
            }

            return true;
        }

        private bool ProcessPhotos(int patrolId, int accountId, bool isFinal)
        {
            var metadata = ParsePhotoMetadata();
            var deleted = ParseDeletedPhotoIds();
            var existing = system_patrol.GetPatrolPhotos(patrolId);

            foreach (int id in deleted)
            {
                var photo = existing.FirstOrDefault(p => p.PhotoID == id);
                if (photo != null)
                {
                    system_patrol.SoftDeletePatrolPhoto(id, accountId);
                    string physical = Server.MapPath(photo.FilePath);
                    if (File.Exists(physical))
                    {
                        File.Delete(physical);
                    }
                }
            }

            foreach (var photo in existing.Where(p => !deleted.Contains(p.PhotoID)))
            {
                string caption = metadata.ContainsKey(photo.PhotoID) ? metadata[photo.PhotoID].caption : photo.Caption;
                system_patrol.UpdatePatrolPhotoCaption(photo.PhotoID, caption, accountId);
            }

            var files = GetPostedFiles();
            var newMetas = metadata.Where(m => m.Key < 0).Select(m => m.Value).ToList();
            var usedFiles = new HashSet<int>();

            string uploadFolder = Server.MapPath(string.Format(CultureInfo.InvariantCulture, "~/_file/patrol/img/{0}/", patrolId));
            Directory.CreateDirectory(uploadFolder);

            foreach (var meta in newMetas)
            {
                if (string.IsNullOrWhiteSpace(meta.fileName)) continue;

                int fileIndex = -1;
                for (int i = 0; i < files.Count; i++)
                {
                    if (usedFiles.Contains(i)) continue;
                    var candidate = files[i];
                    if (candidate != null && string.Equals(Path.GetFileName(candidate.FileName), meta.fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        fileIndex = i;
                        break;
                    }
                }

                if (fileIndex < 0) continue;

                usedFiles.Add(fileIndex);
                var file = files[fileIndex];
                if (file == null || file.ContentLength == 0) continue;

                string originalName = Path.GetFileName(file.FileName);
                string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, originalName);
                string physicalPath = Path.Combine(uploadFolder, savedName);
                file.SaveAs(physicalPath);

                var photo = new PatrolPhoto
                {
                    PatrolID = patrolId,
                    FileName = originalName,
                    FilePath = string.Format(CultureInfo.InvariantCulture, "/_file/patrol/img/{0}/{1}", patrolId, savedName),
                    FileSize = file.ContentLength,
                    Caption = meta.caption
                };

                system_patrol.InsertPatrolPhoto(photo, accountId);
            }

            var remaining = system_patrol.GetPatrolPhotos(patrolId).Count(p => !deleted.Contains(p.PhotoID));
            if (isFinal && remaining == 0)
            {
                ShowMessage("限制", "定稿至少需要一張巡查照片", "warning");
                return false;
            }

            return true;
        }

        private Dictionary<int, (string fileName, string caption)> ParsePhotoMetadata()
        {
            var result = new Dictionary<int, (string fileName, string caption)>();
            string raw = HiddenField_photoMetadata.Value;
            if (string.IsNullOrWhiteSpace(raw)) return result;

            try
            {
                var serializer = new JavaScriptSerializer();
                var items = serializer.Deserialize<List<Dictionary<string, object>>>(raw);
                foreach (var item in items)
                {
                    if (!item.ContainsKey("key")) continue;
                    int key;
                    if (!int.TryParse(item["key"].ToString(), out key)) continue;

                    string fileName = item.ContainsKey("fileName") ? item["fileName"]?.ToString() : null;
                    string caption = item.ContainsKey("caption") ? item["caption"]?.ToString() : null;

                    result[key] = (fileName, caption);
                }
            }
            catch
            {
            }

            return result;
        }

        private List<int> ParseDeletedPhotoIds()
        {
            var ids = new List<int>();
            var parts = (HiddenField_deletedPhotoIds.Value ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                if (int.TryParse(p, out int id))
                {
                    ids.Add(id);
                }
            }
            return ids;
        }

        private List<HttpPostedFile> GetPostedFiles()
        {
            var files = new List<HttpPostedFile>();
            if (FileUpload_pendingPhotos.HasFiles)
            {
                files.AddRange(FileUpload_pendingPhotos.PostedFiles
                    .Cast<HttpPostedFile>()
                    .Where(f => f != null && f.ContentLength > 0));
            }
            return files;
        }

        private void TrySendRiskNotification(PatrolRecord record, TreeRecord tree)
        {
            if (tree == null) return;

            try
            {
                var emails = system_patrol.GetRiskNotificationEmails(tree.CityID ?? -1);
                if (emails == null || emails.Count == 0) return;

                string actionText = this.Action == "add" ? "新增" : "編輯";
                string subject = $"巡查公共安全風險通知 - {tree.SystemTreeNo ?? "樹籍"}";
                string body = $"樹籍：{tree.SystemTreeNo ?? "未提供"} ({tree.Site})\n" +
                              $"此次巡查被標記為有公共安全風險，來源：{actionText}。\n" +
                              $"巡查日期：{record.patrolDate:yyyy/MM/dd}\n" +
                              $"巡查人：{record.patroller}\n" +
                              $"送出時間：{DateTime.Now:yyyy/MM/dd HH:mm}\n" +
                              $"備註：{record.memo}";

                foreach (var mail in emails.Where(m => !string.IsNullOrWhiteSpace(m)))
                {
                    EmailService.SendMail(mail, subject, body);
                }
            }
            catch
            {
                // 若寄信失敗不阻斷流程
            }
        }
    }
}
