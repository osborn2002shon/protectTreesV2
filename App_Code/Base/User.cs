using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;

namespace protectTreesV2
{

    public class Account
    {

        public class stru_accountInfo
        {
            public int accountID { get; set; }
            public string accountType { get; set; }
            public string account { get; set; }
            public bool isExist { get; set; }
            public bool? verifyStatus { get; set; }
            public bool isActive { get; set; }
            public DateTime? lastUpdatePWDateTime { get; set; }
            public DateTime? lastLoginDateTime { get; set; }
            public bool isFirstLogin { get; set; }
            public bool isNeedChangePW { get; set; }
            public bool isAutoStopLogin { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public int auTypeID { get; set; }
            public string auTypeName { get; set; }
            public int unitID { get; set; }
            public string unitGroup { get; set; }
            public string unitName{ get; set; }

            // ===== SSO =====
            public string APP_USER_NODE_UUID { get; set; }
            public string APP_COMPANY_UUID { get; set; }
            public string APP_COMPANY_UUID_n { get; set; }
            public string APP_DEPT_NODE_UUID { get; set; }
            public string APP_DEPT_NODE_UUID_n { get; set; }
            public string APP_USER_LOGIN_ID { get; set; }
        }

        /// <summary>
        /// 抓取使用者的IP位置
        /// </summary>
        /// <returns></returns>
        public static string Get_IP()
        {
            return HttpContext.Current?.Request?.UserHostAddress ?? string.Empty;
        }

        /// <summary>
        /// 取得加密加鹽的密碼字串
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Get_HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                string _pw_salt = "protectTrees";
                var bytes = System.Text.Encoding.UTF8.GetBytes(password + _pw_salt);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// 取得一般帳號狀態資訊（用於登入前檢查）
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public stru_accountInfo Get_AccountInfo(string account, string hashPW)
        {
            var result = new stru_accountInfo
            {
                isExist = false
            };

            //剛申請的帳號不會有密碼，不可用來登入 > 視同查無帳號
            //只會撈一般帳號，不撈SSO的，避免哪天有可能出現SSO跟一般帳號相撞
            string sqlString = @"
                select * from View_UserInfo
                where accountType = 'default' and password is not null and account = @account and password = @hashPW";

            var para = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("account", account),
                    new System.Data.SqlClient.SqlParameter("hashPW", hashPW)
                };

            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sqlString, para.ToArray());

                if (dt.Rows.Count == 0)
                    return result;

                var row = dt.Rows[0];
                result.isExist = true;

                // ===== 基本識別 =====
                result.accountID = row["accountID"] == DBNull.Value ? 0 : (int)row["accountID"];
                result.accountType = row["accountType"].ToString();
                result.account = row["account"].ToString();
                result.name = row["name"]?.ToString();
                result.email = row["email"]?.ToString();

                // ===== 狀態 =====
                result.verifyStatus = row["verifyStatus"] == DBNull.Value
                    ? (bool?)null
                    : (bool)row["verifyStatus"];

                result.isActive = (bool)row["isActive"];

                // ===== 時間欄位 =====
                result.lastUpdatePWDateTime = row["lastUpdatePWDateTime"] == DBNull.Value
                    ? (DateTime?)null
                    : (DateTime)row["lastUpdatePWDateTime"];

                result.lastLoginDateTime = row["lastLoginDateTime"] == DBNull.Value
                    ? (DateTime?)null
                    : (DateTime)row["lastLoginDateTime"];

                result.isFirstLogin = (bool)row["isFirstLogin"];
                result.isNeedChangePW = (bool)row["isNeedChangePW"]; 
                result.isAutoStopLogin = (bool)row["isAutoStopLogin"];

                // ===== 權限 / 單位 =====
                result.auTypeID = (int)row["auTypeID"];
                result.auTypeName = row["auTypeName"].ToString();
                result.unitID = (int)row["unitID"];
                result.unitGroup = row["unitGroup"].ToString();
                result.unitName = row["unitName"].ToString();

                // ===== SSO（可為NULL）=====
                result.APP_USER_NODE_UUID =
                    row["APP_USER_NODE_UUID"] == DBNull.Value
                        ? null
                        : row["APP_USER_NODE_UUID"].ToString();

                result.APP_COMPANY_UUID =
                    row["APP_COMPANY_UUID"] == DBNull.Value
                        ? null
                        : row["APP_COMPANY_UUID"].ToString();

                result.APP_COMPANY_UUID_n =
                    row["APP_COMPANY_UUID_n"] == DBNull.Value
                        ? null
                        : row["APP_COMPANY_UUID_n"].ToString();

                result.APP_DEPT_NODE_UUID =
                    row["APP_DEPT_NODE_UUID"] == DBNull.Value
                        ? null
                        : row["APP_DEPT_NODE_UUID"].ToString();

                result.APP_DEPT_NODE_UUID_n =
                    row["APP_DEPT_NODE_UUID_n"] == DBNull.Value
                        ? null
                        : row["APP_DEPT_NODE_UUID_n"].ToString();

                result.APP_USER_LOGIN_ID =
                    row["APP_USER_LOGIN_ID"] == DBNull.Value
                        ? null
                        : row["APP_USER_LOGIN_ID"].ToString();
            }

            return result;
        }

        /// <summary>
        /// 取得帳號詳細資訊
        /// </summary>
        /// <param name="accountID">使用者ID</param>
        /// <returns></returns>
        public stru_accountInfo Get_AccountInfo(int accountID)
        {
            var result = new stru_accountInfo
            {
                isExist = false
            };

            string sqlString = @"
            SELECT 
                [accountID], [accountType], [account], [name], [email],
                [verifyStatus], [isActive], 
                [lastUpdatePWDateTime], [lastLoginDateTime],
                [isFirstLogin], [isNeedChangePW], [isAutoStopLogin],
                [auTypeID], [auTypeName], 
                [unitID], [unitGroup], [unitName],
                [APP_USER_NODE_UUID], [APP_COMPANY_UUID], [APP_COMPANY_UUID_n],
                [APP_DEPT_NODE_UUID], [APP_DEPT_NODE_UUID_n], [APP_USER_LOGIN_ID]
            FROM [View_UserInfo]
            WHERE [accountID] = @accountID";

            var para = new List<System.Data.SqlClient.SqlParameter>
            {
                new System.Data.SqlClient.SqlParameter("@accountID", accountID)
            };

            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sqlString, para.ToArray());

                if (dt.Rows.Count == 0)
                    return result;

                var row = dt.Rows[0];
                result.isExist = true;

                // ===== 基本資料 =====
                result.accountID = (int)row["accountID"];
                result.accountType = row["accountType"].ToString();
                result.account = row["account"].ToString();
                result.name = row["name"] == DBNull.Value ? "" : row["name"].ToString();
                result.email = row["email"] == DBNull.Value ? "" : row["email"].ToString();

                // ===== 狀態 =====
                result.verifyStatus = row["verifyStatus"] == DBNull.Value
                    ? (bool?)null
                    : (bool)row["verifyStatus"];

                result.isActive = (bool)row["isActive"];

                // ===== 時間欄位 =====
                result.lastUpdatePWDateTime = row["lastUpdatePWDateTime"] == DBNull.Value
                    ? (DateTime?)null
                    : (DateTime)row["lastUpdatePWDateTime"];

                result.lastLoginDateTime = row["lastLoginDateTime"] == DBNull.Value
                    ? (DateTime?)null
                    : (DateTime)row["lastLoginDateTime"];

                // ===== 系統開關與標記 =====

                result.isFirstLogin = (bool)row["isFirstLogin"];
                result.isNeedChangePW = (bool)row["isNeedChangePW"];
                result.isAutoStopLogin = (bool)row["isAutoStopLogin"];

                // ===== 權限 / 單位 =====
                result.auTypeID = (int)row["auTypeID"];
                result.auTypeName = row["auTypeName"] == DBNull.Value ? "" : row["auTypeName"].ToString();
                result.unitID = (int)row["unitID"];
                result.unitGroup = row["unitGroup"] == DBNull.Value ? "" : row["unitGroup"].ToString();
                result.unitName = row["unitName"] == DBNull.Value ? "" : row["unitName"].ToString();

                // ===== SSO（可為NULL）=====
                result.APP_USER_NODE_UUID = row["APP_USER_NODE_UUID"] == DBNull.Value ? null : row["APP_USER_NODE_UUID"].ToString();
                result.APP_COMPANY_UUID = row["APP_COMPANY_UUID"] == DBNull.Value ? null : row["APP_COMPANY_UUID"].ToString();
                result.APP_COMPANY_UUID_n = row["APP_COMPANY_UUID_n"] == DBNull.Value ? null : row["APP_COMPANY_UUID_n"].ToString();
                result.APP_DEPT_NODE_UUID = row["APP_DEPT_NODE_UUID"] == DBNull.Value ? null : row["APP_DEPT_NODE_UUID"].ToString();
                result.APP_DEPT_NODE_UUID_n = row["APP_DEPT_NODE_UUID_n"] == DBNull.Value ? null : row["APP_DEPT_NODE_UUID_n"].ToString();
                result.APP_USER_LOGIN_ID = row["APP_USER_LOGIN_ID"] == DBNull.Value ? null : row["APP_USER_LOGIN_ID"].ToString();
            }

            return result;
        }

        /// <summary>
        /// 取得使用者上級機關的mail清單
        /// </summary>
        /// <param name="userUnitID"></param>
        /// <returns></returns>
        public static List<MailAddress> Get_MailTo(int userUnitID)
        {
            //帳號有可能會有非email（例如開發用帳號），所以要用email欄位
            string sqlString =
                "select name, email from View_UserInfo " +
                "where " +
                "NULLIF(email,'') is not NULL and isActive = 1 and ISNULL(VerifyStatus,0) = 1 and isAutoStopLogin = 0 and " +
                "unitID in (select manageUnitID from System_UnitUnitMapping where unitID = @unitID) ";            

            List<MailAddress> list_mailTo = new List<MailAddress>();
            DataTable dt = new DataTable();
            using (var da = new DataAccess.MS_SQL())
            {
                dt = da.GetDataTable(sqlString, new SqlParameter("@unitID", userUnitID));
                for (int i = 0; i < dt.Rows.Count; i++) { 
                    DataRow currentRow = dt.Rows[i];
                    string name = currentRow["name"].ToString();
                    string email = currentRow["email"].ToString();
                    list_mailTo.Add(new MailAddress(email, name));
                }
                return list_mailTo;
            }
        }

        /// <summary>
        /// 檢查正式帳號清單是否已有該email帳號（不管審核結果或啟用狀態）
        /// </summary>
        public static bool Check_IsAccountExist(string account)
        {
            //不管是SSO或是一般帳號都應該要有email，一般帳號也絕對會同步把account寫入email
            //但因為只會抓accountType = default的檢查，所以判斷account就好
            //此處檢查account，沒檢查email（正常管道會一致）
            string sql = @"SELECT * FROM System_UserAccount WHERE accountType = 'default' and account = @account AND removeDateTime IS NULL";
            DataTable dt;
            using (var da = new DataAccess.MS_SQL())
            {
                dt = da.GetDataTable(sql, new SqlParameter("@account", account));
            }
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckPasswordComplexity(string password)
        {
            // 基礎長度檢查
            if (string.IsNullOrEmpty(password) || password.Length < 8) return false;

            // 正規表達式
            // Regex 說明：
            // (?=.*[a-z])              : 必須包含小寫
            // (?=.*[A-Z])              : 必須包含大寫
            // (?=.*\d)                 : 必須包含數字
            // (?=.*[!@#$%^&*_+=.?\-])  : 必須包含「指定的特殊符號」至少一個
            // .{8,}                    : 長度 8 碼以上 (點號 . 代表接受任何字元，包含空格、中文等)

            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*_+=.?\-]).{8,}$";

            return System.Text.RegularExpressions.Regex.IsMatch(password, pattern);
        }

        /// <summary>
        /// 檢核是否與歷史密碼重複
        /// </summary>
        /// <param name="accountID">使用者ID</param>
        /// <param name="newPassword">新密碼(明碼)</param>
        /// <param name="checkCount">檢查前 N 次</param>
        /// <returns>true=重複(檢核失敗), false=未重複(通過)</returns>
        public bool CheckIsHistoryPassword(int accountID, string newPassword, int checkCount)
        {
            // 防呆：如果次數 <= 0，代表不檢查，直接回傳 false (通過)
            if (checkCount <= 0) return false;

            System.Data.DataTable dtHistory = new System.Data.DataTable();

            using (var da = new DataAccess.MS_SQL())
            {
                // SQL: 抓取該 User 最近 N 筆密碼
                string sql = @"
                    SELECT TOP (@Limit) [password]
                    FROM [System_UserAccountPWLog]
                    WHERE [accountID] = @accountID
                    ORDER BY [setPasswordTime] DESC";

                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@Limit", checkCount),
                    new System.Data.SqlClient.SqlParameter("@accountID", accountID)
                };

                dtHistory = da.GetDataTable(sql, parameters);
            }

            string newPwdHash = Get_HashPassword(newPassword);
            foreach (System.Data.DataRow row in dtHistory.Rows)
            {
                string oldHashedPassword = row["password"].ToString();
                if (newPwdHash == oldHashedPassword)
                {
                    return true; // 發現重複
                }
            }

            return false; 

        }


        /// <summary>
        /// 更新使用者密碼
        /// </summary>
        /// <param name="accountID">使用者ID</param>
        /// <param name="passwordHash">請傳入加密後的密碼字串</param>
        /// <param name="isWriteToLog">是否寫入歷程紀錄 (true=寫入Log並更新時間)</param>
        /// <returns>isSuccess 是否成功</returns>
        public bool UpdatePassword(int accountID, string passwordHash, bool isWriteToLog)
        {
            try
            {
                string sql = "";

                if (isWriteToLog)
                {
                    // 一般變更 (需紀錄歷程 + 更新最後變更時間)
                    sql = @"
                    /* 更新使用者主檔 */
                    UPDATE [System_UserAccount]
                    SET [password] = @pwd, 
                        [lastUpdatePWDateTime] = GETDATE(),
                        [isNeedChangePW] = 0
                    WHERE [accountID] = @id;

                     /* 寫入密碼歷程紀錄表 */
                    INSERT INTO [System_UserAccountPWLog] ([accountID], [password], [setPasswordTime])
                    VALUES (@id, @pwd, GETDATE());
                ";
                }
                else
                {
                    // 情境：特殊變更 (只換密碼，不更新時間，不留紀錄)
                    sql = @"
                    UPDATE [System_UserAccount]
                    SET [password] = @pwd
                    WHERE [accountID] = @id;
                ";
                }

                using (var da = new DataAccess.MS_SQL())
                {
                    var parameters = new System.Data.SqlClient.SqlParameter[]
                    {
                    new System.Data.SqlClient.SqlParameter("@id", accountID),
                    new System.Data.SqlClient.SqlParameter("@pwd", passwordHash)
                    };

                    da.ExecNonQuery(sql, parameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }
    }

    public static class UserInfo
    {
        private const string SESSION_KEY = "LoginUser";

        /// <summary>
        /// 目前登入的使用者（只讀）
        /// </summary>
        public static Account.stru_accountInfo GetCurrentUser
        {
            get
            {
                return HttpContext.Current.Session[SESSION_KEY] as Account.stru_accountInfo;
            }
        }

        /// <summary>
        /// 登入成功時呼叫（唯一 SET Session 的地方）
        /// </summary>
        public static void SignIn(Account.stru_accountInfo user)
        {
            if (user == null || !user.isExist)
                throw new ArgumentException("Invalid login user");

            HttpContext.Current.Session[SESSION_KEY] = user;
        }

        /// <summary>
        /// 登出
        /// </summary>
        public static void SignOut()
        {
            HttpContext.Current.Session.Remove(SESSION_KEY);
        }

        /// <summary>
        /// 便利存取 AccountID（全系統最常用）
        /// </summary>
        public static int AccountID
        {
            get
            {
                if (GetCurrentUser == null)
                    throw new UnauthorizedAccessException("User not logged in");

                return GetCurrentUser.accountID;
            }
        }
    }

}
