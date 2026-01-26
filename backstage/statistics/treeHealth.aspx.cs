using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static protectTreesV2.Base.DropdownBinder;
using Newtonsoft.Json;
using protectTreesV2.Health;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace protectTreesV2.backstage.statistics
{
    public partial class treeHealth : protectTreesV2.Base.BasePage
    {
        #region ViewState 屬性 (查詢條件)

        /// <summary>
        /// [查詢條件] 起始年份
        /// </summary>
        protected string ViewState_StartYear
        {
            get
            {
                return ViewState["ViewState_StartYear"] as string ?? DateTime.Now.Year.ToString();
            }
            set
            {
                ViewState["ViewState_StartYear"] = value;
            }
        }

        /// <summary>
        /// [查詢條件] 起始月份
        /// </summary>
        protected string ViewState_StartMonth
        {
            get
            {
                return ViewState["ViewState_StartMonth"] as string ?? "01";
            }
            set
            {
                ViewState["ViewState_StartMonth"] = value;
            }
        }

        /// <summary>
        /// [查詢條件] 結束年份
        /// </summary>
        protected string ViewState_EndYear
        {
            get
            {
                return ViewState["ViewState_EndYear"] as string ?? DateTime.Now.Year.ToString();
            }
            set
            {
                ViewState["ViewState_EndYear"] = value;
            }
        }

        /// <summary>
        /// [查詢條件] 結束月份
        /// </summary>
        protected string ViewState_EndMonth
        {
            get
            {
                // 預設當前月份
                return ViewState["ViewState_EndMonth"] as string ?? DateTime.Now.Month.ToString("00");
            }
            set
            {
                ViewState["ViewState_EndMonth"] = value;
            }
        }

        #endregion
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
        /// <summary>
        /// 初始化搜尋條件 
        /// </summary>
        private void InitSearchFilters()
        {
            //綁定年份
            Base.DropdownBinder.Bind_DropDownList_Year(ref DropDownList_startYear, RecordType.Health, false);
            Base.DropdownBinder.Bind_DropDownList_Year(ref DropDownList_endYear, RecordType.Health, false);

            // 綁定月份
            Base.DropdownBinder.Bind_DropDownList_Month(ref DropDownList_startMonth, false);
            Base.DropdownBinder.Bind_DropDownList_Month(ref DropDownList_endMonth, false);
        }
        /// <summary>
        /// 將畫面上的搜尋條件儲存到 ViewState
        /// (若起始時間晚於結束時間，會自動交換)
        /// </summary>
        private void UpdateSearchParameters()
        {
            // 先暫存畫面上的值
            string startYear = DropDownList_startYear.SelectedValue;
            string startMonth = DropDownList_startMonth.SelectedValue;
            string endYear = DropDownList_endYear.SelectedValue;
            string endMonth = DropDownList_endMonth.SelectedValue;

            //  轉換為 DateTime 進行比較
            DateTime startDate = new DateTime(int.Parse(startYear), int.Parse(startMonth), 1);
            DateTime endDate = new DateTime(int.Parse(endYear), int.Parse(endMonth), 1);

            // 3. 判斷：若 起始 > 結束，則進行交換
            if (startDate > endDate)
            {
                // 交換變數值
                string tempYear = startYear;
                string tempMonth = startMonth;

                startYear = endYear;
                startMonth = endMonth;

                endYear = tempYear;
                endMonth = tempMonth;

                //同時更新畫面上的下拉選單，讓 UI 跟資料一致
                DropDownList_startYear.SelectedValue = startYear;
                DropDownList_startMonth.SelectedValue = startMonth;
                DropDownList_endYear.SelectedValue = endYear;
                DropDownList_endMonth.SelectedValue = endMonth;
            }

            // 存入 ViewState
            ViewState_StartYear = startYear;
            ViewState_StartMonth = startMonth;
            ViewState_EndYear = endYear;
            ViewState_EndMonth = endMonth;
        }

        /// <summary>
        /// 取得所有建議處理優先順序的名稱清單
        /// </summary>
        private List<string> TargetPriorities
        {
            get
            {
                return Enum.GetNames(typeof(Health.Health.enum_treatmentPriority)).ToList();
            }
        }

        public DataTable GetHealthPriorityData(string startYear, string startMonth, string endYear, string endMonth)
        {
            StringBuilder sql = new StringBuilder();
            string statusList = string.Join(",", TargetPriorities.Select(x => $"'{x}'"));
            sql.Append($@"
                SELECT T.city            AS Location, 
                       T.cityID          AS LocationID,    
                       ISNULL(H.priority, '無資料') AS Priority, 
                       COUNT(H.healthID) AS Count
                FROM System_Taiwan T
                LEFT JOIN Tree_Record R ON T.twID = R.areaID 
                      AND R.removeDateTime IS NULL 
                      AND R.editStatus = 1
                LEFT JOIN Tree_HealthRecord H ON R.treeID = H.treeID 
                      AND H.removeDateTime IS NULL 
                      AND H.dataStatus = 1
                      AND H.priority IN ({statusList})
                      AND H.surveyDate >= @StartDate 
                      AND H.surveyDate < @EndDate
            ");

            var parameters = new List<SqlParameter>();

            // ==========================================
            // 準備日期參數
            // ==========================================
            string startDateStr = $"{startYear}/{startMonth}/01";
            DateTime endDate = Convert.ToDateTime($"{endYear}/{endMonth}/01").AddMonths(1);

            parameters.Add(new SqlParameter("@StartDate", Convert.ToDateTime(startDateStr)));
            parameters.Add(new SqlParameter("@EndDate", endDate));

            // ==========================================
            // 分組與排序
            // ==========================================
            sql.Append(" GROUP BY T.city, T.cityID, H.priority ");
            sql.Append(" ORDER BY T.cityID ASC ");

            using (var da = new DataAccess.MS_SQL())
            {
                return da.GetDataTable(sql.ToString(), parameters.ToArray());
            }
        }
        private void BindChartData(DataTable dtRaw)
        {
            // 若無資料，隱藏圖表區塊
            if (dtRaw == null || dtRaw.Rows.Count == 0)
            {
                hf_ChartCategories.Value = "[]";
                hf_ChartSeries.Value = "[]";
                Panel_result.Visible = false;
                return;
            }

            Panel_result.Visible = true;

            // 處理 X 軸 (地點 - 縣市)
            // 取出所有出現的縣市，並依照 ID 排序
            var categories = dtRaw.AsEnumerable()
                                  .Select(r => new {
                                      Name = r["Location"].ToString(),
                                      Id = Convert.ToInt32(r["LocationID"])
                                  })
                                  .Distinct()
                                  .OrderBy(x => x.Id)
                                  .Select(x => x.Name)
                                  .ToList();

            // 定義 Series
            var targetPriorities = this.TargetPriorities;

            // 定義顏色
            var colorMap = new Dictionary<string, string> {
                {Health.Health.enum_treatmentPriority.緊急處理.ToString(),"#d9534f" },
                { Health.Health.enum_treatmentPriority.優先處理.ToString(), "#f0ad4e" },
                { Health.Health.enum_treatmentPriority.例行養護.ToString(), "#5cb85c" }
            };

            List<ChartSeries> seriesList = new List<ChartSeries>();

            foreach (var priority in targetPriorities)
            {
                List<int> dataPoints = new List<int>();

                foreach (var loc in categories)
                {
                    // 找出該縣市、該優先順序的數量
                    var row = dtRaw.AsEnumerable()
                                   .FirstOrDefault(r => r["Location"].ToString() == loc &&
                                                        r["Priority"].ToString() == priority);

                    dataPoints.Add(row != null ? Convert.ToInt32(row["Count"]) : 0);
                }

                seriesList.Add(new ChartSeries
                {
                    name = priority,
                    data = dataPoints,
                    color = colorMap[priority]
                });
            }

            // 輸出 JSON 
            hf_ChartCategories.Value = JsonConvert.SerializeObject(categories);
            hf_ChartSeries.Value = JsonConvert.SerializeObject(seriesList);

            // 呼叫前端
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CallDrawChart", "drawHighchart();", true);
        }

        /// <summary>
        /// 將原始資料轉為DataTable
        /// </summary>
        private DataTable GetPivotDataForDisplay(DataTable dtRaw)
        {
            if (dtRaw == null || dtRaw.Rows.Count == 0) return null;

            DataTable dtPivot = new DataTable();

            // 定義欄位
            dtPivot.Columns.Add("縣市");

            // 使用全域屬性取得 Enum 清單
            var priorities = this.TargetPriorities;

            // 建立一個字典來動態計算各欄位的總計 
            Dictionary<string, int> columnTotals = new Dictionary<string, int>();

            foreach (var p in priorities)
            {
                dtPivot.Columns.Add(p);
                columnTotals[p] = 0; // 初始化總計為 0
            }
            dtPivot.Columns.Add("合計");

            // 準備資料列 (Row) - 依縣市
            var locations = dtRaw.AsEnumerable()
                                 .Select(r => new {
                                     Name = r["Location"].ToString(),
                                     Id = Convert.ToInt32(r["LocationID"])
                                 })
                                 .Distinct()
                                 .OrderBy(x => x.Id)
                                 .ToList();

            int grandTotal = 0; // 全部總計

            foreach (var loc in locations)
            {
                DataRow newRow = dtPivot.NewRow();
                newRow["縣市"] = loc.Name;

                int rowSum = 0; // 該縣市的橫向合計

                foreach (var p in priorities)
                {
                    var match = dtRaw.AsEnumerable()
                                     .FirstOrDefault(r => r["Location"].ToString() == loc.Name &&
                                                          r["Priority"].ToString() == p);

                    int count = match != null ? Convert.ToInt32(match["Count"]) : 0;

                    // 存入欄位 (格式化 N0)
                    newRow[p] = count.ToString("N0");

                    // 動態累加該欄位的總計 
                    columnTotals[p] += count;

                    // 累加橫向合計
                    rowSum += count;
                }

                newRow["合計"] = rowSum.ToString("N0");
                grandTotal += rowSum;

                dtPivot.Rows.Add(newRow);
            }

            // 插入 [總計] 列到最上方
            DataRow totalRow = dtPivot.NewRow();
            totalRow["縣市"] = "總計";

            // 遍歷 Enum 把剛才算好的總計填進去
            foreach (var p in priorities)
            {
                totalRow[p] = columnTotals[p].ToString("N0");
            }
            totalRow["合計"] = grandTotal.ToString("N0");

            dtPivot.Rows.InsertAt(totalRow, 0);

            return dtPivot;
        }

        private void BindPivotTable(DataTable dtRaw)
        {
            // 呼叫共用函式取得處理好的 DataTable
            DataTable dtPivot = GetPivotDataForDisplay(dtRaw);

            if (dtPivot == null)
            {
                GridView_healthRecStats.DataSource = null;
                GridView_healthRecStats.DataBind();
                return;
            }

            GridView_healthRecStats.DataSource = dtPivot;
            GridView_healthRecStats.DataBind();
        }

        private void BindData()
        {
            // 從 ViewState 取得時間區間
            string sYear = ViewState_StartYear;
            string sMonth = ViewState_StartMonth;
            string eYear = ViewState_EndYear;
            string eMonth = ViewState_EndMonth;

            // 取得原始統計資料
            DataTable dtRaw = GetHealthPriorityData(sYear, sMonth, eYear, eMonth);

            // 綁定圖表
            BindChartData(dtRaw);

            // 綁定表格
            BindPivotTable(dtRaw);

            // 更新畫面上的查詢時間
            Lit_queryTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        }

        protected void LinkButton_query_Click(object sender, EventArgs e)
        {
            //紀錄搜尋紀錄
            UpdateSearchParameters();

            BindData();

        }
        protected void LinkButton_exportExcel_Click(object sender, EventArgs e)
        {
            // ==========================================
            // 準備查詢參數與資料
            // ==========================================
            string sYear = ViewState_StartYear;
            string sMonth = ViewState_StartMonth;
            string eYear = ViewState_EndYear;
            string eMonth = ViewState_EndMonth;

            // 取得原始資料
            DataTable dtRaw = GetHealthPriorityData(sYear, sMonth, eYear, eMonth);
            DataTable dt = GetPivotDataForDisplay(dtRaw);

            if (dt == null || dt.Rows.Count == 0)
            {
                ShowMessage("提示","發生錯誤，查無資料。");
                return;
            }

            // ==========================================
            // 建立 Excel 工作簿
            // ==========================================
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("健檢建議統計表");

            // ==========================================
            // 設定樣式 (Styles)
            // ==========================================
            string fontName = "標楷體";
            short fontHeight = 12;

            // 標題列樣式 (Header)
            ICellStyle headerStyle = workbook.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;

            IFont headerFont = workbook.CreateFont();
            headerFont.FontName = fontName;
            headerFont.FontHeightInPoints = fontHeight;
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;

            // 總計列樣式 (Total Row) - 粗體 + 千分位
            IDataFormat dataFormat = workbook.CreateDataFormat();

            ICellStyle totalRowStyle = workbook.CreateCellStyle();
            totalRowStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRowStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRowStyle.DataFormat = dataFormat.GetFormat("#,##0"); // 千分位

            IFont totalFont = workbook.CreateFont();
            totalFont.FontName = fontName;
            totalFont.FontHeightInPoints = fontHeight;
            totalFont.IsBold = true;
            totalRowStyle.SetFont(totalFont);

            // 一般資料樣式 (Data)
            ICellStyle dataStyle = workbook.CreateCellStyle();
            dataStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            dataStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            dataStyle.DataFormat = dataFormat.GetFormat("#,##0");

            IFont dataFont = workbook.CreateFont();
            dataFont.FontName = fontName;
            dataFont.FontHeightInPoints = fontHeight;
            dataStyle.SetFont(dataFont);

            // 第一欄文字樣式 (靠左對齊)
            ICellStyle textCellStyle = workbook.CreateCellStyle();
            textCellStyle.CloneStyleFrom(dataStyle);
            textCellStyle.DataFormat = 0; // 一般文字格式
            textCellStyle.Alignment = HorizontalAlignment.Left; // 縣市名稱靠左

            // ==========================================
            // 寫入資料
            // ==========================================

            // 寫入標題列
            IRow excelHeaderRow = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = excelHeaderRow.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
                cell.CellStyle = headerStyle;
            }

            // 寫入內容列
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                IRow excelRow = sheet.CreateRow(i + 1);

                // 判斷是否為「總計」列 
                bool isTotalRow = row["縣市"].ToString() == "總計";

                // 決定該列的預設樣式
                ICellStyle currentStyle = isTotalRow ? totalRowStyle : dataStyle;

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = excelRow.CreateCell(j);
                    string valStr = row[j].ToString();

                    // 第一欄 (縣市名稱)
                    if (j == 0)
                    {
                        cell.SetCellValue(valStr);
                       
                        if (isTotalRow)
                        {
                            cell.CellStyle = totalRowStyle;
                        }
                        else
                        {
                            cell.CellStyle = textCellStyle;
                        }
                    }
                    else
                    {
                        // 數值欄位
                        if (double.TryParse(valStr, out double numVal))
                        {
                            cell.SetCellValue(numVal);
                        }
                        else
                        {
                            cell.SetCellValue(0);
                        }
                        cell.CellStyle = currentStyle;
                    }
                }
            }

            // ==========================================
            // 自動調整欄寬
            // ==========================================
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i == 0)
                    sheet.SetColumnWidth(i, 20 * 256); // 第一欄寬一點
                else
                    sheet.SetColumnWidth(i, 12 * 256); // 數字欄位標準寬度
            }

            // ==========================================
            // 輸出檔案
            // ==========================================
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                string fileName = $"各縣市健康檢查建議統計表_{DateTime.Now:yyyyMMddHHmm}.xlsx";

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", $"attachment; filename={System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)}");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }

        protected void GridView_healthRecStats_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void GridView_healthRecStats_PreRender(object sender, EventArgs e)
        {
            // 確保 GridView 有資料且有標題列
            if (GridView_healthRecStats.Rows.Count > 0 && GridView_healthRecStats.HeaderRow != null)
            {
                // 產生 <thead> 標籤
                GridView_healthRecStats.HeaderRow.TableSection = TableRowSection.TableHeader;

                // 確保標題儲存格是用 <th> 
                GridView_healthRecStats.UseAccessibleHeader = true;
            }
        }

      
    }
}