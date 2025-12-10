using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using DataAccess;
using protectTreesV2.User;

namespace protectTreesV2.Log
{
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
