using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using DataAccess;

namespace protectTreesV2._uc.dashboard
{
    public partial class uc_treeStatusChart : UserControl
    {
        public int UnitId { get; set; }

        [Category("自訂屬性")]
        public bool IsStatusTown { get; set; } = false;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RenderChart();
        }

        private void RenderChart()
        {
            var containerId = ChartContainer.ClientID;
            var categories = new List<string>();
            var statuses = new[] { "已公告列管", "符合標準", "其他" };
            var dataLookup = new Dictionary<(string City, string Status), int>();

            string sql;
            if (IsStatusTown)
            {
                sql = @"
                SELECT t.city AS CityName, t.area AS AreaName, r.treeStatus, COUNT(*) AS TotalCount
                FROM Tree_Record r
                INNER JOIN System_Taiwan t ON r.areaID = t.twID
                INNER JOIN System_UnitCityMapping map ON t.twID = map.twID AND map.unitID = @unitID
                WHERE r.editStatus = 1 AND r.removeDateTime IS NULL
                GROUP BY t.city, r.treeStatus, t.twID, t.area
                ORDER BY t.city, t.twID";
            }
            else
            {
                sql = @"
                SELECT t.city AS CityName, r.treeStatus, COUNT(*) AS TotalCount
                FROM Tree_Record r
                INNER JOIN System_Taiwan t ON r.areaID = t.twID
                INNER JOIN System_UnitCityMapping map ON t.twID = map.twID AND map.unitID = @unitID
                WHERE r.editStatus = 1 AND r.removeDateTime IS NULL
                GROUP BY t.city, r.treeStatus
                ORDER BY t.city";
            }

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@unitID", UnitId));
                foreach (DataRow row in dt.Rows)
                {

                    // 統計項目(X軸)
                    var area = "";
                    if (IsStatusTown)
                    {
                        area = row["AreaName"].ToString();
                    }
                    else
                    {
                        area = row["CityName"].ToString();
                    }

                    var status = row["treeStatus"].ToString();
                    var count = row["TotalCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalCount"]);

                    if (!categories.Contains(area))
                    {
                        categories.Add(area);
                    }

                    dataLookup[(area, status)] = count;
                }
            }

            var series = new List<object>();
            foreach (var status in statuses)
            {
                var values = categories.Select(area => dataLookup.TryGetValue((area, status), out var count) ? count : 0).ToList();
                series.Add(new
                {
                    name = status,
                    data = values
                });
            }

            var serializer = new JavaScriptSerializer();
            var categoriesJson = serializer.Serialize(categories);
            var seriesJson = serializer.Serialize(series);

            var script = new StringBuilder();
            script.AppendLine("<script>");
            script.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
            script.AppendLine($"Highcharts.chart('{containerId}', {{");
            script.AppendLine("chart: { type: 'column' },");
            script.AppendLine("title: { text: null },");
            script.AppendLine($"xAxis: {{ categories: {categoriesJson} }},");
            script.AppendLine("yAxis: { min: 0, title: { text: '數量' }, stackLabels: { enabled: true } },");
            script.AppendLine("legend: { align: 'center', verticalAlign: 'bottom' },");
            script.AppendLine("plotOptions: { column: { stacking: 'normal' } },");
            script.AppendLine($"series: {seriesJson}");
            script.AppendLine("});");
            script.AppendLine("});");
            script.AppendLine("</script>");

            litChartScript.Text = script.ToString();
        }
    }
}
