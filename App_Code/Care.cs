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
            public string queryOption { get; set; } = "NoRecord180";
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

            public string careRecordStatusText =>
                !careID.HasValue || careID.Value == 0
                    ? "--"
                    : (dataStatus.GetValueOrDefault() == (int)CareRecordStatus.定稿 ? "定稿" : "草稿");
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
            public int insertAccountID { get; set; }
            public DateTime insertDateTime { get; set; }
            public int? updateAccountID { get; set; }
            public DateTime? updateDateTime { get; set; }
        }

        public List<CareMainQueryResult> GetCareMainList(CareMainQueryFilter filter, int currentUserId)
        {
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            string baseSql = @"
                SELECT 
                    record.treeID, record.systemTreeNo, record.agencyTreeNo, record.agencyJurisdictionCode,
                    record.manager, record.treeStatus,

                    COALESCE(areaInfo.city, cityInfo.city, record.cityName) AS cityName,
                    COALESCE(areaInfo.area, record.areaName) AS areaName,

                    COALESCE(species.commonName, record.speciesCommonName) AS speciesName,

                    latest_care.careID,
                    latest_care.careDate,
                    latest_care.recorder,
                    latest_care.reviewer,
                    latest_care.dataStatus,
                    COALESCE(latest_care.updateDateTime, latest_care.insertDateTime) AS lastUpdate

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
            ";

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
                        whereClauses.Add("(latest_care.careDate IS NULL OR latest_care.careDate < DATEADD(day, -180, CAST(GETDATE() AS date)))");
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
                        lastUpdate = GetNullableDateTime(row, "lastUpdate")
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

                    COALESCE(areaInfo.city, cityInfo.city, record.cityName) AS cityName,
                    COALESCE(areaInfo.area, record.areaName) AS areaName,

                    COALESCE(species.commonName, record.speciesCommonName) AS speciesName,
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
                    insertAccountID = GetNullableInt(row, "insertAccountID") ?? 0,
                    insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                    updateAccountID = GetNullableInt(row, "updateAccountID"),
                    updateDateTime = GetNullableDateTime(row, "updateDateTime")
                };
            }
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
