using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using protectTreesV2.Base;
using protectTreesV2.SystemManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.UI.WebControls;

namespace protectTreesV2.backstage.system
{
    public partial class accountManage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MultiView_main.SetActiveView(View_list);
                InitializeFilters();
                ApplyStateFilter();
                AutoDisableInactiveAccounts();
                BindAccountList();
            }
        }

        private Account.stru_accountInfo CurrentUser => UserInfo.GetCurrentUser;

        private bool HasManagePermission()
        {
            return CurrentUser != null && (CurrentUser.auTypeID == 1 || CurrentUser.auTypeID == 2 || CurrentUser.auTypeID == 4);
        }

        private bool IsSystemAdmin()
        {
            return CurrentUser != null && CurrentUser.auTypeID == 1;
        }

        private IEnumerable<int> GetAllowedManageAuTypes()
        {
            if (CurrentUser == null)
            {
                return Enumerable.Empty<int>();
            }

            switch (CurrentUser.auTypeID)
            {
                case 1:
                    return new[] { 1, 2, 3, 4, 5 };
                case 2:
                    return new[] { 3, 5 };
                case 3:
                    return new[] { 5 };
                default:
                    return Enumerable.Empty<int>();
            }
        }

        private void InitializeFilters()
        {
            Panel_permissionMessage.Visible = !HasManagePermission();
            LinkButton_add.Visible = HasManagePermission();
            LinkButton_exportExcel.Visible = HasManagePermission();
            GridView_accountList.Visible = HasManagePermission();

            BindRoleDropdown();
            BindUnitGroupDropdown();

            if (!IsSystemAdmin())
            {
                var ssoItem = DropDownList_accountType.Items.FindByValue("sso");
                if (ssoItem != null)
                {
                    DropDownList_accountType.Items.Remove(ssoItem);
                }
            }
        }

        private void ApplyStateFilter()
        {
            var state = GetState<AccountManageFilter>();
            if (state?.verfiyStatus == null)
            {
                return;
            }

            switch (state.verfiyStatus)
            {
                case AccountVerifyStatus.尚未審核:
                    DropDownList_verifyStatus.SelectedValue = "pending";
                    break;
                case AccountVerifyStatus.已審核:
                    DropDownList_verifyStatus.SelectedValue = "1";
                    break;
            }
        }

        private void BindRoleDropdown()
        {
            DropDownList_role.Items.Clear();
            DropDownList_role.Items.Add(new ListItem("全部", string.Empty));

            var allowedAuTypes = GetAllowedManageAuTypes().ToHashSet();
            if (allowedAuTypes.Count == 0)
            {
                return;
            }

            const string sql = @"SELECT auTypeID, auTypeName FROM System_UserAuType ORDER BY auTypeID";
            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    int auTypeId = Convert.ToInt32(row["auTypeID"]);
                    if (!allowedAuTypes.Contains(auTypeId))
                    {
                        continue;
                    }

                    DropDownList_role.Items.Add(new ListItem(row["auTypeName"].ToString(), auTypeId.ToString()));
                }
            }
        }

        private void BindUnitGroupDropdown()
        {
            DropDownList_unitGroup.Items.Clear();

            var allowedAuTypes = GetAllowedManageAuTypes().ToHashSet();
            if (allowedAuTypes.Count == 0)
            {
                return;
            }

            const string sql = @"SELECT auTypeID, unitGroup FROM System_Unit GROUP BY auTypeID, unitGroup ORDER BY auTypeID";
            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    int auTypeId = Convert.ToInt32(row["auTypeID"]);
                    if (!allowedAuTypes.Contains(auTypeId))
                    {
                        continue;
                    }

                    DropDownList_unitGroup.Items.Add(new ListItem(row["unitGroup"].ToString(), auTypeId.ToString()));
                }
            }

            if (DropDownList_unitGroup.Items.Count > 0)
            {
                int auTypeId = int.Parse(DropDownList_unitGroup.SelectedValue);
                BindUnitNameDropdown(auTypeId);
            }
        }

        private void BindUnitNameDropdown(int auTypeId)
        {
            DropDownList_unitName.Items.Clear();
            string sql = @"SELECT unitID, unitName
                           FROM System_Unit
                           WHERE auTypeID = @auTypeID";

            var parameters = new List<SqlParameter> { new SqlParameter("@auTypeID", auTypeId) };

            if (!IsSystemAdmin() && CurrentUser != null && (CurrentUser.auTypeID == 2 || CurrentUser.auTypeID == 4))
            {
                sql += " AND unitID IN (SELECT unitID FROM System_UnitUnitMapping WHERE manageUnitID = @manageUnitID)";
                parameters.Add(new SqlParameter("@manageUnitID", CurrentUser.unitID));
            }

            sql += " ORDER BY unitID";

            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    DropDownList_unitName.Items.Add(new ListItem(row["unitName"].ToString(), row["unitID"].ToString()));
                }
            }
        }

        private void AutoDisableInactiveAccounts()
        {
            if (!HasManagePermission())
            {
                return;
            }

            const string sql = @"
                UPDATE System_UserAccount
                SET isActive = 0,
                    updateDateTime = GETDATE(),
                    updateAccountID = @accountID
                WHERE isActive = 1
                  AND lastLoginDateTime IS NOT NULL
                  AND GETDATE() >= DATEADD(DAY, 180, CAST(lastLoginDateTime as date))";

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql, new SqlParameter("@accountID", CurrentUser?.accountID ?? -1));
            }
        }

        private DataTable GetAccountData()
        {
            if (!HasManagePermission())
            {
                return new DataTable();
            }

            var sql = new StringBuilder(@"
                SELECT accountID, accountType, account, name, email, mobile, auTypeID, auTypeName, unitGroup, unitName,
                       verifyStatus, isActive, lastLoginDateTime, lastUpdatePWDateTime
                FROM View_UserInfo
                WHERE 1=1");

            var parameters = new List<SqlParameter>();

            if (!IsSystemAdmin())
            {
                sql.Append(" AND accountType = 'default'");
            }

            // 依使用者權限取得可管理的 AU 類型，避免在 SQL 中硬編碼。
            var allowedAuTypes = GetAllowedManageAuTypes().ToList();
            if (allowedAuTypes.Count == 0)
            {
                return new DataTable();
            }

            var auTypeParameters = new List<string>();
            for (int i = 0; i < allowedAuTypes.Count; i++)
            {
                string parameterName = $"@allowedAuType{i}";
                auTypeParameters.Add(parameterName);
                parameters.Add(new SqlParameter(parameterName, allowedAuTypes[i]));
            }
            sql.Append($" AND auTypeID IN ({string.Join(", ", auTypeParameters)})");

            if (CurrentUser != null && (CurrentUser.auTypeID == 2 || CurrentUser.auTypeID == 4))
            {
                sql.Append(" AND unitID IN (SELECT unitID FROM System_UnitUnitMapping WHERE manageUnitID = @manageUnitID)");
                parameters.Add(new SqlParameter("@manageUnitID", CurrentUser.unitID));
            }

            string accountType = DropDownList_accountType.SelectedValue;
            if (!string.IsNullOrWhiteSpace(accountType))
            {
                sql.Append(" AND accountType = @accountType");
                parameters.Add(new SqlParameter("@accountType", accountType));
            }

            if (DropDownList_status.SelectedValue == "1" || DropDownList_status.SelectedValue == "0")
            {
                sql.Append(" AND isActive = @isActive");
                parameters.Add(new SqlParameter("@isActive", DropDownList_status.SelectedValue == "1"));
            }

            if (!string.IsNullOrWhiteSpace(DropDownList_role.SelectedValue))
            {
                sql.Append(" AND auTypeID = @auTypeID");
                parameters.Add(new SqlParameter("@auTypeID", DropDownList_role.SelectedValue));
            }

            string verifyStatus = DropDownList_verifyStatus.SelectedValue;
            if (verifyStatus == "pending")
            {
                sql.Append(" AND verifyStatus IS NULL");
            }
            else if (verifyStatus == "1" || verifyStatus == "0")
            {
                sql.Append(" AND verifyStatus = @verifyStatus");
                parameters.Add(new SqlParameter("@verifyStatus", verifyStatus == "1"));
            }

            string keyword = TextBox_keyword.Text.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql.Append(@" AND (account LIKE @keyword OR name LIKE @keyword OR unitName LIKE @keyword OR unitGroup LIKE @keyword OR email LIKE @keyword)");
                parameters.Add(new SqlParameter("@keyword", $"%{keyword}%"));
            }

            sql.Append(@" ORDER BY 
                            CASE WHEN verifyStatus IS NULL THEN 0 WHEN verifyStatus = 1 THEN 1 ELSE 2 END,
                            account");

            using (var da = new DataAccess.MS_SQL())
            {
                return da.GetDataTable(sql.ToString(), parameters.ToArray());
            }
        }

        private void BindAccountList()
        {
            var data = GetAccountData();
            GridView_accountList.DataSource = data;
            GridView_accountList.DataBind();
            Label_recordCount.Text = data.Rows.Count.ToString("N0");
        }

        protected void LinkButton_search_Click(object sender, EventArgs e)
        {
            GridView_accountList.PageIndex = 0;
            BindAccountList();
        }

        protected void GridView_accountList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_accountList.PageIndex = e.NewPageIndex;
            BindAccountList();
        }

        protected void GridView_accountList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var dataRow = (DataRowView)e.Row.DataItem;
            string accountType = dataRow["accountType"].ToString();
            bool isActive = dataRow["isActive"] != DBNull.Value && (bool)dataRow["isActive"];
            bool? verifyStatus = dataRow["verifyStatus"] == DBNull.Value ? (bool?)null : (bool)dataRow["verifyStatus"];

            var editButton = (LinkButton)e.Row.FindControl("LinkButton_edit");
            var toggleButton = (LinkButton)e.Row.FindControl("LinkButton_toggle");

            bool canManageAccount = HasManagePermission() && (accountType != "sso" || IsSystemAdmin());
            bool canEdit = canManageAccount && accountType == "default";

            bool isReviewed = verifyStatus != null;
            editButton.Visible = canEdit;
            toggleButton.Visible = canManageAccount && isReviewed;

            if (editButton.Visible)
            {
                editButton.Text = isReviewed ? "編輯" : "審核";
            }

            if (toggleButton.Visible)
            {
                toggleButton.Text = isActive ? "停用" : "啟用";
                toggleButton.CssClass = isActive ? "btn btn-sm btn-outline-secondary" : "btn btn-sm btn-outline-success";
                bool requiresApproval = !isActive && (verifyStatus == null || verifyStatus == false);
                string confirmMessage = isActive
                    ? "此動作會寄送通知信，確定要停用此帳號嗎？"
                    : (requiresApproval
                        ? "啟用會將本帳號設定為審核通過，確定嗎？"
                        : "此動作會寄送通知信，確定要啟用此帳號嗎？");
                toggleButton.OnClientClick = $"return confirm('{confirmMessage}');";
            }
        }

        protected void GridView_accountList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!HasManagePermission())
            {
                ShowMessage("權限不足", "您沒有帳號管理權限。", "warning");
                return;
            }

            if (!int.TryParse(e.CommandArgument?.ToString(), out int accountId))
            {
                return;
            }

            switch (e.CommandName)
            {
                case "EditAccount":
                    StartEditAccount(accountId);
                    break;
                case "ToggleActive":
                    ToggleAccountStatus(accountId);
                    break;
            }
        }

        protected void LinkButton_add_Click(object sender, EventArgs e)
        {
            if (!HasManagePermission())
            {
                ShowMessage("權限不足", "您沒有帳號管理權限。", "warning");
                return;
            }

            ResetEditForm();
            HiddenField_editMode.Value = "add";
            MultiView_main.SetActiveView(View_edit);
        }

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {
            MultiView_main.SetActiveView(View_list);
            BindAccountList();
        }

        protected void DropDownList_unitGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(DropDownList_unitGroup.SelectedValue, out int auTypeId))
            {
                BindUnitNameDropdown(auTypeId);
            }
        }

        protected void LinkButton_save_Click(object sender, EventArgs e)
        {
            if (!HasManagePermission())
            {
                ShowMessage("權限不足", "您沒有帳號管理權限。", "warning");
                return;
            }

            string mode = HiddenField_editMode.Value;
            string account = mode == "edit" ? Label_account.Text.Trim() : TextBox_account.Text.Trim();
            string name = TextBox_name.Text.Trim();
            string mobile = TextBox_mobile.Text.Trim();
            string memo = TextBox_memo.Text.Trim();
            string verifyStatusValue = RadioButtonList_verifyStatus.SelectedValue;

            bool requireAccount = mode == "add";
            if ((requireAccount && string.IsNullOrWhiteSpace(account)) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mobile))
            {
                ShowMessage("提示", "請確認必填欄位皆已填寫。", "warning");
                return;
            }

            if (verifyStatusValue != "1" && verifyStatusValue != "0")
            {
                ShowMessage("提示", "請選擇審核狀態。", "warning");
                return;
            }

            if (!int.TryParse(DropDownList_unitGroup.SelectedValue, out int auTypeId))
            {
                ShowMessage("提示", "請選擇單位類型。", "warning");
                return;
            }

            if (!int.TryParse(DropDownList_unitName.SelectedValue, out int unitId))
            {
                ShowMessage("提示", "請選擇單位名稱。", "warning");
                return;
            }

            var allowedAuTypes = GetAllowedManageAuTypes().ToHashSet();
            if (!allowedAuTypes.Contains(auTypeId))
            {
                ShowMessage("權限不足", "您無權新增或變更該權限層級的帳號。", "warning");
                return;
            }

            bool newVerifyStatus = verifyStatusValue == "1";
            bool? originalVerifyStatus = ParseVerifyStatus(HiddenField_verifyStatusOriginal.Value);
            bool verifyStatusChanged = originalVerifyStatus == null || originalVerifyStatus.Value != newVerifyStatus;

            if (mode == "add")
            {
                if (Account.Check_IsAccountExist(account))
                {
                    ShowMessage("提示", "此帳號已存在，請確認電子信箱是否重複。", "warning");
                    return;
                }

                int accountId = InsertAccount(account, name, mobile, memo, auTypeId, unitId, newVerifyStatus);
                if (newVerifyStatus)
                {
                    var accountInfo = GetAccountInfo(accountId);
                    string generatedPassword = EnsureTemporaryPassword(accountId);
                    SendApprovalNotification(accountInfo, generatedPassword);
                }
                ShowMessage("完成", "帳號新增成功。", "success");
            }
            else if (mode == "edit")
            {
                if (!int.TryParse(HiddenField_accountId.Value, out int accountId))
                {
                    return;
                }

                UpdateAccount(accountId, name, mobile, memo, auTypeId, unitId, newVerifyStatus, verifyStatusChanged);
                ShowMessage("完成", "帳號資料已更新。", "success");

                if (verifyStatusChanged)
                {
                    var accountInfo = GetAccountInfo(accountId);
                    if (newVerifyStatus)
                    {
                        string generatedPassword = EnsureTemporaryPassword(accountId);
                        SendApprovalNotification(accountInfo, generatedPassword);
                    }
                    else
                    {
                        SendAccountNotification(accountInfo, "帳號審核駁回");
                    }
                }
            }

            MultiView_main.SetActiveView(View_list);
            BindAccountList();
        }

        private void ResetEditForm()
        {
            HiddenField_accountId.Value = string.Empty;
            TextBox_account.Text = string.Empty;
            Label_account.Text = string.Empty;
            TextBox_name.Text = string.Empty;
            TextBox_mobile.Text = string.Empty;
            TextBox_memo.Text = string.Empty;
            HiddenField_verifyStatusOriginal.Value = string.Empty;
            RadioButtonList_verifyStatus.ClearSelection();

            BindUnitGroupDropdown();
            SetEditFieldAccessibility(isEditMode: false);
        }

        private void StartEditAccount(int accountId)
        {
            var accountInfo = GetAccountInfo(accountId);
            if (accountInfo == null)
            {
                ShowMessage("提示", "找不到指定帳號。", "warning");
                return;
            }

            if (accountInfo.accountType == "sso" && !IsSystemAdmin())
            {
                ShowMessage("權限不足", "SSO帳號需由系統管理權限進行審核與管理。", "warning");
                return;
            }

            HiddenField_accountId.Value = accountId.ToString();
            HiddenField_editMode.Value = "edit";
            TextBox_account.Text = accountInfo.account;
            Label_account.Text = accountInfo.account;
            TextBox_name.Text = accountInfo.name;
            TextBox_mobile.Text = accountInfo.mobile;
            TextBox_memo.Text = accountInfo.memo ?? string.Empty;
            HiddenField_verifyStatusOriginal.Value = SerializeVerifyStatus(accountInfo.verifyStatus);
            RadioButtonList_verifyStatus.ClearSelection();
            if (accountInfo.verifyStatus != null)
            {
                RadioButtonList_verifyStatus.SelectedValue = accountInfo.verifyStatus.Value ? "1" : "0";
            }

            BindUnitGroupDropdown();
            DropDownList_unitGroup.SelectedValue = accountInfo.auTypeID.ToString();
            BindUnitNameDropdown(accountInfo.auTypeID);
            DropDownList_unitName.SelectedValue = accountInfo.unitID.ToString();

            SetEditFieldAccessibility(isEditMode: true);
            MultiView_main.SetActiveView(View_edit);
        }

        private void SetEditFieldAccessibility(bool isEditMode)
        {
            TextBox_account.Visible = !isEditMode;
            Label_account.Visible = isEditMode;
            bool allowUnitChange = IsSystemAdmin() || !isEditMode;
            DropDownList_unitGroup.Enabled = allowUnitChange;
            DropDownList_unitName.Enabled = allowUnitChange;
        }

        private int InsertAccount(string account, string name, string mobile, string memo, int auTypeId, int unitId, bool verifyStatus)
        {
            const string sql = @"
                INSERT INTO System_UserAccount
                (accountType, auTypeID, unitID, account, password, name, email, mobile, memo, verifyStatus, verifyDateTime, isActive, insertDateTime, insertAccountID)
                VALUES
                (@accountType, @auTypeID, @unitID, @account, NULL, @name, @email, @mobile, @memo, @verifyStatus, @verifyDateTime, @isActive, @insertDateTime, @insertAccountID);
                SELECT SCOPE_IDENTITY();";

            using (var da = new DataAccess.MS_SQL())
            {
                object result = da.ExcuteScalar(sql,
                    new SqlParameter("@accountType", "default"),
                    new SqlParameter("@auTypeID", auTypeId),
                    new SqlParameter("@unitID", unitId),
                    new SqlParameter("@account", account),
                    new SqlParameter("@name", name),
                    new SqlParameter("@email", account),
                    new SqlParameter("@mobile", mobile),
                    new SqlParameter("@memo", string.IsNullOrWhiteSpace(memo) ? (object)DBNull.Value : memo),
                    new SqlParameter("@verifyStatus", verifyStatus),
                    new SqlParameter("@verifyDateTime", DateTime.Now),
                    new SqlParameter("@isActive", verifyStatus),
                    new SqlParameter("@insertDateTime", DateTime.Now),
                    new SqlParameter("@insertAccountID", CurrentUser?.accountID ?? -1));

                int accountId = result == null ? 0 : Convert.ToInt32(result);
                UserLog.Insert_UserLog(CurrentUser?.accountID ?? -1, UserLog.enum_UserLogItem.系統帳號管理, UserLog.enum_UserLogType.新增, $"新增帳號：{account}");
                return accountId;
            }
        }

        private void UpdateAccount(int accountId, string name, string mobile, string memo, int auTypeId, int unitId, bool verifyStatus, bool verifyStatusChanged)
        {
            var sql = new StringBuilder(@"
                UPDATE System_UserAccount
                SET name = @name,
                    mobile = @mobile,
                    memo = @memo,
                    updateDateTime = GETDATE(),
                    updateAccountID = @updateAccountID");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@name", name),
                new SqlParameter("@mobile", mobile),
                new SqlParameter("@memo", string.IsNullOrWhiteSpace(memo) ? (object)DBNull.Value : memo),
                new SqlParameter("@updateAccountID", CurrentUser?.accountID ?? -1),
                new SqlParameter("@accountID", accountId)
            };

            if (IsSystemAdmin())
            {
                sql.Append(", auTypeID = @auTypeID, unitID = @unitID");
                parameters.Add(new SqlParameter("@auTypeID", auTypeId));
                parameters.Add(new SqlParameter("@unitID", unitId));
            }

            if (verifyStatusChanged)
            {
                sql.Append(", verifyStatus = @verifyStatus, verifyDateTime = GETDATE(), isActive = @isActive");
                parameters.Add(new SqlParameter("@verifyStatus", verifyStatus));
                parameters.Add(new SqlParameter("@isActive", verifyStatus));
            }

            sql.Append(" WHERE accountID = @accountID");

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql.ToString(), parameters.ToArray());
            }

            UserLog.Insert_UserLog(CurrentUser?.accountID ?? -1, UserLog.enum_UserLogItem.系統帳號管理, UserLog.enum_UserLogType.修改, $"更新帳號資料：{accountId}");

            if (verifyStatusChanged)
            {
                string resultText = verifyStatus ? "審核通過" : "審核駁回";
                UserLog.Insert_UserLog(CurrentUser?.accountID ?? -1, UserLog.enum_UserLogItem.系統帳號管理, UserLog.enum_UserLogType.修改, $"{resultText}：{accountId}");
            }
        }

        private void ToggleAccountStatus(int accountId)
        {
            var accountInfo = GetAccountInfo(accountId);
            if (accountInfo == null)
            {
                ShowMessage("提示", "找不到指定帳號。", "warning");
                return;
            }

            if (accountInfo.accountType == "sso" && !IsSystemAdmin())
            {
                ShowMessage("權限不足", "SSO帳號需由系統管理權限進行審核與管理。", "warning");
                return;
            }

            bool enable = !accountInfo.isActive;
            bool requiresApproval = enable && (accountInfo.verifyStatus == null || accountInfo.verifyStatus == false);
            var sql = new StringBuilder(@"
                UPDATE System_UserAccount
                SET isActive = @isActive,
                    updateDateTime = GETDATE(),
                    updateAccountID = @updateAccountID");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@isActive", enable),
                new SqlParameter("@updateAccountID", CurrentUser?.accountID ?? -1),
                new SqlParameter("@accountID", accountId)
            };

            if (requiresApproval)
            {
                sql.Append(", verifyStatus = 1, verifyDateTime = GETDATE()");
            }

            sql.Append(" WHERE accountID = @accountID");

            using (var da = new DataAccess.MS_SQL())
            {
                da.ExecNonQuery(sql.ToString(), parameters.ToArray());
            }

            string statusText = enable ? "啟用" : "停用";
            UserLog.Insert_UserLog(CurrentUser?.accountID ?? -1, UserLog.enum_UserLogItem.系統帳號管理, UserLog.enum_UserLogType.修改, $"帳號{statusText}：{accountInfo.account}");

            if (requiresApproval)
            {
                string generatedPassword = EnsureTemporaryPassword(accountId);
                SendApprovalNotification(accountInfo, generatedPassword);
            }
            else
            {
                SendAccountNotification(accountInfo, enable ? "帳號已啟用" : "帳號已停用");
            }

            ShowMessage("完成", $"帳號已{statusText}。", "success");
            BindAccountList();
        }

        private AccountEntry GetAccountInfo(int accountId)
        {
            const string sql = @"
                SELECT accountID, accountType, account, name, email, mobile, auTypeID, unitID, unitName, verifyStatus, isActive, memo
                FROM View_UserInfo
                WHERE accountID = @accountID";

            using (var da = new DataAccess.MS_SQL())
            {
                var dt = da.GetDataTable(sql, new SqlParameter("@accountID", accountId));
                if (dt.Rows.Count == 0)
                {
                    return null;
                }

                var row = dt.Rows[0];
                return new AccountEntry
                {
                    accountID = Convert.ToInt32(row["accountID"]),
                    accountType = row["accountType"].ToString(),
                    account = row["account"].ToString(),
                    name = row["name"].ToString(),
                    email = row["email"].ToString(),
                    mobile = row["mobile"].ToString(),
                    auTypeID = Convert.ToInt32(row["auTypeID"]),
                    unitID = Convert.ToInt32(row["unitID"]),
                    unitName = row["unitName"].ToString(),
                    verifyStatus = row["verifyStatus"] == DBNull.Value ? (bool?)null : (bool)row["verifyStatus"],
                    isActive = row["isActive"] != DBNull.Value && (bool)row["isActive"],
                    memo = row["memo"] == DBNull.Value ? null : row["memo"].ToString()
                };
            }
        }

        private void SendAccountNotification(AccountEntry accountInfo, string actionText)
        {
            if (accountInfo == null || string.IsNullOrWhiteSpace(accountInfo.email))
            {
                return;
            }

            var mailTo = new List<MailAddress> { new MailAddress(accountInfo.email, accountInfo.name) };
            string subject = "[受保護樹木管理系統] 帳號狀態通知";
            string body = string.Format(
                "{0} 您好，您的帳號（{1}）{2}。<br><br>" +
                "此為系統自動發送之信件，請勿回覆。若有疑問請洽系統管理者。",
                accountInfo.name,
                accountInfo.account,
                actionText);

            Mail.SendMail(mailTo, subject, body);
        }

        private void SendApprovalNotification(AccountEntry accountInfo, string generatedPassword)
        {
            if (accountInfo == null || string.IsNullOrWhiteSpace(accountInfo.email))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(generatedPassword))
            {
                SendAccountNotification(accountInfo, "帳號審核通過");
                return;
            }

            var mailTo = new List<MailAddress> { new MailAddress(accountInfo.email, accountInfo.name) };
            string subject = "[受保護樹木管理系統] 帳號審核通過通知";
            string body = string.Format(
                "{0} 您好，您的帳號（{1}）已審核通過。<br><br>" +
                "系統已為您產生登入密碼：<strong>{2}</strong><br>" +
                "請您登入後立即變更密碼，以確保帳號安全。<br><br>" +
                "此為系統自動發送之信件，請勿回覆。若有疑問請洽系統管理者。",
                accountInfo.name,
                accountInfo.account,
                generatedPassword);

            Mail.SendMail(mailTo, subject, body);
        }

        private string EnsureTemporaryPassword(int accountId)
        {
            const string selectSql = "SELECT password FROM System_UserAccount WHERE accountID = @accountID";
            using (var da = new DataAccess.MS_SQL())
            {
                object current = da.ExcuteScalar(selectSql, new SqlParameter("@accountID", accountId));
                if (current != null && current != DBNull.Value && !string.IsNullOrWhiteSpace(current.ToString()))
                {
                    return null;
                }

                string tempPassword = Account.GenerateTemporaryPassword();
                string hashPassword = Account.Get_HashPassword(tempPassword);

                const string updateSql = @"
                    UPDATE System_UserAccount
                    SET password = @password,
                        lastUpdatePWDateTime = GETDATE(),
                        updateDateTime = GETDATE(),
                        updateAccountID = @updateAccountID
                    WHERE accountID = @accountID";

                da.ExecNonQuery(updateSql,
                    new SqlParameter("@password", hashPassword),
                    new SqlParameter("@updateAccountID", CurrentUser?.accountID ?? -1),
                    new SqlParameter("@accountID", accountId));

                return tempPassword;
            }
        }

        protected void LinkButton_exportExcel_Click(object sender, EventArgs e)
        {
            if (!HasManagePermission())
            {
                ShowMessage("權限不足", "您沒有帳號管理權限。", "warning");
                return;
            }

            var data = GetAccountData();
            if (data.Rows.Count == 0)
            {
                ShowMessage("提示", "目前查無資料可供匯出。", "info");
                return;
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("帳號列表");

            string[] headers = { "帳號", "帳號類型", "系統權限別", "單位名稱", "帳號狀態", "審核狀態", "最後登入時間" };
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.CreateCell(i).SetCellValue(headers[i]);
            }

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];
                IRow excelRow = sheet.CreateRow(i + 1);
                excelRow.CreateCell(0).SetCellValue(row["account"].ToString());
                excelRow.CreateCell(1).SetCellValue(GetAccountTypeText(row["accountType"]));
                excelRow.CreateCell(2).SetCellValue(row["auTypeName"].ToString());
                excelRow.CreateCell(3).SetCellValue(row["unitName"].ToString());
                excelRow.CreateCell(4).SetCellValue(GetActiveStatusText(row["isActive"]));
                excelRow.CreateCell(5).SetCellValue(GetVerifyStatusText(row["verifyStatus"]));
                excelRow.CreateCell(6).SetCellValue(FormatDateTime(row["lastLoginDateTime"]));
            }

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            UserLog.Insert_UserLog(CurrentUser?.accountID ?? -1, UserLog.enum_UserLogItem.系統帳號管理, UserLog.enum_UserLogType.下載, "匯出帳號列表");

            string fileName = $"AccountList_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            workbook.Write(Response.OutputStream);
            Response.End();
        }

        protected string GetAccountTypeText(object accountType)
        {
            string value = accountType?.ToString() ?? string.Empty;
            return value == "sso" ? "SSO帳號" : "一般帳號";
        }

        protected string GetActiveStatusText(object isActive)
        {
            bool active = isActive != DBNull.Value && isActive is bool && (bool)isActive;
            return active ? "啟用" : "停用";
        }

        protected string GetVerifyStatusText(object verifyStatus)
        {
            if (verifyStatus == DBNull.Value || verifyStatus == null)
            {
                return "待審";
            }
            return (bool)verifyStatus ? "通過" : "駁回";
        }

        protected string FormatDateTime(object dateValue)
        {
            if (dateValue == DBNull.Value || dateValue == null)
            {
                return string.Empty;
            }

            if (dateValue is DateTime dt)
            {
                return dt.ToString("yyyy/MM/dd HH:mm");
            }

            return dateValue.ToString();
        }

        private static bool? ParseVerifyStatus(string value)
        {
            if (value == "1")
            {
                return true;
            }

            if (value == "0")
            {
                return false;
            }

            return null;
        }

        private static string SerializeVerifyStatus(bool? verifyStatus)
        {
            if (verifyStatus == null)
            {
                return string.Empty;
            }

            return verifyStatus.Value ? "1" : "0";
        }

        private class AccountEntry
        {
            public int accountID { get; set; }
            public string accountType { get; set; }
            public string account { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string mobile { get; set; }
            public int auTypeID { get; set; }
            public int unitID { get; set; }
            public string unitName { get; set; }
            public bool? verifyStatus { get; set; }
            public bool isActive { get; set; }
            public string memo { get; set; }
        }
    }
}
