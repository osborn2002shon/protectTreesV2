using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataAccess;
using protectTreesV2.Base;

namespace protectTreesV2.pages
{
    public partial class analysis : System.Web.UI.Page
    {
        private const string ProtectedStatus = "已公告列管";
        private const int SpeciesTopCount = 8;

        public string CityChartDataJson { get; private set; } = "[]";
        public string SpeciesChartDataJson { get; private set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCityDropDown();
            }

            LoadSummaryCards();
            LoadCityChartData();
            LoadCitySpeciesData();
        }

        private void BindCityDropDown()
        {
            const string sql = @"
SELECT DISTINCT r.cityID,
       cityInfo.city
FROM Tree_Record r
OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) cityInfo
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
  AND r.cityID IS NOT NULL
ORDER BY r.cityID";

            using (var db = new MS_SQL())
            {
                var dt = db.GetDataTable(sql, new SqlParameter("@status", ProtectedStatus));
                DropDownList_city.DataSource = dt;
                DropDownList_city.DataTextField = "city";
                DropDownList_city.DataValueField = "cityID";
                DropDownList_city.DataBind();
            }
        }

        private void LoadSummaryCards()
        {
            using (var db = new MS_SQL())
            {
                Literal_TotalProtected.Text = FormatNumber(GetScalarLong(db, @"
SELECT COUNT(1)
FROM Tree_Record r
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status", new SqlParameter("@status", ProtectedStatus)));

                Literal_SpeciesCount.Text = FormatNumber(GetScalarLong(db, @"
SELECT COUNT(DISTINCT r.speciesID)
FROM Tree_Record r
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
  AND r.speciesID IS NOT NULL", new SqlParameter("@status", ProtectedStatus)));

                LoadSpeciesExtremes(db);
                LoadCoverageStats(db);
                LoadCityHighlights(db);
            }
        }

        private void LoadSpeciesExtremes(MS_SQL db)
        {
            const string sql = @"
SELECT TOP (@top)
       ISNULL(s.commonName, N'未知') AS speciesName,
       COUNT(1) AS treeCount
FROM Tree_Record r
LEFT JOIN Tree_Species s ON s.speciesID = r.speciesID
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
GROUP BY s.commonName
ORDER BY COUNT(1) {0}, s.commonName";

            var most = db.GetDataTable(string.Format(sql, "DESC"),
                new SqlParameter("@status", ProtectedStatus),
                new SqlParameter("@top", 1));
            var least = db.GetDataTable(string.Format(sql, "ASC"),
                new SqlParameter("@status", ProtectedStatus),
                new SqlParameter("@top", 1));

            Literal_MostSpeciesCount.Text = FormatNumber(GetFirstCount(most));
            Literal_MostSpeciesName.Text = JoinNames(most);
            Literal_LeastSpeciesCount.Text = FormatNumber(GetFirstCount(least));
            Literal_LeastSpeciesName.Text = JoinNames(least);
        }

        private void LoadCoverageStats(MS_SQL db)
        {
            Literal_CityCount.Text = FormatNumber(GetScalarLong(db, @"
SELECT COUNT(DISTINCT r.cityID)
FROM Tree_Record r
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
  AND r.cityID IS NOT NULL", new SqlParameter("@status", ProtectedStatus)));

            Literal_AreaCount.Text = FormatNumber(GetScalarLong(db, @"
SELECT COUNT(DISTINCT r.areaID)
FROM Tree_Record r
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
  AND r.areaID IS NOT NULL", new SqlParameter("@status", ProtectedStatus)));
        }

        private void LoadCityHighlights(MS_SQL db)
        {
            const string sql = @"
SELECT r.cityID,
       cityInfo.city AS cityName,
       COUNT(DISTINCT r.speciesID) AS speciesCount,
       COUNT(1) AS treeCount
FROM Tree_Record r
OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) cityInfo
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
  AND r.cityID IS NOT NULL
GROUP BY r.cityID, cityInfo.city";

            var dt = db.GetDataTable(sql, new SqlParameter("@status", ProtectedStatus));
            if (dt.Rows.Count == 0)
            {
                Literal_MostSpeciesCityCount.Text = "0";
                Literal_MostSpeciesCityName.Text = "—";
                Literal_MostProtectedCityCount.Text = "0";
                Literal_MostProtectedCityName.Text = "—";
                return;
            }

            var maxSpeciesCount = dt.AsEnumerable().Max(r => DataRowHelper.GetNullableInt(r, "speciesCount") ?? 0);
            var speciesCities = dt.AsEnumerable()
                .Where(r => (DataRowHelper.GetNullableInt(r, "speciesCount") ?? 0) == maxSpeciesCount)
                .Select(r => DataRowHelper.GetString(r, "cityName"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            Literal_MostSpeciesCityCount.Text = FormatNumber(maxSpeciesCount);
            Literal_MostSpeciesCityName.Text = speciesCities.Count > 0 ? string.Join("、", speciesCities) : "—";

            var maxTreeCount = dt.AsEnumerable().Max(r => GetRowLong(r, "treeCount"));
            var topTreeCities = dt.AsEnumerable()
                .Where(r => GetRowLong(r, "treeCount") == maxTreeCount)
                .Select(r => DataRowHelper.GetString(r, "cityName"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            Literal_MostProtectedCityCount.Text = FormatNumber(maxTreeCount);
            Literal_MostProtectedCityName.Text = topTreeCities.Count > 0 ? string.Join("、", topTreeCities) : "—";
        }

        private void LoadCityChartData()
        {
            const string sql = @"
SELECT r.cityID,
       cityInfo.city AS cityName,
       COUNT(1) AS treeCount
FROM Tree_Record r
OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) cityInfo
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
  AND r.cityID IS NOT NULL
GROUP BY r.cityID, cityInfo.city";

            using (var db = new MS_SQL())
            {
                var dt = db.GetDataTable(sql, new SqlParameter("@status", ProtectedStatus));
                var rows = dt.AsEnumerable().Select(row => new
                {
                    CityId = DataRowHelper.GetNullableInt(row, "cityID") ?? 0,
                    CityName = DataRowHelper.GetString(row, "cityName") ?? "未知",
                    TreeCount = GetRowLong(row, "treeCount")
                });

                bool orderByCount = RadioButtonList_c1Type.SelectedIndex == 1;
                var ordered = orderByCount
                    ? rows.OrderByDescending(r => r.TreeCount).ThenBy(r => r.CityId)
                    : rows.OrderBy(r => r.CityId);

                var chartData = new List<object[]>();
                foreach (var item in ordered)
                {
                    chartData.Add(new object[] { item.CityName, item.TreeCount });
                }

                CityChartDataJson = new JavaScriptSerializer().Serialize(chartData);
            }
        }

        private void LoadCitySpeciesData()
        {
            if (!int.TryParse(DropDownList_city.SelectedValue, out int cityId))
            {
                SpeciesChartDataJson = "[]";
                Repeater_citySpecies.DataSource = null;
                Repeater_citySpecies.DataBind();
                return;
            }

            const string sql = @"
SELECT TOP (@top)
       ISNULL(s.commonName, N'未知') AS speciesName,
       COUNT(1) AS treeCount,
       areaInfo.areaList
FROM Tree_Record r
LEFT JOIN Tree_Species s ON s.speciesID = r.speciesID
OUTER APPLY (
    SELECT STUFF((
        SELECT DISTINCT N'、' + st.area
        FROM Tree_Record r2
        LEFT JOIN System_Taiwan st ON st.twID = r2.areaID
        WHERE r2.editStatus = 1
          AND r2.removeDateTime IS NULL
          AND r2.treeStatus = @status
          AND r2.cityID = r.cityID
          AND r2.speciesID = r.speciesID
          AND st.area IS NOT NULL
        FOR XML PATH(''), TYPE
    ).value('.', 'nvarchar(max)'), 1, 1, '') AS areaList
) areaInfo
WHERE r.editStatus = 1
  AND r.removeDateTime IS NULL
  AND r.treeStatus = @status
  AND r.cityID = @cityID
GROUP BY s.commonName, areaInfo.areaList
ORDER BY COUNT(1) DESC, s.commonName";

            using (var db = new MS_SQL())
            {
                var dt = db.GetDataTable(sql,
                    new SqlParameter("@status", ProtectedStatus),
                    new SqlParameter("@cityID", cityId),
                    new SqlParameter("@top", SpeciesTopCount));

                var tableRows = dt.AsEnumerable()
                    .Select(row => new CitySpeciesRow
                    {
                        SpeciesName = DataRowHelper.GetString(row, "speciesName") ?? "未知",
                        TreeCount = FormatNumber(DataRowHelper.GetNullableInt(row, "treeCount") ?? 0),
                        AreaNames = string.IsNullOrWhiteSpace(DataRowHelper.GetString(row, "areaList"))
                            ? "—"
                            : DataRowHelper.GetString(row, "areaList")
                    })
                    .ToList();

                Repeater_citySpecies.DataSource = tableRows;
                Repeater_citySpecies.DataBind();

                var chartData = tableRows
                    .Select(row => new
                    {
                        name = row.SpeciesName,
                        y = ParseNumber(row.TreeCount)
                    })
                    .ToList();

                SpeciesChartDataJson = new JavaScriptSerializer().Serialize(chartData);
            }
        }

        private static long GetScalarLong(MS_SQL db, string sql, params SqlParameter[] parameters)
        {
            var result = db.ExcuteScalar(sql, parameters);
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt64(result);
        }

        private static long GetFirstCount(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            var value = dt.Rows[0]["treeCount"];
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt64(value);
        }

        private static string JoinNames(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "—";
            }

            var names = dt.AsEnumerable()
                .Select(row => DataRowHelper.GetString(row, "speciesName"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            return names.Count > 0 ? string.Join("、", names) : "—";
        }

        private static string FormatNumber(long value)
        {
            return value.ToString("N0");
        }

        private static long GetRowLong(DataRow row, string columnName)
        {
            var value = DataRowHelper.GetValue(row, columnName);
            return value == null ? 0 : Convert.ToInt64(value);
        }

        private static int ParseNumber(string value)
        {
            return int.TryParse(value.Replace(",", string.Empty), out int number) ? number : 0;
        }

        private class CitySpeciesRow
        {
            public string SpeciesName { get; set; }
            public string TreeCount { get; set; }
            public string AreaNames { get; set; }
        }
    }
}
