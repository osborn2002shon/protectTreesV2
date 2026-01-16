using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class edit : BasePage
    {
        [Serializable]
        private class TempUploadInfo
        {
            public string Key { get; set; }
            public string FileName { get; set; }
            public string VirtualPath { get; set; }
            public string PhysicalPath { get; set; }
            public int ContentLength { get; set; }
        }

        private const int MaxPhotoCount = 5;
        private const int MaxPhotoSize = 10 * 1024 * 1024;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                HandlePendingUploads();
            }
            else
            {
                base.KeepState();
                ClearPendingUploads();
                BindDropdowns();
                LoadData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            int.TryParse(hfTreeID.Value, out int treeId);
            BindPhotoJson(treeId);
        }

        protected string PhotoJson { get; private set; } = "[]";

        private void BindDropdowns()
        {
            ddlCity.Items.Clear();
            Base.DropdownBinder.Bind_DropDownList_City(ref ddlCity, false);
            ddlCity.Items.Insert(0, new ListItem("請選擇", string.Empty));

            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("請選擇", string.Empty));

            ddlStatus.Items.Clear();
            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddlStatus.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }

            DropdownBinder.Bind_DropDownList_Species(ref ddlSpecies);
            DropdownBinder.Bind_DropDownList_LandType(ref ddlLandOwnership);

            cblRecognition.Items.Clear();
            foreach (var item in TreeService.GetRecognitionCriteria())
            {
                cblRecognition.Items.Add(new ListItem(item.Name, item.Code));
            }
        }

        private void LoadData()
        {
            int id;
            if (int.TryParse(setTreeID, out id) && id > 0)
            {
                var tree = TreeService.GetTree(id);
                if (tree != null)
                {
                    hfTreeID.Value = tree.TreeID.ToString();
                    SetSystemTreeNo(tree.SystemTreeNo);
                    txtAgencyTreeNo.Text = tree.AgencyTreeNo;
                    txtJurisdiction.Text = tree.AgencyJurisdictionCode;
                    SelectDropDown(ddlCity, tree.CityID);
                    BindAreas();
                    SelectDropDown(ddlArea, tree.AreaID);
                    SelectDropDown(ddlSpecies, tree.SpeciesID);
                    SelectDropDown(ddlStatus, (int)tree.Status);
                    txtSurveyDate.Text = tree.SurveyDate?.ToString("yyyy-MM-dd");
                    txtSurveyor.Text = tree.Surveyor;
                    txtAnnouncementDate.Text = tree.AnnouncementDate?.ToString("yyyy-MM-dd");
                    txtTreeCount.Text = tree.TreeCount.ToString();
                    txtLatitude.Text = tree.Latitude?.ToString();
                    txtLongitude.Text = tree.Longitude?.ToString();
                    txtSite.Text = tree.Site;
                    txtManager.Text = tree.Manager;
                    txtManagerContact.Text = tree.ManagerContact;
                    SelectDropDownText(ddlLandOwnership, tree.LandOwnership);
                    txtLandOwnershipNote.Text = tree.LandOwnershipNote;
                    txtFacility.Text = tree.FacilityDescription;
                    txtMemo.Text = tree.Memo;
                    txtTreeHeight.Text = tree.TreeHeight?.ToString();
                    txtBreastHeightDiameter.Text = tree.BreastHeightDiameter?.ToString();
                    txtBreastHeightCircumference.Text = tree.BreastHeightCircumference?.ToString();
                    txtCanopyArea.Text = tree.CanopyProjectionArea?.ToString();
                    txtRecognitionNote.Text = tree.RecognitionNote;
                    txtCulturalHistory.Text = tree.CulturalHistoryIntro;
                    txtHealth.Text = tree.HealthCondition;
                    txtEstimatedPlantingYear.Text = tree.EstimatedPlantingYear;
                    txtEstimatedAgeNote.Text = tree.EstimatedAgeNote;
                    txtGroupGrowthInfo.Text = tree.GroupGrowthInfo;
                    txtEpiphyteDescription.Text = tree.EpiphyteDescription;
                    txtParasiteDescription.Text = tree.ParasiteDescription;
                    txtClimbingPlantDescription.Text = tree.ClimbingPlantDescription;
                    foreach (ListItem item in cblRecognition.Items)
                    {
                        item.Selected = tree.RecognitionCriteria.Contains(item.Value);
                    }

                    BindPhotoJson(tree.TreeID);
                    BindLogs(tree.TreeID);
                    SetEditModeLabel(isNew: false, tree.EditStatus);
                    ConfigureFinalState(tree.EditStatus);
                    hfIsFinal.Value = tree.EditStatus == TreeEditState.定稿 ? "1" : "0";
                }
            }
            else
            {
                SetSystemTreeNo(null);
                SetEditModeLabel(isNew: true, TreeEditState.草稿);
                ConfigureFinalState(TreeEditState.草稿);
                hfIsFinal.Value = "0";
            }
        }

        private void BindPhotoJson(int treeId)
        {
            var photos = new List<object>();
            if (treeId > 0)
            {
                photos.AddRange(TreeService.GetPhotos(treeId).Select(p => new
                {
                    photoId = p.PhotoID,
                    filePath = p.FilePath,
                    caption = string.IsNullOrWhiteSpace(p.Caption) ? p.FileName : p.Caption,
                    isCover = p.IsCover
                }));
            }

            foreach (var temp in GetPendingUploads())
            {
                if (!File.Exists(temp.PhysicalPath) && string.IsNullOrWhiteSpace(temp.VirtualPath)) continue;

                photos.Add(new
                {
                    photoId = 0,
                    key = temp.Key,
                    filePath = temp.VirtualPath,
                    fileName = temp.FileName,
                    caption = temp.FileName,
                    isCover = false,
                    isTemp = true
                });
            }

            PhotoJson = new JavaScriptSerializer().Serialize(photos);
        }

        private void SelectDropDown(DropDownList ddl, int? value)
        {
            if (!value.HasValue) return;
            var item = ddl.Items.FindByValue(value.Value.ToString());
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        private void SelectDropDownText(DropDownList ddl, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var item = ddl.Items.Cast<ListItem>().FirstOrDefault(i => i.Text == text);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAreas();
        }

        private void BindAreas()
        {
            ddlArea.Items.Clear();

            if (!string.IsNullOrWhiteSpace(ddlCity.SelectedValue)) { 
                Base.DropdownBinder.Bind_DropDownList_Area(ref ddlArea, ddlCity.SelectedValue, false);
            }
            ddlArea.Items.Insert(0, new ListItem("請選擇", string.Empty));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var requestedState = chkFinalConfirm.Checked ? TreeEditState.定稿 : TreeEditState.草稿;
            bool isFinalLocked = hfIsFinal.Value == "1";
            bool requiresFinalValidation = requestedState == TreeEditState.定稿 || isFinalLocked;
            bool requiresDraftMinimum = !requiresFinalValidation && requestedState == TreeEditState.草稿;
            if (!ValidateForm(requiresFinalValidation, requiresDraftMinimum))
            {
                return;
            }
            if (Save(requestedState))
            {
                base.ReturnState();
            }
        }

        private bool ValidateForm(bool requiresFinalFields, bool requiresDraftMinimum)
        {
            var missingFields = new List<string>();

            if (requiresDraftMinimum || requiresFinalFields)
            {
                if (string.IsNullOrWhiteSpace(ddlCity.SelectedValue))
                {
                    missingFields.Add("縣市");
                }
                if (string.IsNullOrWhiteSpace(ddlSpecies.SelectedValue))
                {
                    missingFields.Add("樹種及學名");
                }
            }

            if (requiresFinalFields)
            {
                if (string.IsNullOrWhiteSpace(ddlArea.SelectedValue))
                {
                    missingFields.Add("鄉鎮區");
                }
                if (!int.TryParse(txtTreeCount.Text, out int treeCount) || treeCount <= 0)
                {
                    missingFields.Add("數量（需大於 0）");
                }
                if (string.IsNullOrWhiteSpace(txtSite.Text))
                {
                    missingFields.Add("坐落地點");
                }
                if (!decimal.TryParse(txtLatitude.Text, out _) || !decimal.TryParse(txtLongitude.Text, out _))
                {
                    missingFields.Add("有效的座標（緯度與經度）");
                }
                if (string.IsNullOrWhiteSpace(txtManager.Text))
                {
                    missingFields.Add("管理人");
                }
                if (string.IsNullOrWhiteSpace(ddlStatus.SelectedValue))
                {
                    missingFields.Add("樹籍狀態");
                }
                if (ddlStatus.SelectedValue == ((int)TreeStatus.已公告列管).ToString())
                {
                    if (string.IsNullOrWhiteSpace(txtAnnouncementDate.Text))
                    {
                        missingFields.Add("公告日期");
                    }
                    if (!cblRecognition.Items.Cast<ListItem>().Any(i => i.Selected))
                    {
                        missingFields.Add("至少一項受保護認定理由");
                    }
                }
                if (string.IsNullOrWhiteSpace(txtCulturalHistory.Text))
                {
                    missingFields.Add("文化歷史價值介紹");
                }
                if (string.IsNullOrWhiteSpace(txtSurveyDate.Text))
                {
                    missingFields.Add("調查日期");
                }
            }

            if (missingFields.Any())
            {
                ShowMessage("驗證", $"請補齊以下欄位：{string.Join("、", missingFields)}", "warning");
                return false;
            }
            return true;
        }

        private bool Save(TreeEditState state)
        {
            int.TryParse(hfTreeID.Value, out int treeId);
            if (!ValidatePhotoChanges(treeId))
            {
                return false;
            }
            bool isNew = treeId <= 0;
            var record = treeId > 0 ? TreeService.GetTree(treeId) ?? new TreeRecord() : new TreeRecord();

            record.TreeID = treeId;
            record.AgencyTreeNo = txtAgencyTreeNo.Text.Trim();
            record.AgencyJurisdictionCode = txtJurisdiction.Text.Trim();
            record.CityID = string.IsNullOrWhiteSpace(ddlCity.SelectedValue) ? (int?)null : Convert.ToInt32(ddlCity.SelectedValue);
            record.CityName = ddlCity.SelectedItem?.Text;
            record.AreaID = string.IsNullOrWhiteSpace(ddlArea.SelectedValue) ? (int?)null : Convert.ToInt32(ddlArea.SelectedValue);
            record.AreaName = ddlArea.SelectedItem?.Text;
            record.SpeciesID = string.IsNullOrWhiteSpace(ddlSpecies.SelectedValue) ? (int?)null : Convert.ToInt32(ddlSpecies.SelectedValue);
            record.SpeciesCommonName = ddlSpecies.SelectedItem?.Text;
            record.Manager = txtManager.Text.Trim();
            record.ManagerContact = txtManagerContact.Text.Trim();
            record.SurveyDate = string.IsNullOrWhiteSpace(txtSurveyDate.Text) ? (DateTime?)null : DateTime.Parse(txtSurveyDate.Text);
            record.Surveyor = txtSurveyor.Text.Trim();
            record.Status = string.IsNullOrWhiteSpace(ddlStatus.SelectedValue) ? TreeStatus.其他 : (TreeStatus)Convert.ToInt32(ddlStatus.SelectedValue);
            if (record.Status == TreeStatus.已公告列管)
            {
                record.AnnouncementDate = string.IsNullOrWhiteSpace(txtAnnouncementDate.Text) ? (DateTime?)null : DateTime.Parse(txtAnnouncementDate.Text);
            }
            else
            {
                record.AnnouncementDate = null;
            }
            record.IsAnnounced = record.Status == TreeStatus.已公告列管 && record.AnnouncementDate.HasValue;
            if (record.TreeID > 0 && record.EditStatus == TreeEditState.定稿)
            {
                state = TreeEditState.定稿;
            }
            record.EditStatus = state;
            record.TreeCount = int.TryParse(txtTreeCount.Text, out int count) && count > 0 ? count : 1;
            record.Site = txtSite.Text.Trim();
            record.Latitude = decimal.TryParse(txtLatitude.Text, out decimal lat) ? lat : (decimal?)null;
            record.Longitude = decimal.TryParse(txtLongitude.Text, out decimal lng) ? lng : (decimal?)null;
            record.LandOwnership = ddlLandOwnership.SelectedValue;
            record.LandOwnershipNote = txtLandOwnershipNote.Text.Trim();
            record.FacilityDescription = txtFacility.Text.Trim();
            record.Memo = txtMemo.Text.Trim();
            record.TreeHeight = decimal.TryParse(txtTreeHeight.Text, out decimal h) ? h : (decimal?)null;
            record.BreastHeightDiameter = decimal.TryParse(txtBreastHeightDiameter.Text, out decimal d) ? d : (decimal?)null;
            record.BreastHeightCircumference = decimal.TryParse(txtBreastHeightCircumference.Text, out decimal c) ? c : (decimal?)null;
            record.CanopyProjectionArea = decimal.TryParse(txtCanopyArea.Text, out decimal area) ? area : (decimal?)null;
            record.EstimatedPlantingYear = txtEstimatedPlantingYear.Text.Trim();
            record.EstimatedAgeNote = txtEstimatedAgeNote.Text.Trim();
            record.GroupGrowthInfo = txtGroupGrowthInfo.Text.Trim();
            record.EpiphyteDescription = txtEpiphyteDescription.Text.Trim();
            record.ParasiteDescription = txtParasiteDescription.Text.Trim();
            record.ClimbingPlantDescription = txtClimbingPlantDescription.Text.Trim();
            record.RecognitionCriteria = cblRecognition.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList();
            record.RecognitionNote = txtRecognitionNote.Text.Trim();
            record.CulturalHistoryIntro = txtCulturalHistory.Text.Trim();
            record.HealthCondition = txtHealth.Text.Trim();

            if (state == TreeEditState.定稿 && string.IsNullOrWhiteSpace(record.SystemTreeNo))
            {
                record.SystemTreeNo = TreeService.GenerateSystemTreeNo(record.CityID, record.AreaID, record.AnnouncementDate ?? record.SurveyDate ?? DateTime.Today);
            }
            
            var user = UserInfo.GetCurrentUser;
            int accountId = user?.accountID ?? 0;

            if (record.TreeID > 0)
            {
                TreeService.UpdateTree(record, accountId);
            }
            else
            {
                int newId = TreeService.InsertTree(record, accountId);
                hfTreeID.Value = newId.ToString();
                setTreeID = newId.ToString();
                record.TreeID = newId;
            }

            if (!ProcessPhotos(record.TreeID, accountId))
            {
                return false;
            }

            string logMemo = isNew ? "新增樹籍" : "編輯樹籍";
            UserLog.Insert_UserLog(accountId, UserLog.enum_UserLogItem.樹籍基本資料管理, isNew ? UserLog.enum_UserLogType.新增 : UserLog.enum_UserLogType.修改, logMemo);
            TreeLog.InsertLog(TreeLog.LogFunctionTypes.TreeCatalog,
                record.TreeID,
                logMemo,
                $"系統樹籍編號：{record.SystemTreeNo ?? "無"}，狀態：{TreeService.GetStatusText(record.Status)}，編輯狀態：{record.EditStatus}",
                Request?.UserHostAddress,
                user?.accountID,
                user?.account,
                user?.name,
                user?.unitName);

            return true;
        }

        private void SetSystemTreeNo(string systemTreeNo)
        {
            if (string.IsNullOrWhiteSpace(systemTreeNo))
            {
                lblTopSystemTreeNo.Visible = false;
                lblTopSystemTreeNo.Text = string.Empty;
                return;
            }

            lblTopSystemTreeNo.Visible = true;
            lblTopSystemTreeNo.Text = $"系統樹籍編號：{systemTreeNo}";
        }

        private void SetEditModeLabel(bool isNew, TreeEditState editState)
        {
            string mode = isNew ? "新增" : "編輯";
            string status = editState == TreeEditState.定稿 ? "定稿" : "草稿";
            lblEditMode.Text = $"目前為{mode}（{status}）";
        }

        private void ConfigureFinalState(TreeEditState editState)
        {
            if (editState == TreeEditState.定稿)
            {
                chkFinalConfirm.Checked = true;
                chkFinalConfirm.Enabled = false;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPendingUploads();
            base.ReturnState();
        }

        private bool ValidatePhotoChanges(int treeId)
        {
            var existing = treeId > 0 ? TreeService.GetPhotos(treeId) : new List<TreePhoto>();
            var deleted = ParseDeletedPhotoIds();
            int remaining = existing.Count(p => !deleted.Contains(p.PhotoID));
            var pending = GetPendingUploads();
            var newKeys = GetNewPhotoKeys();

            if (remaining + pending.Count > MaxPhotoCount)
            {
                ShowMessage("限制", "每棵樹最多保留 5 張照片", "warning");
                return false;
            }

            if (pending.Any(p => p.ContentLength > MaxPhotoSize))
            {
                ShowMessage("限制", "單張照片大小不可超過 10MB", "warning");
                return false;
            }

            var missing = newKeys.Where(k => pending.All(p => p.Key != k)).ToList();
            if (missing.Any())
            {
                ShowMessage("提示", "部分照片未成功暫存，請重新選擇照片後再試一次", "warning");
                return false;
            }

            return true;
        }

        private bool ProcessPhotos(int treeId, int accountId)
        {
            var deleted = ParseDeletedPhotoIds();
            var existing = TreeService.GetPhotos(treeId);
            var pending = GetPendingUploads();

            foreach (var photoId in deleted)
            {
                var photo = existing.FirstOrDefault(p => p.PhotoID == photoId);
                if (photo != null)
                {
                    TreeService.DeletePhoto(photoId, accountId);
                    string physical = Server.MapPath(photo.FilePath);
                    if (File.Exists(physical))
                    {
                        File.Delete(physical);
                    }
                }
            }

            string coverKey = hfCoverPhoto.Value;
            int? coverId = null;

            if (!string.IsNullOrEmpty(coverKey) && coverKey.StartsWith("existing-", StringComparison.Ordinal))
            {
                if (int.TryParse(coverKey.Replace("existing-", string.Empty), out int requestedCover) && !deleted.Contains(requestedCover))
                {
                    coverId = requestedCover;
                }
            }

            if (pending.Any())
            {
                string uploadFolder = Server.MapPath(string.Format(CultureInfo.InvariantCulture, "~/_file/tree/img/{0}/", treeId));
                Directory.CreateDirectory(uploadFolder);

                foreach (var temp in pending)
                {
                    if (string.IsNullOrEmpty(temp.PhysicalPath) || !File.Exists(temp.PhysicalPath)) continue;

                    string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, temp.FileName);
                    string targetPath = Path.Combine(uploadFolder, savedName);
                    File.Move(temp.PhysicalPath, targetPath);

                    var photo = new TreePhoto
                    {
                        TreeID = treeId,
                        FileName = temp.FileName,
                        FilePath = string.Format(CultureInfo.InvariantCulture, "/_file/tree/img/{0}/{1}", treeId, savedName),
                        Caption = temp.FileName,
                        IsCover = false
                    };

                    int photoId = TreeService.InsertPhoto(photo, accountId);
                    if (coverKey == temp.Key)
                    {
                        coverId = photoId;
                    }
                }
            }

            if (!coverId.HasValue)
            {
                var available = TreeService.GetPhotos(treeId).FirstOrDefault();
                coverId = available?.PhotoID;
            }

            if (coverId.HasValue)
            {
                TreeService.SetCoverPhoto(treeId, coverId.Value, accountId);
            }

            ClearPendingUploads();
            return true;
        }

        private List<int> ParseDeletedPhotoIds()
        {
            var result = new List<int>();
            var parts = (hfDeletedPhotos.Value ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int id))
                {
                    result.Add(id);
                }
            }

            return result;
        }

        private void BindLogs(int treeId)
        {
            var logs = TreeLog.GetLogs(TreeLog.LogFunctionTypes.TreeCatalog, treeId) ?? new List<TreeLog.FunctionLogEntry>();
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
            if (int.TryParse(hfTreeID.Value, out int treeId))
            {
                BindLogs(treeId);
            }
        }

        private void HandlePendingUploads()
        {
            var newKeys = GetNewPhotoKeys();
            SyncPendingUploads(newKeys);
            SavePendingUploads(newKeys);
        }

        private List<string> GetNewPhotoKeys()
        {
            return (hfNewPhotoKeys.Value ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .ToList();
        }

        private void SavePendingUploads(List<string> newKeys)
        {
            var files = fuPendingPhotos.PostedFiles?
                .Cast<HttpPostedFile>()
                .Where(f => f != null && f.ContentLength > 0)
                .ToList();

            if (files == null || files.Count == 0) return;

            var pending = GetPendingUploads();
            string tempFolder = GetTempFolderPath();
            Directory.CreateDirectory(tempFolder);

            int fileIndex = 0;
            foreach (var key in newKeys)
            {
                if (pending.Any(p => p.Key == key)) continue;
                if (fileIndex >= files.Count) break;

                var file = files[fileIndex++];
                if (file == null || file.ContentLength == 0) continue;
                if (file.ContentLength > MaxPhotoSize) continue;

                string originalName = Path.GetFileName(file.FileName);
                string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, originalName);
                string physicalPath = Path.Combine(tempFolder, savedName);
                file.SaveAs(physicalPath);

                string virtualPath = string.Format(CultureInfo.InvariantCulture, "/_file/tree/temp/{0}/{1}", Session.SessionID, savedName);

                pending.Add(new TempUploadInfo
                {
                    Key = key,
                    FileName = originalName,
                    PhysicalPath = physicalPath,
                    VirtualPath = virtualPath,
                    ContentLength = file.ContentLength
                });
            }

            Session[PendingUploadSessionKey] = pending;
        }

        private void SyncPendingUploads(List<string> newKeys)
        {
            var pending = GetPendingUploads();
            var keep = new HashSet<string>(newKeys ?? new List<string>());

            for (int i = pending.Count - 1; i >= 0; i--)
            {
                if (!keep.Contains(pending[i].Key))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(pending[i].PhysicalPath) && File.Exists(pending[i].PhysicalPath))
                        {
                            File.Delete(pending[i].PhysicalPath);
                        }
                    }
                    catch
                    {
                    }
                    pending.RemoveAt(i);
                }
            }

            Session[PendingUploadSessionKey] = pending;
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

        private string PendingUploadSessionKey
        {
            get
            {
                if (ViewState[nameof(PendingUploadSessionKey)] == null)
                {
                    string idPart = "new";
                    if (int.TryParse(hfTreeID.Value, out int treeId) && treeId > 0)
                    {
                        idPart = treeId.ToString();
                    }
                    else if (int.TryParse(setTreeID, out int queryId) && queryId > 0)
                    {
                        idPart = queryId.ToString();
                    }

                    ViewState[nameof(PendingUploadSessionKey)] = $"TreePendingUploads_{Session.SessionID}_{idPart}";
                }

                return (string)ViewState[nameof(PendingUploadSessionKey)];
            }
        }

        private string GetTempFolderPath()
        {
            return Server.MapPath(string.Format(CultureInfo.InvariantCulture, "~/_file/tree/temp/{0}/", Session.SessionID));
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
    }
}
