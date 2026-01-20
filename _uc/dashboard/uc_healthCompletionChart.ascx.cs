using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using DataAccess;

namespace protectTreesV2._uc.dashboard
{
    public partial class uc_healthCompletionChart : UserControl
    {
        public int UnitId { get; set; }

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
            var completedCounts = new List<int>();
            var pendingCounts = new List<int>();

            const string sql = @"
                WITH HealthTrees AS (
                    SELECT DISTINCT treeID
                    FROM Tree_HealthRecord
                    WHERE dataStatus = 1 AND removeDateTime IS NULL
                )
                SELECT t.city AS CityName,
                       SUM(CASE WHEN h.treeID IS NOT NULL THEN 1 ELSE 0 END) AS CompletedCount,
                       SUM(CASE WHEN h.treeID IS NULL THEN 1 ELSE 0 END) AS PendingCount
                FROM Tree_Record r
                INNER JOIN System_Taiwan t ON r.areaID = t.twID
                INNER JOIN System_UnitCityMapping map ON t.twID = map.twID AND map.unitID = @unitID
                LEFT JOIN HealthTrees h ON r.treeID = h.treeID
                WHERE r.editStatus = 1 AND r.removeDateTime IS NULL
                GROUP BY t.city
                ORDER BY t.city";

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@unitID", UnitId));
                foreach (DataRow row in dt.Rows)
                {
                    categories.Add(row["CityName"].ToString());
                    completedCounts.Add(row["CompletedCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["CompletedCount"]));
                    pendingCounts.Add(row["PendingCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["PendingCount"]));
                }
            }

            var series = new List<object>
            {
                new { name = "已調查", data = completedCounts },
                new { name = "尚未調查", data = pendingCounts }
            };

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
