using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using protectTreesV2.Care;

namespace protectTreesV2._uc.care
{
    public partial class uc_careRecordModal : System.Web.UI.UserControl
    {
        private const string EmptyImage = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";

        public void BindRecord(protectTreesV2.Care.Care.CareRecord record, List<Care.Care.CarePhotoRecord> photos)
        {
            if (record == null)
            {
                ClearContent();
                phContent.Visible = false;
                phEmpty.Visible = true;
                return;
            }

            phContent.Visible = true;
            phEmpty.Visible = false;

            litCareId.Text = record.careID.ToString(CultureInfo.InvariantCulture);
            litStatus.Text = GetStatusText(record.dataStatus);
            litCareDate.Text = FormatDate(record.careDate);
            litRecorder.Text = FormatText(record.recorder);
            litReviewer.Text = FormatText(record.reviewer);
            litLastUpdate.Text = FormatDateTime(record.updateDateTime ?? record.insertDateTime);

            litCrownSummary.Text = BuildCrownSummary(record);
            litTrunkSummary.Text = BuildTrunkSummary(record);
            litRootSummary.Text = BuildRootSummary(record);
            litEnvSummary.Text = BuildEnvSummary(record);
            litAdjacentSummary.Text = BuildAdjacentSummary(record);

            litTask1Summary.Text = BuildTaskSummary(record.task1Status, record.task1Note);
            litTask2Summary.Text = BuildTaskSummary(record.task2Status, record.task2Note);
            litTask3Summary.Text = BuildTaskSummary(record.task3Status, record.task3Note);
            litTask4Summary.Text = BuildTaskSummary(record.task4Status, record.task4Note);
            litTask5Summary.Text = BuildTaskSummary(record.task5Status, record.task5Note);

            BindPhotos(photos);
        }

        private void BindPhotos(List<Care.Care.CarePhotoRecord> photos)
        {
            if (photos == null || photos.Count == 0)
            {
                rptPhotos.DataSource = null;
                rptPhotos.DataBind();
                phPhotoEmpty.Visible = true;
                return;
            }

            phPhotoEmpty.Visible = false;
            rptPhotos.DataSource = photos;
            rptPhotos.DataBind();
        }

        private void ClearContent()
        {
            litCareId.Text = string.Empty;
            litStatus.Text = string.Empty;
            litCareDate.Text = string.Empty;
            litRecorder.Text = string.Empty;
            litReviewer.Text = string.Empty;
            litLastUpdate.Text = string.Empty;

            litCrownSummary.Text = string.Empty;
            litTrunkSummary.Text = string.Empty;
            litRootSummary.Text = string.Empty;
            litEnvSummary.Text = string.Empty;
            litAdjacentSummary.Text = string.Empty;

            litTask1Summary.Text = string.Empty;
            litTask2Summary.Text = string.Empty;
            litTask3Summary.Text = string.Empty;
            litTask4Summary.Text = string.Empty;
            litTask5Summary.Text = string.Empty;

            BindPhotos(null);
        }

        protected string ResolvePhotoUrl(object pathObj)
        {
            var path = pathObj as string;
            return string.IsNullOrWhiteSpace(path) ? "#" : ResolveUrl(path);
        }

        protected string ResolvePhotoPreview(object pathObj)
        {
            var path = pathObj as string;
            return string.IsNullOrWhiteSpace(path) ? EmptyImage : ResolveUrl(path);
        }

        protected string FormatText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "--" : value.Trim();
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) : "--";
        }

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture) : "--";
        }

        private static string GetStatusText(int status)
        {
            return status == (int)Care.Care.CareRecordStatus.定稿 ? "定稿" : "草稿";
        }

        private static string BuildCrownSummary(protectTreesV2.Care.Care.CareRecord record)
        {
            var detail = new List<string>();
            if (record.crownSeasonalDormant.GetValueOrDefault()) detail.Add("季節性休眠落葉");

            if (record.crownDeadBranch.GetValueOrDefault())
            {
                if (record.crownDeadBranchPercent.HasValue)
                {
                    detail.Add($"有枯枝(現存枝葉量：{record.crownDeadBranchPercent.Value.ToString("0.##", CultureInfo.InvariantCulture)}%)");
                }
                else
                {
                    detail.Add("有枯枝");
                }
            }

            if (record.crownPest.GetValueOrDefault()) detail.Add("有明顯病蟲害(葉部有明顯蟲體或病徵)");
            if (record.crownForeignObject.GetValueOrDefault()) detail.Add("樹冠接觸電線或異物");

            return BuildStatusSummary(GetCrownStatusText(record.crownStatus), detail, record.crownOtherNote);
        }

        private static string BuildTrunkSummary(protectTreesV2.Care.Care.CareRecord record)
        {
            var detail = new List<string>();
            if (record.trunkBarkDamage.GetValueOrDefault()) detail.Add("樹皮破損");
            if (record.trunkDecay.GetValueOrDefault()) detail.Add("莖幹損傷(腐朽中空或膨大)");
            if (record.trunkTermiteTrail.GetValueOrDefault()) detail.Add("有白蟻蟻道");
            if (record.trunkLean.GetValueOrDefault()) detail.Add("主莖傾斜搖晃");
            if (record.trunkFungus.GetValueOrDefault()) detail.Add("莖基部有真菌子實體(如靈芝)");
            if (record.trunkGummosis.GetValueOrDefault()) detail.Add("有流膠或潰瘍");
            if (record.trunkVine.GetValueOrDefault()) detail.Add("有纏勒植物(如雀榕或小花蔓澤蘭)");

            return BuildStatusSummary(GetTrunkStatusText(record.trunkStatus), detail, record.trunkOtherNote);
        }

        private static string BuildRootSummary(protectTreesV2.Care.Care.CareRecord record)
        {
            var detail = new List<string>();
            if (record.rootDamage.GetValueOrDefault()) detail.Add("根部損傷");
            if (record.rootDecay.GetValueOrDefault()) detail.Add("根部有腐朽或可見子實體");
            if (record.rootExpose.GetValueOrDefault()) detail.Add("盤根或浮根");
            if (record.rootRot.GetValueOrDefault()) detail.Add("根部潰爛");
            if (record.rootSucker.GetValueOrDefault()) detail.Add("大量萌櫱(不定芽)");

            return BuildStatusSummary(GetRootStatusText(record.rootStatus), detail, record.rootOtherNote);
        }

        private static string BuildEnvSummary(protectTreesV2.Care.Care.CareRecord record)
        {
            var detail = new List<string>();
            if (record.envPitSmall.GetValueOrDefault()) detail.Add("樹穴過小");
            if (record.envPaved.GetValueOrDefault()) detail.Add("遭鋪面封固(如柏油、混凝土、磚瓦等)");
            if (record.envDebris.GetValueOrDefault()) detail.Add("有石塊或廢棄物推積");
            if (record.envSoilCover.GetValueOrDefault()) detail.Add("根領覆土過高");
            if (record.envCompaction.GetValueOrDefault()) detail.Add("土壤壓實");
            if (record.envWaterlog.GetValueOrDefault()) detail.Add("環境積水");
            if (record.envNearFacility.GetValueOrDefault()) detail.Add("緊鄰設施或建物");

            return BuildStatusSummary(GetEnvStatusText(record.envStatus), detail, record.envOtherNote);
        }

        private static string BuildAdjacentSummary(protectTreesV2.Care.Care.CareRecord record)
        {
            var detail = new List<string>();
            if (record.adjacentBuilding.GetValueOrDefault()) detail.Add("接觸建築物");
            if (record.adjacentWire.GetValueOrDefault()) detail.Add("接觸電線或管線");
            if (record.adjacentSignal.GetValueOrDefault()) detail.Add("遮蔽路燈或號誌");

            return BuildStatusSummary(GetAdjacentStatusText(record.adjacentStatus), detail, record.adjacentOtherNote);
        }

        private static string BuildTaskSummary(int? status, string note)
        {
            var statusText = status.GetValueOrDefault() == 1 ? "處理方式" : "無須處理";
            if (!string.IsNullOrWhiteSpace(note))
            {
                return $"{statusText}；{note.Trim()}";
            }

            return statusText;
        }

        private static string BuildStatusSummary(string statusText, List<string> details, string otherNote)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(statusText)) parts.Add(statusText);

            if (details != null && details.Any())
            {
                parts.Add(string.Join("、", details));
            }

            if (!string.IsNullOrWhiteSpace(otherNote))
            {
                parts.Add($"其他：{otherNote.Trim()}");
            }

            return parts.Count == 0 ? "--" : string.Join("；", parts);
        }

        private static string GetCrownStatusText(int? status)
        {
            if (!status.HasValue) return string.Empty;
            return status.Value == 1 ? "枝葉茂密無枯枝" : "有其他異狀";
        }

        private static string GetTrunkStatusText(int? status)
        {
            if (!status.HasValue) return string.Empty;
            return status.Value == 1 ? "完好健康無異狀" : "有其他異狀";
        }

        private static string GetRootStatusText(int? status)
        {
            if (!status.HasValue) return string.Empty;
            return status.Value == 1 ? "根部完好無異狀" : "有其他異狀";
        }

        private static string GetEnvStatusText(int? status)
        {
            if (!status.HasValue) return string.Empty;
            return status.Value == 1 ? "良好無異狀" : "有其他異狀";
        }

        private static string GetAdjacentStatusText(int? status)
        {
            if (!status.HasValue) return string.Empty;
            return status.Value == 1 ? "無鄰接物" : "有其他異狀";
        }
    }
}
