using protectTreesV2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace protectTreesV2.backstage.health
{
    public partial class edit : BasePage
    {
        /// <summary>
        /// 目前編輯的健檢 ID (若為 0 代表新增)
        /// </summary>
        public int CurrentHealthID
        {
            get { return (int)(ViewState["CurrentHealthID"] ?? 0); }
            set { ViewState["CurrentHealthID"] = value; }
        }

        /// <summary>
        /// 目前針對的樹木 ID 
        /// </summary>
        public int CurrentTreeID
        {
            get { return (int)(ViewState["CurrentTreeID"] ?? 0); }
            set { ViewState["CurrentTreeID"] = value; }
        }

        /// <summary>
        /// 動作判斷
        /// </summary>
        public string Action
        {
            get { return CurrentHealthID > 0 ? "edit" : "add"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 判斷動作
                InitIdsFromSession();

                // 安全檢查：如果兩個 ID 都沒有，代表非法進入
                if (CurrentHealthID == 0 && CurrentTreeID == 0)
                {
                    ShowMessage("錯誤", "無效的參數，請由列表頁進入。");
                    Response.Redirect("list.aspx");
                    return;
                }

                // C. 初始化下拉選單等基礎介面
                InitDropdowns();

                // D. 設定標題文字
                SetPageTitle();

                // E. 如果是編輯模式，載入舊資料
                if (Action == "edit")
                {
                    LoadEditData();
                }
            }
        }

        private void InitIdsFromSession()
        {
            // 處理健檢 ID 
            if (!string.IsNullOrEmpty(this.setHealthID))
            {
                int hId;
                if (int.TryParse(this.setHealthID, out hId))
                {
                    CurrentHealthID = hId;
                }
                // 清空 Session
                this.setHealthID = null;
            }

            // 處理樹木 ID 
            if (!string.IsNullOrEmpty(this.setTreeID))
            {
                int tId;
                if (int.TryParse(this.setTreeID, out tId))
                {
                    CurrentTreeID = tId;
                }
                // 清空 Session
                this.setTreeID = null;
            }
        }

        /// <summary>
        /// 設定頁面標題變數
        /// </summary>
        private void SetPageTitle()
        {
            if (this.Action == "add")
            {
                // 新增模式
                Literal_pathAction.Text = "新增";
                Literal_title.Text = "健檢紀錄新增";
            }
            else
            {
                // 編輯模式
                Literal_pathAction.Text = "編輯";
                Literal_title.Text = "健檢紀錄編輯";
            }
        }

        /// <summary>
        /// 初始化下拉選單
        /// </summary>
        private void InitDropdowns()
        {
            // TODO: 載入縣市、樹種、狀態(草稿/完稿)等下拉選單
        }

        /// <summary>
        /// 載入編輯資料
        /// </summary>
        private void LoadEditData()
        {
            // 使用 CurrentHealthID 查詢資料
            // var record = TreeService.GetHealthRecordByID(this.CurrentHealthID);
            // BindData(record);
        }

        /// <summary>
        /// 儲存按鈕事件 (範例)
        /// </summary>
        protected void LinkButton_save_Click(object sender, EventArgs e)
        {
            // 在這裡儲存時，請使用 this.CurrentHealthID 或 this.CurrentTreeID
            // 不要再用 setHealthID (Session)，因為已經空了
        }

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {

        }
    }
}