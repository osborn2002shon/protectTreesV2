using System;
using System.Collections.Generic;
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
            新增, 刪除, 修改, 查詢, 下載, 其他
        }

        /// <summary>
        /// 操作紀錄：功能名稱
        /// </summary>
        public enum enum_UserLogItem
        {
            登入, 登出, 
            樹籍基本資料管理, 健檢紀錄管理, 巡查紀錄管理, 養護紀錄管理
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

    }

}