using protectTreesV2.Log;
using protectTreesV2.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace protectTreesV2
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Label_msg.Text = string.Empty;
            }
        }

        protected void Button_login_Click(object sender, EventArgs e)
        {
            string account = TextBox_ac.Text.Trim();
            string password = TextBox_pw.Text;

            string captcha = TextBox_Captcha.Text.Trim();
            string sessionCaptcha = Session["CAPTCHA"] as string;
            if (string.IsNullOrEmpty(sessionCaptcha) || !string.Equals(captcha, sessionCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                Label_msg.Text = "驗證碼錯誤";
                return;
            }

            var accountInfo = UserService.GetUserByAccount(account);
            if (accountInfo != null && OperationLogger.IsLockedOut(accountInfo.userID))
            {
                Label_msg.Text = "登入錯誤超過5次，暫停登入15分鐘";
                return;
            }

            var user = UserService.Login(account, password);
            if (user == null)
            {
                if (accountInfo != null)
                {
                    OperationLogger.InsertLog(accountInfo.userID, "登入", "登入失敗", "");
                    if (OperationLogger.IsLockedOut(accountInfo.userID))
                    {
                        Label_msg.Text = "登入錯誤超過5次，暫停登入15分鐘";
                    }
                    else
                    {
                        Label_msg.Text = "帳號或密碼錯誤";
                    }
                }
                else
                {
                    Label_msg.Text = "帳號或密碼錯誤";
                }
                return;
            }

            if (user.auType == protectTreesV2.User.Account.enum_auType.待審核)
            {
                OperationLogger.InsertLog(user.userID, "登入", "登入", "待審核");
                UserService.Logout();
                Response.Redirect("~/Backstage/User/PendingApproval");
                return;
            }
            if (user.auType == protectTreesV2.User.Account.enum_auType.駁回)
            {
                OperationLogger.InsertLog(user.userID, "登入", "登入", "駁回");
                Response.Redirect("~/Backstage/User/Rejected?account=" + HttpUtility.UrlEncode(user.account));
                return;
            }

            var lastLogin = OperationLogger.GetLastLoginTime(user.userID);
            if (lastLogin.HasValue && lastLogin.Value < DateTime.Now.AddDays(-180))
            {
                UserService.DisableAccount(user.userID);
                UserService.Logout();
                Label_msg.Text = "帳號已停用，請聯繫管理者";
                return;
            }

            if (user.isActive != "1")
            {
                OperationLogger.InsertLog(user.userID, "登入", "登入", "停用");
                Label_msg.Text = "帳號已停用，請聯繫管理者";
                return;
            }

            if (user.lastUpdatePWDateTime == null)
            {
                Response.Redirect("~/Backstage/User/ChangePassword?type=first");
                return;
            }
            if (user.lastUpdatePWDateTime.Value < DateTime.Now.AddDays(-180))
            {
                Response.Redirect("~/Backstage/User/ChangePassword?type=expired");
                return;
            }

            OperationLogger.InsertLog(user.userID, "登入", "登入", "");
            string url = UserService.GetFirstMenuUrl(user);
            Response.Redirect(url);
        }
    }
}