using System;
using System.Collections.Generic;
using System.Linq;
using protectTreesV2.Base;

namespace protectTreesV2.backstage.care
{
    public partial class edit : BasePage
    {
        public int CurrentCareID
        {
            get { return (int)(ViewState["CurrentCareID"] ?? 0); }
            set { ViewState["CurrentCareID"] = value; }
        }

        public int CurrentTreeID
        {
            get { return (int)(ViewState["CurrentTreeID"] ?? 0); }
            set { ViewState["CurrentTreeID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitIdsFromSession();

                if (CurrentCareID == 0 && int.TryParse(Request.QueryString["id"], out int careId))
                {
                    CurrentCareID = careId;
                }

                bool isEdit = CurrentCareID > 0;
                Literal_pathAction.Text = isEdit ? "編輯" : "新增";
                Literal_title.Text = isEdit ? "養護紀錄編輯" : "養護紀錄新增";
                HiddenField_treeId.Value = CurrentTreeID.ToString();
                HiddenField_careId.Value = CurrentCareID.ToString();
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

        private void InitIdsFromSession()
        {
            base.KeepState();

            if (!string.IsNullOrEmpty(this.setCareID))
            {
                if (int.TryParse(this.setCareID, out int careId))
                {
                    CurrentCareID = careId;
                }
                this.setCareID = null;
            }

            if (!string.IsNullOrEmpty(this.setTreeID))
            {
                if (int.TryParse(this.setTreeID, out int treeId))
                {
                    CurrentTreeID = treeId;
                }
                this.setTreeID = null;
            }
        }
    }
}
