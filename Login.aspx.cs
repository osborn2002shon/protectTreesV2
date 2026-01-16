using DataAccess;
using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace protectTreesV2
{
    public partial class Login : System.Web.UI.Page
    {

        //===== 登入錯誤五次 BEG =====//

        /// <summary>
        /// 找出目前這一輪的起點
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <returns></returns>
        public DateTime? Get_ActiveFirstErrorTime(string loginAccount)
        {
            string sql = @"SELECT MIN(errorDateTime) FROM System_UserLoginError WHERE loginAccount = @loginAccount AND clearDateTime IS NULL";

            using (var da = new MS_SQL())
            {
                var edt = da.ExcuteScalar(sql, new SqlParameter("@loginAccount", loginAccount));

                return edt == DBNull.Value ? (DateTime?)null : (DateTime)edt;
            }
        }

        /// <summary>
        /// 本輪錯誤次數
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <returns></returns>
        public int Get_ActiveErrorCount(string loginAccount) {
            string sql = @" SELECT COUNT(*) FROM System_UserLoginError WHERE loginAccount = @loginAccount AND clearDateTime IS NULL";
            using (var da = new MS_SQL())
            {
                return (int)da.ExcuteScalar(sql, new SqlParameter("@loginAccount", loginAccount));                
            }
        }

        /// <summary>
        /// 寫入登入失敗LOG
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <param name="ip"></param>
        public void Insert_LoginError(string loginAccount, string errorIP)
        {
            string sql = @"INSERT INTO System_UserLoginError (loginAccount, errorDateTime, errorIP) VALUES (@loginAccount, @errorDateTime, @errorIP)";
            
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(
                    sql,
                    new SqlParameter("@loginAccount", loginAccount),
                    new SqlParameter("@errorDateTime", DateTime.Now),
                    new SqlParameter("@errorIP", errorIP)
                    );
            }
        }

        /// <summary>
        /// 成功登入 or 週期失效
        /// </summary>
        /// <param name="loginAccount"></param>
        public void Clear_LoginError(string loginAccount) {
            string sql = @"UPDATE System_UserLoginError SET clearDateTime = @clearDateTime WHERE loginAccount = @loginAccount AND clearDateTime IS NULL";

            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(
                    sql,
                    new SqlParameter("@loginAccount", loginAccount),
                    new SqlParameter("@clearDateTime", DateTime.Now)
                    );
            }

        }

        /// <summary>
        /// 取得最後錯誤時間
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <returns></returns>
        public DateTime Get_LastActiveErrorTime(string loginAccount)
        {
            string sql = @"SELECT TOP 1 errorDateTime FROM System_UserLoginError WHERE loginAccount = @loginAccount AND clearDateTime IS NULL ORDER BY errorDateTime DESC";

            using (var da = new MS_SQL())
            {
                return (DateTime)da.ExcuteScalar(sql,new SqlParameter("@loginAccount", loginAccount));
            }

        }

        /// <summary>
        /// 檢查帳號是否已被鎖定
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <returns></returns>
        public bool Check_IsAccountLocked(string loginAccount) {
            DateTime? firstErrorTime = Get_ActiveFirstErrorTime(loginAccount);
            if (!firstErrorTime.HasValue)
                return false;

            // 距離前一輪已超過30分鐘，本輪自然失效（同步清除前一輪）
            if (DateTime.Now > firstErrorTime.Value.AddMinutes(30))
            {
                Clear_LoginError(loginAccount);
                return false;
            }

            //檢查錯誤次數，小於5次放行
            int errorCount = Get_ActiveErrorCount(loginAccount);
            if (errorCount < 5) {
                return false;
            }

            //第5次錯誤的時間 = 最新一筆errorDateTime
            //假設 D+15m = 18:00
            //假設 Now = 17:50 → 18:05
            //Now < D+15m : True
            //Now = D+15m : True
            //Now > D+15m : False
            DateTime lastErrorTime = Get_LastActiveErrorTime(loginAccount);
            return DateTime.Now < lastErrorTime.AddMinutes(15);
        }

        //===== 登入錯誤五次 END =====//

        /// <summary>
        /// 更新最後登入時間
        /// </summary>
        public static void Update_LastLoginDateTime(int accountID, DateTime lastLoginDateTime)
        {
            string sql = @"UPDATE System_UserAccount SET lastLoginDateTime = @lastLoginDateTime WHERE accountID = @accountID AND removeDateTime IS NULL";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountID", accountID),
                    new SqlParameter("@lastLoginDateTime", lastLoginDateTime));
            }
        }


        private readonly Account _ptAccount = new Account();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Label_msg.Text = string.Empty;
            }

            //if (!UserInfo.IsAuthenticated)
            //{
            //    Response.Redirect("~/Login.aspx");
            //    return;
            //}
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (_needShowModal)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(),"show_modal_js","showModal();",true);
            }
        }

        private bool _needShowModal = false;

        protected void Button_login_Click(object sender, EventArgs e)
        {
            string account = TextBox_ac.Text.Trim();
            string password = TextBox_pw.Text.Trim();
            string captcha = TextBox_Captcha.Text.Trim();
            string hashPW = Account.Get_HashPassword(password);

            //檢查：帳號或密碼是否有輸入
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                Label_msg.Text = "請確認帳號或密碼是否正確輸入！";
                _needShowModal = true;
                return;
            }

            //檢查：驗證碼
            string sessionCaptcha = Session["CAPTCHA"] as string;
            if (string.IsNullOrEmpty(sessionCaptcha) || !string.Equals(captcha, sessionCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                Label_msg.Text = "請輸入正確的驗證碼！";
                _needShowModal = true;
                return;
            }

            //檢查錯誤次數（鎖定檢查一定要在帳密驗證之前）
            if (Check_IsAccountLocked(account))
            {
                Label_msg.Text = "您的登入失敗次數過多，已暫停登入15分鐘！";
                _needShowModal = true;
                return;
            }

            //取得使用者帳號資訊
            var info = _ptAccount.Get_AccountInfo(account, hashPW);            

            //檢查：帳號是否存在、密碼是否正確
            if (info.isExist == false)
            {
                //寫入登入錯誤紀錄
                string ip = Account.Get_IP();
                Insert_LoginError(account, ip);

                //寫完後判斷目前錯誤次數
                int errorCount = Get_ActiveErrorCount(account);
                if (errorCount >= 5 && Check_IsAccountLocked(account))
                {
                    Label_msg.Text = "您的登入失敗次數過多，已暫停登入15分鐘！";
                }
                else
                {
                    Label_msg.Text = "帳號或密碼錯誤！";
                }
                _needShowModal = true;
                return;
            }

            //申請的時候不會有密碼，審核通過才會發放（寫在信上），所以不可能有登入結果被通知在審核中的狀況
            //檢查：是否審核通過
            //if (info.verifyStatus == null)
            //{
            //    Label_msg.Text = "帳號正在等待審核，若有疑問請洽管理者！";
            //    _needShowModal = true;
            //    return;
            //}
            //if (info.verifyStatus == false)
            //{
            //    Label_msg.Text = "帳號申請已駁回，若有疑問請洽管理者！";
            //    _needShowModal = true;
            //    return;
            //}

            //檢查：是否初次登入應變更密碼
            if (info.isFirstLogin == true)
            {
                //導向初次密碼變更頁
                Response.Redirect("pwc.aspx");
                return;
            }

            //檢查：是否被人工停用帳號（優先度高於180天未登入）
            if (info.isActive == false)
            {
                Label_msg.Text = "您的帳號已被停用，若需恢復使用請洽管理者！";
                _needShowModal = true;
                return;
            }

            //檢查：是否超過180天未登入（利用時間戳記判斷，自動視為停用）
            if (info.isAutoStopLogin == true)
            {
                Label_msg.Text = "您的帳號已超過180天未登入，若需恢復使用請洽管理者。";
                _needShowModal = true;
                return;
            }

            //檢查：是否超過180天應變更密碼
            if (info.isNeedChangePW == true)
            {
                //導向定期密碼變更頁
                Response.Redirect("pwc.aspx");
                return;
            }

            //登入成功
            //清除錯誤紀錄
            //Clear_LoginError(account);
            //設定使用者身分資訊（真正登入成功後才塞Session）
            Session["LoginUser"] = info;
            UserInfo.SignIn(info);
            //更新登入時間
            DateTime _currentDateTime = DateTime.Now;
            Update_LastLoginDateTime(info.accountID, _currentDateTime);
            //寫入User Log
            UserLog.Insert_UserLog(info.accountID, UserLog.enum_UserLogItem.登入, UserLog.enum_UserLogType.其他, "", _currentDateTime);
            //依據身分權限跳轉
            switch (info.auTypeID) {
                case 1:
                    //系統管理權限 本署
                    Response.Redirect("~/backstage/dashboard/sm.aspx");
                    return;
                case 2:
                    //檢視管理權限 分署
                    Response.Redirect("~/backstage/dashboard/rm.aspx");
                    return;
                case 3:
                    //健康管理權限 健檢中心
                    Response.Redirect("~/backstage/dashboard/hm.aspx");
                    return;
                case 4:
                    //樹木管理權限 縣市政府
                    Response.Redirect("~/backstage/dashboard/tm.aspx");
                    return;
                case 5:
                    //養護作業權限 縣市政府下的樹木權管單位或養護單位或養護廠商
                    Response.Redirect("~/backstage/tree/query.aspx");
                    return;
                default:
                    Response.Redirect("login.aspx");
                    return;
            }
            
        }
    }
}