using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace protectTreesV2._mp
{
    public partial class mp_backstage : System.Web.UI.MasterPage
    {

        public class stru_MenuItem
        {
            public int menuID { get; set; }
            public string groupName { get; set; }
            public string menuName { get; set; }
            public string menuURL { get; set; }
            public string iconClass { get; set; }
            public int orderBy_group { get; set; }
            public int orderBy_menu { get; set; }
            public bool isActive { get; set; }
            public bool isShow { get; set; }
            public bool canRead { get; set; }
        }


        /// <summary>
        /// 讓子頁面可以指定其父選單頁面（用於非直接顯示在選單上的頁面）
        /// </summary>
        public string ParentMenuPage { get; set; }

        /// <summary>
        /// 目錄字串
        /// </summary>
        public string menuStr_def
        {
            get { return ViewState["mp_menuStr_def"] as string ?? string.Empty; }
            set { ViewState["mp_menuStr_def"] = value; }
        }

        /// <summary>
        /// 燈箱是否固定（static backdrop）
        /// </summary>
        public string modalStatic = "";

        /// <summary>
        /// 燈箱是否固定（static backdrop）
        /// </summary>
        public bool isNeedStaticModal
        {
            get { return ViewState["isNeedStaticModal"] as bool? ?? false; }
            set { ViewState["isNeedStaticModal"] = value; }
        }

        /// <summary>
        /// 群組名稱與 Icon 對應表
        /// </summary>
        private Dictionary<string, string> GroupIcons
        {
            get
            {
                return new Dictionary<string, string>
        {
            { "系統管理", "fa-solid fa-cogs" },
            { "資料管理", "fa-solid fa-database" },
            { "統計報表", "fa-solid fa-magnifying-glass-chart" }
        };
            }
        }

        /// <summary>
        /// 將路徑統一為以 "/" 開頭的相對路徑（支援虛擬目錄）
        /// </summary>
        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            // 換斜線 + 去空白
            path = path.Replace("\\", "/").Trim();

            // 移除常見相對符號
            path = path.Replace("~/", "/").Replace("../", "/").Replace("./", "/");

            // 移除虛擬目錄前綴
            string appPath = Request.ApplicationPath;
            if (!string.IsNullOrEmpty(appPath) && appPath != "/")
            {
                if (path.StartsWith(appPath, StringComparison.OrdinalIgnoreCase))
                {
                    path = path.Substring(appPath.Length);
                }
            }

            // 移除固定層 /backstage/
            if (path.StartsWith("/backstage/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring("/backstage".Length);
            }

            // 確保開頭為 "/"
            if (!path.StartsWith("/"))
                path = "/" + path;

            return path.ToLower();
        }


        /// <summary>
        /// 清單動態組出選單 HTML（新版，含 active 判斷）
        /// </summary>
        private string BuildMenuHTML(List<stru_MenuItem> menuList, string currentPagePath)
        {
            var sb = new StringBuilder();

            // 分組
            var groups = menuList
                .Where(x => x.isShow && x.canRead)
                .GroupBy(m => m.groupName)
                .OrderBy(g => g.First().orderBy_group);

            foreach (var grp in groups)
            {
                string groupName = grp.Key;
                string menuID = groupName.Replace(" ", "") + "Menu";

                // 是否有 active 項目
                bool hasActiveItem = grp.Any(x =>
                    string.Equals(NormalizePath(x.menuURL), currentPagePath, StringComparison.OrdinalIgnoreCase));

                // 群組 icon
                string groupIcon = GroupIcons.ContainsKey(groupName)
                    ? GroupIcons[groupName]
                    : "fas fa-folder";

                sb.AppendLine("    <div class='nav-item'>");
                sb.AppendLine($"      <a class='nav-link' href='#' data-bs-toggle='collapse' data-bs-target='#{menuID}'>");
                sb.AppendLine($"        <i class='{groupIcon}'></i>{groupName}");
                sb.AppendLine("        <i class='fas fa-chevron-down float-end mt-1'></i>");
                sb.AppendLine("      </a>");

                // active 群組預設展開
                string collapseClass = hasActiveItem ? "collapse show nav-submenu" : "collapse nav-submenu";
                sb.AppendLine($"      <div class='{collapseClass}' id='{menuID}'>");

                // 子選單
                foreach (var item in grp.OrderBy(x => x.orderBy_menu))
                {
                    string menuName = item.menuName;
                    string menuURL = item.menuURL;
                    string iconClass = string.IsNullOrEmpty(item.iconClass)
                        ? "fa-solid fa-angle-right"
                        : item.iconClass;

                    bool isActive = string.Equals(NormalizePath(menuURL), currentPagePath, StringComparison.OrdinalIgnoreCase);
                    string activeClass = isActive ? " active" : "";

                    sb.AppendLine($"        <a class='nav-link{activeClass}' href='{menuURL}'>");
                    sb.AppendLine($"          <i class='{iconClass}'></i>{menuName}");
                    sb.AppendLine("        </a>");
                }

                sb.AppendLine("      </div>");
                sb.AppendLine("    </div>");
            }

            return sb.ToString();
        }


        /// <summary>
        /// 取得使用者可用選單資料（直接轉成 stru_MenuItem 結構清單）
        /// </summary>
        /// <param name="auTypeID">使用者角色代碼</param>
        /// <returns>List&lt;stru_MenuItem&gt;</returns>
        public List<stru_MenuItem> Get_MenuData(int auTypeID)
        {
            string sqlString =
                "SELECT DISTINCT System_Menu.menuID, System_Menu.groupName, System_Menu.menuName, System_Menu.menuURL, " +
                "       System_Menu.iconClass, System_Menu.orderBy_group, System_Menu.orderBy_menu, " +
                "       System_Menu.isActive, System_Menu.isShow " +
                "FROM System_MenuAu " +
                "INNER JOIN System_Menu ON System_MenuAu.menuID = System_Menu.menuID " +
                "WHERE System_Menu.isActive = 1 AND System_MenuAu.auTypeID = @auTypeID " +
                "ORDER BY System_Menu.orderBy_group, System_Menu.orderBy_menu";

            var para = new System.Data.SqlClient.SqlParameter("@auTypeID", auTypeID);

            var menuList = new List<stru_MenuItem>();

            using (var da = new DataAccess.MS_SQL())
            {
                DataTable dt = da.GetDataTable(sqlString, para);

                foreach (DataRow row in dt.Rows)
                {
                    var item = new stru_MenuItem
                    {
                        menuID = row.Field<int>("menuID"),
                        groupName = row.Field<string>("groupName"),
                        menuName = row.Field<string>("menuName"),
                        menuURL = row.Field<string>("menuURL"),
                        iconClass = row.IsNull("iconClass")
                                    ? "fa-solid fa-angle-right"
                                    : row.Field<string>("iconClass"),
                        orderBy_group = row.Field<int>("orderBy_group"),
                        orderBy_menu = row.Field<int>("orderBy_menu"),
                        isActive = row.Field<bool>("isActive"),
                        isShow = row.Field<bool>("isShow"),
                        canRead = true
                    };

                    menuList.Add(item);
                }
            }

            return menuList;
        }

        protected override void OnInit(EventArgs e)
        {
            // 若檢查沒有正確的 Session，直接返回登入畫面
            var ui = UserInfo.GetCurrentUser;
            if (ui == null || !ui.isExist)
            {
                Response.Redirect("~/Login.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                // 目前頁面路徑
                string currentPage = NormalizePath(Request.Path);

                // 要亮的目錄選單名稱
                string activeMenuPage = string.IsNullOrEmpty(ParentMenuPage)
                    ? currentPage
                    : NormalizePath(ParentMenuPage);

                // 組成目錄
                if (string.IsNullOrEmpty(menuStr_def))
                {
                    menuStr_def = BuildMenuHTML(Get_MenuData(1), activeMenuPage);
                }

                // 處理燈箱（Modal）
                if (isNeedStaticModal)
                {
                    modalStatic = " data-bs-backdrop='static' data-bs-keyboard='false' ";
                    Panel_closeModal.Visible = false;
                    Panel_closeModal_img.Visible = false;
                }
                else
                {
                    modalStatic = "";
                    Panel_closeModal.Visible = true;
                    Panel_closeModal_img.Visible = true;
                }
            }
        }

        protected void LinkButton_logout_Click(object sender, EventArgs e)
        {
            UserInfo.SignOut();
            Response.Redirect("~/Login.aspx");
        }
    }
}