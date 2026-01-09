using protectTreesV2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using static protectTreesV2.Health.Health;
using System.IO;
using protectTreesV2.Log;
using protectTreesV2.User;
using protectTreesV2.backstage.Manage;

namespace protectTreesV2.backstage.health
{
    public partial class edit : BasePage
    {
        public protectTreesV2.Health.Health system_health = new protectTreesV2.Health.Health();

        // --- 全域設定：照片 (Photo) ---
        private readonly string[] PhotoAllowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        private const int PhotoMaxSizeBytes = 10 * 1024 * 1024; // 10MB

        // --- 全域設定：附件 (Attachment) ---
        private readonly string[] AttachAllowedExtensions = new[] { ".zip" };
        private const int AttachMaxSizeBytes = 30 * 1024 * 1024; // 30MB

        [Serializable]
        public class AttachmentJsonModel
        {
            public string fileName { get; set; }
            public string filePath { get; set; }
            public bool isTemp { get; set; } 
        }

        [Serializable]
        public class PhotoJsonModel
        {
            public string key { get; set; }
            public string filePath { get; set; }
            public string caption { get; set; }
            public string fileName { get; set; }
        }

        [Serializable]
        public class PhotoMeta
        {
            public string key { get; set; }
            public string fileName { get; set; }
            public string caption { get; set; }
        }



        /// <summary>
        /// 目前編輯的健檢 ID (若為 0 代表新增)
        /// </summary>
        public int CurrentHealthID
        {
            get { return (int)(ViewState["CurrentHealthID"] ?? 0); }
            set { ViewState["CurrentHealthID"] = value; }
        }

        /// <summary>
        /// 目前針對的樹木 ID 
        /// </summary>
        public int CurrentTreeID
        {
            get { return (int)(ViewState["CurrentTreeID"] ?? 0); }
            set { ViewState["CurrentTreeID"] = value; }
        }

        /// <summary>
        /// 動作判斷
        /// </summary>
        public string Action
        {
            get { return CurrentHealthID > 0 ? "edit" : "add"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 判斷動作
                InitIdsFromSession();

                // 安全檢查：如果兩個 ID 都沒有，代表非法進入
                if (CurrentHealthID == 0 && CurrentTreeID == 0)
                {
                    ShowMessage("系統提示", "無效的參數，請由列表頁進入。");
                    Response.Redirect("list.aspx");
                    return;
                }

                // 初始化下拉選單等基礎介面
                InitDropdowns();

                // 設定標題文字
                SetPageTitle();

                //載入資料
                LoadData();
            }
        }

        private void InitIdsFromSession()
        {

            base.KeepState();

            // 處理健檢 ID 
            if (!string.IsNullOrEmpty(this.setHealthID))
            {
                int hId;
                if (int.TryParse(this.setHealthID, out hId))
                {
                    CurrentHealthID = hId;
                }
                // 清空 Session
                this.setHealthID = null;
            }

            // 處理樹木 ID 
            if (!string.IsNullOrEmpty(this.setTreeID))
            {
                int tId;
                if (int.TryParse(this.setTreeID, out tId))
                {
                    CurrentTreeID = tId;
                }
                // 清空 Session
                this.setTreeID = null;
            }
        }

        /// <summary>
        /// 設定頁面標題變數
        /// </summary>
        private void SetPageTitle()
        {
            if (this.Action == "add")
            {
                // 新增模式
                Literal_pathAction.Text = "新增";
                Literal_title.Text = "健檢紀錄新增";
            }
            else
            {
                // 編輯模式
                Literal_pathAction.Text = "編輯";
                Literal_title.Text = "健檢紀錄編輯";
            }
        }

        /// <summary>
        /// 初始化頁面下拉選單
        /// </summary>
        private void InitDropdowns()
        {
            // 樹牌狀態
            Base.DropdownBinder.Bind_enum_treeSignStatus(ref DropDownList_treeSignStatus, true);

            // 錯誤修剪傷害
            Base.DropdownBinder.Bind_enum_pruningDamageType(ref DropDownList_pruningWrongDamage, true);

            // 建議處理優先順序
            Base.DropdownBinder.Bind_enum_treatmentPriority(ref DropDownList_priority, true);
        }

        public TreeHealthRecord GetNewRecordByTreeID(int treeId)
        {
            var treeInfo = TreeCatalog.TreeService.GetTree(treeId);

            // 防呆：找不到樹就回傳 null
            if (treeInfo == null) return null;
            TreeHealthRecord newRecord = new TreeHealthRecord();

            // ==========================================
            // A. 填入 Key 與 樹籍資料 (Header 用)
            // ==========================================
            newRecord.treeID = treeInfo.TreeID;
            newRecord.healthID = 0; // 0 代表這是新的一筆

            newRecord.systemTreeNo = treeInfo.SystemTreeNo;
            newRecord.agencyTreeNo = treeInfo.AgencyTreeNo;
            newRecord.cityName = treeInfo.CityName;
            newRecord.areaName = treeInfo.AreaName;
            newRecord.speciesName = treeInfo.SpeciesCommonName;
            newRecord.manager = treeInfo.Manager;
            newRecord.surveyDate = DateTime.Today; 
            newRecord.dataStatus = 0; 

            // 如果樹籍原本就有座標或樹高，可以先帶入當參考值
            // 使用者在健檢時發現不一樣可以再改
            //newRecord.latitude = treeInfo.Latitude;
            //newRecord.longitude = treeInfo.Longitude;
            //newRecord.treeHeight = treeInfo.TreeHeight;
            //newRecord.canopyArea = treeInfo.CanopyProjectionArea;

            return newRecord;
        }
        /// <summary>
        /// 載入編輯資料
        /// </summary>
        private void LoadData()
        {
            TreeHealthRecord record = null;

            try
            {
                if (this.Action == "edit")
                {
                    // 編輯模式：撈取完整健檢資料
                    record = system_health.GetHealthRecord(this.CurrentHealthID);
                }
                else
                {
                    // 新增模式：撈取樹籍資料並給予預設值
                    record = GetNewRecordByTreeID(this.CurrentTreeID);
                }

                if (record == null)
                {
                    ShowMessage("系統提示", "找不到資料。");
                    return;
                }

                if (this.Action == "edit")
                {
                    // 編輯模式：反查並更新 TreeID
                    this.CurrentTreeID = record.treeID;
                    Literal_btnSaveText.Text = "儲存";

                    //載入編輯紀錄
                    BindLogs();
                }
                else
                {
                    // 新增模式
                    Literal_btnSaveText.Text = "新增";
                }

               

                // 綁定樹籍 Header (唯讀)
                Label_systemTreeNo.Text = record.systemTreeNo;
                Label_speciesName.Text = record.speciesName;
                Label_cityName.Text = record.cityName;
                Label_areaName.Text = record.areaName;
                Label_manager.Text = record.manager;

                // 狀態顯示
                Label_recordStatus.Text = record.dataStatusText;
                Label_recordStatus.CssClass = record.dataStatus == 1 ? "badge bg-success" : "badge bg-secondary";

                // 綁定表單欄位
                BindFormFields(record);

                // 綁定照片 (轉成 JSON 給前端)
                if (record.photos != null && record.photos.Count > 0)
                {
                    var photoList = record.photos.Select(p => new PhotoJsonModel
                    {
                        key = p.photoID.ToString(),
                        filePath = ResolveUrl(p.filePath), // 網頁相對路徑
                        caption = p.caption,
                        fileName = p.fileName
                    }).ToList();
                    HiddenField_existingPhotosData.Value = JsonConvert.SerializeObject(photoList);
                }
                else
                {
                    HiddenField_existingPhotosData.Value = "";
                }

                // 綁定附件 (轉成 JSON 給前端)
                var attach = record.attachments.FirstOrDefault();
                if (attach != null)
                {
                    var fileData = new AttachmentJsonModel
                    {
                        fileName = attach.fileName,
                        filePath = ResolveUrl(attach.filePath), 
                        isTemp = false 
                    };

                    HiddenField_existingFileData.Value = JsonConvert.SerializeObject(fileData);
                }
                else
                {
                   
                    HiddenField_existingFileData.Value = "";
                }
            }
            catch (Exception ex)
            {
                ShowMessage("系統提示", "系統錯誤：" + ex.Message);
            }
        }

        private void BindLogs()
        {
            var logs = FunctionLogService.GetLogs(LogFunctionTypes.Health,this.CurrentHealthID ) ?? new List<FunctionLogEntry>();
            Panel_logs.Visible = true;
            GridView_logs.DataSource = logs;
            GridView_logs.DataBind();
        }

        protected void gvLogs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_logs.PageIndex = e.NewPageIndex;
            BindLogs();
        }

        private void BindFormFields(TreeHealthRecord data)
        {
            // A. 一般調查
            TextBox_surveyDate.Text = data.surveyDate?.ToString("yyyy-MM-dd");
            TextBox_surveyor.Text = data.surveyor;
            if (data.treeSignStatus.HasValue) DropDownList_treeSignStatus.SelectedValue = data.treeSignStatus.ToString();
            bool isFinalized = (data.dataStatus == 1);
            CheckBox_dataStatus.Checked = isFinalized;

            if (isFinalized)
            {
                // 如果已經是定稿，鎖定 CheckBox，不允許取消
                CheckBox_dataStatus.Enabled = false;
                CheckBox_dataStatus.ToolTip = "此紀錄已定稿，無法變更為草稿";
            }
            else
            {
                // 如果是草稿，允許自由切換
                CheckBox_dataStatus.Enabled = true;
                CheckBox_dataStatus.ToolTip = "勾選後存檔即為定稿";
            }

            // B. 座標 & 規格
            TextBox_latitude.Text = data.latitude?.ToString();
            TextBox_longitude.Text = data.longitude?.ToString();
            TextBox_treeHeight.Text = data.treeHeight?.ToString();
            TextBox_canopyArea.Text = data.canopyArea?.ToString();

            TextBox_girth100.Text = data.girth100;
            TextBox_diameter100.Text = data.diameter100;
            TextBox_girth130.Text = data.girth130;
            TextBox_diameter130.Text = data.diameter130;
            TextBox_measureNote.Text = data.measureNote;

            // C. 病蟲害 (CheckBox)
            SetCheck(CheckBox_diseaseBrownRoot, data.majorDiseaseBrownRoot);
            SetCheck(CheckBox_diseaseGanoderma, data.majorDiseaseGanoderma);
            SetCheck(CheckBox_diseaseWoodDecay, data.majorDiseaseWoodDecayFungus);
            SetCheck(CheckBox_diseaseCanker, data.majorDiseaseCanker);
            SetCheck(CheckBox_diseaseOther, data.majorDiseaseOther);
            TextBox_diseaseOtherNote.Text = data.majorDiseaseOtherNote;

            SetCheck(CheckBox_majorPestRootTunnel, data.majorPestRootTunnel);
            SetCheck(CheckBox_majorPestRootChew, data.majorPestRootChew);
            SetCheck(CheckBox_majorPestRootLive, data.majorPestRootLive);
            SetCheck(CheckBox_majorPestBaseTunnel, data.majorPestBaseTunnel);
            SetCheck(CheckBox_majorPestBaseChew, data.majorPestBaseChew);
            SetCheck(CheckBox_majorPestBaseLive, data.majorPestBaseLive);
            SetCheck(CheckBox_majorPestTrunkTunnel, data.majorPestTrunkTunnel);
            SetCheck(CheckBox_majorPestTrunkChew, data.majorPestTrunkChew);
            SetCheck(CheckBox_majorPestTrunkLive, data.majorPestTrunkLive);
            SetCheck(CheckBox_majorPestBranchTunnel, data.majorPestBranchTunnel);
            SetCheck(CheckBox_majorPestBranchChew, data.majorPestBranchChew);
            SetCheck(CheckBox_majorPestBranchLive, data.majorPestBranchLive);
            SetCheck(CheckBox_majorPestCrownTunnel, data.majorPestCrownTunnel);
            SetCheck(CheckBox_majorPestCrownChew, data.majorPestCrownChew);
            SetCheck(CheckBox_majorPestCrownLive, data.majorPestCrownLive);
            SetCheck(CheckBox_majorPestOtherTunnel, data.majorPestOtherTunnel);
            SetCheck(CheckBox_majorPestOtherChew, data.majorPestOtherChew);
            SetCheck(CheckBox_majorPestOtherLive, data.majorPestOtherLive);

            // D. 一般描述
            TextBox_generalPestRoot.Text = data.generalPestRoot;
            TextBox_generalPestBase.Text = data.generalPestBase;
            TextBox_generalPestTrunk.Text = data.generalPestTrunk;
            TextBox_generalPestBranch.Text = data.generalPestBranch;
            TextBox_generalPestCrown.Text = data.generalPestCrown;
            TextBox_generalPestOther.Text = data.generalPestOther;

            TextBox_generalDiseaseRoot.Text = data.generalDiseaseRoot;
            TextBox_generalDiseaseBase.Text = data.generalDiseaseBase;
            TextBox_generalDiseaseTrunk.Text = data.generalDiseaseTrunk;
            TextBox_generalDiseaseBranch.Text = data.generalDiseaseBranch;
            TextBox_generalDiseaseCrown.Text = data.generalDiseaseCrown;
            TextBox_generalDiseaseOther.Text = data.generalDiseaseOther;
            TextBox_pestOtherNote.Text = data.pestOtherNote;

            // E. 細節檢測
            TextBox_rootDecayPercent.Text = data.rootDecayPercent?.ToString();
            TextBox_rootCavityMaxDiameter.Text = data.rootCavityMaxDiameter?.ToString();
            TextBox_rootWoundMaxDiameter.Text = data.rootWoundMaxDiameter?.ToString();
            SetCheck(CheckBox_rootMechanicalDamage, data.rootMechanicalDamage);
            SetCheck(CheckBox_rootMowingInjury, data.rootMowingInjury);
            SetCheck(CheckBox_rootInjury, data.rootInjury);
            SetCheck(CheckBox_rootGirdling, data.rootGirdling);
            TextBox_rootOtherNote.Text = data.rootOtherNote;

            TextBox_baseDecayPercent.Text = data.baseDecayPercent?.ToString();
            TextBox_baseCavityMaxDiameter.Text = data.baseCavityMaxDiameter?.ToString();
            TextBox_baseWoundMaxDiameter.Text = data.baseWoundMaxDiameter?.ToString();
            SetCheck(CheckBox_baseMechanicalDamage, data.baseMechanicalDamage);
            SetCheck(CheckBox_baseMowingInjury, data.baseMowingInjury);
            TextBox_baseOtherNote.Text = data.baseOtherNote;

            TextBox_trunkDecayPercent.Text = data.trunkDecayPercent?.ToString();
            TextBox_trunkCavityMaxDiameter.Text = data.trunkCavityMaxDiameter?.ToString();
            TextBox_trunkWoundMaxDiameter.Text = data.trunkWoundMaxDiameter?.ToString();
            SetCheck(CheckBox_trunkMechanicalDamage, data.trunkMechanicalDamage);
            SetCheck(CheckBox_trunkIncludedBark, data.trunkIncludedBark);
            TextBox_trunkOtherNote.Text = data.trunkOtherNote;

            TextBox_branchDecayPercent.Text = data.branchDecayPercent?.ToString();
            TextBox_branchCavityMaxDiameter.Text = data.branchCavityMaxDiameter?.ToString();
            TextBox_branchWoundMaxDiameter.Text = data.branchWoundMaxDiameter?.ToString();
            SetCheck(CheckBox_branchMechanicalDamage, data.branchMechanicalDamage);
            SetCheck(CheckBox_branchIncludedBark, data.branchIncludedBark);
            SetCheck(CheckBox_branchDrooping, data.branchDrooping);
            TextBox_branchOtherNote.Text = data.branchOtherNote;

            TextBox_crownLeafCoveragePercent.Text = data.crownLeafCoveragePercent?.ToString();
            TextBox_crownDeadBranchPercent.Text = data.crownDeadBranchPercent?.ToString();
            SetCheck(CheckBox_crownHangingBranch, data.crownHangingBranch);
            TextBox_crownOtherNote.Text = data.crownOtherNote;
            TextBox_growthNote.Text = data.growthNote;

            // F. 修剪與支撐
            DropDownList_pruningWrongDamage.SelectedValue = data.pruningWrongDamage;
            SetCheck(CheckBox_pruningWoundHealing, data.pruningWoundHealing);
            SetCheck(CheckBox_pruningEpiphyte, data.pruningEpiphyte);
            SetCheck(CheckBox_pruningParasite, data.pruningParasite);
            SetCheck(CheckBox_pruningVine, data.pruningVine);
            TextBox_pruningOtherNote.Text = data.pruningOtherNote;

            TextBox_supportCount.Text = data.supportCount?.ToString();
            SetCheck(CheckBox_supportEmbedded, data.supportEmbedded);
            TextBox_supportOtherNote.Text = data.supportOtherNote;

            // G. 棲地與土壤
            TextBox_siteCementPercent.Text = data.siteCementPercent?.ToString();
            TextBox_siteAsphaltPercent.Text = data.siteAsphaltPercent?.ToString();
            SetCheck(CheckBox_sitePlanter, data.sitePlanter);
            SetCheck(CheckBox_siteRecreationFacility, data.siteRecreationFacility);
            SetCheck(CheckBox_siteDebrisStack, data.siteDebrisStack);
            SetCheck(CheckBox_siteBetweenBuildings, data.siteBetweenBuildings);
            SetCheck(CheckBox_siteSoilCompaction, data.siteSoilCompaction);
            SetCheck(CheckBox_siteOverburiedSoil, data.siteOverburiedSoil);
            TextBox_siteOtherNote.Text = data.siteOtherNote;

            TextBox_soilPh.Text = data.soilPh;
            TextBox_soilOrganicMatter.Text = data.soilOrganicMatter;
            TextBox_soilEc.Text = data.soilEc;

            // H. 管理建議
            TextBox_managementStatus.Text = data.managementStatus;
            DropDownList_priority.SelectedValue = data.priority;
            TextBox_treatmentDescription.Text = data.treatmentDescription;
        }

        private void SetCheck(CheckBox cb, bool? val)
        {
            cb.Checked = val.HasValue && val.Value;
        }

        private void TrySaveTempFile()
        {
            // 1) 讀取現有 JSON
            List<PhotoJsonModel> currentList = new List<PhotoJsonModel>();
            string existingJson = HiddenField_existingPhotosData.Value;
            if (!string.IsNullOrEmpty(existingJson))
            {
                try
                {
                    currentList = JsonConvert.DeserializeObject<List<PhotoJsonModel>>(existingJson) ?? new List<PhotoJsonModel>();
                }
                catch { }
            }

            // 2) 讀取 Metadata JSON（caption / 新檔對照）
            List<PhotoMeta> metaList = new List<PhotoMeta>();
            string metaJson = HiddenField_photoMetadata.Value;
            if (!string.IsNullOrEmpty(metaJson))
            {
                try
                {
                    metaList = JsonConvert.DeserializeObject<List<PhotoMeta>>(metaJson) ?? new List<PhotoMeta>();
                }
                catch { }
            }

            // 3) 讀取刪除清單
            HashSet<string> deletedKeySet = new HashSet<string>();
            string deletedIdsStr = HiddenField_deletedPhotoIds.Value;
            if (!string.IsNullOrEmpty(deletedIdsStr))
            {
                foreach (var id in deletedIdsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    deletedKeySet.Add(id.Trim());
                }
            }

            // 4) 先把「舊照片」最新 caption 回寫進 currentList
            if (currentList.Any() && metaList.Any())
            {
                var captionByKey = metaList
                    .Where(m => !string.IsNullOrWhiteSpace(m.key))
                    .GroupBy(m => m.key)
                    .ToDictionary(g => g.Key, g => g.Last().caption ?? "");

                foreach (var p in currentList)
                {
                    if (!string.IsNullOrWhiteSpace(p.key) && captionByKey.TryGetValue(p.key, out var latestCaption))
                    {
                        p.caption = latestCaption;
                    }
                }
            }

            // 5) 把刪除狀態同步到 existingPhotosData
            if (deletedKeySet.Count > 0 && currentList.Any())
            {
                currentList = currentList
                    .Where(p => p != null && !deletedKeySet.Contains(p.key))
                    .ToList();
            }

            // 先更新 HiddenField
            HiddenField_existingPhotosData.Value = JsonConvert.SerializeObject(currentList);

            // 如果沒有新上傳檔案，到此結束（但 caption/刪除都已被保存）
            if (!FileUpload_pendingPhotos.HasFiles) return;

            // 7) 計算最小 Key（給新 temp key 用）
            long minKey = 0;
            foreach (var p in currentList)
            {
                if (long.TryParse(p.key, out long k) && k < minKey) minKey = k;
            }

            // 8) 準備暫存目錄
            string tempVirtualDir = "~/_file/health/temp/";
            string tempPhysicalDir = Server.MapPath(tempVirtualDir);
            if (!Directory.Exists(tempPhysicalDir)) Directory.CreateDirectory(tempPhysicalDir);

            // 9) 新檔 Metadata Queue（只取 key<0 且 fileName 有值）
            var metaLookup = metaList
                .Where(m => !string.IsNullOrWhiteSpace(m.fileName)
                         && int.TryParse(m.key, out var k) && k < 0)
                .GroupBy(m => Path.GetFileName(m.fileName.Trim()))
                .ToDictionary(g => g.Key, g => g.ToList());

            var postedFiles = FileUpload_pendingPhotos.PostedFiles;

            // 10) 存 temp
            for (int i = postedFiles.Count - 1; i >= 0; i--)
            {
                var file = postedFiles[i];

                if (file == null || file.ContentLength == 0) continue;

                string clientFileName = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(clientFileName).ToLowerInvariant();

                if (!PhotoAllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)) continue;
                if (file.ContentLength > PhotoMaxSizeBytes) continue;

                // 產生 GUID 檔名
                string safeName = $"{Guid.NewGuid():N}{ext}";
                string fullPath = Path.Combine(tempPhysicalDir, safeName);
                while (File.Exists(fullPath))
                {
                    safeName = $"{Guid.NewGuid():N}{ext}";
                    fullPath = Path.Combine(tempPhysicalDir, safeName);
                }
                file.SaveAs(fullPath);

                // caption 對應（Queue 消耗）
                string caption = "";
                if (metaLookup.TryGetValue(clientFileName, out var candidates) && candidates.Count > 0)
                {
                    caption = candidates[0].caption ?? "";
                    candidates.RemoveAt(0);
                }

                minKey--;

                // 若這個 temp key 已被刪除，則不加入清單
                if (deletedKeySet.Contains(minKey.ToString())) continue;

                currentList.Add(new PhotoJsonModel
                {
                    key = minKey.ToString(),
                    filePath = ResolveUrl(tempVirtualDir + safeName),
                    fileName = clientFileName,
                    caption = caption
                });
            }

            // 11) 最後再更新 HiddenField
            HiddenField_existingPhotosData.Value = JsonConvert.SerializeObject(currentList);
        }
        private void TrySaveTempAttachment()
        {
            // 1) 如果使用者已按刪除，就不要暫存
            if (HiddenField_isFileDeleted.Value == "true")
            {
                HiddenField_existingFileData.Value = "";
                return;
            }

            // 2) 沒有新檔就不用動
            if (!FileUpload_attachment.HasFile) return;

            var file = FileUpload_attachment.PostedFile;
            if (file == null || file.ContentLength == 0) return;

            string originalName = Path.GetFileName(file.FileName);
            string ext = Path.GetExtension(originalName).ToLowerInvariant();

            // 3) zip only 驗證
            if (!AttachAllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            // 4) 大小限制
            if (file.ContentLength > AttachMaxSizeBytes) return;

            // 5) 暫存資料夾
            string tempVirtualDir = "~/_file/health/temp/";
            string tempPhysicalDir = Server.MapPath(tempVirtualDir);
            if (!Directory.Exists(tempPhysicalDir)) Directory.CreateDirectory(tempPhysicalDir);

            // 6) 用 GUID 存成安全檔名（避免覆蓋）
            string safeName = $"{Guid.NewGuid():N}{ext}";
            string fullPath = Path.Combine(tempPhysicalDir, safeName);
            file.SaveAs(fullPath);

            // 7) 回寫 HiddenField_existingFileData（isTemp=true）
            var tempJson = new AttachmentJsonModel
            {
                fileName = originalName,                         // 使用者看到的原始檔名
                filePath = ResolveUrl(tempVirtualDir + safeName), // temp 檔路徑
                isTemp = true
            };

            HiddenField_existingFileData.Value = JsonConvert.SerializeObject(tempJson);

            // 8) 有新檔就代表不是刪除
            HiddenField_isFileDeleted.Value = "false";
        }

        /// <summary>
        /// 儲存按鈕事件 
        /// </summary>
        protected void LinkButton_save_Click(object sender, EventArgs e)
        {
            try
            {
                string surveyDateStr = TextBox_surveyDate.Text.Trim();
                bool isFinalized = CheckBox_dataStatus.Checked;

                // 必填檢查: 調查日期
                if (string.IsNullOrEmpty(surveyDateStr) || !DateTime.TryParse(surveyDateStr, out DateTime surveyDate))
                {
                    TrySaveTempFile();
                    TrySaveTempAttachment();
                    ShowMessage("系統提示", "「調查日期」為必填欄位。");
                    return;
                }

                if (system_health.CheckSurveyDateDuplicate(this.CurrentTreeID, surveyDate, this.CurrentHealthID))
                {
                    TrySaveTempFile();
                    TrySaveTempAttachment();
                    ShowMessage("系統提示", $"該樹木在 {surveyDate:yyyy/MM/dd} 已經有其他的調查紀錄，請勿重複建立。");
                    return;
                }

                // 定稿驗證
                string errorMsg = ValidateFormData(isFinalized);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    TrySaveTempFile();
                    TrySaveTempAttachment();
                    ShowMessage("系統提示", errorMsg);
                    return;
                }

                // 如果是編輯模式 (ID > 0)
                if (this.CurrentHealthID > 0)
                {
                    // 重新從資料庫讀取目前的狀態
                    var currentRecord = system_health.GetHealthRecord(this.CurrentHealthID);

                    // 如果資料庫裡已經是定稿 (1)，但前端傳來的卻是未勾選 (false)
                    if (currentRecord != null && currentRecord.dataStatus == 1 && !CheckBox_dataStatus.Checked)
                    {
                        TrySaveTempFile();
                        TrySaveTempAttachment();
                        ShowMessage("系統提示", "此紀錄已經定稿，不允許變更回草稿狀態。");

                        // 強制把畫面勾選回去並鎖定，避免誤會
                        CheckBox_dataStatus.Checked = true;
                        CheckBox_dataStatus.Enabled = false;
                        return;
                    }
                }

                // 準備資料物件
                TreeHealthRecord record = GetFormData();

                // 儲存主檔 (Service 會自動判斷 healthID <= 0 做 Insert)
                var user = UserService.GetCurrentUser();
                int accountID = user?.userID ?? 0;
                int savedHealthID = system_health.SaveHealthRecord(record, accountID);

                // 更新 CurrentHealthID，確保後續照片存檔有 ID
                this.CurrentHealthID = savedHealthID;

                // 處理照片
                ProcessPhotos(savedHealthID, accountID);

                // 處理附件
                ProcessAttachment(savedHealthID, accountID);

                ShowMessage("系統提示", "資料儲存成功！");

                setHealthID = savedHealthID.ToString();
                setTreeID = null;
                ReloadWithState("edit.aspx");

                return;
            }
            catch (Exception ex)
            {
                TrySaveTempFile();
                TrySaveTempAttachment();
                ShowMessage("系統提示", "系統錯誤："+ex.Message);
            }
        }

        private TreeHealthRecord GetFormData()
        {
            TreeHealthRecord r = new TreeHealthRecord();
            r.healthID = this.CurrentHealthID;
            r.treeID = this.CurrentTreeID;

            // ==========================================
            // 1. 一般資料
            // ==========================================
            if (DateTime.TryParse(TextBox_surveyDate.Text, out DateTime dt)) r.surveyDate = dt;
            r.surveyor = TextBox_surveyor.Text.Trim();
            r.dataStatus = CheckBox_dataStatus.Checked ? 1 : 0;
            if (byte.TryParse(DropDownList_treeSignStatus.SelectedValue, out byte sign)) r.treeSignStatus = sign;

            // ==========================================
            // 2. 樹木規格
            // ==========================================
            r.latitude = ParseDec(TextBox_latitude.Text);
            r.longitude = ParseDec(TextBox_longitude.Text);
            r.treeHeight = ParseDec(TextBox_treeHeight.Text);
            r.canopyArea = ParseDec(TextBox_canopyArea.Text);

            r.girth100 = TextBox_girth100.Text.Trim();
            r.diameter100 = TextBox_diameter100.Text.Trim();
            r.girth130 = TextBox_girth130.Text.Trim();
            r.diameter130 = TextBox_diameter130.Text.Trim();
            r.measureNote = TextBox_measureNote.Text.Trim();

            // ==========================================
            // 3. 重大病害 (CheckBox)
            // ==========================================
            r.majorDiseaseBrownRoot = CheckBox_diseaseBrownRoot.Checked;
            r.majorDiseaseGanoderma = CheckBox_diseaseGanoderma.Checked;
            r.majorDiseaseWoodDecayFungus = CheckBox_diseaseWoodDecay.Checked;
            r.majorDiseaseCanker = CheckBox_diseaseCanker.Checked;
            r.majorDiseaseOther = CheckBox_diseaseOther.Checked;
            r.majorDiseaseOtherNote = TextBox_diseaseOtherNote.Text.Trim();

            // ==========================================
            // 4. 重大蟲害 (CheckBox) - 白蟻
            // ==========================================

            // 根系
            r.majorPestRootTunnel = CheckBox_majorPestRootTunnel.Checked;
            r.majorPestRootChew = CheckBox_majorPestRootChew.Checked;
            r.majorPestRootLive = CheckBox_majorPestRootLive.Checked;

            // 基部
            r.majorPestBaseTunnel = CheckBox_majorPestBaseTunnel.Checked;
            r.majorPestBaseChew = CheckBox_majorPestBaseChew.Checked;
            r.majorPestBaseLive = CheckBox_majorPestBaseLive.Checked;

            // 主幹
            r.majorPestTrunkTunnel = CheckBox_majorPestTrunkTunnel.Checked;
            r.majorPestTrunkChew = CheckBox_majorPestTrunkChew.Checked;
            r.majorPestTrunkLive = CheckBox_majorPestTrunkLive.Checked;

            // 枝幹
            r.majorPestBranchTunnel = CheckBox_majorPestBranchTunnel.Checked;
            r.majorPestBranchChew = CheckBox_majorPestBranchChew.Checked;
            r.majorPestBranchLive = CheckBox_majorPestBranchLive.Checked;

            // 樹冠
            r.majorPestCrownTunnel = CheckBox_majorPestCrownTunnel.Checked;
            r.majorPestCrownChew = CheckBox_majorPestCrownChew.Checked;
            r.majorPestCrownLive = CheckBox_majorPestCrownLive.Checked;

            // 其他
            r.majorPestOtherTunnel = CheckBox_majorPestOtherTunnel.Checked;
            r.majorPestOtherChew = CheckBox_majorPestOtherChew.Checked;
            r.majorPestOtherLive = CheckBox_majorPestOtherLive.Checked;

            // ==========================================
            // 5. 一般病蟲害 (文字描述)
            // ==========================================
            // 蟲害
            r.generalPestRoot = TextBox_generalPestRoot.Text.Trim();
            r.generalPestBase = TextBox_generalPestBase.Text.Trim();
            r.generalPestTrunk = TextBox_generalPestTrunk.Text.Trim();
            r.generalPestBranch = TextBox_generalPestBranch.Text.Trim();
            r.generalPestCrown = TextBox_generalPestCrown.Text.Trim();
            r.generalPestOther = TextBox_generalPestOther.Text.Trim();
            // 病害
            r.generalDiseaseRoot = TextBox_generalDiseaseRoot.Text.Trim();
            r.generalDiseaseBase = TextBox_generalDiseaseBase.Text.Trim();
            r.generalDiseaseTrunk = TextBox_generalDiseaseTrunk.Text.Trim();
            r.generalDiseaseBranch = TextBox_generalDiseaseBranch.Text.Trim();
            r.generalDiseaseCrown = TextBox_generalDiseaseCrown.Text.Trim();
            r.generalDiseaseOther = TextBox_generalDiseaseOther.Text.Trim();

            r.pestOtherNote = TextBox_pestOtherNote.Text.Trim();

            // ==========================================
            // 6. 生長外觀 - 詳細檢測
            // ==========================================
            // 根系
            r.rootDecayPercent = ParseDec(TextBox_rootDecayPercent.Text);
            r.rootCavityMaxDiameter = ParseDec(TextBox_rootCavityMaxDiameter.Text);
            r.rootWoundMaxDiameter = ParseDec(TextBox_rootWoundMaxDiameter.Text);
            r.rootMechanicalDamage = CheckBox_rootMechanicalDamage.Checked;
            r.rootMowingInjury = CheckBox_rootMowingInjury.Checked;
            r.rootInjury = CheckBox_rootInjury.Checked;
            r.rootGirdling = CheckBox_rootGirdling.Checked;
            r.rootOtherNote = TextBox_rootOtherNote.Text.Trim();

            // 基部
            r.baseDecayPercent = ParseDec(TextBox_baseDecayPercent.Text);
            r.baseCavityMaxDiameter = ParseDec(TextBox_baseCavityMaxDiameter.Text);
            r.baseWoundMaxDiameter = ParseDec(TextBox_baseWoundMaxDiameter.Text);
            r.baseMechanicalDamage = CheckBox_baseMechanicalDamage.Checked;
            r.baseMowingInjury = CheckBox_baseMowingInjury.Checked;
            r.baseOtherNote = TextBox_baseOtherNote.Text.Trim();

            // 主幹
            r.trunkDecayPercent = ParseDec(TextBox_trunkDecayPercent.Text);
            r.trunkCavityMaxDiameter = ParseDec(TextBox_trunkCavityMaxDiameter.Text);
            r.trunkWoundMaxDiameter = ParseDec(TextBox_trunkWoundMaxDiameter.Text);
            r.trunkMechanicalDamage = CheckBox_trunkMechanicalDamage.Checked;
            r.trunkIncludedBark = CheckBox_trunkIncludedBark.Checked;
            r.trunkOtherNote = TextBox_trunkOtherNote.Text.Trim();

            // 枝幹
            r.branchDecayPercent = ParseDec(TextBox_branchDecayPercent.Text);
            r.branchCavityMaxDiameter = ParseDec(TextBox_branchCavityMaxDiameter.Text);
            r.branchWoundMaxDiameter = ParseDec(TextBox_branchWoundMaxDiameter.Text);
            r.branchMechanicalDamage = CheckBox_branchMechanicalDamage.Checked;
            r.branchIncludedBark = CheckBox_branchIncludedBark.Checked;
            r.branchDrooping = CheckBox_branchDrooping.Checked;
            r.branchOtherNote = TextBox_branchOtherNote.Text.Trim();

            // 樹冠
            r.crownLeafCoveragePercent = ParseDec(TextBox_crownLeafCoveragePercent.Text);
            r.crownDeadBranchPercent = ParseDec(TextBox_crownDeadBranchPercent.Text);
            r.crownHangingBranch = CheckBox_crownHangingBranch.Checked;
            r.crownOtherNote = TextBox_crownOtherNote.Text.Trim();

            r.growthNote = TextBox_growthNote.Text.Trim();

            // ==========================================
            // 7. 修剪與支撐
            // ==========================================
            r.pruningWrongDamage = DropDownList_pruningWrongDamage.SelectedValue;
            r.pruningWoundHealing = CheckBox_pruningWoundHealing.Checked;
            r.pruningEpiphyte = CheckBox_pruningEpiphyte.Checked;
            r.pruningParasite = CheckBox_pruningParasite.Checked;
            r.pruningVine = CheckBox_pruningVine.Checked;
            r.pruningOtherNote = TextBox_pruningOtherNote.Text.Trim();

            r.supportCount = ParseInt(TextBox_supportCount.Text);
            r.supportEmbedded = CheckBox_supportEmbedded.Checked;
            r.supportOtherNote = TextBox_supportOtherNote.Text.Trim();

            // ==========================================
            // 8. 棲地與土壤
            // ==========================================
            r.siteCementPercent = ParseDec(TextBox_siteCementPercent.Text);
            r.siteAsphaltPercent = ParseDec(TextBox_siteAsphaltPercent.Text);
            r.sitePlanter = CheckBox_sitePlanter.Checked;
            r.siteRecreationFacility = CheckBox_siteRecreationFacility.Checked;
            r.siteDebrisStack = CheckBox_siteDebrisStack.Checked;
            r.siteBetweenBuildings = CheckBox_siteBetweenBuildings.Checked;
            r.siteSoilCompaction = CheckBox_siteSoilCompaction.Checked;
            r.siteOverburiedSoil = CheckBox_siteOverburiedSoil.Checked;
            r.siteOtherNote = TextBox_siteOtherNote.Text.Trim();

            r.soilPh = TextBox_soilPh.Text.Trim();
            r.soilOrganicMatter = TextBox_soilOrganicMatter.Text.Trim();
            r.soilEc = TextBox_soilEc.Text.Trim();

            // ==========================================
            // 9. 管理建議
            // ==========================================
            r.managementStatus = TextBox_managementStatus.Text.Trim();
            r.priority = DropDownList_priority.SelectedValue;
            r.treatmentDescription = TextBox_treatmentDescription.Text.Trim();

            return r;
        }

        /// <summary>
        /// 統一驗證表單資料 (包含必填、數值範圍、字串長度)
        /// </summary>
        /// <param name="isFinalized">是否為定稿狀態 (true=嚴格檢查必填, false=只檢查格式與長度)</param>
        /// <returns>錯誤訊息，若無錯誤回傳空字串</returns>
        private string ValidateFormData(bool isFinalized)
        {
            List<string> errors = new List<string>();

            // ==============================================================================
            // 1. 一般資料
            // ==============================================================================
            // 調查人: 必填(定稿時), Max 50字
            if (!CheckStringLength(TextBox_surveyor.Text, 100, isFinalized))
                errors.Add("調查人姓名 (必填且長度須小於50字)");

            // 樹牌狀態: 必填
            string treeSignVal = DropDownList_treeSignStatus.SelectedValue;
            if (isFinalized && string.IsNullOrWhiteSpace(treeSignVal))
                errors.Add("樹牌狀態 (必填)");
            else if (!string.IsNullOrWhiteSpace(treeSignVal))
            {
                // 檢查是否為有效的 int，且存在於 Enum 中
                // 因為 enum_treeSignStatus 的 Value 是數字 (1, 2, 3)
                if (!int.TryParse(treeSignVal, out int tsId) || !Enum.IsDefined(typeof(enum_treeSignStatus), tsId))
                {
                    errors.Add("樹牌狀態 (選項數值不合法)");
                }
            }

            // ==============================================================================
            // 2. 數值與範圍檢查
            // ==============================================================================

            // 緯度 (-90 ~ 90)
            if (!CheckDecimalRange(TextBox_latitude.Text, -90, 90, isFinalized))
                errors.Add("緯度 (格式錯誤或超出範圍 -90~90)");

            // 經度 (-180 ~ 180)
            if (!CheckDecimalRange(TextBox_longitude.Text, -180, 180, isFinalized))
                errors.Add("經度 (格式錯誤或超出範圍 -180~180)");

            // 樹高: decimal(8,2) -> Max 999999.99.99
            if (!CheckDecimalRange(TextBox_treeHeight.Text, 0, 999999.99, isFinalized))
                errors.Add("樹高 (格式錯誤或須介於 0~999999.99)");

            // 樹冠面積: decimal(8,2) -> Max 999999.99.99 
            if (!CheckDecimalRange(TextBox_canopyArea.Text, 0, 999999.99, isFinalized))
                errors.Add("樹冠投影面積 (格式錯誤或須介於 0~999999.99)");

            // 米圍/米徑...: 逗號分隔數字
            // 這裡的長度檢查比較難做精確，因為是字串串接，主要檢查是否為數字格式
            if (!CheckMultiNumberFormat(TextBox_girth100.Text, isFinalized)) errors.Add("米圍 (格式錯誤)");
            if (!CheckMultiNumberFormat(TextBox_diameter100.Text, isFinalized)) errors.Add("米徑 (格式錯誤)");
            if (!CheckMultiNumberFormat(TextBox_girth130.Text, isFinalized)) errors.Add("胸圍 (格式錯誤)");
            if (!CheckMultiNumberFormat(TextBox_diameter130.Text, isFinalized)) errors.Add("胸徑 (格式錯誤)");

            // 測量備註: Max 200字
            if (!CheckStringLength(TextBox_measureNote.Text, 200, false))
                errors.Add("測量備註 (長度須小於200字)");

            // ==============================================================================
            // 3. 重大病蟲害 (備註檢查)
            // ==============================================================================
            // 勾選其他時必填，且長度限制 200
            if (CheckBox_diseaseOther.Checked)
            {
                if (isFinalized && string.IsNullOrWhiteSpace(TextBox_diseaseOtherNote.Text))
                    errors.Add("勾選「其他重大病害」時，備註欄位為必填");
            }
            if (!CheckStringLength(TextBox_diseaseOtherNote.Text, 200, false))
                errors.Add("重大病害備註 (長度須小於200字)");

            // ==============================================================================
            // 4. 一般病蟲害 (文字描述)
            // ==============================================================================
            // 假設資料庫欄位是 nvarchar(100)
            int generalLen = 200;
            if (!CheckStringLength(TextBox_generalPestRoot.Text, generalLen, false)) errors.Add($"一般蟲害-根系 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalPestBase.Text, generalLen, false)) errors.Add($"一般蟲害-基部 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalPestTrunk.Text, generalLen, false)) errors.Add($"一般蟲害-主幹 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalPestBranch.Text, generalLen, false)) errors.Add($"一般蟲害-枝幹 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalPestCrown.Text, generalLen, false)) errors.Add($"一般蟲害-樹冠 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalPestOther.Text, generalLen, false)) errors.Add($"一般蟲害-其他 (長度須小於{generalLen}字)");

            if (!CheckStringLength(TextBox_generalDiseaseRoot.Text, generalLen, false)) errors.Add($"一般病害-根系 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalDiseaseBase.Text, generalLen, false)) errors.Add($"一般病害-基部 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalDiseaseTrunk.Text, generalLen, false)) errors.Add($"一般病害-主幹 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalDiseaseBranch.Text, generalLen, false)) errors.Add($"一般病害-枝幹 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalDiseaseCrown.Text, generalLen, false)) errors.Add($"一般病害-樹冠 (長度須小於{generalLen}字)");
            if (!CheckStringLength(TextBox_generalDiseaseOther.Text, generalLen, false)) errors.Add($"一般病害-其他 (長度須小於{generalLen}字)");

            if (!CheckStringLength(TextBox_pestOtherNote.Text, 200, false)) errors.Add("病蟲害其他備註 (長度須小於200字)");

            // ==============================================================================
            // 5. 生長外觀細節 (數值 0~100, 備註 Max 200)
            // ==============================================================================
            // 根系
            if (!CheckDecimalRange(TextBox_rootDecayPercent.Text, 0, 100, isFinalized)) errors.Add("根系腐朽百分比 (0~100)");
            if (!CheckDecimalRange(TextBox_rootCavityMaxDiameter.Text, 0, 9999.99, false)) errors.Add("根系空洞最大徑 (0~9999.99)");
            if (!CheckDecimalRange(TextBox_rootWoundMaxDiameter.Text, 0, 9999.99, false)) errors.Add("根系傷口最大徑 (0~9999.99)");
            if (!CheckStringLength(TextBox_rootOtherNote.Text, 200, false)) errors.Add("根系備註 (長度須小於200字)");

            // 基部
            if (!CheckDecimalRange(TextBox_baseDecayPercent.Text, 0, 100, isFinalized)) errors.Add("基部腐朽百分比 (0~100)");
            if (!CheckDecimalRange(TextBox_baseCavityMaxDiameter.Text, 0, 9999.99, false)) errors.Add("基部空洞最大徑 (0~9999.99)");
            if (!CheckDecimalRange(TextBox_baseWoundMaxDiameter.Text, 0, 9999.99, false)) errors.Add("基部傷口最大徑 (0~9999.99)");
            if (!CheckStringLength(TextBox_baseOtherNote.Text, 200, false)) errors.Add("基部備註 (長度須小於200字)");

            // 主幹
            if (!CheckDecimalRange(TextBox_trunkDecayPercent.Text, 0, 100, isFinalized)) errors.Add("主幹腐朽百分比 (0~100)");
            if (!CheckDecimalRange(TextBox_trunkCavityMaxDiameter.Text, 0, 9999.99, false)) errors.Add("主幹空洞最大徑 (0~9999.99)");
            if (!CheckDecimalRange(TextBox_trunkWoundMaxDiameter.Text, 0, 9999.99, false)) errors.Add("主幹傷口最大徑 (0~9999.99)");
            if (!CheckStringLength(TextBox_trunkOtherNote.Text, 200, false)) errors.Add("主幹備註 (長度須小於200字)");

            // 枝幹
            if (!CheckDecimalRange(TextBox_branchDecayPercent.Text, 0, 100, isFinalized)) errors.Add("枝幹腐朽百分比 (0~100)");
            if (!CheckDecimalRange(TextBox_branchCavityMaxDiameter.Text, 0, 9999.99, false)) errors.Add("枝幹空洞最大徑 (0~9999.99)");
            if (!CheckDecimalRange(TextBox_branchWoundMaxDiameter.Text, 0, 9999.99, false)) errors.Add("枝幹傷口最大徑 (0~9999.99)");
            if (!CheckStringLength(TextBox_branchOtherNote.Text, 200, false)) errors.Add("枝幹備註 (長度須小於200字)");

            // 樹冠
            if (!CheckDecimalRange(TextBox_crownLeafCoveragePercent.Text, 0, 100, isFinalized)) errors.Add("樹冠葉生長覆蓋度 (0~100)");
            if (!CheckDecimalRange(TextBox_crownDeadBranchPercent.Text, 0, 100, isFinalized)) errors.Add("一般枯枝百分比 (0~100)");
            if (!CheckStringLength(TextBox_crownOtherNote.Text, 200, false)) errors.Add("樹冠備註 (長度須小於200字)");

            if (!CheckStringLength(TextBox_growthNote.Text, 200, false)) errors.Add("生長其他問題詳加備註 (長度須小於200字)");

            // ==============================================================================
            // 6. 修剪與支撐
            // ==============================================================================
            string pruningVal = DropDownList_pruningWrongDamage.SelectedValue;
            if (isFinalized && string.IsNullOrWhiteSpace(pruningVal))
                errors.Add("錯誤修剪傷害 (必填)");
            else if (!string.IsNullOrWhiteSpace(pruningVal))
            {
                // 因為 enum_pruningDamageType 的 Value 是文字 ("截幹"...)
                if (!Enum.IsDefined(typeof(enum_pruningDamageType), pruningVal))
                {
                    errors.Add("錯誤修剪傷害 (選項數值不合法)");
                }
            }

            if (!CheckStringLength(TextBox_pruningOtherNote.Text, 200, false)) errors.Add("修剪備註 (長度須小於200字)");

            // 支架數量 (必須是整數 >= 0)
            if (!CheckIntRange(TextBox_supportCount.Text, 0, int.MaxValue, isFinalized))
                errors.Add("支架數量 (須為正整數或0)");

            if (!CheckStringLength(TextBox_supportOtherNote.Text, 200, false)) errors.Add("支撐備註 (長度須小於200字)");

            // ==============================================================================
            // 7. 棲地與土壤
            // ==============================================================================
            if (!CheckDecimalRange(TextBox_siteCementPercent.Text, 0, 100, isFinalized)) errors.Add("水泥鋪面 (0~100)");
            if (!CheckDecimalRange(TextBox_siteAsphaltPercent.Text, 0, 100, isFinalized)) errors.Add("柏油鋪面 (0~100)");
            if (!CheckStringLength(TextBox_siteOtherNote.Text, 200, false)) errors.Add("立地備註 (長度須小於200字)");

            // 土壤數值 (文字型態，Max 50)
            if (!CheckStringLength(TextBox_soilPh.Text, 50, isFinalized)) errors.Add("土壤酸鹼度 (必填且長度須小於50字)");
            if (!CheckStringLength(TextBox_soilOrganicMatter.Text, 50, isFinalized)) errors.Add("有機質含量 (必填且長度須小於50字)");
            if (!CheckStringLength(TextBox_soilEc.Text, 50, isFinalized)) errors.Add("電導度 (必填且長度須小於50字)");

            // ==============================================================================
            // 8. 管理建議
            // ==============================================================================
            //if (!CheckStringLength(TextBox_managementStatus.Text, 100, isFinalized)) errors.Add("管理情況 (必填且長度須小於100字)");
            string priorityVal = DropDownList_priority.SelectedValue;
            if (isFinalized && string.IsNullOrWhiteSpace(priorityVal)) 
                errors.Add("建議處理優先順序 (必填)");
            else if (!string.IsNullOrWhiteSpace(priorityVal))
            {
                // 因為 enum_treatmentPriority 的 Value 是文字 ("緊急處理"...)
                if (!Enum.IsDefined(typeof(enum_treatmentPriority), priorityVal))
                {
                    errors.Add("建議處理優先順序 (選項數值不合法)");
                }
            }

            //if (!CheckStringLength(TextBox_treatmentDescription.Text, 500, isFinalized)) errors.Add("處理情形說明 (必填且長度須小於500字)");
            // ==============================================================================
            // 9. 照片檢查 (數量、格式、大小)
            // ==============================================================================

            // --- 步驟 A: 準備刪除清單 ---
            HashSet<string> deletedIds = new HashSet<string>();
            if (!string.IsNullOrEmpty(HiddenField_deletedPhotoIds.Value))
            {
                var ids = HiddenField_deletedPhotoIds.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in ids) deletedIds.Add(id.Trim());
            }

            // --- 步驟 B: 計算「DB 有效照片數」 ---
            int dbValidCount = 0;
            // 嘗試解析目前 Session 中的 ID
            if (int.TryParse(this.setHealthID, out int hid) && hid > 0)
            {
                var list = system_health.GetHealthPhotos(hid);

                // DB 裡有的照片，且「不在」刪除清單裡的，才算有效
                dbValidCount = list.Count(p => !deletedIds.Contains(p.photoID.ToString()));
            }

            // --- 步驟 C: 計算「暫存 (HiddenField) 有效照片數」---
            int tempValidCount = 0;
            string existingJson = HiddenField_existingPhotosData.Value;
            if (!string.IsNullOrEmpty(existingJson))
            {
                try
                {
                    var jsonList = JsonConvert.DeserializeObject<List<PhotoJsonModel>>(existingJson);
                    if (jsonList != null)
                    {
                        // 邏輯：
                        // 1. Key 是負數 (代表是暫存檔)
                        // 2. Key 不在刪除清單裡
                        tempValidCount = jsonList.Count(p =>
                            long.TryParse(p.key, out long k) && k < 0 &&
                            !deletedIds.Contains(p.key));
                    }
                }
                catch { }
            }

            // --- 步驟 D: 計算「這次新上傳」 ---
            int newUploadCount = FileUpload_pendingPhotos.HasFiles ? FileUpload_pendingPhotos.PostedFiles.Count : 0;

            // --- 步驟 E: 總結算 ---
            int finalCount = dbValidCount + tempValidCount + newUploadCount;

            if (finalCount > 5)
            {
                int existingCount = dbValidCount + tempValidCount;
                errors.Add($"照片數量限制為 5 張 (現有 {existingCount} + 新增 {newUploadCount} = {finalCount} 張)。");
            }

            // --- 步驟 F: 檢查新檔案格式 (維持您原本邏輯) ---
            if (FileUpload_pendingPhotos.HasFiles)
            {
                foreach (HttpPostedFile file in FileUpload_pendingPhotos.PostedFiles)
                {
                    // 檢查副檔名 
                    string ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                    if (!PhotoAllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    {

                        string allowedExtMsg = string.Join(", ", PhotoAllowedExtensions);
                        errors.Add($"照片格式錯誤：{file.FileName} (僅支援 {allowedExtMsg})");
                    }

                    // 檢查大小
                    if (file.ContentLength > PhotoMaxSizeBytes)
                    {
                        double sizeMB = (double)file.ContentLength / (1024 * 1024);
                        errors.Add($"照片大小超限：{file.FileName} ({sizeMB:0.00} MB，上限為 10 MB)");
                    }
                }
            }

            // ==============================================================================
            // 10. 附件檢查 (格式、大小) 
            // ==============================================================================
            if (FileUpload_attachment.HasFile)
            {
                HttpPostedFile file = FileUpload_attachment.PostedFile;
                string ext = System.IO.Path.GetExtension(file.FileName).ToLower();

                // 檢查副檔名
                if (!AttachAllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                {
                    errors.Add($"附件格式錯誤：{file.FileName} (僅支援 .zip)");
                }

                // 檢查大小 (30MB = 30 * 1024 * 1024 bytes)
                if (file.ContentLength > AttachMaxSizeBytes)
                {
                    double sizeMB = (double)file.ContentLength / (1024 * 1024);
                    errors.Add($"附件大小超限：{file.FileName} ({sizeMB:0.00} MB，上限為 {AttachMaxSizeBytes} MB)");
                }
            }

            // ==============================================================================
            // 回傳
            // ==============================================================================
            if (errors.Count > 0)
            {
                string title = isFinalized ? "定稿失敗，資料不完整或格式錯誤：" : "儲存失敗，輸入格式有誤：";
                return $"<strong>{title}</strong><br/><ul class='mb-0 text-start'>" +
                       string.Join("", errors.Select(x => $"<li>{x}</li>")) +
                       "</ul>";
            }

            return "";
        }

        private bool CheckStringLength(string input, int maxLength, bool isRequired)
        {
            input = input?.Trim();

            // 1. 必填檢查
            if (string.IsNullOrEmpty(input))
            {
                // 如果必填(true)且為空 -> false(失敗)
                // 如果非必填(false)且為空 -> true(通過)
                return !isRequired;
            }

            // 2. 長度檢查 (無論是否必填，只要有值就不能爆掉)
            return input.Length <= maxLength;
        }

        private bool CheckDecimalRange(string input, double min, double max, bool isRequired)
        {
            input = input?.Trim();
            if (string.IsNullOrWhiteSpace(input)) return !isRequired; // 空值：必填就False，非必填就True
            if (decimal.TryParse(input, out decimal val)) return val >= (decimal)min && val <= (decimal)max;
            return false; // 格式錯誤 (如 "ABC")
        }

        private bool CheckIntRange(string input, int min, int max, bool isRequired)
        {
            input = input?.Trim();
            if (string.IsNullOrWhiteSpace(input)) return !isRequired;
            if (int.TryParse(input, out int val)) return val >= min && val <= max;
            return false;
        }

        private bool CheckMultiNumberFormat(string input, bool isRequired)
        {
            input = input?.Trim();

            // 如果是空值
            if (string.IsNullOrWhiteSpace(input)) return !isRequired;

            // 如果有值，必須格式正確 (逗號分隔的數字)
            string[] parts = input.Split(',');
            foreach (string part in parts)
            {
                string p = part.Trim();
                if (string.IsNullOrEmpty(p)) continue; // 允許 "10, , 20"

                if (!decimal.TryParse(p, out _))
                {
                    return false; // 只要有一個不是數字就失敗
                }
            }
            return true;
        }

        private decimal? ParseDec(string s)
        {
            if (decimal.TryParse(s, out decimal v)) return v;
            return null;
        }

        private int? ParseInt(string s)
        {
            if (int.TryParse(s, out int v)) return v;
            return null;
        }

        // ==========================================================================
        //  照片處理 (Photos)
        // ==========================================================================

        private void ProcessPhotos(int healthId, int accountId)
        {
            
            // 解析前端傳來的 Metadata 
            string jsonMeta = HiddenField_photoMetadata.Value;
            List<PhotoJsonModel> metaList = new List<PhotoJsonModel>();
            if (!string.IsNullOrEmpty(jsonMeta))
            {
                try { metaList = JsonConvert.DeserializeObject<List<PhotoJsonModel>>(jsonMeta) ?? new List<PhotoJsonModel>(); } catch { }
            }

            // 解析刪除清單 
            HashSet<string> deletedKeySet = new HashSet<string>();
            string deletedIdsStr = HiddenField_deletedPhotoIds.Value;
            if (!string.IsNullOrEmpty(deletedIdsStr))
            {
                var rawIds = deletedIdsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in rawIds) deletedKeySet.Add(id.Trim());
            }

            // 建立 Metadata 查找表
            // Lookup A: 給「舊照片」與「暫存檔」用 (用 Key 找 Caption)
            var captionByKeyLookup = metaList.ToDictionary(m => m.key, m => m.caption);

            // Lookup B: 給「新上傳檔案 (FileUpload)」用 (用 檔名 找 Metadata Queue)
            // 只找 Key < 0 的新資料，並依檔名分組排隊，解決同名對應問題
            var newFileMetaLookup = metaList
                .Where(m => int.TryParse(m.key, out int k) && k < 0)
                .GroupBy(m => Path.GetFileName(m.fileName ?? ""))
                .ToDictionary(g => g.Key, g => new Queue<PhotoJsonModel>(g));

            // 1-4. 準備目標資料夾
            string targetVirtualDir = $@"~/_file/health/img/{healthId}/";
            string targetPhysicalDir = Server.MapPath(targetVirtualDir);
            if (!Directory.Exists(targetPhysicalDir)) Directory.CreateDirectory(targetPhysicalDir);


            //刪除照片
            List<int> dbIdsToDelete = new List<int>();
            foreach (string key in deletedKeySet)
            {
                if (int.TryParse(key, out int pid) && pid > 0)
                {
                    dbIdsToDelete.Add(pid);
                }
            }
            if (dbIdsToDelete.Count > 0)
            {
                system_health.DeleteHealthPhotos(healthId, dbIdsToDelete, accountId);
            }


            //更新備註
            List<TreeHealthPhoto> updates = new List<TreeHealthPhoto>();
            foreach (var item in metaList)
            {
                // 條件：Key > 0 且 不在刪除名單中
                if (int.TryParse(item.key, out int pid) && pid > 0 && !deletedKeySet.Contains(item.key))
                {
                    updates.Add(new TreeHealthPhoto
                    {
                        photoID = pid,
                        caption = item.caption
                    });
                }
            }
            if (updates.Count > 0)
            {
                system_health.UpdateHealthPhotoCaptions(healthId, updates);
            }


            //處理暫存檔
            string existingJson = HiddenField_existingPhotosData.Value;
            if (!string.IsNullOrEmpty(existingJson))
            {
                try
                {
                    var existingList = JsonConvert.DeserializeObject<List<PhotoJsonModel>>(existingJson);
                    if (existingList != null)
                    {
                        foreach (var p in existingList)
                        {
                            // 條件：是暫存檔 (Key < 0) 且 沒有被刪除
                            if (long.TryParse(p.key, out long key) && key < 0 && !deletedKeySet.Contains(p.key))
                            {
                                // p.filePath 是 "~/_file/health/temp/GUID_xxx.jpg"
                                string tempPath = Server.MapPath(p.filePath);

                                if (File.Exists(tempPath))
                                {
                                    // 取得最新的 Caption
                                    string caption = captionByKeyLookup.ContainsKey(p.key) ? captionByKeyLookup[p.key] : p.caption;
                                    string baseFileName = $"{DateTime.Now.Ticks}_{p.fileName}";
                                    string fullPath = Path.Combine(targetPhysicalDir, baseFileName);
                                    string finalSaveName = baseFileName;

                                    int counter = 1;
                                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
                                    string fileExt = Path.GetExtension(baseFileName);

                                    while (File.Exists(fullPath))
                                    {
                                        finalSaveName = $"{fileNameWithoutExt}_({counter}){fileExt}";
                                        fullPath = Path.Combine(targetPhysicalDir, finalSaveName);
                                        counter++;
                                    }
                                    try
                                    {
                                        File.Move(tempPath, fullPath);
                                        TreeHealthPhoto photo = new TreeHealthPhoto
                                        {
                                            healthID = healthId,
                                            fileName = p.fileName,
                                            filePath = targetVirtualDir + finalSaveName,
                                            fileSize = (int)new FileInfo(fullPath).Length,
                                            caption = caption
                                        };
                                        system_health.InsertHealthPhoto(photo, accountId);
                                    }
                                    catch (Exception ex)
                                    {
                                        // 搬移失敗處理 (Log or Ignore)
                                    }
                                }
                            }
                        }
                    }
                }
                catch { /* JSON 解析失敗忽略 */ }
            }


            //處理新的上傳
            if (FileUpload_pendingPhotos.HasFiles)
            {
                var files = FileUpload_pendingPhotos.PostedFiles;
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    HttpPostedFile file = files[i];
                    // 找出對應的 Metadata (Caption)
                    // 使用 Queue 模式，避免同檔名對應錯誤
                    string caption = "";
                    string currentKey = "";

                    var clientFileName = Path.GetFileName(file.FileName);
                    if (newFileMetaLookup.ContainsKey(clientFileName) && newFileMetaLookup[clientFileName].Count > 0)
                    {
                        // 取出並消耗一個 Metadata
                        var meta = newFileMetaLookup[clientFileName].Dequeue();
                        caption = meta.caption;
                        currentKey = meta.key;

                        // 如果這個 Key 已經被標記刪除，這張檔案就不存了！
                        if (deletedKeySet.Contains(currentKey)) continue;
                    }

                    // --- 處理檔名重複邏輯 ---
                    string baseFileName = $"{DateTime.Now.Ticks}_{file.FileName}";
                    string fullPath = Path.Combine(targetPhysicalDir, baseFileName);
                    string finalSaveName = baseFileName;

                    int counter = 1;
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
                    string fileExt = Path.GetExtension(baseFileName);

                    while (File.Exists(fullPath))
                    {
                        finalSaveName = $"{fileNameWithoutExt}_({counter}){fileExt}";
                        fullPath = Path.Combine(targetPhysicalDir, finalSaveName);
                        counter++;
                    }

                    // 存檔 
                    file.SaveAs(fullPath);

                    // 寫入 DB 
                    TreeHealthPhoto photo = new TreeHealthPhoto
                    {
                        healthID = healthId,
                        fileName = clientFileName,
                        filePath = targetVirtualDir + finalSaveName,
                        fileSize = file.ContentLength,
                        caption = caption
                    };
                    system_health.InsertHealthPhoto(photo, accountId);
                }
            }
        }

        // ==========================================================================
        //  附件處理 (Attachments)
        // ==========================================================================
        private void ProcessAttachment(int healthId, int accountId)
        {
            // 正式資料夾路徑
            string finalDirVirtual = $@"~/_file/health/doc/{healthId}/";
            string finalDirPhysical = Server.MapPath(finalDirVirtual);
            if (!Directory.Exists(finalDirPhysical)) Directory.CreateDirectory(finalDirPhysical);

            bool isDeleted = HiddenField_isFileDeleted.Value == "true";

            // 解析 HiddenField (取得暫存檔資訊) - 只有「未刪除」才需要解析
            AttachmentJsonModel jsonFile = null;
            if (!isDeleted && !string.IsNullOrEmpty(HiddenField_existingFileData.Value))
            {
                try { jsonFile = JsonConvert.DeserializeObject<AttachmentJsonModel>(HiddenField_existingFileData.Value); }
                catch { }
            }

            // ----------------------------------------------------------------------
            // 決定資料來源與操作模式
            // ----------------------------------------------------------------------
            bool hasNewFile = false;
            string sourceOriginalName = ""; // 使用者看到的原始檔名
            string tempPathToMove = null;   // 若是 temp 來源，這裡放實體路徑

            // 模式 A: 來自 FileUpload (優先)
            if (FileUpload_attachment.HasFile)
            {
                sourceOriginalName = Path.GetFileName(FileUpload_attachment.FileName);
                hasNewFile = true;
            }
            // 模式 B: 來自 Temp 暫存檔
            else if (jsonFile != null && jsonFile.isTemp && !string.IsNullOrEmpty(jsonFile.filePath))
            {
                string tempPhysicalPath = Server.MapPath(jsonFile.filePath);
                if (File.Exists(tempPhysicalPath))
                {
                    sourceOriginalName = Path.GetFileName(jsonFile.fileName);
                    tempPathToMove = tempPhysicalPath;
                    hasNewFile = true;
                }
            }

            // ----------------------------------------------------------------------
            // Case 1: 有新檔（上傳或 temp） 
            // ----------------------------------------------------------------------
            if (hasNewFile)
            {
                // 刪除舊 DB 紀錄（你是刪全部，OK）
                var oldAttachments = system_health.GetHealthAttachments(healthId);
                foreach (var oldAtt in oldAttachments)
                    system_health.DeleteHealthAttachment(healthId, oldAtt.attachmentID, accountId);

                // 產生安全唯一檔名
                string baseFileName = $"{DateTime.Now.Ticks}_{sourceOriginalName}";
                string finalPath = Path.Combine(finalDirPhysical, baseFileName);
                string finalNameOnly = baseFileName;

                int counter = 1;
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
                string fileExt = Path.GetExtension(baseFileName);

                while (File.Exists(finalPath))
                {
                    finalNameOnly = $"{fileNameWithoutExt}_({counter}){fileExt}";
                    finalPath = Path.Combine(finalDirPhysical, finalNameOnly);
                    counter++;
                }

                // 存檔/搬移
                if (FileUpload_attachment.HasFile)
                {
                    FileUpload_attachment.SaveAs(finalPath);
                }
                else if (tempPathToMove != null)
                {
                    File.Move(tempPathToMove, finalPath);
                }

                // 寫 DB（fileName 放原始檔名，filePath 放系統路徑）
                var att = new TreeHealthAttachment
                {
                    healthID = healthId,
                    fileName = sourceOriginalName,
                    filePath = finalDirVirtual + finalNameOnly,
                    fileSize = (int)new FileInfo(finalPath).Length,
                    description = "健檢附件"
                };
                system_health.InsertHealthAttachment(att, accountId);

                return;
            }

            // ----------------------------------------------------------------------
            // Case 2: 沒新檔，但標記刪除 -> 刪除舊附件
            // ----------------------------------------------------------------------
            if (isDeleted)
            {
                var oldAttachments = system_health.GetHealthAttachments(healthId);
                foreach (var oldAtt in oldAttachments)
                    system_health.DeleteHealthAttachment(healthId, oldAtt.attachmentID, accountId);

                return;
            }

            // Case 3: 沒新檔也沒刪除 -> 不動（保留舊附件）
        }

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {
            base.ReturnState();
        }
    }
}