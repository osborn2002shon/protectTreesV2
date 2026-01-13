using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using protectTreesV2.Base;
using protectTreesV2.Health;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class detail : BasePage
    {
        private readonly Health system_health = new Health();

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
                BindHealthRecords(treeId);
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
            treePhotoAlbum.SetPhotos(photos);

            pnlAnnouncementSection.Visible = tree.Status == TreeStatus.已公告列管;
        }

        private int? SelectedHealthId
        {
            get { return ViewState["SelectedHealthId"] as int?; }
            set { ViewState["SelectedHealthId"] = value; }
        }

        private void BindHealthRecords(int treeId)
        {
            var records = system_health.GetHealthRecordsByTreeId(treeId) ?? new List<Health.TreeHealthRecord>();
            foreach (var record in records)
            {
                record.attachments = system_health.GetHealthAttachments(record.healthID);
            }

            if (!records.Any())
            {
                SelectedHealthId = null;
                phHealthRecordEmpty.Visible = true;
                rptHealthRecords.DataSource = null;
                rptHealthRecords.DataBind();
                BindHealthPhotos(null);
                return;
            }

            var selectedId = SelectedHealthId;
            if (!selectedId.HasValue || records.All(record => record.healthID != selectedId.Value))
            {
                selectedId = records.First().healthID;
            }

            SelectedHealthId = selectedId;
            phHealthRecordEmpty.Visible = false;
            rptHealthRecords.DataSource = records;
            rptHealthRecords.DataBind();
            BindHealthPhotos(selectedId);
        }

        private void BindHealthPhotos(int? healthId)
        {
            if (!healthId.HasValue)
            {
                pnlHealthPhotoGallery.Visible = false;
                pnlHealthCoverPhoto.Visible = false;
                rptHealthGallery.DataSource = null;
                rptHealthGallery.DataBind();
                lblNoHealthPhotos.Visible = false;
                lblHealthPhotoHint.Visible = true;
                return;
            }

            var photos = system_health.GetHealthPhotos(healthId.Value) ?? new List<Health.TreeHealthPhoto>();
            if (!photos.Any())
            {
                pnlHealthPhotoGallery.Visible = false;
                pnlHealthCoverPhoto.Visible = false;
                rptHealthGallery.DataSource = null;
                rptHealthGallery.DataBind();
                lblNoHealthPhotos.Visible = true;
                lblHealthPhotoHint.Visible = false;
                return;
            }

            lblNoHealthPhotos.Visible = false;
            lblHealthPhotoHint.Visible = false;
            pnlHealthPhotoGallery.Visible = true;

            var coverPhoto = photos.FirstOrDefault();
            pnlHealthCoverPhoto.Visible = coverPhoto != null;
            if (coverPhoto != null)
            {
                var coverUrl = ResolveUrl(coverPhoto.filePath);
                imgHealthCover.ImageUrl = coverUrl;
                imgHealthCover.AlternateText = BuildHealthPhotoTitle(coverPhoto);
                lnkHealthCoverLightbox.HRef = coverUrl;
                lnkHealthCoverLightbox.Attributes["data-gallery"] = "health-photos";
                lnkHealthCoverLightbox.Attributes["data-title"] = BuildHealthPhotoTitle(coverPhoto);
                lnkHealthCoverLightbox.Attributes["data-description"] = BuildHealthPhotoDescriptionAttribute(coverPhoto);
            }

            var galleryPhotos = photos
                .Where(photo => coverPhoto == null || photo.photoID != coverPhoto.photoID)
                .ToList();

            rptHealthGallery.DataSource = galleryPhotos;
            rptHealthGallery.DataBind();
        }

        protected void rptHealthRecords_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandArgument == null)
            {
                return;
            }

            if (!int.TryParse(e.CommandArgument.ToString(), out int healthId))
            {
                return;
            }

            int treeId = int.Parse(hfTreeID.Value);

            if (e.CommandName == "SelectHealth")
            {
                SelectedHealthId = healthId;
                BindHealthRecords(treeId);
                return;
            }

            if (e.CommandName == "ViewHealth")
            {
                var record = system_health.GetHealthRecord(healthId);
                if (record != null)
                {
                    lblModal_healthId.Text = record.healthID.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    lblModal_systemTreeNo.Text = string.IsNullOrWhiteSpace(record.systemTreeNo) ? "--" : record.systemTreeNo.Trim();
                    lblModal_status.Text = string.IsNullOrWhiteSpace(record.dataStatusText) ? "--" : record.dataStatusText.Trim();
                    string location = (record.cityName ?? "") + (record.areaName ?? "");
                    lblModal_location.Text = string.IsNullOrWhiteSpace(location) ? "--" : location.Trim();
                    lblModal_species.Text = string.IsNullOrWhiteSpace(record.speciesName) ? "--" : record.speciesName.Trim();
                    lblModal_lastUpdate.Text = string.IsNullOrWhiteSpace(record.lastUpdateDisplay) ? "--" : record.lastUpdateDisplay.Trim();
                }
                else
                {
                    lblModal_healthId.Text = "";
                    lblModal_systemTreeNo.Text = "";
                    lblModal_status.Text = "";
                    lblModal_location.Text = "";
                    lblModal_species.Text = "";
                    lblModal_lastUpdate.Text = "";
                }

                uc_healthRecordModal.BindRecord(record);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "showHealthRecordModal();", true);
            }
        }

        protected string FormatText(object value)
        {
            return string.IsNullOrWhiteSpace(value?.ToString()) ? "無資料" : value.ToString();
        }

        protected string FormatSurveyDate(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "無資料";
            }

            if (value is DateTime date)
            {
                return date.ToString("yyyy/MM/dd");
            }

            return DateTime.TryParse(value.ToString(), out var parsed)
                ? parsed.ToString("yyyy/MM/dd")
                : "無資料";
        }

        protected string GetHealthRecordCardCss(object dataItem)
        {
            var record = dataItem as Health.TreeHealthRecord;
            var isActive = record != null && SelectedHealthId.HasValue && record.healthID == SelectedHealthId.Value;
            return isActive ? "card mb-3 health-record-card is-active" : "card mb-3 health-record-card";
        }

        protected string GetAttachmentUrl(object dataItem)
        {
            var record = dataItem as Health.TreeHealthRecord;
            var attachment = record?.attachments?.FirstOrDefault();
            return string.IsNullOrWhiteSpace(attachment?.filePath) ? "javascript:void(0);" : ResolveUrl(attachment.filePath);
        }

        protected string GetAttachmentButtonCss(object dataItem)
        {
            var record = dataItem as Health.TreeHealthRecord;
            var attachment = record?.attachments?.FirstOrDefault();
            return string.IsNullOrWhiteSpace(attachment?.filePath)
                ? "btn btn-sm btn-outline-secondary disabled"
                : "btn btn-sm btn-outline-secondary";
        }

        protected string ResolveHealthPhotoUrl(object dataItem)
        {
            var photo = dataItem as Health.TreeHealthPhoto;
            return string.IsNullOrWhiteSpace(photo?.filePath) ? "#" : ResolveUrl(photo.filePath);
        }

        protected string BuildHealthPhotoTitle(Health.TreeHealthPhoto photo)
        {
            if (photo == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(photo.caption))
            {
                return photo.caption;
            }
            return string.IsNullOrWhiteSpace(photo.fileName) ? "未命名照片" : photo.fileName;
        }

        protected string BuildHealthPhotoDescriptionAttribute(Health.TreeHealthPhoto photo)
        {
            if (photo == null) return string.Empty;
            var title = BuildHealthPhotoTitle(photo);
            var uploadTime = photo.insertDateTime == default ? "未知時間" : photo.insertDateTime.ToString("yyyy/MM/dd HH:mm");
            return HttpUtility.HtmlAttributeEncode($"{title}｜上傳：{uploadTime}");
        }

        protected string BuildHealthPhotoTitleFromData(object dataItem)
        {
            return BuildHealthPhotoTitle(dataItem as Health.TreeHealthPhoto);
        }

        protected string BuildHealthPhotoDescriptionFromData(object dataItem)
        {
            return BuildHealthPhotoDescriptionAttribute(dataItem as Health.TreeHealthPhoto);
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
    }
}
