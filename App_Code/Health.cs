using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using DataAccess;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;
using static protectTreesV2.Base.DataRowHelper;

namespace protectTreesV2.Health
{
    public class Health
    {
        public enum enum_healthRecordStatus
        {
            草稿 = 0,
            定稿 = 1
        }

        /// <summary>
        /// 樹牌狀態 
        /// </summary>
        public enum enum_treeSignStatus
        {
            有 = 1,
            沒有 = 2,
            毀損 = 3
        }

        /// <summary>
        /// 錯誤修剪傷害
        /// </summary>
        public enum enum_pruningDamageType
        {
            無,
            截幹,
            截頂,
            不當縮剪
        }

        /// <summary>
        /// 樹冠葉片覆蓋率定義
        /// </summary>
        public class CrownCoverageTypes
        {
            // 定義常數
            public const string LevelLow = "0~40%生長不良";
            public const string LevelMid = "40~70%尚可";
            public const string LevelHigh = "70~100%正常";

            public static readonly List<string> AllList = new List<string>
            {
                LevelLow,
                LevelMid,
                LevelHigh
            };
        }
        /// <summary>
        /// 建議處理優先順序 
        /// </summary>
        public enum enum_treatmentPriority
        {
            緊急處理,
            優先處理,
            例行養護
        }

        public class TreeHealthRecord
        {
            // ==========================================
            // 1. Primary Key & Foreign Key
            // ==========================================
            public int healthID { get; set; }
            public int treeID { get; set; }

            // ==========================================
            // 樹籍資料
            // ==========================================
            public string systemTreeNo { get; set; }
            public string agencyTreeNo { get; set; }
            public string cityName { get; set; }
            public string areaName { get; set; }
            public string speciesName { get; set; }
            public string manager { get; set; }

            // ==========================================
            // 基本資料
            // ==========================================
            public DateTime? surveyDate { get; set; }
            public string surveyor { get; set; }
            public int dataStatus { get; set; } // 0=草稿, 1=完稿
            public string memo { get; set; }
            public byte? treeSignStatus { get; set; }

            // ==========================================
            // 樹木規格
            // ==========================================
            public decimal? latitude { get; set; }
            public decimal? longitude { get; set; }
            public decimal? treeHeight { get; set; }
            public decimal? canopyArea { get; set; }

            // 樹圍與直徑
            public string girth100 { get; set; }
            public string diameter100 { get; set; }
            public string girth130 { get; set; }
            public string diameter130 { get; set; }
            public string measureNote { get; set; }

            // ==========================================
            // 主要病害 
            // ==========================================
            public bool? majorDiseaseBrownRoot { get; set; }
            public bool? majorDiseaseGanoderma { get; set; }
            public bool? majorDiseaseWoodDecayFungus { get; set; }
            public bool? majorDiseaseCanker { get; set; }
            public bool? majorDiseaseOther { get; set; }
            public string majorDiseaseOtherNote { get; set; }

            // ==========================================
            // 主要害蟲
            // ==========================================
            // 根部
            public bool? majorPestRootTunnel { get; set; }
            public bool? majorPestRootChew { get; set; }
            public bool? majorPestRootLive { get; set; }
            // 基部
            public bool? majorPestBaseTunnel { get; set; }
            public bool? majorPestBaseChew { get; set; }
            public bool? majorPestBaseLive { get; set; }
            // 樹幹
            public bool? majorPestTrunkTunnel { get; set; }
            public bool? majorPestTrunkChew { get; set; }
            public bool? majorPestTrunkLive { get; set; }
            // 枝條
            public bool? majorPestBranchTunnel { get; set; }
            public bool? majorPestBranchChew { get; set; }
            public bool? majorPestBranchLive { get; set; }
            // 樹冠
            public bool? majorPestCrownTunnel { get; set; }
            public bool? majorPestCrownChew { get; set; }
            public bool? majorPestCrownLive { get; set; }
            // 其他
            public bool? majorPestOtherTunnel { get; set; }
            public bool? majorPestOtherChew { get; set; }
            public bool? majorPestOtherLive { get; set; }

            // ==========================================
            // 一般病蟲害描述 
            // ==========================================
            public string generalPestRoot { get; set; }
            public string generalPestBase { get; set; }
            public string generalPestTrunk { get; set; }
            public string generalPestBranch { get; set; }
            public string generalPestCrown { get; set; }
            public string generalPestOther { get; set; }

            public string generalDiseaseRoot { get; set; }
            public string generalDiseaseBase { get; set; }
            public string generalDiseaseTrunk { get; set; }
            public string generalDiseaseBranch { get; set; }
            public string generalDiseaseCrown { get; set; }
            public string generalDiseaseOther { get; set; }

            public string pestOtherNote { get; set; }

            // ==========================================
            // 各部位細節檢測
            // ==========================================
            // 根部
            public decimal? rootDecayPercent { get; set; }
            public decimal? rootCavityMaxDiameter { get; set; }
            public decimal? rootWoundMaxDiameter { get; set; }
            public bool? rootMechanicalDamage { get; set; }
            public bool? rootMowingInjury { get; set; }
            public bool? rootInjury { get; set; }
            public bool? rootGirdling { get; set; }
            public string rootOtherNote { get; set; }

            // 基部
            public decimal? baseDecayPercent { get; set; }
            public decimal? baseCavityMaxDiameter { get; set; }
            public decimal? baseWoundMaxDiameter { get; set; }
            public bool? baseMechanicalDamage { get; set; }
            public bool? baseMowingInjury { get; set; }
            public string baseOtherNote { get; set; }

            // 樹幹
            public decimal? trunkDecayPercent { get; set; }
            public decimal? trunkCavityMaxDiameter { get; set; }
            public decimal? trunkWoundMaxDiameter { get; set; }
            public bool? trunkMechanicalDamage { get; set; }
            public bool? trunkIncludedBark { get; set; }
            public string trunkOtherNote { get; set; }

            // 枝條
            public decimal? branchDecayPercent { get; set; }
            public decimal? branchCavityMaxDiameter { get; set; }
            public decimal? branchWoundMaxDiameter { get; set; }
            public bool? branchMechanicalDamage { get; set; }
            public bool? branchIncludedBark { get; set; }
            public bool? branchDrooping { get; set; }
            public string branchOtherNote { get; set; }

            // 樹冠
            public string crownLeafCoveragePercent { get; set; }
            public decimal? crownDeadBranchPercent { get; set; }
            public bool? crownHangingBranch { get; set; }
            public string crownOtherNote { get; set; }

            public string growthNote { get; set; }

            // ==========================================
            // 修剪與支撐
            // ==========================================
            public string pruningWrongDamage { get; set; }
            public bool? pruningWoundHealing { get; set; }
            public bool? pruningEpiphyte { get; set; }
            public bool? pruningParasite { get; set; }
            public bool? pruningVine { get; set; }
            public string pruningOtherNote { get; set; }

            public int? supportCount { get; set; }
            public bool? supportEmbedded { get; set; }
            public string supportOtherNote { get; set; }

            // ==========================================
            // 棲地與土壤 
            // ==========================================
            public decimal? siteCementPercent { get; set; }
            public decimal? siteAsphaltPercent { get; set; }
            public bool? sitePlanter { get; set; }
            public bool? siteRecreationFacility { get; set; }
            public bool? siteDebrisStack { get; set; }
            public bool? siteBetweenBuildings { get; set; }
            public bool? siteSoilCompaction { get; set; }
            public bool? siteOverburiedSoil { get; set; }
            public string siteOtherNote { get; set; }

            public string soilPh { get; set; }
            public string soilOrganicMatter { get; set; }
            public string soilEc { get; set; }

            // ==========================================
            // 11. 管理建議 (Management)
            // ==========================================
            public string managementStatus { get; set; }
            public string priority { get; set; }
            public string treatmentDescription { get; set; }

            public int insertAccountID { get; set; }
            public DateTime insertDateTime { get; set; }
            public int? updateAccountID { get; set; }
            public DateTime? updateDateTime { get; set; }

            public List<TreeHealthPhoto> photos { get; set; }
            public List<TreeHealthAttachment> attachments { get; set; }

            // ==========================================
            // 輔助顯示屬性 
            // ==========================================

            // 顯示日期 (yyyy/MM/dd)
            public string surveyDateDisplay
                => surveyDate.HasValue ? surveyDate.Value.ToString("yyyy/MM/dd") : string.Empty;

            // 顯示狀態
            public string dataStatusText
                => dataStatus == 1 ? enum_healthRecordStatus.定稿.ToString() : enum_healthRecordStatus.草稿.ToString();

            // 顯示最後更新時間
            public string lastUpdateDisplay
            {
                get
                {
                    if (updateDateTime.HasValue)
                    {
                        return updateDateTime.Value.ToString("yyyy/MM/dd HH:mm");
                    }
                    return insertDateTime == DateTime.MinValue ? string.Empty : insertDateTime.ToString("yyyy/MM/dd HH:mm");
                }
            }

            public TreeHealthRecord()
            {
                photos = new List<TreeHealthPhoto>();
                attachments = new List<TreeHealthAttachment>();
            }
        }

        public class TreeHealthPhoto
        {
            public int photoID { get; set; }
            public int healthID { get; set; }
            public string fileName { get; set; }
            public string filePath { get; set; }
            public int? fileSize { get; set; }
            public string caption { get; set; }
            public DateTime insertDateTime { get; set; }
        }

        public class TreeHealthAttachment
        {
            public int attachmentID { get; set; }
            public int healthID { get; set; }
            public string fileName { get; set; }
            public string filePath { get; set; }
            public int? fileSize { get; set; }
            public string description { get; set; }
            public DateTime insertDateTime { get; set; }
        }

        public class TreeHealthFilter
        {
            public int? CityID { get; set; }
            public int? AreaID { get; set; }
            public string SystemTreeNo { get; set; }
            public DateTime? SurveyDateStart { get; set; }
            public DateTime? SurveyDateEnd { get; set; }
            public enum_healthRecordStatus? DataStatus { get; set; }
        }

        [Serializable]
        public class HealthMainQueryFilter
        {
            public int? cityID { get; set; }
            public int? areaID { get; set; }
            public int? speciesID { get; set; }
            public string keyword { get; set; }
            public bool onlyNoRecord { get; set; } // 僅撈取無健檢紀錄
            public bool includeDraft { get; set; } // 包含草稿
            public string sortExpression { get; set; }
            public string sortDirection { get; set; }
        }

        public class HealthMainQueryResult
        {
            public int treeID { get; set; }
            public int? healthID { get; set; }
            public string systemTreeNo { get; set; }
            public string agencyTreeNo { get; set; }
            public string agencyJurisdictionCode { get; set; }
            public string cityName { get; set; }
            public string areaName { get; set; }
            public string speciesName { get; set; }
            public string manager { get; set; }

            public TreeStatus? treeStatus { get; set; }
            public TreeEditState? editState { get; set; }

            public string treeStatusText => treeStatus.HasValue ? protectTreesV2.TreeCatalog.TreeService.GetStatusText(treeStatus.Value) : string.Empty;

            public DateTime? surveyDate { get; set; }
            public string surveyor { get; set; }
            public enum_healthRecordStatus? dataStatus { get; set; }
            public DateTime? lastUpdate { get; set; }

            public bool hasHealthRecord => healthID.HasValue;

            public bool isAdded { get; set; }
        }

        public class HealthBatchSettingResult
        {
            public int settingID { get; set; }
            public DateTime insertDateTime { get; set; }
            public int treeID { get; set; }
            public string systemTreeNo { get; set; }
            public string agencyTreeNo { get; set; }
            public string agencyJurisdictionCode { get; set; }
            public string cityName { get; set; }
            public string areaName { get; set; }
            public string speciesName { get; set; }
            public string manager { get; set; }
        }

        [Serializable]
        public class HealthRecordListFilter
        {
            public int? cityID { get; set; }
            public int? areaID { get; set; }
            public int? speciesID { get; set; }
            public string keyword { get; set; }

            public string scope { get; set; } = "Unit";

            // [新增] 日期區間
            public DateTime? dateStart { get; set; }
            public DateTime? dateEnd { get; set; }

            public string sortExpression { get; set; }
            public string sortDirection { get; set; }
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static TreeStatus ParseStatus(object value)
        {
            if (value == null || value == DBNull.Value) return TreeStatus.其他;

            string text = value.ToString();
            if (string.IsNullOrWhiteSpace(text)) return TreeStatus.其他;

            text = text.Trim();

            //if (protectTreesV2.TreeCatalog.TreeService.StatusLookupMap.TryGetValue(text, out TreeStatus status))
            //{
            //    return status;
            //}

            if (int.TryParse(text, out int numeric) && Enum.IsDefined(typeof(TreeStatus), numeric))
            {
                return (TreeStatus)numeric;
            }

            return TreeStatus.其他;
        }

        public  List<HealthMainQueryResult> GetHealthMainList(HealthMainQueryFilter filter, int currentUserID)
        {
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            // 注意：這裡使用 OUTER APPLY 來抓取「符合條件的最新一筆」
            // @includeDraft 的邏輯放在 APPLY 裡面：
            //   - 如果包含草稿：不加篩選，直接依日期排，抓最新。
            //   - 如果不含草稿：只抓 dataStatus=1 的，依日期排，抓最新。
            string baseSql = @"
                SELECT 
                    /* 樹籍基本資料 */
                    record.treeID, record.systemTreeNo, record.agencyTreeNo, record.agencyJurisdictionCode,
                    record.manager, record.treeStatus,
            
                    /* 縣市鄉鎮名稱 */
                    COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                    areaInfo.area AS areaName,
            
                    /* 樹種名稱 */
                    species.commonName AS speciesName,
            
                    /* 最新健檢紀錄欄位 (可能為 NULL) */
                    latest_health.healthID, 
                    latest_health.surveyDate, 
                    latest_health.surveyor, 
                    latest_health.dataStatus,
                    COALESCE(latest_health.updateDateTime, latest_health.insertDateTime) AS lastUpdate,
                    
                    /* 判斷是否已加入設定  */
                    CASE WHEN batch.treeID IS NOT NULL THEN 1 ELSE 0 END AS isAdded

                FROM Tree_Record record
        
                /* 抓取最新一筆健檢紀錄 */
                OUTER APPLY (
                    SELECT TOP 1 h.*
                    FROM Tree_HealthRecord h
                    WHERE h.treeID = record.treeID
                    AND h.removeDateTime IS NULL
                    /* 草稿篩選邏輯：若不包含草稿(@incDraft=0)，則只撈 dataStatus=1 */
                    AND (@incDraft = 1 OR h.dataStatus = 1)
                    ORDER BY h.surveyDate DESC
                ) latest_health
        
                /* 其他關聯表 */
                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
                LEFT JOIN Tree_HealthBatchSetting batch ON record.treeID = batch.treeID AND batch.accountID = @currentUserID
            ";

            // 加入草稿參數
            // 若 filter.includeDraft 為 true，傳入 1，否則 0
            parameters.Add(new SqlParameter("@incDraft", filter.includeDraft ? 1 : 0));

            //使用者ID檢查設定檔
            parameters.Add(new SqlParameter("@currentUserID", currentUserID));

            // ==========================================
            // 2. WHERE 篩選條件
            // ==========================================
            whereClauses.Add("record.removeDateTime IS NULL");
            whereClauses.Add("record.editStatus = 1");
            string unitLimitSql = @"
                record.areaID IN (
                    SELECT map.twID 
                    FROM System_UnitCityMapping map
                    INNER JOIN System_UserAccount u ON u.unitID = map.unitID
                    WHERE u.accountID = @currentUserID
                )
            ";
            whereClauses.Add(unitLimitSql);

            // 篩選：僅撈取無健檢紀錄樹籍
            // 因為用了 OUTER APPLY，無紀錄的 latest_health.healthID 會是 NULL
            if (filter.onlyNoRecord)
            {
                whereClauses.Add("latest_health.healthID IS NULL");
            }

            // 一般篩選
            if (filter.cityID.HasValue)
            {
                whereClauses.Add("record.cityID = @cityID");
                parameters.Add(new SqlParameter("@cityID", filter.cityID));
            }

            if (filter.areaID.HasValue)
            {
                whereClauses.Add("record.areaID = @areaID");
                parameters.Add(new SqlParameter("@areaID", filter.areaID));
            }

            if (filter.speciesID.HasValue)
            {
                whereClauses.Add("record.speciesID = @speciesID");
                parameters.Add(new SqlParameter("@speciesID", filter.speciesID));
            }

            if (!string.IsNullOrWhiteSpace(filter.keyword))
            {
                // 搜尋樹籍 + 最新健檢的調查人
                string kwSql = "(record.systemTreeNo LIKE @kw OR record.agencyTreeNo LIKE @kw OR record.manager LIKE @kw OR latest_health.surveyor LIKE @kw)";
                whereClauses.Add(kwSql);
                parameters.Add(new SqlParameter("@kw", "%" + filter.keyword.Trim() + "%"));
            }

            // 組合 SQL
            string finalSql = $"{baseSql} WHERE {string.Join(" AND ", whereClauses)}";

            // ==========================================
            // 3. 排序邏輯 
            // ==========================================
            string sortSql = "";
            string dir = (filter.sortDirection == "DESC") ? "DESC" : "ASC";

            // 處理特殊排序需求
            if (filter.sortExpression == "DefaultSort" || string.IsNullOrEmpty(filter.sortExpression))
            {
                // 無健檢紀錄排最上方 (healthID IS NULL -> 0, ELSE -> 1)
                // 接著依 systemTreeNo ASC
                sortSql = "ORDER BY (CASE WHEN latest_health.healthID IS NULL THEN 0 ELSE 1 END) ASC, record.systemTreeNo ASC";
            }
            else if (filter.sortExpression == "areaID")
            {
                // 縣市鄉鎮排序
                sortSql = $"ORDER BY record.areaID {dir}";
            }
            else
            {
                // 其他欄位對應
                string sortField = filter.sortExpression;

                // 簡單的欄位對應表
                if (sortField == "surveyDate" || sortField == "surveyor" || sortField == "dataStatus")
                    sortField = "latest_health." + sortField;
                else if (!sortField.Contains(".")) // 若不是別名欄位(如 speciesName)，加上 table alias
                    sortField = "record." + sortField;

                // 白名單檢查 
                sortSql = $"ORDER BY {sortField} {dir}";
            }

            finalSql += " " + sortSql;

            // ==========================================
            // 4. 執行與回傳
            // ==========================================
            var result = new List<HealthMainQueryResult>();
            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(finalSql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    var item = new HealthMainQueryResult();

                    // 樹籍資料
                    item.treeID = DataRowHelper.GetNullableInt(row, "treeID") ?? 0;
                    item.systemTreeNo = DataRowHelper.GetString(row, "systemTreeNo");
                    item.agencyTreeNo = DataRowHelper.GetString(row, "agencyTreeNo");
                    item.cityName = DataRowHelper.GetString(row, "cityName");
                    item.areaName = DataRowHelper.GetString(row, "areaName");
                    item.speciesName = DataRowHelper.GetString(row, "speciesName");
                    item.manager = DataRowHelper.GetString(row, "manager");

                    // 健檢資料 
                    item.healthID = DataRowHelper.GetNullableInt(row, "healthID");
                    item.surveyDate = DataRowHelper.GetNullableDateTime(row, "surveyDate");
                    item.surveyor = DataRowHelper.GetString(row, "surveyor");

                    // 狀態 Enum
                    string tStatus = DataRowHelper.GetString(row, "treeStatus");
                    if (!string.IsNullOrEmpty(tStatus) && Enum.TryParse<TreeStatus>(tStatus, out var ts))
                        item.treeStatus = ts;

                    int? dStatus = DataRowHelper.GetNullableInt(row, "dataStatus");
                    if (dStatus.HasValue && Enum.IsDefined(typeof(enum_healthRecordStatus), dStatus.Value))
                        item.dataStatus = (enum_healthRecordStatus)dStatus.Value;

                    item.lastUpdate = DataRowHelper.GetNullableDateTime(row, "lastUpdate");
                    item.isAdded = DataRowHelper.GetNullableInt(row, "isAdded") == 1;

                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 取得某使用者的批次設定清單
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public List<HealthBatchSettingResult> GetHealthBatchSetting(int accountID)
        {
            string sql = @"
                SELECT s.settingID, s.insertDateTime,
                       record.treeID, 
                       record.systemTreeNo, 
                       record.agencyTreeNo, 
                       record.agencyJurisdictionCode,

                       COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                       areaInfo.area AS areaName,
                       /* 樹種 */
                       species.commonName AS speciesName,
               
                       record.manager
               
                FROM Tree_HealthBatchSetting s
                JOIN Tree_Record record ON s.treeID = record.treeID

                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
        
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
        
                WHERE s.accountID = @accountID
                ORDER BY s.insertDateTime DESC
            ";

            var result = new List<HealthBatchSettingResult>();

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@accountID", accountID));
                foreach (DataRow row in dt.Rows)
                {
                    var item = new HealthBatchSettingResult();

                    item.settingID = DataRowHelper.GetNullableInt(row, "settingID") ?? 0;
                    item.insertDateTime = DataRowHelper.GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue;

                    item.treeID = DataRowHelper.GetNullableInt(row, "treeID") ?? 0;
                    item.systemTreeNo = DataRowHelper.GetString(row, "systemTreeNo");
                    item.agencyTreeNo = DataRowHelper.GetString(row, "agencyTreeNo");
                    item.agencyJurisdictionCode = DataRowHelper.GetString(row, "agencyJurisdictionCode");

                    item.cityName = DataRowHelper.GetString(row, "cityName");
                    item.areaName = DataRowHelper.GetString(row, "areaName");

                    item.speciesName = DataRowHelper.GetString(row, "speciesName");
                    item.manager = DataRowHelper.GetString(row, "manager");

                    result.Add(item);
                }
            }
            return result;
        }
        /// <summary>
        /// 加入批次設定 (若已存在則忽略)
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="treeID"></param>
        public void AddToHealthBatchSetting(int accountID, int treeID)
        {
            string sql = @"
            IF NOT EXISTS (SELECT 1 FROM Tree_HealthBatchSetting WHERE accountID = @accountID AND treeID = @treeID)
            BEGIN
                INSERT INTO Tree_HealthBatchSetting (accountID, treeID)
                VALUES (@accountID, @treeID)
            END
        ";

            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountID", accountID),
                    new SqlParameter("@treeID", treeID));
            }
        }

        /// <summary>
        ///  移除批次設定 (硬刪除)
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="treeID"></param>
        public void RemoveFromHealthBatchSetting(int accountID, int? treeID = null)
        {
            string sql = "";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@accountID", accountID));

            if (treeID.HasValue)
            {
                // 移除單筆
                sql = "DELETE FROM Tree_HealthBatchSetting WHERE accountID = @accountID AND treeID = @treeID";
                parameters.Add(new SqlParameter("@treeID", treeID.Value));
            }
            else
            {
                // 清空全部
                sql = "DELETE FROM Tree_HealthBatchSetting WHERE accountID = @accountID";
            }

            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql, parameters.ToArray());
            }
        }

        public List<HealthMainQueryResult> GetHealthRecordList(HealthRecordListFilter filter, int currentUserID)
        {
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            // ==========================================
            // 1. SQL 架構
            // ==========================================
            string sql = @"
                SELECT 
                    /* 紀錄本身資訊 */
                    health.healthID, 
                    health.surveyDate, 
                    health.surveyor, 
                    health.dataStatus,
                    COALESCE(health.updateDateTime, health.insertDateTime) AS lastUpdate,

                    /* 關聯樹籍資訊 */
                    record.treeID, 
                    record.systemTreeNo, 
                    record.agencyTreeNo,
            
                    /* 名稱轉換 (優先權：System_Taiwan > Tree_Record 原欄位) */
                    COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                    areaInfo.area AS areaName,
            
                    /* 樹種名稱 */
                    species.commonName AS speciesName,
            
                    record.manager

                FROM Tree_HealthRecord health
                JOIN Tree_Record record ON health.treeID = record.treeID

                /* 關聯表 */
                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
            ";

            parameters.Add(new SqlParameter("@currentUserID", currentUserID));

            // ==========================================
            // 2. 篩選條件
            // ==========================================
            whereClauses.Add("health.removeDateTime IS NULL");
            whereClauses.Add("record.removeDateTime IS NULL");

            // 必須是 editStatus = 1 (已完稿/有樹號) 才能有健檢紀錄
            whereClauses.Add("record.editStatus = 1");

            //過濾單位
            string unitLimitSql = @"
                record.areaID IN (
                    SELECT map.twID 
                    FROM System_UnitCityMapping map
                    INNER JOIN System_UserAccount u ON u.unitID = map.unitID
                    WHERE u.accountID = @currentUserID
                )
            ";
            whereClauses.Add(unitLimitSql);

            // Scope: 我的紀錄 vs 單位全部
            if (filter.scope == "My")
            {
                // 我的紀錄
                // 只撈取 insertAccountID 等於自己的紀錄
                whereClauses.Add("health.insertAccountID = @currentUserID");
            }
            

            // 日期區間 (針對 surveyDate)
            if (filter.dateStart.HasValue)
            {
                whereClauses.Add("health.surveyDate >= @dStart");
                parameters.Add(new SqlParameter("@dStart", filter.dateStart.Value));
            }
            if (filter.dateEnd.HasValue)
            {
                // 包含當天結束
                whereClauses.Add("health.surveyDate <= @dEnd");
                parameters.Add(new SqlParameter("@dEnd", filter.dateEnd.Value));
            }

            // 一般欄位篩選
            if (filter.cityID.HasValue)
            {
                whereClauses.Add("record.cityID = @cityID");
                parameters.Add(new SqlParameter("@cityID", filter.cityID));
            }

            if (filter.areaID.HasValue)
            {
                whereClauses.Add("record.areaID = @areaID");
                parameters.Add(new SqlParameter("@areaID", filter.areaID));
            }

            if (filter.speciesID.HasValue)
            {
                whereClauses.Add("record.speciesID = @speciesID");
                parameters.Add(new SqlParameter("@speciesID", filter.speciesID));
            }

            // 關鍵字 (查樹號、管理人、調查人)
            if (!string.IsNullOrWhiteSpace(filter.keyword))
            {
                string kwSql = @"(
                record.systemTreeNo LIKE @kw OR 
                record.agencyTreeNo LIKE @kw OR 
                record.manager LIKE @kw OR 
                health.surveyor LIKE @kw
            )";
                whereClauses.Add(kwSql);
                parameters.Add(new SqlParameter("@kw", "%" + filter.keyword.Trim() + "%"));
            }

            // ==========================================
            // 3. 排序邏輯 (區分 預設 vs 指定)
            // ==========================================
            string sortSql = "";

            // 判斷是否為「預設排序」(前端 ViewState 初始值或 null)
            if (string.IsNullOrEmpty(filter.sortExpression) || filter.sortExpression == "DefaultSort")
            {
                // [預設] 調查日期 DESC + 系統樹籍編號 DESC
                sortSql = "ORDER BY health.surveyDate DESC, record.systemTreeNo DESC";
            }
            else
            {
                // [指定] 依照使用者點擊的欄位
                string sortField = filter.sortExpression;
                string sortDir = (filter.sortDirection == "DESC") ? "DESC" : "ASC";

                // 欄位別名對應 (確保 SQL 知道是哪個 Table)
                if (sortField == "surveyDate" || sortField == "dataStatus" || sortField == "surveyor")
                {
                    sortField = "health." + sortField;
                }
                else if (sortField != "speciesName" && sortField != "cityName" && sortField != "areaName")
                {
                    // 若不是 JOIN 出來的 Name 別名，則屬於 record
                    sortField = "record." + sortField;
                }

                sortSql = $"ORDER BY {sortField} {sortDir}";
            }

            // 組合最終 SQL
            sql += $" WHERE {string.Join(" AND ", whereClauses)} {sortSql}";

            // ==========================================
            // 4. 執行與 Mapping
            // ==========================================
            var result = new List<HealthMainQueryResult>();
            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    var item = new HealthMainQueryResult();

                    // Health 資訊
                    item.healthID = DataRowHelper.GetNullableInt(row, "healthID");
                    item.surveyDate = DataRowHelper.GetNullableDateTime(row, "surveyDate");
                    item.surveyor = DataRowHelper.GetString(row, "surveyor");
                    item.lastUpdate = DataRowHelper.GetNullableDateTime(row, "lastUpdate");

                    int? dStatus = DataRowHelper.GetNullableInt(row, "dataStatus");
                    if (dStatus.HasValue) item.dataStatus = (enum_healthRecordStatus)dStatus.Value;

                    // Record 資訊
                    item.treeID = DataRowHelper.GetNullableInt(row, "treeID") ?? 0;
                    item.systemTreeNo = DataRowHelper.GetString(row, "systemTreeNo");
                    item.agencyTreeNo = DataRowHelper.GetString(row, "agencyTreeNo");
                    item.cityName = DataRowHelper.GetString(row, "cityName");
                    item.areaName = DataRowHelper.GetString(row, "areaName");
                    item.speciesName = DataRowHelper.GetString(row, "speciesName");
                    item.manager = DataRowHelper.GetString(row, "manager");

                    result.Add(item);
                }
            }
            return result;
        }

        public List<TreeHealthRecord> GetHealthRecordsByTree(int treeId)
        {
            string sql = @"
                SELECT
                    h.healthID,
                    h.treeID,
                    h.surveyDate,
                    h.surveyor,
                    h.managementStatus,
                    h.priority,
                    h.treatmentDescription,
                    h.dataStatus,
                    h.insertDateTime,
                    h.updateDateTime
                FROM Tree_HealthRecord h
                WHERE h.treeID = @treeID
                  AND h.removeDateTime IS NULL
                ORDER BY h.surveyDate DESC, h.healthID DESC";

            var result = new List<TreeHealthRecord>();
            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@treeID", treeId));
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(ToHealthRecord(row));
                }
            }

            return result;
        }

        /// <summary>
        /// 取得健檢紀錄
        /// </summary>
        public TreeHealthRecord GetHealthRecord(int healthID)
        {
            TreeHealthRecord record = null;

            // 1. 查詢主檔並 JOIN 樹籍資料
            string sql = @"
               SELECT 
                h.*, 
                t.systemTreeNo, 
                t.agencyTreeNo, 
                t.manager,
                COALESCE(areaInfo.city, cityInfo.city) AS cityName,
                areaInfo.area AS areaName,
                species.commonName AS speciesCommonName
            FROM Tree_HealthRecord h
            INNER JOIN Tree_Record t ON h.treeID = t.treeID
            OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = t.cityID) cityInfo
            LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = t.areaID
            LEFT JOIN Tree_Species species ON species.speciesID = t.speciesID

            WHERE h.healthID = @id";

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@id", healthID));
                if (dt.Rows.Count > 0)
                {
                    record = ToHealthRecord(dt.Rows[0]);
                }
            }

            // 2. 如果主檔存在，順便撈取照片與附件
            if (record != null)
            {
                record.photos = GetHealthPhotos(healthID);
                record.attachments = GetHealthAttachments(healthID);
            }

            return record;
        }

        public bool CheckSurveyDateDuplicate(int treeID, DateTime surveyDate, int excludeHealthID)
        {
            // SQL 邏輯：
            // 1. 樹木 ID 相同
            // 2. 日期相同
            // 3. 排除目前正在編輯的 ID (新增時 excludeHealthID 為 0，不影響)
            // 4. 排除已刪除的資料
            string sql = @"
                SELECT COUNT(1)
                FROM Tree_HealthRecord
                WHERE treeID = @treeID
                  AND surveyDate = @surveyDate
                  AND healthID <> @excludeHealthID 
                  AND removeDateTime IS NULL";

            using (var da = new MS_SQL())
            {
                // 參數設定
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@treeID", treeID),
                    new SqlParameter("@surveyDate", surveyDate.Date), 
                    new SqlParameter("@excludeHealthID", excludeHealthID)
                };

                object result = da.ExcuteScalar(sql, parameters);
                int count = result != null ? Convert.ToInt32(result) : 0;

                return count > 0; // 大於 0 代表有重複
            }
        }

        /// <summary>
        /// 自動判斷是新增 (Insert) 還是編輯 (Update)
        /// </summary>
        public int SaveHealthRecord(TreeHealthRecord record, int accountID)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            using (var da = new MS_SQL())
            {
                // 準備參數 (共用)
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@treeID", record.treeID),
                    new SqlParameter("@accountID", accountID)
                };
                parameters.AddRange(GetRecordParameters(record));

                if (record.healthID <= 0)
                {
                    // --- INSERT ---
                    string insertSql = @"
                        INSERT INTO Tree_HealthRecord
                        (
                            treeID, surveyDate, surveyor, dataStatus, memo, treeSignStatus, 
                            latitude, longitude, treeHeight, canopyArea,
                            girth100, diameter100, girth130, diameter130, measureNote,
                            majorDiseaseBrownRoot, majorDiseaseGanoderma, majorDiseaseWoodDecayFungus, majorDiseaseCanker, majorDiseaseOther, majorDiseaseOtherNote,
                            majorPestRootTunnel, majorPestRootChew, majorPestRootLive,
                            majorPestBaseTunnel, majorPestBaseChew, majorPestBaseLive,
                            majorPestTrunkTunnel, majorPestTrunkChew, majorPestTrunkLive,
                            majorPestBranchTunnel, majorPestBranchChew, majorPestBranchLive,
                            majorPestCrownTunnel, majorPestCrownChew, majorPestCrownLive,
                            majorPestOtherTunnel, majorPestOtherChew, majorPestOtherLive,
                            generalPestRoot, generalPestBase, generalPestTrunk, generalPestBranch, generalPestCrown, generalPestOther,
                            generalDiseaseRoot, generalDiseaseBase, generalDiseaseTrunk, generalDiseaseBranch, generalDiseaseCrown, generalDiseaseOther,
                            pestOtherNote,
                            rootDecayPercent, rootCavityMaxDiameter, rootWoundMaxDiameter, rootMechanicalDamage, rootMowingInjury, rootInjury, rootGirdling, rootOtherNote,
                            baseDecayPercent, baseCavityMaxDiameter, baseWoundMaxDiameter, baseMechanicalDamage, baseMowingInjury, baseOtherNote,
                            trunkDecayPercent, trunkCavityMaxDiameter, trunkWoundMaxDiameter, trunkMechanicalDamage, trunkIncludedBark, trunkOtherNote,
                            branchDecayPercent, branchCavityMaxDiameter, branchWoundMaxDiameter, branchMechanicalDamage, branchIncludedBark, branchDrooping, branchOtherNote,
                            crownLeafCoveragePercent, crownDeadBranchPercent, crownHangingBranch, crownOtherNote, growthNote,
                            pruningWrongDamage, pruningWoundHealing, pruningEpiphyte, pruningParasite, pruningVine, pruningOtherNote,
                            supportCount, supportEmbedded, supportOtherNote,
                            siteCementPercent, siteAsphaltPercent, sitePlanter, siteRecreationFacility, siteDebrisStack, siteBetweenBuildings, siteSoilCompaction, siteOverburiedSoil, siteOtherNote,
                            soilPh, soilOrganicMatter, soilEc,
                            managementStatus, priority, treatmentDescription,
                            insertAccountID, insertDateTime
                        )
                        OUTPUT INSERTED.healthID
                        VALUES
                        (
                            @treeID, @surveyDate, @surveyor, @dataStatus, @memo, @treeSignStatus, 
                            @latitude, @longitude, @treeHeight, @canopyArea,
                            @girth100, @diameter100, @girth130, @diameter130, @measureNote,
                            @majorDiseaseBrownRoot, @majorDiseaseGanoderma, @majorDiseaseWoodDecayFungus, @majorDiseaseCanker, @majorDiseaseOther, @majorDiseaseOtherNote,
                            @majorPestRootTunnel, @majorPestRootChew, @majorPestRootLive,
                            @majorPestBaseTunnel, @majorPestBaseChew, @majorPestBaseLive,
                            @majorPestTrunkTunnel, @majorPestTrunkChew, @majorPestTrunkLive,
                            @majorPestBranchTunnel, @majorPestBranchChew, @majorPestBranchLive,
                            @majorPestCrownTunnel, @majorPestCrownChew, @majorPestCrownLive,
                            @majorPestOtherTunnel, @majorPestOtherChew, @majorPestOtherLive,
                            @generalPestRoot, @generalPestBase, @generalPestTrunk, @generalPestBranch, @generalPestCrown, @generalPestOther,
                            @generalDiseaseRoot, @generalDiseaseBase, @generalDiseaseTrunk, @generalDiseaseBranch, @generalDiseaseCrown, @generalDiseaseOther,
                            @pestOtherNote,
                            @rootDecayPercent, @rootCavityMaxDiameter, @rootWoundMaxDiameter, @rootMechanicalDamage, @rootMowingInjury, @rootInjury, @rootGirdling, @rootOtherNote,
                            @baseDecayPercent, @baseCavityMaxDiameter, @baseWoundMaxDiameter, @baseMechanicalDamage, @baseMowingInjury, @baseOtherNote,
                            @trunkDecayPercent, @trunkCavityMaxDiameter, @trunkWoundMaxDiameter, @trunkMechanicalDamage, @trunkIncludedBark, @trunkOtherNote,
                            @branchDecayPercent, @branchCavityMaxDiameter, @branchWoundMaxDiameter, @branchMechanicalDamage, @branchIncludedBark, @branchDrooping, @branchOtherNote,
                            @crownLeafCoveragePercent, @crownDeadBranchPercent, @crownHangingBranch, @crownOtherNote, @growthNote,
                            @pruningWrongDamage, @pruningWoundHealing, @pruningEpiphyte, @pruningParasite, @pruningVine, @pruningOtherNote,
                            @supportCount, @supportEmbedded, @supportOtherNote,
                            @siteCementPercent, @siteAsphaltPercent, @sitePlanter, @siteRecreationFacility, @siteDebrisStack, @siteBetweenBuildings, @siteSoilCompaction, @siteOverburiedSoil, @siteOtherNote,
                            @soilPh, @soilOrganicMatter, @soilEc,
                            @managementStatus, @priority, @treatmentDescription,
                            @accountID, GETDATE()
                        )";

                    return Convert.ToInt32(da.ExcuteScalar(insertSql, parameters.ToArray()));
                }
                else
                {
                    // --- UPDATE ---
                    parameters.Add(new SqlParameter("@id", record.healthID));

                    string updateSql = @"
                        UPDATE Tree_HealthRecord
                        SET 
                            surveyDate=@surveyDate, surveyor=@surveyor, dataStatus=@dataStatus, memo=@memo, treeSignStatus=@treeSignStatus,
                            latitude=@latitude, longitude=@longitude, treeHeight=@treeHeight, canopyArea=@canopyArea,
                            girth100=@girth100, diameter100=@diameter100, girth130=@girth130, diameter130=@diameter130, measureNote=@measureNote,
                            
                            majorDiseaseBrownRoot=@majorDiseaseBrownRoot, majorDiseaseGanoderma=@majorDiseaseGanoderma, majorDiseaseWoodDecayFungus=@majorDiseaseWoodDecayFungus, 
                            majorDiseaseCanker=@majorDiseaseCanker, majorDiseaseOther=@majorDiseaseOther, majorDiseaseOtherNote=@majorDiseaseOtherNote,
                            
                            majorPestRootTunnel=@majorPestRootTunnel, majorPestRootChew=@majorPestRootChew, majorPestRootLive=@majorPestRootLive,
                            majorPestBaseTunnel=@majorPestBaseTunnel, majorPestBaseChew=@majorPestBaseChew, majorPestBaseLive=@majorPestBaseLive,
                            majorPestTrunkTunnel=@majorPestTrunkTunnel, majorPestTrunkChew=@majorPestTrunkChew, majorPestTrunkLive=@majorPestTrunkLive,
                            majorPestBranchTunnel=@majorPestBranchTunnel, majorPestBranchChew=@majorPestBranchChew, majorPestBranchLive=@majorPestBranchLive,
                            majorPestCrownTunnel=@majorPestCrownTunnel, majorPestCrownChew=@majorPestCrownChew, majorPestCrownLive=@majorPestCrownLive,
                            majorPestOtherTunnel=@majorPestOtherTunnel, majorPestOtherChew=@majorPestOtherChew, majorPestOtherLive=@majorPestOtherLive,
                            
                            generalPestRoot=@generalPestRoot, generalPestBase=@generalPestBase, generalPestTrunk=@generalPestTrunk, generalPestBranch=@generalPestBranch, generalPestCrown=@generalPestCrown, generalPestOther=@generalPestOther,
                            generalDiseaseRoot=@generalDiseaseRoot, generalDiseaseBase=@generalDiseaseBase, generalDiseaseTrunk=@generalDiseaseTrunk, generalDiseaseBranch=@generalDiseaseBranch, generalDiseaseCrown=@generalDiseaseCrown, generalDiseaseOther=@generalDiseaseOther,
                            pestOtherNote=@pestOtherNote,
                            
                            rootDecayPercent=@rootDecayPercent, rootCavityMaxDiameter=@rootCavityMaxDiameter, rootWoundMaxDiameter=@rootWoundMaxDiameter, rootMechanicalDamage=@rootMechanicalDamage, rootMowingInjury=@rootMowingInjury, rootInjury=@rootInjury, rootGirdling=@rootGirdling, rootOtherNote=@rootOtherNote,
                            baseDecayPercent=@baseDecayPercent, baseCavityMaxDiameter=@baseCavityMaxDiameter, baseWoundMaxDiameter=@baseWoundMaxDiameter, baseMechanicalDamage=@baseMechanicalDamage, baseMowingInjury=@baseMowingInjury, baseOtherNote=@baseOtherNote,
                            trunkDecayPercent=@trunkDecayPercent, trunkCavityMaxDiameter=@trunkCavityMaxDiameter, trunkWoundMaxDiameter=@trunkWoundMaxDiameter, trunkMechanicalDamage=@trunkMechanicalDamage, trunkIncludedBark=@trunkIncludedBark, trunkOtherNote=@trunkOtherNote,
                            branchDecayPercent=@branchDecayPercent, branchCavityMaxDiameter=@branchCavityMaxDiameter, branchWoundMaxDiameter=@branchWoundMaxDiameter, branchMechanicalDamage=@branchMechanicalDamage, branchIncludedBark=@branchIncludedBark, branchDrooping=@branchDrooping, branchOtherNote=@branchOtherNote,
                            crownLeafCoveragePercent=@crownLeafCoveragePercent, crownDeadBranchPercent=@crownDeadBranchPercent, crownHangingBranch=@crownHangingBranch, crownOtherNote=@crownOtherNote, growthNote=@growthNote,
                            
                            pruningWrongDamage=@pruningWrongDamage, pruningWoundHealing=@pruningWoundHealing, pruningEpiphyte=@pruningEpiphyte, pruningParasite=@pruningParasite, pruningVine=@pruningVine, pruningOtherNote=@pruningOtherNote,
                            supportCount=@supportCount, supportEmbedded=@supportEmbedded, supportOtherNote=@supportOtherNote,
                            
                            siteCementPercent=@siteCementPercent, siteAsphaltPercent=@siteAsphaltPercent, sitePlanter=@sitePlanter, siteRecreationFacility=@siteRecreationFacility, siteDebrisStack=@siteDebrisStack, siteBetweenBuildings=@siteBetweenBuildings, siteSoilCompaction=@siteSoilCompaction, siteOverburiedSoil=@siteOverburiedSoil, siteOtherNote=@siteOtherNote,
                            soilPh=@soilPh, soilOrganicMatter=@soilOrganicMatter, soilEc=@soilEc,
                            
                            managementStatus=@managementStatus, priority=@priority, treatmentDescription=@treatmentDescription,
                            
                            updateAccountID=@accountID, updateDateTime=GETDATE()
                        WHERE healthID=@id AND removeDateTime IS NULL";

                    da.ExecNonQuery(updateSql, parameters.ToArray());
                    return record.healthID;
                }
            }
        }
        /// <summary>
        /// 刪除健檢紀錄
        /// </summary>
        /// <param name="healthID">健檢紀錄 ID</param>
        /// <param name="accountID">帳號ID</param>
        /// <returns>是否成功</returns>
        public  bool DeleteHealthRecord(int healthID, int accountID)
        {
            //只有草稿才能刪除
            string sql = @"
                UPDATE Tree_HealthRecord 
                SET 
                    removeDateTime = GETDATE(),    
                    removeAccountID = @accountID    
                WHERE healthID = @healthID 
                  AND removeDateTime IS NULL
                  AND dataStatus = 0;  
            ";

            var parameters = new System.Data.SqlClient.SqlParameter[] {
                new System.Data.SqlClient.SqlParameter("@healthID", healthID),
                new System.Data.SqlClient.SqlParameter("@accountID", accountID)
            };

            using (var da = new DataAccess.MS_SQL())
            {
                int rows = da.ExecNonQuery(sql, parameters);
                return rows > 0;
            }
        }
        public int InsertHealthPhoto(TreeHealthPhoto photo, int accountID)
        {
            if (photo == null) throw new ArgumentNullException(nameof(photo));

            string sql = @"
                INSERT INTO Tree_HealthPhoto
                (healthID, fileName, filePath, fileSize, caption, insertAccountID, insertDateTime)
                OUTPUT INSERTED.photoID
                VALUES
                (@healthID, @fileName, @filePath, @fileSize, @caption, @accountID, GETDATE())";

            using (var da = new MS_SQL())
            {
                return Convert.ToInt32(da.ExcuteScalar(sql,
                    new SqlParameter("@healthID", photo.healthID),
                    new SqlParameter("@fileName", ToDbValue(photo.fileName)),
                    new SqlParameter("@filePath", ToDbValue(photo.filePath)),
                    new SqlParameter("@fileSize", ToDbValue(photo.fileSize)),
                    new SqlParameter("@caption", ToDbValue(photo.caption)),
                    new SqlParameter("@accountID", accountID)));
            }
        }

        public void DeleteHealthPhotos(int healthID, List<int> photoIDs, int accountID)
        {
            if (photoIDs == null || photoIDs.Count == 0) return;

            // 將 int list 轉成 "1,2,3" 字串
            string idList = string.Join(",", photoIDs);

            string sql = $"UPDATE Tree_HealthPhoto SET removeDateTime=GETDATE(), removeAccountID=@accountID WHERE photoID IN ({idList}) AND healthID=@healthID";

            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountID", accountID),
                    new SqlParameter("@healthID", healthID));
            }
        }

        public void UpdateHealthPhotoCaptions(int healthID, List<TreeHealthPhoto> updates)
        {
            if (updates == null || updates.Count == 0) return;

            // 設定每批次處理的數量
            int batchSize = 100;
            for (int i = 0; i < updates.Count; i += batchSize)
            {
                // 取出這一批要處理的資料 (例如第 0~99 筆)
                var currentBatch = updates.Skip(i).Take(batchSize).ToList();

                // 建立這一批次的 SQL 與 參數
                StringBuilder sqlBuilder = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();

                // 加入共用參數 (注意：每次 Execute 都要重新加入，因為參數物件不能跨 Command 重用)
                parameters.Add(new SqlParameter("@healthID", healthID));

                for (int j = 0; j < currentBatch.Count; j++)
                {
                    // 
                    string captionParam = $"@caption{j}";
                    string idParam = $"@id{j}";

                    // 拼接 SQL：加上 healthID 雙重保險
                    sqlBuilder.Append($@"
                        UPDATE Tree_HealthPhoto 
                        SET caption = {captionParam} 
                        WHERE photoID = {idParam} AND healthID = @healthID;");

                    parameters.Add(new SqlParameter(captionParam, ToDbValue(currentBatch[j].caption)));
                    parameters.Add(new SqlParameter(idParam, currentBatch[j].photoID));
                }

                // 執行這一批次
                if (sqlBuilder.Length > 0)
                {
                    using (var da = new MS_SQL())
                    {
                        da.ExecNonQuery(sqlBuilder.ToString(), parameters.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// 取得照片列表
        /// </summary>
        public List<TreeHealthPhoto> GetHealthPhotos(int healthID)
        {
            string sql = "SELECT photoID, healthID, fileName, filePath, fileSize, caption, insertDateTime FROM Tree_HealthPhoto WHERE healthID=@id AND removeDateTime IS NULL ORDER BY insertDateTime DESC, photoID DESC";

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@id", healthID));
                List<TreeHealthPhoto> list = new List<TreeHealthPhoto>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new TreeHealthPhoto
                    {
                        photoID = GetNullableInt(row, "photoID") ?? 0,
                        healthID = GetNullableInt(row, "healthID") ?? 0,
                        fileName = GetString(row, "fileName"),
                        filePath = GetString(row, "filePath"),
                        fileSize = GetNullableInt(row, "fileSize"),
                        caption = GetString(row, "caption"),
                        insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue
                    });
                }
                return list;
            }
        }

        public int InsertHealthAttachment(TreeHealthAttachment attachment, int accountID)
        {
            if (attachment == null) throw new ArgumentNullException(nameof(attachment));

            string sql = @"
                INSERT INTO Tree_HealthAttachment
                (healthID, fileName, filePath, fileSize, description, insertAccountID, insertDateTime)
                OUTPUT INSERTED.attachmentID
                VALUES
                (@healthID, @fileName, @filePath, @fileSize, @description, @accountID, GETDATE())";

            using (var da = new MS_SQL())
            {
                return Convert.ToInt32(da.ExcuteScalar(sql,
                    new SqlParameter("@healthID", attachment.healthID),
                    new SqlParameter("@fileName", ToDbValue(attachment.fileName)),
                    new SqlParameter("@filePath", ToDbValue(attachment.filePath)),
                    new SqlParameter("@fileSize", ToDbValue(attachment.fileSize)),
                    new SqlParameter("@description", ToDbValue(attachment.description)),
                    new SqlParameter("@accountID", accountID)));
            }
        }

        public void DeleteHealthAttachment(int healthID, int attachmentID, int accountID)
        {
            string sql = "UPDATE Tree_HealthAttachment SET removeDateTime=GETDATE(), removeAccountID=@accountID WHERE attachmentID=@id AND healthID=@healthID";
            using (var da = new MS_SQL())
            {
                da.ExecNonQuery(sql,
                    new SqlParameter("@accountID", accountID),
                    new SqlParameter("@id", attachmentID),
                    new SqlParameter("@healthID", healthID));
            }
        }

        /// <summary>
        /// 取得附件列表
        /// </summary>
        public List<TreeHealthAttachment> GetHealthAttachments(int healthID)
        {
            string sql = "SELECT attachmentID, healthID, fileName, filePath, fileSize, description, insertDateTime FROM Tree_HealthAttachment WHERE healthID=@id AND removeDateTime IS NULL ORDER BY insertDateTime DESC, attachmentID DESC";

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@id", healthID));
                List<TreeHealthAttachment> list = new List<TreeHealthAttachment>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new TreeHealthAttachment
                    {
                        attachmentID = GetNullableInt(row, "attachmentID") ?? 0,
                        healthID = GetNullableInt(row, "healthID") ?? 0,
                        fileName = GetString(row, "fileName"),
                        filePath = GetString(row, "filePath"),
                        fileSize = GetNullableInt(row, "fileSize"),
                        description = GetString(row, "description"),
                        insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue
                    });
                }
                return list;
            }
        }

        private TreeHealthRecord ToHealthRecord(DataRow row)
        {
            if (row == null) return null;

            int statusValue = GetNullableInt(row, "dataStatus") ?? 0;

            return new TreeHealthRecord
            {
                // PK & FK
                healthID = GetNullableInt(row, "healthID") ?? 0,
                treeID = GetNullableInt(row, "treeID") ?? 0,

                // 樹籍資訊
                systemTreeNo = GetString(row, "systemTreeNo"),
                agencyTreeNo = GetString(row, "agencyTreeNo"),
                cityName = GetString(row, "cityName"),
                areaName = GetString(row, "areaName"),
                speciesName = GetString(row, "speciesCommonName"),
                manager = GetString(row, "manager"),

                // 基本資料
                surveyDate = GetNullableDateTime(row, "surveyDate"),
                surveyor = GetString(row, "surveyor"),
                dataStatus = statusValue,
                memo = GetString(row, "memo"),
                treeSignStatus = (byte?)GetNullableInt(row, "treeSignStatus"),

                // 樹木規格
                latitude = GetNullableDecimal(row, "latitude"),
                longitude = GetNullableDecimal(row, "longitude"),
                treeHeight = GetNullableDecimal(row, "treeHeight"),
                canopyArea = GetNullableDecimal(row, "canopyArea"),
                girth100 = GetString(row, "girth100"),
                diameter100 = GetString(row, "diameter100"),
                girth130 = GetString(row, "girth130"),
                diameter130 = GetString(row, "diameter130"),
                measureNote = GetString(row, "measureNote"),

                // 病害
                majorDiseaseBrownRoot = GetNullableBoolean(row, "majorDiseaseBrownRoot"),
                majorDiseaseGanoderma = GetNullableBoolean(row, "majorDiseaseGanoderma"),
                majorDiseaseWoodDecayFungus = GetNullableBoolean(row, "majorDiseaseWoodDecayFungus"),
                majorDiseaseCanker = GetNullableBoolean(row, "majorDiseaseCanker"),
                majorDiseaseOther = GetNullableBoolean(row, "majorDiseaseOther"),
                majorDiseaseOtherNote = GetString(row, "majorDiseaseOtherNote"),

                // 蟲害
                majorPestRootTunnel = GetNullableBoolean(row, "majorPestRootTunnel"),
                majorPestRootChew = GetNullableBoolean(row, "majorPestRootChew"),
                majorPestRootLive = GetNullableBoolean(row, "majorPestRootLive"),
                majorPestBaseTunnel = GetNullableBoolean(row, "majorPestBaseTunnel"),
                majorPestBaseChew = GetNullableBoolean(row, "majorPestBaseChew"),
                majorPestBaseLive = GetNullableBoolean(row, "majorPestBaseLive"),
                majorPestTrunkTunnel = GetNullableBoolean(row, "majorPestTrunkTunnel"),
                majorPestTrunkChew = GetNullableBoolean(row, "majorPestTrunkChew"),
                majorPestTrunkLive = GetNullableBoolean(row, "majorPestTrunkLive"),
                majorPestBranchTunnel = GetNullableBoolean(row, "majorPestBranchTunnel"),
                majorPestBranchChew = GetNullableBoolean(row, "majorPestBranchChew"),
                majorPestBranchLive = GetNullableBoolean(row, "majorPestBranchLive"),
                majorPestCrownTunnel = GetNullableBoolean(row, "majorPestCrownTunnel"),
                majorPestCrownChew = GetNullableBoolean(row, "majorPestCrownChew"),
                majorPestCrownLive = GetNullableBoolean(row, "majorPestCrownLive"),
                majorPestOtherTunnel = GetNullableBoolean(row, "majorPestOtherTunnel"),
                majorPestOtherChew = GetNullableBoolean(row, "majorPestOtherChew"),
                majorPestOtherLive = GetNullableBoolean(row, "majorPestOtherLive"),

                // 一般描述
                generalPestRoot = GetString(row, "generalPestRoot"),
                generalPestBase = GetString(row, "generalPestBase"),
                generalPestTrunk = GetString(row, "generalPestTrunk"),
                generalPestBranch = GetString(row, "generalPestBranch"),
                generalPestCrown = GetString(row, "generalPestCrown"),
                generalPestOther = GetString(row, "generalPestOther"),
                generalDiseaseRoot = GetString(row, "generalDiseaseRoot"),
                generalDiseaseBase = GetString(row, "generalDiseaseBase"),
                generalDiseaseTrunk = GetString(row, "generalDiseaseTrunk"),
                generalDiseaseBranch = GetString(row, "generalDiseaseBranch"),
                generalDiseaseCrown = GetString(row, "generalDiseaseCrown"),
                generalDiseaseOther = GetString(row, "generalDiseaseOther"),
                pestOtherNote = GetString(row, "pestOtherNote"),

                // 細節檢測
                rootDecayPercent = GetNullableDecimal(row, "rootDecayPercent"),
                rootCavityMaxDiameter = GetNullableDecimal(row, "rootCavityMaxDiameter"),
                rootWoundMaxDiameter = GetNullableDecimal(row, "rootWoundMaxDiameter"),
                rootMechanicalDamage = GetNullableBoolean(row, "rootMechanicalDamage"),
                rootMowingInjury = GetNullableBoolean(row, "rootMowingInjury"),
                rootInjury = GetNullableBoolean(row, "rootInjury"),
                rootGirdling = GetNullableBoolean(row, "rootGirdling"),
                rootOtherNote = GetString(row, "rootOtherNote"),

                baseDecayPercent = GetNullableDecimal(row, "baseDecayPercent"),
                baseCavityMaxDiameter = GetNullableDecimal(row, "baseCavityMaxDiameter"),
                baseWoundMaxDiameter = GetNullableDecimal(row, "baseWoundMaxDiameter"),
                baseMechanicalDamage = GetNullableBoolean(row, "baseMechanicalDamage"),
                baseMowingInjury = GetNullableBoolean(row, "baseMowingInjury"),
                baseOtherNote = GetString(row, "baseOtherNote"),

                trunkDecayPercent = GetNullableDecimal(row, "trunkDecayPercent"),
                trunkCavityMaxDiameter = GetNullableDecimal(row, "trunkCavityMaxDiameter"),
                trunkWoundMaxDiameter = GetNullableDecimal(row, "trunkWoundMaxDiameter"),
                trunkMechanicalDamage = GetNullableBoolean(row, "trunkMechanicalDamage"),
                trunkIncludedBark = GetNullableBoolean(row, "trunkIncludedBark"),
                trunkOtherNote = GetString(row, "trunkOtherNote"),

                branchDecayPercent = GetNullableDecimal(row, "branchDecayPercent"),
                branchCavityMaxDiameter = GetNullableDecimal(row, "branchCavityMaxDiameter"),
                branchWoundMaxDiameter = GetNullableDecimal(row, "branchWoundMaxDiameter"),
                branchMechanicalDamage = GetNullableBoolean(row, "branchMechanicalDamage"),
                branchIncludedBark = GetNullableBoolean(row, "branchIncludedBark"),
                branchDrooping = GetNullableBoolean(row, "branchDrooping"),
                branchOtherNote = GetString(row, "branchOtherNote"),

                crownLeafCoveragePercent = GetString(row, "crownLeafCoveragePercent"),
                crownDeadBranchPercent = GetNullableDecimal(row, "crownDeadBranchPercent"),
                crownHangingBranch = GetNullableBoolean(row, "crownHangingBranch"),
                crownOtherNote = GetString(row, "crownOtherNote"),
                growthNote = GetString(row, "growthNote"),

                // 修剪與支撐
                pruningWrongDamage = GetString(row, "pruningWrongDamage"),
                pruningWoundHealing = GetNullableBoolean(row, "pruningWoundHealing"),
                pruningEpiphyte = GetNullableBoolean(row, "pruningEpiphyte"),
                pruningParasite = GetNullableBoolean(row, "pruningParasite"),
                pruningVine = GetNullableBoolean(row, "pruningVine"),
                pruningOtherNote = GetString(row, "pruningOtherNote"),
                supportCount = GetNullableInt(row, "supportCount"),
                supportEmbedded = GetNullableBoolean(row, "supportEmbedded"),
                supportOtherNote = GetString(row, "supportOtherNote"),

                // 棲地與土壤
                siteCementPercent = GetNullableDecimal(row, "siteCementPercent"),
                siteAsphaltPercent = GetNullableDecimal(row, "siteAsphaltPercent"),
                sitePlanter = GetNullableBoolean(row, "sitePlanter"),
                siteRecreationFacility = GetNullableBoolean(row, "siteRecreationFacility"),
                siteDebrisStack = GetNullableBoolean(row, "siteDebrisStack"),
                siteBetweenBuildings = GetNullableBoolean(row, "siteBetweenBuildings"),
                siteSoilCompaction = GetNullableBoolean(row, "siteSoilCompaction"),
                siteOverburiedSoil = GetNullableBoolean(row, "siteOverburiedSoil"),
                siteOtherNote = GetString(row, "siteOtherNote"),
                soilPh = GetString(row, "soilPh"),
                soilOrganicMatter = GetString(row, "soilOrganicMatter"),
                soilEc = GetString(row, "soilEc"),

                // 管理建議
                managementStatus = GetString(row, "managementStatus"),
                priority = GetString(row, "priority"),
                treatmentDescription = GetString(row, "treatmentDescription"),

                insertAccountID = GetNullableInt(row, "insertAccountID") ?? 0,
                insertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                updateAccountID = GetNullableInt(row, "updateAccountID"),
                updateDateTime = GetNullableDateTime(row, "updateDateTime")
            };
        }

        private IEnumerable<SqlParameter> GetRecordParameters(TreeHealthRecord record)
        {
            // 基本資料
            yield return new SqlParameter("@surveyDate", record.surveyDate.HasValue ? (object)record.surveyDate.Value : DBNull.Value);
            yield return new SqlParameter("@surveyor", ToDbValue(record.surveyor));
            yield return new SqlParameter("@dataStatus", record.dataStatus);
            yield return new SqlParameter("@memo", ToDbValue(record.memo));
            yield return new SqlParameter("@treeSignStatus", ToDbValue(record.treeSignStatus));

            // 樹木規格
            yield return new SqlParameter("@latitude", ToDbValue(record.latitude));
            yield return new SqlParameter("@longitude", ToDbValue(record.longitude));
            yield return new SqlParameter("@treeHeight", ToDbValue(record.treeHeight));
            yield return new SqlParameter("@canopyArea", ToDbValue(record.canopyArea));
            yield return new SqlParameter("@girth100", ToDbValue(record.girth100));
            yield return new SqlParameter("@diameter100", ToDbValue(record.diameter100));
            yield return new SqlParameter("@girth130", ToDbValue(record.girth130));
            yield return new SqlParameter("@diameter130", ToDbValue(record.diameter130));
            yield return new SqlParameter("@measureNote", ToDbValue(record.measureNote));

            // 病害
            yield return new SqlParameter("@majorDiseaseBrownRoot", ToDbValue(record.majorDiseaseBrownRoot));
            yield return new SqlParameter("@majorDiseaseGanoderma", ToDbValue(record.majorDiseaseGanoderma));
            yield return new SqlParameter("@majorDiseaseWoodDecayFungus", ToDbValue(record.majorDiseaseWoodDecayFungus));
            yield return new SqlParameter("@majorDiseaseCanker", ToDbValue(record.majorDiseaseCanker));
            yield return new SqlParameter("@majorDiseaseOther", ToDbValue(record.majorDiseaseOther));
            yield return new SqlParameter("@majorDiseaseOtherNote", ToDbValue(record.majorDiseaseOtherNote));

            // 蟲害
            yield return new SqlParameter("@majorPestRootTunnel", ToDbValue(record.majorPestRootTunnel));
            yield return new SqlParameter("@majorPestRootChew", ToDbValue(record.majorPestRootChew));
            yield return new SqlParameter("@majorPestRootLive", ToDbValue(record.majorPestRootLive));
            yield return new SqlParameter("@majorPestBaseTunnel", ToDbValue(record.majorPestBaseTunnel));
            yield return new SqlParameter("@majorPestBaseChew", ToDbValue(record.majorPestBaseChew));
            yield return new SqlParameter("@majorPestBaseLive", ToDbValue(record.majorPestBaseLive));
            yield return new SqlParameter("@majorPestTrunkTunnel", ToDbValue(record.majorPestTrunkTunnel));
            yield return new SqlParameter("@majorPestTrunkChew", ToDbValue(record.majorPestTrunkChew));
            yield return new SqlParameter("@majorPestTrunkLive", ToDbValue(record.majorPestTrunkLive));
            yield return new SqlParameter("@majorPestBranchTunnel", ToDbValue(record.majorPestBranchTunnel));
            yield return new SqlParameter("@majorPestBranchChew", ToDbValue(record.majorPestBranchChew));
            yield return new SqlParameter("@majorPestBranchLive", ToDbValue(record.majorPestBranchLive));
            yield return new SqlParameter("@majorPestCrownTunnel", ToDbValue(record.majorPestCrownTunnel));
            yield return new SqlParameter("@majorPestCrownChew", ToDbValue(record.majorPestCrownChew));
            yield return new SqlParameter("@majorPestCrownLive", ToDbValue(record.majorPestCrownLive));
            yield return new SqlParameter("@majorPestOtherTunnel", ToDbValue(record.majorPestOtherTunnel));
            yield return new SqlParameter("@majorPestOtherChew", ToDbValue(record.majorPestOtherChew));
            yield return new SqlParameter("@majorPestOtherLive", ToDbValue(record.majorPestOtherLive));

            // 一般描述
            yield return new SqlParameter("@generalPestRoot", ToDbValue(record.generalPestRoot));
            yield return new SqlParameter("@generalPestBase", ToDbValue(record.generalPestBase));
            yield return new SqlParameter("@generalPestTrunk", ToDbValue(record.generalPestTrunk));
            yield return new SqlParameter("@generalPestBranch", ToDbValue(record.generalPestBranch));
            yield return new SqlParameter("@generalPestCrown", ToDbValue(record.generalPestCrown));
            yield return new SqlParameter("@generalPestOther", ToDbValue(record.generalPestOther));
            yield return new SqlParameter("@generalDiseaseRoot", ToDbValue(record.generalDiseaseRoot));
            yield return new SqlParameter("@generalDiseaseBase", ToDbValue(record.generalDiseaseBase));
            yield return new SqlParameter("@generalDiseaseTrunk", ToDbValue(record.generalDiseaseTrunk));
            yield return new SqlParameter("@generalDiseaseBranch", ToDbValue(record.generalDiseaseBranch));
            yield return new SqlParameter("@generalDiseaseCrown", ToDbValue(record.generalDiseaseCrown));
            yield return new SqlParameter("@generalDiseaseOther", ToDbValue(record.generalDiseaseOther));
            yield return new SqlParameter("@pestOtherNote", ToDbValue(record.pestOtherNote));

            // 細節
            yield return new SqlParameter("@rootDecayPercent", ToDbValue(record.rootDecayPercent));
            yield return new SqlParameter("@rootCavityMaxDiameter", ToDbValue(record.rootCavityMaxDiameter));
            yield return new SqlParameter("@rootWoundMaxDiameter", ToDbValue(record.rootWoundMaxDiameter));
            yield return new SqlParameter("@rootMechanicalDamage", ToDbValue(record.rootMechanicalDamage));
            yield return new SqlParameter("@rootMowingInjury", ToDbValue(record.rootMowingInjury));
            yield return new SqlParameter("@rootInjury", ToDbValue(record.rootInjury));
            yield return new SqlParameter("@rootGirdling", ToDbValue(record.rootGirdling));
            yield return new SqlParameter("@rootOtherNote", ToDbValue(record.rootOtherNote));

            yield return new SqlParameter("@baseDecayPercent", ToDbValue(record.baseDecayPercent));
            yield return new SqlParameter("@baseCavityMaxDiameter", ToDbValue(record.baseCavityMaxDiameter));
            yield return new SqlParameter("@baseWoundMaxDiameter", ToDbValue(record.baseWoundMaxDiameter));
            yield return new SqlParameter("@baseMechanicalDamage", ToDbValue(record.baseMechanicalDamage));
            yield return new SqlParameter("@baseMowingInjury", ToDbValue(record.baseMowingInjury));
            yield return new SqlParameter("@baseOtherNote", ToDbValue(record.baseOtherNote));

            yield return new SqlParameter("@trunkDecayPercent", ToDbValue(record.trunkDecayPercent));
            yield return new SqlParameter("@trunkCavityMaxDiameter", ToDbValue(record.trunkCavityMaxDiameter));
            yield return new SqlParameter("@trunkWoundMaxDiameter", ToDbValue(record.trunkWoundMaxDiameter));
            yield return new SqlParameter("@trunkMechanicalDamage", ToDbValue(record.trunkMechanicalDamage));
            yield return new SqlParameter("@trunkIncludedBark", ToDbValue(record.trunkIncludedBark));
            yield return new SqlParameter("@trunkOtherNote", ToDbValue(record.trunkOtherNote));

            yield return new SqlParameter("@branchDecayPercent", ToDbValue(record.branchDecayPercent));
            yield return new SqlParameter("@branchCavityMaxDiameter", ToDbValue(record.branchCavityMaxDiameter));
            yield return new SqlParameter("@branchWoundMaxDiameter", ToDbValue(record.branchWoundMaxDiameter));
            yield return new SqlParameter("@branchMechanicalDamage", ToDbValue(record.branchMechanicalDamage));
            yield return new SqlParameter("@branchIncludedBark", ToDbValue(record.branchIncludedBark));
            yield return new SqlParameter("@branchDrooping", ToDbValue(record.branchDrooping));
            yield return new SqlParameter("@branchOtherNote", ToDbValue(record.branchOtherNote));

            yield return new SqlParameter("@crownLeafCoveragePercent", ToDbValue(record.crownLeafCoveragePercent));
            yield return new SqlParameter("@crownDeadBranchPercent", ToDbValue(record.crownDeadBranchPercent));
            yield return new SqlParameter("@crownHangingBranch", ToDbValue(record.crownHangingBranch));
            yield return new SqlParameter("@crownOtherNote", ToDbValue(record.crownOtherNote));
            yield return new SqlParameter("@growthNote", ToDbValue(record.growthNote));

            // 修剪與支撐
            yield return new SqlParameter("@pruningWrongDamage", ToDbValue(record.pruningWrongDamage));
            yield return new SqlParameter("@pruningWoundHealing", ToDbValue(record.pruningWoundHealing));
            yield return new SqlParameter("@pruningEpiphyte", ToDbValue(record.pruningEpiphyte));
            yield return new SqlParameter("@pruningParasite", ToDbValue(record.pruningParasite));
            yield return new SqlParameter("@pruningVine", ToDbValue(record.pruningVine));
            yield return new SqlParameter("@pruningOtherNote", ToDbValue(record.pruningOtherNote));
            yield return new SqlParameter("@supportCount", ToDbValue(record.supportCount));
            yield return new SqlParameter("@supportEmbedded", ToDbValue(record.supportEmbedded));
            yield return new SqlParameter("@supportOtherNote", ToDbValue(record.supportOtherNote));

            // 棲地與土壤
            yield return new SqlParameter("@siteCementPercent", ToDbValue(record.siteCementPercent));
            yield return new SqlParameter("@siteAsphaltPercent", ToDbValue(record.siteAsphaltPercent));
            yield return new SqlParameter("@sitePlanter", ToDbValue(record.sitePlanter));
            yield return new SqlParameter("@siteRecreationFacility", ToDbValue(record.siteRecreationFacility));
            yield return new SqlParameter("@siteDebrisStack", ToDbValue(record.siteDebrisStack));
            yield return new SqlParameter("@siteBetweenBuildings", ToDbValue(record.siteBetweenBuildings));
            yield return new SqlParameter("@siteSoilCompaction", ToDbValue(record.siteSoilCompaction));
            yield return new SqlParameter("@siteOverburiedSoil", ToDbValue(record.siteOverburiedSoil));
            yield return new SqlParameter("@siteOtherNote", ToDbValue(record.siteOtherNote));
            yield return new SqlParameter("@soilPh", ToDbValue(record.soilPh));
            yield return new SqlParameter("@soilOrganicMatter", ToDbValue(record.soilOrganicMatter));
            yield return new SqlParameter("@soilEc", ToDbValue(record.soilEc));

            // 管理建議與系統資訊
            yield return new SqlParameter("@managementStatus", ToDbValue(record.managementStatus));
            yield return new SqlParameter("@priority", ToDbValue(record.priority));
            yield return new SqlParameter("@treatmentDescription", ToDbValue(record.treatmentDescription));
        }

        
    }
    

   
}
