using System;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using static protectTreesV2.Health.Health;
using System.Security.Cryptography;

namespace protectTreesV2.backstage.health
{
    public partial class main : protectTreesV2.Base.BasePage
    {
        public protectTreesV2.Health.Health system_health = new protectTreesV2.Health.Health();

        protected HealthMainQueryFilter CurrentFilter
        {
            get
            {
                // 如果 ViewState 裡沒有，就回傳一個空的(或預設的)
                return ViewState["CurrentFilter"] as HealthMainQueryFilter ?? new HealthMainQueryFilter();
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

                //取得搜尋紀錄
                var savedFilter = base.GetState<HealthMainQueryFilter>();
                if (savedFilter != null)
                {
                    // 把值填回 UI 
                    PopulateFilterToUI(savedFilter);

                    // 同步更新 CurrentFilter
                    CurrentFilter = savedFilter;
                }

                BindResult();
                BindSelectedList(); // 載入下方已加入的暫存清單
            }

        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitSearchFilters()
        {
            //縣市
            Base.DropdownBinder.Bind_DropDownList_City(ref DropDownList_city);

            //鄉鎮
            Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area,DropDownList_city.SelectedValue);

            //樹種
            Base.DropdownBinder.Bind_DropDownList_Species(ref DropDownList_species);
        }
        private void PopulateFilterToUI(HealthMainQueryFilter filter)
        {
            // 還原縣市
            if (filter.cityID.HasValue)
            {
                string cityVal = filter.cityID.ToString();
                if (DropDownList_city.Items.FindByValue(cityVal) != null)
                {
                    DropDownList_city.SelectedValue = cityVal;
                    Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, cityVal);
                }
            }

            // 還原鄉鎮
            if (filter.areaID.HasValue)
            {
                // 檢查選單內是否有該值
                if (DropDownList_area.Items.FindByValue(filter.areaID.ToString()) != null)
                {
                    DropDownList_area.SelectedValue = filter.areaID.ToString();
                }
            }

            // 還原樹種
            if (filter.speciesID.HasValue)
            {
                string speciesVal = filter.speciesID.ToString();
                if (DropDownList_species.Items.FindByValue(speciesVal) != null)
                {
                    DropDownList_species.SelectedValue = speciesVal;
                }
            }

            TextBox_keyword.Text = filter.keyword;
            CheckBox_onlyNoRecord.Checked = filter.onlyNoRecord;
            CheckBox_includeDraft.Checked = filter.includeDraft;
        }
        private void CollectFilterFromUI()
        {
            var filter = new HealthMainQueryFilter();

            if (int.TryParse(DropDownList_city.SelectedValue, out int cityId))
                filter.cityID = cityId;

            if (int.TryParse(DropDownList_area.SelectedValue, out int areaId))
                filter.areaID = areaId;

            if (int.TryParse(DropDownList_species.SelectedValue, out int speciesId))
                filter.speciesID = speciesId;

            filter.keyword = TextBox_keyword.Text.Trim();
            filter.onlyNoRecord = CheckBox_onlyNoRecord.Checked;
            filter.includeDraft = CheckBox_includeDraft.Checked;

            // 存入 ViewState
            CurrentFilter = filter;
        }

        private void BindResult()
        {
            // 從 ViewState 拿出上次的篩選條件
            var filter = CurrentFilter;

            // 把目前的排序狀態填進去 
            filter.sortExpression = SortExpression;
            filter.sortDirection = SortDirection;

            // 取得資料
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;
            List<HealthMainQueryResult> data = system_health.GetHealthMainList(filter, accountID);

            // 更新筆數顯示
            Label_recordCount.Text = data != null ? data.Count.ToString() : "0";

            //  綁定
            GridView_list.DataSource = data;
            GridView_list.DataBind();
        }
        protected void DropDownList_city_SelectedIndexChanged(object sender, EventArgs e)
        {
            Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, DropDownList_city.SelectedValue);
        }

        protected void LinkButton_search_Click(object sender, EventArgs e)
        {
            // 1. 鎖定當前 UI 條件
            CollectFilterFromUI();

            // 2. 搜尋時通常要回到第一頁
            GridView_list.PageIndex = 0;
            SortExpression = null;
            SortDirection = null;

            // 3. 撈資料
            BindResult();
        }

        protected void GridView_list_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
        {
            // 如果點擊同一個欄位，切換排序方向
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

        // 分頁
        protected void GridView_list_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            GridView_list.PageIndex = e.NewPageIndex;
            BindResult();
        }

       
        protected void GridView_list_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string arg = e.CommandArgument?.ToString();

            // A. 加入上傳清單
            if (e.CommandName == "_AddToUpload")
            {
                int treeId = Convert.ToInt32(arg);

                var user = UserInfo.GetCurrentUser;
                int accountID = user?.accountID ?? 0;

                try
                {
                    system_health.AddToHealthBatchSetting(accountID, treeId);
                    BindSelectedList();
                    BindResult();
                    ShowMessage("提示", "加入成功");
                }
                catch (Exception ex)
                {
                    // 捕捉錯誤並顯示 (例如資料庫連線失敗)
                    // ex.Message 會顯示具體的錯誤原因
                    ShowMessage("提示", "加入失敗：" + ex.Message);
                }
            }

            // B. 新增紀錄 (跳轉)
            if (e.CommandName == "_AddRecord")
            {
                // 設定樹木 ID
                setTreeID = arg;

                // 清空健檢 ID 
                setHealthID = null;

                // 跳轉至新增頁
                base.RedirectState("edit.aspx", this.CurrentFilter);
            }

            // C. 檢視樹籍 (開新視窗)
            if (e.CommandName == "_ViewTree")
            {
                // 透過 Session 
                setTreeID = arg;

                // 使用 ScriptManager 開啟新視窗
                string targetUrl = ResolveUrl($"~/Backstage/tree/detail.aspx");
                string script = $"window.open('{targetUrl}', '_blank');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenTreeWindow", script, true);
            }

            // D. 檢視紀錄 (查詢歷史)
            if (e.CommandName == "_ViewRecord")
            {
                setTreeID = arg;
                Response.Redirect("list.aspx");

            }
        }

        protected void GridView_list_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as HealthMainQueryResult;

                if (item != null)
                {
                    // 判斷條件：沒有健檢紀錄 (healthID 為 null 或 0)
                    if (!item.healthID.HasValue || item.healthID.Value == 0)
                    {
                        // 設定整列背景色 (建議使用 Bootstrap class 或自定義 CSS)
                        // table-warning (黃色系), table-light (灰色系)
                        e.Row.CssClass += " table-warning";

                        // 或者直接設定 Style
                        // e.Row.BackColor = System.Drawing.Color.LightYellow;
                    }
                }
            }
        }

        private void BindSelectedList()
        {
            // 1. 取得目前使用者 ID
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            try
            {
                // 2. 取得清單
                List<HealthBatchSettingResult> list = system_health.GetHealthBatchSetting(accountID);

                // 3. 綁定資料
                GridView_selectedList.DataSource = list;
                GridView_selectedList.DataBind();
            }
            catch (Exception ex)
            {
                GridView_selectedList.DataSource = null;
                GridView_selectedList.DataBind();
                ShowMessage("提示", "讀取設定清單失敗：" + ex.Message);
            }
        }

        private void CreateCell(IRow row, int cellIndex, string value)
        {
            // 若該格原本就有，就 Get，沒有就 Create
            ICell cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);

            // 設定值 (處理 null)
            cell.SetCellValue(value ?? string.Empty);
        }

        protected void GridView_selectedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // 判斷動作是否為移除
            if (e.CommandName == "_Remove")
            {
                // 1. 取得傳入的 TreeID
                int treeId = Convert.ToInt32(e.CommandArgument);

                // 2. 取得目前使用者 ID
                var user = UserInfo.GetCurrentUser;
                int accountID = user?.accountID ?? 0;

                try
                {
                    // 3. 呼叫 Service 執行移除 (傳入 treeId 代表只刪除這一筆)
                    system_health.RemoveFromHealthBatchSetting(accountID, treeId);

                    // 4. 重新綁定列表 (畫面更新)
                    BindSelectedList();
                    BindResult();

                    // 5. 提示成功
                    ShowMessage("提示", "移除成功");
                }
                catch (Exception ex)
                {
                    ShowMessage("提示", "移除失敗：" + ex.Message);
                }
            }
        }

        protected void LinkButton_clearList_Click(object sender, EventArgs e)
        {
            // 1. 取得目前使用者 ID
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            try
            {
                // 2. 執行清空
                system_health.RemoveFromHealthBatchSetting(accountID, null);

                // 3. 重新綁定列表 
                BindSelectedList();
                BindResult();

                // 4. 提示成功
                ShowMessage("提示", "已清空列表");
            }
            catch (Exception ex)
            {
                ShowMessage("提示", "清空失敗：" + ex.Message);
            }
        }

        protected void LinkButton_generateExcel_Click(object sender, EventArgs e)
        {
            // 取得使用者與資料
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            try
            {
                var list = system_health.GetHealthBatchSetting(accountID);

                if (list == null || list.Count == 0)
                {
                    ShowMessage("提示", "清單目前沒有任何資料，請先加入樹籍後再產製。");
                    return;
                }

                // 設定範本路徑
                string templatePath = Server.MapPath("~/_doc/受保護樹木健檢資料蒐集用表格.xlsx");
                if (!File.Exists(templatePath))
                {
                    ShowMessage("提示", "找不到 Excel 範本檔案。");
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
                    int startRowIndex = 2; // 第3列
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        int currentRowIdx = startRowIndex + i;

                        // 取得或建立列
                        IRow row = sheetBasic.GetRow(currentRowIdx) ?? sheetBasic.CreateRow(currentRowIdx);

                        CreateCell(row, 0, (i + 1).ToString()); // 次序
                        CreateCell(row, 1, item.systemTreeNo);  // 樹籍編號
                        CreateCell(row, 2, item.agencyTreeNo);  // 機關樹木編號
                        CreateCell(row, 3, item.cityName);      // 縣市
                        CreateCell(row, 4, item.areaName);      // 鄉鎮
                        CreateCell(row, 5, item.speciesName);   // 樹種
                    }
                }

                ISheet sheetHealth = workbook.GetSheet("健康檢查結果及風險評估");
                if (sheetHealth != null)
                {
                    int startRowIndex = 2; // 第3列
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        int currentRowIdx = startRowIndex + i;

                        IRow row = sheetHealth.GetRow(currentRowIdx) ?? sheetHealth.CreateRow(currentRowIdx);

                        CreateCell(row, 0, (i + 1).ToString()); // 次序
                        CreateCell(row, 1, item.systemTreeNo);  // 樹籍編號
                        CreateCell(row, 2, item.speciesName);   // 樹種 
                    }
                }

                string[] otherSheets = new string[] {
                    "病蟲害調查",
                    "樹木生長外觀情況",
                    "樹木修剪與支撐情況",
                    "生育地環境與土讓檢測情況"
                };

                foreach (string sheetName in otherSheets)
                {
                    ISheet sheet = workbook.GetSheet(sheetName);
                    if (sheet != null)
                    {
                        int startRowIndex = 3; // 第4列
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            int currentRowIdx = startRowIndex + i;

                            IRow row = sheet.GetRow(currentRowIdx) ?? sheet.CreateRow(currentRowIdx);

                            CreateCell(row, 0, (i + 1).ToString()); // 次序
                            CreateCell(row, 1, item.systemTreeNo);  // 樹籍編號
                            CreateCell(row, 2, item.speciesName);   // 樹種
                        }
                    }
                }

                // 3. 輸出下載
                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    string fileName = $"健檢資料蒐集表_{DateTime.Now:yyyyMMddHHmm}.xlsx";

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    // 設定檔名 (相容性寫法)
                    string encodedFileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                    Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");

                    Response.BinaryWrite(ms.ToArray());

                    // 使用 Response.End() 強制結束
                    Response.End();
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // 這是 Response.End() 觸發的正常中斷，不需處理，也不要顯示錯誤
            }
            catch (Exception ex)
            {
                ShowMessage("提示", "產製失敗：" + ex.Message);
            }
        }
    }
}