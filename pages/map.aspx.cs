using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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
       species.scientificName
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

            var criteriaLookup = TreeService.GetRecognitionCriteria()
                .GroupBy(c => c.Code)
                .ToDictionary(g => g.Key, g => g.First().Name);

            using (var db = new MS_SQL())
            {
                DataTable dt = db.GetDataTable(sql);
                var photoLookup = BuildTreePhotoLookup(db);
                var records = new List<TreeMapRecord>();

                foreach (DataRow row in dt.Rows)
                {
                    var estimatedAge = ParseEstimatedAge(DataRowHelper.GetString(row, "estimatedPlantingYear"));
                    int treeId = DataRowHelper.GetNullableInt(row, "treeID") ?? 0;
                    var announcementDate = DataRowHelper.GetNullableDateTime(row, "announcementDate");
                    records.Add(new TreeMapRecord
                    {
                        TreeId = treeId,
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
                        AnnouncementDate = announcementDate.HasValue ? announcementDate.Value.ToString("yyyy/MM/dd") : null,
                        RecognitionCriteriaDisplay = BuildRecognitionCriteriaDisplay(DataRowHelper.GetString(row, "recognitionCriteria"), criteriaLookup),
                        CulturalHistoryIntro = DataRowHelper.GetString(row, "culturalHistoryIntro"),
                        TreeHeight = DataRowHelper.GetString(row, "treeHeight"),
                        Photos = photoLookup.TryGetValue(treeId, out var photos) ? photos : new List<TreeMapPhoto>()
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

        private static Dictionary<int, List<TreeMapPhoto>> BuildTreePhotoLookup(MS_SQL db)
        {
            const string photoSql = @"
SELECT p.treeID,
       p.filePath,
       p.caption,
       p.isCover
FROM Tree_RecordPhoto p
INNER JOIN Tree_Record r ON r.treeID = p.treeID
WHERE r.editStatus = 1
  AND r.treeStatus = N'已公告列管'
  AND r.latitude IS NOT NULL
  AND r.longitude IS NOT NULL
  AND LTRIM(RTRIM(r.latitude)) <> ''
  AND LTRIM(RTRIM(r.longitude)) <> ''
  AND p.removeDateTime IS NULL
ORDER BY p.treeID, CASE WHEN p.isCover = 1 THEN 0 ELSE 1 END, p.photoID";
            var lookup = new Dictionary<int, List<TreeMapPhoto>>();
            DataTable dt = db.GetDataTable(photoSql);
            foreach (DataRow row in dt.Rows)
            {
                int treeId = DataRowHelper.GetNullableInt(row, "treeID") ?? 0;
                if (!lookup.TryGetValue(treeId, out var list))
                {
                    list = new List<TreeMapPhoto>();
                    lookup[treeId] = list;
                }
                list.Add(new TreeMapPhoto
                {
                    FilePath = DataRowHelper.GetString(row, "filePath"),
                    Caption = DataRowHelper.GetString(row, "caption"),
                    IsCover = DataRowHelper.GetNullableBoolean(row, "isCover") ?? false
                });
            }
            return lookup;
        }

        private static List<string> BuildRecognitionCriteriaDisplay(string recognitionCriteria, IDictionary<string, string> lookup)
        {
            if (string.IsNullOrWhiteSpace(recognitionCriteria))
            {
                return new List<string>();
            }

            var codes = recognitionCriteria
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(code => code.Trim())
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .ToList();

            return codes
                .Select(code => lookup.TryGetValue(code, out var name) ? name : code)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();
        }

        private class TreeMapRecord
        {
            public int TreeId { get; set; }
            public string SystemTreeNo { get; set; }
            public string City { get; set; }
            public string Area { get; set; }
            public string Species { get; set; }
            public string SpeciesScientificName { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int? Age { get; set; }
            public string BreastHeightDiameter { get; set; }
            public string BreastHeightCircumference { get; set; }
            public int? TreeCount { get; set; }
            public string Site { get; set; }
            public string Manager { get; set; }
            public string TreeStatus { get; set; }
            public string AnnouncementDate { get; set; }
            public List<string> RecognitionCriteriaDisplay { get; set; }
            public string CulturalHistoryIntro { get; set; }
            public string TreeHeight { get; set; }
            public List<TreeMapPhoto> Photos { get; set; }
        }

        private class TreeMapPhoto
        {
            public string FilePath { get; set; }
            public string Caption { get; set; }
            public bool IsCover { get; set; }
        }
    }
}
