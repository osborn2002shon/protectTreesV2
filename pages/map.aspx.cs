using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;
using DataAccess;
using protectTreesV2.Base;

namespace protectTreesV2.pages
{
    public partial class map1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TreeDataJson.Value = BuildTreeDataJson();
            }
        }

        private string BuildTreeDataJson()
        {
            const string sql = @"
SELECT r.treeID,
       r.systemTreeNo,
       r.latitude,
       r.longitude,
       r.breastHeightDiameter,
       r.breastHeightCircumference,
       r.estimatedPlantingYear,
       cityInfo.city,
       areaInfo.area,
       species.commonName
FROM Tree_Record r
OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) cityInfo
LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = r.areaID
LEFT JOIN Tree_Species species ON species.speciesID = r.speciesID
WHERE r.editStatus = 1
  AND r.treeStatus = N'已公告列管'
  AND r.latitude IS NOT NULL
  AND r.longitude IS NOT NULL
  AND LTRIM(RTRIM(r.latitude)) <> ''
  AND LTRIM(RTRIM(r.longitude)) <> ''";

            using (var db = new MS_SQL())
            {
                DataTable dt = db.GetDataTable(sql);
                var records = new List<TreeMapRecord>();

                foreach (DataRow row in dt.Rows)
                {
                    var estimatedAge = ParseEstimatedAge(DataRowHelper.GetString(row, "estimatedPlantingYear"));
                    records.Add(new TreeMapRecord
                    {
                        TreeId = DataRowHelper.GetNullableInt(row, "treeID") ?? 0,
                        SystemTreeNo = DataRowHelper.GetString(row, "systemTreeNo"),
                        City = DataRowHelper.GetString(row, "city"),
                        Area = DataRowHelper.GetString(row, "area"),
                        Species = DataRowHelper.GetString(row, "commonName"),
                        Latitude = DataRowHelper.GetString(row, "latitude"),
                        Longitude = DataRowHelper.GetString(row, "longitude"),
                        Age = estimatedAge,
                        BreastHeightDiameter = DataRowHelper.GetString(row, "breastHeightDiameter"),
                        BreastHeightCircumference = DataRowHelper.GetString(row, "breastHeightCircumference")
                    });
                }

                return new JavaScriptSerializer().Serialize(records);
            }
        }

        private static int? ParseEstimatedAge(string estimatedPlantingYear)
        {
            if (string.IsNullOrWhiteSpace(estimatedPlantingYear))
            {
                return null;
            }

            var match = Regex.Match(estimatedPlantingYear, @"\d{1,4}");
            if (!match.Success || !int.TryParse(match.Value, out int year))
            {
                return null;
            }

            if (year < 1000)
            {
                year += 1911;
            }

            int currentYear = DateTime.Now.Year;
            int age = currentYear - year;
            return age >= 0 ? (int?)age : null;
        }

        private class TreeMapRecord
        {
            public int TreeId { get; set; }
            public string SystemTreeNo { get; set; }
            public string City { get; set; }
            public string Area { get; set; }
            public string Species { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int? Age { get; set; }
            public string BreastHeightDiameter { get; set; }
            public string BreastHeightCircumference { get; set; }
        }
    }
}
