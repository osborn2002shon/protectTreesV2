using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace protectTreesV2
{
    public partial class reg : System.Web.UI.Page
    {

        /// <summary>
        /// 產生 DropDownList：單位
        /// </summary>
        public static void Bind_DropDownList_UnitGroup(DropDownList ddlUnit)
        {
            string sql = @"SELECT auTypeID, unitGroup FROM System_Unit WHERE auTypeID not in (1,2) GROUP BY auTypeID, unitGroup ORDER BY auTypeID";

            DataTable dt;
            using (var da = new DataAccess.MS_SQL())
            {
                dt = da.GetDataTable(sql);
            }

            ddlUnit.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                ddlUnit.Items.Add(new ListItem(row["unitGroup"].ToString(),row["auTypeID"].ToString()));
            }
        }

        /// <summary>
        /// 產生 DropDownList：單位
        /// </summary>
        public static void Bind_DropDownList_UnitName(DropDownList ddlUnit, int auTypeID)
        {
            string sql = @"SELECT unitID, unitName FROM System_Unit WHERE auTypeID = @auTypeID ORDER BY unitID";

            DataTable dt;
            using (var da = new DataAccess.MS_SQL())
            {
                dt = da.GetDataTable(sql, new SqlParameter("@auTypeID", auTypeID));
            }

            ddlUnit.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                ddlUnit.Items.Add(new ListItem(row["unitName"].ToString(),row["unitID"].ToString()));
            }
        }

        /// <summary>
        /// 寫入帳號申請資料
        /// </summary>
        public static void Insert_UserAccountMailVerify(string email, int auTypeID, int unitID, string name, string mobile, string memo, string IP, string hashCode)
        {
            string sql = @"
                INSERT INTO System_UserAccountMailVerify
                (auTypeID,unitID,name,email,mobile,memo,insertDateTime,IP, hashCode)
                VALUES
                (@auTypeID,@unitID,@name,@email,@mobile,@memo,@insertDateTime,@IP,@hashCode);
                SELECT SCOPE_IDENTITY();";

            decimal accountID = -1;
            using (var da = new DataAccess.MS_SQL())
            {
                accountID = (decimal)da.ExcuteScalar(
                   sql,
                   new SqlParameter("@email", email),
                   new SqlParameter("@auTypeID", auTypeID),
                   new SqlParameter("@unitID", unitID),
                   new SqlParameter("@name", name),
                   new SqlParameter("@mobile", mobile),
                   new SqlParameter("@memo", string.IsNullOrWhiteSpace(memo) ? (object)DBNull.Value : memo),
                   new SqlParameter("@insertDateTime", DateTime.Now),
                   new SqlParameter("@IP", IP),
                   new SqlParameter("@hashCode", hashCode)
                );
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Bind_DropDownList_UnitGroup(DropDownList_unitGroup);
                int auTypeID = int.Parse(DropDownList_unitGroup.SelectedValue);
                Bind_DropDownList_UnitName(DropDownList_unitName, auTypeID);
            }
        }

        private bool _needShowModal = false;
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (_needShowModal)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "show_modal_js", "showModal();", true);
            }
        }

        protected void DropDownList_unitGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int auTypeID = int.Parse(DropDownList_unitGroup.SelectedValue);
            Bind_DropDownList_UnitName(DropDownList_unitName, auTypeID);
        }

        protected void Button_submitCheck_Click(object sender, EventArgs e)
        {
            MultiView_main.SetActiveView(View_info);
            _needShowModal = true;
        }

        protected void Button_submit_Click(object sender, EventArgs e)
        {
            Label_msg.Text = "";
            string email = TextBox_email.Text.Trim();
            string name = TextBox_name.Text.Trim();
            string mobile = TextBox_mobile.Text.Trim();
            string memo = TextBox_memo.Text.Trim();
            int auTypeID = int.Parse(DropDownList_unitGroup.SelectedValue);
            int unitID = int.Parse(DropDownList_unitName.SelectedValue);

            //檢查控制項皆已輸入
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mobile))
            {
                // 顯示錯誤訊息，並中斷流程
                Label_msg.Text = "申請失敗，請確認星號欄位皆已正確填寫！";
                _needShowModal = true;
                MultiView_main.SetActiveView(View_msg);
                return;
            }

            //檢查帳號=信箱是否已重複
            if (Account.Check_IsAccountExist(email) == true) {
                Label_msg.Text = "申請失敗，電子信箱已重複！";
                _needShowModal = true;                
                MultiView_main.SetActiveView(View_msg);
                return;
            }

            //寫入帳號資料            
            string ip = Account.Get_IP();
            string hashCode = Account.Get_HashPassword(email + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Insert_UserAccountMailVerify(email, auTypeID, unitID, name, mobile, memo, ip, hashCode);

            //寄驗證信（點選驗證信再寫入Account + 寫入Account前檢查是否已有email）
            //發信不會留LOG，但是會BCC到信箱
            List<MailAddress> list_mailTo = new List<MailAddress>();
            list_mailTo.Add(new MailAddress(email, name));
            string DNS_Name = ConfigurationManager.AppSettings["DNS_Name"];
            string verifyURL = DNS_Name + "regVerify.aspx?vc=" + hashCode;
            string mailSubject = "[受保護樹木管理系統] 帳號申請電子信箱驗證信件";
            string mailBody = 
                string.Format(
                    "{0} 您好，請點選下面網址進行電子信箱驗證：<br>" +
                    "<a href='{1}' target='_blank'>{1}</a><br><br>" +
                    "此為系統自動發送之信件，請勿回覆。若有疑問請洽系統管理者。"
                    , name, verifyURL);
            Mail.SendMail(list_mailTo, mailSubject, mailBody);

            Label_msg.Text = "電子信箱驗證信已寄出，請依照信上指示完成帳號申請！";
            _needShowModal = true;            
            MultiView_main.SetActiveView(View_msg);
        }
    }
}