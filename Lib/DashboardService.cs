using System;
using System.Data;
using System.Data.SqlClient;
using DataAccess;

namespace protectTreesV2.Dashboard
{
    public class PendingAccountSummary
    {
        public int PendingCount { get; set; }
        public int LastMonthApproved { get; set; }
        public int CurrentMonthApproved { get; set; }
    }

    public class MonthlySummary
    {
        public int TotalCount { get; set; }
        public int LastMonthCount { get; set; }
        public int CurrentMonthCount { get; set; }
    }

    public static class DashboardService
    {
        public static PendingAccountSummary GetPendingAccountSummary(int unitId)
        {
            var summary = new PendingAccountSummary();
            if (unitId <= 0)
            {
                return summary;
            }

            var now = DateTime.Now;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = currentMonthStart.AddMonths(-1);

            const string sql = @"
                SELECT
                    SUM(CASE WHEN v.verifyStatus IS NULL THEN 1 ELSE 0 END) AS PendingCount,
                    SUM(CASE WHEN v.verifyStatus = 1 AND v.verifyDateTime >= @lastMonthStart AND v.verifyDateTime < @currentMonthStart THEN 1 ELSE 0 END) AS LastMonthApproved,
                    SUM(CASE WHEN v.verifyStatus = 1 AND v.verifyDateTime >= @currentMonthStart THEN 1 ELSE 0 END) AS CurrentMonthApproved
                FROM View_UserInfo v
                INNER JOIN System_UnitUnitMapping m ON v.unitID = m.unitID
                --WHERE m.manageUnitID = @unitID";

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql,
                    new SqlParameter("@unitID", unitId),
                    new SqlParameter("@lastMonthStart", lastMonthStart),
                    new SqlParameter("@currentMonthStart", currentMonthStart));

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    summary.PendingCount = row["PendingCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["PendingCount"]);
                    summary.LastMonthApproved = row["LastMonthApproved"] == DBNull.Value ? 0 : Convert.ToInt32(row["LastMonthApproved"]);
                    summary.CurrentMonthApproved = row["CurrentMonthApproved"] == DBNull.Value ? 0 : Convert.ToInt32(row["CurrentMonthApproved"]);
                }
            }

            return summary;
        }

        public static MonthlySummary GetTreeRecordSummary(int unitId)
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN r.insertDateTime >= @lastMonthStart AND r.insertDateTime < @currentMonthStart THEN 1 ELSE 0 END) AS LastMonthCount,
                    SUM(CASE WHEN r.insertDateTime >= @currentMonthStart THEN 1 ELSE 0 END) AS CurrentMonthCount
                FROM Tree_Record r
                INNER JOIN System_UnitCityMapping map ON r.areaID = map.twID AND map.unitID = @unitID
                WHERE r.editStatus = 1 AND r.removeDateTime IS NULL";

            return GetMonthlySummary(sql, unitId);
        }

        public static MonthlySummary GetHealthRecordSummary(int unitId)
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN hr.insertDateTime >= @lastMonthStart AND hr.insertDateTime < @currentMonthStart THEN 1 ELSE 0 END) AS LastMonthCount,
                    SUM(CASE WHEN hr.insertDateTime >= @currentMonthStart THEN 1 ELSE 0 END) AS CurrentMonthCount
                FROM Tree_HealthRecord hr
                INNER JOIN Tree_Record r ON hr.treeID = r.treeID
                INNER JOIN System_UnitCityMapping map ON r.areaID = map.twID AND map.unitID = @unitID
                WHERE hr.dataStatus = 1 AND hr.removeDateTime IS NULL AND r.removeDateTime IS NULL";

            return GetMonthlySummary(sql, unitId);
        }

        public static MonthlySummary GetPatrolRecordSummary(int unitId)
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN pr.insertDateTime >= @lastMonthStart AND pr.insertDateTime < @currentMonthStart THEN 1 ELSE 0 END) AS LastMonthCount,
                    SUM(CASE WHEN pr.insertDateTime >= @currentMonthStart THEN 1 ELSE 0 END) AS CurrentMonthCount
                FROM Tree_PatrolRecord pr
                INNER JOIN Tree_Record r ON pr.treeID = r.treeID
                INNER JOIN System_UnitCityMapping map ON r.areaID = map.twID AND map.unitID = @unitID
                WHERE pr.dataStatus = 1 AND pr.removeDateTime IS NULL AND r.removeDateTime IS NULL";

            return GetMonthlySummary(sql, unitId);
        }

        public static MonthlySummary GetCareRecordSummary(int unitId)
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN cr.insertDateTime >= @lastMonthStart AND cr.insertDateTime < @currentMonthStart THEN 1 ELSE 0 END) AS LastMonthCount,
                    SUM(CASE WHEN cr.insertDateTime >= @currentMonthStart THEN 1 ELSE 0 END) AS CurrentMonthCount
                FROM Tree_CareRecord cr
                INNER JOIN Tree_Record r ON cr.treeID = r.treeID
                INNER JOIN System_UnitCityMapping map ON r.areaID = map.twID AND map.unitID = @unitID
                WHERE cr.dataStatus = 1 AND cr.removeDateTime IS NULL AND r.removeDateTime IS NULL";

            return GetMonthlySummary(sql, unitId);
        }

        public static int GetTreeStatusCount(int unitId, string status)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Tree_Record r
                INNER JOIN System_UnitCityMapping map ON r.areaID = map.twID AND map.unitID = @unitID
                WHERE r.editStatus = 1 AND r.removeDateTime IS NULL AND r.treeStatus = @status";

            using (var da = new MS_SQL())
            {
                var result = da.ExcuteScalar(sql,
                    new SqlParameter("@unitID", unitId),
                    new SqlParameter("@status", status ?? string.Empty));

                return result == DBNull.Value || result == null ? 0 : Convert.ToInt32(result);
            }
        }

        private static MonthlySummary GetMonthlySummary(string sql, int unitId)
        {
            var summary = new MonthlySummary();
            if (unitId <= 0)
            {
                return summary;
            }

            var now = DateTime.Now;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = currentMonthStart.AddMonths(-1);

            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql,
                    new SqlParameter("@unitID", unitId),
                    new SqlParameter("@lastMonthStart", lastMonthStart),
                    new SqlParameter("@currentMonthStart", currentMonthStart));

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    summary.TotalCount = row["TotalCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalCount"]);
                    summary.LastMonthCount = row["LastMonthCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["LastMonthCount"]);
                    summary.CurrentMonthCount = row["CurrentMonthCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["CurrentMonthCount"]);
                }
            }

            return summary;
        }
    }
}
