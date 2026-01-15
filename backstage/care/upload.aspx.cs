using protectTreesV2.Base;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using static protectTreesV2.Batch.Batch;

namespace protectTreesV2.backstage.care
{
    public partial class upload : BasePage
    {
        protectTreesV2.Batch.Batch system_batch = new Batch.Batch();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }
        private void BindData()
        {
            // 取得當前使用者 ID
            var user = UserInfo.GetCurrentUser;
            int userId = user?.accountID ?? 0;

            // 綁定歷史紀錄
            var historyList = system_batch.GetBatchTaskList(enum_treeBatchType.Care_Record, userId);
            GridView_History.DataSource = historyList.Take(5).ToList();
            GridView_History.DataBind();

            // 綁定最新一筆的明細 (若有歷史紀錄)
            if (historyList.Count > 0)
            {
                var latest = historyList[0];
                Label_LastStatus.Text = $"最新上傳：{latest.insertDateTime?.ToString("yyyy/MM/dd HH:mm")} (成功 {latest.successCount} / 失敗 {latest.failCount})";

                var details = system_batch.GetLatestBatchTaskLogs(latest.taskID);
                GridView_Detail.DataSource = details;
                GridView_Detail.DataBind();
            }
            else
            {
                Label_LastStatus.Text = "目前尚無上傳紀錄";
                GridView_Detail.DataSource = null;
                GridView_Detail.DataBind();
            }
        }
        protected void Button_StartUpload_Click(object sender, EventArgs e)
        {

        }
    }
}