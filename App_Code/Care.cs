using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;
using static protectTreesV2.Base.DataRowHelper;

namespace protectTreesV2.Care
{
    public class Care
    {
        public enum CareRecordStatus
        {
            草稿 = 0,
            定稿 = 1
        }

        [Serializable]
        public class CareMainQueryFilter
        {
            public int? cityID { get; set; }
            public int? areaID { get; set; }
            public int? speciesID { get; set; }
            public string keyword { get; set; }
            public string queryOption { get; set; } = "NoRecord365";
            public string sortExpression { get; set; }
            public string sortDirection { get; set; }
        }

        public class CareMainQueryResult
        {
            public int treeID { get; set; }
            public int? careID { get; set; }
            public int? dataStatus { get; set; }
            public string systemTreeNo { get; set; }
            public string agencyTreeNo { get; set; }
            public string agencyJurisdictionCode { get; set; }
            public string cityName { get; set; }
            public string areaName { get; set; }
            public string speciesName { get; set; }
            public string manager { get; set; }
            public TreeStatus? treeStatus { get; set; }
            public string treeStatusText => treeStatus.HasValue ? TreeService.GetStatusText(treeStatus.Value) : string.Empty;
            public DateTime? careDate { get; set; }
            public string recorder { get; set; }
            public string reviewer { get; set; }
            public DateTime? lastUpdate { get; set; }
            public bool isAdded { get; set; }

            public string careRecordStatusText =>
                !careID.HasValue || careID.Value == 0
                    ? "--"
                    : (dataStatus.GetValueOrDefault() == (int)CareRecordStatus.定稿 ? "定稿" : "草稿");
        }

        public class CareBatchSettingResult
        {
            public int settingID { get; set; }
            public DateTime insertDateTime { get; set; }
            public int treeID { get; set; }
            public string systemTreeNo { get; set; }
            public string agencyTreeNo { get; set; }
            public string agencyJurisdictionCode { get; set; }
            public string cityName { get; set; }
            public string areaName { get; set; }
            public string speciesName { get; set; }
            public string manager { get; set; }
            public DateTime? careDate { get; set; }
            public string recorder { get; set; }
        }

        [Serializable]
        public class CareRecordListFilter
        {
            public string scope { get; set; } = "Unit";
            public int? cityID { get; set; }
            public int? areaID { get; set; }
            public int? speciesID { get; set; }
            public DateTime? dateStart { get; set; }
            public DateTime? dateEnd { get; set; }
            public string keyword { get; set; }
            public string sortExpression { get; set; }
            public string sortDirection { get; set; }
            public TreeEditState? dataState { get; set; }
            public TreeEditState? dataStatus { get; set; }
        }

        public class CareRecordListResult
        {
            public int careID { get; set; }
            public DateTime? careDate { get; set; }
            public string recorder { get; set; }
            public string reviewer { get; set; }
            public int? dataStatus { get; set; }
            public DateTime? lastUpdate { get; set; }

            public int treeID { get; set; }
            public string systemTreeNo { get; set; }
            public string agencyTreeNo { get; set; }
            public string cityName { get; set; }
            public string areaName { get; set; }
            public string speciesName { get; set; }
            public string manager { get; set; }
            public int? areaID { get; set; }

            public string dataStatusText
                => dataStatus.GetValueOrDefault() == (int)CareRecordStatus.定稿 ? "定稿" : "草稿";
        }

        public class CareRecord
        {
            public int careID { get; set; }
            public int treeID { get; set; }
            public DateTime? careDate { get; set; }
            public string recorder { get; set; }
            public string reviewer { get; set; }
            public int dataStatus { get; set; }
            public int? crownStatus { get; set; }
            public bool? crownSeasonalDormant { get; set; }
            public bool? crownDeadBranch { get; set; }
            public decimal? crownDeadBranchPercent { get; set; }
            public bool? crownPest { get; set; }
            public bool? crownForeignObject { get; set; }
            public string crownOtherNote { get; set; }
            public int? trunkStatus { get; set; }
            public bool? trunkBarkDamage { get; set; }
            public bool? trunkDecay { get; set; }
            public bool? trunkTermiteTrail { get; set; }
            public bool? trunkLean { get; set; }
            public bool? trunkFungus { get; set; }
            public bool? trunkGummosis { get; set; }
            public bool? trunkVine { get; set; }
            public string trunkOtherNote { get; set; }
            public int? rootStatus { get; set; }
            public bool? rootDamage { get; set; }
            public bool? rootDecay { get; set; }
            public bool? rootExpose { get; set; }
            public bool? rootRot { get; set; }
            public bool? rootSucker { get; set; }
            public string rootOtherNote { get; set; }
            public int? envStatus { get; set; }
            public bool? envPitSmall { get; set; }
            public bool? envPaved { get; set; }
            public bool? envDebris { get; set; }
            public bool? envSoilCover { get; set; }
            public bool? envCompaction { get; set; }
            public bool? envWaterlog { get; set; }
            public bool? envNearFacility { get; set; }
            public string envOtherNote { get; set; }
            public int? adjacentStatus { get; set; }
            public bool? adjacentBuilding { get; set; }
            public bool? adjacentWire { get; set; }
            public bool? adjacentSignal { get; set; }
            public string adjacentOtherNote { get; set; }
            public int? task1Status { get; set; }
            public string task1Note { get; set; }
            public int? task2Status { get; set; }
            public string task2Note { get; set; }
            public int? task3Status { get; set; }
            public string task3Note { get; set; }
            public int? task4Status { get; set; }
            public string task4Note { get; set; }
            public int? task5Status { get; set; }
            public string task5Note { get; set; }
            public int insertAccountID { get; set; }
            public DateTime insertDateTime { get; set; }
            public int? updateAccountID { get; set; }
            public DateTime? updateDateTime { get; set; }
        }

        public class CarePhotoRecord
        {
            public int photoID { get; set; }
            public int careID { get; set; }
            public string itemName { get; set; }
            public string beforeFileName { get; set; }
            public string beforeFilePath { get; set; }
            public int? beforeFileSize { get; set; }
            public string afterFileName { get; set; }
            public string afterFilePath { get; set; }
            public int? afterFileSize { get; set; }
            public DateTime? insertDateTime { get; set; }
        }

        public List<CareMainQueryResult> GetCareMainList(CareMainQueryFilter filter, int currentUserId)
        {
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            string baseSql = @"
                SELECT 
                    record.treeID, record.systemTreeNo, record.agencyTreeNo, record.agencyJurisdictionCode,
                    record.manager, record.treeStatus,

                    COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                    areaInfo.area AS areaName,

                    species.commonName AS speciesName,

                    latest_care.careID,
                    latest_care.careDate,
                    latest_care.recorder,
                    latest_care.reviewer,
                    latest_care.dataStatus,
                    COALESCE(latest_care.updateDateTime, latest_care.insertDateTime) AS lastUpdate,

                    CASE WHEN batch.treeID IS NOT NULL THEN 1 ELSE 0 END AS isAdded

                FROM Tree_Record record

                OUTER APPLY (
                    SELECT TOP 1 c.*
                    FROM Tree_CareRecord c
                    WHERE c.treeID = record.treeID AND c.removeDateTime IS NULL
                    ORDER BY c.careDate DESC, c.careID DESC
                ) latest_care

                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
                LEFT JOIN Tree_CareBatchSetting batch ON record.treeID = batch.treeID AND batch.accountID = @currentUserId
            ";

            parameters.Add(new SqlParameter("@currentUserId", currentUserId));

            whereClauses.Add("record.removeDateTime IS NULL");
            whereClauses.Add("record.editStatus = 1");

            if (filter != null)
            {
                if (filter.cityID.HasValue)
                {
                    whereClauses.Add("record.cityID = @cityID");
                    parameters.Add(new SqlParameter("@cityID", filter.cityID));
                }

                if (filter.areaID.HasValue)
                {
                    whereClauses.Add("record.areaID = @areaID");
                    parameters.Add(new SqlParameter("@areaID", filter.areaID));
                }

                if (filter.speciesID.HasValue)
                {
                    whereClauses.Add("record.speciesID = @speciesID");
                    parameters.Add(new SqlParameter("@speciesID", filter.speciesID));
                }

                if (!string.IsNullOrWhiteSpace(filter.keyword))
                {
                    string kwSql = @"(
                        record.systemTreeNo LIKE @kw OR
                        record.agencyTreeNo LIKE @kw OR
                        record.agencyJurisdictionCode LIKE @kw OR
                        record.manager LIKE @kw OR
                        latest_care.recorder LIKE @kw OR
                        latest_care.reviewer LIKE @kw
                    )";
                    whereClauses.Add(kwSql);
                    parameters.Add(new SqlParameter("@kw", "%" + filter.keyword.Trim() + "%"));
                }

                switch (filter.queryOption)
                {
                    case "Never":
                        whereClauses.Add("latest_care.careID IS NULL");
                        break;
                    case "NoRecord180":
                    case "NoRecord365":
                        whereClauses.Add("(latest_care.careDate IS NULL OR latest_care.careDate < DATEADD(day, -365, CAST(GETDATE() AS date)))");
                        break;
                    default:
                        break;
                }
            }

            string finalSql = $"{baseSql} WHERE {string.Join(" AND ", whereClauses)}";

            string dir = (filter?.sortDirection == "DESC") ? "DESC" : "ASC";
            string sortSql;
            if (string.IsNullOrEmpty(filter?.sortExpression) || filter.sortExpression == "DefaultSort")
            {
                sortSql = "ORDER BY (CASE WHEN latest_care.careID IS NULL THEN 0 ELSE 1 END) ASC, latest_care.careDate DESC, record.systemTreeNo ASC";
            }
            else if (filter.sortExpression == "areaID")
            {
                sortSql = $"ORDER BY record.areaID {dir}";
            }
            else
            {
                string sortField = filter.sortExpression;
                if (sortField == "careDate" || sortField == "recorder" || sortField == "reviewer" || sortField == "dataStatus")
                {
                    sortField = "latest_care." + sortField;
                }
                else if (!sortField.Contains("."))
                {
                    sortField = "record." + sortField;
                }
                sortSql = $"ORDER BY {sortField} {dir}";
            }

            finalSql += " " + sortSql;

            var result = new List<CareMainQueryResult>();
            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(finalSql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    var item = new CareMainQueryResult
                    {
                        treeID = GetNullableInt(row, "treeID") ?? 0,
                        systemTreeNo = GetString(row, "systemTreeNo"),
                        agencyTreeNo = GetString(row, "agencyTreeNo"),
                        agencyJurisdictionCode = GetString(row, "agencyJurisdictionCode"),
                        cityName = GetString(row, "cityName"),
                        areaName = GetString(row, "areaName"),
                        speciesName = GetString(row, "speciesName"),
                        manager = GetString(row, "manager"),
                        careID = GetNullableInt(row, "careID"),
                        careDate = GetNullableDateTime(row, "careDate"),
                        recorder = GetString(row, "recorder"),
                        reviewer = GetString(row, "reviewer"),
                        dataStatus = GetNullableInt(row, "dataStatus"),
                        lastUpdate = GetNullableDateTime(row, "lastUpdate"),
                        isAdded = GetNullableInt(row, "isAdded") == 1
                    };

                    string statusText = GetString(row, "treeStatus");
                    if (!string.IsNullOrEmpty(statusText) && Enum.TryParse(statusText, out TreeStatus parsedStatus))
                    {
                        item.treeStatus = parsedStatus;
                    }

                    result.Add(item);
                }
            }

            return result;
        }

        public List<CareBatchSettingResult> GetCareBatchSetting(int accountId)
        {
            string sql = @"
                SELECT s.settingID, s.insertDateTime,
                       record.treeID,
                       record.systemTreeNo,
                       record.agencyTreeNo,
                       record.agencyJurisdictionCode,

                       COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                       areaInfo.area AS areaName,
                       species.commonName AS speciesName,
                       record.manager,

                       latest_care.careDate,
                       latest_care.recorder

                FROM Tree_CareBatchSetting s
                JOIN Tree_Record record ON s.treeID = record.treeID

                OUTER APPLY (
                    SELECT TOP 1 c.careDate, c.recorder
                    FROM Tree_CareRecord c
                    WHERE c.treeID = record.treeID AND c.removeDateTime IS NULL
                    ORDER BY c.careDate DESC, c.careID DESC
                ) latest_care

                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID

                WHERE s.accountID = @accountID
                ORDER BY s.insertDateTime DESC
            ";

            var result = new List<CareBatchSettingResult>();

            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@accountID", accountId));
                foreach (DataRow row in dt.Rows)
                {
                    var item = new CareBatchSettingResult
                    {
                        settingID = GetNullableInt(row, "settingID") ?? 0,
                        insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                        treeID = GetNullableInt(row, "treeID") ?? 0,
                        systemTreeNo = GetString(row, "systemTreeNo"),
                        agencyTreeNo = GetString(row, "agencyTreeNo"),
                        agencyJurisdictionCode = GetString(row, "agencyJurisdictionCode"),
                        cityName = GetString(row, "cityName"),
                        areaName = GetString(row, "areaName"),
                        speciesName = GetString(row, "speciesName"),
                        manager = GetString(row, "manager"),
                        careDate = GetNullableDateTime(row, "careDate"),
                        recorder = GetString(row, "recorder")
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        public void AddToCareBatchSetting(int accountId, int treeId)
        {
            const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM Tree_CareBatchSetting WHERE accountID = @accountID AND treeID = @treeID)
            BEGIN
                INSERT INTO Tree_CareBatchSetting (accountID, treeID)
                VALUES (@accountID, @treeID)
            END
        ";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountID", accountId),
                    new SqlParameter("@treeID", treeId));
            }
        }

        public void RemoveFromCareBatchSetting(int accountId, int? treeId = null)
        {
            string sql;
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@accountID", accountId)
            };

            if (treeId.HasValue)
            {
                sql = "DELETE FROM Tree_CareBatchSetting WHERE accountID = @accountID AND treeID = @treeID";
                parameters.Add(new SqlParameter("@treeID", treeId.Value));
            }
            else
            {
                sql = "DELETE FROM Tree_CareBatchSetting WHERE accountID = @accountID";
            }

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql, parameters.ToArray());
            }
        }

        public List<CareRecordListResult> GetCareRecordList(CareRecordListFilter filter, int currentUserId)
        {
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            string sql = @"
                SELECT
                    care.careID,
                    care.careDate,
                    care.recorder,
                    care.reviewer,
                    care.dataStatus,
                    COALESCE(care.updateDateTime, care.insertDateTime) AS lastUpdate,

                    record.treeID,
                    record.systemTreeNo,
                    record.agencyTreeNo,

                    COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                    areaInfo.area AS areaName,

                    species.commonName AS speciesName,
                    record.manager,
                    record.areaID

                FROM Tree_CareRecord care
                JOIN Tree_Record record ON care.treeID = record.treeID

                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
            ";

            whereClauses.Add("care.removeDateTime IS NULL");
            whereClauses.Add("record.removeDateTime IS NULL");
            whereClauses.Add("record.editStatus = 1");

            if (filter != null)
            {
                if (filter.scope == "My")
                {
                    whereClauses.Add("care.insertAccountID = @userID");
                    parameters.Add(new SqlParameter("@userID", currentUserId));
                }

                if (filter.dataStatus.HasValue)
                {
                    whereClauses.Add("care.dataStatus = @dataStatus");
                    parameters.Add(new SqlParameter("@dataStatus", (int)filter.dataStatus.Value));
                }

                if (filter.dateStart.HasValue)
                {
                    whereClauses.Add("care.careDate >= @dStart");
                    parameters.Add(new SqlParameter("@dStart", filter.dateStart.Value));
                }

                if (filter.dateEnd.HasValue)
                {
                    whereClauses.Add("care.careDate <= @dEnd");
                    parameters.Add(new SqlParameter("@dEnd", filter.dateEnd.Value));
                }

                if (filter.cityID.HasValue)
                {
                    whereClauses.Add("record.cityID = @cityID");
                    parameters.Add(new SqlParameter("@cityID", filter.cityID));
                }

                if (filter.areaID.HasValue)
                {
                    whereClauses.Add("record.areaID = @areaID");
                    parameters.Add(new SqlParameter("@areaID", filter.areaID));
                }

                if (filter.speciesID.HasValue)
                {
                    whereClauses.Add("record.speciesID = @speciesID");
                    parameters.Add(new SqlParameter("@speciesID", filter.speciesID));
                }

                if (!string.IsNullOrWhiteSpace(filter.keyword))
                {
                    string kwSql = @"(
                        record.systemTreeNo LIKE @kw OR
                        record.agencyTreeNo LIKE @kw OR
                        record.manager LIKE @kw OR
                        care.recorder LIKE @kw OR
                        care.reviewer LIKE @kw
                    )";
                    whereClauses.Add(kwSql);
                    parameters.Add(new SqlParameter("@kw", "%" + filter.keyword.Trim() + "%"));
                }
            }

            sql += $" WHERE {string.Join(" AND ", whereClauses)}";

            string sortField = filter?.sortExpression;
            string sortDir = (filter?.sortDirection == "ASC") ? "ASC" : "DESC";

            if (string.IsNullOrEmpty(sortField) || sortField == "DefaultSort")
            {
                sql += " ORDER BY care.careDate DESC, record.systemTreeNo DESC";
            }
            else
            {
                if (sortField == "careDate" || sortField == "recorder" || sortField == "reviewer" || sortField == "lastUpdate" || sortField == "dataStatus")
                {
                    sortField = "care." + sortField;
                }
                else if (sortField != "speciesName" && sortField != "cityName" && sortField != "areaName")
                {
                    sortField = "record." + sortField;
                }

                sql += $" ORDER BY {sortField} {sortDir}";
            }

            var result = new List<CareRecordListResult>();
            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    var item = new CareRecordListResult
                    {
                        careID = GetNullableInt(row, "careID") ?? 0,
                        careDate = GetNullableDateTime(row, "careDate"),
                        recorder = GetString(row, "recorder"),
                        reviewer = GetString(row, "reviewer"),
                        dataStatus = GetNullableInt(row, "dataStatus"),
                        lastUpdate = GetNullableDateTime(row, "lastUpdate"),

                        treeID = GetNullableInt(row, "treeID") ?? 0,
                        systemTreeNo = GetString(row, "systemTreeNo"),
                        agencyTreeNo = GetString(row, "agencyTreeNo"),
                        cityName = GetString(row, "cityName"),
                        areaName = GetString(row, "areaName"),
                        speciesName = GetString(row, "speciesName"),
                        manager = GetString(row, "manager"),
                        areaID = GetNullableInt(row, "areaID")
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        public List<CareRecordListResult> GetCareRecordsByTree(int treeId)
        {
            const string sql = @"
                SELECT
                    careID,
                    careDate,
                    recorder,
                    reviewer,
                    dataStatus,
                    COALESCE(updateDateTime, insertDateTime) AS lastUpdate
                FROM Tree_CareRecord
                WHERE treeID = @treeID AND removeDateTime IS NULL
                ORDER BY careDate DESC, careID DESC";

            var result = new List<CareRecordListResult>();
            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@treeID", treeId));
                foreach (DataRow row in dt.Rows)
                {
                    var item = new CareRecordListResult
                    {
                        careID = GetNullableInt(row, "careID") ?? 0,
                        careDate = GetNullableDateTime(row, "careDate"),
                        recorder = GetString(row, "recorder"),
                        reviewer = GetString(row, "reviewer"),
                        dataStatus = GetNullableInt(row, "dataStatus"),
                        lastUpdate = GetNullableDateTime(row, "lastUpdate")
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        public CareRecord GetCareRecord(int careId)
        {
            const string sql = @"SELECT TOP 1 * FROM Tree_CareRecord WHERE careID=@id AND removeDateTime IS NULL";
            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", careId));
                if (dt.Rows.Count != 1) return null;

                var row = dt.Rows[0];
                return new CareRecord
                {
                    careID = GetNullableInt(row, "careID") ?? 0,
                    treeID = GetNullableInt(row, "treeID") ?? 0,
                    careDate = GetNullableDateTime(row, "careDate"),
                    recorder = GetString(row, "recorder"),
                    reviewer = GetString(row, "reviewer"),
                    dataStatus = GetNullableInt(row, "dataStatus") ?? 0,
                    crownStatus = GetNullableInt(row, "crownStatus"),
                    crownSeasonalDormant = GetNullableBoolean(row, "crownSeasonalDormant"),
                    crownDeadBranch = GetNullableBoolean(row, "crownDeadBranch"),
                    crownDeadBranchPercent = GetNullableDecimal(row, "crownDeadBranchPercent"),
                    crownPest = GetNullableBoolean(row, "crownPest"),
                    crownForeignObject = GetNullableBoolean(row, "crownForeignObject"),
                    crownOtherNote = GetString(row, "crownOtherNote"),
                    trunkStatus = GetNullableInt(row, "trunkStatus"),
                    trunkBarkDamage = GetNullableBoolean(row, "trunkBarkDamage"),
                    trunkDecay = GetNullableBoolean(row, "trunkDecay"),
                    trunkTermiteTrail = GetNullableBoolean(row, "trunkTermiteTrail"),
                    trunkLean = GetNullableBoolean(row, "trunkLean"),
                    trunkFungus = GetNullableBoolean(row, "trunkFungus"),
                    trunkGummosis = GetNullableBoolean(row, "trunkGummosis"),
                    trunkVine = GetNullableBoolean(row, "trunkVine"),
                    trunkOtherNote = GetString(row, "trunkOtherNote"),
                    rootStatus = GetNullableInt(row, "rootStatus"),
                    rootDamage = GetNullableBoolean(row, "rootDamage"),
                    rootDecay = GetNullableBoolean(row, "rootDecay"),
                    rootExpose = GetNullableBoolean(row, "rootExpose"),
                    rootRot = GetNullableBoolean(row, "rootRot"),
                    rootSucker = GetNullableBoolean(row, "rootSucker"),
                    rootOtherNote = GetString(row, "rootOtherNote"),
                    envStatus = GetNullableInt(row, "envStatus"),
                    envPitSmall = GetNullableBoolean(row, "envPitSmall"),
                    envPaved = GetNullableBoolean(row, "envPaved"),
                    envDebris = GetNullableBoolean(row, "envDebris"),
                    envSoilCover = GetNullableBoolean(row, "envSoilCover"),
                    envCompaction = GetNullableBoolean(row, "envCompaction"),
                    envWaterlog = GetNullableBoolean(row, "envWaterlog"),
                    envNearFacility = GetNullableBoolean(row, "envNearFacility"),
                    envOtherNote = GetString(row, "envOtherNote"),
                    adjacentStatus = GetNullableInt(row, "adjacentStatus"),
                    adjacentBuilding = GetNullableBoolean(row, "adjacentBuilding"),
                    adjacentWire = GetNullableBoolean(row, "adjacentWire"),
                    adjacentSignal = GetNullableBoolean(row, "adjacentSignal"),
                    adjacentOtherNote = GetString(row, "adjacentOtherNote"),
                    task1Status = GetNullableInt(row, "task1Status"),
                    task1Note = GetString(row, "task1Note"),
                    task2Status = GetNullableInt(row, "task2Status"),
                    task2Note = GetString(row, "task2Note"),
                    task3Status = GetNullableInt(row, "task3Status"),
                    task3Note = GetString(row, "task3Note"),
                    task4Status = GetNullableInt(row, "task4Status"),
                    task4Note = GetString(row, "task4Note"),
                    task5Status = GetNullableInt(row, "task5Status"),
                    task5Note = GetString(row, "task5Note"),
                    insertAccountID = GetNullableInt(row, "insertAccountID") ?? 0,
                    insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                    updateAccountID = GetNullableInt(row, "updateAccountID"),
                    updateDateTime = GetNullableDateTime(row, "updateDateTime")
                };
            }
        }

        public int InsertCareRecord(CareRecord record, int accountId)
        {
            const string sql = @"
INSERT INTO Tree_CareRecord(
    treeID, careDate, recorder, reviewer, dataStatus,
    crownStatus, crownSeasonalDormant, crownDeadBranch, crownDeadBranchPercent, crownPest, crownForeignObject, crownOtherNote,
    trunkStatus, trunkBarkDamage, trunkDecay, trunkTermiteTrail, trunkLean, trunkFungus, trunkGummosis, trunkVine, trunkOtherNote,
    rootStatus, rootDamage, rootDecay, rootExpose, rootRot, rootSucker, rootOtherNote,
    envStatus, envPitSmall, envPaved, envDebris, envSoilCover, envCompaction, envWaterlog, envNearFacility, envOtherNote,
    adjacentStatus, adjacentBuilding, adjacentWire, adjacentSignal, adjacentOtherNote,
    task1Status, task1Note, task2Status, task2Note, task3Status, task3Note, task4Status, task4Note, task5Status, task5Note,
    insertAccountID
)
VALUES(
    @treeID, @careDate, @recorder, @reviewer, @dataStatus,
    @crownStatus, @crownSeasonalDormant, @crownDeadBranch, @crownDeadBranchPercent, @crownPest, @crownForeignObject, @crownOtherNote,
    @trunkStatus, @trunkBarkDamage, @trunkDecay, @trunkTermiteTrail, @trunkLean, @trunkFungus, @trunkGummosis, @trunkVine, @trunkOtherNote,
    @rootStatus, @rootDamage, @rootDecay, @rootExpose, @rootRot, @rootSucker, @rootOtherNote,
    @envStatus, @envPitSmall, @envPaved, @envDebris, @envSoilCover, @envCompaction, @envWaterlog, @envNearFacility, @envOtherNote,
    @adjacentStatus, @adjacentBuilding, @adjacentWire, @adjacentSignal, @adjacentOtherNote,
    @task1Status, @task1Note, @task2Status, @task2Note, @task3Status, @task3Note, @task4Status, @task4Note, @task5Status, @task5Note,
    @insertAccountID
);
SELECT SCOPE_IDENTITY();";

            using (var da = new DataAccess.MS_SQL())
            {
                var id = da.ExcuteScalar(sql,
                    new SqlParameter("@treeID", record.treeID),
                    new SqlParameter("@careDate", record.careDate),
                    new SqlParameter("@recorder", ToDbValue(record.recorder)),
                    new SqlParameter("@reviewer", ToDbValue(record.reviewer)),
                    new SqlParameter("@dataStatus", record.dataStatus),
                    new SqlParameter("@crownStatus", ToDbValue(record.crownStatus)),
                    new SqlParameter("@crownSeasonalDormant", ToDbValue(record.crownSeasonalDormant)),
                    new SqlParameter("@crownDeadBranch", ToDbValue(record.crownDeadBranch)),
                    new SqlParameter("@crownDeadBranchPercent", ToDbValue(record.crownDeadBranchPercent)),
                    new SqlParameter("@crownPest", ToDbValue(record.crownPest)),
                    new SqlParameter("@crownForeignObject", ToDbValue(record.crownForeignObject)),
                    new SqlParameter("@crownOtherNote", ToDbValue(record.crownOtherNote)),
                    new SqlParameter("@trunkStatus", ToDbValue(record.trunkStatus)),
                    new SqlParameter("@trunkBarkDamage", ToDbValue(record.trunkBarkDamage)),
                    new SqlParameter("@trunkDecay", ToDbValue(record.trunkDecay)),
                    new SqlParameter("@trunkTermiteTrail", ToDbValue(record.trunkTermiteTrail)),
                    new SqlParameter("@trunkLean", ToDbValue(record.trunkLean)),
                    new SqlParameter("@trunkFungus", ToDbValue(record.trunkFungus)),
                    new SqlParameter("@trunkGummosis", ToDbValue(record.trunkGummosis)),
                    new SqlParameter("@trunkVine", ToDbValue(record.trunkVine)),
                    new SqlParameter("@trunkOtherNote", ToDbValue(record.trunkOtherNote)),
                    new SqlParameter("@rootStatus", ToDbValue(record.rootStatus)),
                    new SqlParameter("@rootDamage", ToDbValue(record.rootDamage)),
                    new SqlParameter("@rootDecay", ToDbValue(record.rootDecay)),
                    new SqlParameter("@rootExpose", ToDbValue(record.rootExpose)),
                    new SqlParameter("@rootRot", ToDbValue(record.rootRot)),
                    new SqlParameter("@rootSucker", ToDbValue(record.rootSucker)),
                    new SqlParameter("@rootOtherNote", ToDbValue(record.rootOtherNote)),
                    new SqlParameter("@envStatus", ToDbValue(record.envStatus)),
                    new SqlParameter("@envPitSmall", ToDbValue(record.envPitSmall)),
                    new SqlParameter("@envPaved", ToDbValue(record.envPaved)),
                    new SqlParameter("@envDebris", ToDbValue(record.envDebris)),
                    new SqlParameter("@envSoilCover", ToDbValue(record.envSoilCover)),
                    new SqlParameter("@envCompaction", ToDbValue(record.envCompaction)),
                    new SqlParameter("@envWaterlog", ToDbValue(record.envWaterlog)),
                    new SqlParameter("@envNearFacility", ToDbValue(record.envNearFacility)),
                    new SqlParameter("@envOtherNote", ToDbValue(record.envOtherNote)),
                    new SqlParameter("@adjacentStatus", ToDbValue(record.adjacentStatus)),
                    new SqlParameter("@adjacentBuilding", ToDbValue(record.adjacentBuilding)),
                    new SqlParameter("@adjacentWire", ToDbValue(record.adjacentWire)),
                    new SqlParameter("@adjacentSignal", ToDbValue(record.adjacentSignal)),
                    new SqlParameter("@adjacentOtherNote", ToDbValue(record.adjacentOtherNote)),
                    new SqlParameter("@task1Status", ToDbValue(record.task1Status)),
                    new SqlParameter("@task1Note", ToDbValue(record.task1Note)),
                    new SqlParameter("@task2Status", ToDbValue(record.task2Status)),
                    new SqlParameter("@task2Note", ToDbValue(record.task2Note)),
                    new SqlParameter("@task3Status", ToDbValue(record.task3Status)),
                    new SqlParameter("@task3Note", ToDbValue(record.task3Note)),
                    new SqlParameter("@task4Status", ToDbValue(record.task4Status)),
                    new SqlParameter("@task4Note", ToDbValue(record.task4Note)),
                    new SqlParameter("@task5Status", ToDbValue(record.task5Status)),
                    new SqlParameter("@task5Note", ToDbValue(record.task5Note)),
                    new SqlParameter("@insertAccountID", accountId));

                return Convert.ToInt32(id);
            }
        }

        public void UpdateCareRecord(CareRecord record, int accountId)
        {
            const string sql = @"
UPDATE Tree_CareRecord SET
    careDate=@careDate,
    recorder=@recorder,
    reviewer=@reviewer,
    dataStatus=@dataStatus,
    crownStatus=@crownStatus,
    crownSeasonalDormant=@crownSeasonalDormant,
    crownDeadBranch=@crownDeadBranch,
    crownDeadBranchPercent=@crownDeadBranchPercent,
    crownPest=@crownPest,
    crownForeignObject=@crownForeignObject,
    crownOtherNote=@crownOtherNote,
    trunkStatus=@trunkStatus,
    trunkBarkDamage=@trunkBarkDamage,
    trunkDecay=@trunkDecay,
    trunkTermiteTrail=@trunkTermiteTrail,
    trunkLean=@trunkLean,
    trunkFungus=@trunkFungus,
    trunkGummosis=@trunkGummosis,
    trunkVine=@trunkVine,
    trunkOtherNote=@trunkOtherNote,
    rootStatus=@rootStatus,
    rootDamage=@rootDamage,
    rootDecay=@rootDecay,
    rootExpose=@rootExpose,
    rootRot=@rootRot,
    rootSucker=@rootSucker,
    rootOtherNote=@rootOtherNote,
    envStatus=@envStatus,
    envPitSmall=@envPitSmall,
    envPaved=@envPaved,
    envDebris=@envDebris,
    envSoilCover=@envSoilCover,
    envCompaction=@envCompaction,
    envWaterlog=@envWaterlog,
    envNearFacility=@envNearFacility,
    envOtherNote=@envOtherNote,
    adjacentStatus=@adjacentStatus,
    adjacentBuilding=@adjacentBuilding,
    adjacentWire=@adjacentWire,
    adjacentSignal=@adjacentSignal,
    adjacentOtherNote=@adjacentOtherNote,
    task1Status=@task1Status,
    task1Note=@task1Note,
    task2Status=@task2Status,
    task2Note=@task2Note,
    task3Status=@task3Status,
    task3Note=@task3Note,
    task4Status=@task4Status,
    task4Note=@task4Note,
    task5Status=@task5Status,
    task5Note=@task5Note,
    updateAccountID=@updateAccountID,
    updateDateTime=GETDATE()
WHERE careID=@careID AND removeDateTime IS NULL;";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@careID", record.careID),
                    new SqlParameter("@careDate", record.careDate),
                    new SqlParameter("@recorder", ToDbValue(record.recorder)),
                    new SqlParameter("@reviewer", ToDbValue(record.reviewer)),
                    new SqlParameter("@dataStatus", record.dataStatus),
                    new SqlParameter("@crownStatus", ToDbValue(record.crownStatus)),
                    new SqlParameter("@crownSeasonalDormant", ToDbValue(record.crownSeasonalDormant)),
                    new SqlParameter("@crownDeadBranch", ToDbValue(record.crownDeadBranch)),
                    new SqlParameter("@crownDeadBranchPercent", ToDbValue(record.crownDeadBranchPercent)),
                    new SqlParameter("@crownPest", ToDbValue(record.crownPest)),
                    new SqlParameter("@crownForeignObject", ToDbValue(record.crownForeignObject)),
                    new SqlParameter("@crownOtherNote", ToDbValue(record.crownOtherNote)),
                    new SqlParameter("@trunkStatus", ToDbValue(record.trunkStatus)),
                    new SqlParameter("@trunkBarkDamage", ToDbValue(record.trunkBarkDamage)),
                    new SqlParameter("@trunkDecay", ToDbValue(record.trunkDecay)),
                    new SqlParameter("@trunkTermiteTrail", ToDbValue(record.trunkTermiteTrail)),
                    new SqlParameter("@trunkLean", ToDbValue(record.trunkLean)),
                    new SqlParameter("@trunkFungus", ToDbValue(record.trunkFungus)),
                    new SqlParameter("@trunkGummosis", ToDbValue(record.trunkGummosis)),
                    new SqlParameter("@trunkVine", ToDbValue(record.trunkVine)),
                    new SqlParameter("@trunkOtherNote", ToDbValue(record.trunkOtherNote)),
                    new SqlParameter("@rootStatus", ToDbValue(record.rootStatus)),
                    new SqlParameter("@rootDamage", ToDbValue(record.rootDamage)),
                    new SqlParameter("@rootDecay", ToDbValue(record.rootDecay)),
                    new SqlParameter("@rootExpose", ToDbValue(record.rootExpose)),
                    new SqlParameter("@rootRot", ToDbValue(record.rootRot)),
                    new SqlParameter("@rootSucker", ToDbValue(record.rootSucker)),
                    new SqlParameter("@rootOtherNote", ToDbValue(record.rootOtherNote)),
                    new SqlParameter("@envStatus", ToDbValue(record.envStatus)),
                    new SqlParameter("@envPitSmall", ToDbValue(record.envPitSmall)),
                    new SqlParameter("@envPaved", ToDbValue(record.envPaved)),
                    new SqlParameter("@envDebris", ToDbValue(record.envDebris)),
                    new SqlParameter("@envSoilCover", ToDbValue(record.envSoilCover)),
                    new SqlParameter("@envCompaction", ToDbValue(record.envCompaction)),
                    new SqlParameter("@envWaterlog", ToDbValue(record.envWaterlog)),
                    new SqlParameter("@envNearFacility", ToDbValue(record.envNearFacility)),
                    new SqlParameter("@envOtherNote", ToDbValue(record.envOtherNote)),
                    new SqlParameter("@adjacentStatus", ToDbValue(record.adjacentStatus)),
                    new SqlParameter("@adjacentBuilding", ToDbValue(record.adjacentBuilding)),
                    new SqlParameter("@adjacentWire", ToDbValue(record.adjacentWire)),
                    new SqlParameter("@adjacentSignal", ToDbValue(record.adjacentSignal)),
                    new SqlParameter("@adjacentOtherNote", ToDbValue(record.adjacentOtherNote)),
                    new SqlParameter("@task1Status", ToDbValue(record.task1Status)),
                    new SqlParameter("@task1Note", ToDbValue(record.task1Note)),
                    new SqlParameter("@task2Status", ToDbValue(record.task2Status)),
                    new SqlParameter("@task2Note", ToDbValue(record.task2Note)),
                    new SqlParameter("@task3Status", ToDbValue(record.task3Status)),
                    new SqlParameter("@task3Note", ToDbValue(record.task3Note)),
                    new SqlParameter("@task4Status", ToDbValue(record.task4Status)),
                    new SqlParameter("@task4Note", ToDbValue(record.task4Note)),
                    new SqlParameter("@task5Status", ToDbValue(record.task5Status)),
                    new SqlParameter("@task5Note", ToDbValue(record.task5Note)),
                    new SqlParameter("@updateAccountID", accountId));
            }
        }

        public List<CarePhotoRecord> GetCarePhotos(int careId)
        {
            const string sql = @"SELECT * FROM Tree_CarePhoto WHERE careID=@careID AND removeDateTime IS NULL ORDER BY photoID";
            var photos = new List<CarePhotoRecord>();
            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@careID", careId));
                foreach (DataRow row in dt.Rows)
                {
                    photos.Add(new CarePhotoRecord
                    {
                        photoID = GetNullableInt(row, "photoID") ?? 0,
                        careID = GetNullableInt(row, "careID") ?? 0,
                        itemName = GetString(row, "itemName"),
                        beforeFileName = GetString(row, "beforeFileName"),
                        beforeFilePath = GetString(row, "beforeFilePath"),
                        beforeFileSize = GetNullableInt(row, "beforeFileSize"),
                        afterFileName = GetString(row, "afterFileName"),
                        afterFilePath = GetString(row, "afterFilePath"),
                        afterFileSize = GetNullableInt(row, "afterFileSize"),
                        insertDateTime = GetNullableDateTime(row, "insertDateTime")
                    });
                }
            }

            return photos;
        }

        public int InsertCarePhoto(CarePhotoRecord photo, int accountId)
        {
            const string sql = @"
INSERT INTO Tree_CarePhoto(
    careID, itemName,
    beforeFileName, beforeFilePath, beforeFileSize,
    afterFileName, afterFilePath, afterFileSize,
    insertAccountID
)
VALUES(
    @careID, @itemName,
    @beforeFileName, @beforeFilePath, @beforeFileSize,
    @afterFileName, @afterFilePath, @afterFileSize,
    @insertAccountID
);
SELECT SCOPE_IDENTITY();";

            using (var da = new DataAccess.MS_SQL())
            {
                var id = da.ExcuteScalar(sql,
                    new SqlParameter("@careID", photo.careID),
                    new SqlParameter("@itemName", ToDbValue(photo.itemName)),
                    new SqlParameter("@beforeFileName", ToDbValue(photo.beforeFileName)),
                    new SqlParameter("@beforeFilePath", ToDbValue(photo.beforeFilePath)),
                    new SqlParameter("@beforeFileSize", ToDbValue(photo.beforeFileSize)),
                    new SqlParameter("@afterFileName", ToDbValue(photo.afterFileName)),
                    new SqlParameter("@afterFilePath", ToDbValue(photo.afterFilePath)),
                    new SqlParameter("@afterFileSize", ToDbValue(photo.afterFileSize)),
                    new SqlParameter("@insertAccountID", accountId));

                return Convert.ToInt32(id);
            }
        }

        public void UpdateCarePhoto(CarePhotoRecord photo)
        {
            const string sql = @"
UPDATE Tree_CarePhoto SET
    itemName=@itemName,
    beforeFileName=@beforeFileName,
    beforeFilePath=@beforeFilePath,
    beforeFileSize=@beforeFileSize,
    afterFileName=@afterFileName,
    afterFilePath=@afterFilePath,
    afterFileSize=@afterFileSize
WHERE photoID=@photoID AND removeDateTime IS NULL;";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@photoID", photo.photoID),
                    new SqlParameter("@itemName", ToDbValue(photo.itemName)),
                    new SqlParameter("@beforeFileName", ToDbValue(photo.beforeFileName)),
                    new SqlParameter("@beforeFilePath", ToDbValue(photo.beforeFilePath)),
                    new SqlParameter("@beforeFileSize", ToDbValue(photo.beforeFileSize)),
                    new SqlParameter("@afterFileName", ToDbValue(photo.afterFileName)),
                    new SqlParameter("@afterFilePath", ToDbValue(photo.afterFilePath)),
                    new SqlParameter("@afterFileSize", ToDbValue(photo.afterFileSize)));
            }
        }

        public void SoftDeleteCarePhoto(int photoId, int accountId)
        {
            const string sql = @"UPDATE Tree_CarePhoto SET removeDateTime=GETDATE(), removeAccountID=@accountId WHERE photoID=@photoID AND removeDateTime IS NULL;";
            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@photoID", photoId));
            }
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        public void DeleteCareRecord(int careId, int accountId)
        {
            const string sql = @"
UPDATE Tree_CareRecord
SET removeDateTime=GETDATE(), removeAccountID=@accountId
WHERE careID=@careID AND removeDateTime IS NULL;

UPDATE Tree_CarePhoto
SET removeDateTime=GETDATE(), removeAccountID=@accountId
WHERE careID=@careID AND removeDateTime IS NULL;";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@careID", careId));
            }
        }
    }
}
