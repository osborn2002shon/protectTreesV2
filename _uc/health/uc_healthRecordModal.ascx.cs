using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using static protectTreesV2.Health.Health;

namespace protectTreesV2._uc.health
{
    public partial class uc_healthRecordModal : System.Web.UI.UserControl
    {
        public void BindRecord(TreeHealthRecord record)
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

            // 1. 基本資料
            litHealthId.Text = record.healthID.ToString(CultureInfo.InvariantCulture);
            litSystemTreeNo.Text = FormatText(record.systemTreeNo);
            litLocation.Text = FormatText(CombineLocation(record.cityName, record.areaName));
            litSpecies.Text = FormatText(record.speciesName);
            litSurveyDate.Text = FormatText(record.surveyDateDisplay);
            litSurveyor.Text = FormatText(record.surveyor);
            litStatus.Text = FormatText(record.dataStatusText);
            litTreeSign.Text = FormatText(GetTreeSignStatusText(record.treeSignStatus));
            litLastUpdate.Text = FormatText(record.lastUpdateDisplay);
            litMemo.Text = FormatText(record.memo);

            // 2. 座標與規格
            litLatitude.Text = FormatCoordinate(record.latitude);
            litLongitude.Text = FormatCoordinate(record.longitude);
            litTreeHeight.Text = FormatNumber(record.treeHeight);
            litCanopyArea.Text = FormatNumber(record.canopyArea);

            litGirth100.Text = FormatText(record.girth100);
            litDiameter100.Text = FormatText(record.diameter100);
            litGirth130.Text = FormatText(record.girth130);
            litDiameter130.Text = FormatText(record.diameter130);
            litMeasureNote.Text = FormatText(record.measureNote);

            // 3. 病蟲害摘要
            litMajorDisease.Text = FormatSummaryHtml(BuildMajorDiseaseSummary(record));
            litMajorPest.Text = FormatSummaryHtml(BuildMajorPestSummary(record));

            litGeneralPest.Text = FormatSummaryHtml(BuildGeneralIssueSummary(
                ("根系", record.generalPestRoot),
                ("樹基部", record.generalPestBase),
                ("主幹", record.generalPestTrunk),
                ("枝幹", record.generalPestBranch),
                ("樹冠", record.generalPestCrown),
                ("其他", record.generalPestOther)));

            litGeneralDisease.Text = FormatSummaryHtml(BuildGeneralIssueSummary(
                ("根系", record.generalDiseaseRoot),
                ("樹基部", record.generalDiseaseBase),
                ("主幹", record.generalDiseaseTrunk),
                ("枝幹", record.generalDiseaseBranch),
                ("樹冠", record.generalDiseaseCrown),
                ("其他", record.generalDiseaseOther)));

            litPestOtherNote.Text = FormatText(record.pestOtherNote);

            // 4. 結構細節 (根、基、幹、枝)
            litRootStatus.Text = FormatSummaryHtml(BuildStructureSummary(
                record.rootDecayPercent,
                record.rootCavityMaxDiameter,
                record.rootWoundMaxDiameter,
                new (bool?, string)[]
                {
            (record.rootMechanicalDamage, "機械傷害"),
            (record.rootMowingInjury, "割草或修剪傷害"),
            (record.rootInjury, "傷痕"),
            (record.rootGirdling, "纏繞")
                },
                record.rootOtherNote));

            litBaseStatus.Text = FormatSummaryHtml(BuildStructureSummary(
                record.baseDecayPercent,
                record.baseCavityMaxDiameter,
                record.baseWoundMaxDiameter,
                new (bool?, string)[]
                {
            (record.baseMechanicalDamage, "機械傷害"),
            (record.baseMowingInjury, "割草或修剪傷害")
                },
                record.baseOtherNote));

            litTrunkStatus.Text = FormatSummaryHtml(BuildStructureSummary(
                record.trunkDecayPercent,
                record.trunkCavityMaxDiameter,
                record.trunkWoundMaxDiameter,
                new (bool?, string)[]
                {
            (record.trunkMechanicalDamage, "機械傷害"),
            (record.trunkIncludedBark, "夾皮")
                },
                record.trunkOtherNote));

            litBranchStatus.Text = FormatSummaryHtml(BuildStructureSummary(
                record.branchDecayPercent,
                record.branchCavityMaxDiameter,
                record.branchWoundMaxDiameter,
                new (bool?, string)[]
                {
            (record.branchMechanicalDamage, "機械傷害"),
            (record.branchIncludedBark, "夾皮"),
            (record.branchDrooping, "下垂")
                },
                record.branchOtherNote));

            // 5. 其他狀態 (樹冠、修剪、支撐、棲地、土壤)
            litCrownStatus.Text = FormatSummaryHtml(BuildCrownSummary(record));
            litPruningStatus.Text = FormatSummaryHtml(BuildPruningSummary(record));
            litSupportStatus.Text = FormatSummaryHtml(BuildSupportSummary(record));
            litSiteStatus.Text = FormatSummaryHtml(BuildSiteSummary(record));
            litSoilStatus.Text = FormatSummaryHtml(BuildSoilSummary(record));

            // 6. 管理建議
            litManagementStatus.Text = FormatText(record.managementStatus);
            litPriority.Text = FormatText(record.priority);
            litTreatmentDescription.Text = FormatText(record.treatmentDescription);

            // 7. 附件與照片
            BindAttachments(record.attachments);
            BindPhotos(record.photos);
        }

        private void ClearContent()
        {
            // 將所有 Literal 清空
            litHealthId.Text = string.Empty;
            litSystemTreeNo.Text = string.Empty;
            litLocation.Text = string.Empty;
            litSpecies.Text = string.Empty;
            litSurveyDate.Text = string.Empty;
            litSurveyor.Text = string.Empty;
            litStatus.Text = string.Empty;
            litTreeSign.Text = string.Empty;
            litLastUpdate.Text = string.Empty;
            litMemo.Text = string.Empty;
            litLatitude.Text = string.Empty;
            litLongitude.Text = string.Empty;
            litTreeHeight.Text = string.Empty;
            litCanopyArea.Text = string.Empty;
            litGirth100.Text = string.Empty;
            litDiameter100.Text = string.Empty;
            litGirth130.Text = string.Empty;
            litDiameter130.Text = string.Empty;
            litMeasureNote.Text = string.Empty;
            litMajorDisease.Text = string.Empty;
            litMajorPest.Text = string.Empty;
            litGeneralPest.Text = string.Empty;
            litGeneralDisease.Text = string.Empty;
            litPestOtherNote.Text = string.Empty;
            litRootStatus.Text = string.Empty;
            litBaseStatus.Text = string.Empty;
            litTrunkStatus.Text = string.Empty;
            litBranchStatus.Text = string.Empty;
            litCrownStatus.Text = string.Empty;
            litPruningStatus.Text = string.Empty;
            litSupportStatus.Text = string.Empty;
            litSiteStatus.Text = string.Empty;
            litSoilStatus.Text = string.Empty;
            litManagementStatus.Text = string.Empty;
            litPriority.Text = string.Empty;
            litTreatmentDescription.Text = string.Empty;

            BindAttachments(null);
            BindPhotos(null);
        }

        // ---------------------------------------------------------
        // Helper Formatting Methods
        // ---------------------------------------------------------

        private static string FormatText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "--" : value.Trim();
        }

        private static string FormatNumber(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("0.##", CultureInfo.InvariantCulture) : "--";
        }

        private static string FormatCoordinate(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("0.######", CultureInfo.InvariantCulture) : "--";
        }

        private static string CombineLocation(string city, string area)
        {
            if (string.IsNullOrWhiteSpace(city)) return string.IsNullOrWhiteSpace(area) ? string.Empty : area;
            if (string.IsNullOrWhiteSpace(area)) return city;
            return city + area;
        }

        private static string GetTreeSignStatusText(byte? status)
        {
            switch (status)
            {
                case 1: return "有";
                case 2: return "沒有";
                case 3: return "毀損";
                default: return "--";
            }
        }

        private static string FormatSummaryHtml(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "--";
            // 簡單的 HTML Encode 並將中文分號轉為換行
            string encoded = HttpUtility.HtmlEncode(text);
            return encoded.Replace("；", "<br />");
        }

        // ---------------------------------------------------------
        // Build Summary Logic (使用 camelCase 屬性)
        // ---------------------------------------------------------

        private static string BuildMajorDiseaseSummary(TreeHealthRecord record)
        {
            var selections = new List<string>();

            if (record.majorDiseaseBrownRoot == true) selections.Add("樹木褐根病");
            if (record.majorDiseaseGanoderma == true) selections.Add("靈芝");
            if (record.majorDiseaseWoodDecayFungus == true) selections.Add("木材腐朽菌");
            if (record.majorDiseaseCanker == true) selections.Add("潰瘍");

            if (record.majorDiseaseOther == true)
            {
                selections.Add(!string.IsNullOrWhiteSpace(record.majorDiseaseOtherNote)
                    ? "其他：" + record.majorDiseaseOtherNote.Trim()
                    : "其他");
            }

            return selections.Count > 0 ? string.Join("、", selections) : "無";
        }

        private static string BuildMajorPestSummary(TreeHealthRecord record)
        {
            var parts = new List<string>();

            AppendIfNotEmpty(parts, "根系", FormatSelections(
                (record.majorPestRootTunnel, "白蟻蟻道"),
                (record.majorPestRootChew, "白蟻蛀蝕"),
                (record.majorPestRootLive, "白蟻活體")));

            AppendIfNotEmpty(parts, "樹基部", FormatSelections(
                (record.majorPestBaseTunnel, "白蟻蟻道"),
                (record.majorPestBaseChew, "白蟻蛀蝕"),
                (record.majorPestBaseLive, "白蟻活體")));

            AppendIfNotEmpty(parts, "主幹", FormatSelections(
                (record.majorPestTrunkTunnel, "白蟻蟻道"),
                (record.majorPestTrunkChew, "白蟻蛀蝕"),
                (record.majorPestTrunkLive, "白蟻活體")));

            AppendIfNotEmpty(parts, "枝幹", FormatSelections(
                (record.majorPestBranchTunnel, "白蟻蟻道"),
                (record.majorPestBranchChew, "白蟻蛀蝕"),
                (record.majorPestBranchLive, "白蟻活體")));

            AppendIfNotEmpty(parts, "樹冠", FormatSelections(
                (record.majorPestCrownTunnel, "白蟻蟻道"),
                (record.majorPestCrownChew, "白蟻蛀蝕"),
                (record.majorPestCrownLive, "白蟻活體")));

            AppendIfNotEmpty(parts, "其他部位", FormatSelections(
                (record.majorPestOtherTunnel, "白蟻蟻道"),
                (record.majorPestOtherChew, "白蟻蛀蝕"),
                (record.majorPestOtherLive, "白蟻活體")));

            return parts.Count > 0 ? string.Join("；", parts) : "無";
        }

        // 使用 ValueTuple (string Label, string Content) 簡化語法
        private static string BuildGeneralIssueSummary(params (string Label, string Content)[] items)
        {
            var list = new List<string>();
            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item.Content)) continue;
                list.Add($"{item.Label}：{item.Content.Trim()}");
            }
            return list.Count > 0 ? string.Join("；", list) : "無";
        }

        // 使用 ValueTuple 簡化選項傳遞
        private static string BuildStructureSummary(decimal? decayPercent, decimal? cavityMax, decimal? woundMax,
            IEnumerable<(bool? IsSelected, string Label)> options, string otherNote)
        {
            var details = new List<string>();

            if (decayPercent.HasValue) details.Add($"腐朽比例 {FormatDecimal(decayPercent)}%");
            if (cavityMax.HasValue) details.Add($"空洞最大直徑 {FormatDecimal(cavityMax)}");
            if (woundMax.HasValue) details.Add($"傷口最大直徑 {FormatDecimal(woundMax)}");

            var selections = FormatSelections(options == null ? Array.Empty<(bool?, string)>() : options.ToArray());
            if (!string.IsNullOrWhiteSpace(selections) && selections != "--")
            {
                details.Add("狀況：" + selections);
            }

            if (!string.IsNullOrWhiteSpace(otherNote))
            {
                details.Add("其他：" + otherNote.Trim());
            }

            return details.Count > 0 ? string.Join("；", details) : "無";
        }

        private static string BuildCrownSummary(TreeHealthRecord record)
        {
            var details = new List<string>();

            if (record.crownLeafCoveragePercent.HasValue)
                details.Add($"葉量比例 {FormatDecimal(record.crownLeafCoveragePercent)}%");

            if (record.crownDeadBranchPercent.HasValue)
                details.Add($"枯枝比例 {FormatDecimal(record.crownDeadBranchPercent)}%");

            var selections = FormatSelections((record.crownHangingBranch, "懸枝"));
            if (!string.IsNullOrWhiteSpace(selections) && selections != "--")
            {
                details.Add("狀況：" + selections);
            }

            if (!string.IsNullOrWhiteSpace(record.crownOtherNote))
            {
                details.Add("其他：" + record.crownOtherNote.Trim());
            }

            return details.Count > 0 ? string.Join("；", details) : "無";
        }

        private static string BuildPruningSummary(TreeHealthRecord record)
        {
            var details = new List<string>();

            if (!string.IsNullOrWhiteSpace(record.pruningWrongDamage))
            {
                details.Add("修剪錯誤造成傷害：" + record.pruningWrongDamage.Trim());
            }

            var selections = FormatSelections(
                (record.pruningWoundHealing, "傷口癒合不良"),
                (record.pruningEpiphyte, "附生植物"),
                (record.pruningParasite, "寄生植物"),
                (record.pruningVine, "攀附藤本"));

            if (!string.IsNullOrWhiteSpace(selections) && selections != "--")
            {
                details.Add("狀況：" + selections);
            }

            if (!string.IsNullOrWhiteSpace(record.pruningOtherNote))
            {
                details.Add("其他：" + record.pruningOtherNote.Trim());
            }

            return details.Count > 0 ? string.Join("；", details) : "無";
        }

        private static string BuildSupportSummary(TreeHealthRecord record)
        {
            var details = new List<string>();

            if (record.supportCount.HasValue)
            {
                details.Add("支柱數量 " + record.supportCount.Value.ToString(CultureInfo.InvariantCulture));
            }

            var selections = FormatSelections((record.supportEmbedded, "支柱嵌入樹體"));
            if (!string.IsNullOrWhiteSpace(selections) && selections != "--")
            {
                details.Add(selections);
            }

            if (!string.IsNullOrWhiteSpace(record.supportOtherNote))
            {
                details.Add("其他：" + record.supportOtherNote.Trim());
            }

            return details.Count > 0 ? string.Join("；", details) : "無";
        }

        private static string BuildSiteSummary(TreeHealthRecord record)
        {
            var details = new List<string>();

            if (record.siteCementPercent.HasValue)
                details.Add($"鋪面比例 {FormatDecimal(record.siteCementPercent)}%");

            if (record.siteAsphaltPercent.HasValue)
                details.Add($"瀝青比例 {FormatDecimal(record.siteAsphaltPercent)}%");

            var selections = FormatSelections(
                (record.sitePlanter, "花台或綠帶"),
                (record.siteRecreationFacility, "遊憩設施"),
                (record.siteDebrisStack, "堆置雜物"),
                (record.siteBetweenBuildings, "建築夾擠"),
                (record.siteSoilCompaction, "土壤受踩踏夯實"),
                (record.siteOverburiedSoil, "覆土過深"));

            if (!string.IsNullOrWhiteSpace(selections) && selections != "--")
            {
                details.Add("狀況：" + selections);
            }

            if (!string.IsNullOrWhiteSpace(record.siteOtherNote))
            {
                details.Add("其他：" + record.siteOtherNote.Trim());
            }

            return details.Count > 0 ? string.Join("；", details) : "無";
        }

        private static string BuildSoilSummary(TreeHealthRecord record)
        {
            var details = new List<string>();
            details.Add($"pH值 {FormatText(record.soilPh)}");
            details.Add($"有機質含量 {FormatText(record.soilOrganicMatter)}");
            details.Add($"電導度 {FormatText(record.soilEc)}");
               

            return details.Count > 0 ? string.Join("；", details) : "無";
        }

        private void BindAttachments(IList<TreeHealthAttachment> attachments)
        {
            attachments = attachments ?? new List<TreeHealthAttachment>();
            rptAttachments.DataSource = attachments;
            rptAttachments.DataBind();
            phAttachmentEmpty.Visible = attachments.Count == 0;
        }

        private void BindPhotos(IList<TreeHealthPhoto> photos)
        {
            photos = photos ?? new List<TreeHealthPhoto>();
            rptPhotos.DataSource = photos;
            rptPhotos.DataBind();
            phPhotoEmpty.Visible = photos.Count == 0;
        }

        private static void AppendIfNotEmpty(ICollection<string> target, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "--") return;
            target.Add($"{label}：{value}");
        }

        // 核心修改：改用 params (bool?, string)[] 語法，這比 params Tuple<bool?, string>[] 乾淨很多
        private static string FormatSelections(params (bool? IsSelected, string Label)[] items)
        {
            var selected = new List<string>();
            foreach (var item in items)
            {
                if (item.IsSelected == true)
                {
                    selected.Add(item.Label);
                }
            }
            return selected.Count > 0 ? string.Join("、", selected) : "--";
        }

        private static string FormatDecimal(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("0.##", CultureInfo.InvariantCulture) : string.Empty;
        }
    }
}