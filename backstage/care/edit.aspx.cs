using System;
using System.Collections.Generic;
using System.Linq;
using protectTreesV2.Base;
using protectTreesV2.Care;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.care
{
    public partial class edit : BasePage
    {
        private readonly Care.Care system_care = new Care.Care();
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

                if (isEdit)
                {
                    var record = system_care.GetCareRecord(CurrentCareID);
                    if (record == null)
                    {
                        ShowMessage("錯誤", "找不到養護資料");
                        Response.Redirect("list.aspx");
                        return;
                    }

                    CurrentTreeID = record.treeID;
                    TextBox_careDate.Text = record.careDate?.ToString("yyyy-MM-dd");
                    TextBox_recorder.Text = record.recorder;
                    TextBox_reviewer.Text = record.reviewer;
                    CheckBox_isFinal.Checked = record.dataStatus == (int)Care.Care.CareRecordStatus.定稿;
                    Label_recordStatus.CssClass = record.dataStatus == (int)Care.Care.CareRecordStatus.定稿 ? "badge bg-success" : "badge bg-warning text-dark";
                    Label_recordStatus.Text = record.dataStatus == (int)Care.Care.CareRecordStatus.定稿 ? "定稿" : "草稿";
                }
                else
                {
                    Label_recordStatus.CssClass = "badge bg-secondary";
                    Label_recordStatus.Text = "新增";
                }

                if (CurrentTreeID <= 0)
                {
                    Response.Redirect("/backstage/care/main.aspx");
                    return;
                }

                BindTreeInfo();
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

        private void BindTreeInfo()
        {
            var tree = TreeService.GetTree(CurrentTreeID);
            if (tree == null)
            {
                ShowMessage("錯誤", "無法取得樹籍資料");
                Response.Redirect("/backstage/care/main.aspx");
                return;
            }

            Label_systemTreeNo.Text = tree.SystemTreeNo ?? "--";
            Label_speciesName.Text = tree.SpeciesDisplayName ?? "--";
            Label_cityName.Text = tree.CityName ?? "--";
            Label_areaName.Text = tree.AreaName;
            Label_manager.Text = tree.Manager ?? "--";
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

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {
            base.ReturnState();
        }
    }
}
