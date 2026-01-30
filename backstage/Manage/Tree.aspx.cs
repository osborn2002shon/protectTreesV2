using DataAccess;
using protectTreesV2.Base;
using protectTreesV2.Log;
using protectTreesV2.TreeCatalog;
using protectTreesV2.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace protectTreesV2.backstage.Manage
{
    public partial class Tree : BasePage
    {

        private const string UploadRoot = "~/Upload/Tree";
        private const int LogPageSize = 10;
        private static readonly int MaxUploadBytes = GetMaxUploadBytes();

        private bool DetailEditable
        {
            get => ViewState["DetailEditable"] is bool value && value;
            set => ViewState["DetailEditable"] = value;
        }

        private int LogPageIndex
        {
            get => ViewState["LogPageIndex"] is int value ? value : 0;
            set => ViewState["LogPageIndex"] = value;
        }

        protected Account CurrentUser => UserService.GetCurrentUser();

        protected string PhotoUploadLimitDisplay => MaxUploadBytes > 0 ? FormatBytes(MaxUploadBytes) : "未設定";

        protected void Page_Load(object sender, EventArgs e)
        {
            ConfigurePhotoUploadControl();
            if (!IsPostBack)
            {
                InitializeControls();
                BindGrid();
                LoadTreeFromQuery();
            }
        }

        private void InitializeControls()
        {
            BindCityDropdown(ddlCity, includeAll: true);
            BindAreaDropdown(ddlArea, null, includeAll: true);

            BindCityDropdown(ddlDetailCity, includeAll: false);
            BindAreaDropdown(ddlDetailArea, null, includeAll: false);

            BindEditStatusDropdown(ddlEditStatus, includeAll: true);
            BindEditStatusDropdown(ddlDetailEditStatus, includeAll: false);

            BindTreeStatusDropdown(ddlTreeStatus, includeAll: true);
            BindTreeStatusDropdown(ddlDetailStatus, includeAll: false);
            BindTreeStatusDropdown(ddlBatchStatus, includeAll: false);

            BindSpeciesDropdown(ddlSpecies, includeAll: true);
            BindSpeciesDropdown(ddlDetailSpecies, includeAll: false);

            BindCriteriaChecklist();

            ddlTreeStatus.SelectedValue = string.Empty;
            ddlEditStatus.SelectedValue = string.Empty;
            ddlSpecies.SelectedValue = string.Empty;

            btnAdd.Visible = IsEditorAccount();
            pnlBatchStatus.Visible = IsEditorAccount();
        }

        private void ConfigurePhotoUploadControl()
        {
            if (fuPhoto == null) return;

            fuPhoto.Attributes["data-max-total-bytes"] = MaxUploadBytes.ToString(CultureInfo.InvariantCulture);
            fuPhoto.Attributes["data-max-total-display"] = PhotoUploadLimitDisplay;
        }

        private void BindCriteriaChecklist()
        {
            var criteria = TreeService.GetRecognitionCriteria();
            cblCriteria.DataSource = criteria;
            cblCriteria.DataValueField = nameof(RecognitionCriterion.Code);
            cblCriteria.DataTextField = nameof(RecognitionCriterion.Name);
            cblCriteria.DataBind();
        }

        private void LoadTreeFromQuery()
        {
            if (!int.TryParse(Request.QueryString["treeId"], out int treeId) || treeId <= 0)
            {
                return;
            }

            var record = TreeService.GetTree(treeId);
            if (record == null)
            {
                ShowMessage("提示", "找不到指定樹籍資料", "warning");
                return;
            }

            if (IsEditorAccount())
            {
                LoadTree(treeId);
            }
            else
            {
                Response.Redirect("TreeView.aspx?treeId=" + treeId, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private void BindSpeciesDropdown(DropDownList ddl, bool includeAll)
        {
            ddl.Items.Clear();
            if (includeAll)
            {
                ddl.Items.Add(new ListItem("*不拘", string.Empty));
            }
            else
            {
                ddl.Items.Add(new ListItem("請選擇", string.Empty));
            }

            foreach (var species in TreeService.GetSpecies())
            {
                ddl.Items.Add(new ListItem(species.DisplayName, species.SpeciesID.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private void BindTreeStatusDropdown(DropDownList ddl, bool includeAll)
        {
            ddl.Items.Clear();
            if (includeAll)
            {
                ddl.Items.Add(new ListItem("*不拘", string.Empty));
            }

            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddl.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }
        }

        private void BindEditStatusDropdown(DropDownList ddl, bool includeAll)
        {
            ddl.Items.Clear();
            if (includeAll)
            {
                ddl.Items.Add(new ListItem("*不拘", string.Empty));
            }
            foreach (TreeEditState state in Enum.GetValues(typeof(TreeEditState)))
            {
                ddl.Items.Add(new ListItem(TreeService.GetEditStatusText(state), ((int)state).ToString()));
            }
        }

        private void BindCityDropdown(DropDownList ddl, bool includeAll)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem(includeAll ? "*不拘" : "請選擇", string.Empty));
            foreach (var item in GetCities())
            {
                ddl.Items.Add(new ListItem(item.Text, item.Value));
            }
        }

        private void BindAreaDropdown(DropDownList ddl, int? cityId, bool includeAll)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem(includeAll ? "*不拘" : "請選擇", string.Empty));
            if (!cityId.HasValue) return;

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable("SELECT twID, area FROM System_Taiwan WHERE cityID=@city ORDER BY area",
                    new SqlParameter("@city", cityId.Value));
                foreach (DataRow row in dt.Rows)
                {
                    ddl.Items.Add(new ListItem(row["area"].ToString(), row["twID"].ToString()));
                }
            }
        }

        private TreeFilter BuildFilter()
        {
            var filter = new TreeFilter
            {
                CityID = ParseNullableInt(ddlCity.SelectedValue),
                AreaID = ParseNullableInt(ddlArea.SelectedValue),
                SpeciesID = ParseNullableInt(ddlSpecies.SelectedValue),
                Keyword = string.IsNullOrWhiteSpace(txtKeyword.Text) ? null : txtKeyword.Text.Trim()
            };

            if (int.TryParse(ddlEditStatus.SelectedValue, out int editState))
            {
                filter.EditStatus = (TreeEditState)editState;
            }

            if (int.TryParse(ddlTreeStatus.SelectedValue, out int status))
            {
                filter.Status = (TreeStatus)status;
            }

            if (DateTime.TryParse(txtSurveyStart.Text, out DateTime surveyStart))
            {
                filter.SurveyDateStart = surveyStart;
            }

            if (DateTime.TryParse(txtSurveyEnd.Text, out DateTime surveyEnd))
            {
                filter.SurveyDateEnd = surveyEnd;
            }

            if (DateTime.TryParse(txtAnnouncementStart.Text, out DateTime announceStart))
            {
                filter.AnnouncementDateStart = announceStart;
            }

            if (DateTime.TryParse(txtAnnouncementEnd.Text, out DateTime announceEnd))
            {
                filter.AnnouncementDateEnd = announceEnd;
            }

            return filter;
        }

        private void BindGrid()
        {
            var records = TreeService.SearchTrees(BuildFilter());
            gvTrees.DataSource = records;
            gvTrees.DataBind();
            lblResultCount.Text = records.Count.ToString(CultureInfo.InvariantCulture);
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            gvTrees.PageIndex = 0;
            BindGrid();
        }

        protected void gvTrees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTrees.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvTrees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "OpenTree", StringComparison.OrdinalIgnoreCase)) return;
            if (!int.TryParse(e.CommandArgument?.ToString(), out int treeId)) return;

            var record = TreeService.GetTree(treeId);
            if (record == null)
            {
                ShowMessage("提示", "找不到指定樹籍資料", "warning");
                BindGrid();
                return;
            }

            if (IsEditorAccount())
            {
                LoadTree(treeId);
            }
            else
            {
                Response.Redirect("TreeView.aspx?treeId=" + treeId, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void gvTrees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var record = e.Row.DataItem as TreeRecord;
            if (record == null) return;

            if (e.Row.FindControl("litStatus") is HtmlGenericControl litStatus)
            {
                litStatus.InnerText = record.StatusText;
                litStatus.Attributes["style"] = "color:" + GetStatusColor(record.Status);
            }

            if (e.Row.FindControl("litEditStatus") is HtmlGenericControl litEdit)
            {
                litEdit.InnerText = record.EditStatusText;
                litEdit.Attributes["style"] = "color:" + GetEditStatusColor(record.EditStatus);
            }

        }

        private static string GetStatusColor(TreeStatus status)
        {
            switch (status)
            {
                case TreeStatus.已公告列管:
                    return "#adff2f";
                case TreeStatus.符合標準:
                    return "#ffd700";
                default:
                    return "#c7a3ff";
            }
        }

        private static string GetEditStatusColor(TreeEditState state)
        {
            return state == TreeEditState.定稿 ? "#f6ad49" : "#00bfff";
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk)
            {
                foreach (GridViewRow row in gvTrees.Rows)
                {
                    if (row.FindControl("chkSelect") is CheckBox item)
                    {
                        item.Checked = chk.Checked;
                    }
                }
            }
        }

        protected void btnBatchStatus_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedTreeIds();
            if (ids.Count == 0)
            {
                ShowMessage("提示", "請先勾選至少一筆樹籍資料", "warning");
                return;
            }

            if (!CanCreate())
            {
                ShowMessage("權限不足", "您沒有批次變更樹籍狀態的權限", "warning");
                return;
            }

            if (!int.TryParse(ddlBatchStatus.SelectedValue, out int statusValue))
            {
                ShowMessage("提示", "請選擇要變更的樹籍狀態", "warning");
                return;
            }

            var status = (TreeStatus)statusValue;
            DateTime? announcementDate = null;
            if (status == TreeStatus.已公告列管)
            {
                if (!DateTime.TryParse(txtBatchAnnouncementDate.Text, out DateTime annDate))
                {
                    ShowMessage("提示", "請輸入公告日期", "warning");
                    return;
                }
                announcementDate = annDate;
            }

            try
            {
                TreeService.BulkUpdateStatus(ids, status, announcementDate, CurrentUser?.userID ?? 0);

                string memo = $"狀態：{TreeService.GetStatusText(status)}" + (announcementDate.HasValue ? $"，公告日期：{announcementDate:yyyy-MM-dd}" : string.Empty);
                foreach (int id in ids)
                {
                    LogTreeAction(id, "批次變更狀態", memo);
                }

                ShowMessage("成功", "已更新樹籍狀態", "success");
                BindGrid();

                if (mvMain.ActiveViewIndex == 1 && int.TryParse(hfTreeID.Value, out int currentId) && ids.Contains(currentId))
                {
                    LoadTree(currentId);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "批次更新失敗：" + ex.Message, "error");
            }
        }

        private List<int> GetSelectedTreeIds()
        {
            var ids = new List<int>();
            foreach (GridViewRow row in gvTrees.Rows)
            {
                if (row.FindControl("chkSelect") is CheckBox chk && chk.Checked)
                {
                    if (row.FindControl("hfTreeID") is HiddenField hf && int.TryParse(hf.Value, out int id))
                    {
                        ids.Add(id);
                    }
                }
            }
            return ids;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!CanCreate())
            {
                ShowMessage("權限不足", "您沒有新增樹籍資料的權限", "warning");
                return;
            }

            ClearDetailForm();
            hfTreeID.Value = string.Empty;
            hfSourceUnit.Value = CurrentUser?.unit;
            SetDetailEditable(true);
            lblDetailNotice.Text = string.Empty;
            pnlPhotoUpload.Visible = true;
            pnlDetailNavigation.Visible = false;
            pnlAnnouncementDate.Visible = true;
            SetDetailLinks(null);
            mvMain.ActiveViewIndex = 1;
        }

        private void LoadTree(int treeId)
        {
            var record = TreeService.GetTree(treeId);
            if (record == null)
            {
                ShowMessage("提示", "找不到指定樹籍資料", "warning");
                return;
            }

            ClearDetailForm();

            hfTreeID.Value = record.TreeID.ToString();
            hfSourceUnit.Value = record.SourceUnit;

            if (record.CityID.HasValue)
            {
                ddlDetailCity.SelectedValue = record.CityID.Value.ToString(CultureInfo.InvariantCulture);
                BindAreaDropdown(ddlDetailArea, record.CityID, includeAll: false);
            }
            else
            {
                ddlDetailCity.SelectedValue = string.Empty;
                BindAreaDropdown(ddlDetailArea, null, includeAll: false);
            }

            if (record.AreaID.HasValue && ddlDetailArea.Items.FindByValue(record.AreaID.Value.ToString(CultureInfo.InvariantCulture)) != null)
            {
                ddlDetailArea.SelectedValue = record.AreaID.Value.ToString(CultureInfo.InvariantCulture);
            }

            txtSurveyDate.Text = record.SurveyDate?.ToString("yyyy-MM-dd");
            txtSurveyor.Text = record.Surveyor;
            txtSystemTreeNo.Text = record.SystemTreeNo;
            txtJurisdictionCode.Text = record.AgencyJurisdictionCode;
            txtAgencyTreeNo.Text = record.AgencyTreeNo;

            if (record.SpeciesID.HasValue && ddlDetailSpecies.Items.FindByValue(record.SpeciesID.Value.ToString(CultureInfo.InvariantCulture)) != null)
            {
                ddlDetailSpecies.SelectedValue = record.SpeciesID.Value.ToString(CultureInfo.InvariantCulture);
            }

            txtTreeCount.Text = record.TreeCount.ToString(CultureInfo.InvariantCulture);
            ddlDetailStatus.SelectedValue = ((int)record.Status).ToString();
            txtDetailAnnouncementDate.Text = record.AnnouncementDate?.ToString("yyyy-MM-dd");
            ddlDetailEditStatus.SelectedValue = ((int)record.EditStatus).ToString();
            txtManager.Text = record.Manager;
            txtManagerContact.Text = record.ManagerContact;
            txtSite.Text = record.Site;
            txtLatitude.Text = record.Latitude?.ToString(CultureInfo.InvariantCulture);
            txtLongitude.Text = record.Longitude?.ToString(CultureInfo.InvariantCulture);
            txtLandOwnership.Text = record.LandOwnership;
            txtLandNote.Text = record.LandOwnershipNote;
            txtFacility.Text = record.FacilityDescription;
            txtMemo.Text = record.Memo;
            txtRecognitionNote.Text = record.RecognitionNote;
            txtCulturalHistory.Text = record.CulturalHistoryIntro;
            txtEstimatedPlantingYear.Text = record.EstimatedPlantingYear;
            txtEstimatedAgeNote.Text = record.EstimatedAgeNote;
            txtGroupGrowthInfo.Text = record.GroupGrowthInfo;
            txtTreeHeight.Text = record.TreeHeight?.ToString(CultureInfo.InvariantCulture);
            txtBreastHeightDiameter.Text = record.BreastHeightDiameter?.ToString(CultureInfo.InvariantCulture);
            txtBreastHeightCircumference.Text = record.BreastHeightCircumference?.ToString(CultureInfo.InvariantCulture);
            txtCanopyProjectionArea.Text = record.CanopyProjectionArea?.ToString(CultureInfo.InvariantCulture);
            txtHealthCondition.Text = record.HealthCondition;
            chkHasEpiphyte.Checked = record.HasEpiphyte ?? false;
            txtEpiphyteDescription.Text = record.EpiphyteDescription;
            chkHasParasite.Checked = record.HasParasite ?? false;
            txtParasiteDescription.Text = record.ParasiteDescription;
            chkHasClimbingPlant.Checked = record.HasClimbingPlant ?? false;
            txtClimbingPlantDescription.Text = record.ClimbingPlantDescription;
            txtSurveyOtherNote.Text = record.SurveyOtherNote;

            foreach (ListItem item in cblCriteria.Items)
            {
                item.Selected = record.RecognitionCriteria.Contains(item.Value);
            }

            pnlAnnouncementDate.Visible = record.Status == TreeStatus.已公告列管;

            bool canEdit = CanEditRecord(record);
            SetDetailEditable(canEdit);
            pnlPhotoUpload.Visible = canEdit;
            lblDetailNotice.Text = canEdit ? string.Empty : "⚠ 非資料持有單位";

            DetailEditable = canEdit;

            BindPhotoList(record.TreeID);
            SetDetailLinks(record.TreeID);
            BindLogs(record.TreeID, resetPage: true);

            mvMain.ActiveViewIndex = 1;
        }

        private void BindPhotoList(int treeId)
        {
            var photos = TreeService.GetPhotos(treeId) ?? Enumerable.Empty<TreePhoto>();
            var photoList = photos.ToList();
            phPhotoList.Visible = photoList.Count > 0;
            pnlPhotosEmpty.Visible = photoList.Count == 0;
            rptPhotos.DataSource = photoList;
            rptPhotos.DataBind();
        }

        private void BindLogs(int treeId, bool resetPage = false)
        {
            if (resetPage)
            {
                LogPageIndex = 0;
            }

            var logs = FunctionLogService.GetLogs(LogFunctionTypes.TreeCatalog, treeId) ?? Enumerable.Empty<FunctionLogEntry>();
            var logList = logs.ToList();
            int totalCount = logList.Count;
            phLogList.Visible = totalCount > 0;
            pnlLogsEmpty.Visible = totalCount == 0;

            int pageCount = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)LogPageSize) : 0;
            if (pageCount == 0)
            {
                LogPageIndex = 0;
            }
            else
            {
                LogPageIndex = Math.Max(0, Math.Min(LogPageIndex, pageCount - 1));
            }

            var pageLogs = logList
                .Skip(LogPageIndex * LogPageSize)
                .Take(LogPageSize)
                .ToList();

            rptLogs.DataSource = pageLogs;
            rptLogs.DataBind();

            BindLogPager(totalCount, pageCount);
        }

        private void BindLogPager(int totalCount, int pageCount)
        {
            if (litLogSummary != null)
            {
                if (totalCount > 0)
                {
                    int displayPageCount = Math.Max(pageCount, 1);
                    int currentPage = Math.Min(LogPageIndex + 1, displayPageCount);
                    litLogSummary.Text = string.Format(CultureInfo.InvariantCulture, "第 {0} / {1} 頁，共 {2} 筆", currentPage, displayPageCount, totalCount);
                }
                else
                {
                    litLogSummary.Text = string.Empty;
                }
            }

            if (phLogPager == null || rptLogPager == null)
            {
                return;
            }

            phLogPager.Visible = totalCount > 0;

            if (totalCount <= LogPageSize)
            {
                rptLogPager.Visible = false;
                rptLogPager.DataSource = null;
                rptLogPager.DataBind();
                return;
            }

            var pageItems = Enumerable.Range(0, pageCount)
                .Select(i => new
                {
                    Index = i,
                    Text = (i + 1).ToString(CultureInfo.InvariantCulture),
                    IsCurrent = i == LogPageIndex
                })
                .ToList();

            rptLogPager.Visible = true;
            rptLogPager.DataSource = pageItems;
            rptLogPager.DataBind();
        }

        protected void rptLogPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "Page", StringComparison.OrdinalIgnoreCase)) return;
            if (!int.TryParse(e.CommandArgument?.ToString(), out int pageIndex)) return;
            if (!int.TryParse(hfTreeID.Value, out int treeId) || treeId <= 0) return;

            LogPageIndex = pageIndex;
            BindLogs(treeId);
        }

        private int SaveUploadedPhotos(int treeId)
        {
            var postedFiles = fuPhoto.PostedFiles;
            if (postedFiles == null || postedFiles.Count == 0) return 0;

            var files = postedFiles.Cast<HttpPostedFile>().Where(f => f != null && f.ContentLength > 0).ToList();
            if (files.Count == 0) return 0;

            string relativeFolder = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", UploadRoot.TrimEnd('/'), treeId);
            string folder = Server.MapPath(relativeFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            int savedCount = 0;

            int totalBytes = files.Sum(f => f.ContentLength);
            if (MaxUploadBytes > 0 && totalBytes > MaxUploadBytes)
            {
                throw new InvalidOperationException($"選擇的照片總大小超過限制（{PhotoUploadLimitDisplay}），請分批上傳。");
            }

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                string extension = Path.GetExtension(file.FileName);
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture) + "_" + Guid.NewGuid().ToString("N") + extension;
                string physicalPath = Path.Combine(folder, fileName);
                file.SaveAs(physicalPath);

                string relativePath = VirtualPathUtility.ToAbsolute(string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", UploadRoot.TrimEnd('/'), treeId, fileName));
                string captionField = Request.Form[$"photoCaptionList_{i}"];
                string caption = string.IsNullOrWhiteSpace(captionField) ? Path.GetFileNameWithoutExtension(file.FileName) : captionField.Trim();

                var photo = new TreePhoto
                {
                    TreeID = treeId,
                    FileName = fileName,
                    FilePath = relativePath,
                    Caption = caption,
                    IsCover = false
                };

                int photoId = TreeService.InsertPhoto(photo, CurrentUser?.userID ?? 0);
                LogTreeAction(treeId, "新增照片", caption);
                savedCount++;
            }

            BindPhotoList(treeId);
            BindLogs(treeId, resetPage: true);
            return savedCount;
        }

        private static int GetMaxUploadBytes()
        {
            try
            {
                var section = WebConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
                if (section != null && section.MaxRequestLength > 0)
                {
                    return section.MaxRequestLength * 1024;
                }
            }
            catch (Exception)
            {
                // Ignore and fallback to default value below.
            }

            // Default to 4 MB if not configured.
            return 4 * 1024 * 1024;
        }

        private static string FormatBytes(int bytes)
        {
            const double OneMB = 1024d * 1024d;
            const double OneKB = 1024d;

            if (bytes >= OneMB)
            {
                double value = bytes / OneMB;
                return value >= 10 ? value.ToString("0", CultureInfo.InvariantCulture) + " MB"
                    : value.ToString("0.0", CultureInfo.InvariantCulture) + " MB";
            }

            if (bytes >= OneKB)
            {
                double value = bytes / OneKB;
                return value.ToString("0", CultureInfo.InvariantCulture) + " KB";
            }

            return bytes + " Bytes";
        }

        private void ClearDetailForm()
        {
            txtSurveyDate.Text = string.Empty;
            txtSurveyor.Text = string.Empty;
            ddlDetailCity.SelectedValue = string.Empty;
            BindAreaDropdown(ddlDetailArea, null, includeAll: false);
            txtSystemTreeNo.Text = string.Empty;
            txtJurisdictionCode.Text = string.Empty;
            txtAgencyTreeNo.Text = string.Empty;
            ddlDetailSpecies.SelectedValue = string.Empty;
            txtTreeCount.Text = "1";
            ddlDetailStatus.SelectedIndex = 0;
            txtDetailAnnouncementDate.Text = string.Empty;
            ddlDetailEditStatus.SelectedIndex = 0;
            txtManager.Text = string.Empty;
            txtManagerContact.Text = string.Empty;
            txtSite.Text = string.Empty;
            txtLatitude.Text = string.Empty;
            txtLongitude.Text = string.Empty;
            txtLandOwnership.Text = string.Empty;
            txtLandNote.Text = string.Empty;
            txtFacility.Text = string.Empty;
            txtMemo.Text = string.Empty;
            txtRecognitionNote.Text = string.Empty;
            txtCulturalHistory.Text = string.Empty;
            txtEstimatedPlantingYear.Text = string.Empty;
            txtEstimatedAgeNote.Text = string.Empty;
            txtGroupGrowthInfo.Text = string.Empty;
            txtTreeHeight.Text = string.Empty;
            txtBreastHeightDiameter.Text = string.Empty;
            txtBreastHeightCircumference.Text = string.Empty;
            txtCanopyProjectionArea.Text = string.Empty;
            txtHealthCondition.Text = string.Empty;
            chkHasEpiphyte.Checked = false;
            txtEpiphyteDescription.Text = string.Empty;
            chkHasParasite.Checked = false;
            txtParasiteDescription.Text = string.Empty;
            chkHasClimbingPlant.Checked = false;
            txtClimbingPlantDescription.Text = string.Empty;
            txtSurveyOtherNote.Text = string.Empty;
            foreach (ListItem item in cblCriteria.Items)
            {
                item.Selected = false;
            }
            phPhotoList.Visible = false;
            pnlPhotosEmpty.Visible = true;
            rptPhotos.DataSource = null;
            rptPhotos.DataBind();
            pnlAnnouncementDate.Visible = false;
            phLogList.Visible = false;
            pnlLogsEmpty.Visible = true;
            rptLogs.DataSource = null;
            rptLogs.DataBind();
        }

        private void SetDetailEditable(bool editable)
        {
            var controls = new WebControl[]
            {
            txtSurveyDate, txtSurveyor, ddlDetailCity, ddlDetailArea,
            txtJurisdictionCode, txtAgencyTreeNo, ddlDetailSpecies, txtTreeCount,
            ddlDetailStatus, txtDetailAnnouncementDate, ddlDetailEditStatus,
            txtManager, txtManagerContact, txtSite, txtLatitude, txtLongitude,
            txtLandOwnership, txtLandNote, txtFacility, txtMemo,
            txtRecognitionNote, txtCulturalHistory, txtEstimatedPlantingYear, txtEstimatedAgeNote,
            txtGroupGrowthInfo, txtTreeHeight, txtBreastHeightDiameter, txtBreastHeightCircumference,
            txtCanopyProjectionArea, txtHealthCondition, txtEpiphyteDescription, txtParasiteDescription,
            txtClimbingPlantDescription, txtSurveyOtherNote
            };

            foreach (var control in controls)
            {
                control.Enabled = editable;
            }

            chkHasEpiphyte.Enabled = editable;
            chkHasParasite.Enabled = editable;
            chkHasClimbingPlant.Enabled = editable;
            txtSystemTreeNo.Enabled = false;
            txtSystemTreeNo.ReadOnly = true;

            cblCriteria.Enabled = editable;
            btnSave.Visible = editable;
            pnlPhotoUpload.Visible = editable;
            DetailEditable = editable;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            mvMain.ActiveViewIndex = 0;
            BindGrid();
        }

        private bool TryPrepareTreeRecord(out TreeRecord record, out bool isNew, out int treeId)
        {
            record = null;
            treeId = 0;
            isNew = true;

            if (CurrentUser == null)
            {
                ShowMessage("未登入", "請先登入後再進行操作", "warning");
                return false;
            }

            if (int.TryParse(hfTreeID.Value, out int parsedId) && parsedId > 0)
            {
                treeId = parsedId;
                isNew = false;
            }

            if (isNew && !IsEditorAccount())
            {
                ShowMessage("權限不足", "您沒有新增樹籍資料的權限", "warning");
                return false;
            }

            if (!isNew && !DetailEditable)
            {
                ShowMessage("權限不足", "您沒有編輯此樹籍資料的權限", "warning");
                return false;
            }

            record = new TreeRecord
            {
                TreeID = treeId,
                AgencyJurisdictionCode = txtJurisdictionCode.Text.Trim(),
                AgencyTreeNo = txtAgencyTreeNo.Text.Trim(),
                CityID = ParseNullableInt(ddlDetailCity.SelectedValue),
                AreaID = ParseNullableInt(ddlDetailArea.SelectedValue),
                SpeciesID = ParseNullableInt(ddlDetailSpecies.SelectedValue),
                Manager = txtManager.Text.Trim(),
                ManagerContact = txtManagerContact.Text.Trim(),
                Site = txtSite.Text.Trim(),
                LandOwnership = txtLandOwnership.Text.Trim(),
                LandOwnershipNote = txtLandNote.Text.Trim(),
                FacilityDescription = txtFacility.Text.Trim(),
                Memo = txtMemo.Text.Trim(),
                RecognitionNote = txtRecognitionNote.Text.Trim(),
                CulturalHistoryIntro = txtCulturalHistory.Text.Trim(),
                EstimatedPlantingYear = txtEstimatedPlantingYear.Text.Trim(),
                EstimatedAgeNote = txtEstimatedAgeNote.Text.Trim(),
                GroupGrowthInfo = txtGroupGrowthInfo.Text.Trim(),
                HealthCondition = txtHealthCondition.Text.Trim(),
                EpiphyteDescription = txtEpiphyteDescription.Text.Trim(),
                ParasiteDescription = txtParasiteDescription.Text.Trim(),
                ClimbingPlantDescription = txtClimbingPlantDescription.Text.Trim(),
                SurveyOtherNote = txtSurveyOtherNote.Text.Trim(),
                SourceUnit = string.IsNullOrWhiteSpace(hfSourceUnit.Value) ? CurrentUser.unit : hfSourceUnit.Value,
                RecognitionCriteria = cblCriteria.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList()
            };

            if (record.SpeciesID.HasValue)
            {
                // 把 out 參數的值複製到一般變數（避免捕捉 out 參數）
                int speciesId = record.SpeciesID.Value;

                var species = TreeService.GetSpecies()
                                         .FirstOrDefault(s => s.SpeciesID == speciesId);
                if (species != null)
                {
                    record.SpeciesCommonName = species.CommonName;
                    record.SpeciesScientificName = species.ScientificName;
                }
            }

            record.CityName = record.CityID.HasValue ? ddlDetailCity.SelectedItem?.Text : null;
            record.AreaName = record.AreaID.HasValue ? ddlDetailArea.SelectedItem?.Text : null;

            if (DateTime.TryParse(txtSurveyDate.Text, out DateTime survey))
            {
                record.SurveyDate = survey;
            }

            if (int.TryParse(ddlDetailStatus.SelectedValue, out int statusValue))
            {
                record.Status = (TreeStatus)statusValue;
            }

            if (int.TryParse(ddlDetailEditStatus.SelectedValue, out int editValue))
            {
                record.EditStatus = (TreeEditState)editValue;
            }

            if (!int.TryParse(txtTreeCount.Text, out int count) || count <= 0)
            {
                count = 1;
            }
            record.TreeCount = count;

            record.Latitude = ParseNullableDecimal(txtLatitude.Text);
            record.Longitude = ParseNullableDecimal(txtLongitude.Text);
            record.TreeHeight = ParseNullableDecimal(txtTreeHeight.Text);
            record.BreastHeightDiameter = ParseNullableDecimal(txtBreastHeightDiameter.Text);
            record.BreastHeightCircumference = ParseNullableDecimal(txtBreastHeightCircumference.Text);
            record.CanopyProjectionArea = ParseNullableDecimal(txtCanopyProjectionArea.Text);
            record.HasEpiphyte = chkHasEpiphyte.Checked;
            record.HasParasite = chkHasParasite.Checked;
            record.HasClimbingPlant = chkHasClimbingPlant.Checked;

            record.Surveyor = txtSurveyor.Text.Trim();

            if (record.Status == TreeStatus.已公告列管 && DateTime.TryParse(txtDetailAnnouncementDate.Text, out DateTime announcement))
            {
                record.AnnouncementDate = announcement;
            }
            else
            {
                record.AnnouncementDate = null;
            }

            record.IsAnnounced = record.Status == TreeStatus.已公告列管;

            if (record.Status == TreeStatus.已公告列管 && !record.AnnouncementDate.HasValue)
            {
                ShowMessage("提示", "請輸入公告日期", "warning");
                return false;
            }

            if (isNew)
            {
                if (!record.CityID.HasValue || !record.AreaID.HasValue)
                {
                    ShowMessage("提示", "請先選擇縣市與鄉鎮市區以產生系統樹籍編號", "warning");
                    return false;
                }

                try
                {
                    record.SystemTreeNo = TreeService.GenerateSystemTreeNo(record.CityID, record.AreaID, record.SurveyDate ?? DateTime.Today);
                }
                catch (Exception ex)
                {
                    ShowMessage("錯誤", "產生系統樹籍編號時發生錯誤：" + ex.Message, "error");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(record.SystemTreeNo))
                {
                    ShowMessage("提示", "無法產生系統樹籍編號，請確認縣市鄉鎮設定及代碼資料", "warning");
                    return false;
                }

                txtSystemTreeNo.Text = record.SystemTreeNo;
            }
            else
            {
                record.SystemTreeNo = txtSystemTreeNo.Text.Trim();
            }

            return true;
        }

        private bool TryPersistTreeRecord(TreeRecord record, bool isNew, int currentTreeId, out int savedTreeId)
        {
            savedTreeId = currentTreeId;

            try
            {
                if (isNew)
                {
                    int newId = TreeService.InsertTree(record, CurrentUser.userID);
                    savedTreeId = newId;
                    hfTreeID.Value = newId.ToString(CultureInfo.InvariantCulture);
                    hfSourceUnit.Value = record.SourceUnit;
                    LogTreeAction(newId, "新增");
                }
                else
                {
                    record.TreeID = currentTreeId;
                    TreeService.UpdateTree(record, CurrentUser.userID);
                    LogTreeAction(currentTreeId, "編輯");
                    savedTreeId = currentTreeId;
                }

                hfTreeID.Value = savedTreeId.ToString(CultureInfo.InvariantCulture);
                BindGrid();
                return true;
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "儲存樹籍資料時發生錯誤：" + ex.Message, "error");
                return false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!TryPrepareTreeRecord(out TreeRecord record, out bool isNew, out int treeId))
            {
                return;
            }

            if (!TryPersistTreeRecord(record, isNew, treeId, out int savedTreeId))
            {
                return;
            }

            int uploadedCount = 0;

            try
            {
                uploadedCount = SaveUploadedPhotos(savedTreeId);
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage("提示", ex.Message, "warning");
                LoadTree(savedTreeId);
                return;
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "上傳照片失敗：" + ex.Message, "error");
                LoadTree(savedTreeId);
                return;
            }

            string actionText = isNew ? "新增" : "更新";
            string message = $"樹籍資料已{actionText}";
            if (uploadedCount > 0)
            {
                message += $"，並上傳 {uploadedCount} 張照片";
            }
            ShowMessage("成功", message, "success");
            LoadTree(savedTreeId);
        }

        protected void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                ShowMessage("未登入", "請先登入後再進行操作", "warning");
                return;
            }

            if (!DetailEditable)
            {
                ShowMessage("權限不足", "您沒有編輯照片的權限", "warning");
                return;
            }

            if (!fuPhoto.HasFile && (fuPhoto.PostedFiles == null || fuPhoto.PostedFiles.Count == 0))
            {
                ShowMessage("提示", "請選擇要上傳的照片", "warning");
                return;
            }

            bool createdDuringUpload = false;
            int treeId;

            if (!int.TryParse(hfTreeID.Value, out treeId) || treeId <= 0)
            {
                if (!TryPrepareTreeRecord(out TreeRecord record, out bool isNew, out treeId))
                {
                    return;
                }

                if (!TryPersistTreeRecord(record, isNew, treeId, out int savedTreeId))
                {
                    return;
                }

                treeId = savedTreeId;
                createdDuringUpload = isNew;
            }

            try
            {
                int count = SaveUploadedPhotos(treeId);
                if (count > 0)
                {
                    string successMessage = createdDuringUpload
                        ? $"樹籍資料已新增，並上傳 {count} 張照片"
                        : $"已上傳 {count} 張照片";
                    ShowMessage("成功", successMessage, "success");
                    if (createdDuringUpload)
                    {
                        LoadTree(treeId);
                    }
                }
                else
                {
                    ShowMessage("提示", "請選擇要上傳的照片", "warning");
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage("提示", ex.Message, "warning");
                if (createdDuringUpload && treeId > 0)
                {
                    LoadTree(treeId);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "上傳照片失敗：" + ex.Message, "error");
                if (createdDuringUpload && treeId > 0)
                {
                    LoadTree(treeId);
                }
            }
        }

        protected void rptPhotos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(hfTreeID.Value, out int treeId) || treeId <= 0) return;
            if (!int.TryParse(e.CommandArgument?.ToString(), out int photoId)) return;
            if (!DetailEditable) return;

            try
            {
                string caption = null;
                if (e.Item.FindControl("hfPhotoCaption") is HiddenField hfCaption)
                {
                    caption = hfCaption.Value;
                }

                if (e.CommandName == "SetCover")
                {
                    TreeService.SetCoverPhoto(treeId, photoId, CurrentUser?.userID ?? 0);
                    LogTreeAction(treeId, "設定封面照片", caption);
                    ShowMessage("成功", "已設定封面照片", "success");
                }
                else if (e.CommandName == "DeletePhoto")
                {
                    TreeService.DeletePhoto(photoId, CurrentUser?.userID ?? 0);
                    LogTreeAction(treeId, "刪除照片", caption);
                    ShowMessage("成功", "照片已刪除", "success");
                }
                BindPhotoList(treeId);
                BindLogs(treeId, resetPage: true);
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "處理照片時發生錯誤：" + ex.Message, "error");
            }
        }

        protected void rptPhotos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            if (DetailEditable) return;

            if (e.Item.FindControl("btnSetCover") is LinkButton btnSetCover)
            {
                btnSetCover.Visible = false;
            }
            if (e.Item.FindControl("btnRemovePhoto") is LinkButton btnRemove)
            {
                btnRemove.Visible = false;
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var dt = TreeService.ExportTrees(BuildFilter());
                if (dt.Rows.Count == 0)
                {
                    ShowMessage("提示", "目前查無資料可供下載", "info");
                    return;
                }

                string fileName = $"受保護樹木清冊_{DateTime.Now:yyyyMMddHHmmss}.csv";
                var response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "text/csv";
                response.ContentEncoding = Encoding.UTF8;
                response.Charset = "utf-8";
                response.AddHeader("Content-Disposition", "attachment;filename=" + Uri.EscapeDataString(fileName));
                response.Write('\uFEFF');

                using (var sw = new StringWriter())
                {
                    WriteCsv(dt, sw);
                    response.Write(sw.ToString());
                }

                response.Flush();
                response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "產生匯出檔案時發生錯誤：" + ex.Message, "error");
            }
        }

        private static void WriteCsv(DataTable table, StringWriter sw)
        {
            var headers = table.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName));
            sw.WriteLine(string.Join(",", headers));

            foreach (DataRow row in table.Rows)
            {
                var values = table.Columns.Cast<DataColumn>().Select(c => EscapeCsv(row[c]?.ToString()));
                sw.WriteLine(string.Join(",", values));
            }
        }

        private static string EscapeCsv(string value)
        {
            if (value == null) return string.Empty;
            if (value.Contains("\"") || value.Contains(",") || value.Contains("\n"))
            {
                return '"' + value.Replace("\"", "\"\"") + '"';
            }
            return value;
        }

        private bool IsEditorAccount()
        {
            if (CurrentUser == null) return false;
            return CurrentUser.auType == Account.enum_auType.系統管理權限 || CurrentUser.auType == Account.enum_auType.樹木管理權限;
        }

        private bool CanCreate()
        {
            return IsEditorAccount();
        }

        private bool CanEditRecord(TreeRecord record)
        {
            if (CurrentUser == null) return false;
            if (CurrentUser.auType == Account.enum_auType.系統管理權限) return true;
            if (CurrentUser.auType == Account.enum_auType.樹木管理權限)
            {
                if (string.IsNullOrWhiteSpace(record.SourceUnit) || string.IsNullOrWhiteSpace(CurrentUser.unit)) return false;
                return string.Equals(record.SourceUnit.Trim(), CurrentUser.unit.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private static int? ParseNullableInt(string value)
        {
            if (int.TryParse(value, out int result)) return result;
            return null;
        }

        private static decimal? ParseNullableDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            string trimmed = value.Trim();
            if (decimal.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            if (decimal.TryParse(trimmed, NumberStyles.Float, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        private void LogTreeAction(int treeId, string actionType, string memo = null)
        {
            var user = CurrentUser;
            string ip = Request?.UserHostAddress;
            FunctionLogService.InsertLog(LogFunctionTypes.TreeCatalog, treeId, actionType ?? string.Empty, memo, ip, user?.userID, user?.account, user?.name, user?.unit);
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAreaDropdown(ddlArea, ParseNullableInt(ddlCity.SelectedValue), includeAll: true);
        }

        protected void ddlDetailCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAreaDropdown(ddlDetailArea, ParseNullableInt(ddlDetailCity.SelectedValue), includeAll: false);
        }

        private void SetDetailLinks(int? treeId)
        {
            string idParam = treeId.HasValue ? "?treeId=" + treeId.Value : string.Empty;
            lnkReturnToDetail.NavigateUrl = treeId.HasValue ? "~/Backstage/Manage/Tree.aspx" + idParam : "#";
            lnkHealth.NavigateUrl = treeId.HasValue ? "~/Backstage/Manage/Health.aspx" + idParam : "#";
            lnkPatrol.NavigateUrl = treeId.HasValue ? "~/Backstage/Patrol/TreeManage_Patrol_List.aspx" + idParam : "#";
            lnkCare.NavigateUrl = treeId.HasValue ? "~/Backstage/Care/TreeManage_Care_List.aspx" + idParam : "#";
            lnkReturnToDetail.Enabled = lnkHealth.Enabled = lnkPatrol.Enabled = lnkCare.Enabled = treeId.HasValue;
            pnlDetailNavigation.Visible = treeId.HasValue;
        }

        protected string ResolvePhotoPath(object pathObj)
        {
            var path = pathObj as string;
            var resolvedPath = VirtualPathHelper.ApplyVirtualName(path);
            return string.IsNullOrWhiteSpace(resolvedPath) ? "#" : ResolveUrl(resolvedPath);
        }
    }
}
