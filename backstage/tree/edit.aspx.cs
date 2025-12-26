using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.Log;
using protectTreesV2.TreeCatalog;
using protectTreesV2.User;

namespace protectTreesV2.backstage.tree
{
    public partial class edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                LoadData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (int.TryParse(hfTreeID.Value, out int treeId) && treeId > 0)
            {
                BindPhotoJson(treeId);
            }
        }

        private int LogPageIndex
        {
            get => ViewState[nameof(LogPageIndex)] as int? ?? 0;
            set => ViewState[nameof(LogPageIndex)] = value;
        }

        private const int LogsPageSize = 5;

        protected string PhotoJson { get; private set; } = "[]";

        private void BindDropdowns()
        {
            ddlCity.Items.Clear();
            ddlCity.Items.Add(new ListItem("請選擇", string.Empty));
            foreach (var city in GetCities())
            {
                ddlCity.Items.Add(city);
            }

            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("請選擇", string.Empty));

            ddlStatus.Items.Clear();
            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddlStatus.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }

            DropdownBinder.Bind_DropDownList_LandType(ref ddlSpecies, true);
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
                    hfIsFinal.Value = tree.EditStatus == TreeEditState.完稿 ? "1" : "0";
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
            var photos = TreeService.GetPhotos(treeId).Select(p => new
            {
                photoId = p.PhotoID,
                filePath = p.FilePath,
                caption = string.IsNullOrWhiteSpace(p.Caption) ? p.FileName : p.Caption,
                isCover = p.IsCover
            }).ToList();

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
            ddlArea.Items.Add(new ListItem("請選擇", string.Empty));
            if (string.IsNullOrWhiteSpace(ddlCity.SelectedValue)) return;

            using (var da = new DataAccess.MS_SQL())
            {
                const string sql = "SELECT twID, area FROM System_Taiwan WHERE cityID=@city ORDER BY area";
                var dt = da.GetDataTable(sql, new System.Data.SqlClient.SqlParameter("@city", ddlCity.SelectedValue));
                foreach (DataRow row in dt.Rows)
                {
                    ddlArea.Items.Add(new ListItem(row["area"].ToString(), row["twID"].ToString()));
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var requestedState = chkFinalConfirm.Checked ? TreeEditState.完稿 : TreeEditState.草稿;
            bool isFinalLocked = hfIsFinal.Value == "1";
            bool requiresFinalValidation = requestedState == TreeEditState.完稿 || isFinalLocked;
            bool requiresDraftMinimum = !requiresFinalValidation && requestedState == TreeEditState.草稿;
            if (!ValidateForm(requiresFinalValidation, requiresDraftMinimum))
            {
                return;
            }
            if (Save(requestedState))
            {
                Response.Redirect("query.aspx");
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
            if (record.TreeID > 0 && record.EditStatus == TreeEditState.完稿)
            {
                state = TreeEditState.完稿;
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

            if (state == TreeEditState.完稿 && string.IsNullOrWhiteSpace(record.SystemTreeNo))
            {
                record.SystemTreeNo = TreeService.GenerateSystemTreeNo(record.CityID, record.AreaID, record.AnnouncementDate ?? record.SurveyDate ?? DateTime.Today);
            }
            
            var user = UserService.GetCurrentUser();
            int accountId = user?.userID ?? 0;

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
            OperationLogger.InsertLog("樹籍管理", isNew ? "新增" : "編輯", logMemo);
            TreeService.InsertTreeLog(record.TreeID,
                logMemo,
                $"系統樹籍編號：{record.SystemTreeNo ?? "無"}，狀態：{TreeService.GetStatusText(record.Status)}，編輯狀態：{record.EditStatus}",
                Request?.UserHostAddress,
                user?.userID,
                user?.account,
                user?.name,
                user?.unit);

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
            string status = editState == TreeEditState.完稿 ? "定稿" : "草稿";
            lblEditMode.Text = $"目前為{mode}（{status}）";
        }

        private void ConfigureFinalState(TreeEditState editState)
        {
            if (editState == TreeEditState.完稿)
            {
                chkFinalConfirm.Checked = true;
                chkFinalConfirm.Enabled = false;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("query.aspx");
        }

        private bool ValidatePhotoChanges(int treeId)
        {
            var existing = treeId > 0 ? TreeService.GetPhotos(treeId) : new List<TreePhoto>();
            var deleted = ParseDeletedPhotoIds();
            int remaining = existing.Count(p => !deleted.Contains(p.PhotoID));

            var files = fuPendingPhotos.PostedFiles?
                .Cast<System.Web.HttpPostedFile>()
                .Where(f => f != null && f.ContentLength > 0)
                .ToList();
            int newCount = files?.Count ?? 0;
            string[] newKeys = (hfNewPhotoKeys.Value ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            const int maxCount = 5;
            const int maxSize = 10 * 1024 * 1024;

            if (newCount > maxCount)
            {
                ShowMessage("限制", "一次最多上傳 5 張照片", "warning");
                return false;
            }

            if (remaining + newCount > maxCount)
            {
                ShowMessage("限制", "每棵樹最多保留 5 張照片", "warning");
                return false;
            }

            if (files != null)
            {
                if (files.Count != newKeys.Length)
                {
                    ShowMessage("提示", "照片資訊不一致，請重新選擇照片", "warning");
                    return false;
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.ContentLength > maxSize)
                    {
                        ShowMessage("限制", $"{file.FileName} 超過 10MB，請重新選擇", "warning");
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ProcessPhotos(int treeId, int accountId)
        {
            var deleted = ParseDeletedPhotoIds();
            var existing = TreeService.GetPhotos(treeId);

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

            var files = fuPendingPhotos.PostedFiles;
            string[] newKeys = (hfNewPhotoKeys.Value ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string coverKey = hfCoverPhoto.Value;
            int? coverId = null;

            if (!string.IsNullOrEmpty(coverKey) && coverKey.StartsWith("existing-", StringComparison.Ordinal))
            {
                if (int.TryParse(coverKey.Replace("existing-", string.Empty), out int requestedCover) && !deleted.Contains(requestedCover))
                {
                    coverId = requestedCover;
                }
            }

            if (files != null && files.Count > 0)
            {
                string uploadFolder = Server.MapPath(string.Format(CultureInfo.InvariantCulture, "~/upload/tree/{0}/", treeId));
                Directory.CreateDirectory(uploadFolder);

                int count = Math.Min(files.Count, newKeys.Length);
                for (int i = 0; i < count; i++)
                {
                    var file = files[i];
                    if (file == null || file.ContentLength == 0) continue;

                    string fileName = Path.GetFileName(file.FileName);
                    string savedName = string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddHHmmssfff}_{1}", DateTime.Now, fileName);
                    string physicalPath = Path.Combine(uploadFolder, savedName);
                    file.SaveAs(physicalPath);

                    var photo = new TreePhoto
                    {
                        TreeID = treeId,
                        FileName = fileName,
                        FilePath = string.Format(CultureInfo.InvariantCulture, "/upload/tree/{0}/{1}", treeId, savedName),
                        Caption = fileName,
                        IsCover = false
                    };

                    int photoId = TreeService.InsertPhoto(photo, accountId);
                    if (coverKey == newKeys[i])
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
            var logs = TreeService.GetTreeLogs(treeId);
            pnlLogs.Visible = true;
            lblLogEmpty.Visible = logs.Count == 0;
            rptLogs.Visible = logs.Count > 0;

            if (logs.Count == 0)
            {
                lnkLogPrev.Enabled = false;
                lnkLogNext.Enabled = false;
                lblLogPageInfo.Text = "0/0";
                return;
            }

            int totalPages = (int)Math.Ceiling(logs.Count / (double)LogsPageSize);
            LogPageIndex = Math.Max(0, Math.Min(LogPageIndex, totalPages - 1));

            var pagedLogs = logs.Skip(LogPageIndex * LogsPageSize).Take(LogsPageSize).ToList();
            rptLogs.DataSource = pagedLogs;
            rptLogs.DataBind();

            lnkLogPrev.Enabled = LogPageIndex > 0;
            lnkLogNext.Enabled = LogPageIndex < totalPages - 1;
            lblLogPageInfo.Text = string.Format("{0}/{1}", LogPageIndex + 1, totalPages);
        }

        protected void lnkLogPrev_Click(object sender, EventArgs e)
        {
            if (LogPageIndex > 0 && int.TryParse(hfTreeID.Value, out int treeId))
            {
                LogPageIndex--;
                BindLogs(treeId);
            }
        }

        protected void lnkLogNext_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfTreeID.Value, out int treeId))
            {
                LogPageIndex++;
                BindLogs(treeId);
            }
        }
    }
}
