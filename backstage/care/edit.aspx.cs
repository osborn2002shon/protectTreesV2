using System;
using System.Collections.Generic;
using System.Linq;
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
                BindCarePhotoBlocks(1);
            }
        }

        protected void Button_addCarePhotoBlock_Click(object sender, EventArgs e)
        {
            int count = ViewState["CarePhotoBlockCount"] as int? ?? 1;
            count += 1;
            BindCarePhotoBlocks(count);
        }

        private void BindCarePhotoBlocks(int count)
        {
            ViewState["CarePhotoBlockCount"] = count;
            Repeater_carePhotos.DataSource = Enumerable.Range(1, count).ToList();
            Repeater_carePhotos.DataBind();
        }
    }
}
