using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class view : BasePage
    {
        private const string SortExpressionKey = "TreeView_SortExpression";
        private const string SortDirectionKey = "TreeView_SortDirection";

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
            if (managedAreas == null)
                managedAreas = new HashSet<int>();
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
            if (e.CommandName == "ViewTree")
            {
                CollectFilterFromUI();
                setTreeID = e.CommandArgument.ToString();
                base.RedirectState("detail.aspx", CurrentFilter);
            }
        }
    }
}
