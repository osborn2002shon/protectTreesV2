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

        [Serializable]
        private class TempUploadInfo
        {
            public int Key { get; set; }
            public string FileName { get; set; }
            public string VirtualPath { get; set; }
            public string PhysicalPath { get; set; }
            public string Caption { get; set; }
        }

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

        private string PendingUploadSessionKey
        {
            get
            {
                if (ViewState["PendingUploadSessionKey"] == null)
                {
                    ViewState["PendingUploadSessionKey"] = $"PatrolPendingUploads_{Session.SessionID}_{CurrentPatrolID}_{CurrentTreeID}";
                }

                return (string)ViewState["PendingUploadSessionKey"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LinkButton_save.OnClientClick = "return confirmPatrolRisk();";

            if (!IsPostBack)
            {
                InitIdsFromSession();
                ClearPendingUploads();

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
                CheckBox_isFinal.Checked = record.dataStatus == (int)PatrolRecordStatus.定稿;
                CheckBox_hasPublicSafetyRisk.Checked = record.hasPublicSafetyRisk;

                Label_recordStatus.CssClass = record.dataStatus == (int)PatrolRecordStatus.定稿 ? "badge bg-success" : "badge bg-warning text-dark";
                Label_recordStatus.Text = record.dataStatus == (int)PatrolRecordStatus.定稿 ? "定稿" : "草稿";
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

        private void BindPhotoData(Dictionary<int, (string fileName, string caption)> metadata = null)
        {
            var serializer = new JavaScriptSerializer();
            var photos = new List<object>();

            if (CurrentPatrolID > 0)
            {
                var existingPhotos = system_patrol.GetPatrolPhotos(CurrentPatrolID)
                    .Select(p => new
                    {
                        key = p.PhotoID,
                        fileName = p.FileName,
                        filePath = p.FilePath,
                        caption = p.Caption
                    }).ToList();
                photos.AddRange(existingPhotos);
            }

            if (metadata != null)
            {
                SyncPendingUploadsWithMetadata(metadata);
            }

            foreach (var temp in GetPendingUploads())
            {
                if (File.Exists(temp.PhysicalPath) || !string.IsNullOrEmpty(temp.VirtualPath))
                {
                    photos.Add(new
                    {
                        key = temp.Key,
                        fileName = temp.FileName,
                        filePath = temp.VirtualPath,
                        caption = temp.Caption,
                        isTemp = true
                    });
                }
            }

            HiddenField_existingPhotosData.Value = serializer.Serialize(photos);
        }

        private void SavePendingUploads(Dictionary<int, (string fileName, string caption)> metadata)
        {
            var files = GetPostedFiles();
            if (files.Count == 0) return;

            var pending = GetPendingUploads();
            string tempFolder = GetTempFolderPath();
            Directory.CreateDirectory(tempFolder);

            foreach (var file in files)
            {
                if (file == null || file.ContentLength == 0) continue;
                if (file.ContentLength > 10 * 1024 * 1024) continue;

                string originalName = Path.GetFileName(file.FileName);
                int key = 0;
                if (metadata != null)
                {
                    key = metadata.Where(m => m.Key < 0 && string.Equals(m.Value.fileName, originalName, StringComparison.OrdinalIgnoreCase))
                                  .Select(m => m.Key)
                                  .FirstOrDefault();
                }

                if (key == 0)
                {
                    key = GetNextTempKey(pending);
                }

                var existed = pending.FirstOrDefault(p => p.Key == key);
                if (existed != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(existed.PhysicalPath) && File.Exists(existed.PhysicalPath))
                        {
                            File.Delete(existed.PhysicalPath);
                        }
                    }
                    catch { }
                    pending.Remove(existed);
                }

                string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, originalName);
                string physicalPath = Path.Combine(tempFolder, savedName);
                file.SaveAs(physicalPath);

                string virtualPath = string.Format(CultureInfo.InvariantCulture, "/_file/patrol/temp/{0}/{1}", Session.SessionID, savedName);
                string caption = metadata != null && metadata.ContainsKey(key) ? metadata[key].caption : string.Empty;

                pending.Add(new TempUploadInfo
                {
                    Key = key,
                    FileName = originalName,
                    PhysicalPath = physicalPath,
                    VirtualPath = virtualPath,
                    Caption = caption
                });
            }

            Session[PendingUploadSessionKey] = pending;
        }

        private void SyncPendingUploadsWithMetadata(Dictionary<int, (string fileName, string caption)> metadata)
        {
            if (metadata == null) return;

            var pending = GetPendingUploads();
            var validKeys = new HashSet<int>(metadata.Where(m => m.Key < 0).Select(m => m.Key));

            for (int i = pending.Count - 1; i >= 0; i--)
            {
                var temp = pending[i];
                if (!validKeys.Contains(temp.Key))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(temp.PhysicalPath) && File.Exists(temp.PhysicalPath))
                        {
                            File.Delete(temp.PhysicalPath);
                        }
                    }
                    catch { }
                    pending.RemoveAt(i);
                }
                else
                {
                    if (metadata.ContainsKey(temp.Key))
                    {
                        temp.Caption = metadata[temp.Key].caption;
                    }
                    else
                    {
                        var matchByName = metadata.FirstOrDefault(m =>
                            m.Key < 0 && string.Equals(m.Value.fileName, temp.FileName, StringComparison.OrdinalIgnoreCase));
                        if (!matchByName.Equals(default(KeyValuePair<int, (string fileName, string caption)>)))
                        {
                            temp.Caption = matchByName.Value.caption;
                            temp.Key = matchByName.Key;
                        }
                    }
                }
            }

            Session[PendingUploadSessionKey] = pending;
        }

        private int GetNextTempKey(List<TempUploadInfo> pending)
        {
            return pending.Any() ? pending.Min(p => p.Key) - 1 : -1;
        }

        private string GetTempFolderPath()
        {
            return Server.MapPath(string.Format(CultureInfo.InvariantCulture, "~/_file/patrol/temp/{0}/", Session.SessionID));
        }

        private List<TempUploadInfo> GetPendingUploads()
        {
            var pending = Session[PendingUploadSessionKey] as List<TempUploadInfo>;
            if (pending == null)
            {
                pending = new List<TempUploadInfo>();
                Session[PendingUploadSessionKey] = pending;
            }
            return pending;
        }

        private void ClearPendingUploads()
        {
            var pending = Session[PendingUploadSessionKey] as List<TempUploadInfo>;
            if (pending != null)
            {
                foreach (var temp in pending)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(temp.PhysicalPath) && File.Exists(temp.PhysicalPath))
                        {
                            File.Delete(temp.PhysicalPath);
                        }
                    }
                    catch { }
                }
            }

            Session[PendingUploadSessionKey] = null;

            try
            {
                string folder = GetTempFolderPath();
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
            catch
            {
            }
        }

        protected void LinkButton_save_Click(object sender, EventArgs e)
        {
            var metadata = ParsePhotoMetadata();
            SavePendingUploads(metadata);
            SyncPendingUploadsWithMetadata(metadata);

            bool isFinal = CheckBox_isFinal.Checked;
            bool hasRisk = CheckBox_hasPublicSafetyRisk.Checked;

            if (!ValidateForm(out DateTime? patrolDate, isFinal))
            {
                BindPhotoData(metadata);
                return;
            }

            var record = this.Action == "edit"
                ? system_patrol.GetPatrolRecord(this.CurrentPatrolID) ?? new PatrolRecord()
                : new PatrolRecord();

            record.treeID = this.CurrentTreeID;
            record.patrolDate = patrolDate;
            record.patroller = TextBox_patroller.Text.Trim();
            record.memo = TextBox_memo.Text.Trim();
            record.hasPublicSafetyRisk = hasRisk;
            record.dataStatus = isFinal ? (int)PatrolRecordStatus.定稿 : (int)PatrolRecordStatus.草稿;

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

            if (!ProcessPhotos(record.patrolID, accountId, isFinal, metadata))
            {
                BindPhotoData(metadata);
                return;
            }

            if (isFinal && hasRisk)
            {
                TrySendRiskNotification(record, tree);
            }

            ClearPendingUploads();
            Response.Redirect("list.aspx");
        }

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {
            ClearPendingUploads();
            Response.Redirect("list.aspx");
        }

        private bool ValidateForm(out DateTime? patrolDate, bool isFinal)
        {
            patrolDate = null;
            var missing = new List<string>();

            if (!string.IsNullOrWhiteSpace(TextBox_patrolDate.Text))
            {
                if (DateTime.TryParse(TextBox_patrolDate.Text, out DateTime parsed))
                {
                    patrolDate = parsed;
                }
                else
                {
                    missing.Add("巡查日期格式不正確");
                }
            }
            else
            {
                missing.Add("巡查日期");
            }

            if (string.IsNullOrWhiteSpace(TextBox_patroller.Text) && isFinal)
            {
                missing.Add("巡查人姓名");
            }

            if (missing.Any())
            {
                ShowMessage("驗證", $"請填寫：{string.Join("、", missing)}", "warning");
                return false;
            }

            var deleted = ParseDeletedPhotoIds();
            var existing = CurrentPatrolID > 0 ? system_patrol.GetPatrolPhotos(CurrentPatrolID) : new List<PatrolPhoto>();
            int remainingExisting = existing.Count(p => !deleted.Contains(p.PhotoID));
            var files = GetPostedFiles();
            var pendingTemp = GetPendingUploads();

            if (files.Any(f => f.ContentLength > 10 * 1024 * 1024))
            {
                ShowMessage("限制", "單張照片大小不可超過 10MB", "warning");
                return false;
            }

            if (isFinal && (remainingExisting + files.Count + pendingTemp.Count == 0))
            {
                ShowMessage("限制", "定稿至少需要一張巡查照片", "warning");
                return false;
            }

            return true;
        }

        private bool ProcessPhotos(int patrolId, int accountId, bool isFinal, Dictionary<int, (string fileName, string caption)> metadata)
        {
            var deleted = ParseDeletedPhotoIds();
            var existing = system_patrol.GetPatrolPhotos(patrolId);
            var pendingUploads = GetPendingUploads();

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
            var processedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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
                processedFileNames.Add(meta.fileName);
            }

            foreach (var meta in newMetas.Where(m => !processedFileNames.Contains(m.fileName)))
            {
                var temp = pendingUploads.FirstOrDefault(p =>
                    string.Equals(p.FileName, meta.fileName, StringComparison.OrdinalIgnoreCase));

                if (temp == null || !File.Exists(temp.PhysicalPath)) continue;

                string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, temp.FileName);
                string targetPath = Path.Combine(uploadFolder, savedName);
                File.Move(temp.PhysicalPath, targetPath);

                var photo = new PatrolPhoto
                {
                    PatrolID = patrolId,
                    FileName = temp.FileName,
                    FilePath = string.Format(CultureInfo.InvariantCulture, "/_file/patrol/img/{0}/{1}", patrolId, savedName),
                    FileSize = (int)(new FileInfo(targetPath).Length),
                    Caption = meta.caption ?? temp.Caption
                };

                system_patrol.InsertPatrolPhoto(photo, accountId);
                processedFileNames.Add(meta.fileName);
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
