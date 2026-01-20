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
    public partial class uc_speciesStatusChart : UserControl
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
            var statuses = new[] { "已公告列管", "符合標準", "其他" };
            var dataLookup = new Dictionary<(string Species, string Status), int>();

            const string sql = @"
                SELECT ISNULL(s.commonName, N'未填寫') AS SpeciesName, r.treeStatus, COUNT(*) AS TotalCount
                FROM Tree_Record r
                LEFT JOIN Tree_Species s ON r.speciesID = s.speciesID
                INNER JOIN System_UnitCityMapping map ON r.areaID = map.twID AND map.unitID = @unitID
                WHERE r.editStatus = 1 AND r.removeDateTime IS NULL
                GROUP BY s.commonName, r.treeStatus
                ORDER BY s.commonName";

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@unitID", UnitId));
                foreach (DataRow row in dt.Rows)
                {
                    var species = row["SpeciesName"].ToString();
                    var status = row["treeStatus"].ToString();
                    var count = row["TotalCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalCount"]);

                    if (!categories.Contains(species))
                    {
                        categories.Add(species);
                    }

                    dataLookup[(species, status)] = count;
                }
            }

            var series = new List<object>();
            foreach (var status in statuses)
            {
                var values = categories.Select(species => dataLookup.TryGetValue((species, status), out var count) ? count : 0).ToList();
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
            script.AppendLine($"xAxis: {{ categories: {categoriesJson}, labels: {{ rotation: -45 }} }},");
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
