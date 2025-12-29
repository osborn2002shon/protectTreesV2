using System;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
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

            lblSystemTreeNo.Text = DisplayOrDefault(tree.SystemTreeNo);
            lblAgencyTreeNo.Text = DisplayOrDefault(tree.AgencyTreeNo);
            lblJurisdiction.Text = DisplayOrDefault(tree.AgencyJurisdictionCode);
            lblCity.Text = DisplayOrDefault(tree.CityName);
            lblArea.Text = DisplayOrDefault(tree.AreaName);
            lblSpecies.Text = DisplayOrDefault(tree.SpeciesDisplayName);
            lblStatus.Text = DisplayOrDefault(tree.StatusText);
            lblEditStatus.Text = DisplayOrDefault(tree.EditStatusText);
            lblSurveyDate.Text = DisplayOrDefault(tree.SurveyDate);
            lblAnnouncementDate.Text = DisplayOrDefault(tree.AnnouncementDate);
            lblManager.Text = DisplayOrDefault(tree.Manager);
            lblManagerContact.Text = DisplayOrDefault(tree.ManagerContact);
            lblSite.Text = DisplayOrDefault(tree.Site);
            lblLandOwnership.Text = DisplayOrDefault(tree.LandOwnership);
            lblLandOwnershipNote.Text = DisplayOrDefault(tree.LandOwnershipNote);
            lblFacility.Text = DisplayOrDefault(tree.FacilityDescription);
            lblTreeCount.Text = DisplayOrDefault((int?)tree.TreeCount);
            lblTreeHeight.Text = DisplayOrDefault(tree.TreeHeight);
            lblBreastHeightDiameter.Text = DisplayOrDefault(tree.BreastHeightDiameter);
            lblBreastHeightCircumference.Text = DisplayOrDefault(tree.BreastHeightCircumference);
            lblCanopyArea.Text = DisplayOrDefault(tree.CanopyProjectionArea);
            lblLatitude.Text = DisplayOrDefault(tree.Latitude);
            lblLongitude.Text = DisplayOrDefault(tree.Longitude);
            lblSurveyor.Text = DisplayOrDefault(tree.Surveyor);
            lblEstimatedPlantingYear.Text = DisplayOrDefault(tree.EstimatedPlantingYear);
            lblEstimatedAgeNote.Text = DisplayOrDefault(tree.EstimatedAgeNote);
            lblGroupGrowthInfo.Text = DisplayOrDefault(tree.GroupGrowthInfo);
            lblEpiphyteDescription.Text = DisplayOrDefault(tree.EpiphyteDescription);
            lblParasiteDescription.Text = DisplayOrDefault(tree.ParasiteDescription);
            lblClimbingPlantDescription.Text = DisplayOrDefault(tree.ClimbingPlantDescription);
            var criteriaLookup = TreeService.GetRecognitionCriteria()
                .GroupBy(c => c.Code)
                .ToDictionary(g => g.Key, g => g.First().Name);

            var recognitionNames = (tree.RecognitionCriteria ?? Enumerable.Empty<string>())
                .Select(code => criteriaLookup.TryGetValue(code, out var name) ? name : null)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();

            string recognitionText = string.Empty;
            if (recognitionNames.Any())
            {
                var htmlReadyNames = recognitionNames
                    .Select(name => Server.HtmlEncode(name).Replace("\n", "<br />"));

                recognitionText = string.Join("<br />", htmlReadyNames);
            }
            else
            {
                recognitionText = Server.HtmlEncode(string.Join(",", tree.RecognitionCriteria ?? Enumerable.Empty<string>()));
            }

            ltlRecognition.Text = DisplayOrDefault(recognitionText);
            lblRecognitionNote.Text = DisplayOrDefault(tree.RecognitionNote);
            lblCulturalHistory.Text = DisplayOrDefault(tree.CulturalHistoryIntro);
            lblHealth.Text = DisplayOrDefault(tree.HealthCondition);
            lblMemo.Text = DisplayOrDefault(tree.Memo);

            var photos = TreeService.GetPhotos(treeId)?.ToList() ?? Enumerable.Empty<TreePhoto>().ToList();
            BindPhotoGallery(photos);

            pnlAnnouncementSection.Visible = tree.Status == TreeStatus.已公告列管;
        }

        private void BindPhotoGallery(System.Collections.Generic.List<TreePhoto> photos)
        {
            if (photos == null || !photos.Any())
            {
                pnlPhotoGallery.Visible = false;
                lblNoPhotos.Visible = true;
                return;
            }

            var coverPhoto = photos.FirstOrDefault(p => p.IsCover) ?? photos.FirstOrDefault();

            if (coverPhoto != null)
            {
                imgCover.ImageUrl = coverPhoto.FilePath;
                imgCover.AlternateText = BuildLightboxTitle(coverPhoto);
                lblCoverCaption.Text = BuildPhotoCaption(coverPhoto);
                lblCoverUploadTime.Text = $"上傳：{BuildUploadTimeDisplay(coverPhoto)}";

                lnkCoverLightbox.HRef = coverPhoto.FilePath;
                lnkCoverLightbox.Attributes["data-gallery"] = "tree-photos";
                lnkCoverLightbox.Attributes["data-title"] = BuildLightboxTitle(coverPhoto);
                lnkCoverLightbox.Attributes["data-description"] = BuildLightboxDescriptionAttribute(coverPhoto);
            }

            rptGallery.DataSource = photos;
            rptGallery.DataBind();

            pnlPhotoGallery.Visible = true;
            lblNoPhotos.Visible = false;
        }

        private string DisplayOrDefault(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "無資料" : value;
        }

        private string DisplayOrDefault(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy/MM/dd") : "無資料";
        }

        private string DisplayOrDefault(decimal? value)
        {
            return value.HasValue ? value.Value.ToString() : "無資料";
        }

        private string DisplayOrDefault(int? value)
        {
            return value.HasValue ? value.Value.ToString() : "無資料";
        }

        protected string BuildPhotoCaption(TreePhoto photo)
        {
            if (photo == null) return "未命名照片";
            return string.IsNullOrWhiteSpace(photo.Caption) ? "未命名照片" : photo.Caption;
        }

        protected string BuildUploadTimeDisplay(TreePhoto photo)
        {
            if (photo == null || photo.InsertDateTime == default)
            {
                return "未知時間";
            }

            return photo.InsertDateTime.ToString("yyyy/MM/dd HH:mm");
        }

        protected string BuildLightboxTitle(TreePhoto photo)
        {
            if (photo == null) return string.Empty;
            var caption = BuildPhotoCaption(photo);
            return photo.IsCover ? $"{caption}（封面）" : caption;
        }

        protected string BuildLightboxDescriptionAttribute(TreePhoto photo)
        {
            if (photo == null) return string.Empty;
            var caption = BuildPhotoCaption(photo);
            var uploadTime = BuildUploadTimeDisplay(photo);
            var description = $"{caption}｜上傳：{uploadTime}";
            return HttpUtility.HtmlAttributeEncode(description);
        }
    }
}
