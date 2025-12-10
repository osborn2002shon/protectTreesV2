using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DataAccess;
using protectTreesV2.Log;

namespace protectTreesV2.User {

    public class MailVerify { 
        public enum enum_VerifyType {
               帳號申請,
               忘記密碼
        }
    }

    public class PasswordHistory {
        public int userID { get; set; }
        public string password { get; set; }
        public DateTime setPasswordTime { get; set; }
    }

    /// <summary>
    /// 使用者帳號
    /// </summary>
    public class Account
    {
        const string _pw_salt = "protectTrees";

        public int userID { get; set; }
        public enum_auType auType { get; set; }
        public string account { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }

        public string unit { get; set; }
        public string memo { get;set; }
        public string isActive { get; set; }
        public DateTime insertDateTime { get; set; }
        public int insertUserID { get; set; }

        public DateTime? updateDateTime { get; set; }
        public int? updateUserID { get; set; }
        public DateTime? lastUpdatePWDateTime { get; set; }
        public enum enum_auType {
            系統管理權限 = 1,
            檢視管理權限 = 2,
            健康管理權限 = 3,
            樹木管理權限 = 4,
            養護管理權限 = 5,
            待審核 = 6,
            駁回 = 7,
            未登入 = -1
        }

        public Account()
        {
            this.auType = enum_auType.未登入;
        }

        public List<enum_auType> GetEnum_AuTypes()
        {
            return Enum.GetValues(typeof(enum_auType)).Cast<enum_auType>().ToList();
        }

        public static string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password + _pw_salt);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public class Log { 
        enum enum_LogType {
            登入,
            登出,
            登入失敗,
            新增,
            修改,
            刪除,
            其他
        }
    }

    public static class UserService
    {
        private const string SESSION_KEY = "CurrentUser";
        public const string PasswordPolicyDescription = "密碼需包含至少一個大寫字母、一個小寫字母及一個數字。";

        private static User.Account DataRowToUser(DataRow row)
        {
            return new User.Account
            {
                userID = Convert.ToInt32(row["accountID"]),
                auType = (User.Account.enum_auType)Convert.ToInt32(row["auTypeID"]),
                account = row["account"].ToString(),
                name = row["name"].ToString(),
                email = row["email"] as string,
                mobile = row["mobile"] as string,
                unit = row.Table.Columns.Contains("unit") ? row["unit"] as string : null,
                memo = row["memo"] as string,
                isActive = Convert.ToBoolean(row["isActive"]) ? "1" : "0",
                insertDateTime = Convert.ToDateTime(row["insertDateTime"]),
                insertUserID = Convert.ToInt32(row["insertAccountID"]),
                updateDateTime = row["updateDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["updateDateTime"]),
                updateUserID = row["updateAccountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["updateAccountID"]),
                lastUpdatePWDateTime = row["lastUpdatePWDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["lastUpdatePWDateTime"])
            };
        }

        public static Account Login(string account, string password)
        {
            string sql = "SELECT TOP 1 * FROM User_Account WHERE account=@account AND password=@password AND removeDateTime IS NULL";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql,
                    new SqlParameter("@account", account),
                    new SqlParameter("@password", User.Account.HashPassword(password)));
                if (dt.Rows.Count == 1)
                {
                    var user = DataRowToUser(dt.Rows[0]);
                    if (user.isActive == "1")
                    {
                        HttpContext.Current.Session[SESSION_KEY] = user;
                    }
                    return user;
                }
            }
            return null;
        }

        public static Account GetUserByAccount(string account)
        {
            string sql = "SELECT TOP 1 * FROM User_Account WHERE account=@account AND removeDateTime IS NULL";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@account", account));
                if (dt.Rows.Count == 1) return DataRowToUser(dt.Rows[0]);
            }
            return null;
        }

        public static void Logout()
        {
            HttpContext.Current.Session.Remove(SESSION_KEY);
        }

        public static User.Account GetCurrentUser()
        {
            return HttpContext.Current.Session[SESSION_KEY] as User.Account;
        }

        public static User.Account GetUser(int userID)
        {
            string sql = "SELECT TOP 1 * FROM User_Account WHERE accountID=@id AND removeDateTime IS NULL";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", userID));
                if (dt.Rows.Count == 1) return DataRowToUser(dt.Rows[0]);
            }
            return null;
        }

        public static bool CheckPermission(User.Account.enum_auType required)
        {
            var user = GetCurrentUser();
            return user != null && user.auType == required;
        }

        public static int GetFailedLoginCount(int userID)
        {
            return OperationLogger.GetFailedLoginCount(userID);
        }

        public static bool IsLockedOut(int userID)
        {
            return OperationLogger.IsLockedOut(userID);
        }

        public static void DisableAccount(int userID)
        {
            string sql = "UPDATE User_Account SET isActive=0 WHERE accountID=@id";
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql, new SqlParameter("@id", userID));
            }
            OperationLogger.InsertLog(userID, "帳號管理", "停用", "超過180天未登入");
        }

        public static bool ChangePassword(int userID, string newPassword)
        {
            string hash = Account.HashPassword(newPassword);
            using (var da = new MS_SQL())
            {
                da.StartTransaction();
                da.ExecNonQuery("UPDATE User_Account SET password=@pw, lastUpdatePWDateTime=@dt WHERE accountID=@id",
                    new SqlParameter("@pw", hash),
                    new SqlParameter("@dt", DateTime.Now),
                    new SqlParameter("@id", userID));
                da.ExecNonQuery("INSERT INTO User_PasswordHistory(userID,password,setPasswordTime) VALUES(@id,@pw,@dt)",
                    new SqlParameter("@id", userID),
                    new SqlParameter("@pw", hash),
                    new SqlParameter("@dt", DateTime.Now));
                da.Commit();
            }
            OperationLogger.InsertLog(userID, "帳號管理", "修改密碼", $"帳號ID:{userID}");
            return true;
        }

        public static bool IsPasswordUsedRecently(int userID, string hashedPassword)
        {
            string sql = "SELECT TOP 3 password FROM User_PasswordHistory WHERE userID=@id ORDER BY setPasswordTime DESC";
            using (var da = new MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@id", userID));
                foreach (DataRow row in dt.Rows)
                {
                    if (row["password"].ToString() == hashedPassword) return true;
                }
            }
            return false;
        }

        public static bool ValidatePasswordPolicy(string password, out string message)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                message = "請輸入新密碼。";
                return false;
            }

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            if (!hasUpper || !hasLower || !hasDigit)
            {
                //message = PasswordPolicyDescription;
                message = "不符合密碼原則，請確認您的密碼符合密碼原則。";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static DataTable GetMenusByUser(Account user)
        {
            string sql = @"SELECT m.groupName,m.menuName,m.menuURL,m.orderBy_group,m.orderBy_menu
                            FROM System_Menu m
                            JOIN System_MenuAu a ON m.menuID=a.menuID
                            WHERE a.auTypeID=@auType AND m.isActive=1
                            ORDER BY m.orderBy_group, m.orderBy_menu";
            using (var da = new MS_SQL())
            {
                return da.GetDataTable(sql, new SqlParameter("@auType", (int)user.auType));
            }
        }

        public static string GetFirstMenuUrl(Account user)
        {
            var dt = GetMenusByUser(user);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["menuURL"].ToString();
            }
            return "Login";
        }
    }
}

