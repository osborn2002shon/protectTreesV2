using System;
using System.Linq;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class detail : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int treeId;
                if (!int.TryParse(setTreeID, out treeId) || treeId <= 0)
                {
                    Response.Redirect("query.aspx");
                    return;
                }

                hfTreeID.Value = treeId.ToString();
                BindData();
            }
        }

        private void BindData()
        {
            int treeId = int.Parse(hfTreeID.Value);
            var tree = TreeService.GetTree(treeId);
            if (tree == null)
            {
                Response.Redirect("query.aspx");
                return;
            }

            lblSystemTreeNo.Text = tree.SystemTreeNo;
            lblAgencyTreeNo.Text = tree.AgencyTreeNo;
            lblJurisdiction.Text = tree.AgencyJurisdictionCode;
            lblCity.Text = tree.CityName;
            lblArea.Text = tree.AreaName;
            lblSpecies.Text = tree.SpeciesDisplayName;
            lblStatus.Text = tree.StatusText;
            lblEditStatus.Text = tree.EditStatusText;
            lblSurveyDate.Text = tree.SurveyDate?.ToString("yyyy/MM/dd");
            lblAnnouncementDate.Text = tree.AnnouncementDate?.ToString("yyyy/MM/dd");
            lblManager.Text = tree.Manager;
            lblManagerContact.Text = tree.ManagerContact;
            lblSite.Text = tree.Site;
            lblLandOwnership.Text = tree.LandOwnership;
            lblLandOwnershipNote.Text = tree.LandOwnershipNote;
            lblTreeCount.Text = tree.TreeCount.ToString();
            lblTreeHeight.Text = tree.TreeHeight?.ToString();
            lblBreastHeightDiameter.Text = tree.BreastHeightDiameter?.ToString();
            lblBreastHeightCircumference.Text = tree.BreastHeightCircumference?.ToString();
            lblCanopyArea.Text = tree.CanopyProjectionArea?.ToString();
            lblLatitude.Text = tree.Latitude?.ToString();
            lblLongitude.Text = tree.Longitude?.ToString();
            lblIsAnnounced.Text = tree.IsAnnounced ? "是" : "否";
            lblRecognition.Text = string.Join(",", tree.RecognitionCriteria ?? Enumerable.Empty<string>());
            lblRecognitionNote.Text = tree.RecognitionNote;
            lblCulturalHistory.Text = tree.CulturalHistoryIntro;
            lblHealth.Text = tree.HealthCondition;
            lblMemo.Text = tree.Memo;

            rptPhotos.DataSource = TreeService.GetPhotos(treeId);
            rptPhotos.DataBind();
        }
    }
}
