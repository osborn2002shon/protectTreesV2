using System;
using System.Collections.Generic;
using System.Linq;
using protectTreesV2.Base;
using protectTreesV2.Health;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class detail : BasePage
    {
        private readonly Health.Health systemHealth = new Health.Health();

        private class HealthRecordCardViewModel
        {
            public int HealthId { get; set; }
            public string SurveyDateDisplay { get; set; }
            public string SurveyorDisplay { get; set; }
            public string ManagementStatusDisplay { get; set; }
            public string PriorityDisplay { get; set; }
            public string TreatmentDescriptionDisplay { get; set; }
            public bool IsSelected { get; set; }
            public IList<Health.Health.TreeHealthAttachment> Attachments { get; set; }
        }

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
            treePhotoAlbum.SetPhotos(photos);

            pnlAnnouncementSection.Visible = tree.Status == TreeStatus.已公告列管;

            BindHealthRecords(treeId);
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

        private void BindHealthRecords(int treeId)
        {
            var records = systemHealth.GetHealthRecordsByTree(treeId) ?? new List<Health.Health.TreeHealthRecord>();

            if (!records.Any())
            {
                pnlHealthRecordEmpty.Visible = true;
                rptHealthRecords.Visible = false;
                BindHealthPhotos(null);
                return;
            }

            foreach (var record in records)
            {
                record.attachments = systemHealth.GetHealthAttachments(record.healthID);
            }

            int? selectedHealthId = GetSelectedHealthId(records);

            var viewModels = records.Select(record => new HealthRecordCardViewModel
            {
                HealthId = record.healthID,
                SurveyDateDisplay = DisplayOrDefault(record.surveyDateDisplay),
                SurveyorDisplay = DisplayOrDefault(record.surveyor),
                ManagementStatusDisplay = DisplayOrDefault(record.managementStatus),
                PriorityDisplay = DisplayOrDefault(record.priority),
                TreatmentDescriptionDisplay = DisplayOrDefault(record.treatmentDescription),
                IsSelected = selectedHealthId.HasValue && record.healthID == selectedHealthId.Value,
                Attachments = record.attachments ?? new List<Health.Health.TreeHealthAttachment>()
            }).ToList();

            rptHealthRecords.DataSource = viewModels;
            rptHealthRecords.DataBind();
            rptHealthRecords.Visible = true;
            pnlHealthRecordEmpty.Visible = false;

            BindHealthPhotos(selectedHealthId);
        }

        private int? GetSelectedHealthId(IEnumerable<Health.Health.TreeHealthRecord> records)
        {
            if (int.TryParse(hfSelectedHealthId.Value, out int selectedId) && records.Any(r => r.healthID == selectedId))
            {
                return selectedId;
            }

            int? fallbackId = records.FirstOrDefault()?.healthID;
            hfSelectedHealthId.Value = fallbackId?.ToString() ?? string.Empty;
            return fallbackId;
        }

        private void BindHealthPhotos(int? healthId)
        {
            var photos = healthId.HasValue ? systemHealth.GetHealthPhotos(healthId.Value) : new List<Health.Health.TreeHealthPhoto>();
            healthPhotoAlbum.GalleryName = "health-photos";
            healthPhotoAlbum.SetPhotos(MapHealthPhotosToTreePhotos(photos));
        }

        private IEnumerable<TreePhoto> MapHealthPhotosToTreePhotos(IEnumerable<Health.Health.TreeHealthPhoto> photos)
        {
            var photoList = photos?.ToList() ?? new List<Health.Health.TreeHealthPhoto>();
            var mappedPhotos = new List<TreePhoto>();

            for (int i = 0; i < photoList.Count; i++)
            {
                var photo = photoList[i];
                if (photo == null)
                {
                    continue;
                }

                mappedPhotos.Add(new TreePhoto
                {
                    PhotoID = photo.photoID,
                    FileName = photo.fileName,
                    FilePath = ResolveUrl(photo.filePath),
                    Caption = photo.caption,
                    InsertDateTime = photo.insertDateTime,
                    IsCover = i == 0
                });
            }

            return mappedPhotos;
        }

        protected void rptHealthRecords_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument?.ToString(), out int healthId))
            {
                return;
            }

            int treeId = int.Parse(hfTreeID.Value);

            if (e.CommandName == "SelectHealth")
            {
                hfSelectedHealthId.Value = healthId.ToString();
                BindHealthRecords(treeId);
                ActivateHealthTab();
                return;
            }

            if (e.CommandName == "ViewReport")
            {
                ShowHealthRecordModal(healthId);
                ActivateHealthTab();
            }
        }

        protected void rptHealthRecords_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                return;
            }

            var viewModel = e.Item.DataItem as HealthRecordCardViewModel;
            if (viewModel == null)
            {
                return;
            }

            var card = e.Item.FindControl("pnlHealthCard") as System.Web.UI.WebControls.Panel;
            if (card != null)
            {
                card.Attributes["data-select-target"] = (e.Item.FindControl("btnSelectHealth") as System.Web.UI.WebControls.LinkButton)?.ClientID ?? string.Empty;
                if (viewModel.IsSelected)
                {
                    card.CssClass = $"{card.CssClass} is-selected";
                }
            }

            var selectionHint = e.Item.FindControl("lblHealthSelectionHint") as System.Web.UI.WebControls.Label;
            if (selectionHint != null && viewModel.IsSelected)
            {
                selectionHint.Text = "顯示照片中";
                selectionHint.CssClass = "text-success fw-semibold small";
            }

            var attachmentToggle = e.Item.FindControl("btnAttachmentToggle") as System.Web.UI.HtmlControls.HtmlButton;
            var attachmentRepeater = e.Item.FindControl("rptHealthAttachments") as System.Web.UI.WebControls.Repeater;
            var attachments = viewModel.Attachments ?? new List<Health.Health.TreeHealthAttachment>();

            if (attachmentRepeater != null)
            {
                attachmentRepeater.DataSource = attachments;
                attachmentRepeater.DataBind();
            }

            if (attachmentToggle != null && !attachments.Any())
            {
                attachmentToggle.Attributes["disabled"] = "disabled";
                attachmentToggle.Attributes["aria-disabled"] = "true";
                attachmentToggle.InnerText = "無附件";
            }
        }

        private void ShowHealthRecordModal(int healthId)
        {
            var record = systemHealth.GetHealthRecord(healthId);

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
                lblModal_healthId.Text = string.Empty;
                lblModal_systemTreeNo.Text = string.Empty;
                lblModal_status.Text = string.Empty;
                lblModal_location.Text = string.Empty;
                lblModal_species.Text = string.Empty;
                lblModal_lastUpdate.Text = string.Empty;
            }

            uc_healthRecordModal.BindRecord(record);
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowRecordModal", "showRecordModal();", true);
        }

        private void ActivateHealthTab()
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowHealthTab", "showHealthTab();", true);
        }

    }
}
