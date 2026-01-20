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
    public partial class uc_areaGroupPieChart : UserControl
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
            var dataPoints = new List<object>();

            const string sql = @"
                SELECT ISNULL(t.areaGroup, N'未分類') AS AreaGroup, COUNT(*) AS TotalCount
                FROM Tree_Record r
                INNER JOIN System_Taiwan t ON r.areaID = t.twID
                INNER JOIN System_UnitCityMapping map ON t.twID = map.twID AND map.unitID = @unitID
                WHERE r.editStatus = 1 AND r.removeDateTime IS NULL
                GROUP BY t.areaGroup
                ORDER BY TotalCount DESC";

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@unitID", UnitId));
                foreach (DataRow row in dt.Rows)
                {
                    dataPoints.Add(new
                    {
                        name = row["AreaGroup"].ToString(),
                        y = row["TotalCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalCount"])
                    });
                }
            }

            var serializer = new JavaScriptSerializer();
            var dataJson = serializer.Serialize(dataPoints);

            var script = new StringBuilder();
            script.AppendLine("<script>");
            script.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
            script.AppendLine($"Highcharts.chart('{containerId}', {{");
            script.AppendLine("chart: { type: 'pie' },");
            script.AppendLine("title: { text: null },");
            script.AppendLine("tooltip: { pointFormat: '{series.name}: <b>{point.y}</b>' },");
            script.AppendLine("plotOptions: { pie: { allowPointSelect: true, cursor: 'pointer', dataLabels: { enabled: true } } },");
            script.AppendLine($"series: [{{ name: '數量', colorByPoint: true, data: {dataJson} }}]");
            script.AppendLine("});");
            script.AppendLine("});");
            script.AppendLine("</script>");

            litChartScript.Text = script.ToString();
        }
    }
}
