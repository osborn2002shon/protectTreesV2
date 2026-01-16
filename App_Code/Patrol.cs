using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;
using static protectTreesV2.Base.DataRowHelper;

namespace protectTreesV2.Patrol
{
    public class Patrol
    {
        public enum PatrolRecordStatus
        {
            草稿 = 0,
            定稿 = 1
        }

        [Serializable]
        public class PatrolMainQueryFilter
        {
            public int? cityID { get; set; }
            public int? areaID { get; set; }
            public int? speciesID { get; set; }
            public string keyword { get; set; }
            public string queryOption { get; set; } = "NoRecord180"; // All / NoRecord180 / Never
            public string sortExpression { get; set; }
            public string sortDirection { get; set; }
        }

        public class PatrolMainQueryResult
        {
            public int treeID { get; set; }
            public int? patrolID { get; set; }
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

            public DateTime? patrolDate { get; set; }
            public string patroller { get; set; }
            public bool? hasPublicSafetyRisk { get; set; }
            public DateTime? lastUpdate { get; set; }
            public bool isAdded { get; set; }

            public string patrolRecordStatusText =>
                !patrolID.HasValue || patrolID.Value == 0
                    ? "--"
                    : (dataStatus.GetValueOrDefault() == (int)PatrolRecordStatus.定稿 ? "定稿" : "草稿");
        }

        public class PatrolBatchSettingResult
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
            public DateTime? patrolDate { get; set; }
            public string patroller { get; set; }
        }

        [Serializable]
        public class PatrolRecordListFilter
        {
            public string scope { get; set; } = "Unit"; // My / Unit
            public int? cityID { get; set; }
            public int? areaID { get; set; }
            public int? speciesID { get; set; }
            public DateTime? dateStart { get; set; }
            public DateTime? dateEnd { get; set; }
            public string keyword { get; set; }
            public string sortExpression { get; set; }
            public string sortDirection { get; set; }
        }

        public class PatrolRecordListResult
        {
            public int patrolID { get; set; }
            public DateTime? patrolDate { get; set; }
            public string patroller { get; set; }
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
                => dataStatus.GetValueOrDefault() == (int)PatrolRecordStatus.定稿 ? "定稿" : "草稿";
        }

        public class PatrolRecord
        {
            public int patrolID { get; set; }
            public int treeID { get; set; }
            public DateTime? patrolDate { get; set; }
            public string patroller { get; set; }
            public int dataStatus { get; set; }
            public string memo { get; set; }
            public bool hasPublicSafetyRisk { get; set; }
            public string sourceUnit { get; set; }
            public int? sourceUnitID { get; set; }
            public int insertAccountID { get; set; }
            public DateTime insertDateTime { get; set; }
            public int? updateAccountID { get; set; }
            public DateTime? updateDateTime { get; set; }
        }

        public class PatrolPhoto
        {
            public int PhotoID { get; set; }
            public int PatrolID { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public int? FileSize { get; set; }
            public string Caption { get; set; }
        }

        public List<PatrolMainQueryResult> GetPatrolMainList(PatrolMainQueryFilter filter, int currentUserId)
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

                    latest_patrol.patrolID,
                    latest_patrol.patrolDate,
                    latest_patrol.patroller,
                    latest_patrol.dataStatus,
                    latest_patrol.hasPublicSafetyRisk,
                    COALESCE(latest_patrol.updateDateTime, latest_patrol.insertDateTime) AS lastUpdate,

                    CASE WHEN batch.treeID IS NOT NULL THEN 1 ELSE 0 END AS isAdded

                FROM Tree_Record record

                OUTER APPLY (
                    SELECT TOP 1 p.*
                    FROM Tree_PatrolRecord p
                    WHERE p.treeID = record.treeID AND p.removeDateTime IS NULL
                    ORDER BY p.patrolDate DESC, p.patrolID DESC
                ) latest_patrol

                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
                LEFT JOIN Tree_PatrolBatchSetting batch ON record.treeID = batch.treeID AND batch.accountID = @currentUserId
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
                        latest_patrol.patroller LIKE @kw
                    )";
                    whereClauses.Add(kwSql);
                    parameters.Add(new SqlParameter("@kw", "%" + filter.keyword.Trim() + "%"));
                }

                switch (filter.queryOption)
                {
                    case "Never":
                        whereClauses.Add("latest_patrol.patrolID IS NULL");
                        break;
                    case "NoRecord180":
                        whereClauses.Add("(latest_patrol.patrolDate IS NULL OR latest_patrol.patrolDate < DATEADD(day, -180, CAST(GETDATE() AS date)))");
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
                sortSql = "ORDER BY (CASE WHEN latest_patrol.patrolID IS NULL THEN 0 ELSE 1 END) ASC, latest_patrol.patrolDate DESC, record.systemTreeNo ASC";
            }
            else if (filter.sortExpression == "areaID")
            {
                sortSql = $"ORDER BY record.areaID {dir}";
            }
            else
            {
                string sortField = filter.sortExpression;
                if (sortField == "patrolDate" || sortField == "patroller" || sortField == "hasPublicSafetyRisk" || sortField == "dataStatus")
                {
                    sortField = "latest_patrol." + sortField;
                }
                else if (!sortField.Contains("."))
                {
                    sortField = "record." + sortField;
                }
                sortSql = $"ORDER BY {sortField} {dir}";
            }

            finalSql += " " + sortSql;

            var result = new List<PatrolMainQueryResult>();
            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(finalSql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    var item = new PatrolMainQueryResult
                    {
                        treeID = GetNullableInt(row, "treeID") ?? 0,
                        systemTreeNo = GetString(row, "systemTreeNo"),
                        agencyTreeNo = GetString(row, "agencyTreeNo"),
                        agencyJurisdictionCode = GetString(row, "agencyJurisdictionCode"),
                        cityName = GetString(row, "cityName"),
                        areaName = GetString(row, "areaName"),
                        speciesName = GetString(row, "speciesName"),
                        manager = GetString(row, "manager"),
                        patrolID = GetNullableInt(row, "patrolID"),
                        patrolDate = GetNullableDateTime(row, "patrolDate"),
                        patroller = GetString(row, "patroller"),
                        dataStatus = GetNullableInt(row, "dataStatus"),
                        hasPublicSafetyRisk = GetNullableBoolean(row, "hasPublicSafetyRisk"),
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

        public List<PatrolBatchSettingResult> GetPatrolBatchSetting(int accountId)
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

                       latest_patrol.patrolDate,
                       latest_patrol.patroller

                FROM Tree_PatrolBatchSetting s
                JOIN Tree_Record record ON s.treeID = record.treeID

                OUTER APPLY (
                    SELECT TOP 1 p.patrolDate, p.patroller
                    FROM Tree_PatrolRecord p
                    WHERE p.treeID = record.treeID AND p.removeDateTime IS NULL
                    ORDER BY p.patrolDate DESC, p.patrolID DESC
                ) latest_patrol

                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID

                WHERE s.accountID = @accountID
                ORDER BY s.insertDateTime DESC
            ";

            var result = new List<PatrolBatchSettingResult>();

            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@accountID", accountId));
                foreach (DataRow row in dt.Rows)
                {
                    var item = new PatrolBatchSettingResult
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
                        patrolDate = GetNullableDateTime(row, "patrolDate"),
                        patroller = GetString(row, "patroller")
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        public void AddToPatrolBatchSetting(int accountId, int treeId)
        {
            const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM Tree_PatrolBatchSetting WHERE accountID = @accountID AND treeID = @treeID)
            BEGIN
                INSERT INTO Tree_PatrolBatchSetting (accountID, treeID)
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

        public void RemoveFromPatrolBatchSetting(int accountId, int? treeId = null)
        {
            string sql;
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@accountID", accountId)
            };

            if (treeId.HasValue)
            {
                sql = "DELETE FROM Tree_PatrolBatchSetting WHERE accountID = @accountID AND treeID = @treeID";
                parameters.Add(new SqlParameter("@treeID", treeId.Value));
            }
            else
            {
                sql = "DELETE FROM Tree_PatrolBatchSetting WHERE accountID = @accountID";
            }

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql, parameters.ToArray());
            }
        }

        public List<PatrolRecordListResult> GetPatrolRecordList(PatrolRecordListFilter filter, int currentUserId)
        {
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            string sql = @"
                SELECT
                    patrol.patrolID,
                    patrol.patrolDate,
                    patrol.patroller,
                    patrol.dataStatus,
                    COALESCE(patrol.updateDateTime, patrol.insertDateTime) AS lastUpdate,

                    record.treeID,
                    record.systemTreeNo,
                    record.agencyTreeNo,

                    COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                    areaInfo.area AS areaName,

                    species.commonName AS speciesName,
                    record.manager,
                    record.areaID

                FROM Tree_PatrolRecord patrol
                JOIN Tree_Record record ON patrol.treeID = record.treeID

                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
            ";

            whereClauses.Add("patrol.removeDateTime IS NULL");
            whereClauses.Add("record.removeDateTime IS NULL");
            whereClauses.Add("record.editStatus = 1");

            if (filter != null)
            {
                if (filter.scope == "My")
                {
                    whereClauses.Add("patrol.insertAccountID = @userID");
                    parameters.Add(new SqlParameter("@userID", currentUserId));
                }

                if (filter.dateStart.HasValue)
                {
                    whereClauses.Add("patrol.patrolDate >= @dStart");
                    parameters.Add(new SqlParameter("@dStart", filter.dateStart.Value));
                }

                if (filter.dateEnd.HasValue)
                {
                    whereClauses.Add("patrol.patrolDate <= @dEnd");
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
                        patrol.patroller LIKE @kw
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
                sql += " ORDER BY patrol.patrolDate DESC, record.systemTreeNo DESC";
            }
            else
            {
                if (sortField == "patrolDate" || sortField == "patroller" || sortField == "lastUpdate" || sortField == "dataStatus")
                {
                    sortField = "patrol." + sortField;
                }
                else if (sortField != "speciesName" && sortField != "cityName" && sortField != "areaName")
                {
                    sortField = "record." + sortField;
                }

                sql += $" ORDER BY {sortField} {sortDir}";
            }

            var result = new List<PatrolRecordListResult>();
            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    var item = new PatrolRecordListResult
                    {
                        patrolID = GetNullableInt(row, "patrolID") ?? 0,
                        patrolDate = GetNullableDateTime(row, "patrolDate"),
                        patroller = GetString(row, "patroller"),
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

        public PatrolRecord GetPatrolRecord(int patrolId)
        {
            const string sql = @"SELECT TOP 1 * FROM Tree_PatrolRecord WHERE patrolID=@id AND removeDateTime IS NULL";
            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", patrolId));
                if (dt.Rows.Count != 1) return null;

                var row = dt.Rows[0];
                return new PatrolRecord
                {
                    patrolID = GetNullableInt(row, "patrolID") ?? 0,
                    treeID = GetNullableInt(row, "treeID") ?? 0,
                    patrolDate = GetNullableDateTime(row, "patrolDate"),
                    patroller = GetString(row, "patroller"),
                    dataStatus = GetNullableInt(row, "dataStatus") ?? 0,
                    memo = GetString(row, "memo"),
                    hasPublicSafetyRisk = GetBoolean(row, "hasPublicSafetyRisk"),
                    sourceUnit = GetString(row, "sourceUnit"),
                    sourceUnitID = GetNullableInt(row, "sourceUnitID"),
                    insertAccountID = GetNullableInt(row, "insertAccountID") ?? 0,
                    insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                    updateAccountID = GetNullableInt(row, "updateAccountID"),
                    updateDateTime = GetNullableDateTime(row, "updateDateTime")
                };
            }
        }

        public List<PatrolRecord> GetPatrolRecordsByTree(int treeId)
        {
            const string sql = @"SELECT * FROM Tree_PatrolRecord WHERE treeID=@treeId AND removeDateTime IS NULL ORDER BY patrolDate DESC, patrolID DESC";
            var result = new List<PatrolRecord>();

            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@treeId", treeId));
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new PatrolRecord
                    {
                        patrolID = GetNullableInt(row, "patrolID") ?? 0,
                        treeID = GetNullableInt(row, "treeID") ?? 0,
                        patrolDate = GetNullableDateTime(row, "patrolDate"),
                        patroller = GetString(row, "patroller"),
                        dataStatus = GetNullableInt(row, "dataStatus") ?? 0,
                        memo = GetString(row, "memo"),
                        hasPublicSafetyRisk = GetBoolean(row, "hasPublicSafetyRisk"),
                        sourceUnit = GetString(row, "sourceUnit"),
                        sourceUnitID = GetNullableInt(row, "sourceUnitID"),
                        insertAccountID = GetNullableInt(row, "insertAccountID") ?? 0,
                        insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                        updateAccountID = GetNullableInt(row, "updateAccountID"),
                        updateDateTime = GetNullableDateTime(row, "updateDateTime")
                    });
                }
            }

            return result;
        }

        public List<PatrolPhoto> GetPatrolPhotos(int patrolId)
        {
            const string sql = @"SELECT * FROM Tree_PatrolPhoto WHERE patrolID=@id AND removeDateTime IS NULL ORDER BY insertDateTime";
            var result = new List<PatrolPhoto>();

            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", patrolId));
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new PatrolPhoto
                    {
                        PhotoID = GetNullableInt(row, "photoID") ?? 0,
                        PatrolID = GetNullableInt(row, "patrolID") ?? 0,
                        FileName = GetString(row, "fileName"),
                        FilePath = GetString(row, "filePath"),
                        FileSize = GetNullableInt(row, "fileSize"),
                        Caption = GetString(row, "caption")
                    });
                }
            }

            return result;
        }

        public int InsertPatrolRecord(PatrolRecord record, int accountId)
        {
            const string sql = @"
INSERT INTO Tree_PatrolRecord(treeID, patrolDate, patroller, dataStatus, memo, hasPublicSafetyRisk, sourceUnit, sourceUnitID, insertAccountID, insertDateTime)
VALUES(@treeID, @patrolDate, @patroller, @dataStatus, @memo, @hasPublicSafetyRisk, @sourceUnit, @sourceUnitID, @accountId, GETDATE());
SELECT SCOPE_IDENTITY();";

            using (var da = new DataAccess.MS_SQL())
            {
                object result = da.ExcuteScalar(sql,
                    new SqlParameter("@treeID", record.treeID),
                    new SqlParameter("@patrolDate", (object)record.patrolDate ?? DBNull.Value),
                    new SqlParameter("@patroller", (object)record.patroller ?? DBNull.Value),
                    new SqlParameter("@dataStatus", record.dataStatus),
                    new SqlParameter("@memo", (object)record.memo ?? DBNull.Value),
                    new SqlParameter("@hasPublicSafetyRisk", record.hasPublicSafetyRisk),
                    new SqlParameter("@sourceUnit", (object)record.sourceUnit ?? DBNull.Value),
                    new SqlParameter("@sourceUnitID", (object)record.sourceUnitID ?? DBNull.Value),
                    new SqlParameter("@accountId", accountId));

                return Convert.ToInt32(result);
            }
        }

        public void UpdatePatrolRecord(PatrolRecord record, int accountId)
        {
            const string sql = @"
UPDATE Tree_PatrolRecord
SET patrolDate=@patrolDate,
    patroller=@patroller,
    dataStatus=@dataStatus,
    memo=@memo,
    hasPublicSafetyRisk=@hasPublicSafetyRisk,
    updateAccountID=@accountId,
    updateDateTime=GETDATE()
WHERE patrolID=@patrolID AND removeDateTime IS NULL";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@patrolDate", (object)record.patrolDate ?? DBNull.Value),
                    new SqlParameter("@patroller", (object)record.patroller ?? DBNull.Value),
                    new SqlParameter("@dataStatus", record.dataStatus),
                    new SqlParameter("@memo", (object)record.memo ?? DBNull.Value),
                    new SqlParameter("@hasPublicSafetyRisk", record.hasPublicSafetyRisk),
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@patrolID", record.patrolID));
            }
        }

        public void DeletePatrolRecord(int patrolId, int accountId)
        {
            const string sql = @"
UPDATE Tree_PatrolRecord
SET removeDateTime=GETDATE(), removeAccountID=@accountId
WHERE patrolID=@patrolID AND removeDateTime IS NULL;

UPDATE Tree_PatrolPhoto
SET removeDateTime=GETDATE(), removeAccountID=@accountId
WHERE patrolID=@patrolID AND removeDateTime IS NULL;";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@patrolID", patrolId));
            }
        }

        public int InsertPatrolPhoto(PatrolPhoto photo, int accountId)
        {
            const string sql = @"
INSERT INTO Tree_PatrolPhoto(patrolID, fileName, filePath, fileSize, caption, insertAccountID)
VALUES(@patrolID, @fileName, @filePath, @fileSize, @caption, @accountId);
SELECT SCOPE_IDENTITY();";

            using (var da = new DataAccess.MS_SQL())
            {
                object result = da.ExcuteScalar(sql,
                    new SqlParameter("@patrolID", photo.PatrolID),
                    new SqlParameter("@fileName", (object)photo.FileName ?? DBNull.Value),
                    new SqlParameter("@filePath", (object)photo.FilePath ?? DBNull.Value),
                    new SqlParameter("@fileSize", (object)photo.FileSize ?? DBNull.Value),
                    new SqlParameter("@caption", (object)photo.Caption ?? DBNull.Value),
                    new SqlParameter("@accountId", accountId));

                return Convert.ToInt32(result);
            }
        }

        public void UpdatePatrolPhotoCaption(int photoId, string caption, int accountId)
        {
            const string sql = @"UPDATE Tree_PatrolPhoto SET caption=@caption WHERE photoID=@id AND removeDateTime IS NULL";
            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@caption", (object)caption ?? DBNull.Value),
                    new SqlParameter("@id", photoId));
            }
        }

        public void SoftDeletePatrolPhoto(int photoId, int accountId)
        {
            const string sql = @"UPDATE Tree_PatrolPhoto SET removeDateTime=GETDATE(), removeAccountID=@accountId WHERE photoID=@id AND removeDateTime IS NULL";
            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountId", accountId),
                    new SqlParameter("@id", photoId));
            }
        }

        public List<string> GetRiskNotificationEmails(int areaID)
        {
            const string sql = @"
            select distinct System_UserAccount.email from System_UserAccount
            inner join System_UnitCityMapping on System_UserAccount.unitID = System_UnitCityMapping.unitID
            where (System_UserAccount.auTypeID = 4 or System_UserAccount.auTypeID = 1)
            and System_UserAccount.isActive = 1
            and System_UserAccount.removeDateTime is null
            and System_UnitCityMapping.twID = @areaID";

            var emails = new List<string>();
            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@areaID", areaID.ToString(CultureInfo.InvariantCulture)));
                foreach (DataRow row in dt.Rows)
                {
                    var mail = GetString(row, "email");
                    if (!string.IsNullOrWhiteSpace(mail))
                    {
                        emails.Add(mail);
                    }
                }
            }
            return emails;
        }
    }
}
