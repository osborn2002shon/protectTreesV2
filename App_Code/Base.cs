using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.Base
{
    public class BasePage : System.Web.UI.Page
    {
        public string setTreeID
        {
            get
            {
                return Session["setTreeID"] != null ? Session["setTreeID"].ToString() : string.Empty;
            }
            set
            {
                Session["setTreeID"] = value;
            }
        }
        public List<ListItem> GetCities()
        {
            using (var da = new DataAccess.MS_SQL())
            {
                string sql = "SELECT distinct cityID, city FROM System_Taiwan ORDER BY cityID";
                var dt = da.GetDataTable(sql);
                List<ListItem> cities = new List<ListItem>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    ListItem item = new ListItem
                    {
                        Value = row["cityID"].ToString(),
                        Text = row["city"].ToString()
                    };
                    cities.Add(item);
                }
                return cities;
            }
        }

        public void ShowMessage(string title, string text, string icon = "info")
        {
            var serializer = new JavaScriptSerializer();
            var options = new { title, text, icon };

            string script = $@"
                (function() {{
                    var data = {serializer.Serialize(options)};
                    var modalElement = document.getElementById('divMSG');
                    if (!modalElement || typeof bootstrap === 'undefined') return;

                    var titleElement = modalElement.querySelector('.modal-title');
                    var bodyElement = modalElement.querySelector('.modal-body');
                    var iconMap = {{
                        success: 'fa-circle-check text-success',
                        warning: 'fa-triangle-exclamation text-warning',
                        error: 'fa-circle-xmark text-danger',
                        info: 'fa-circle-info text-info'
                    }};
                    var iconClass = iconMap[(data.icon || '').toLowerCase()] || iconMap.info;

                    if (titleElement) {{
                        titleElement.innerHTML = '';
                        var icon = document.createElement('i');
                        icon.className = 'message-icon fa-solid ' + iconClass;
                        titleElement.appendChild(icon);
                        titleElement.appendChild(document.createTextNode(' ' + (data.title || '')));
                    }}

                    if (bodyElement) {{
                        bodyElement.textContent = data.text || '';
                    }}

                    var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
                    modal.show();
                }})();";
            string key = "BootstrapModal_" + Guid.NewGuid().ToString("N");

            if (ScriptManager.GetCurrent(Page) != null)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), key, script, true);
            }
        }
    }

    public static class DataRowHelper
    {
        public static bool HasColumn(DataRow row, string columnName)
        {
            return row?.Table?.Columns.Contains(columnName) ?? false;
        }

        public static object GetValue(DataRow row, string columnName)
        {
            if (!HasColumn(row, columnName))
            {
                return null;
            }

            var value = row[columnName];
            return value == DBNull.Value ? null : value;
        }

        public static string GetString(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value?.ToString();
        }

        public static int? GetNullableInt(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (int?)null : Convert.ToInt32(value);
        }

        public static decimal? GetNullableDecimal(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (decimal?)null : Convert.ToDecimal(value);
        }

        public static DateTime? GetNullableDateTime(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (DateTime?)null : Convert.ToDateTime(value);
        }

        public static bool GetBoolean(DataRow row, string columnName, bool defaultValue = false)
        {
            var value = GetValue(row, columnName);
            return value == null ? defaultValue : Convert.ToBoolean(value);
        }

        public static bool? GetNullableBoolean(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (bool?)null : Convert.ToBoolean(value);
        }
    }

    public static class DropdownBinder
    {
        public static void Bind_DropDownList_LandType(ref DropDownList dropdownlist)
        {
            dropdownlist.Items.Clear();
            dropdownlist.Items.Add(new ListItem("請選擇", string.Empty));

            dropdownlist.Items.Add(new ListItem("國有"));
            dropdownlist.Items.Add(new ListItem("公有"));
            dropdownlist.Items.Add(new ListItem("私有"));
            dropdownlist.Items.Add(new ListItem("其他"));
            dropdownlist.Items.Add(new ListItem("無資料"));
        }

        public static void Bind_DropDownList_Species(ref DropDownList dropdownlist)
        {
            dropdownlist.Items.Clear();
            dropdownlist.Items.Add(new ListItem("請選擇", string.Empty));

            foreach (var species in TreeService.GetSpecies())
            {
                dropdownlist.Items.Add(new ListItem(species.DisplayName, species.SpeciesID.ToString()));
            }

            return;
        }
    }
}
