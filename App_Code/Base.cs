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

        public string setHealthID
        {
            get
            {
                return Session["setHealthID"] != null ? Session["setHealthID"].ToString() : string.Empty;
            }
            set
            {
                Session["setHealthID"] = value;
            }
        }
        public string setPatrolID
        {
            get
            {
                return Session["setPatrolID"] != null ? Session["setPatrolID"].ToString() : string.Empty;
            }
            set
            {
                Session["setPatrolID"] = value;
            }
        }

        [Serializable]
        public class PageStateInfo
        {
            public const string sessionKey = "__PageState_Session";
            public const string viewStateKey = "__PageState_View";
            public string sourcePage { get; set; }
            public object filterData { get; set; }
        }
        /// <summary>
        /// 存狀態並跳轉
        /// </summary>
        public void RedirectState(string url, object filter)
        {
            // 直接使用 PageStateInfo 裡的 Key
            Session[PageStateInfo.sessionKey] = new PageStateInfo
            {
                sourcePage = Request.Url.PathAndQuery,
                filterData = filter
            };
            Response.Redirect(url);
        }

        /// <summary>
        ///  取回頁面紀錄
        /// </summary>
        public T GetState<T>() where T : class
        {
            if (Session[PageStateInfo.sessionKey] is PageStateInfo state)
            {
                string currentPath = Request.Url.AbsolutePath;

                if (state.sourcePage != null &&
                 state.sourcePage.StartsWith(currentPath, StringComparison.OrdinalIgnoreCase))
                {
                    Session.Remove(PageStateInfo.sessionKey);
                    return state.filterData as T;
                }
                else
                {
                    // 路徑不對
                    Session.Remove(PageStateInfo.sessionKey);
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 保存頁面紀錄
        /// </summary>
        public void KeepState()
        {
            if (Session[PageStateInfo.sessionKey] is PageStateInfo state)
            {
                ViewState[PageStateInfo.viewStateKey] = state;
                Session.Remove(PageStateInfo.sessionKey);
            }
        }

        /// <summary>
        /// 返回頁面
        /// </summary>
        public void ReturnState()
        {
            if (ViewState[PageStateInfo.viewStateKey] is PageStateInfo state)
            {
                Session[PageStateInfo.sessionKey] = state; // 丟回 Session
                Response.Redirect(state.sourcePage);       // 跳回來源頁
            }
            else
            {
                Response.Redirect("/Login.aspx");
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
                        bodyElement.innerHTML = data.text || '';
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

        /// <summary>
        /// 綁定縣市下拉選單
        /// </summary>
        /// <param name="ddl">下拉選單控制項</param>
        /// <param name="showAll">是否顯示「不拘/請選擇」，預設 true</param>
        public static void Bind_DropDownList_City(ref DropDownList ddl, bool showAll = true)
        {
            ddl.Items.Clear();

            // 判斷是否要加入預設選項
            if (showAll)
            {
                ddl.Items.Add(new ListItem("縣市不拘*", string.Empty));
            }

            using (var da = new DataAccess.MS_SQL())
            {
                const string sql = "SELECT DISTINCT cityID, city FROM System_Taiwan ORDER BY cityID";

                var dt = da.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    ddl.Items.Add(new ListItem(row["city"].ToString(), row["cityID"].ToString()));
                }
            }
        }

        /// <summary>
        /// 綁定鄉鎮下拉選單 (依據縣市ID)
        /// </summary>
        /// <param name="ddl">下拉選單控制項</param>
        /// <param name="cityID">選中的縣市 ID</param>
        /// <param name="showAll">是否顯示「不拘/請選擇」，預設 true</param>
        public static void Bind_DropDownList_Area(ref DropDownList ddl, string cityID, bool showAll = true)
        {
            ddl.Items.Clear();

            // 判斷是否要加入預設選項
            if (showAll)
            {
                ddl.Items.Add(new ListItem("鄉鎮不拘*", string.Empty));
            }

            // 如果沒有傳入縣市ID，就只顯示「不拘」
            if (string.IsNullOrWhiteSpace(cityID)) return;

            using (var da = new DataAccess.MS_SQL())
            {
                const string sql = "SELECT twID, area FROM System_Taiwan WHERE cityID=@city ORDER BY area";

                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                new System.Data.SqlClient.SqlParameter("@city", cityID)
                };

                var dt = da.GetDataTable(sql, parameters);
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    ddl.Items.Add(new System.Web.UI.WebControls.ListItem(row["area"].ToString(), row["twID"].ToString()));
                }
            }
        }

        /// <summary>
        /// 綁定樹牌狀態
        /// </summary>
        /// <param name="ddl">下拉選單</param>
        /// <param name="showSelect">是否顯示「請選擇」，預設 true</param>
        public static void Bind_enum_treeSignStatus(ref DropDownList ddl, bool showSelect = true)
        {
            ddl.Items.Clear();

            if (showSelect)
            {
                ddl.Items.Add(new ListItem("請選擇", ""));
            }

            // 遍歷 Enum
            foreach (Health.Health.enum_treeSignStatus item in Enum.GetValues(typeof(Health.Health.enum_treeSignStatus)))
            {
                string text = item.ToString();
                string value = ((int)item).ToString(); 

                ddl.Items.Add(new ListItem(text, value));
            }
        }

        /// <summary>
        /// 綁定錯誤修剪傷害
        /// </summary>
        /// <param name="ddl">下拉選單</param>
        /// <param name="showSelect">是否顯示「請選擇」，預設 true</param>
        public static void Bind_enum_pruningDamageType(ref DropDownList ddl, bool showSelect = true)
        {
            ddl.Items.Clear();

            if (showSelect)
            {
                ddl.Items.Add(new ListItem("請選擇", ""));
            }

            foreach (Health.Health.enum_pruningDamageType item in Enum.GetValues(typeof(Health.Health.enum_pruningDamageType)))
            {
                string text = item.ToString();
                string value = text; 

                ddl.Items.Add(new ListItem(text, value));
            }
        }

        /// <summary>
        /// 綁定建議處理優先順序
        /// </summary>
        /// <param name="ddl">下拉選單</param>
        /// <param name="showSelect">是否顯示「請選擇」，預設 true</param>
        public static void Bind_enum_treatmentPriority(ref DropDownList ddl, bool showSelect = true)
        {
            ddl.Items.Clear();

            if (showSelect)
            {
                ddl.Items.Add(new ListItem("請選擇", ""));
            }

            foreach (Health.Health.enum_treatmentPriority item in Enum.GetValues(typeof(Health.Health.enum_treatmentPriority)))
            {
                string text = item.ToString();
                string value = text; 

                ddl.Items.Add(new ListItem(text, value));
            }
        }
    }
}
