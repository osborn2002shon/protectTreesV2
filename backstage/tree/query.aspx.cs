using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class query : BasePage
    {
        private const string SortExpressionKey = "TreeQuery_SortExpression";
        private const string SortDirectionKey = "TreeQuery_SortDirection";

        protected TreeFilter CurrentFilter
        {
            get
            {
                return ViewState["CurrentFilter"] as TreeFilter ?? new TreeFilter();
            }
            set { ViewState["CurrentFilter"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsEditorAccount())
            {
                var savedFilter = base.GetState<TreeFilter>();
                base.RedirectState("view.aspx", savedFilter);
                return;
            }

            if (!IsPostBack)
            {
                BindDropdowns();
                var savedFilter = base.GetState<TreeFilter>();
                if (savedFilter != null)
                {
                    PopulateFilterToUI(savedFilter);
                    CurrentFilter = savedFilter;
                }
                else
                {
                    CollectFilterFromUI();
                }
                BindTrees();
            }
        }

        private void BindDropdowns()
        {
            ddlCity.Items.Clear();
            ddlCity.Items.Add(new ListItem("不拘", string.Empty));
            var cities = GetCities();
            foreach (var city in cities)
            {
                ddlCity.Items.Add(city);
            }

            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("不拘", string.Empty));

            ddlEditStatus.Items.Clear();
            ddlEditStatus.Items.Add(new ListItem("不拘", string.Empty));
            ddlEditStatus.Items.Add(new ListItem("草稿", ((int)TreeEditState.草稿).ToString()));
            ddlEditStatus.Items.Add(new ListItem("定稿", ((int)TreeEditState.定稿).ToString()));

            ddlTreeStatus.Items.Clear();
            ddlTreeStatus.Items.Add(new ListItem("不拘", string.Empty));
            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddlTreeStatus.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }

            ddlBulkStatus.Items.Clear();
            ddlBulkStatus.Items.Add(new ListItem("請選擇", string.Empty));
            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddlBulkStatus.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }

            ddlSpecies.Items.Clear();
            ddlSpecies.Items.Add(new ListItem("不拘", string.Empty));
            var species = TreeService.GetSpecies();
            foreach (var s in species)
            {
                ddlSpecies.Items.Add(new ListItem(s.DisplayName, s.SpeciesID.ToString()));
            }
        }

        private TreeFilter BuildFilter()
        {
            DateTime? ParseDate(string text)
            {
                if (DateTime.TryParse(text, out DateTime dt))
                {
                    return dt;
                }
                return null;
            }

            return new TreeFilter
            {
                CityID = string.IsNullOrWhiteSpace(ddlCity.SelectedValue) ? (int?)null : Convert.ToInt32(ddlCity.SelectedValue),
                AreaID = string.IsNullOrWhiteSpace(ddlArea.SelectedValue) ? (int?)null : Convert.ToInt32(ddlArea.SelectedValue),
                EditStatus = string.IsNullOrWhiteSpace(ddlEditStatus.SelectedValue) ? (TreeEditState?)null : (TreeEditState)Convert.ToInt32(ddlEditStatus.SelectedValue),
                Status = string.IsNullOrWhiteSpace(ddlTreeStatus.SelectedValue) ? (TreeStatus?)null : (TreeStatus)Convert.ToInt32(ddlTreeStatus.SelectedValue),
                SpeciesID = string.IsNullOrWhiteSpace(ddlSpecies.SelectedValue) ? (int?)null : Convert.ToInt32(ddlSpecies.SelectedValue),
                SurveyDateStart = ParseDate(txtSurveyStart.Text),
                SurveyDateEnd = ParseDate(txtSurveyEnd.Text),
                AnnouncementDateStart = ParseDate(txtAnnouncementStart.Text),
                AnnouncementDateEnd = ParseDate(txtAnnouncementEnd.Text),
                Keyword = txtKeyword.Text.Trim()
            };
        }

        private void CollectFilterFromUI()
        {
            CurrentFilter = BuildFilter();
        }

        private void PopulateFilterToUI(TreeFilter filter)
        {
            if (filter == null) return;

            if (filter.CityID.HasValue)
            {
                var cityVal = filter.CityID.Value.ToString();
                if (ddlCity.Items.FindByValue(cityVal) != null)
                {
                    ddlCity.SelectedValue = cityVal;
                    BindAreas();
                }
            }

            if (filter.AreaID.HasValue)
            {
                var areaVal = filter.AreaID.Value.ToString();
                if (ddlArea.Items.FindByValue(areaVal) != null)
                {
                    ddlArea.SelectedValue = areaVal;
                }
            }

            if (filter.EditStatus.HasValue)
            {
                var editStatusVal = ((int)filter.EditStatus.Value).ToString();
                if (ddlEditStatus.Items.FindByValue(editStatusVal) != null)
                {
                    ddlEditStatus.SelectedValue = editStatusVal;
                }
            }

            if (filter.Status.HasValue)
            {
                var statusVal = ((int)filter.Status.Value).ToString();
                if (ddlTreeStatus.Items.FindByValue(statusVal) != null)
                {
                    ddlTreeStatus.SelectedValue = statusVal;
                }
            }

            if (filter.SpeciesID.HasValue)
            {
                var speciesVal = filter.SpeciesID.Value.ToString();
                if (ddlSpecies.Items.FindByValue(speciesVal) != null)
                {
                    ddlSpecies.SelectedValue = speciesVal;
                }
            }

            txtSurveyStart.Text = filter.SurveyDateStart?.ToString("yyyy-MM-dd") ?? string.Empty;
            txtSurveyEnd.Text = filter.SurveyDateEnd?.ToString("yyyy-MM-dd") ?? string.Empty;
            txtAnnouncementStart.Text = filter.AnnouncementDateStart?.ToString("yyyy-MM-dd") ?? string.Empty;
            txtAnnouncementEnd.Text = filter.AnnouncementDateEnd?.ToString("yyyy-MM-dd") ?? string.Empty;
            txtKeyword.Text = filter.Keyword ?? string.Empty;
        }

        private void BindAreas()
        {
            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("不拘", string.Empty));
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

        private void BindTrees()
        {
            var filter = CurrentFilter;
            var records = TreeService.SearchTrees(filter) ?? new List<TreeRecord>();
            var managedAreas = GetManagedAreaIds();
            records = ApplyVisibilityFilter(records, managedAreas);

            var sortExpression = ViewState[SortExpressionKey] as string;
            var sortDirection = ViewState[SortDirectionKey] as string ?? "ASC";

            records = ApplySort(records, sortExpression, sortDirection);

            gvTrees.DataSource = records;
            gvTrees.DataBind();

            var total = ApplyVisibilityFilter(TreeService.SearchTrees(null) ?? new List<TreeRecord>(), managedAreas).Count;
            lblCount.Text = $"資料總筆數：{total}／查詢結果：{records.Count}";
        }

        private static List<TreeRecord> ApplySort(IEnumerable<TreeRecord> source, string expression, string direction)
        {
            if (string.IsNullOrWhiteSpace(expression)) return source.ToList();

            Func<TreeRecord, object> keySelector;

            switch (expression)
            {
                case "SystemTreeNo": keySelector = r => r.SystemTreeNo; break;
                case "AgencyTreeNo": keySelector = r => r.AgencyTreeNo; break;
                case "AgencyJurisdictionCode": keySelector = r => r.AgencyJurisdictionCode; break;
                case "CityName": keySelector = r => r.CityName; break;
                case "AreaName": keySelector = r => r.AreaName; break;
                case "SpeciesCommonName": keySelector = r => r.SpeciesCommonName; break;
                case "SurveyDate": keySelector = r => r.SurveyDate; break;
                case "AnnouncementDate": keySelector = r => r.AnnouncementDate; break;
                case "Status": keySelector = r => r.Status; break;
                case "EditStatus": keySelector = r => r.EditStatus; break;
                default: keySelector = r => r.SystemTreeNo; break;
            }

            return string.Equals(direction, "DESC", StringComparison.OrdinalIgnoreCase)
                ? source.OrderByDescending(keySelector).ToList()
                : source.OrderBy(keySelector).ToList();
        }

        private static List<TreeRecord> ApplyVisibilityFilter(IEnumerable<TreeRecord> source, HashSet<int> managedAreas)
        {
            if (source == null) return new List<TreeRecord>();
            managedAreas ??= new HashSet<int>();
            return source
                .Where(record => record != null && (IsManagedTree(record, managedAreas) || record.EditStatus == TreeEditState.定稿))
                .ToList();
        }

        private static bool IsManagedTree(TreeRecord record, HashSet<int> managedAreas)
        {
            if (record?.AreaID == null) return false;
            return managedAreas != null && managedAreas.Contains(record.AreaID.Value);
        }

        private HashSet<int> GetManagedAreaIds()
        {
            var user = UserInfo.GetCurrentUser;
            if (user == null) return new HashSet<int>();

            using (var da = new DataAccess.MS_SQL())
            {
                const string sql = "SELECT twID FROM System_UnitCityMapping WHERE unitID=@unitID";
                var dt = da.GetDataTable(sql, new System.Data.SqlClient.SqlParameter("@unitID", user.unitID));
                var managedAreas = new HashSet<int>();
                foreach (DataRow row in dt.Rows)
                {
                    if (int.TryParse(row["twID"]?.ToString(), out int twId))
                    {
                        managedAreas.Add(twId);
                    }
                }
                return managedAreas;
            }
        }

        private static bool IsEditorAccount()
        {
            var user = UserInfo.GetCurrentUser;
            return user != null && (user.auTypeID == 1 || user.auTypeID == 4);
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAreas();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CollectFilterFromUI();
            BindTrees();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlCity.SelectedIndex = 0;
            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("不拘", string.Empty));
            ddlEditStatus.SelectedIndex = 0;
            ddlTreeStatus.SelectedIndex = 0;
            ddlSpecies.SelectedIndex = 0;
            txtSurveyStart.Text = string.Empty;
            txtSurveyEnd.Text = string.Empty;
            txtAnnouncementStart.Text = string.Empty;
            txtAnnouncementEnd.Text = string.Empty;
            txtKeyword.Text = string.Empty;
            CollectFilterFromUI();
            BindTrees();
        }

        protected void gvTrees_Sorting(object sender, GridViewSortEventArgs e)
        {
            string currentExpression = ViewState[SortExpressionKey] as string;
            string currentDirection = ViewState[SortDirectionKey] as string ?? "ASC";

            if (string.Equals(currentExpression, e.SortExpression, StringComparison.OrdinalIgnoreCase))
            {
                currentDirection = currentDirection == "ASC" ? "DESC" : "ASC";
            }
            else
            {
                currentExpression = e.SortExpression;
                currentDirection = "ASC";
            }

            ViewState[SortExpressionKey] = currentExpression;
            ViewState[SortDirectionKey] = currentDirection;

            BindTrees();
        }

        protected void gvTrees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditTree" || e.CommandName == "ViewTree")
            {
                CollectFilterFromUI();
                setTreeID = e.CommandArgument.ToString();
                string target = e.CommandName == "EditTree" ? "edit.aspx" : "detail.aspx";
                base.RedirectState(target, CurrentFilter);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            CollectFilterFromUI();
            setTreeID = string.Empty;
            base.RedirectState("edit.aspx", CurrentFilter);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var filter = BuildFilter();
            DataTable data = TreeService.ExportTrees(filter);
            if (data == null || data.Rows.Count == 0)
            {
                ShowMessage("下載列表", "目前無可匯出資料", "info");
                return;
            }

            var user = UserInfo.GetCurrentUser;
            int accountId = user?.accountID ?? 0;
            UserLog.Insert_UserLog(accountId, UserLog.enum_UserLogItem.樹籍基本資料管理, UserLog.enum_UserLogType.下載, "下載樹籍基本資料");

            var sb = new StringBuilder();
            sb.Append("<table border='1'>");
            sb.Append("<tr>");
            foreach (DataColumn col in data.Columns)
            {
                sb.AppendFormat("<th>{0}</th>", col.ColumnName);
            }
            sb.Append("</tr>");

            foreach (DataRow row in data.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn col in data.Columns)
                {
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(row[col]));
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=TreeRecords.xls");
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(sb.ToString());
            Response.End();
        }

        protected void btnApplyStatus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlBulkStatus.SelectedValue))
            {
                ShowMessage("批次設定", "請選擇狀態", "warning");
                return;
            }

            var selected = new List<int>();
            foreach (GridViewRow row in gvTrees.Rows)
            {
                var chk = row.FindControl("chkSelect") as CheckBox;
                var hid = row.FindControl("hfTreeId") as HiddenField;
                if (chk != null && hid != null && chk.Checked && int.TryParse(hid.Value, out int id))
                {
                    selected.Add(id);
                }
            }

            if (selected.Count == 0)
            {
                ShowMessage("批次設定", "請先選擇要更新的樹籍", "warning");
                return;
            }

            var status = (TreeStatus)Convert.ToInt32(ddlBulkStatus.SelectedValue);
            DateTime? announcement = null;
            if (status == TreeStatus.已公告列管 && DateTime.TryParse(txtBulkAnnouncement.Text, out DateTime dt))
            {
                announcement = dt;
            }

            var user = UserInfo.GetCurrentUser;
            int accountId = user?.accountID ?? 0;

            TreeService.BulkUpdateStatus(selected, status, announcement, accountId);
            //OperationLogger.InsertLog("樹籍管理", "批次設定", $"更新{selected.Count}筆狀態為{TreeService.GetStatusText(status)}");
            UserLog.Insert_UserLog(user.accountID, UserLog.enum_UserLogItem.樹籍基本資料管理, UserLog.enum_UserLogType.修改, $"更新{selected.Count}筆狀態為{TreeService.GetStatusText(status)}");
            foreach (var treeId in selected)
            {
                TreeLog.InsertLog(TreeLog.LogFunctionTypes.TreeCatalog,
                    treeId,
                    "批次設定樹籍狀態",
                    $"狀態更新為{TreeService.GetStatusText(status)}",
                    Request?.UserHostAddress,
                    user?.accountID,
                    user?.account,
                    user?.name,
                    user?.unitName);
            }
            BindTrees();
        }
    }
}
