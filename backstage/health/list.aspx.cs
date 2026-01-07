using protectTreesV2.backstage.Manage;
using protectTreesV2.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static protectTreesV2.Health.Health;

namespace protectTreesV2.backstage.health
{
    public partial class list : Base.BasePage
    {
        public protectTreesV2.Health.Health system_health = new protectTreesV2.Health.Health();
       
        protected HealthRecordListFilter CurrentFilter
        {
            get
            {
                return ViewState["CurrentFilter"] as HealthRecordListFilter ?? new HealthRecordListFilter();
            }
            set { ViewState["CurrentFilter"] = value; }
        }

        /// <summary>
        /// 排序欄位
        /// 預設值為 "DefaultSort"， (調查日期 DESC + 樹號 DESC)
        /// </summary>
        protected string SortExpression
        {
            get { return ViewState["SortExpression"] as string ?? "DefaultSort"; }
            set { ViewState["SortExpression"] = value; }
        }

        /// <summary>
        /// 排序方向 (預設 DESC)
        /// </summary>
        protected string SortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "DESC"; }
            set { ViewState["SortDirection"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //初始化
                InitSearchFilters();

                var savedFilter = base.GetState<HealthRecordListFilter>();

                if (savedFilter != null)
                {
                    // 從編輯頁返回 -> 還原舊條件
                    PopulateFilterToUI(savedFilter);
                    CurrentFilter = savedFilter;
                }
                else
                {
                    // 從健檢紀錄:樹籍帶參數進入
                    CheckExternalRequest();

                    // 收集 UI 目前的值
                    CollectFilterFromUI();
                }

                // 載入資料
                BindResult();
            }
        }

        private void CheckExternalRequest()
        {
            // 檢查 Session 是否有值
            if (!string.IsNullOrEmpty(setTreeID))
            {
                int targetId;
                if (int.TryParse(setTreeID, out targetId))
                {
                    // 透過 ID 查出 "系統樹籍編號" 
                    var tree = TreeCatalog.TreeService.GetTree(targetId);
                    string treeNo = tree.SystemTreeNo;

                    if (!string.IsNullOrEmpty(treeNo))
                    {
                        // 將編號填入 "關鍵字" 欄位
                        TextBox_keyword.Text = treeNo;

                        // 將查詢範圍切換為 "單位全部" 
                        RadioButtonList_scope.SelectedValue = "Unit";

                        // 清空日期
                        TextBox_dateStart.Text = "";
                        TextBox_dateEnd.Text = "";
                    }
                }

                // 清除 Session
                setTreeID = null;
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
            Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, DropDownList_city.SelectedValue);

            //樹種
            Base.DropdownBinder.Bind_DropDownList_Species(ref DropDownList_species);
        }

        /// <summary>
        /// 將 Filter 的值填回 UI 控制項 (還原搜尋條件)
        /// </summary>
        private void PopulateFilterToUI(HealthRecordListFilter filter)
        {
            // 查詢範圍
            if (!string.IsNullOrEmpty(filter.scope))
            {
                if (RadioButtonList_scope.Items.FindByValue(filter.scope) != null)
                    RadioButtonList_scope.SelectedValue = filter.scope;
            }

            // 縣市
            if (filter.cityID.HasValue)
            {
                string cityVal = filter.cityID.ToString();
                if (DropDownList_city.Items.FindByValue(cityVal) != null)
                {
                    DropDownList_city.SelectedValue = cityVal;
                    Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, cityVal);
                }
            }

            // 鄉鎮
            if (filter.areaID.HasValue)
            {
                string areaVal = filter.areaID.ToString();
                if (DropDownList_area.Items.FindByValue(areaVal) != null)
                    DropDownList_area.SelectedValue = areaVal;
            }

            // 樹種
            if (filter.speciesID.HasValue)
            {
                string speciesVal = filter.speciesID.ToString();
                if (DropDownList_species.Items.FindByValue(speciesVal) != null)
                    DropDownList_species.SelectedValue = speciesVal;
            }

            // 日期區間
            if (filter.dateStart.HasValue)
                TextBox_dateStart.Text = filter.dateStart.Value.ToString("yyyy-MM-dd");

            if (filter.dateEnd.HasValue)
                TextBox_dateEnd.Text = filter.dateEnd.Value.ToString("yyyy-MM-dd");

            // 關鍵字
            TextBox_keyword.Text = filter.keyword;
        }
        private void CollectFilterFromUI()
        {
            var filter = new HealthRecordListFilter();

            // 1. 查詢範圍 (My=我的紀錄, Unit=單位全部)
            filter.scope = RadioButtonList_scope.SelectedValue;

            // 2. 地區與樹種
            if (int.TryParse(DropDownList_city.SelectedValue, out int city)) filter.cityID = city;
            if (int.TryParse(DropDownList_area.SelectedValue, out int area)) filter.areaID = area;
            if (int.TryParse(DropDownList_species.SelectedValue, out int species)) filter.speciesID = species;

            // 3. 日期區間
            if (DateTime.TryParse(TextBox_dateStart.Text, out DateTime start)) filter.dateStart = start;
            if (DateTime.TryParse(TextBox_dateEnd.Text, out DateTime end)) filter.dateEnd = end;

            // 4. 關鍵字
            filter.keyword = TextBox_keyword.Text.Trim();

            // 存入屬性
            CurrentFilter = filter;
        }

        // 執行查詢並綁定 GridView
        private void BindResult()
        {
            // 1. 從 ViewState 取出條件
            var filter = CurrentFilter;
            filter.sortExpression = SortExpression;
            filter.sortDirection = SortDirection;

            // 2. 取得當前使用者 ID
            var user = UserService.GetCurrentUser();
            int userId = user?.userID ?? 0;

            // 3. 呼叫 Service 
            List<HealthMainQueryResult> data = system_health.GetHealthRecordList(filter, userId);

            // 4. 更新筆數顯示
            Label_recordCount.Text = data != null ? data.Count.ToString() : "0";

            // 5. 綁定
            GridView_healthList.DataSource = data;
            GridView_healthList.DataBind();
        }

        protected void DropDownList_city_SelectedIndexChanged(object sender, EventArgs e)
        {
            Base.DropdownBinder.Bind_DropDownList_Area(ref DropDownList_area, DropDownList_city.SelectedValue);
        }
        protected void LinkButton_search_Click(object sender, EventArgs e)
        {
            CollectFilterFromUI();

            // 查詢時重置分頁與排序
            GridView_healthList.PageIndex = 0;
            SortExpression = null;
            SortDirection = null;

            BindResult();
        }

        protected void GridView_healthList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_healthList.PageIndex = e.NewPageIndex;
            BindResult();
        }

        protected void GridView_healthList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // 取得ID
            string healthStr = e.CommandArgument.ToString();

            // 檢視 (開新視窗)
            if (e.CommandName == "_ViewHealth")
            {
                int healthId = Convert.ToInt32(healthStr);
                var record = system_health.GetHealthRecord(healthId);
                uc_healthRecordModal.BindRecord(record);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "showHealthRecordModal();", true);
            }
            // 編輯 (跳轉)
            else if (e.CommandName == "_EditHealth")
            {
                // 設定健檢 ID 
                setHealthID = healthStr;

                // 清空樹木 ID 
                setTreeID = null;

                // 跳轉至編輯頁
                base.RedirectState("edit.aspx", this.CurrentFilter);
            }
        }

        protected void GridView_healthList_Sorting(object sender, GridViewSortEventArgs e)
        {
            // 如果點擊的是同一個欄位，反轉方向
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

       
    }
}