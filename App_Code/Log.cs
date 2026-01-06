using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using DataAccess;
using protectTreesV2.User;

namespace protectTreesV2.Log
{
    public static class LogFunctionTypes
    {
        public const string TreeCatalog = "樹籍";
        public const string Health = "健檢";
        public const string Patrol = "巡查";
        public const string Care = "養護";
    }

    public class FunctionLogEntry
    {
        public int LogID { get; set; }
        public string FunctionType { get; set; }
        public int DataID { get; set; }
        public string ActionType { get; set; }
        public string Memo { get; set; }
        public string IPAddress { get; set; }
        public int? AccountID { get; set; }
        public string Account { get; set; }
        public string AccountName { get; set; }
        public string AccountUnit { get; set; }
        public DateTime LogDateTime { get; set; }
    }

    public static class FunctionLogService
    {
        private const string SelectLogSql = @"SELECT logID, functionType, dataID, actionType, memo, ipAddress, accountID, account, accountName, accountUnit, logDateTime
                                              FROM Tree_Log
                                              WHERE functionType=@functionType AND dataID=@dataID
                                              ORDER BY logDateTime DESC, logID DESC";

        private const string InsertLogSql = @"INSERT INTO Tree_Log(functionType, dataID, actionType, memo, ipAddress, accountID, account, accountName, accountUnit)
                                              VALUES(@functionType, @dataID, @actionType, @memo, @ipAddress, @accountID, @account, @accountName, @accountUnit)";

        public static List<FunctionLogEntry> GetLogs(string functionType, int dataId)
        {
            var logs = new List<FunctionLogEntry>();
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(SelectLogSql,
                    new SqlParameter("@functionType", functionType ?? string.Empty),
                    new SqlParameter("@dataID", dataId));
                foreach (DataRow row in dt.Rows)
                {
                    logs.Add(MapRow(row));
                }
            }
            return logs;
        }

        public static void InsertLog(string functionType, int dataId, string actionType, string memo, string ipAddress, int? accountId, string account, string accountName, string accountUnit)
        {
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(InsertLogSql,
                    new SqlParameter("@functionType", functionType ?? string.Empty),
                    new SqlParameter("@dataID", dataId),
                    new SqlParameter("@actionType", actionType ?? string.Empty),
                    new SqlParameter("@memo", ToDbValue(memo)),
                    new SqlParameter("@ipAddress", ToDbValue(ipAddress)),
                    new SqlParameter("@accountID", ToDbValue(accountId)),
                    new SqlParameter("@account", ToDbValue(account)),
                    new SqlParameter("@accountName", ToDbValue(accountName)),
                    new SqlParameter("@accountUnit", ToDbValue(accountUnit)));
            }
        }

        private static FunctionLogEntry MapRow(DataRow row)
        {
            return new FunctionLogEntry
            {
                LogID = Convert.ToInt32(row["logID"]),
                FunctionType = row["functionType"] as string,
                DataID = Convert.ToInt32(row["dataID"]),
                ActionType = row["actionType"] as string,
                Memo = row["memo"] as string,
                IPAddress = row["ipAddress"] as string,
                AccountID = row["accountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["accountID"]),
                Account = row["account"] as string,
                AccountName = row["accountName"] as string,
                AccountUnit = row["accountUnit"] as string,
                LogDateTime = Convert.ToDateTime(row["logDateTime"])
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }

    public static class OperationLogger
    {
        public static void InsertLog(int userID, string logItem, string logType, string memo)
        {
            string sql = "INSERT INTO User_Log(accountID, logDateTime, IP, logItem, logType, memo) VALUES(@id, @dt, @ip, @item, @type, @memo)";
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@id", userID),
                    new SqlParameter("@dt", DateTime.Now),
                    new SqlParameter("@ip", HttpContext.Current?.Request?.UserHostAddress ?? string.Empty),
                    new SqlParameter("@item", logItem),
                    new SqlParameter("@type", logType),
                    new SqlParameter("@memo", memo));
            }
        }

        public static void InsertLog(string logItem, string logType, string memo = "")
        {
            var user = UserService.GetCurrentUser();
            if (user != null)
            {
                InsertLog(user.userID, logItem, logType, memo);
            }
        }

        public static void InsertLog(Account user, string logItem, string logType, string memo = "")
        {
            if (user != null)
            {
                InsertLog(user.userID, logItem, logType, memo);
            }
        }

        public static DateTime? GetLastLoginTime(int userID)
        {
            string sql = "SELECT TOP 1 logDateTime FROM User_Log WHERE accountID=@id AND logType='登入' ORDER BY logDateTime DESC";
            using (var da = new MS_SQL())
            {
                var obj = da.ExcuteScalar(sql, new SqlParameter("@id", userID));
                if (obj != null && obj != DBNull.Value)
                {
                    return Convert.ToDateTime(obj);
                }
            }
            return null;
        }

        public static int GetFailedLoginCount(int userID)
        {
            var lastLogin = GetLastLoginTime(userID);
            DateTime start = DateTime.Now.AddMinutes(-15);
            if (lastLogin.HasValue && lastLogin.Value > start)
            {
                start = lastLogin.Value;
            }

            string sql = "SELECT COUNT(*) FROM User_Log WHERE accountID=@id AND logType='登入失敗' AND logDateTime >= @dt";
            using (var da = new MS_SQL())
            {
                var obj = da.ExcuteScalar(sql,
                    new SqlParameter("@id", userID),
                    new SqlParameter("@dt", start));
                return Convert.ToInt32(obj);
            }
        }

        public static bool IsLockedOut(int userID)
        {
            return GetFailedLoginCount(userID) >= 5;
        }
    }
}
