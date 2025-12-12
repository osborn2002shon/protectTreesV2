using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.Log;
using protectTreesV2.User;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class query : BasePage
    {
        private const string SortExpressionKey = "TreeQuery_SortExpression";
        private const string SortDirectionKey = "TreeQuery_SortDirection";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                BindTrees();
            }
        }

        private void BindDropdowns()
        {
            ddlCity.Items.Clear();
            ddlCity.Items.Add(new ListItem("不拘", string.Empty));
            var cities = GetCities();
            foreach (var city in cities)
            {
                ddlCity.Items.Add(city);
            }

            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("不拘", string.Empty));

            ddlEditStatus.Items.Clear();
            ddlEditStatus.Items.Add(new ListItem("不拘", string.Empty));
            ddlEditStatus.Items.Add(new ListItem("草稿", ((int)TreeEditState.草稿).ToString()));
            ddlEditStatus.Items.Add(new ListItem("完稿", ((int)TreeEditState.完稿).ToString()));

            ddlTreeStatus.Items.Clear();
            ddlTreeStatus.Items.Add(new ListItem("不拘", string.Empty));
            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddlTreeStatus.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }

            ddlBulkStatus.Items.Clear();
            ddlBulkStatus.Items.Add(new ListItem("請選擇", string.Empty));
            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddlBulkStatus.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }

            ddlSpecies.Items.Clear();
            ddlSpecies.Items.Add(new ListItem("不拘", string.Empty));
            var species = TreeService.GetSpecies();
            foreach (var s in species)
            {
                ddlSpecies.Items.Add(new ListItem(s.DisplayName, s.SpeciesID.ToString()));
            }
        }

        private TreeFilter BuildFilter()
        {
            DateTime? ParseDate(string text)
            {
                if (DateTime.TryParse(text, out DateTime dt))
                {
                    return dt;
                }
                return null;
            }

            return new TreeFilter
            {
                CityID = string.IsNullOrWhiteSpace(ddlCity.SelectedValue) ? (int?)null : Convert.ToInt32(ddlCity.SelectedValue),
                AreaID = string.IsNullOrWhiteSpace(ddlArea.SelectedValue) ? (int?)null : Convert.ToInt32(ddlArea.SelectedValue),
                EditStatus = string.IsNullOrWhiteSpace(ddlEditStatus.SelectedValue) ? (TreeEditState?)null : (TreeEditState)Convert.ToInt32(ddlEditStatus.SelectedValue),
                Status = string.IsNullOrWhiteSpace(ddlTreeStatus.SelectedValue) ? (TreeStatus?)null : (TreeStatus)Convert.ToInt32(ddlTreeStatus.SelectedValue),
                SpeciesID = string.IsNullOrWhiteSpace(ddlSpecies.SelectedValue) ? (int?)null : Convert.ToInt32(ddlSpecies.SelectedValue),
                SurveyDateStart = ParseDate(txtSurveyStart.Text),
                SurveyDateEnd = ParseDate(txtSurveyEnd.Text),
                AnnouncementDateStart = ParseDate(txtAnnouncementStart.Text),
                AnnouncementDateEnd = ParseDate(txtAnnouncementEnd.Text),
                Keyword = txtKeyword.Text.Trim(),
                SourceUnit = txtSourceUnit.Text.Trim()
            };
        }

        private void BindAreas()
        {
            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("不拘", string.Empty));
            if (string.IsNullOrWhiteSpace(ddlCity.SelectedValue)) return;

            using (var da = new DataAccess.MS_SQL())
            {
                const string sql = "SELECT twID, area FROM System_Taiwan WHERE cityID=@city ORDER BY area";
                var dt = da.GetDataTable(sql, new System.Data.SqlClient.SqlParameter("@city", ddlCity.SelectedValue));
                foreach (DataRow row in dt.Rows)
                {
                    ddlArea.Items.Add(new ListItem(row["area"].ToString(), row["twID"].ToString()));
                }
            }
        }

        private void BindTrees()
        {
            var filter = BuildFilter();
            var records = TreeService.SearchTrees(filter) ?? new List<TreeRecord>();

            var sortExpression = ViewState[SortExpressionKey] as string;
            var sortDirection = ViewState[SortDirectionKey] as string ?? "ASC";

            records = ApplySort(records, sortExpression, sortDirection);

            gvTrees.DataSource = records;
            gvTrees.DataBind();

            var total = TreeService.SearchTrees(null)?.Count ?? 0;
            lblCount.Text = $"資料總筆數：{total}／查詢結果：{records.Count}";
        }

        private static List<TreeRecord> ApplySort(IEnumerable<TreeRecord> source, string expression, string direction)
        {
            if (string.IsNullOrWhiteSpace(expression)) return source.ToList();

            Func<TreeRecord, object> keySelector;

            switch (expression)
            {
                case "SystemTreeNo": keySelector = r => r.SystemTreeNo; break;
                case "AgencyTreeNo": keySelector = r => r.AgencyTreeNo; break;
                case "AgencyJurisdictionCode": keySelector = r => r.AgencyJurisdictionCode; break;
                case "CityName": keySelector = r => r.CityName; break;
                case "AreaName": keySelector = r => r.AreaName; break;
                case "SpeciesDisplayName": keySelector = r => r.SpeciesDisplayName; break;
                case "SurveyDate": keySelector = r => r.SurveyDate; break;
                case "AnnouncementDate": keySelector = r => r.AnnouncementDate; break;
                case "Status": keySelector = r => r.Status; break;
                case "EditStatus": keySelector = r => r.EditStatus; break;
                default: keySelector = r => r.SystemTreeNo; break;
            }

            return string.Equals(direction, "DESC", StringComparison.OrdinalIgnoreCase)
                ? source.OrderByDescending(keySelector).ToList()
                : source.OrderBy(keySelector).ToList();
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAreas();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindTrees();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlCity.SelectedIndex = 0;
            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("不拘", string.Empty));
            ddlEditStatus.SelectedIndex = 0;
            ddlTreeStatus.SelectedIndex = 0;
            ddlSpecies.SelectedIndex = 0;
            txtSurveyStart.Text = string.Empty;
            txtSurveyEnd.Text = string.Empty;
            txtAnnouncementStart.Text = string.Empty;
            txtAnnouncementEnd.Text = string.Empty;
            txtKeyword.Text = string.Empty;
            txtSourceUnit.Text = string.Empty;
            BindTrees();
        }

        protected void gvTrees_Sorting(object sender, GridViewSortEventArgs e)
        {
            string currentExpression = ViewState[SortExpressionKey] as string;
            string currentDirection = ViewState[SortDirectionKey] as string ?? "ASC";

            if (string.Equals(currentExpression, e.SortExpression, StringComparison.OrdinalIgnoreCase))
            {
                currentDirection = currentDirection == "ASC" ? "DESC" : "ASC";
            }
            else
            {
                currentExpression = e.SortExpression;
                currentDirection = "ASC";
            }

            ViewState[SortExpressionKey] = currentExpression;
            ViewState[SortDirectionKey] = currentDirection;

            BindTrees();
        }

        protected void gvTrees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditTree" || e.CommandName == "ViewTree")
            {
                setTreeID = e.CommandArgument.ToString();
                string target = e.CommandName == "EditTree" ? "edit.aspx" : "detail.aspx";
                Response.Redirect(target);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            setTreeID = string.Empty;
            Response.Redirect("edit.aspx");
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var filter = BuildFilter();
            DataTable data = TreeService.ExportTrees(filter);
            if (data == null || data.Rows.Count == 0)
            {
                ShowMessage("下載列表", "目前無可匯出資料", "info");
                return;
            }

            OperationLogger.InsertLog("樹籍列表", "匯出", "下載樹籍基本資料");

            var sb = new StringBuilder();
            sb.Append("<table border='1'>");
            sb.Append("<tr>");
            foreach (DataColumn col in data.Columns)
            {
                sb.AppendFormat("<th>{0}</th>", col.ColumnName);
            }
            sb.Append("</tr>");

            foreach (DataRow row in data.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn col in data.Columns)
                {
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(row[col]));
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=TreeRecords.xls");
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(sb.ToString());
            Response.End();
        }

        protected void btnApplyStatus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlBulkStatus.SelectedValue))
            {
                ShowMessage("批次設定", "請選擇狀態", "warning");
                return;
            }

            var selected = new List<int>();
            foreach (GridViewRow row in gvTrees.Rows)
            {
                var chk = row.FindControl("chkSelect") as CheckBox;
                var hid = row.FindControl("hfTreeId") as HiddenField;
                if (chk != null && hid != null && chk.Checked && int.TryParse(hid.Value, out int id))
                {
                    selected.Add(id);
                }
            }

            if (selected.Count == 0)
            {
                ShowMessage("批次設定", "請先選擇要更新的樹籍", "warning");
                return;
            }

            var status = (TreeStatus)Convert.ToInt32(ddlBulkStatus.SelectedValue);
            DateTime? announcement = null;
            if (DateTime.TryParse(txtBulkAnnouncement.Text, out DateTime dt))
            {
                announcement = dt;
            }

            var user = UserService.GetCurrentUser();
            int accountId = user?.userID ?? 0;

            TreeService.BulkUpdateStatus(selected, status, announcement, accountId);
            OperationLogger.InsertLog("樹籍管理", "批次設定", $"更新{selected.Count}筆狀態為{TreeService.GetStatusText(status)}");
            foreach (var treeId in selected)
            {
                TreeService.InsertTreeLog(treeId,
                    "批次設定樹籍狀態",
                    $"狀態更新為{TreeService.GetStatusText(status)}",
                    Request?.UserHostAddress,
                    user?.userID,
                    user?.account,
                    user?.name,
                    user?.unit);
            }
            BindTrees();
        }
    }
}
