using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataAccess;

namespace protectTreesV2.backstage.system
{
    public partial class treeList : System.Web.UI.Page
    {
        private static readonly Regex ScientificSeparatorRegex = new Regex(@"\s+(var\.|subsp\.)\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSpecies();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvSpecies.PageIndex = 0;
            BindSpecies();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtKeyword.Text = string.Empty;
            gvSpecies.PageIndex = 0;
            BindSpecies();
        }

        protected string FormatScientificName(string scientificName)
        {
            if (string.IsNullOrWhiteSpace(scientificName))
            {
                return string.Empty;
            }

            var trimmed = scientificName.Trim();
            var matches = ScientificSeparatorRegex.Matches(trimmed);
            if (matches.Count == 0)
            {
                return $"<i>{HttpUtility.HtmlEncode(trimmed)}</i>";
            }

            var sb = new StringBuilder();
            var lastIndex = 0;

            foreach (Match match in matches)
            {
                var segment = trimmed.Substring(lastIndex, match.Index - lastIndex);
                if (!string.IsNullOrWhiteSpace(segment))
                {
                    sb.Append("<i>");
                    sb.Append(HttpUtility.HtmlEncode(segment));
                    sb.Append("</i>");
                }

                sb.Append(HttpUtility.HtmlEncode(match.Value));
                lastIndex = match.Index + match.Length;
            }

            var tail = trimmed.Substring(lastIndex);
            if (!string.IsNullOrWhiteSpace(tail))
            {
                sb.Append("<i>");
                sb.Append(HttpUtility.HtmlEncode(tail));
                sb.Append("</i>");
            }

            return sb.ToString();
        }

        protected string FormatNative(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "--";
            }

            return Convert.ToBoolean(value) ? "是" : "否";
        }

        protected void gvSpecies_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSpecies.PageIndex = e.NewPageIndex;
            BindSpecies();
        }

        private void BindSpecies()
        {
            var keyword = txtKeyword.Text?.Trim();
            var data = GetSpeciesData(keyword);
            gvSpecies.DataSource = data;
            gvSpecies.DataBind();
            lblCount.Text = $"共 {data.Rows.Count} 筆";
        }

        private DataTable GetSpeciesData(string keyword)
        {
            var sql = new StringBuilder(@"
SELECT
    speciesID,
    commonName,
    scientificName,
    isNative,
    memo
FROM Tree_Species
WHERE isActive = 1");

            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql.Append(@"
AND (
    commonName LIKE @keyword
    OR scientificName LIKE @keyword
    OR aliasName LIKE @keyword
    OR memo LIKE @keyword
)");
                parameters.Add(new SqlParameter("@keyword", $"%{keyword}%"));
            }

            sql.Append(@"
ORDER BY ISNULL(orderBy, 999999), commonName");

            using (var da = new MS_SQL())
            {
                return da.GetDataTable(sql.ToString(), parameters.ToArray());
            }
        }
    }
}
