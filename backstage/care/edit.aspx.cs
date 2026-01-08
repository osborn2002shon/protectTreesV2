using System;
using protectTreesV2.Base;

namespace protectTreesV2.backstage.care
{
    public partial class edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool isEdit = !string.IsNullOrEmpty(Request.QueryString["id"]);
                Literal_pathAction.Text = isEdit ? "編輯" : "新增";
                Literal_title.Text = isEdit ? "養護紀錄編輯" : "養護紀錄新增";
            }
        }
    }
}
