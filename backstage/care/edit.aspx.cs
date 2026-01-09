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
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitIdsFromSession();

                if (CurrentCareID == 0 && int.TryParse(Request.QueryString["id"], out int careId))
                {
                    CurrentCareID = careId;
                }

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
            var blocks = CollectPhotoBlocksFromRepeater();
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

            if (beforeLink != null)
            {
                bool hasBefore = !string.IsNullOrWhiteSpace(data.beforeFilePath);
                beforeLink.Visible = hasBefore;
                beforeLink.Text = string.IsNullOrWhiteSpace(data.beforeFileName) ? "已上傳" : data.beforeFileName;
                beforeLink.NavigateUrl = data.beforeFilePath;
            }

            if (afterLink != null)
            {
                bool hasAfter = !string.IsNullOrWhiteSpace(data.afterFilePath);
                afterLink.Visible = hasAfter;
                afterLink.Text = string.IsNullOrWhiteSpace(data.afterFileName) ? "已上傳" : data.afterFileName;
                afterLink.NavigateUrl = data.afterFilePath;
            }

            if (deleteCheck != null)
            {
                deleteCheck.Visible = data.photoID > 0;
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
            UpdatePhotoBlocksFromRepeater();

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
            int existingCount = CurrentCareID > 0 ? system_care.GetCarePhotos(CurrentCareID).Count : 0;
            int deletedCount = 0;
            int newUploads = 0;

            foreach (RepeaterItem item in Repeater_carePhotos.Items)
            {
                var idField = (HiddenField)item.FindControl("HiddenField_photoId");
                var deleteCheck = (CheckBox)item.FindControl("CheckBox_deletePhoto");
                var beforeUpload = (FileUpload)item.FindControl("FileUpload_beforePhoto");
                var afterUpload = (FileUpload)item.FindControl("FileUpload_afterPhoto");

                int photoId = 0;
                if (idField != null)
                {
                    int.TryParse(idField.Value, out photoId);
                }

                if (photoId > 0 && deleteCheck != null && deleteCheck.Checked)
                {
                    deletedCount++;
                    continue;
                }

                bool hasUpload = (beforeUpload != null && beforeUpload.HasFile) || (afterUpload != null && afterUpload.HasFile);
                if (photoId == 0 && hasUpload)
                {
                    newUploads++;
                }
            }

            return (existingCount - deletedCount + newUploads) > 0;
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
                var beforeUpload = (FileUpload)item.FindControl("FileUpload_beforePhoto");
                var afterUpload = (FileUpload)item.FindControl("FileUpload_afterPhoto");
                var deleteCheck = (CheckBox)item.FindControl("CheckBox_deletePhoto");

                int photoId = 0;
                if (idField != null)
                {
                    int.TryParse(idField.Value, out photoId);
                }

                string itemName = itemNameBox?.Text.Trim() ?? string.Empty;
                bool isDelete = deleteCheck != null && deleteCheck.Checked;
                bool hasBefore = beforeUpload != null && beforeUpload.HasFile;
                bool hasAfter = afterUpload != null && afterUpload.HasFile;

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

                    if (hasBefore)
                    {
                        var beforeInfo = SavePhotoFile(beforeUpload.PostedFile, uploadFolder, careId);
                        DeleteFileIfExists(origin.beforeFilePath);
                        updated.beforeFileName = beforeInfo.fileName;
                        updated.beforeFilePath = beforeInfo.filePath;
                        updated.beforeFileSize = beforeInfo.fileSize;
                    }

                    if (hasAfter)
                    {
                        var afterInfo = SavePhotoFile(afterUpload.PostedFile, uploadFolder, careId);
                        DeleteFileIfExists(origin.afterFilePath);
                        updated.afterFileName = afterInfo.fileName;
                        updated.afterFilePath = afterInfo.filePath;
                        updated.afterFileSize = afterInfo.fileSize;
                    }

                    system_care.UpdateCarePhoto(updated);
                }
                else
                {
                    if (!hasBefore && !hasAfter)
                    {
                        continue;
                    }

                    var newPhoto = new Care.Care.CarePhotoRecord
                    {
                        careID = careId,
                        itemName = NormalizeText(itemName)
                    };

                    if (hasBefore)
                    {
                        var beforeInfo = SavePhotoFile(beforeUpload.PostedFile, uploadFolder, careId);
                        newPhoto.beforeFileName = beforeInfo.fileName;
                        newPhoto.beforeFilePath = beforeInfo.filePath;
                        newPhoto.beforeFileSize = beforeInfo.fileSize;
                    }

                    if (hasAfter)
                    {
                        var afterInfo = SavePhotoFile(afterUpload.PostedFile, uploadFolder, careId);
                        newPhoto.afterFileName = afterInfo.fileName;
                        newPhoto.afterFilePath = afterInfo.filePath;
                        newPhoto.afterFileSize = afterInfo.fileSize;
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

        private (string fileName, string filePath, int fileSize) SavePhotoFile(HttpPostedFile file, string uploadFolder, int careId)
        {
            string originalName = Path.GetFileName(file.FileName);
            string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, originalName);
            string physicalPath = Path.Combine(uploadFolder, savedName);
            file.SaveAs(physicalPath);

            string virtualPath = string.Format(CultureInfo.InvariantCulture, "/_file/care/img/{0}/{1}", careId, savedName);
            return (originalName, virtualPath, file.ContentLength);
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
            }

            return blocks;
        }

        private void UpdatePhotoBlocksFromRepeater()
        {
            PhotoBlocks = CollectPhotoBlocksFromRepeater();
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
