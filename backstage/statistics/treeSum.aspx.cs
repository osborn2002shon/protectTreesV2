using DataAccess;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace protectTreesV2.backstage.statistics
{
    public partial class treeSum : BasePage
    {
        /// <summary>
        /// [查詢條件] 統計項目
        /// "tw" = 依縣市 (預設)
        /// "city" = 依鄉鎮
        /// </summary>
        protected string ViewState_StatItem
        {
            get
            {
                // 若 ViewState 為空，預設回傳 "tw"
                return ViewState["ViewState_StatItem"] as string ?? "tw";
            }
            set
            {
                ViewState["ViewState_StatItem"] = value;
            }
        }
        /// <summary>
        /// [查詢條件] 選擇的縣市
        /// </summary>
        protected string ViewState_FilterCity
        {
            get
            {
                return ViewState["ViewState_FilterCity"] as string ?? string.Empty;
            }
            set
            {
                ViewState["ViewState_FilterCity"] = value;
            }
        }

        /// <summary>
        /// [查詢條件] 選擇的樹籍狀態 (複選)
        /// </summary>
        protected List<int> ViewState_FilterTreeStatus
        {
            get
            {
                return ViewState["ViewState_FilterTreeStatus"] as List<int> ?? new List<int>();
            }
            set
            {
                ViewState["ViewState_FilterTreeStatus"] = value;
            }
        }



        public class ChartSeries
        {
            public string name { get; set; }
            public List<int> data { get; set; }
            public string color { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitSearchFilters();

                UpdateSearchParameters();

                BindData();
            }
        }

        private void InitSearchFilters()
        {
            //縣市
            Base.DropdownBinder.Bind_DropDownList_City(ref DropDownList_citySelect,false);

            //樹籍狀態
            Base.DropdownBinder.Bind_Checkbox_TreeStatus(ref CheckBoxList_treeStatus);
            foreach (ListItem item in CheckBoxList_treeStatus.Items)
            {
                item.Selected = true;
            }
        }

        public DataTable GetChartData(string statType, int? cityId, List<string> statusNames)
        {
            string locationCol = (statType == "tw") ? "T.city" : "T.area";
            string sortCol = (statType == "tw") ? "T.cityID" : "T.twID";

            StringBuilder sql = new StringBuilder();

            sql.Append($@"
                SELECT {locationCol} AS Location, 
                       r.treeStatus AS Status, 
                       COUNT(r.treeID) AS Count
                FROM System_Taiwan T
                LEFT JOIN Tree_Record r ON T.twID = r.areaID 
                     AND r.editStatus = 1 
                     AND r.removeDateTime IS NULL
            ");

            var parameters = new List<SqlParameter>();

            string statusCondition = "";
            if (statusNames != null && statusNames.Count > 0)
            {
                List<string> paramNames = new List<string>();
                for (int i = 0; i < statusNames.Count; i++)
                {
                    string pName = $"@Status{i}";
                    paramNames.Add(pName);
                    parameters.Add(new SqlParameter(pName, statusNames[i]));
                }
                statusCondition = $" AND r.treeStatus IN ({string.Join(",", paramNames)}) ";

                sql.Append(statusCondition);
            }

            sql.Append(" WHERE 1=1 "); // 方便串接

            if (statType == "city" && cityId.HasValue)
            {
                sql.Append(" AND T.cityID = @CityID ");
                parameters.Add(new SqlParameter("@CityID", cityId.Value));
            }

            // GROUP BY
            sql.Append($" GROUP BY {locationCol}, {sortCol}, r.treeStatus ");

            // ORDER BY
            sql.Append($" ORDER BY {sortCol} ASC, Count DESC ");

            using (var da = new MS_SQL())
            {
                return da.GetDataTable(sql.ToString(), parameters.ToArray());
            }
        }

        public DataTable GetSpeciesData(string statType, int? cityId, List<string> statusNames)
        {
            string locationCol = (statType == "tw") ? "T.city" : "T.area";
            string sortCol = (statType == "tw") ? "T.cityID" : "T.twID";

            StringBuilder sql = new StringBuilder();

            sql.Append($@"
                SELECT {locationCol} AS Location, 
                       {sortCol}     AS LocationID,   
                       ISNULL(s.commonName, N'未填寫') AS Species, 
                       ISNULL(s.speciesID, 99999)     AS SpeciesID,
                       COUNT(r.treeID) AS Count
                FROM System_Taiwan T
            ");

            // 準備參數物件
            var parameters = new List<SqlParameter>();

            // ------------------------------------------------------------
            // 處理樹籍狀態 (Status)，必須放在 JOIN 的 ON 裡面
            // ------------------------------------------------------------
            string statusCondition = "";
            if (statusNames != null && statusNames.Count > 0)
            {
                List<string> paramNames = new List<string>();
                for (int i = 0; i < statusNames.Count; i++)
                {
                    string pName = $"@Status{i}";
                    paramNames.Add(pName);
                    parameters.Add(new SqlParameter(pName, statusNames[i]));
                }
                statusCondition = $" AND r.treeStatus IN ({string.Join(",", paramNames)}) ";
            }

            // 串接 JOIN
            // 將 treeStatus 和 editStatus 的條件都寫在 ON 後面
            sql.Append($@"
                LEFT JOIN Tree_Record r ON T.twID = r.areaID 
                          AND r.editStatus = 1 
                          AND r.removeDateTime IS NULL
                          {statusCondition}
                LEFT JOIN Tree_Species s ON r.speciesID = s.speciesID
            ");

            
            sql.Append(" WHERE 1=1 ");

            // 鄉鎮模式：只撈特定縣市的鄉鎮
            if (statType == "city" && cityId.HasValue)
            {
                sql.Append(" AND T.cityID = @CityID ");
                parameters.Add(new SqlParameter("@CityID", cityId.Value));
            }

            // GROUP BY
            sql.Append($" GROUP BY {locationCol}, {sortCol}, s.commonName, s.speciesID ");

            // ORDER BY
            sql.Append($" ORDER BY {sortCol} ASC, s.speciesID ASC ");

            using (var da = new MS_SQL())
            {
                return da.GetDataTable(sql.ToString(), parameters.ToArray());
            }
        }

        /// <summary>
        /// 取得目前選取的樹籍狀態中文名稱清單
        /// </summary>
        private List<string> GetSelectedStatusNames()
        {
            List<int> ids = this.ViewState_FilterTreeStatus;

            // 如果 ViewState 是空的，回傳空 List
            if (ids == null || ids.Count == 0)
            {
                return new List<string>();
            }

            List<string> names = new List<string>();
            foreach (int id in ids)
            {
                names.Add(TreeService.GetStatusText((TreeStatus)id));
            }

            return names;
        }

        private DataTable GetPivotDataTable(string statType, int? cityId, List<string> statusNames)
        {
            // 1. 取得原始資料 (含有 LocationID 和 SpeciesID)
            DataTable dtRaw = GetSpeciesData(statType, cityId, statusNames);
            if (dtRaw == null || dtRaw.Rows.Count == 0) return null;

            DataTable dtPivot = new DataTable();

            // ==========================================
            // 決定欄位順序 (X軸：地點) - 使用 LocationID 排序
            // ==========================================

            // 建立第一欄固定欄位
            dtPivot.Columns.Add("樹種");

            // 取出 [名稱+ID] -> 去重複 -> 依照 ID 排序 -> 只取名稱
            var locations = dtRaw.AsEnumerable()
                                 .Select(r => new {
                                     Name = r["Location"].ToString(),
                                     Id = Convert.ToInt32(r["LocationID"])
                                 })
                                 .Distinct()          // 去除重複的地點
                                 .OrderBy(x => x.Id)  // 這裡強制用 ID 排序 (基隆1 -> 台北2...)
                                 .Select(x => x.Name) // 最後只取名字當欄位名稱
                                 .ToList();

            // 動態建立地點欄位
            foreach (var loc in locations)
            {
                dtPivot.Columns.Add(loc, typeof(string));
            }

            // ==========================================
            // 決定列順序 (Y軸：樹種) - 使用 SpeciesID 排序
            // ==========================================

            // 建立 [合計] 列 (第一列)
            DataRow rowTotal = dtPivot.NewRow();
            rowTotal["樹種"] = "-合計-";
            foreach (var loc in locations)
            {
                var sum = dtRaw.AsEnumerable()
                               .Where(r => r["Location"].ToString() == loc)
                               .Sum(r => Convert.ToInt32(r["Count"]));
                rowTotal[loc] = sum.ToString("N0");
            }
            dtPivot.Rows.Add(rowTotal);

            // 取出 [樹種+ID] -> 去重複 -> 依照 ID 排序
            var speciesList = dtRaw.AsEnumerable()
                                   .Where(r => Convert.ToInt32(r["Count"]) > 0)
                                   .Select(r => new {
                                       Name = r["Species"].ToString(),
                                       Id = Convert.ToInt32(r["SpeciesID"])
                                   })
                                   .Distinct()
                                   .OrderBy(x => x.Id) //這裡強制用 ID 排序 (榕樹 -> 樟樹...)
                                   .ToList();

            // 填入樹種資料
            foreach (var spItem in speciesList)
            {
                DataRow newRow = dtPivot.NewRow();
                newRow["樹種"] = spItem.Name; // 顯示中文名稱

                foreach (var loc in locations)
                {
                    var match = dtRaw.AsEnumerable()
                                     .FirstOrDefault(r => r["Location"].ToString() == loc &&
                                                          r["Species"].ToString() == spItem.Name);

                    int count = match != null ? Convert.ToInt32(match["Count"]) : 0;
                    newRow[loc] = count.ToString("N0");
                }
                dtPivot.Rows.Add(newRow);
            }

            return dtPivot;
        }

        private void BindPivotTable(string statType, int? cityId, List<string> statusNames)
        {
            // 取得資料
            DataTable dtPivot = GetPivotDataTable(statType, cityId, statusNames);

            // 綁定 GridView
            GridView_SpeciesPivot.DataSource = dtPivot;
            GridView_SpeciesPivot.DataBind();

            // 更新查詢時間
            Lit_QueryTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        }



        private void BindChartData(string statType, int? cityId, List<string> statusNames)
        {
            // 取得資料
            DataTable dtStatus = GetChartData(statType, cityId, statusNames);

            if (dtStatus.Rows.Count == 0)
            {
                hf_ChartCategories.Value = "[]";
                hf_ChartSeries.Value = "[]";
                Panel_result.Visible = false;
                return;
            }

            Panel_result.Visible = true;

            // 處理 X 軸 (地點)
            var categories = dtStatus.AsEnumerable()
                                     .Select(r => r["Location"].ToString())
                                     .Distinct()
                                     .ToList();

            // ==========================================
            // 自訂排序邏輯
            // ==========================================

            // 定義您想要的「絕對順序」
            var masterOrder = new List<string> { "已公告列管", "符合標準", "其他" };

            // 取交集：從 masterOrder 裡面挑出 statusNames (使用者勾選的) 有的項目 
            var sortedStatuses = masterOrder.Where(s => statusNames.Contains(s)).ToList();

            // ==========================================
            // 產生 Series
            // ==========================================

            List<ChartSeries> seriesList = new List<ChartSeries>();

            // 定義顏色
            var colorMap = new Dictionary<string, string> {
                { "已公告列管", "#d9534f" }, // 紅色
                { "符合標準", "#f0ad4e" },   // 黃色 
                { "其他", "#999999" }        // 灰色 
            };

            foreach (var status in sortedStatuses)
            {
                List<int> dataPoints = new List<int>();

                foreach (var loc in categories)
                {
                    var row = dtStatus.AsEnumerable()
                        .FirstOrDefault(r => r["Location"].ToString() == loc &&
                                             r["Status"].ToString() == status);

                    dataPoints.Add(row != null ? Convert.ToInt32(row["Count"]) : 0);
                }

                seriesList.Add(new ChartSeries
                {
                    name = status,
                    data = dataPoints,
                    // 如果字典裡有定義顏色就用，沒有就讓 Highcharts 自動配
                    color = colorMap.ContainsKey(status) ? colorMap[status] : null
                });
            }

            // 4. 輸出 JSON
            hf_ChartCategories.Value = JsonConvert.SerializeObject(categories);
            hf_ChartSeries.Value = JsonConvert.SerializeObject(seriesList);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CallDrawChart", "drawHighchart();", true);
        }
        private void BindData()
        {
            // 統計項目 (依縣市 "tw" 或 依鄉鎮 "city")
            string statType = this.ViewState_StatItem;

            // 縣市 ID 
            int? filterCityId = null;
            if (int.TryParse(this.ViewState_FilterCity, out int cId))
            {
                filterCityId = cId;
            }

            // 如果是「依縣市」模式，強制設為 null (撈全國)
            if (statType == "tw") filterCityId = null;

            // 樹籍狀態 (List<int>)
            List<string> searchStatusNames = GetSelectedStatusNames();

            BindChartData(statType, filterCityId, searchStatusNames);

            BindPivotTable(statType, filterCityId, searchStatusNames);
        }

        /// <summary>
        /// 將畫面上的搜尋條件儲存到 ViewState
        /// </summary>
        private void UpdateSearchParameters()
        {
            // 決定統計模式
            if (RadioButton_city.Checked)
            {
                ViewState_StatItem = "city";
            }
            else
            {
                ViewState_StatItem = "tw";
            }

            // 儲存縣市
            ViewState_FilterCity = DropDownList_citySelect.SelectedValue;

            // 儲存樹籍狀態 
            List<int> selectedStatus = new List<int>();
            foreach (ListItem item in CheckBoxList_treeStatus.Items)
            {
                if (item.Selected)
                {
                    if (int.TryParse(item.Value, out int val))
                    {
                        selectedStatus.Add(val);
                    }
                }
            }
            ViewState_FilterTreeStatus = selectedStatus;
        }

        protected void LinkButton_query_Click(object sender, EventArgs e)
        {
            if (CheckBoxList_treeStatus.SelectedIndex == -1)
            {
                ShowMessage("提示", "請至少勾選一種樹籍狀態");
                return;
            }

            UpdateSearchParameters();

            BindData();
        }
      
        protected void GridView_SpeciesPivot_PreRender(object sender, EventArgs e)
        {
            // 確保 GridView 有資料且有標題列
            if (GridView_SpeciesPivot.Rows.Count > 0 && GridView_SpeciesPivot.HeaderRow != null)
            {
                // 產生 <thead> 標籤
                GridView_SpeciesPivot.HeaderRow.TableSection = TableRowSection.TableHeader;

                // 確保標題儲存格是用 <th> 
                GridView_SpeciesPivot.UseAccessibleHeader = true;
            }
        }

        protected void GridView_SpeciesPivot_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 取得第一格的文字 (樹種名稱)
                string speciesName = e.Row.Cells[0].Text;

                // 如果是 "-合計-" 這一列，加上特殊 CSS 樣式
                if (speciesName == "-合計-")
                {
                    e.Row.CssClass = "row-total"; // 對應前端 CSS
                }
            }
        }

        protected void LinkButton_exportExcel_Click(object sender, EventArgs e)
        {
            // ==========================================
            // 準備查詢參數
            // ==========================================

            // 統計項目
            string statType = this.ViewState_StatItem;

            // 縣市 ID
            int? filterCityId = null;
            if (int.TryParse(this.ViewState_FilterCity, out int cId))
            {
                filterCityId = cId;
            }
            // 如果是「依縣市」模式，強制設為 null (撈全國)
            if (statType == "tw") filterCityId = null;

            // 樹籍狀態 
            List<string> searchStatusNames = GetSelectedStatusNames();

            // 取得 GridView 相同的資料源
            DataTable dt = GetPivotDataTable(statType, filterCityId, searchStatusNames);

            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }

            // 建立 Excel 工作伯
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("樹種統計表");

            // ==========================================
            //  設定樣式 
            // ==========================================
            string fontName = "標楷體";
            short fontHeight = 12; // 字體大小

            // 標題樣式 
            ICellStyle headerStyle = workbook.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.Center;
            IFont headerFont = workbook.CreateFont();
            headerFont.FontName = fontName; 
            headerFont.FontHeightInPoints = fontHeight;
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            // 合計列樣式
            ICellStyle totalRowStyle = workbook.CreateCellStyle();
            totalRowStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRowStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            IFont totalFont = workbook.CreateFont();
            totalFont.FontName = fontName; 
            totalFont.FontHeightInPoints = fontHeight;
            totalFont.IsBold = true;
            totalRowStyle.SetFont(totalFont);
            // 設定數值格式為千分位 "#,##0"
            IDataFormat dataFormat = workbook.CreateDataFormat();
            totalRowStyle.DataFormat = dataFormat.GetFormat("#,##0");

            // 一般資料樣式
            ICellStyle dataStyle = workbook.CreateCellStyle();
            dataStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            dataStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            dataStyle.DataFormat = dataFormat.GetFormat("#,##0");

            IFont dataFont = workbook.CreateFont();
            dataFont.FontName = fontName;
            dataFont.FontHeightInPoints = fontHeight;
            dataFont.IsBold = false;
            dataStyle.SetFont(dataFont);

            // ==========================================
            // 寫入資料
            // ==========================================

            //寫入標題列 (Header)
            IRow excelHeaderRow = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = excelHeaderRow.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
                cell.CellStyle = headerStyle;
            }

            //  寫入內容列 
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                IRow excelRow = sheet.CreateRow(i + 1);

                // 判斷是否為「合計」列 
                bool isTotalRow = row[0].ToString() == "-合計-";

                // 選擇對應的樣式
                ICellStyle currentStyle = isTotalRow ? totalRowStyle : dataStyle;

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = excelRow.CreateCell(j);
                    string valStr = row[j].ToString();

                    // 第一欄 (樹種名稱) 永遠是文字
                    if (j == 0)
                    {
                        cell.SetCellValue(valStr);

                        // 針對第一欄文字
                        ICellStyle textStyle = workbook.CreateCellStyle();
                        textStyle.CloneStyleFrom(currentStyle); // 繼承底色
                        textStyle.DataFormat = 0; // 清除數值格式
                        cell.CellStyle = textStyle;
                    }
                    else
                    {

                        if (double.TryParse(valStr.Replace(",", ""), out double numVal))
                        {
                            cell.SetCellValue(numVal); // 存入數字
                        }
                        else
                        {
                            cell.SetCellValue(0);
                        }
                        cell.CellStyle = currentStyle; // 套用含千分位的樣式
                    }
                }
            }

            // 自動調整欄寬
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i == 0)
                {
                    // 第一欄：樹種名稱
                    sheet.SetColumnWidth(i, 20 * 256);
                }
                else
                {
                    // 其他欄位：縣市數據
                    sheet.SetColumnWidth(i, 10 * 256);
                }
            }

            // ==========================================
            // 輸出檔案到瀏覽器
            // ==========================================
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);

                string fileName = $"樹種數量統計_{DateTime.Now:yyyyMMddHHmm}.xlsx";

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // 使用 UrlEncode 避免中文檔名亂碼
                Response.AddHeader("Content-Disposition", $"attachment; filename={System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)}");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }
    }
}