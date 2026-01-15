using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace protectTreesV2
{
    public partial class regVerify : System.Web.UI.Page
    {

        public struct stru_verifyInfo {
            public bool isHashCodeAlive;
            public string email;
            public string name;
            public string mobile;
            public string memo;
            public int auTypeID ;
            public int unitID;
        }

        /// <summary>
        /// 檢查驗證碼是否正確、存在
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns></returns>
        public static stru_verifyInfo Check_IsHashCodeAlive(string hashCode) {
            //只要是30分鐘內的hashCode都可以用，但用完之後要把同個email的刷掉，這樣其他就會失效了
            string sqlString =
                "select * from System_UserAccountMailVerify " +
                "where hashCode = @hashCode and verifyDateTime is null and GETDATE() <= DATEADD(MINUTE,30,insertDateTime)";
            DataTable dt = new DataTable();
            using (var da = new DataAccess.MS_SQL()) {
                dt = da.GetDataTable(sqlString, new SqlParameter("@hashCode", hashCode));
            }
            stru_verifyInfo verifyInfo = new stru_verifyInfo();
            if (dt.Rows.Count > 0) {
                verifyInfo.isHashCodeAlive = true;
                verifyInfo.email = dt.Rows[0]["email"].ToString();
                verifyInfo.name = dt.Rows[0]["name"].ToString();
                verifyInfo.mobile = dt.Rows[0]["mobile"].ToString();
                verifyInfo.memo = dt.Rows[0]["memo"].ToString();
                verifyInfo.auTypeID = (int)dt.Rows[0]["auTypeID"];
                verifyInfo.unitID = (int)dt.Rows[0]["unitID"];

            } else {
                verifyInfo.isHashCodeAlive = false;
            }

            return verifyInfo;
        }

        /// <summary>
        /// 寫入帳號申請資料
        /// </summary>
        public static int Insert_UserAccount_Apply(string account, int auTypeID, int unitID, string name, string mobile, string memo)
        {
            string sql = @"
                INSERT INTO System_UserAccount
                (accountType,auTypeID,unitID,account,password,name,email,mobile,memo,verifyStatus,isActive,insertDateTime,insertAccountID)
                VALUES
                (@accountType,@auTypeID,@unitID,@account,NULL,@name,@email,@mobile,@memo,@verifyStatus,@isActive,@insertDateTime,@insertAccountID);
                SELECT SCOPE_IDENTITY();";

            decimal accountID = -1;
            using (var da = new DataAccess.MS_SQL())
            {
                accountID = (decimal)da.ExcuteScalar(
                   sql,
                    new SqlParameter("@accountType", "default"),
                    new SqlParameter("@verifyStatus", DBNull.Value),
                    new SqlParameter("@isActive", false),
                    new SqlParameter("@auTypeID", auTypeID),
                    new SqlParameter("@unitID", unitID),
                    new SqlParameter("@account", account),
                    new SqlParameter("@name", name),
                    new SqlParameter("@email", account),
                    new SqlParameter("@mobile", mobile),
                    new SqlParameter("@memo", string.IsNullOrWhiteSpace(memo) ? (object)DBNull.Value : memo),
                    new SqlParameter("@insertDateTime", DateTime.Now),
                    new SqlParameter("@insertAccountID", -1)
                );
                return Convert.ToInt32(accountID);
            }
        }

        public void Update_EmailVerify(string email) {
            string sqlString =
                "update System_UserAccountMailVerify set verifyDateTime = @verifyDateTime " +
                "where email = @email and verifyDateTime is null";

            using (var da = new DataAccess.MS_SQL()) {
                da.ExecNonQuery(sqlString,
                    new SqlParameter("@email", email),
                    new SqlParameter("@verifyDateTime", DateTime.Now));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //沒有傳入參數，導回登入頁面
                string verifyCode = Request.QueryString["vc"];
                if (string.IsNullOrWhiteSpace(verifyCode))
                {
                    Response.Redirect("login.aspx");
                    return;
                }

                //傳入參數不正確、傳入參數已過期
                stru_verifyInfo verifyInfo = Check_IsHashCodeAlive(verifyCode);
                if (verifyInfo.isHashCodeAlive == false) {
                    MultiView_main.SetActiveView(View_err);
                    Label_errMsg.Text = "驗證連結已失效或不存在，請重新註冊。";
                    return;
                }

                //System_UserAccount已有註冊資料
                if (Account.Check_IsAccountExist(verifyInfo.email) == true) {
                    MultiView_main.SetActiveView(View_err);
                    Label_errMsg.Text = "帳號已存在，請重新註冊。";
                    return;
                }

                //驗證通過
                MultiView_main.SetActiveView(View_def);
                Label_email.Text = verifyInfo.email;

                //把註冊資料寫到System_UserAccount                
                Insert_UserAccount_Apply(verifyInfo.email, verifyInfo.auTypeID, verifyInfo.unitID, verifyInfo.name, verifyInfo.mobile, verifyInfo.memo);

                //申請完畢，讓驗證碼失效
                Update_EmailVerify(verifyInfo.email);

                //發信通知管理者
                //發信不會留LOG，但是會BCC到信箱
                List<MailAddress> list_mailTo = Account.Get_MailTo(verifyInfo.unitID);
                if (list_mailTo.Count > 0) { 
                    string mailSubject = "[受保護樹木管理系統]帳號審核需求通知信";
                    string mailBody =
                        string.Format(
                            "您好，系統已接獲一筆帳號申請之需求（姓名：{0}），請盡速至系統查看並完成審核作業。<br><br>" +
                            "此為系統自動發送之信件，請勿回覆。若有疑問請洽系統管理者。", verifyInfo.name);
                    Mail.SendMail(list_mailTo, mailSubject, mailBody);
                }

            }
        }
    }
}