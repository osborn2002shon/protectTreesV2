using protectTreesV2.User;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using static protectTreesV2.Care.Care;

namespace protectTreesV2.backstage.care
{
    public partial class main : protectTreesV2.Base.BasePage
    {
        public protectTreesV2.Care.Care system_care = new protectTreesV2.Care.Care();

        protected CareMainQueryFilter CurrentFilter
        {
            get
            {
                return ViewState["CurrentFilter"] as CareMainQueryFilter ?? new CareMainQueryFilter();
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
            get { return ViewState["SortDirection"] as string ?? "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitSearchFilters();

                var savedFilter = base.GetState<CareMainQueryFilter>();
                if (savedFilter != null)
                {
                    PopulateFilterToUI(savedFilter);
                    CurrentFilter = savedFilter;
                }
                else
                {
                    CollectFilterFromUI();
                }

                BindResult();
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
            var filter = new CareMainQueryFilter();

            if (int.TryParse(DropDownList_city.SelectedValue, out int cityId)) filter.cityID = cityId;
            if (int.TryParse(DropDownList_area.SelectedValue, out int areaId)) filter.areaID = areaId;
            if (int.TryParse(DropDownList_species.SelectedValue, out int speciesId)) filter.speciesID = speciesId;

            filter.keyword = TextBox_keyword.Text.Trim();
            filter.queryOption = RadioButtonList_queryOption.SelectedValue;

            CurrentFilter = filter;
        }

        private void PopulateFilterToUI(CareMainQueryFilter filter)
        {
            if (filter == null) return;

            if (filter.cityID.HasValue)
            {
                string cityVal = filter.cityID.ToString();
                if (DropDownList_city.Items.FindByValue(cityVal) != null)
                {
                    DropDownList_city.SelectedValue = cityVal;
                    Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, cityVal);
                }
            }

            if (filter.areaID.HasValue)
            {
                string areaVal = filter.areaID.ToString();
                if (DropDownList_area.Items.FindByValue(areaVal) != null)
                {
                    DropDownList_area.SelectedValue = areaVal;
                }
            }

            if (filter.speciesID.HasValue)
            {
                string speciesVal = filter.speciesID.ToString();
                if (DropDownList_species.Items.FindByValue(speciesVal) != null)
                {
                    DropDownList_species.SelectedValue = speciesVal;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.queryOption))
            {
                if (RadioButtonList_queryOption.Items.FindByValue(filter.queryOption) != null)
                {
                    RadioButtonList_queryOption.SelectedValue = filter.queryOption;
                }
            }

            TextBox_keyword.Text = filter.keyword ?? string.Empty;
        }

        private void BindResult()
        {
            var filter = CurrentFilter;
            filter.sortExpression = SortExpression;
            filter.sortDirection = SortDirection;

            var user = UserService.GetCurrentUser();
            int accountID = user?.userID ?? 0;

            List<CareMainQueryResult> data = system_care.GetCareMainList(filter, accountID);

            Label_recordCount.Text = data != null ? data.Count.ToString() : "0";

            GridView_list.DataSource = data;
            GridView_list.DataBind();
        }

        protected void LinkButton_search_Click(object sender, EventArgs e)
        {
            CollectFilterFromUI();
            GridView_list.PageIndex = 0;
            SortExpression = null;
            SortDirection = null;
            BindResult();
        }

        protected void GridView_list_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (SortExpression == e.SortExpression)
            {
                SortDirection = (SortDirection == "ASC") ? "DESC" : "ASC";
            }
            else
            {
                SortExpression = e.SortExpression;
                SortDirection = "ASC";
            }
            BindResult();
        }

        protected void GridView_list_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_list.PageIndex = e.NewPageIndex;
            BindResult();
        }

        protected void DropDownList_city_SelectedIndexChanged(object sender, EventArgs e)
        {
            Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, DropDownList_city.SelectedValue);
        }

        protected void GridView_list_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string arg = e.CommandArgument?.ToString();

            if (e.CommandName == "_AddRecord")
            {
                setTreeID = arg;
                setCareID = null;
                base.RedirectState("edit.aspx", this.CurrentFilter);
            }

            if (e.CommandName == "_ViewTree")
            {
                setTreeID = arg;
                string targetUrl = ResolveUrl("~/Backstage/tree/detail.aspx");
                string script = $"window.open('{targetUrl}', '_blank');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenTreeWindow", script, true);
            }

            if (e.CommandName == "_ViewRecord")
            {
                setTreeID = arg;
                base.RedirectState("list.aspx", this.CurrentFilter);
            }
        }

        protected void GridView_list_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as CareMainQueryResult;
                if (item != null)
                {
                    if (!item.careID.HasValue || item.careID.Value == 0)
                    {
                        e.Row.CssClass += " table-warning";
                    }
                }
            }
        }
    }
}
