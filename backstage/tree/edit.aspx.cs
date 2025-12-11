using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                LoadData();
            }
        }

        private void BindDropdowns()
        {
            ddlCity.Items.Clear();
            ddlCity.Items.Add(new ListItem("請選擇", string.Empty));
            foreach (var city in GetCities())
            {
                ddlCity.Items.Add(city);
            }

            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("請選擇", string.Empty));

            ddlStatus.Items.Clear();
            foreach (TreeStatus status in Enum.GetValues(typeof(TreeStatus)))
            {
                ddlStatus.Items.Add(new ListItem(TreeService.GetStatusText(status), ((int)status).ToString()));
            }

            ddlSpecies.Items.Clear();
            ddlSpecies.Items.Add(new ListItem("請選擇", string.Empty));
            foreach (var species in TreeService.GetSpecies())
            {
                ddlSpecies.Items.Add(new ListItem(species.DisplayName, species.SpeciesID.ToString()));
            }

            ddlLandOwnership.Items.Clear();
            ddlLandOwnership.Items.Add(new ListItem("請選擇", string.Empty));
            ddlLandOwnership.Items.Add(new ListItem("國有"));
            ddlLandOwnership.Items.Add(new ListItem("公有"));
            ddlLandOwnership.Items.Add(new ListItem("私有"));
            ddlLandOwnership.Items.Add(new ListItem("其他"));

            cblRecognition.Items.Clear();
            foreach (var item in TreeService.GetRecognitionCriteria())
            {
                cblRecognition.Items.Add(new ListItem(item.Name, item.Code));
            }
        }

        private void LoadData()
        {
            int id;
            if (int.TryParse(setTreeID, out id) && id > 0)
            {
                var tree = TreeService.GetTree(id);
                if (tree != null)
                {
                    hfTreeID.Value = tree.TreeID.ToString();
                    lblSystemTreeNo.Text = tree.SystemTreeNo;
                    txtAgencyTreeNo.Text = tree.AgencyTreeNo;
                    txtJurisdiction.Text = tree.AgencyJurisdictionCode;
                    SelectDropDown(ddlCity, tree.CityID);
                    BindAreas();
                    SelectDropDown(ddlArea, tree.AreaID);
                    SelectDropDown(ddlSpecies, tree.SpeciesID);
                    SelectDropDown(ddlStatus, (int)tree.Status);
                    txtSurveyDate.Text = tree.SurveyDate?.ToString("yyyy-MM-dd");
                    txtSurveyor.Text = tree.Surveyor;
                    txtAnnouncementDate.Text = tree.AnnouncementDate?.ToString("yyyy-MM-dd");
                    txtTreeCount.Text = tree.TreeCount.ToString();
                    txtLatitude.Text = tree.Latitude?.ToString();
                    txtLongitude.Text = tree.Longitude?.ToString();
                    txtSite.Text = tree.Site;
                    txtManager.Text = tree.Manager;
                    txtManagerContact.Text = tree.ManagerContact;
                    SelectDropDownText(ddlLandOwnership, tree.LandOwnership);
                    txtLandOwnershipNote.Text = tree.LandOwnershipNote;
                    txtFacility.Text = tree.FacilityDescription;
                    txtMemo.Text = tree.Memo;
                    txtTreeHeight.Text = tree.TreeHeight?.ToString();
                    txtBreastHeightDiameter.Text = tree.BreastHeightDiameter?.ToString();
                    txtBreastHeightCircumference.Text = tree.BreastHeightCircumference?.ToString();
                    txtCanopyArea.Text = tree.CanopyProjectionArea?.ToString();
                    txtRecognitionNote.Text = tree.RecognitionNote;
                    txtCulturalHistory.Text = tree.CulturalHistoryIntro;
                    txtHealth.Text = tree.HealthCondition;
                    foreach (ListItem item in cblRecognition.Items)
                    {
                        item.Selected = tree.RecognitionCriteria.Contains(item.Value);
                    }
                }
            }
        }

        private void SelectDropDown(DropDownList ddl, int? value)
        {
            if (!value.HasValue) return;
            var item = ddl.Items.FindByValue(value.Value.ToString());
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        private void SelectDropDownText(DropDownList ddl, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var item = ddl.Items.Cast<ListItem>().FirstOrDefault(i => i.Text == text);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAreas();
        }

        private void BindAreas()
        {
            ddlArea.Items.Clear();
            ddlArea.Items.Add(new ListItem("請選擇", string.Empty));
            if (string.IsNullOrWhiteSpace(ddlCity.SelectedValue)) return;

            using (var da = new DataAccess.MS_SQL())
            {
                const string sql = "SELECT twID, area FROM System_Taiwan WHERE cityID=@city ORDER BY area";
                var dt = da.GetDataTable(sql, new System.Data.SqlClient.SqlParameter("@city", ddlCity.SelectedValue));
                foreach (DataRow row in dt.Rows)
                {
                    ddlArea.Items.Add(new ListItem(row["area"].ToString(), row["twID"].ToString()));
                }
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            Save(TreeEditState.草稿);
        }

        protected void btnSaveFinal_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            Save(TreeEditState.完稿);
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(ddlCity.SelectedValue))
            {
                ShowMessage("驗證", "請選擇縣市", "warning");
                return false;
            }
            if (string.IsNullOrWhiteSpace(ddlSpecies.SelectedValue))
            {
                ShowMessage("驗證", "請選擇樹種", "warning");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSurveyDate.Text))
            {
                ShowMessage("驗證", "請填寫調查日期", "warning");
                return false;
            }
            return true;
        }

        private void Save(TreeEditState state)
        {
            int.TryParse(hfTreeID.Value, out int treeId);
            var record = treeId > 0 ? TreeService.GetTree(treeId) ?? new TreeRecord() : new TreeRecord();

            record.TreeID = treeId;
            record.AgencyTreeNo = txtAgencyTreeNo.Text.Trim();
            record.AgencyJurisdictionCode = txtJurisdiction.Text.Trim();
            record.CityID = string.IsNullOrWhiteSpace(ddlCity.SelectedValue) ? (int?)null : Convert.ToInt32(ddlCity.SelectedValue);
            record.CityName = ddlCity.SelectedItem?.Text;
            record.AreaID = string.IsNullOrWhiteSpace(ddlArea.SelectedValue) ? (int?)null : Convert.ToInt32(ddlArea.SelectedValue);
            record.AreaName = ddlArea.SelectedItem?.Text;
            record.SpeciesID = string.IsNullOrWhiteSpace(ddlSpecies.SelectedValue) ? (int?)null : Convert.ToInt32(ddlSpecies.SelectedValue);
            record.SpeciesCommonName = ddlSpecies.SelectedItem?.Text;
            record.Manager = txtManager.Text.Trim();
            record.ManagerContact = txtManagerContact.Text.Trim();
            record.SurveyDate = string.IsNullOrWhiteSpace(txtSurveyDate.Text) ? (DateTime?)null : DateTime.Parse(txtSurveyDate.Text);
            record.Surveyor = txtSurveyor.Text.Trim();
            record.AnnouncementDate = string.IsNullOrWhiteSpace(txtAnnouncementDate.Text) ? (DateTime?)null : DateTime.Parse(txtAnnouncementDate.Text);
            record.IsAnnounced = record.AnnouncementDate.HasValue;
            record.Status = string.IsNullOrWhiteSpace(ddlStatus.SelectedValue) ? TreeStatus.其他 : (TreeStatus)Convert.ToInt32(ddlStatus.SelectedValue);
            record.EditStatus = state;
            record.TreeCount = int.TryParse(txtTreeCount.Text, out int count) && count > 0 ? count : 1;
            record.Site = txtSite.Text.Trim();
            record.Latitude = decimal.TryParse(txtLatitude.Text, out decimal lat) ? lat : (decimal?)null;
            record.Longitude = decimal.TryParse(txtLongitude.Text, out decimal lng) ? lng : (decimal?)null;
            record.LandOwnership = ddlLandOwnership.SelectedValue;
            record.LandOwnershipNote = txtLandOwnershipNote.Text.Trim();
            record.FacilityDescription = txtFacility.Text.Trim();
            record.Memo = txtMemo.Text.Trim();
            record.TreeHeight = decimal.TryParse(txtTreeHeight.Text, out decimal h) ? h : (decimal?)null;
            record.BreastHeightDiameter = decimal.TryParse(txtBreastHeightDiameter.Text, out decimal d) ? d : (decimal?)null;
            record.BreastHeightCircumference = decimal.TryParse(txtBreastHeightCircumference.Text, out decimal c) ? c : (decimal?)null;
            record.CanopyProjectionArea = decimal.TryParse(txtCanopyArea.Text, out decimal area) ? area : (decimal?)null;
            record.RecognitionCriteria = cblRecognition.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList();
            record.RecognitionNote = txtRecognitionNote.Text.Trim();
            record.CulturalHistoryIntro = txtCulturalHistory.Text.Trim();
            record.HealthCondition = txtHealth.Text.Trim();

            if (state == TreeEditState.完稿 && string.IsNullOrWhiteSpace(record.SystemTreeNo))
            {
                record.SystemTreeNo = TreeService.GenerateSystemTreeNo(record.CityID, record.AreaID, record.AnnouncementDate ?? record.SurveyDate ?? DateTime.Today);
            }
            
            var user = protectTreesV2.User.UserService.GetCurrentUser();
            int accountId = user?.userID ?? 0;

            if (record.TreeID > 0)
            {
                TreeService.UpdateTree(record, accountId);
            }
            else
            {
                int newId = TreeService.InsertTree(record, accountId);
                hfTreeID.Value = newId.ToString();
                setTreeID = newId.ToString();
            }

            Response.Redirect("query.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("query.aspx");
        }
    }
}
