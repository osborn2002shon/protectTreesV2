using System;
using System.Collections.Generic;
using System.Linq;
using protectTreesV2.Base;
using protectTreesV2.Health;
using protectTreesV2.Patrol;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class detail : BasePage
    {
        private readonly Health.Health systemHealth = new Health.Health();
        private readonly Patrol.Patrol systemPatrol = new Patrol.Patrol();

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

        private class PatrolRecordCardViewModel
        {
            public int PatrolId { get; set; }
            public string PatrolDateDisplay { get; set; }
            public string PatrollerDisplay { get; set; }
            public string RiskDisplay { get; set; }
            public string MemoDisplay { get; set; }
            public string StatusDisplay { get; set; }
            public bool IsSelected { get; set; }
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
            BindPatrolRecords(treeId);
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

        private void BindPatrolRecords(int treeId)
        {
            var records = systemPatrol.GetPatrolRecordsByTree(treeId) ?? new List<Patrol.Patrol.PatrolRecord>();

            if (!records.Any())
            {
                pnlPatrolRecordEmpty.Visible = true;
                rptPatrolRecords.Visible = false;
                BindPatrolPhotos(null);
                return;
            }

            int? selectedPatrolId = GetSelectedPatrolId(records);

            var viewModels = records.Select(record => new PatrolRecordCardViewModel
            {
                PatrolId = record.patrolID,
                PatrolDateDisplay = DisplayOrDefault(record.patrolDate),
                PatrollerDisplay = DisplayOrDefault(record.patroller),
                RiskDisplay = record.hasPublicSafetyRisk ? "有" : "無",
                MemoDisplay = DisplayOrDefault(record.memo),
                StatusDisplay = record.dataStatus == (int)Patrol.PatrolRecordStatus.定稿 ? "定稿" : "草稿",
                IsSelected = selectedPatrolId.HasValue && record.patrolID == selectedPatrolId.Value
            }).ToList();

            rptPatrolRecords.DataSource = viewModels;
            rptPatrolRecords.DataBind();
            rptPatrolRecords.Visible = true;
            pnlPatrolRecordEmpty.Visible = false;

            BindPatrolPhotos(selectedPatrolId);
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

        private int? GetSelectedPatrolId(IEnumerable<Patrol.Patrol.PatrolRecord> records)
        {
            if (int.TryParse(hfSelectedPatrolId.Value, out int selectedId) && records.Any(r => r.patrolID == selectedId))
            {
                return selectedId;
            }

            int? fallbackId = records.FirstOrDefault()?.patrolID;
            hfSelectedPatrolId.Value = fallbackId?.ToString() ?? string.Empty;
            return fallbackId;
        }

        private void BindHealthPhotos(int? healthId)
        {
            var photos = healthId.HasValue ? systemHealth.GetHealthPhotos(healthId.Value) : new List<Health.Health.TreeHealthPhoto>();
            healthPhotoAlbum.GalleryName = "health-photos";
            healthPhotoAlbum.SetPhotos(MapHealthPhotosToTreePhotos(photos));
        }

        private void BindPatrolPhotos(int? patrolId)
        {
            var photos = patrolId.HasValue ? systemPatrol.GetPatrolPhotos(patrolId.Value) : new List<Patrol.Patrol.PatrolPhoto>();
            patrolPhotoAlbum.GalleryName = "patrol-photos";
            patrolPhotoAlbum.SetPhotos(MapPatrolPhotosToTreePhotos(photos));
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

        private IEnumerable<TreePhoto> MapPatrolPhotosToTreePhotos(IEnumerable<Patrol.Patrol.PatrolPhoto> photos)
        {
            var photoList = photos?.ToList() ?? new List<Patrol.Patrol.PatrolPhoto>();
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
                    PhotoID = photo.PhotoID,
                    FileName = photo.FileName,
                    FilePath = ResolveUrl(photo.FilePath),
                    Caption = photo.Caption,
                    InsertDateTime = DateTime.MinValue,
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

        protected void rptPatrolRecords_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument?.ToString(), out int patrolId))
            {
                return;
            }

            int treeId = int.Parse(hfTreeID.Value);

            if (e.CommandName == "SelectPatrol")
            {
                hfSelectedPatrolId.Value = patrolId.ToString();
                BindPatrolRecords(treeId);
                ActivatePatrolTab();
                return;
            }

            if (e.CommandName == "ViewReport")
            {
                ShowPatrolRecordModal(patrolId);
                ActivatePatrolTab();
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

        protected void rptPatrolRecords_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                return;
            }

            var viewModel = e.Item.DataItem as PatrolRecordCardViewModel;
            if (viewModel == null)
            {
                return;
            }

            var card = e.Item.FindControl("pnlPatrolCard") as System.Web.UI.WebControls.Panel;
            if (card != null)
            {
                card.Attributes["data-select-target"] = (e.Item.FindControl("btnSelectPatrol") as System.Web.UI.WebControls.LinkButton)?.ClientID ?? string.Empty;
                if (viewModel.IsSelected)
                {
                    card.CssClass = $"{card.CssClass} is-selected";
                }
            }

            var selectionHint = e.Item.FindControl("lblPatrolSelectionHint") as System.Web.UI.WebControls.Label;
            if (selectionHint != null && viewModel.IsSelected)
            {
                selectionHint.Text = "顯示照片中";
                selectionHint.CssClass = "text-success fw-semibold small";
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

            SetRecordModalMode(isHealth: true);
            uc_healthRecordModal.BindRecord(record);
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowRecordModal", "showRecordModal();", true);
        }

        private void ShowPatrolRecordModal(int patrolId)
        {
            var record = systemPatrol.GetPatrolRecord(patrolId);

            if (record != null)
            {
                var tree = TreeService.GetTree(record.treeID);
                lblModal_healthId.Text = record.patrolID.ToString(System.Globalization.CultureInfo.InvariantCulture);
                lblModal_systemTreeNo.Text = string.IsNullOrWhiteSpace(tree?.SystemTreeNo) ? "--" : tree.SystemTreeNo.Trim();
                lblModal_status.Text = record.dataStatus == (int)Patrol.PatrolRecordStatus.定稿 ? "定稿" : "草稿";

                string location = (tree?.CityName ?? "") + (tree?.AreaName ?? "");
                lblModal_location.Text = string.IsNullOrWhiteSpace(location) ? "--" : location.Trim();
                lblModal_species.Text = string.IsNullOrWhiteSpace(tree?.SpeciesDisplayName) ? "--" : tree.SpeciesDisplayName.Trim();

                DateTime? lastUpdate = record.updateDateTime ?? record.insertDateTime;
                lblModal_lastUpdate.Text = DisplayOrDefault(lastUpdate);
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

            var photos = record != null ? systemPatrol.GetPatrolPhotos(patrolId) : new List<Patrol.Patrol.PatrolPhoto>();
            SetRecordModalMode(isHealth: false);
            uc_patrolRecordModal.BindRecord(record, photos);
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowRecordModal", "showRecordModal();", true);
        }

        private void SetRecordModalMode(bool isHealth)
        {
            phHealthRecordModal.Visible = isHealth;
            phPatrolRecordModal.Visible = !isHealth;
            ltlRecordModalTitle.Text = isHealth ? "健檢紀錄" : "巡查紀錄";
        }

        private void ActivateHealthTab()
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowHealthTab", "showHealthTab();", true);
        }

        private void ActivatePatrolTab()
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPatrolTab", "showPatrolTab();", true);
        }

    }
}
