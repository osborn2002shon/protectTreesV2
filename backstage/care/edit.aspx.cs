using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.Care;
using protectTreesV2.Log;
using protectTreesV2.TreeCatalog;
using protectTreesV2.User;

namespace protectTreesV2.backstage.care
{
    public partial class edit : BasePage
    {
        private readonly Care.Care system_care = new Care.Care();

        [Serializable]
        private class CarePhotoViewModel
        {
            public int photoID { get; set; }
            public string itemName { get; set; }
            public string beforeFileName { get; set; }
            public string beforeFilePath { get; set; }
            public string afterFileName { get; set; }
            public string afterFilePath { get; set; }
            public string beforeTempKey { get; set; }
            public string beforeTempFileName { get; set; }
            public string beforeTempFilePath { get; set; }
            public bool beforeDelete { get; set; }
            public string afterTempKey { get; set; }
            public string afterTempFileName { get; set; }
            public string afterTempFilePath { get; set; }
            public bool afterDelete { get; set; }
        }

        [Serializable]
        private class TempUploadInfo
        {
            public string Key { get; set; }
            public string FileName { get; set; }
            public string PhysicalPath { get; set; }
            public string VirtualPath { get; set; }
        }

        private const string PendingUploadSessionKey = "CarePendingUploads";

        public int CurrentCareID
        {
            get { return (int)(ViewState["CurrentCareID"] ?? 0); }
            set { ViewState["CurrentCareID"] = value; }
        }

        public int CurrentTreeID
        {
            get { return (int)(ViewState["CurrentTreeID"] ?? 0); }
            set { ViewState["CurrentTreeID"] = value; }
        }

        private string Action => CurrentCareID > 0 ? "edit" : "add";

        private List<CarePhotoViewModel> PhotoBlocks
        {
            get { return ViewState["CarePhotoBlocks"] as List<CarePhotoViewModel> ?? new List<CarePhotoViewModel>(); }
            set { ViewState["CarePhotoBlocks"] = value; }
        }

        private Dictionary<string, TempUploadInfo> GetPendingUploads()
        {
            var pending = Session[PendingUploadSessionKey] as Dictionary<string, TempUploadInfo>;
            if (pending == null)
            {
                pending = new Dictionary<string, TempUploadInfo>();
                Session[PendingUploadSessionKey] = pending;
            }
            return pending;
        }

        private string GetTempFolderPath()
        {
            return Server.MapPath(string.Format(CultureInfo.InvariantCulture, "~/_file/care/temp/{0}/", Session.SessionID));
        }

        private TempUploadInfo GetTempUpload(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            var pending = GetPendingUploads();
            return pending.ContainsKey(key) ? pending[key] : null;
        }

        private void RemoveTempUpload(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            var pending = GetPendingUploads();
            if (!pending.ContainsKey(key)) return;

            var temp = pending[key];
            try
            {
                if (!string.IsNullOrEmpty(temp.PhysicalPath) && File.Exists(temp.PhysicalPath))
                {
                    File.Delete(temp.PhysicalPath);
                }
            }
            catch
            {
            }

            pending.Remove(key);
        }

        private string SaveTempUpload(HttpPostedFile file, string existingKey)
        {
            if (file == null || file.ContentLength == 0) return existingKey;
            if (file.ContentLength > 10 * 1024 * 1024) return existingKey;

            string key = string.IsNullOrWhiteSpace(existingKey) ? Guid.NewGuid().ToString("N") : existingKey;
            var pending = GetPendingUploads();
            if (pending.ContainsKey(key))
            {
                RemoveTempUpload(key);
            }

            string originalName = Path.GetFileName(file.FileName);
            string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, originalName);
            string tempFolder = GetTempFolderPath();
            Directory.CreateDirectory(tempFolder);

            string physicalPath = Path.Combine(tempFolder, savedName);
            file.SaveAs(physicalPath);
            string virtualPath = string.Format(CultureInfo.InvariantCulture, "/_file/care/temp/{0}/{1}", Session.SessionID, savedName);

            pending[key] = new TempUploadInfo
            {
                Key = key,
                FileName = originalName,
                PhysicalPath = physicalPath,
                VirtualPath = virtualPath
            };

            return key;
        }

        private void ClearPendingUploads()
        {
            var pending = Session[PendingUploadSessionKey] as Dictionary<string, TempUploadInfo>;
            if (pending != null)
            {
                foreach (var temp in pending.Values)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(temp.PhysicalPath) && File.Exists(temp.PhysicalPath))
                        {
                            File.Delete(temp.PhysicalPath);
                        }
                    }
                    catch
                    {
                    }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitIdsFromSession();

                if (CurrentCareID == 0 && CurrentTreeID == 0)
                {
                    ShowMessage("錯誤", "無效的參數，請由列表頁進入。");
                    Response.Redirect("list.aspx");
                    return;
                }

                LoadRecord();
                SetPageTitle();

                if (CurrentTreeID <= 0)
                {
                    ShowMessage("錯誤", "無法取得樹籍資料");
                    Response.Redirect("list.aspx");
                    return;
                }

                BindTreeInfo();
                BindCarePhotoBlocks();

                Literal_btnSaveText.Text = Action == "add" ? "新增" : "儲存";
                HiddenField_treeId.Value = CurrentTreeID.ToString();
                HiddenField_careId.Value = CurrentCareID.ToString();
            }
        }

        protected void Button_addCarePhotoBlock_Click(object sender, EventArgs e)
        {
            SavePendingUploadsFromRepeater();
            var blocks = PhotoBlocks;
            blocks.Add(new CarePhotoViewModel());
            PhotoBlocks = blocks;
            BindCarePhotoBlocks();
        }

        private void BindCarePhotoBlocks()
        {
            var blocks = PhotoBlocks;

            if (blocks.Count == 0)
            {
                if (Action == "edit")
                {
                    var existing = system_care.GetCarePhotos(CurrentCareID)
                        .Select(p => new CarePhotoViewModel
                        {
                            photoID = p.photoID,
                            itemName = p.itemName,
                            beforeFileName = p.beforeFileName,
                            beforeFilePath = p.beforeFilePath,
                            afterFileName = p.afterFileName,
                            afterFilePath = p.afterFilePath
                        })
                        .ToList();
                    blocks.AddRange(existing);
                }

                if (blocks.Count == 0)
                {
                    blocks.Add(new CarePhotoViewModel());
                }

                PhotoBlocks = blocks;
            }

            AttachTempInfoToBlocks(blocks);
            Repeater_carePhotos.DataSource = blocks;
            Repeater_carePhotos.DataBind();
        }

        protected void Repeater_carePhotos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var data = e.Item.DataItem as CarePhotoViewModel;
            if (data == null)
            {
                return;
            }

            var beforeLink = (HyperLink)e.Item.FindControl("HyperLink_beforePhoto");
            var afterLink = (HyperLink)e.Item.FindControl("HyperLink_afterPhoto");
            var deleteCheck = (CheckBox)e.Item.FindControl("CheckBox_deletePhoto");
            var beforeImage = (Image)e.Item.FindControl("Image_beforePreview");
            var afterImage = (Image)e.Item.FindControl("Image_afterPreview");
            var beforeFileLabel = (Label)e.Item.FindControl("Label_beforeFileName");
            var afterFileLabel = (Label)e.Item.FindControl("Label_afterFileName");
            var beforeTempKey = (HiddenField)e.Item.FindControl("HiddenField_beforeTempKey");
            var afterTempKey = (HiddenField)e.Item.FindControl("HiddenField_afterTempKey");
            var beforeDeleteField = (HiddenField)e.Item.FindControl("HiddenField_beforeDelete");
            var afterDeleteField = (HiddenField)e.Item.FindControl("HiddenField_afterDelete");
            var beforeExistingPath = (HiddenField)e.Item.FindControl("HiddenField_beforeExistingPath");
            var afterExistingPath = (HiddenField)e.Item.FindControl("HiddenField_afterExistingPath");

            if (beforeLink != null)
            {
                bool hasBefore = !string.IsNullOrWhiteSpace(data.beforeFilePath) && !data.beforeDelete;
                beforeLink.Visible = hasBefore;
                beforeLink.Text = string.IsNullOrWhiteSpace(data.beforeFileName) ? "已上傳" : data.beforeFileName;
                beforeLink.NavigateUrl = data.beforeFilePath;
            }

            if (afterLink != null)
            {
                bool hasAfter = !string.IsNullOrWhiteSpace(data.afterFilePath) && !data.afterDelete;
                afterLink.Visible = hasAfter;
                afterLink.Text = string.IsNullOrWhiteSpace(data.afterFileName) ? "已上傳" : data.afterFileName;
                afterLink.NavigateUrl = data.afterFilePath;
            }

            if (deleteCheck != null)
            {
                deleteCheck.Visible = data.photoID > 0;
            }

            if (beforeTempKey != null)
            {
                beforeTempKey.Value = data.beforeTempKey ?? string.Empty;
            }

            if (afterTempKey != null)
            {
                afterTempKey.Value = data.afterTempKey ?? string.Empty;
            }

            if (beforeDeleteField != null)
            {
                beforeDeleteField.Value = data.beforeDelete ? "1" : "0";
            }

            if (afterDeleteField != null)
            {
                afterDeleteField.Value = data.afterDelete ? "1" : "0";
            }

            if (beforeExistingPath != null)
            {
                beforeExistingPath.Value = data.beforeFilePath ?? string.Empty;
            }

            if (afterExistingPath != null)
            {
                afterExistingPath.Value = data.afterFilePath ?? string.Empty;
            }

            string beforePreview = data.beforeDelete ? null : (string.IsNullOrWhiteSpace(data.beforeTempFilePath) ? data.beforeFilePath : data.beforeTempFilePath);
            string afterPreview = data.afterDelete ? null : (string.IsNullOrWhiteSpace(data.afterTempFilePath) ? data.afterFilePath : data.afterTempFilePath);

            if (beforeImage != null)
            {
                beforeImage.ImageUrl = beforePreview ?? string.Empty;
                beforeImage.Visible = true;
            }

            if (afterImage != null)
            {
                afterImage.ImageUrl = afterPreview ?? string.Empty;
                afterImage.Visible = true;
            }

            if (beforeFileLabel != null)
            {
                var name = data.beforeDelete ? string.Empty : (!string.IsNullOrWhiteSpace(data.beforeTempFileName) ? data.beforeTempFileName : data.beforeFileName);
                beforeFileLabel.Text = name ?? string.Empty;
            }

            if (afterFileLabel != null)
            {
                var name = data.afterDelete ? string.Empty : (!string.IsNullOrWhiteSpace(data.afterTempFileName) ? data.afterTempFileName : data.afterFileName);
                afterFileLabel.Text = name ?? string.Empty;
            }
        }

        private void LoadRecord()
        {
            if (Action == "edit")
            {
                var record = system_care.GetCareRecord(CurrentCareID);
                if (record == null)
                {
                    ShowMessage("錯誤", "找不到養護資料");
                    Response.Redirect("list.aspx");
                    return;
                }

                CurrentTreeID = record.treeID;

                TextBox_careDate.Text = record.careDate?.ToString("yyyy-MM-dd");
                TextBox_recorder.Text = record.recorder;
                TextBox_reviewer.Text = record.reviewer;
                CheckBox_isFinal.Checked = record.dataStatus == (int)Care.Care.CareRecordStatus.定稿;

                RadioButtonList_crownStatus.SelectedValue = record.crownStatus?.ToString() ?? RadioButtonList_crownStatus.SelectedValue;
                CheckBox_crownSeasonal.Checked = record.crownSeasonalDormant.GetValueOrDefault();
                CheckBox_crownDeadBranch.Checked = record.crownDeadBranch.GetValueOrDefault();
                TextBox_crownDeadBranchPercent.Text = record.crownDeadBranchPercent?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
                CheckBox_crownPest.Checked = record.crownPest.GetValueOrDefault();
                CheckBox_crownForeign.Checked = record.crownForeignObject.GetValueOrDefault();
                TextBox_crownOtherNote.Text = record.crownOtherNote;

                RadioButtonList_trunkStatus.SelectedValue = record.trunkStatus?.ToString() ?? RadioButtonList_trunkStatus.SelectedValue;
                CheckBox_trunkBark.Checked = record.trunkBarkDamage.GetValueOrDefault();
                CheckBox_trunkDecay.Checked = record.trunkDecay.GetValueOrDefault();
                CheckBox_trunkTermite.Checked = record.trunkTermiteTrail.GetValueOrDefault();
                CheckBox_trunkLean.Checked = record.trunkLean.GetValueOrDefault();
                CheckBox_trunkFungus.Checked = record.trunkFungus.GetValueOrDefault();
                CheckBox_trunkGummosis.Checked = record.trunkGummosis.GetValueOrDefault();
                CheckBox_trunkVine.Checked = record.trunkVine.GetValueOrDefault();
                TextBox_trunkOtherNote.Text = record.trunkOtherNote;

                RadioButtonList_rootStatus.SelectedValue = record.rootStatus?.ToString() ?? RadioButtonList_rootStatus.SelectedValue;
                CheckBox_rootDamage.Checked = record.rootDamage.GetValueOrDefault();
                CheckBox_rootDecay.Checked = record.rootDecay.GetValueOrDefault();
                CheckBox_rootExpose.Checked = record.rootExpose.GetValueOrDefault();
                CheckBox_rootRot.Checked = record.rootRot.GetValueOrDefault();
                CheckBox_rootSucker.Checked = record.rootSucker.GetValueOrDefault();
                TextBox_rootOtherNote.Text = record.rootOtherNote;

                RadioButtonList_envStatus.SelectedValue = record.envStatus?.ToString() ?? RadioButtonList_envStatus.SelectedValue;
                CheckBox_envPitSmall.Checked = record.envPitSmall.GetValueOrDefault();
                CheckBox_envPaved.Checked = record.envPaved.GetValueOrDefault();
                CheckBox_envDebris.Checked = record.envDebris.GetValueOrDefault();
                CheckBox_envSoilCover.Checked = record.envSoilCover.GetValueOrDefault();
                CheckBox_envCompaction.Checked = record.envCompaction.GetValueOrDefault();
                CheckBox_envWater.Checked = record.envWaterlog.GetValueOrDefault();
                CheckBox_envFacility.Checked = record.envNearFacility.GetValueOrDefault();
                TextBox_envOtherNote.Text = record.envOtherNote;

                RadioButtonList_adjacentStatus.SelectedValue = record.adjacentStatus?.ToString() ?? RadioButtonList_adjacentStatus.SelectedValue;
                CheckBox_adjacentBuilding.Checked = record.adjacentBuilding.GetValueOrDefault();
                CheckBox_adjacentWire.Checked = record.adjacentWire.GetValueOrDefault();
                CheckBox_adjacentSignal.Checked = record.adjacentSignal.GetValueOrDefault();
                TextBox_adjacentOtherNote.Text = record.adjacentOtherNote;

                SetTaskStatus(RadioButton_task1None, RadioButton_task1Do, record.task1Status);
                SetTaskStatus(RadioButton_task2None, RadioButton_task2Do, record.task2Status);
                SetTaskStatus(RadioButton_task3None, RadioButton_task3Do, record.task3Status);
                SetTaskStatus(RadioButton_task4None, RadioButton_task4Do, record.task4Status);
                SetTaskStatus(RadioButton_task5None, RadioButton_task5Do, record.task5Status);

                TextBox_task1Note.Text = record.task1Note;
                TextBox_task2Note.Text = record.task2Note;
                TextBox_task3Note.Text = record.task3Note;
                TextBox_task4Note.Text = record.task4Note;
                TextBox_task5Note.Text = record.task5Note;

                Label_recordStatus.CssClass = record.dataStatus == (int)Care.Care.CareRecordStatus.定稿 ? "badge bg-success" : "badge bg-warning text-dark";
                Label_recordStatus.Text = record.dataStatus == (int)Care.Care.CareRecordStatus.定稿 ? "定稿" : "草稿";

                BindLogs(CurrentCareID);
            }
            else
            {
                Label_recordStatus.CssClass = "badge bg-secondary";
                Label_recordStatus.Text = "新增";
            }
        }

        private void SetTaskStatus(RadioButton noneRadio, RadioButton doRadio, int? status)
        {
            if (status.GetValueOrDefault() == 1)
            {
                doRadio.Checked = true;
            }
            else
            {
                noneRadio.Checked = true;
            }
        }

        private void SetPageTitle()
        {
            if (Action == "add")
            {
                Literal_pathAction.Text = "新增";
                Literal_title.Text = "養護紀錄新增";
                Label_recordStatus.CssClass = "badge bg-secondary";
                Label_recordStatus.Text = "新增";
            }
            else
            {
                Literal_pathAction.Text = "編輯";
                Literal_title.Text = "養護紀錄編輯";
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

        private void InitIdsFromSession()
        {
            base.KeepState();

            if (!string.IsNullOrEmpty(this.setCareID))
            {
                if (int.TryParse(this.setCareID, out int careId))
                {
                    CurrentCareID = careId;
                }
                this.setCareID = null;
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

        protected void LinkButton_save_Click(object sender, EventArgs e)
        {
            SavePendingUploadsFromRepeater();

            bool isFinal = CheckBox_isFinal.Checked;

            if (!ValidateForm(out DateTime? careDate, isFinal))
            {
                BindCarePhotoBlocks();
                return;
            }

            var record = Action == "edit"
                ? system_care.GetCareRecord(CurrentCareID) ?? new Care.Care.CareRecord()
                : new Care.Care.CareRecord();
            bool isNew = record.careID == 0;

            record.treeID = CurrentTreeID;
            record.careDate = careDate;
            record.recorder = TextBox_recorder.Text.Trim();
            record.reviewer = TextBox_reviewer.Text.Trim();
            record.dataStatus = isFinal ? (int)Care.Care.CareRecordStatus.定稿 : (int)Care.Care.CareRecordStatus.草稿;

            record.crownStatus = ParseNullableInt(RadioButtonList_crownStatus.SelectedValue);
            record.crownSeasonalDormant = CheckBox_crownSeasonal.Checked;
            record.crownDeadBranch = CheckBox_crownDeadBranch.Checked;
            record.crownDeadBranchPercent = ParseNullableDecimal(TextBox_crownDeadBranchPercent.Text);
            record.crownPest = CheckBox_crownPest.Checked;
            record.crownForeignObject = CheckBox_crownForeign.Checked;
            record.crownOtherNote = NormalizeText(TextBox_crownOtherNote.Text);

            record.trunkStatus = ParseNullableInt(RadioButtonList_trunkStatus.SelectedValue);
            record.trunkBarkDamage = CheckBox_trunkBark.Checked;
            record.trunkDecay = CheckBox_trunkDecay.Checked;
            record.trunkTermiteTrail = CheckBox_trunkTermite.Checked;
            record.trunkLean = CheckBox_trunkLean.Checked;
            record.trunkFungus = CheckBox_trunkFungus.Checked;
            record.trunkGummosis = CheckBox_trunkGummosis.Checked;
            record.trunkVine = CheckBox_trunkVine.Checked;
            record.trunkOtherNote = NormalizeText(TextBox_trunkOtherNote.Text);

            record.rootStatus = ParseNullableInt(RadioButtonList_rootStatus.SelectedValue);
            record.rootDamage = CheckBox_rootDamage.Checked;
            record.rootDecay = CheckBox_rootDecay.Checked;
            record.rootExpose = CheckBox_rootExpose.Checked;
            record.rootRot = CheckBox_rootRot.Checked;
            record.rootSucker = CheckBox_rootSucker.Checked;
            record.rootOtherNote = NormalizeText(TextBox_rootOtherNote.Text);

            record.envStatus = ParseNullableInt(RadioButtonList_envStatus.SelectedValue);
            record.envPitSmall = CheckBox_envPitSmall.Checked;
            record.envPaved = CheckBox_envPaved.Checked;
            record.envDebris = CheckBox_envDebris.Checked;
            record.envSoilCover = CheckBox_envSoilCover.Checked;
            record.envCompaction = CheckBox_envCompaction.Checked;
            record.envWaterlog = CheckBox_envWater.Checked;
            record.envNearFacility = CheckBox_envFacility.Checked;
            record.envOtherNote = NormalizeText(TextBox_envOtherNote.Text);

            record.adjacentStatus = ParseNullableInt(RadioButtonList_adjacentStatus.SelectedValue);
            record.adjacentBuilding = CheckBox_adjacentBuilding.Checked;
            record.adjacentWire = CheckBox_adjacentWire.Checked;
            record.adjacentSignal = CheckBox_adjacentSignal.Checked;
            record.adjacentOtherNote = NormalizeText(TextBox_adjacentOtherNote.Text);

            record.task1Status = RadioButton_task1Do.Checked ? 1 : 0;
            record.task2Status = RadioButton_task2Do.Checked ? 1 : 0;
            record.task3Status = RadioButton_task3Do.Checked ? 1 : 0;
            record.task4Status = RadioButton_task4Do.Checked ? 1 : 0;
            record.task5Status = RadioButton_task5Do.Checked ? 1 : 0;
            record.task1Note = NormalizeText(TextBox_task1Note.Text);
            record.task2Note = NormalizeText(TextBox_task2Note.Text);
            record.task3Note = NormalizeText(TextBox_task3Note.Text);
            record.task4Note = NormalizeText(TextBox_task4Note.Text);
            record.task5Note = NormalizeText(TextBox_task5Note.Text);

            var user = UserService.GetCurrentUser();
            int accountId = user?.userID ?? 0;

            if (record.careID > 0)
            {
                system_care.UpdateCareRecord(record, accountId);
            }
            else
            {
                int newId = system_care.InsertCareRecord(record, accountId);
                CurrentCareID = newId;
                record.careID = newId;
                HiddenField_careId.Value = newId.ToString();
            }

            if (!ProcessCarePhotos(record.careID, accountId, isFinal))
            {
                BindCarePhotoBlocks();
                return;
            }

            ClearPendingUploads();

            var tree = TreeService.GetTree(CurrentTreeID);
            string actionText = isNew ? "新增" : "編輯";
            string logMemo = isNew ? "新增養護" : "編輯養護";
            OperationLogger.InsertLog("養護管理", actionText, logMemo);
            FunctionLogService.InsertLog(LogFunctionTypes.Care,
                record.careID,
                logMemo,
                $"系統樹籍編號：{tree?.SystemTreeNo ?? "無"}，養護日期：{record.careDate:yyyy-MM-dd}，狀態：{(record.dataStatus == (int)Care.Care.CareRecordStatus.定稿 ? "定稿" : "草稿")}",
                Request?.UserHostAddress,
                user?.userID,
                user?.account,
                user?.name,
                user?.unit);

            base.ReturnState();
        }

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {
            ClearPendingUploads();
            base.ReturnState();
        }

        private bool ValidateForm(out DateTime? careDate, bool isFinal)
        {
            careDate = null;
            var missing = new List<string>();

            if (!string.IsNullOrWhiteSpace(TextBox_careDate.Text))
            {
                if (DateTime.TryParse(TextBox_careDate.Text, out DateTime parsed))
                {
                    careDate = parsed;
                }
                else
                {
                    missing.Add("養護日期格式不正確");
                }
            }
            else
            {
                missing.Add("養護日期");
            }

            if (string.IsNullOrWhiteSpace(TextBox_recorder.Text) && isFinal)
            {
                missing.Add("記錄人員");
            }

            if (missing.Any())
            {
                ShowMessage("驗證", $"請填寫：{string.Join("、", missing)}", "warning");
                return false;
            }

            if (HasOversizedPhoto())
            {
                ShowMessage("限制", "單張照片大小不可超過 10MB", "warning");
                return false;
            }

            if (isFinal && !HasAnyPhotoAfterChange())
            {
                ShowMessage("限制", "定稿至少需要一張養護照片", "warning");
                return false;
            }

            return true;
        }

        private bool HasOversizedPhoto()
        {
            foreach (RepeaterItem item in Repeater_carePhotos.Items)
            {
                var beforeUpload = (FileUpload)item.FindControl("FileUpload_beforePhoto");
                var afterUpload = (FileUpload)item.FindControl("FileUpload_afterPhoto");

                if (beforeUpload != null && beforeUpload.HasFile && beforeUpload.PostedFile.ContentLength > 10 * 1024 * 1024)
                {
                    return true;
                }

                if (afterUpload != null && afterUpload.HasFile && afterUpload.PostedFile.ContentLength > 10 * 1024 * 1024)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAnyPhotoAfterChange()
        {
            int totalPhotos = 0;

            foreach (RepeaterItem item in Repeater_carePhotos.Items)
            {
                var idField = (HiddenField)item.FindControl("HiddenField_photoId");
                var deleteCheck = (CheckBox)item.FindControl("CheckBox_deletePhoto");
                var beforeUpload = (FileUpload)item.FindControl("FileUpload_beforePhoto");
                var afterUpload = (FileUpload)item.FindControl("FileUpload_afterPhoto");
                var beforeTempKey = (HiddenField)item.FindControl("HiddenField_beforeTempKey");
                var afterTempKey = (HiddenField)item.FindControl("HiddenField_afterTempKey");
                var beforeDeleteField = (HiddenField)item.FindControl("HiddenField_beforeDelete");
                var afterDeleteField = (HiddenField)item.FindControl("HiddenField_afterDelete");
                var beforeExistingPath = (HiddenField)item.FindControl("HiddenField_beforeExistingPath");
                var afterExistingPath = (HiddenField)item.FindControl("HiddenField_afterExistingPath");

                int photoId = 0;
                if (idField != null)
                {
                    int.TryParse(idField.Value, out photoId);
                }

                if (photoId > 0 && deleteCheck != null && deleteCheck.Checked)
                {
                    continue;
                }

                bool beforeDelete = beforeDeleteField != null && beforeDeleteField.Value == "1";
                bool afterDelete = afterDeleteField != null && afterDeleteField.Value == "1";
                bool hasBeforeTemp = beforeTempKey != null && !string.IsNullOrWhiteSpace(beforeTempKey.Value);
                bool hasAfterTemp = afterTempKey != null && !string.IsNullOrWhiteSpace(afterTempKey.Value);
                bool hasBeforeUpload = beforeUpload != null && beforeUpload.HasFile;
                bool hasAfterUpload = afterUpload != null && afterUpload.HasFile;
                bool hasBeforeExisting = beforeExistingPath != null && !string.IsNullOrWhiteSpace(beforeExistingPath.Value);
                bool hasAfterExisting = afterExistingPath != null && !string.IsNullOrWhiteSpace(afterExistingPath.Value);

                bool beforeExists = !beforeDelete && (hasBeforeUpload || hasBeforeTemp || hasBeforeExisting);
                bool afterExists = !afterDelete && (hasAfterUpload || hasAfterTemp || hasAfterExisting);

                if (beforeExists) totalPhotos++;
                if (afterExists) totalPhotos++;
            }

            return totalPhotos > 0;
        }

        private bool ProcessCarePhotos(int careId, int accountId, bool isFinal)
        {
            var existing = system_care.GetCarePhotos(careId);
            string uploadFolder = Server.MapPath(string.Format(CultureInfo.InvariantCulture, "~/_file/care/img/{0}/", careId));
            Directory.CreateDirectory(uploadFolder);

            foreach (RepeaterItem item in Repeater_carePhotos.Items)
            {
                var idField = (HiddenField)item.FindControl("HiddenField_photoId");
                var itemNameBox = (TextBox)item.FindControl("TextBox_itemName");
                var deleteCheck = (CheckBox)item.FindControl("CheckBox_deletePhoto");
                var beforeTempKeyField = (HiddenField)item.FindControl("HiddenField_beforeTempKey");
                var afterTempKeyField = (HiddenField)item.FindControl("HiddenField_afterTempKey");
                var beforeDeleteField = (HiddenField)item.FindControl("HiddenField_beforeDelete");
                var afterDeleteField = (HiddenField)item.FindControl("HiddenField_afterDelete");

                int photoId = 0;
                if (idField != null)
                {
                    int.TryParse(idField.Value, out photoId);
                }

                string itemName = itemNameBox?.Text.Trim() ?? string.Empty;
                bool isDelete = deleteCheck != null && deleteCheck.Checked;
                bool beforeDelete = beforeDeleteField != null && beforeDeleteField.Value == "1";
                bool afterDelete = afterDeleteField != null && afterDeleteField.Value == "1";

                string beforeTempKey = beforeTempKeyField?.Value;
                string afterTempKey = afterTempKeyField?.Value;
                var beforeTemp = GetTempUpload(beforeTempKey);
                var afterTemp = GetTempUpload(afterTempKey);
                bool hasBeforeTemp = beforeTemp != null;
                bool hasAfterTemp = afterTemp != null;

                if (photoId > 0)
                {
                    var origin = existing.FirstOrDefault(p => p.photoID == photoId);
                    if (origin == null)
                    {
                        continue;
                    }

                    if (isDelete)
                    {
                        system_care.SoftDeleteCarePhoto(photoId, accountId);
                        DeleteFileIfExists(origin.beforeFilePath);
                        DeleteFileIfExists(origin.afterFilePath);
                        RemoveTempUpload(beforeTempKey);
                        RemoveTempUpload(afterTempKey);
                        continue;
                    }

                    var updated = new Care.Care.CarePhotoRecord
                    {
                        photoID = origin.photoID,
                        careID = origin.careID,
                        itemName = NormalizeText(itemName),
                        beforeFileName = origin.beforeFileName,
                        beforeFilePath = origin.beforeFilePath,
                        beforeFileSize = origin.beforeFileSize,
                        afterFileName = origin.afterFileName,
                        afterFilePath = origin.afterFilePath,
                        afterFileSize = origin.afterFileSize
                    };

                    if (hasBeforeTemp)
                    {
                        var beforeInfo = SaveTempFile(beforeTemp, uploadFolder, careId);
                        DeleteFileIfExists(origin.beforeFilePath);
                        updated.beforeFileName = beforeInfo.fileName;
                        updated.beforeFilePath = beforeInfo.filePath;
                        updated.beforeFileSize = beforeInfo.fileSize;
                        RemoveTempUpload(beforeTempKey);
                    }
                    else if (beforeDelete)
                    {
                        DeleteFileIfExists(origin.beforeFilePath);
                        updated.beforeFileName = null;
                        updated.beforeFilePath = null;
                        updated.beforeFileSize = 0;
                    }

                    if (hasAfterTemp)
                    {
                        var afterInfo = SaveTempFile(afterTemp, uploadFolder, careId);
                        DeleteFileIfExists(origin.afterFilePath);
                        updated.afterFileName = afterInfo.fileName;
                        updated.afterFilePath = afterInfo.filePath;
                        updated.afterFileSize = afterInfo.fileSize;
                        RemoveTempUpload(afterTempKey);
                    }
                    else if (afterDelete)
                    {
                        DeleteFileIfExists(origin.afterFilePath);
                        updated.afterFileName = null;
                        updated.afterFilePath = null;
                        updated.afterFileSize = 0;
                    }

                    system_care.UpdateCarePhoto(updated);
                }
                else
                {
                    if (!hasBeforeTemp && !hasAfterTemp)
                    {
                        continue;
                    }

                    var newPhoto = new Care.Care.CarePhotoRecord
                    {
                        careID = careId,
                        itemName = NormalizeText(itemName)
                    };

                    if (hasBeforeTemp)
                    {
                        var beforeInfo = SaveTempFile(beforeTemp, uploadFolder, careId);
                        newPhoto.beforeFileName = beforeInfo.fileName;
                        newPhoto.beforeFilePath = beforeInfo.filePath;
                        newPhoto.beforeFileSize = beforeInfo.fileSize;
                        RemoveTempUpload(beforeTempKey);
                    }

                    if (hasAfterTemp)
                    {
                        var afterInfo = SaveTempFile(afterTemp, uploadFolder, careId);
                        newPhoto.afterFileName = afterInfo.fileName;
                        newPhoto.afterFilePath = afterInfo.filePath;
                        newPhoto.afterFileSize = afterInfo.fileSize;
                        RemoveTempUpload(afterTempKey);
                    }

                    system_care.InsertCarePhoto(newPhoto, accountId);
                }
            }

            if (isFinal && system_care.GetCarePhotos(careId).Count == 0)
            {
                ShowMessage("限制", "定稿至少需要一張養護照片", "warning");
                return false;
            }

            return true;
        }

        private void DeleteFileIfExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            try
            {
                string physicalPath = Server.MapPath(filePath);
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch
            {
            }
        }

        private (string fileName, string filePath, int fileSize) SaveTempFile(TempUploadInfo temp, string uploadFolder, int careId)
        {
            string originalName = Path.GetFileName(temp.FileName);
            string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, originalName);
            string physicalPath = Path.Combine(uploadFolder, savedName);

            if (!string.IsNullOrWhiteSpace(temp.PhysicalPath) && File.Exists(temp.PhysicalPath))
            {
                File.Move(temp.PhysicalPath, physicalPath);
            }
            else
            {
                throw new FileNotFoundException("Temp file not found.", temp.PhysicalPath);
            }

            string virtualPath = string.Format(CultureInfo.InvariantCulture, "/_file/care/img/{0}/{1}", careId, savedName);
            int fileSize = 0;
            try
            {
                fileSize = (int)new FileInfo(physicalPath).Length;
            }
            catch
            {
            }
            return (originalName, virtualPath, fileSize);
        }

        private List<CarePhotoViewModel> CollectPhotoBlocksFromRepeater()
        {
            var blocks = PhotoBlocks;
            if (blocks.Count == 0)
            {
                blocks = new List<CarePhotoViewModel>();
            }

            for (int i = 0; i < Repeater_carePhotos.Items.Count; i++)
            {
                var item = Repeater_carePhotos.Items[i];
                var itemNameBox = (TextBox)item.FindControl("TextBox_itemName");
                var idField = (HiddenField)item.FindControl("HiddenField_photoId");
                var beforeTempKey = (HiddenField)item.FindControl("HiddenField_beforeTempKey");
                var afterTempKey = (HiddenField)item.FindControl("HiddenField_afterTempKey");
                var beforeDeleteField = (HiddenField)item.FindControl("HiddenField_beforeDelete");
                var afterDeleteField = (HiddenField)item.FindControl("HiddenField_afterDelete");

                int photoId = 0;
                if (idField != null)
                {
                    int.TryParse(idField.Value, out photoId);
                }

                string itemName = itemNameBox?.Text.Trim() ?? string.Empty;

                if (i >= blocks.Count)
                {
                    blocks.Add(new CarePhotoViewModel());
                }

                blocks[i].photoID = photoId;
                blocks[i].itemName = itemName;
                blocks[i].beforeTempKey = beforeTempKey?.Value;
                blocks[i].afterTempKey = afterTempKey?.Value;
                blocks[i].beforeDelete = beforeDeleteField != null && beforeDeleteField.Value == "1";
                blocks[i].afterDelete = afterDeleteField != null && afterDeleteField.Value == "1";
            }

            return blocks;
        }

        private void SavePendingUploadsFromRepeater()
        {
            var blocks = CollectPhotoBlocksFromRepeater();

            for (int i = 0; i < Repeater_carePhotos.Items.Count; i++)
            {
                var item = Repeater_carePhotos.Items[i];
                var beforeUpload = (FileUpload)item.FindControl("FileUpload_beforePhoto");
                var afterUpload = (FileUpload)item.FindControl("FileUpload_afterPhoto");
                var beforeDeleteField = (HiddenField)item.FindControl("HiddenField_beforeDelete");
                var afterDeleteField = (HiddenField)item.FindControl("HiddenField_afterDelete");

                if (i >= blocks.Count)
                {
                    blocks.Add(new CarePhotoViewModel());
                }

                var block = blocks[i];
                bool beforeDelete = beforeDeleteField != null && beforeDeleteField.Value == "1";
                bool afterDelete = afterDeleteField != null && afterDeleteField.Value == "1";

                if (beforeDelete)
                {
                    RemoveTempUpload(block.beforeTempKey);
                    block.beforeTempKey = null;
                    block.beforeTempFileName = null;
                    block.beforeTempFilePath = null;
                    block.beforeDelete = true;
                    if (beforeDeleteField != null)
                    {
                        beforeDeleteField.Value = "1";
                    }
                    var beforeTempKeyField = (HiddenField)item.FindControl("HiddenField_beforeTempKey");
                    if (beforeTempKeyField != null)
                    {
                        beforeTempKeyField.Value = string.Empty;
                    }
                }
                else if (beforeUpload != null && beforeUpload.HasFile)
                {
                    block.beforeTempKey = SaveTempUpload(beforeUpload.PostedFile, block.beforeTempKey);
                    block.beforeDelete = false;
                    var beforeTempKeyField = (HiddenField)item.FindControl("HiddenField_beforeTempKey");
                    if (beforeTempKeyField != null)
                    {
                        beforeTempKeyField.Value = block.beforeTempKey ?? string.Empty;
                    }
                    if (beforeDeleteField != null)
                    {
                        beforeDeleteField.Value = "0";
                    }
                }

                if (afterDelete)
                {
                    RemoveTempUpload(block.afterTempKey);
                    block.afterTempKey = null;
                    block.afterTempFileName = null;
                    block.afterTempFilePath = null;
                    block.afterDelete = true;
                    if (afterDeleteField != null)
                    {
                        afterDeleteField.Value = "1";
                    }
                    var afterTempKeyField = (HiddenField)item.FindControl("HiddenField_afterTempKey");
                    if (afterTempKeyField != null)
                    {
                        afterTempKeyField.Value = string.Empty;
                    }
                }
                else if (afterUpload != null && afterUpload.HasFile)
                {
                    block.afterTempKey = SaveTempUpload(afterUpload.PostedFile, block.afterTempKey);
                    block.afterDelete = false;
                    var afterTempKeyField = (HiddenField)item.FindControl("HiddenField_afterTempKey");
                    if (afterTempKeyField != null)
                    {
                        afterTempKeyField.Value = block.afterTempKey ?? string.Empty;
                    }
                    if (afterDeleteField != null)
                    {
                        afterDeleteField.Value = "0";
                    }
                }

                AttachTempInfo(block);
            }

            PhotoBlocks = blocks;
        }

        private void AttachTempInfoToBlocks(List<CarePhotoViewModel> blocks)
        {
            if (blocks == null) return;
            foreach (var block in blocks)
            {
                AttachTempInfo(block);
            }
        }

        private void AttachTempInfo(CarePhotoViewModel block)
        {
            if (block == null) return;

            var beforeTemp = GetTempUpload(block.beforeTempKey);
            if (beforeTemp != null)
            {
                block.beforeTempFileName = beforeTemp.FileName;
                block.beforeTempFilePath = beforeTemp.VirtualPath;
            }
            else
            {
                block.beforeTempFileName = null;
                block.beforeTempFilePath = null;
                block.beforeTempKey = string.IsNullOrWhiteSpace(block.beforeTempKey) ? null : block.beforeTempKey;
            }

            var afterTemp = GetTempUpload(block.afterTempKey);
            if (afterTemp != null)
            {
                block.afterTempFileName = afterTemp.FileName;
                block.afterTempFilePath = afterTemp.VirtualPath;
            }
            else
            {
                block.afterTempFileName = null;
                block.afterTempFilePath = null;
                block.afterTempKey = string.IsNullOrWhiteSpace(block.afterTempKey) ? null : block.afterTempKey;
            }
        }

        private static int? ParseNullableInt(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return null;
        }

        private static decimal? ParseNullableDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            if (decimal.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        private static string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private void BindLogs(int careId)
        {
            var logs = FunctionLogService.GetLogs(LogFunctionTypes.Care, careId) ?? new List<FunctionLogEntry>();
            pnlLogs.Visible = true;
            lblLogEmpty.Visible = logs.Count == 0;
            gvLogs.Visible = logs.Count > 0;

            if (logs.Count == 0)
            {
                return;
            }

            gvLogs.DataSource = logs;
            gvLogs.DataBind();
        }

        protected void gvLogs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLogs.PageIndex = e.NewPageIndex;
            if (int.TryParse(HiddenField_careId.Value, out int careId))
            {
                BindLogs(careId);
            }
        }
    }
}
