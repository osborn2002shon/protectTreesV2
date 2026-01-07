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

        [Serializable]
        public class AttachmentJsonModel
        {
            public string fileName { get; set; }
            public string filePath { get; set; }
            public bool isTemp { get; set; } 
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
                    var photoList = record.photos.Select(p => new
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
                    ShowMessage("系統提示", "「調查日期」為必填欄位。");
                    return;
                }

                if (system_health.CheckSurveyDateDuplicate(this.CurrentTreeID, surveyDate, this.CurrentHealthID))
                {
                    ShowMessage("系統提示", $"該樹木在 {surveyDate:yyyy/MM/dd} 已經有其他的調查紀錄，請勿重複建立。");
                    return;
                }

                // 定稿驗證
                string errorMsg = ValidateFormData(isFinalized);
                if (!string.IsNullOrEmpty(errorMsg))
                {
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
                Response.Redirect("edit.aspx", false); 
            }
            catch (Exception ex)
            {
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

            int dbPhotoCount = 0;
            List<int> dbPhotoIds = new List<int>(); // 用來存 DB 裡真正的 ID

            // 嘗試解析目前 Session 中的 ID
            if (int.TryParse(this.setHealthID, out int hid) && hid > 0)
            {
                var list = system_health.GetHealthPhotos(hid);
                dbPhotoCount = list.Count;
                // 取出這筆健檢紀錄下，所有真正的照片 ID
                dbPhotoIds = list.Select(p => p.photoID).ToList();
            }

            // 計算 "有效" 刪除數量
            int validDeleteCount = 0;
            if (!string.IsNullOrEmpty(HiddenField_deletedPhotoIds.Value))
            {
                // 1. 解析前端傳來的 ID 字串
                string[] rawIds = HiddenField_deletedPhotoIds.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string rawId in rawIds)
                {
                    if (int.TryParse(rawId, out int id))
                    {
                        // 防止有人亂傳假 ID 來騙過數量檢查
                        if (dbPhotoIds.Contains(id))
                        {
                            validDeleteCount++;
                        }
                    }
                }
            }

            int newUploadCount = FileUpload_pendingPhotos.HasFiles ? FileUpload_pendingPhotos.PostedFiles.Count : 0;

            // 計算結果 
            int finalCount = (dbPhotoCount - validDeleteCount) + newUploadCount;

            if (finalCount > 5)
            {
                errors.Add($"照片數量限制為 5 張 (現有{dbPhotoCount} - 刪除{validDeleteCount} + 新增{newUploadCount} = {finalCount} 張)。請確認您的刪除操作是否正確。");
            }
            if (FileUpload_pendingPhotos.HasFiles)
            {
                foreach (HttpPostedFile file in FileUpload_pendingPhotos.PostedFiles)
                {
                    string ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                    // 檢查副檔名
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                    {
                        errors.Add($"照片格式錯誤：{file.FileName} (僅支援 .jpg, .jpeg, .png)");
                    }
                    // 檢查大小 (10MB = 10 * 1024 * 1024 bytes)
                    if (file.ContentLength > 10 * 1024 * 1024)
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
                if (ext != ".zip")
                {
                    errors.Add($"附件格式錯誤：{file.FileName} (僅支援 .zip)");
                }

                // 檢查大小 (30MB = 30 * 1024 * 1024 bytes)
                if (file.ContentLength > 30 * 1024 * 1024)
                {
                    double sizeMB = (double)file.ContentLength / (1024 * 1024);
                    errors.Add($"附件大小超限：{file.FileName} ({sizeMB:0.00} MB，上限為 30 MB)");
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
            // -----------------------------------------------------------
            // 批次刪除 (Batch Delete)
            // -----------------------------------------------------------
            string deletedIdsStr = HiddenField_deletedPhotoIds.Value;
            if (!string.IsNullOrEmpty(deletedIdsStr))
            {
                List<int> idsToDelete = new List<int>();
                string[] rawIds = deletedIdsStr.Split(',');

                foreach (string idStr in rawIds)
                {
                    if (int.TryParse(idStr, out int pid) && pid > 0)
                    {
                        idsToDelete.Add(pid);
                    }
                }

                // 如果有要刪除的 ID，一次送進 Service 處理
                if (idsToDelete.Count > 0)
                {
                    system_health.DeleteHealthPhotos(healthId, idsToDelete, accountId);
                }
            }

            // -----------------------------------------------------------
            // 解析前端 Metadata (用於更新備註與對應新上傳檔案)
            // -----------------------------------------------------------
            string jsonMeta = HiddenField_photoMetadata.Value;
            if (string.IsNullOrEmpty(jsonMeta)) return;

            var metaList = JsonConvert.DeserializeObject<List<PhotoMeta>>(jsonMeta);

            // -----------------------------------------------------------
            // 批次更新舊照片備註 (Batch Update Captions)
            // -----------------------------------------------------------
            List<TreeHealthPhoto> updates = new List<TreeHealthPhoto>();

            foreach (var item in metaList)
            {
                // key > 0 代表是資料庫已存在的舊照片
                if (int.TryParse(item.key, out int pid) && pid > 0)
                {
                    updates.Add(new TreeHealthPhoto
                    {
                        photoID = pid,
                        caption = item.caption
                    });
                }
            }

            // 若有變更，一次送出更新
            if (updates.Count > 0)
            {
                system_health.UpdateHealthPhotoCaptions(healthId, updates);
            }

            // -----------------------------------------------------------
            // 處理新照片上傳 (Insert)
            // -----------------------------------------------------------
            if (FileUpload_pendingPhotos.HasFiles)
            {
                // 路徑結構：~/_file/health/img/{healthID}/
                string virtualDir = $@"~/_file/health/img/{healthId}/";
                string physicalPath = Server.MapPath(virtualDir);

                // 如果資料夾不存在則建立
                if (!Directory.Exists(physicalPath)) Directory.CreateDirectory(physicalPath);

                var postedFiles = FileUpload_pendingPhotos.PostedFiles;
                int fileCount = postedFiles.Count;

                for (int i = fileCount - 1; i >= 0; i--)
                {
                    HttpPostedFile file = postedFiles[i];
                    // 從 metaList 找出對應的 caption (key < 0 且 fileName 吻合)
                    // 前端 JS 生成的新照片 key 都是負數
                    var meta = metaList.FirstOrDefault(m =>
                        (int.TryParse(m.key, out int k) && k < 0) &&
                        m.fileName == file.FileName);

                    string caption = meta != null ? meta.caption : "";

                    string baseFileName = $"{DateTime.Now.Ticks}_{file.FileName}";
                    string fullPath = Path.Combine(physicalPath, baseFileName);

                    // 最後確定的檔名
                    string finalSaveName = baseFileName;

                    //防止檔名重複(若存在則自動改名)
                    int counter = 1;
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
                    string fileExt = Path.GetExtension(baseFileName);

                    while (File.Exists(fullPath))
                    {
                        // 發現重複！改名為: {Ticks}_{OriginalName}_(1).jpg
                        finalSaveName = $"{fileNameWithoutExt}_({counter}){fileExt}";
                        fullPath = Path.Combine(physicalPath, finalSaveName);
                        counter++;
                    }
                    // -----------------------------------------------------------

                    // 存檔
                    file.SaveAs(fullPath);

                    // 寫入 DB
                    TreeHealthPhoto photo = new TreeHealthPhoto
                    {
                        healthID = healthId,
                        fileName = file.FileName,        // 原始檔名
                        filePath = virtualDir + finalSaveName, // 相對路徑
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

            // 解析 HiddenField (取得暫存檔資訊)
            AttachmentJsonModel jsonFile = null;
            if (!string.IsNullOrEmpty(HiddenField_existingFileData.Value))
            {
                try { jsonFile = JsonConvert.DeserializeObject<AttachmentJsonModel>(HiddenField_existingFileData.Value); } catch { }
            }

            // ----------------------------------------------------------------------
            //  決定資料來源與操作模式
            // ----------------------------------------------------------------------
            bool hasNewFile = false;
            string sourceFileName = "";

            // 模式 A: 來自 FileUpload (優先)
            if (FileUpload_attachment.HasFile)
            {
                sourceFileName = FileUpload_attachment.FileName;
                hasNewFile = true;
            }
            // 模式 B: 來自 Temp 暫存檔
            else if (jsonFile != null && jsonFile.isTemp && !string.IsNullOrEmpty(jsonFile.filePath))
            {
                string tempPath = Server.MapPath(jsonFile.filePath);
                if (File.Exists(tempPath))
                {
                    sourceFileName = jsonFile.fileName;
                    hasNewFile = true;
                }
            }

            // ----------------------------------------------------------------------
            // 執行處理 (如有新檔 -> 取代舊檔)
            // ----------------------------------------------------------------------
            if (hasNewFile)
            {
                //刪除該樹的所有舊附件紀錄
                var oldAttachments = system_health.GetHealthAttachments(healthId);
                foreach (var oldAtt in oldAttachments)
                {
                    system_health.DeleteHealthAttachment(healthId, oldAtt.attachmentID, accountId);
                }

                // 產生安全的唯一檔名
                string baseFileName = $"{DateTime.Now.Ticks}_{sourceFileName}";
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

                // 執行存檔 
                if (FileUpload_attachment.HasFile)
                {
                    FileUpload_attachment.SaveAs(finalPath);
                }
                else
                {
                    string tempPath = Server.MapPath(jsonFile.filePath);
                    if (File.Exists(tempPath))
                    {
                        File.Move(tempPath, finalPath);
                    }
                }

                // 寫入 DB
                TreeHealthAttachment att = new TreeHealthAttachment
                {
                    healthID = healthId,
                    fileName = sourceFileName, // 顯示給使用者看的原始檔名
                    filePath = finalDirVirtual + finalNameOnly, // 系統實際儲存路徑
                    fileSize = (int)new FileInfo(finalPath).Length,
                    description = "健檢附件"
                };
                system_health.InsertHealthAttachment(att, accountId);
            }
            // ----------------------------------------------------------------------
            // 處理單純刪除 (無新檔，但標記刪除)
            // ----------------------------------------------------------------------
            else if (HiddenField_isFileDeleted.Value == "true")
            {
                var oldAttachments = system_health.GetHealthAttachments(healthId);
                foreach (var oldAtt in oldAttachments)
                {
                    system_health.DeleteHealthAttachment(healthId, oldAtt.attachmentID, accountId);
                }
            }
        }

        protected void LinkButton_cancel_Click(object sender, EventArgs e)
        {
            base.ReturnState();
        }
    }
}