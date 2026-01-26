using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using DataAccess;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;

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
       r.treeCount,
       r.site,
       r.manager,
       r.treeStatus,
       r.announcementDate,
       r.recognitionCriteria,
       r.culturalHistoryIntro,
       r.treeHeight,
       cityInfo.city,
       areaInfo.area,
       species.commonName,
       species.scientificName,
       photoInfo.photoUrl
FROM Tree_Record r
OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) cityInfo
LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = r.areaID
LEFT JOIN Tree_Species species ON species.speciesID = r.speciesID
OUTER APPLY (
    SELECT TOP 1 filePath AS photoUrl
    FROM Tree_RecordPhoto
    WHERE treeID = r.treeID AND removeDateTime IS NULL
    ORDER BY CASE WHEN isCover = 1 THEN 0 ELSE 1 END, photoID
) photoInfo
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
                var photoLookup = new Dictionary<int, List<string>>();

                var criteriaLookup = TreeService.GetRecognitionCriteria()
                    .GroupBy(c => c.Code)
                    .ToDictionary(g => g.Key, g => g.First().Name);

                var treeIds = dt.AsEnumerable()
                    .Select(row => DataRowHelper.GetNullableInt(row, "treeID"))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Distinct()
                    .ToList();

                if (treeIds.Count > 0)
                {
                    var photoSql = $@"
SELECT treeID,
       filePath
FROM Tree_RecordPhoto
WHERE removeDateTime IS NULL
  AND treeID IN ({string.Join(",", treeIds)})
ORDER BY treeID, CASE WHEN isCover = 1 THEN 0 ELSE 1 END, photoID";

                    DataTable photoTable = db.GetDataTable(photoSql);
                    foreach (DataRow row in photoTable.Rows)
                    {
                        var treeId = DataRowHelper.GetNullableInt(row, "treeID");
                        var filePath = DataRowHelper.GetString(row, "filePath");
                        if (!treeId.HasValue || string.IsNullOrWhiteSpace(filePath))
                        {
                            continue;
                        }

                        if (!photoLookup.TryGetValue(treeId.Value, out var list))
                        {
                            list = new List<string>();
                            photoLookup[treeId.Value] = list;
                        }

                        list.Add(filePath);
                    }
                }

                foreach (DataRow row in dt.Rows)
                {
                    var estimatedAge = ParseEstimatedAge(DataRowHelper.GetString(row, "estimatedPlantingYear"));
                    var recognitionCriteria = DataRowHelper.GetString(row, "recognitionCriteria");
                    var announcementDate = DataRowHelper.GetNullableDateTime(row, "announcementDate");
                    var treeIdValue = DataRowHelper.GetNullableInt(row, "treeID") ?? 0;
                    photoLookup.TryGetValue(treeIdValue, out var photoUrls);
                    records.Add(new TreeMapRecord
                    {
                        TreeId = treeIdValue,
                        SystemTreeNo = DataRowHelper.GetString(row, "systemTreeNo"),
                        City = DataRowHelper.GetString(row, "city"),
                        Area = DataRowHelper.GetString(row, "area"),
                        Species = DataRowHelper.GetString(row, "commonName"),
                        SpeciesScientificName = DataRowHelper.GetString(row, "scientificName"),
                        Latitude = DataRowHelper.GetString(row, "latitude"),
                        Longitude = DataRowHelper.GetString(row, "longitude"),
                        Age = estimatedAge,
                        BreastHeightDiameter = DataRowHelper.GetString(row, "breastHeightDiameter"),
                        BreastHeightCircumference = DataRowHelper.GetString(row, "breastHeightCircumference"),
                        TreeCount = DataRowHelper.GetNullableInt(row, "treeCount"),
                        Site = DataRowHelper.GetString(row, "site"),
                        Manager = DataRowHelper.GetString(row, "manager"),
                        TreeStatus = DataRowHelper.GetString(row, "treeStatus"),
                        AnnouncementDate = announcementDate?.ToString("yyyy/MM/dd"),
                        RecognitionReasonsHtml = BuildRecognitionDisplay(recognitionCriteria, criteriaLookup),
                        CulturalHistoryIntro = DataRowHelper.GetString(row, "culturalHistoryIntro"),
                        TreeHeight = DataRowHelper.GetString(row, "treeHeight"),
                        PhotoUrl = DataRowHelper.GetString(row, "photoUrl"),
                        PhotoUrls = photoUrls ?? new List<string>()
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

        private static string BuildRecognitionDisplay(string criteriaRaw, IDictionary<string, string> criteriaLookup)
        {
            if (string.IsNullOrWhiteSpace(criteriaRaw))
            {
                return null;
            }

            var codes = criteriaRaw
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(code => code.Trim())
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .ToList();

            if (codes.Count == 0)
            {
                return null;
            }

            var names = codes
                .Select(code => criteriaLookup.TryGetValue(code, out var name) ? name : null)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();

            if (names.Count > 0)
            {
                return string.Join("<br />", names.Select(name => HttpUtility.HtmlEncode(name).Replace("\n", "<br />")));
            }

            return HttpUtility.HtmlEncode(string.Join(",", codes));
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
            public string SpeciesScientificName { get; set; }
            public int? TreeCount { get; set; }
            public string Site { get; set; }
            public string Manager { get; set; }
            public string TreeStatus { get; set; }
            public string AnnouncementDate { get; set; }
            public string RecognitionReasonsHtml { get; set; }
            public string CulturalHistoryIntro { get; set; }
            public string TreeHeight { get; set; }
            public string PhotoUrl { get; set; }
            public List<string> PhotoUrls { get; set; }
        }
    }
}
