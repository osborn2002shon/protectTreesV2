using DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace protectTreesV2
{
    public class UserLog
    {

        /// <summary>
        /// 操作紀錄：動作類型
        /// </summary>
        public enum enum_UserLogType
        {
            新增, 刪除, 修改, 查詢, 下載, 其他, 上傳
        }

        /// <summary>
        /// 操作紀錄：功能名稱
        /// </summary>
        public enum enum_UserLogItem
        {
            登入, 登出, 
            樹籍基本資料管理, 健檢紀錄管理, 巡查紀錄管理, 養護紀錄管理, 我的帳號管理, 系統帳號管理
        }

        /// <summary>
        /// 新增使用者操作紀錄
        /// </summary>
        public static void Insert_UserLog(int accountID, enum_UserLogItem logItem, enum_UserLogType logType, string memo = "", DateTime? logDateTime = null)
        {
            string sqlString =
                "insert into System_UserLog " +
                "(accountID, logDateTime, IP, logItem, logType, memo) values " +
                "(@accountID, @logDateTime, @IP, @logItem, @logType, @memo)";

            List<SqlParameter> para = new List<SqlParameter>
            {
                new SqlParameter("@accountID", accountID),
                new SqlParameter("@IP", Account.Get_IP()),
                new SqlParameter("@logItem", logItem.ToString()),
                new SqlParameter("@logType", logType.ToString()),
                new SqlParameter("@memo", string.IsNullOrWhiteSpace(memo) ? (object)DBNull.Value : memo),
                new SqlParameter("@logDateTime", logDateTime ?? DateTime.Now)
            };
            //para.Add(new SqlParameter("@logDateTime", DateTime.Now));

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sqlString, para.ToArray());
            }
        }
    }

    public class TreeLog
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

}