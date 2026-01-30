using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using DataAccess;

namespace protectTreesV2._uc.dashboard
{
    public partial class uc_userLoginChart : UserControl
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
            var counts = new List<int>();

            const string sql = @"
                SELECT u.unitName AS UnitName, COUNT(*) AS TotalCount
                FROM System_UserLog log
                INNER JOIN View_UserInfo u ON log.accountID = u.accountID
                INNER JOIN System_UnitUnitMapping m ON u.unitID = m.unitID
                WHERE m.manageUnitID = @unitID
                  AND log.logItem = N'登入'
                  AND log.logDateTime >= DATEADD(DAY, -30, GETDATE())
                GROUP BY u.unitName
                ORDER BY TotalCount DESC";

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@unitID", UnitId));
                foreach (DataRow row in dt.Rows)
                {
                    categories.Add(row["UnitName"].ToString());
                    counts.Add(row["TotalCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalCount"]));
                }
            }

            var serializer = new JavaScriptSerializer();
            var categoriesJson = serializer.Serialize(categories);
            var seriesJson = serializer.Serialize(new[]
            {
                new { name = "登入次數", data = counts }
            });

            var script = new StringBuilder();
            script.AppendLine("<script>");
            script.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
            script.AppendLine($"Highcharts.chart('{containerId}', {{");
            script.AppendLine("chart: { type: 'column' },");
            script.AppendLine("title: { text: null },");
            script.AppendLine("credits: { enabled: false }, exporting: { enabled: false },");
            script.AppendLine($"xAxis: {{ categories: {categoriesJson}, labels: {{ rotation: -45 }} }},");
            script.AppendLine("yAxis: { min: 0, title: { text: '次數' } },");
            script.AppendLine("legend: { enabled: false },");
            script.AppendLine("plotOptions: { series: { dataLabels: { enabled: true } } },");
            script.AppendLine($"series: {seriesJson}");
            script.AppendLine("});");
            script.AppendLine("});");
            script.AppendLine("</script>");

            litChartScript.Text = script.ToString();
        }
    }
}
