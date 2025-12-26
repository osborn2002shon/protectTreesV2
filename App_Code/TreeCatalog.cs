using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using DataAccess;
using static protectTreesV2.Base.DataRowHelper;

namespace protectTreesV2.TreeCatalog
{
    /// <summary>
    /// 樹籍狀態
    /// </summary>
    public enum TreeStatus
    {
        已公告列管 = 1,
        符合標準 = 2,
        其他 = 9
    }

    /// <summary>
    /// 編輯狀態
    /// </summary>
    public enum TreeEditState
    {
        草稿 = 0,
        完稿 = 1
    }

    /// <summary>
    /// 樹籍認定理由選項
    /// </summary>
    public class RecognitionCriterion
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }

    /// <summary>
    /// 樹種
    /// </summary>
    public class TreeSpecies
    {
        public int SpeciesID { get; set; }
        public string CommonName { get; set; }
        public string ScientificName { get; set; }

        public string DisplayName => string.IsNullOrWhiteSpace(ScientificName)
            ? CommonName
            : $"{CommonName} - {ScientificName}";
    }

    /// <summary>
    /// 樹籍照片
    /// </summary>
    public class TreePhoto
    {
        public int PhotoID { get; set; }
        public int TreeID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Caption { get; set; }
        public bool IsCover { get; set; }
        public DateTime InsertDateTime { get; set; }
        public int InsertAccountID { get; set; }
    }

    public class TreeRecordLog
    {
        public int LogID { get; set; }
        public int TreeID { get; set; }
        public string ActionType { get; set; }
        public string Memo { get; set; }
        public string IPAddress { get; set; }
        public int? AccountID { get; set; }
        public string Account { get; set; }
        public string AccountName { get; set; }
        public string AccountUnit { get; set; }
        public DateTime LogDateTime { get; set; }
    }

    /// <summary>
    /// 樹籍資料
    /// </summary>
    [Serializable]
    public class TreeRecord
    {
        public int TreeID { get; set; }
        public string SystemTreeNo { get; set; }
        public string AgencyTreeNo { get; set; }
        public string AgencyJurisdictionCode { get; set; }
        public int? CityID { get; set; }
        public string CityName { get; set; }
        public int? AreaID { get; set; }
        public string AreaName { get; set; }
        public int? SpeciesID { get; set; }
        public string SpeciesCommonName { get; set; }
        public string SpeciesScientificName { get; set; }
        public string Manager { get; set; }
        public string ManagerContact { get; set; }
        public DateTime? SurveyDate { get; set; }
        public string Surveyor { get; set; }
        public DateTime? AnnouncementDate { get; set; }
        public bool IsAnnounced { get; set; }
        public TreeStatus Status { get; set; }
        public TreeEditState EditStatus { get; set; }
        public int TreeCount { get; set; }
        public string Site { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string LandOwnership { get; set; }
        public string LandOwnershipNote { get; set; }
        public string FacilityDescription { get; set; }
        public string Memo { get; set; }
        public string Keywords { get; set; }
        public string RecognitionCriteriaRaw { get; set; }
        public string RecognitionNote { get; set; }
        public string CulturalHistoryIntro { get; set; }
        public string EstimatedPlantingYear { get; set; }
        public string EstimatedAgeNote { get; set; }
        public string GroupGrowthInfo { get; set; }
        public decimal? TreeHeight { get; set; }
        public decimal? BreastHeightDiameter { get; set; }
        public decimal? BreastHeightCircumference { get; set; }
        public decimal? CanopyProjectionArea { get; set; }
        public string HealthCondition { get; set; }
        public bool? HasEpiphyte { get; set; }
        public string EpiphyteDescription { get; set; }
        public bool? HasParasite { get; set; }
        public string ParasiteDescription { get; set; }
        public bool? HasClimbingPlant { get; set; }
        public string ClimbingPlantDescription { get; set; }
        public string SurveyOtherNote { get; set; }
        public string SourceUnit { get; set; }
        public int? SourceUnitID { get; set; }
        public int InsertAccountID { get; set; }
        public DateTime InsertDateTime { get; set; }
        public int? UpdateAccountID { get; set; }
        public DateTime? UpdateDateTime { get; set; }

        public List<string> RecognitionCriteria
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RecognitionCriteriaRaw)) return new List<string>();
                return RecognitionCriteriaRaw.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            }
            set
            {
                RecognitionCriteriaRaw = value == null ? null : string.Join(",", value.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()));
            }
        }

        public string SpeciesDisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SpeciesCommonName)) return SpeciesScientificName ?? string.Empty;
                if (string.IsNullOrWhiteSpace(SpeciesScientificName)) return SpeciesCommonName;
                return $"{SpeciesCommonName} - {SpeciesScientificName}";
            }
        }

        public string StatusText => TreeService.GetStatusText(Status);
        public string EditStatusText => TreeService.GetEditStatusText(EditStatus);

        public string AnnouncementDisplay
        {
            get
            {
                if (!IsAnnounced) return "否";
                return AnnouncementDate.HasValue ? AnnouncementDate.Value.ToString("yyyy/MM/dd") : "是";
            }
        }

        public string LastEditDisplay
        {
            get
            {
                if (!UpdateDateTime.HasValue) return string.Empty;
                return UpdateDateTime.Value.ToString("yyyy/MM/dd HH:mm");
            }
        }
    }

    /// <summary>
    /// 查詢條件
    /// </summary>
    public class TreeFilter
    {
        public int? CityID { get; set; }
        public int? AreaID { get; set; }
        public TreeEditState? EditStatus { get; set; }
        public TreeStatus? Status { get; set; }
        public int? SpeciesID { get; set; }
        public DateTime? SurveyDateStart { get; set; }
        public DateTime? SurveyDateEnd { get; set; }
        public DateTime? AnnouncementDateStart { get; set; }
        public DateTime? AnnouncementDateEnd { get; set; }
        public string Keyword { get; set; }
        public string SourceUnit { get; set; }
    }

    public static partial class TreeService
    {
        private static readonly Dictionary<TreeStatus, string> StatusValueMap = new Dictionary<TreeStatus, string>
        {
            { TreeStatus.已公告列管, "已公告列管" },
            { TreeStatus.符合標準, "符合標準" },
            { TreeStatus.其他, "其他" }
        };

        private static readonly Dictionary<string, TreeStatus> StatusLookupMap = StatusValueMap
            .ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.Ordinal);

        private class AreaCodeInfo
        {
            public string CityCode { get; set; }
            public string AreaCode { get; set; }
        }

        private const string InsertTreeSql = @"
INSERT INTO Tree_Record
(systemTreeNo, agencyTreeNo, agencyJurisdictionCode,
 cityID, areaID,
 speciesID,
 manager, managerContact, surveyDate, surveyor,
 announcementDate, isAnnounced, treeStatus, editStatus,
 treeCount, site, latitude, longitude,
 landOwnership, landOwnershipNote, facilityDescription,
 memo, keywords, recognitionCriteria,
 recognitionNote, culturalHistoryIntro, estimatedPlantingYear, estimatedAgeNote,
 groupGrowthInfo, treeHeight, breastHeightDiameter, breastHeightCircumference,
 canopyProjectionArea, healthCondition, hasEpiphyte, epiphyteDescription,
 hasParasite, parasiteDescription, hasClimbingPlant, climbingPlantDescription,
 surveyOtherNote,
 sourceUnit, sourceUnitID,
 insertAccountID, insertDateTime)
OUTPUT INSERTED.treeID
VALUES
 (@systemTreeNo, @agencyTreeNo, @agencyJurisdictionCode,
 @cityID, @areaID,
 @speciesID,
 @manager, @managerContact, @surveyDate, @surveyor,
 @announcementDate, @isAnnounced, @treeStatus, @editStatus,
 @treeCount, @site, @latitude, @longitude,
 @landOwnership, @landOwnershipNote, @facilityDescription,
 @memo, @keywords, @recognitionCriteria,
 @recognitionNote, @culturalHistoryIntro, @estimatedPlantingYear, @estimatedAgeNote,
 @groupGrowthInfo, @treeHeight, @breastHeightDiameter, @breastHeightCircumference,
 @canopyProjectionArea, @healthCondition, @hasEpiphyte, @epiphyteDescription,
 @hasParasite, @parasiteDescription, @hasClimbingPlant, @climbingPlantDescription,
 @surveyOtherNote,
 @sourceUnit, @sourceUnitID,
 @accountId, GETDATE())";

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static TreeRecord ToTreeRecord(DataRow row)
        {
            if (row == null)
            {
                return null;
            }

            var editStatusValue = GetNullableInt(row, "editStatus");

            return new TreeRecord
            {
                TreeID = GetNullableInt(row, "treeID") ?? 0,
                SystemTreeNo = GetString(row, "systemTreeNo"),
                AgencyTreeNo = GetString(row, "agencyTreeNo"),
                AgencyJurisdictionCode = GetString(row, "agencyJurisdictionCode"),
                CityID = GetNullableInt(row, "cityID"),
                CityName = GetString(row, "cityName"),
                AreaID = GetNullableInt(row, "areaID"),
                AreaName = GetString(row, "areaName"),
                SpeciesID = GetNullableInt(row, "speciesID"),
                SpeciesCommonName = GetString(row, "speciesCommonName"),
                SpeciesScientificName = GetString(row, "speciesScientificName"),
                Manager = GetString(row, "manager"),
                ManagerContact = GetString(row, "managerContact"),
                SurveyDate = GetNullableDateTime(row, "surveyDate"),
                Surveyor = GetString(row, "surveyor"),
                AnnouncementDate = GetNullableDateTime(row, "announcementDate"),
                IsAnnounced = GetBoolean(row, "isAnnounced"),
                Status = ParseStatus(GetValue(row, "treeStatus")),
                EditStatus = editStatusValue.HasValue ? (TreeEditState)editStatusValue.Value : TreeEditState.草稿,
                TreeCount = GetNullableInt(row, "treeCount") ?? 1,
                Site = GetString(row, "site"),
                Latitude = GetNullableDecimal(row, "latitude"),
                Longitude = GetNullableDecimal(row, "longitude"),
                LandOwnership = GetString(row, "landOwnership"),
                LandOwnershipNote = GetString(row, "landOwnershipNote"),
                FacilityDescription = GetString(row, "facilityDescription"),
                Memo = GetString(row, "memo"),
                Keywords = GetString(row, "keywords"),
                RecognitionCriteriaRaw = GetString(row, "recognitionCriteria"),
                RecognitionNote = GetString(row, "recognitionNote"),
                CulturalHistoryIntro = GetString(row, "culturalHistoryIntro"),
                EstimatedPlantingYear = GetString(row, "estimatedPlantingYear"),
                EstimatedAgeNote = GetString(row, "estimatedAgeNote"),
                GroupGrowthInfo = GetString(row, "groupGrowthInfo"),
                TreeHeight = GetNullableDecimal(row, "treeHeight"),
                BreastHeightDiameter = GetNullableDecimal(row, "breastHeightDiameter"),
                BreastHeightCircumference = GetNullableDecimal(row, "breastHeightCircumference"),
                CanopyProjectionArea = GetNullableDecimal(row, "canopyProjectionArea"),
                HealthCondition = GetString(row, "healthCondition"),
                HasEpiphyte = GetNullableBoolean(row, "hasEpiphyte"),
                EpiphyteDescription = GetString(row, "epiphyteDescription"),
                HasParasite = GetNullableBoolean(row, "hasParasite"),
                ParasiteDescription = GetString(row, "parasiteDescription"),
                HasClimbingPlant = GetNullableBoolean(row, "hasClimbingPlant"),
                ClimbingPlantDescription = GetString(row, "climbingPlantDescription"),
                SurveyOtherNote = GetString(row, "surveyOtherNote"),
                SourceUnit = GetString(row, "sourceUnit"),
                SourceUnitID = GetNullableInt(row, "sourceUnitID"),
                InsertAccountID = GetNullableInt(row, "insertAccountID") ?? 0,
                InsertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                UpdateAccountID = GetNullableInt(row, "updateAccountID"),
                UpdateDateTime = GetNullableDateTime(row, "updateDateTime")
            };
        }

        /// <summary>
        /// 取得樹籍資料
        /// </summary>
        public static TreeRecord GetTree(int treeId)
        {
            const string sql = @"
SELECT TOP 1 r.treeID, r.systemTreeNo, r.agencyTreeNo, r.agencyJurisdictionCode,
       r.cityID, COALESCE(area.city, city.city, r.cityName) AS cityName,
       r.areaID, COALESCE(area.area, r.areaName) AS areaName,
       r.speciesID, COALESCE(species.commonName, r.speciesCommonName) AS speciesCommonName,
       COALESCE(species.scientificName, r.speciesScientificName) AS speciesScientificName,
       r.manager, r.managerContact, r.surveyDate, r.surveyor,
       r.announcementDate, r.isAnnounced, r.treeStatus, r.editStatus,
       r.treeCount, r.site, r.latitude, r.longitude,
       r.landOwnership, r.landOwnershipNote, r.facilityDescription,
       r.memo, r.keywords, r.recognitionCriteria,
       r.recognitionNote, r.culturalHistoryIntro, r.estimatedPlantingYear, r.estimatedAgeNote,
       r.groupGrowthInfo, r.treeHeight, r.breastHeightDiameter, r.breastHeightCircumference,
       r.canopyProjectionArea, r.healthCondition,
       r.hasEpiphyte, r.epiphyteDescription,
       r.hasParasite, r.parasiteDescription,
       r.hasClimbingPlant, r.climbingPlantDescription,
       r.surveyOtherNote,
       r.sourceUnit, r.sourceUnitID,
       r.insertAccountID, r.insertDateTime, r.updateAccountID, r.updateDateTime
FROM Tree_Record r
OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) city
LEFT JOIN System_Taiwan area ON area.twID = r.areaID
LEFT JOIN Tree_Species species ON species.speciesID = r.speciesID
WHERE r.treeID=@id AND r.removeDateTime IS NULL";

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", treeId));
                return dt.Rows.Count == 1 ? ToTreeRecord(dt.Rows[0]) : null;
            }
        }

        public static TreeRecord GetTreeBySystemTreeNo(string systemTreeNo)
        {
            if (string.IsNullOrWhiteSpace(systemTreeNo)) return null;

            const string sql = @"
SELECT TOP 1 r.treeID, r.systemTreeNo, r.agencyTreeNo, r.agencyJurisdictionCode,
       r.cityID, COALESCE(area.city, city.city, r.cityName) AS cityName,
       r.areaID, COALESCE(area.area, r.areaName) AS areaName,
       r.speciesID, COALESCE(species.commonName, r.speciesCommonName) AS speciesCommonName,
       COALESCE(species.scientificName, r.speciesScientificName) AS speciesScientificName,
       r.manager, r.managerContact, r.surveyDate, r.surveyor,
       r.announcementDate, r.isAnnounced, r.treeStatus, r.editStatus,
       r.treeCount, r.site, r.latitude, r.longitude,
       r.landOwnership, r.landOwnershipNote, r.facilityDescription,
       r.memo, r.keywords, r.recognitionCriteria,
       r.recognitionNote, r.culturalHistoryIntro, r.estimatedPlantingYear, r.estimatedAgeNote,
       r.groupGrowthInfo, r.treeHeight, r.breastHeightDiameter, r.breastHeightCircumference,
       r.canopyProjectionArea, r.healthCondition,
       r.hasEpiphyte, r.epiphyteDescription,
       r.hasParasite, r.parasiteDescription,
       r.hasClimbingPlant, r.climbingPlantDescription,
       r.surveyOtherNote,
       r.sourceUnit, r.sourceUnitID,
       r.insertAccountID, r.insertDateTime, r.updateAccountID, r.updateDateTime
FROM Tree_Record r
OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) city
LEFT JOIN System_Taiwan area ON area.twID = r.areaID
LEFT JOIN Tree_Species species ON species.speciesID = r.speciesID
WHERE r.systemTreeNo=@systemTreeNo AND r.removeDateTime IS NULL";

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@systemTreeNo", systemTreeNo));
                return dt.Rows.Count == 1 ? ToTreeRecord(dt.Rows[0]) : null;
            }
        }

        /// <summary>
        /// 查詢樹籍資料
        /// </summary>
        public static List<TreeRecord> SearchTrees(TreeFilter filter)
        {
            var records = new List<TreeRecord>();
            var sql = new StringBuilder();
            sql.Append(@"SELECT r.treeID, r.systemTreeNo, r.agencyTreeNo, r.agencyJurisdictionCode,
                                r.cityID, COALESCE(area.city, city.city, r.cityName) AS cityName,
                                r.areaID, COALESCE(area.area, r.areaName) AS areaName,
                                r.speciesID, COALESCE(species.commonName, r.speciesCommonName) AS speciesCommonName,
                                COALESCE(species.scientificName, r.speciesScientificName) AS speciesScientificName,
                                r.manager, r.managerContact, r.surveyDate, r.announcementDate, r.isAnnounced,
                                r.treeStatus, r.editStatus, r.updateDateTime,
                                r.sourceUnit
                         FROM Tree_Record r
                         OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) city
                         LEFT JOIN System_Taiwan area ON area.twID = r.areaID
                         LEFT JOIN Tree_Species species ON species.speciesID = r.speciesID
                         WHERE r.removeDateTime IS NULL");

            var parameters = new List<SqlParameter>();

            if (filter != null)
            {
                if (filter.CityID.HasValue)
                {
                    sql.Append(" AND r.cityID=@cityID");
                    parameters.Add(new SqlParameter("@cityID", filter.CityID.Value));
                }

                if (filter.AreaID.HasValue)
                {
                    sql.Append(" AND r.areaID=@areaID");
                    parameters.Add(new SqlParameter("@areaID", filter.AreaID.Value));
                }

                if (filter.EditStatus.HasValue)
                {
                    sql.Append(" AND r.editStatus=@editStatus");
                    parameters.Add(new SqlParameter("@editStatus", (int)filter.EditStatus.Value));
                }

                if (filter.Status.HasValue)
                {
                    sql.Append(" AND r.treeStatus=@status");
                    parameters.Add(new SqlParameter("@status", GetStatusValue(filter.Status.Value)));
                }

                if (filter.SpeciesID.HasValue)
                {
                    sql.Append(" AND r.speciesID=@speciesID");
                    parameters.Add(new SqlParameter("@speciesID", filter.SpeciesID.Value));
                }

                if (filter.SurveyDateStart.HasValue)
                {
                    sql.Append(" AND r.surveyDate>=@surveyStart");
                    parameters.Add(new SqlParameter("@surveyStart", filter.SurveyDateStart.Value));
                }

                if (filter.SurveyDateEnd.HasValue)
                {
                    sql.Append(" AND r.surveyDate<=@surveyEnd");
                    parameters.Add(new SqlParameter("@surveyEnd", filter.SurveyDateEnd.Value));
                }

                if (filter.AnnouncementDateStart.HasValue)
                {
                    sql.Append(" AND r.announcementDate>=@announceStart");
                    parameters.Add(new SqlParameter("@announceStart", filter.AnnouncementDateStart.Value));
                }

                if (filter.AnnouncementDateEnd.HasValue)
                {
                    sql.Append(" AND r.announcementDate<=@announceEnd");
                    parameters.Add(new SqlParameter("@announceEnd", filter.AnnouncementDateEnd.Value));
                }

                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    sql.Append(" AND (r.systemTreeNo LIKE @kw OR r.agencyTreeNo LIKE @kw OR r.agencyJurisdictionCode LIKE @kw OR r.manager LIKE @kw)");
                    parameters.Add(new SqlParameter("@kw", "%" + filter.Keyword.Trim() + "%"));
                }

                if (!string.IsNullOrWhiteSpace(filter.SourceUnit))
                {
                    sql.Append(" AND r.sourceUnit=@sourceUnit");
                    parameters.Add(new SqlParameter("@sourceUnit", filter.SourceUnit));
                }
            }

            sql.Append(" ORDER BY systemTreeNo");

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql.ToString(), parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    records.Add(ToTreeRecord(row));
                }
            }

            return records;
        }

        /// <summary>
        /// 匯出樹籍基本資料
        /// </summary>
        public static DataTable ExportTrees(TreeFilter filter)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT r.systemTreeNo AS [系統樹籍編號],
                                r.agencyTreeNo AS [機關樹木編號],
                                r.agencyJurisdictionCode AS [機關管轄編碼],
                                COALESCE(area.city, city.city, r.cityName) AS [縣市],
                                COALESCE(area.area, r.areaName) AS [鄉鎮市區],
                                COALESCE(species.commonName, r.speciesCommonName) AS [樹種],
                                COALESCE(species.scientificName, r.speciesScientificName) AS [學名],
                                r.manager AS [管理人員],
                                r.managerContact AS [管理人聯絡資訊],
                                r.surveyDate AS [調查日期],
                                r.surveyor AS [調查人員],
                                r.announcementDate AS [公告日期],
                                r.isAnnounced AS [是否公告列管],
                                r.treeStatus AS [樹籍狀態],
                                CASE WHEN r.editStatus=1 THEN N'完稿' ELSE N'草稿' END AS [編輯狀態],
                                r.treeCount AS [數量],
                                r.site AS [坐落地點],
                                r.latitude AS [座標(WGS84)_緯度(N)],
                                r.longitude AS [座標(WGS84)_經度(E)],
                                r.landOwnership AS [土地權屬],
                                r.landOwnershipNote AS [土地權屬備註],
                                r.facilityDescription AS [管理設施描述],
                                r.memo AS [其他備註],
                                r.keywords AS [關鍵字],
                                r.recognitionCriteria AS [受保護認定理由],
                                r.recognitionNote AS [認定理由備註],
                                r.culturalHistoryIntro AS [文化歷史價值介紹],
                                r.estimatedPlantingYear AS [推估種植年間],
                                r.estimatedAgeNote AS [推估樹齡備註],
                                r.groupGrowthInfo AS [群生竹木或行道樹生長資訊],
                                r.treeHeight AS [樹高],
                                r.breastHeightDiameter AS [胸高直徑],
                                r.breastHeightCircumference AS [胸高樹圍],
                                r.canopyProjectionArea AS [樹冠投影面積],
                                r.healthCondition AS [樹木健康及生育地概況],
                                r.hasEpiphyte AS [是否有附生植物],
                                r.epiphyteDescription AS [附生植物概況],
                                r.hasParasite AS [是否有寄生植物],
                                r.parasiteDescription AS [寄生植物概況],
                                r.hasClimbingPlant AS [是否有纏勒植物],
                                r.climbingPlantDescription AS [纏勒植物概況],
                                r.surveyOtherNote AS [其他調查備註],
                                r.sourceUnit AS [資料來源單位],
                                r.sourceUnitID AS [資料來源代碼]
                         FROM Tree_Record r
                         OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = r.cityID) city
                         LEFT JOIN System_Taiwan area ON area.twID = r.areaID
                         LEFT JOIN Tree_Species species ON species.speciesID = r.speciesID
                         WHERE r.removeDateTime IS NULL");

            var parameters = new List<SqlParameter>();
            if (filter != null)
            {
                if (filter.CityID.HasValue)
                {
                    sql.Append(" AND r.cityID=@cityID");
                    parameters.Add(new SqlParameter("@cityID", filter.CityID.Value));
                }
                if (filter.AreaID.HasValue)
                {
                    sql.Append(" AND r.areaID=@areaID");
                    parameters.Add(new SqlParameter("@areaID", filter.AreaID.Value));
                }
                if (filter.EditStatus.HasValue)
                {
                    sql.Append(" AND r.editStatus=@editStatus");
                    parameters.Add(new SqlParameter("@editStatus", (int)filter.EditStatus.Value));
                }
                if (filter.Status.HasValue)
                {
                    sql.Append(" AND r.treeStatus=@status");
                    parameters.Add(new SqlParameter("@status", GetStatusValue(filter.Status.Value)));
                }
                if (filter.SpeciesID.HasValue)
                {
                    sql.Append(" AND r.speciesID=@speciesID");
                    parameters.Add(new SqlParameter("@speciesID", filter.SpeciesID.Value));
                }
                if (filter.SurveyDateStart.HasValue)
                {
                    sql.Append(" AND r.surveyDate>=@surveyStart");
                    parameters.Add(new SqlParameter("@surveyStart", filter.SurveyDateStart.Value));
                }
                if (filter.SurveyDateEnd.HasValue)
                {
                    sql.Append(" AND r.surveyDate<=@surveyEnd");
                    parameters.Add(new SqlParameter("@surveyEnd", filter.SurveyDateEnd.Value));
                }
                if (filter.AnnouncementDateStart.HasValue)
                {
                    sql.Append(" AND r.announcementDate>=@announceStart");
                    parameters.Add(new SqlParameter("@announceStart", filter.AnnouncementDateStart.Value));
                }
                if (filter.AnnouncementDateEnd.HasValue)
                {
                    sql.Append(" AND r.announcementDate<=@announceEnd");
                    parameters.Add(new SqlParameter("@announceEnd", filter.AnnouncementDateEnd.Value));
                }
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    sql.Append(" AND (r.systemTreeNo LIKE @kw OR r.agencyTreeNo LIKE @kw OR r.agencyJurisdictionCode LIKE @kw OR r.manager LIKE @kw)");
                    parameters.Add(new SqlParameter("@kw", "%" + filter.Keyword.Trim() + "%"));
                }
                if (!string.IsNullOrWhiteSpace(filter.SourceUnit))
                {
                    sql.Append(" AND r.sourceUnit=@sourceUnit");
                    parameters.Add(new SqlParameter("@sourceUnit", filter.SourceUnit));
                }
            }

            sql.Append(" ORDER BY systemTreeNo");

            using (var da = new MS_SQL())
            {
                var table = da.GetDataTable(sql.ToString(), parameters.ToArray());
                ReplaceRecognitionCriteriaWithOrderNo(table);
                return table;
            }
        }

        private static void ReplaceRecognitionCriteriaWithOrderNo(DataTable dt)
        {
            if (dt == null || !dt.Columns.Contains("受保護認定理由"))
            {
                return;
            }

            var orderMap = GetRecognitionCriteria().ToDictionary(c => c.Code, c => c.Order);

            foreach (DataRow row in dt.Rows)
            {
                var raw = row["受保護認定理由"]?.ToString();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    row["受保護認定理由"] = string.Empty;
                    continue;
                }

                var mappedOrders = raw
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(code => code.Trim())
                    .Where(orderMap.ContainsKey)
                    .Select(code => orderMap[code])
                    .Distinct()
                    .OrderBy(order => order)
                    .ToList();

                row["受保護認定理由"] = mappedOrders.Count == 0
                    ? string.Empty
                    : string.Join(",", mappedOrders);
            }
        }

        /// <summary>
        /// 新增樹籍
        /// </summary>
        public static int InsertTree(TreeRecord record, int accountId)
        {
            using (var da = new MS_SQL())
            {
                return ExecuteInsertTree(record, accountId, da);
            }
        }

        /// <summary>
        /// 更新樹籍
        /// </summary>
        public static void UpdateTree(TreeRecord record, int accountId)
        {
            const string sql = @"
UPDATE Tree_Record
SET systemTreeNo=@systemTreeNo,
    agencyTreeNo=@agencyTreeNo,
    agencyJurisdictionCode=@agencyJurisdictionCode,
    cityID=@cityID,
    areaID=@areaID,
    speciesID=@speciesID,
    manager=@manager,
    managerContact=@managerContact,
    surveyDate=@surveyDate,
    surveyor=@surveyor,
    announcementDate=@announcementDate,
    isAnnounced=@isAnnounced,
    treeStatus=@treeStatus,
    editStatus=@editStatus,
    treeCount=@treeCount,
    site=@site,
    latitude=@latitude,
    longitude=@longitude,
    landOwnership=@landOwnership,
    landOwnershipNote=@landOwnershipNote,
    facilityDescription=@facilityDescription,
    memo=@memo,
    keywords=@keywords,
    recognitionCriteria=@recognitionCriteria,
    recognitionNote=@recognitionNote,
    culturalHistoryIntro=@culturalHistoryIntro,
    estimatedPlantingYear=@estimatedPlantingYear,
    estimatedAgeNote=@estimatedAgeNote,
    groupGrowthInfo=@groupGrowthInfo,
    treeHeight=@treeHeight,
    breastHeightDiameter=@breastHeightDiameter,
    breastHeightCircumference=@breastHeightCircumference,
    canopyProjectionArea=@canopyProjectionArea,
    healthCondition=@healthCondition,
    hasEpiphyte=@hasEpiphyte,
    epiphyteDescription=@epiphyteDescription,
    hasParasite=@hasParasite,
    parasiteDescription=@parasiteDescription,
    hasClimbingPlant=@hasClimbingPlant,
    climbingPlantDescription=@climbingPlantDescription,
    surveyOtherNote=@surveyOtherNote,
    updateAccountID=@accountId,
    updateDateTime=GETDATE()
WHERE treeID=@id AND removeDateTime IS NULL";

            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@systemTreeNo", ToDbValue(record.SystemTreeNo)),
                    new SqlParameter("@agencyTreeNo", ToDbValue(record.AgencyTreeNo)),
                    new SqlParameter("@agencyJurisdictionCode", ToDbValue(record.AgencyJurisdictionCode)),
                    new SqlParameter("@cityID", ToDbValue(record.CityID)),
                    new SqlParameter("@areaID", ToDbValue(record.AreaID)),
                    new SqlParameter("@speciesID", ToDbValue(record.SpeciesID)),
                    new SqlParameter("@manager", ToDbValue(record.Manager)),
                    new SqlParameter("@managerContact", ToDbValue(record.ManagerContact)),
                    new SqlParameter("@surveyDate", ToDbValue(record.SurveyDate)),
                    new SqlParameter("@surveyor", ToDbValue(record.Surveyor)),
                    new SqlParameter("@announcementDate", ToDbValue(record.AnnouncementDate)),
                    new SqlParameter("@isAnnounced", record.IsAnnounced),
                    new SqlParameter("@treeStatus", GetStatusValue(record.Status)),
                    new SqlParameter("@editStatus", (int)record.EditStatus),
                    new SqlParameter("@treeCount", record.TreeCount),
                    new SqlParameter("@site", ToDbValue(record.Site)),
                    new SqlParameter("@latitude", ToDbValue(record.Latitude)),
                    new SqlParameter("@longitude", ToDbValue(record.Longitude)),
                    new SqlParameter("@landOwnership", ToDbValue(record.LandOwnership)),
                    new SqlParameter("@landOwnershipNote", ToDbValue(record.LandOwnershipNote)),
                    new SqlParameter("@facilityDescription", ToDbValue(record.FacilityDescription)),
                    new SqlParameter("@memo", ToDbValue(record.Memo)),
                    new SqlParameter("@keywords", ToDbValue(record.Keywords)),
                    new SqlParameter("@recognitionCriteria", ToDbValue(record.RecognitionCriteriaRaw)),
                    new SqlParameter("@recognitionNote", ToDbValue(record.RecognitionNote)),
                    new SqlParameter("@culturalHistoryIntro", ToDbValue(record.CulturalHistoryIntro)),
                    new SqlParameter("@estimatedPlantingYear", ToDbValue(record.EstimatedPlantingYear)),
                    new SqlParameter("@estimatedAgeNote", ToDbValue(record.EstimatedAgeNote)),
                    new SqlParameter("@groupGrowthInfo", ToDbValue(record.GroupGrowthInfo)),
                    new SqlParameter("@treeHeight", ToDbValue(record.TreeHeight)),
                    new SqlParameter("@breastHeightDiameter", ToDbValue(record.BreastHeightDiameter)),
                    new SqlParameter("@breastHeightCircumference", ToDbValue(record.BreastHeightCircumference)),
                    new SqlParameter("@canopyProjectionArea", ToDbValue(record.CanopyProjectionArea)),
                    new SqlParameter("@healthCondition", ToDbValue(record.HealthCondition)),
                    new SqlParameter("@hasEpiphyte", ToDbValue(record.HasEpiphyte)),
                    new SqlParameter("@epiphyteDescription", ToDbValue(record.EpiphyteDescription)),
                    new SqlParameter("@hasParasite", ToDbValue(record.HasParasite)),
                    new SqlParameter("@parasiteDescription", ToDbValue(record.ParasiteDescription)),
                    new SqlParameter("@hasClimbingPlant", ToDbValue(record.HasClimbingPlant)),
                    new SqlParameter("@climbingPlantDescription", ToDbValue(record.ClimbingPlantDescription)),
                    new SqlParameter("@surveyOtherNote", ToDbValue(record.SurveyOtherNote)),
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@id", record.TreeID));
            }
        }

        /// <summary>
        /// 刪除樹籍
        /// </summary>
        public static void DeleteTree(int treeId, int accountId)
        {
            const string sql = @"UPDATE Tree_Record SET removeDateTime=GETDATE(), removeAccountID=@accountId WHERE treeID=@id AND removeDateTime IS NULL";
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@id", treeId));
            }
        }

        /// <summary>
        /// 批次更新狀態
        /// </summary>
        public static void BulkUpdateStatus(IEnumerable<int> treeIds, TreeStatus status, DateTime? announcementDate, int accountId)
        {
            var ids = treeIds?.Distinct().ToList();
            if (ids == null || ids.Count == 0) return;

            bool isAnnounced = status == TreeStatus.已公告列管;
            DateTime? finalAnnouncementDate = isAnnounced ? announcementDate : null;

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@status", GetStatusValue(status)),
                new SqlParameter("@isAnnounced", isAnnounced),
                new SqlParameter("@announcementDate", ToDbValue(finalAnnouncementDate)),
                new SqlParameter("@accountId", accountId)
            };

            var inClause = new StringBuilder();
            for (int i = 0; i < ids.Count; i++)
            {
                string paramName = "@id" + i;
                inClause.Append(i == 0 ? paramName : "," + paramName);
                parameters.Add(new SqlParameter(paramName, ids[i]));
            }

            string sql = $@"UPDATE Tree_Record
                             SET treeStatus=@status,
                                 isAnnounced=@isAnnounced,
                                 announcementDate=@announcementDate,
                                 updateAccountID=@accountId,
                                 updateDateTime=GETDATE()
                             WHERE treeID IN ({inClause}) AND removeDateTime IS NULL";

            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql, parameters.ToArray());
            }
        }

        private static AreaCodeInfo GetAreaCodeInfo(int? cityId, int? areaId)
        {
            using (var da = new MS_SQL())
            {
                if (areaId.HasValue)
                {
                    const string areaSql = "SELECT TOP 1 cityCode, areaCode FROM System_Taiwan WHERE twID=@id";
                    var dt = da.GetDataTable(areaSql, new SqlParameter("@id", areaId.Value));
                    if (dt.Rows.Count > 0)
                    {
                        return new AreaCodeInfo
                        {
                            CityCode = dt.Rows[0]["cityCode"] as string,
                            AreaCode = dt.Rows[0]["areaCode"] as string
                        };
                    }
                }

                if (cityId.HasValue)
                {
                    const string citySql = "SELECT TOP 1 cityCode FROM System_Taiwan WHERE cityID=@city";
                    var dt = da.GetDataTable(citySql, new SqlParameter("@city", cityId.Value));
                    if (dt.Rows.Count > 0)
                    {
                        return new AreaCodeInfo
                        {
                            CityCode = dt.Rows[0]["cityCode"] as string,
                            AreaCode = null
                        };
                    }
                }
            }
            return null;
        }

        private static int ToRocYear(DateTime date)
        {
            int year = date.Year - 1911;
            return year < 1 ? 1 : year;
        }

        public static string GenerateSystemTreeNo(int? cityId, int? areaId, DateTime? referenceDate = null)
        {
            if (!cityId.HasValue && !areaId.HasValue) return null;

            var info = GetAreaCodeInfo(cityId, areaId);
            string cityCode = !string.IsNullOrWhiteSpace(info?.CityCode)
                ? info.CityCode.Trim().ToUpperInvariant()
                : (cityId.HasValue ? cityId.Value.ToString("D2", CultureInfo.InvariantCulture) : "XX");

            string prefix = cityCode;

            const string sql = "SELECT MAX(systemTreeNo) FROM Tree_Record WHERE systemTreeNo LIKE @prefix + '%'";
            using (var da = new MS_SQL())
            {
                var result = da.ExcuteScalar(sql, new SqlParameter("@prefix", prefix));
                int sequence = 0;
                if (result != null && result != DBNull.Value)
                {
                    string current = result.ToString();
                    if (!string.IsNullOrEmpty(current) && current.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        string suffix = current.Substring(prefix.Length);
                        int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out sequence);
                    }
                }

                sequence++;
                return prefix + sequence.ToString("D5", CultureInfo.InvariantCulture);
            }
        }

        public static List<TreeRecordLog> GetTreeLogs(int treeId)
        {
            var logs = new List<TreeRecordLog>();
            const string sql = @"SELECT logID, treeID, actionType, memo, ipAddress, accountID, account, accountName, accountUnit, logDateTime
                                 FROM Tree_RecordLog
                                 WHERE treeID=@id
                                 ORDER BY logDateTime DESC, logID DESC";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", treeId));
                foreach (DataRow row in dt.Rows)
                {
                    logs.Add(new TreeRecordLog
                    {
                        LogID = Convert.ToInt32(row["logID"]),
                        TreeID = Convert.ToInt32(row["treeID"]),
                        ActionType = row["actionType"] as string,
                        Memo = row["memo"] as string,
                        IPAddress = row["ipAddress"] as string,
                        AccountID = row["accountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["accountID"]),
                        Account = row["account"] as string,
                        AccountName = row["accountName"] as string,
                        AccountUnit = row["accountUnit"] as string,
                        LogDateTime = Convert.ToDateTime(row["logDateTime"])
                    });
                }
            }
            return logs;
        }

        public static void InsertTreeLog(int treeId, string actionType, string memo, string ipAddress, int? accountId, string account, string accountName, string accountUnit)
        {
            const string sql = @"INSERT INTO Tree_RecordLog(treeID, actionType, memo, ipAddress, accountID, account, accountName, accountUnit)
                                 VALUES(@treeID, @actionType, @memo, @ipAddress, @accountID, @account, @accountName, @accountUnit)";
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@treeID", treeId),
                    new SqlParameter("@actionType", actionType ?? string.Empty),
                    new SqlParameter("@memo", ToDbValue(memo)),
                    new SqlParameter("@ipAddress", ToDbValue(ipAddress)),
                    new SqlParameter("@accountID", ToDbValue(accountId)),
                    new SqlParameter("@account", ToDbValue(account)),
                    new SqlParameter("@accountName", ToDbValue(accountName)),
                    new SqlParameter("@accountUnit", ToDbValue(accountUnit)));
            }
        }

        /// <summary>
        /// 取得樹種清單
        /// </summary>
        public static List<TreeSpecies> GetSpecies()
        {
            var list = new List<TreeSpecies>();
            const string sql = "SELECT speciesID, commonName, scientificName FROM Tree_Species WHERE isActive=1 ORDER BY commonName";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new TreeSpecies
                    {
                        SpeciesID = Convert.ToInt32(row["speciesID"]),
                        CommonName = row["commonName"] as string,
                        ScientificName = row["scientificName"] as string
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 取得認定理由
        /// </summary>
        public static List<RecognitionCriterion> GetRecognitionCriteria()
        {
            var list = new List<RecognitionCriterion>();
            const string sql = "SELECT criterionCode, criterionName, orderNo FROM Tree_RecognitionCriterion WHERE isActive=1 ORDER BY orderNo";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new RecognitionCriterion
                    {
                        Code = row["criterionCode"].ToString(),
                        Name = row["criterionName"].ToString(),
                        Order = Convert.ToInt32(row["orderNo"])
                    });
                }
            }
            if (list.Count == 0)
            {
                // 預設選項
                list.AddRange(new[]
                {
                    new RecognitionCriterion { Code = "age_100", Name = "一、樹齡達一百年以上。", Order = 1 },
                    new RecognitionCriterion { Code = "dbh_large", Name = "二、離地一點三公尺處（以下簡稱胸高），\n闊葉樹之樹幹胸高直徑達一點五公尺以上或胸高樹圍達四點七公尺以上；\n針葉樹之樹幹胸高直徑達零點七五公尺以上或胸高樹圍達二點四公尺以上。", Order = 2 },
                    new RecognitionCriterion { Code = "canopy_400", Name = "三、樹冠投影面積達四百平方公尺以上。", Order = 3 },
                    new RecognitionCriterion { Code = "biodiversity", Name = "四、形成具生物多樣性豐富之生態環境。", Order = 4 },
                    new RecognitionCriterion { Code = "regional_representative", Name = "五、為區域具地理上代表性樹木。", Order = 5 },
                    new RecognitionCriterion { Code = "aesthetic_value", Name = "六、具重大美學欣賞價值之景觀。", Order = 6 },
                    new RecognitionCriterion { Code = "cultural_connection", Name = "七、與當地居民生活、情感、祭祀、民俗或信仰具有重大連結性。", Order = 7 },
                    new RecognitionCriterion { Code = "historical_event", Name = "八、與重大歷史事件具有關聯性。", Order = 8 },
                    new RecognitionCriterion { Code = "research_education", Name = "九、具有人文、科學研究及自然教育價值。", Order = 9 },
                    new RecognitionCriterion { Code = "shared_memory", Name = "十、當地居民之共同記憶場域。", Order = 10 },
                    new RecognitionCriterion { Code = "other_significance", Name = "十一、具有其他重要意義。", Order = 11 }
                });
            }
            return list.OrderBy(c => c.Order).ToList();
        }


        public static List<TreePhoto> GetPhotos(int treeId)
        {
            var list = new List<TreePhoto>();
            const string sql = @"SELECT photoID, treeID, fileName, filePath, caption, isCover, insertDateTime, insertAccountID
                                 FROM Tree_RecordPhoto
                                 WHERE treeID=@id AND removeDateTime IS NULL
                                 ORDER BY CASE WHEN isCover=1 THEN 0 ELSE 1 END, photoID";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", treeId));
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new TreePhoto
                    {
                        PhotoID = Convert.ToInt32(row["photoID"]),
                        TreeID = Convert.ToInt32(row["treeID"]),
                        FileName = row["fileName"].ToString(),
                        FilePath = row["filePath"].ToString(),
                        Caption = row["caption"] as string,
                        IsCover = Convert.ToBoolean(row["isCover"]),
                        InsertDateTime = Convert.ToDateTime(row["insertDateTime"]),
                        InsertAccountID = Convert.ToInt32(row["insertAccountID"])
                    });
                }
            }
            return list;
        }
      

        /// <summary>
        /// 一次性匯入資料
        /// </summary>
        public static void BulkImport(DataTable source, int accountId, string sourceUnit)
        {
            if (source == null || source.Rows.Count == 0) return;

            using (var da = new MS_SQL())
            {
                da.StartTransaction();
                try
                {
                    foreach (DataRow row in source.Rows)
                    {
                        var record = new TreeRecord
                        {
                            SystemTreeNo = row.Table.Columns.Contains("SystemTreeNo") ? row["SystemTreeNo"] as string : null,
                            AgencyTreeNo = row.Table.Columns.Contains("AgencyTreeNo") ? row["AgencyTreeNo"] as string : null,
                            AgencyJurisdictionCode = row.Table.Columns.Contains("JurisdictionCode") ? row["JurisdictionCode"] as string : null,
                            CityName = row.Table.Columns.Contains("City") ? row["City"] as string : null,
                            AreaName = row.Table.Columns.Contains("Area") ? row["Area"] as string : null,
                            SpeciesCommonName = row.Table.Columns.Contains("Species") ? row["Species"] as string : null,
                            SpeciesScientificName = row.Table.Columns.Contains("ScientificName") ? row["ScientificName"] as string : null,
                            Manager = row.Table.Columns.Contains("Manager") ? row["Manager"] as string : null,
                            SurveyDate = ParseDate(row.Table.Columns.Contains("SurveyDate") ? row["SurveyDate"] : null),
                            AnnouncementDate = ParseDate(row.Table.Columns.Contains("AnnouncementDate") ? row["AnnouncementDate"] : null),
                            Status = TreeStatus.其他,
                            EditStatus = TreeEditState.草稿,
                            SourceUnit = sourceUnit,
                            TreeCount = 1
                        };
                        ExecuteInsertTree(record, accountId, da);
                    }
                    da.Commit();
                }
                catch
                {
                    da.RollBack();
                    throw;
                }
            }
        }

        private static DateTime? ParseDate(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            if (value is DateTime dt) return dt;
            if (DateTime.TryParse(value.ToString(), out DateTime parsed)) return parsed;
            return null;
        }

        private static int ExecuteInsertTree(TreeRecord record, int accountId, MS_SQL da)
        {
            return Convert.ToInt32(da.ExcuteScalar(InsertTreeSql,
                new SqlParameter("@systemTreeNo", ToDbValue(record.SystemTreeNo)),
                new SqlParameter("@agencyTreeNo", ToDbValue(record.AgencyTreeNo)),
                new SqlParameter("@agencyJurisdictionCode", ToDbValue(record.AgencyJurisdictionCode)),
                new SqlParameter("@cityID", ToDbValue(record.CityID)),
                new SqlParameter("@areaID", ToDbValue(record.AreaID)),
                new SqlParameter("@speciesID", ToDbValue(record.SpeciesID)),
                new SqlParameter("@manager", ToDbValue(record.Manager)),
                new SqlParameter("@managerContact", ToDbValue(record.ManagerContact)),
                new SqlParameter("@surveyDate", ToDbValue(record.SurveyDate)),
                new SqlParameter("@surveyor", ToDbValue(record.Surveyor)),
                new SqlParameter("@announcementDate", ToDbValue(record.AnnouncementDate)),
                new SqlParameter("@isAnnounced", record.IsAnnounced),
                new SqlParameter("@treeStatus", GetStatusValue(record.Status)),
                new SqlParameter("@editStatus", (int)record.EditStatus),
                new SqlParameter("@treeCount", record.TreeCount),
                new SqlParameter("@site", ToDbValue(record.Site)),
                new SqlParameter("@latitude", ToDbValue(record.Latitude)),
                new SqlParameter("@longitude", ToDbValue(record.Longitude)),
                new SqlParameter("@landOwnership", ToDbValue(record.LandOwnership)),
                new SqlParameter("@landOwnershipNote", ToDbValue(record.LandOwnershipNote)),
                new SqlParameter("@facilityDescription", ToDbValue(record.FacilityDescription)),
                new SqlParameter("@memo", ToDbValue(record.Memo)),
                new SqlParameter("@keywords", ToDbValue(record.Keywords)),
                new SqlParameter("@recognitionCriteria", ToDbValue(record.RecognitionCriteriaRaw)),
                new SqlParameter("@recognitionNote", ToDbValue(record.RecognitionNote)),
                new SqlParameter("@culturalHistoryIntro", ToDbValue(record.CulturalHistoryIntro)),
                new SqlParameter("@estimatedPlantingYear", ToDbValue(record.EstimatedPlantingYear)),
                new SqlParameter("@estimatedAgeNote", ToDbValue(record.EstimatedAgeNote)),
                new SqlParameter("@groupGrowthInfo", ToDbValue(record.GroupGrowthInfo)),
                new SqlParameter("@treeHeight", ToDbValue(record.TreeHeight)),
                new SqlParameter("@breastHeightDiameter", ToDbValue(record.BreastHeightDiameter)),
                new SqlParameter("@breastHeightCircumference", ToDbValue(record.BreastHeightCircumference)),
                new SqlParameter("@canopyProjectionArea", ToDbValue(record.CanopyProjectionArea)),
                new SqlParameter("@healthCondition", ToDbValue(record.HealthCondition)),
                new SqlParameter("@hasEpiphyte", ToDbValue(record.HasEpiphyte)),
                new SqlParameter("@epiphyteDescription", ToDbValue(record.EpiphyteDescription)),
                new SqlParameter("@hasParasite", ToDbValue(record.HasParasite)),
                new SqlParameter("@parasiteDescription", ToDbValue(record.ParasiteDescription)),
                new SqlParameter("@hasClimbingPlant", ToDbValue(record.HasClimbingPlant)),
                new SqlParameter("@climbingPlantDescription", ToDbValue(record.ClimbingPlantDescription)),
                new SqlParameter("@surveyOtherNote", ToDbValue(record.SurveyOtherNote)),
                new SqlParameter("@sourceUnit", ToDbValue(record.SourceUnit)),
                new SqlParameter("@sourceUnitID", ToDbValue(record.SourceUnitID)),
                new SqlParameter("@accountId", accountId)));
        }

        /// <summary>
        /// 新增照片
        /// </summary>
        public static int InsertPhoto(TreePhoto photo, int accountId)
        {
            const string sql = @"
INSERT INTO Tree_RecordPhoto(treeID, fileName, filePath, caption, isCover, insertAccountID, insertDateTime)
OUTPUT INSERTED.photoID
VALUES(@treeID, @fileName, @filePath, @caption, @isCover, @accountId, GETDATE())";
            using (var da = new MS_SQL())
            {
                return Convert.ToInt32(da.ExcuteScalar(sql,
                    new SqlParameter("@treeID", photo.TreeID),
                    new SqlParameter("@fileName", photo.FileName),
                    new SqlParameter("@filePath", photo.FilePath),
                    new SqlParameter("@caption", ToDbValue(photo.Caption)),
                    new SqlParameter("@isCover", photo.IsCover),
                    new SqlParameter("@accountId", accountId)));
            }
        }

        public static void SetCoverPhoto(int treeId, int photoId, int accountId)
        {
            const string resetSql = "UPDATE Tree_RecordPhoto SET isCover=0, updateAccountID=@accountId, updateDateTime=GETDATE() WHERE treeID=@id";
            const string setSql = "UPDATE Tree_RecordPhoto SET isCover=1, updateAccountID=@accountId, updateDateTime=GETDATE() WHERE photoID=@photoId";
            using (var da = new MS_SQL())
            {
                da.StartTransaction();
                try
                {
                    da.ExecNonQuery(resetSql,
                        new SqlParameter("@accountId", accountId),
                        new SqlParameter("@id", treeId));
                    da.ExecNonQuery(setSql,
                        new SqlParameter("@accountId", accountId),
                        new SqlParameter("@photoId", photoId));
                    da.Commit();
                }
                catch
                {
                    da.RollBack();
                    throw;
                }
            }
        }

        public static void DeletePhoto(int photoId, int accountId)
        {
            const string sql = "UPDATE Tree_RecordPhoto SET removeDateTime=GETDATE(), removeAccountID=@accountId WHERE photoID=@id";
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@id", photoId));
            }
        }    
     

        private static string GetStatusValue(TreeStatus status)
        {
            if (StatusValueMap.TryGetValue(status, out string value))
            {
                return value;
            }

            if (StatusValueMap.TryGetValue(TreeStatus.其他, out string fallback))
            {
                return fallback;
            }

            return "其他";
        }

        private static TreeStatus ParseStatus(object value)
        {
            if (value == null || value == DBNull.Value) return TreeStatus.其他;

            string text = value.ToString();
            if (string.IsNullOrWhiteSpace(text)) return TreeStatus.其他;

            text = text.Trim();

            if (StatusLookupMap.TryGetValue(text, out TreeStatus status))
            {
                return status;
            }

            if (int.TryParse(text, out int numeric) && Enum.IsDefined(typeof(TreeStatus), numeric))
            {
                return (TreeStatus)numeric;
            }

            return TreeStatus.其他;
        }

        public static string GetStatusText(TreeStatus status)
        {
            return GetStatusValue(status);
        }

        public static string GetEditStatusText(TreeEditState state)
        {
            return state == TreeEditState.完稿 ? "完稿" : "草稿";
        }
    }
}

