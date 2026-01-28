using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace protectTreesV2.backstage.system
{
    public partial class profile : protectTreesV2.Base.BasePage
    {
        protectTreesV2.Account system_account = new Account();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUserData();
            }
        }
        /// <summary>
        /// 讀取並綁定使用者資料 
        /// </summary>
        private void BindUserData()
        {
            // 先取得user
            var sessionUser = UserInfo.GetCurrentUser;

            if (sessionUser == null)
            {
                // Session 失效，導回登入頁
                Response.Redirect("~/Login.aspx");
                return;
            }

            // 使用 ID 去資料庫撈取最新的使用者資料
            var dbUser = system_account.Get_AccountInfo(sessionUser.accountID);

            // 防呆：如果資料庫撈不到 
            if (!dbUser.isExist)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // ==========================================
            // 綁定基本資料
            // ==========================================
            Label_account.Text = dbUser.account;
            Label_name.Text = dbUser.name;
            Label_unit.Text = dbUser.unitName;
            Label_role.Text = dbUser.auTypeName;

            // 最後登入時間
            Label_lastLogin.Text = dbUser.lastLoginDateTime.HasValue
                ? dbUser.lastLoginDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss")
                : "首次登入或無紀錄";

            // ==========================================
            // 判斷是否為 SSO 帳號
            // ==========================================
            // 使用 DB 裡的最新狀態來判斷
            bool isSSO = dbUser.accountType == "SSO";

            if (isSSO)
            {
                // SSO 使用者隱藏密碼變更區塊
                Panel_passwordChange.Visible = false;
            }
            else
            {
                // 一般使用者顯示密碼變更區塊
                Panel_passwordChange.Visible = true;

                // 綁定上次密碼變更時間 
                Label_passwordChangedDate.Text = dbUser.lastUpdatePWDateTime.HasValue
                    ? dbUser.lastUpdatePWDateTime.Value.ToString("yyyy/MM/dd HH:mm")
                    : "尚未變更過";
            }
        }
        /// <summary>
        /// 變更密碼按鈕事件
        /// </summary>
        protected void LinkButton_changePassword_Click(object sender, EventArgs e)
        {
            var user = UserInfo.GetCurrentUser;
            if (user == null) return;

            // 取得輸入值
            string newPwd = TextBox_newPassword.Text.Trim();
            string confirmPwd = TextBox_confirmPassword.Text.Trim();

            // 基本檢核
            if (string.IsNullOrEmpty(newPwd) || string.IsNullOrEmpty(confirmPwd))
            {
                ShowMessage("提示", "請輸入新密碼與確認密碼！");
                return;
            }

            if (newPwd != confirmPwd)
            {
                ShowMessage("提示", "兩次輸入的密碼不一致，請重新輸入！");
                return;
            }

            // 密碼複雜度檢核
            // 規格：長度至少 8 碼，包含英文大寫、英文小寫、數字及符號
            if (!system_account.CheckPasswordComplexity(newPwd))
            {
                ShowMessage("提示",
                "密碼長度須至少 8 碼，且包含：<br>" +
                "1. 英文大寫 (A-Z)<br>" +
                "2. 英文小寫 (a-z)<br>" +
                "3. 數字 (0-9)<br>" +
                "4. 特殊符號 ( ! @ # $ % ^ & * _ - + = . ? )");
                        return;
            }

            // 設定：是否啟用歷史密碼檢核
            bool enableHistoryCheck = false;

            // 設定：不可與前 N 次重複
            int historyLimit = 3;

            if (enableHistoryCheck)
            {
                if (system_account.CheckIsHistoryPassword(user.accountID, newPwd, historyLimit))
                {
                    ShowMessage("提示", $"新密碼不可與前 {historyLimit} 次使用過的密碼相同，請重新輸入。");
                    return;
                }
            }

            // 執行資料庫更新

            // 加密
            string hashedPassword = Account.Get_HashPassword(newPwd);

            // 設定：是否寫入 Log
            bool isWriteToLog = true;

            // 呼叫更新函式 
            bool isSuccess = system_account.UpdatePassword(user.accountID, hashedPassword, isWriteToLog);

            if (isSuccess)
            {
                BindUserData();
                TextBox_newPassword.Text = "";
                TextBox_confirmPassword.Text = "";

                //寫入操作紀錄
                UserLog.Insert_UserLog(user.accountID, UserLog.enum_UserLogItem.我的帳號管理, UserLog.enum_UserLogType.修改, "變更密碼");

                ShowMessage("提示", "密碼變更成功！");
            }
            else
            {
                ShowMessage("提示", "密碼變更失敗，請稍後再試或聯繫管理員。");
            }
        }
    }
}