using protectTreesV2.TreeCatalog;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using static protectTreesV2.Patrol.Patrol;

namespace protectTreesV2.backstage.patrol
{
    public partial class list : protectTreesV2.Base.BasePage
    {
        public protectTreesV2.Patrol.Patrol system_patrol = new protectTreesV2.Patrol.Patrol();

        protected PatrolRecordListFilter CurrentFilter
        {
            get
            {
                return ViewState["CurrentFilter"] as PatrolRecordListFilter ?? new PatrolRecordListFilter();
            }
            set { ViewState["CurrentFilter"] = value; }
        }

        protected string SortExpression
        {
            get { return ViewState["SortExpression"] as string ?? "DefaultSort"; }
            set { ViewState["SortExpression"] = value; }
        }

        protected string SortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "DESC"; }
            set { ViewState["SortDirection"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitSearchFilters();

                var savedFilter = base.GetState<PatrolRecordListFilter>();
                if (savedFilter != null)
                {
                    PopulateFilterToUI(savedFilter);
                    CurrentFilter = savedFilter;
                }
                else
                {
                    CheckExternalRequest();
                    CollectFilterFromUI();
                }

                BindResult();
            }
        }

        /// <summary>
        /// 檢查跨頁傳值 (由其他頁面設定 setTreeID)
        /// </summary>
        private void CheckExternalRequest()
        {
            if (!string.IsNullOrEmpty(setTreeID))
            {
                int targetId;
                if (int.TryParse(setTreeID, out targetId))
                {
                    var tree = TreeCatalog.TreeService.GetTree(targetId);
                    string treeNo = tree?.SystemTreeNo;

                    if (!string.IsNullOrEmpty(treeNo))
                    {
                        TextBox_keyword.Text = treeNo;
                        RadioButtonList_scope.SelectedValue = "Unit";
                        TextBox_dateStart.Text = "";
                        TextBox_dateEnd.Text = "";
                    }
                }

                setTreeID = null;
            }
        }

        private void InitSearchFilters()
        {
            Base.DropdownBinder.Bind_DropDownList_City(ref DropDownList_city);
            Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, DropDownList_city.SelectedValue);
            Base.DropdownBinder.Bind_DropDownList_Species(ref DropDownList_species);
        }

        private void CollectFilterFromUI()
        {
            var filter = new PatrolRecordListFilter
            {
                scope = RadioButtonList_scope.SelectedValue,
                keyword = TextBox_keyword.Text.Trim()
            };

            if (int.TryParse(DropDownList_city.SelectedValue, out int city))
                filter.cityID = city;

            if (int.TryParse(DropDownList_area.SelectedValue, out int area))
                filter.areaID = area;

            if (int.TryParse(DropDownList_species.SelectedValue, out int species))
                filter.speciesID = species;

            if (DateTime.TryParse(TextBox_dateStart.Text, out DateTime start))
                filter.dateStart = start;

            if (DateTime.TryParse(TextBox_dateEnd.Text, out DateTime end))
                filter.dateEnd = end;

            CurrentFilter = filter;
        }

        private void PopulateFilterToUI(PatrolRecordListFilter filter)
        {
            if (filter == null) return;

            if (!string.IsNullOrEmpty(filter.scope))
            {
                if (RadioButtonList_scope.Items.FindByValue(filter.scope) != null)
                {
                    RadioButtonList_scope.SelectedValue = filter.scope;
                }
            }

            if (filter.cityID.HasValue)
            {
                string cityVal = filter.cityID.Value.ToString();
                if (DropDownList_city.Items.FindByValue(cityVal) != null)
                {
                    DropDownList_city.SelectedValue = cityVal;
                    Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, cityVal);
                }
            }

            if (filter.areaID.HasValue)
            {
                string areaVal = filter.areaID.Value.ToString();
                if (DropDownList_area.Items.FindByValue(areaVal) != null)
                {
                    DropDownList_area.SelectedValue = areaVal;
                }
            }

            if (filter.speciesID.HasValue)
            {
                string speciesVal = filter.speciesID.Value.ToString();
                if (DropDownList_species.Items.FindByValue(speciesVal) != null)
                {
                    DropDownList_species.SelectedValue = speciesVal;
                }
            }

            TextBox_dateStart.Text = filter.dateStart?.ToString("yyyy-MM-dd") ?? string.Empty;
            TextBox_dateEnd.Text = filter.dateEnd?.ToString("yyyy-MM-dd") ?? string.Empty;
            TextBox_keyword.Text = filter.keyword ?? string.Empty;
        }

        private void BindResult()
        {
            var filter = CurrentFilter;
            filter.sortExpression = SortExpression;
            filter.sortDirection = SortDirection;

            var user = UserInfo.GetCurrentUser;
            int userId = user?.accountID ?? 0;

            List<PatrolRecordListResult> data = system_patrol.GetPatrolRecordList(filter, userId);

            Label_recordCount.Text = data != null ? data.Count.ToString() : "0";

            GridView_patrolList.DataSource = data;
            GridView_patrolList.DataBind();
        }

        protected void LinkButton_search_Click(object sender, EventArgs e)
        {
            CollectFilterFromUI();
            GridView_patrolList.PageIndex = 0;
            SortExpression = null;
            SortDirection = null;
            BindResult();
        }

        protected void GridView_patrolList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_patrolList.PageIndex = e.NewPageIndex;
            BindResult();
        }

        protected void GridView_patrolList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string arg = e.CommandArgument?.ToString();

            if (e.CommandName == "_ViewPatrol")
            {
                if (int.TryParse(arg, out int patrolId))
                {
                    var record = system_patrol.GetPatrolRecord(patrolId);
                    var tree = record != null ? TreeService.GetTree(record.treeID) : null;
                    var photos = record != null ? system_patrol.GetPatrolPhotos(patrolId) : null;

                    BindBasicInfo(record, tree);
                    uc_patrolRecordModal.BindRecord(record, photos);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPatrolModal", "showPatrolRecordModal();", true);
                }
            }
            else if (e.CommandName == "_EditPatrol")
            {
                if (int.TryParse(arg, out int patrolId))
                {
                    setPatrolID = patrolId.ToString();
                    setTreeID = null;
                    base.RedirectState("edit.aspx", this.CurrentFilter);
                }
            }
            else if (e.CommandName == "_DeletePatrol")
            {
                if (int.TryParse(arg, out int patrolId))
                {
                    var record = system_patrol.GetPatrolRecord(patrolId);
                    if (record == null)
                    {
                        ShowMessage("刪除失敗", "找不到巡查紀錄。", "error");
                        return;
                    }

                    if (record.dataStatus != (int)PatrolRecordStatus.草稿)
                    {
                        ShowMessage("刪除失敗", "只有草稿狀態可以刪除。", "warning");
                        return;
                    }

                    var user = UserInfo.GetCurrentUser;
                    int accountId = user?.accountID ?? 0;
                    system_patrol.DeletePatrolRecord(patrolId, accountId);

                    ShowMessage("完成", "已刪除草稿巡查紀錄。", "success");
                    BindResult();
                }
            }
        }

        protected void GridView_patrolList_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (SortExpression == e.SortExpression)
            {
                SortDirection = (SortDirection == "ASC") ? "DESC" : "ASC";
            }
            else
            {
                SortExpression = e.SortExpression;
                SortDirection = "DESC";
            }

            BindResult();
        }

        protected void DropDownList_city_SelectedIndexChanged(object sender, EventArgs e)
        {
            Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, DropDownList_city.SelectedValue);
        }

        private void BindBasicInfo(protectTreesV2.Patrol.Patrol.PatrolRecord record, TreeRecord tree)
        {
            if (record == null)
            {
                litPatrolId.Text = string.Empty;
                litStatus.Text = string.Empty;
                litPatrolDate.Text = string.Empty;
                litPatroller.Text = string.Empty;
                litLastUpdate.Text = string.Empty;
                litSourceUnit.Text = string.Empty;
                litSystemTreeNo.Text = string.Empty;
                litAgencyTreeNo.Text = string.Empty;
                litLocation.Text = string.Empty;
                litSpecies.Text = string.Empty;
                litManager.Text = string.Empty;
                return;
            }

            litPatrolId.Text = record.patrolID.ToString(System.Globalization.CultureInfo.InvariantCulture);
            litStatus.Text = FormatText(GetStatusText(record.dataStatus));
            litPatrolDate.Text = FormatDate(record.patrolDate);
            litPatroller.Text = FormatText(record.patroller);
            litLastUpdate.Text = FormatDateTime(record.updateDateTime ?? record.insertDateTime);
            litSourceUnit.Text = FormatText(record.sourceUnit);

            if (tree == null)
            {
                litSystemTreeNo.Text = "--";
                litAgencyTreeNo.Text = "--";
                litLocation.Text = "--";
                litSpecies.Text = "--";
                litManager.Text = "--";
            }
            else
            {
                litSystemTreeNo.Text = FormatText(tree.SystemTreeNo);
                litAgencyTreeNo.Text = FormatText(tree.AgencyTreeNo);
                litLocation.Text = FormatText(CombineLocation(tree.CityName, tree.AreaName));
                litSpecies.Text = FormatText(tree.SpeciesDisplayName);
                litManager.Text = FormatText(tree.Manager);
            }
        }

        private static string FormatText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "--" : value.Trim();
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture) : "--";
        }

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy/MM/dd HH:mm", System.Globalization.CultureInfo.InvariantCulture) : "--";
        }

        private static string CombineLocation(string city, string area)
        {
            if (string.IsNullOrWhiteSpace(city)) return string.IsNullOrWhiteSpace(area) ? string.Empty : area;
            if (string.IsNullOrWhiteSpace(area)) return city;
            return city + area;
        }

        private static string GetStatusText(int status)
        {
            return status == (int)PatrolRecordStatus.定稿 ? "定稿" : "草稿";
        }
    }
}
