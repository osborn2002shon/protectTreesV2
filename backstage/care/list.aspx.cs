using protectTreesV2.TreeCatalog;
using protectTreesV2.User;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using static protectTreesV2.Care.Care;

namespace protectTreesV2.backstage.care
{
    public partial class list : protectTreesV2.Base.BasePage
    {

        public protectTreesV2.Care.Care system_care = new protectTreesV2.Care.Care();

        protected CareRecordListFilter CurrentFilter
        {
            get
            {
                return ViewState["CurrentFilter"] as CareRecordListFilter ?? new CareRecordListFilter();
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

                var savedFilter = base.GetState<CareRecordListFilter>();
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

        private void CheckExternalRequest()
        {
            if (!string.IsNullOrEmpty(setTreeID))
            {
                if (int.TryParse(setTreeID, out int targetId))
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
            var filter = new CareRecordListFilter
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

        private void PopulateFilterToUI(CareRecordListFilter filter)
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

            var user = UserService.GetCurrentUser();
            int userId = user?.userID ?? 0;

            List<CareRecordListResult> data = system_care.GetCareRecordList(filter, userId);

            Label_recordCount.Text = data != null ? data.Count.ToString() : "0";

            GridView_careList.DataSource = data;
            GridView_careList.DataBind();
        }

        protected void LinkButton_search_Click(object sender, EventArgs e)
        {
            CollectFilterFromUI();
            GridView_careList.PageIndex = 0;
            SortExpression = null;
            SortDirection = null;
            BindResult();
        }

        protected void GridView_careList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_careList.PageIndex = e.NewPageIndex;
            BindResult();
        }

        protected void GridView_careList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string arg = e.CommandArgument?.ToString();

            if (e.CommandName == "_ViewCare")
            {
                if (int.TryParse(arg, out int careId))
                {
                    setCareID = careId.ToString();
                    setTreeID = null;
                    base.RedirectState("edit.aspx", this.CurrentFilter);
                }
            }
            else if (e.CommandName == "_EditCare")
            {
                if (int.TryParse(arg, out int careId))
                {
                    setCareID = careId.ToString();
                    setTreeID = null;
                    base.RedirectState("edit.aspx", this.CurrentFilter);
                }
            }
            else if (e.CommandName == "_DeleteCare")
            {
                if (int.TryParse(arg, out int careId))
                {
                    var record = system_care.GetCareRecord(careId);
                    if (record == null)
                    {
                        ShowMessage("刪除失敗", "找不到養護紀錄。", "error");
                        return;
                    }

                    if (record.dataStatus != (int)CareRecordStatus.草稿)
                    {
                        ShowMessage("刪除失敗", "只有草稿狀態可以刪除。", "warning");
                        return;
                    }

                    var user = UserService.GetCurrentUser();
                    int accountId = user?.userID ?? 0;
                    system_care.DeleteCareRecord(careId, accountId);

                    ShowMessage("完成", "已刪除草稿養護紀錄。", "success");
                    BindResult();
                }
            }
        }

        protected void GridView_careList_Sorting(object sender, GridViewSortEventArgs e)
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
    }
}
