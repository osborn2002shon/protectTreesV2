using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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
                BindSelectedList();
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

            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

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

            if (e.CommandName == "_AddToUpload")
            {
                int treeId = Convert.ToInt32(arg);
                var user = UserInfo.GetCurrentUser;
                int accountID = user?.accountID ?? 0;

                try
                {
                    system_care.AddToCareBatchSetting(accountID, treeId);
                    BindSelectedList();
                    BindResult();
                    ShowMessage("提示", "加入成功");
                }
                catch (Exception ex)
                {
                    ShowMessage("錯誤", "加入失敗：" + ex.Message);
                }
            }

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

        private void BindSelectedList()
        {
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            try
            {
                List<CareBatchSettingResult> list = system_care.GetCareBatchSetting(accountID);
                GridView_selectedList.DataSource = list;
                GridView_selectedList.DataBind();
            }
            catch (Exception ex)
            {
                GridView_selectedList.DataSource = null;
                GridView_selectedList.DataBind();
                ShowMessage("錯誤", "讀取設定清單失敗：" + ex.Message);
            }
        }

        private void CreateCell(IRow row, int cellIndex, string value)
        {
            ICell cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);
            cell.SetCellValue(value ?? string.Empty);
        }

        protected void GridView_selectedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "_Remove")
            {
                int treeId = Convert.ToInt32(e.CommandArgument);
                var user = UserInfo.GetCurrentUser;
                int accountID = user?.accountID ?? 0;

                try
                {
                    system_care.RemoveFromCareBatchSetting(accountID, treeId);
                    BindSelectedList();
                    BindResult();
                    ShowMessage("提示", "移除成功");
                }
                catch (Exception ex)
                {
                    ShowMessage("錯誤", "移除失敗：" + ex.Message);
                }
            }
        }

        protected void LinkButton_clearList_Click(object sender, EventArgs e)
        {
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            try
            {
                system_care.RemoveFromCareBatchSetting(accountID, null);
                BindSelectedList();
                BindResult();
                ShowMessage("提示", "已清空列表");
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "清空失敗：" + ex.Message);
            }
        }

        protected void LinkButton_generateExcel_Click(object sender, EventArgs e)
        {
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            try
            {
                var list = system_care.GetCareBatchSetting(accountID);
                if (list == null || list.Count == 0)
                {
                    ShowMessage("警告", "清單目前沒有任何資料，請先加入樹籍後再產製。");
                    return;
                }

                string templatePath = Server.MapPath("~/_doc/受保護樹木養護資料蒐集用表格.xlsx");
                if (!File.Exists(templatePath))
                {
                    ShowMessage("錯誤", "找不到 Excel 範本檔案。");
                    return;
                }

                IWorkbook workbook;
                using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(fs);
                }

                ISheet sheetBasic = workbook.GetSheet("基本資料");
                if (sheetBasic != null)
                {
                    int startRowIndex = 2;
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        int currentRowIdx = startRowIndex + i;
                        IRow row = sheetBasic.GetRow(currentRowIdx) ?? sheetBasic.CreateRow(currentRowIdx);

                        CreateCell(row, 0, (i + 1).ToString());
                        CreateCell(row, 1, item.systemTreeNo);
                        CreateCell(row, 2, item.agencyTreeNo);
                        CreateCell(row, 3, item.cityName);
                        CreateCell(row, 4, item.areaName);
                        CreateCell(row, 5, item.speciesName);
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    string fileName = $"養護資料蒐集表_{DateTime.Now:yyyyMMddHHmm}.xlsx";

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    string encodedFileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                    Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");

                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ShowMessage("錯誤", "產製失敗：" + ex.Message);
            }
        }
    }
}
