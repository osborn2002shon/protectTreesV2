using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataAccess;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;
using static protectTreesV2.Base.DataRowHelper;

namespace protectTreesV2.Health
{
    public class Health
    {
        public enum HealthRecordStatus
        {
            草稿 = 0,
            完稿 = 1
        }

        public class TreeHealthRecord
        {
            public int HealthID { get; set; }
            public int TreeID { get; set; }
            public string SystemTreeNo { get; set; }
            public string AgencyTreeNo { get; set; }
            public string CityName { get; set; }
            public string AreaName { get; set; }
            public string SpeciesName { get; set; }
            public DateTime? SurveyDate { get; set; }
            public string Surveyor { get; set; }
            public HealthRecordStatus DataStatus { get; set; }
            public string Memo { get; set; }
            public byte? TreeSignStatus { get; set; }
            public decimal? Latitude { get; set; }
            public decimal? Longitude { get; set; }
            public decimal? TreeHeight { get; set; }
            public decimal? CanopyArea { get; set; }
            public decimal? Girth100 { get; set; }
            public decimal? Diameter100 { get; set; }
            public decimal? Girth130 { get; set; }
            public decimal? Diameter130 { get; set; }
            public string MeasureNote { get; set; }
            public bool? MajorDiseaseBrownRoot { get; set; }
            public bool? MajorDiseaseGanoderma { get; set; }
            public bool? MajorDiseaseWoodDecayFungus { get; set; }
            public bool? MajorDiseaseCanker { get; set; }
            public bool? MajorDiseaseOther { get; set; }
            public string MajorDiseaseOtherNote { get; set; }
            public bool? MajorPestRootTunnel { get; set; }
            public bool? MajorPestRootChew { get; set; }
            public bool? MajorPestRootLive { get; set; }
            public bool? MajorPestBaseTunnel { get; set; }
            public bool? MajorPestBaseChew { get; set; }
            public bool? MajorPestBaseLive { get; set; }
            public bool? MajorPestTrunkTunnel { get; set; }
            public bool? MajorPestTrunkChew { get; set; }
            public bool? MajorPestTrunkLive { get; set; }
            public bool? MajorPestBranchTunnel { get; set; }
            public bool? MajorPestBranchChew { get; set; }
            public bool? MajorPestBranchLive { get; set; }
            public bool? MajorPestCrownTunnel { get; set; }
            public bool? MajorPestCrownChew { get; set; }
            public bool? MajorPestCrownLive { get; set; }
            public bool? MajorPestOtherTunnel { get; set; }
            public bool? MajorPestOtherChew { get; set; }
            public bool? MajorPestOtherLive { get; set; }
            public string GeneralPestRoot { get; set; }
            public string GeneralPestBase { get; set; }
            public string GeneralPestTrunk { get; set; }
            public string GeneralPestBranch { get; set; }
            public string GeneralPestCrown { get; set; }
            public string GeneralPestOther { get; set; }
            public string GeneralDiseaseRoot { get; set; }
            public string GeneralDiseaseBase { get; set; }
            public string GeneralDiseaseTrunk { get; set; }
            public string GeneralDiseaseBranch { get; set; }
            public string GeneralDiseaseCrown { get; set; }
            public string GeneralDiseaseOther { get; set; }
            public string PestOtherNote { get; set; }
            public decimal? RootDecayPercent { get; set; }
            public decimal? RootCavityMaxDiameter { get; set; }
            public decimal? RootWoundMaxDiameter { get; set; }
            public bool? RootMechanicalDamage { get; set; }
            public bool? RootMowingInjury { get; set; }
            public bool? RootInjury { get; set; }
            public bool? RootGirdling { get; set; }
            public string RootOtherNote { get; set; }
            public decimal? BaseDecayPercent { get; set; }
            public decimal? BaseCavityMaxDiameter { get; set; }
            public decimal? BaseWoundMaxDiameter { get; set; }
            public bool? BaseMechanicalDamage { get; set; }
            public bool? BaseMowingInjury { get; set; }
            public string BaseOtherNote { get; set; }
            public decimal? TrunkDecayPercent { get; set; }
            public decimal? TrunkCavityMaxDiameter { get; set; }
            public decimal? TrunkWoundMaxDiameter { get; set; }
            public bool? TrunkMechanicalDamage { get; set; }
            public bool? TrunkIncludedBark { get; set; }
            public string TrunkOtherNote { get; set; }
            public decimal? BranchDecayPercent { get; set; }
            public decimal? BranchCavityMaxDiameter { get; set; }
            public decimal? BranchWoundMaxDiameter { get; set; }
            public bool? BranchMechanicalDamage { get; set; }
            public bool? BranchIncludedBark { get; set; }
            public bool? BranchDrooping { get; set; }
            public string BranchOtherNote { get; set; }
            public decimal? CrownLeafCoveragePercent { get; set; }
            public decimal? CrownDeadBranchPercent { get; set; }
            public bool? CrownHangingBranch { get; set; }
            public string CrownOtherNote { get; set; }
            public string PruningWrongDamage { get; set; }
            public bool? PruningWoundHealing { get; set; }
            public bool? PruningEpiphyte { get; set; }
            public bool? PruningParasite { get; set; }
            public bool? PruningVine { get; set; }
            public string PruningOtherNote { get; set; }
            public int? SupportCount { get; set; }
            public bool? SupportEmbedded { get; set; }
            public string SupportOtherNote { get; set; }
            public decimal? SiteCementPercent { get; set; }
            public decimal? SiteAsphaltPercent { get; set; }
            public bool? SitePlanter { get; set; }
            public bool? SiteRecreationFacility { get; set; }
            public bool? SiteDebrisStack { get; set; }
            public bool? SiteBetweenBuildings { get; set; }
            public bool? SiteSoilCompaction { get; set; }
            public bool? SiteOverburiedSoil { get; set; }
            public string SiteOtherNote { get; set; }
            public decimal? SoilPh { get; set; }
            public decimal? SoilOrganicMatter { get; set; }
            public decimal? SoilEc { get; set; }
            public string ManagementStatus { get; set; }
            public string Priority { get; set; }
            public string TreatmentDescription { get; set; }
            public string SourceUnit { get; set; }
            public int? SourceUnitID { get; set; }
            public int InsertAccountID { get; set; }
            public DateTime InsertDateTime { get; set; }
            public int? UpdateAccountID { get; set; }
            public DateTime? UpdateDateTime { get; set; }

            public string SurveyDateDisplay => SurveyDate.HasValue ? SurveyDate.Value.ToString("yyyy/MM/dd") : string.Empty;

            public string DataStatusText => DataStatus == HealthRecordStatus.完稿 ? "完稿" : "草稿";

            public string LastUpdateDisplay
            {
                get
                {
                    if (UpdateDateTime.HasValue)
                    {
                        return UpdateDateTime.Value.ToString("yyyy/MM/dd HH:mm");
                    }
                    return InsertDateTime == DateTime.MinValue ? string.Empty : InsertDateTime.ToString("yyyy/MM/dd HH:mm");
                }
            }
        }

        public class TreeHealthPhoto
        {
            public int PhotoID { get; set; }
            public int HealthID { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public int? FileSize { get; set; }
            public string Caption { get; set; }
            public DateTime InsertDateTime { get; set; }
        }

        public class TreeHealthAttachment
        {
            public int AttachmentID { get; set; }
            public int HealthID { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public int? FileSize { get; set; }
            public string Description { get; set; }
            public DateTime InsertDateTime { get; set; }
        }

        public class TreeHealthFilter
        {
            public int? CityID { get; set; }
            public int? AreaID { get; set; }
            public string SystemTreeNo { get; set; }
            public DateTime? SurveyDateStart { get; set; }
            public DateTime? SurveyDateEnd { get; set; }
            public HealthRecordStatus? DataStatus { get; set; }
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

            public string treeStatusText => treeStatus.HasValue ? protectTreesV2.TreeCatalog.TreeService.GetStatusText(treeStatus.Value) : string.Empty;

            public DateTime? surveyDate { get; set; }
            public string surveyor { get; set; }
            public HealthRecordStatus? dataStatus { get; set; }
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

        public  List<HealthMainQueryResult> GetHealthMainList(HealthMainQueryFilter filter, int currentUserId)
        {
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            // ==========================================
            // 1. SQL 架構：Tree_Record (主) + Top 1 HealthRecord
            // ==========================================
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
                    COALESCE(areaInfo.city, cityInfo.city, record.cityName) AS cityName,
                    COALESCE(areaInfo.area, record.areaName) AS areaName,
            
                    /* 樹種名稱 */
                    COALESCE(species.commonName, record.speciesCommonName) AS speciesName,
            
                    /* 最新健檢紀錄欄位 (可能為 NULL) */
                    latest_health.healthID, 
                    latest_health.surveyDate, 
                    latest_health.surveyor, 
                    latest_health.dataStatus,
                    COALESCE(latest_health.updateDateTime, latest_health.insertDateTime) AS lastUpdate,
                    
                    /* 判斷是否已加入設定  */
                    CASE WHEN batch.treeID IS NOT NULL THEN 1 ELSE 0 END AS isAdded

                FROM Tree_Record record
        
                /* [核心修改] 抓取最新一筆健檢紀錄 */
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
                LEFT JOIN Tree_HealthBatchSetting batch ON record.treeID = batch.treeID AND batch.accountID = @currentUserId
            ";

            // 加入草稿參數
            // 若 filter.includeDraft 為 true，傳入 1，否則 0
            parameters.Add(new SqlParameter("@incDraft", filter.includeDraft ? 1 : 0));

            //使用者ID檢查設定檔
            parameters.Add(new SqlParameter("@currentUserId", currentUserId));

            // ==========================================
            // 2. WHERE 篩選條件
            // ==========================================
            whereClauses.Add("record.removeDateTime IS NULL");
            whereClauses.Add("record.editStatus = 1");

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
            // 3. 排序邏輯 (包含特殊預設排序)
            // ==========================================
            string sortSql = "";
            string dir = (filter.sortDirection == "DESC") ? "DESC" : "ASC";

            // 處理特殊排序需求
            if (filter.sortExpression == "DefaultSort" || string.IsNullOrEmpty(filter.sortExpression))
            {
                // 題目要求：無健檢紀錄排最上方 (healthID IS NULL -> 0, ELSE -> 1)
                // 接著依 systemTreeNo ASC
                sortSql = "ORDER BY (CASE WHEN latest_health.healthID IS NULL THEN 0 ELSE 1 END) ASC, record.systemTreeNo ASC";
            }
            else if (filter.sortExpression == "areaID")
            {
                // 題目要求：縣市鄉鎮排序要用 areaID
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

                // 白名單檢查 (略，建議加上)
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

                    // 健檢資料 (若無紀錄，這些 GetString 會回傳 null，符合要求)
                    item.healthID = DataRowHelper.GetNullableInt(row, "healthID");
                    item.surveyDate = DataRowHelper.GetNullableDateTime(row, "surveyDate");
                    item.surveyor = DataRowHelper.GetString(row, "surveyor");

                    // 狀態 Enum
                    string tStatus = DataRowHelper.GetString(row, "treeStatus");
                    if (!string.IsNullOrEmpty(tStatus) && Enum.TryParse<TreeStatus>(tStatus, out var ts))
                        item.treeStatus = ts;

                    int? dStatus = DataRowHelper.GetNullableInt(row, "dataStatus");
                    if (dStatus.HasValue && Enum.IsDefined(typeof(HealthRecordStatus), dStatus.Value))
                        item.dataStatus = (HealthRecordStatus)dStatus.Value;

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
        /// <param name="accountId"></param>
        /// <returns></returns>
        public  List<HealthBatchSettingResult> GetHealthBatchSetting(int accountId)
        {
            string sql = @"
                SELECT s.settingID, s.insertDateTime,
                       record.treeID, 
                       record.systemTreeNo, 
                       record.agencyTreeNo, 
                       record.agencyJurisdictionCode,
               
                       /* 樹種 */
                       COALESCE(species.commonName, record.speciesCommonName) AS speciesName,
               
                       record.manager
               
                FROM Tree_HealthBatchSetting s
                JOIN Tree_Record record ON s.treeID = record.treeID
        
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
        
                WHERE s.accountID = @accountID
                ORDER BY s.insertDateTime DESC
            ";

            var result = new List<HealthBatchSettingResult>();

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@accountID", accountId));
                foreach (DataRow row in dt.Rows)
                {
                    var item = new HealthBatchSettingResult();

                    item.settingID = DataRowHelper.GetNullableInt(row, "settingID") ?? 0;
                    item.insertDateTime = DataRowHelper.GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue;

                    item.treeID = DataRowHelper.GetNullableInt(row, "treeID") ?? 0;
                    item.systemTreeNo = DataRowHelper.GetString(row, "systemTreeNo");
                    item.agencyTreeNo = DataRowHelper.GetString(row, "agencyTreeNo");
                    item.agencyJurisdictionCode = DataRowHelper.GetString(row, "agencyJurisdictionCode");

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
        /// <param name="accountId"></param>
        /// <param name="treeId"></param>
        public  void AddToHealthBatchSetting(int accountId, int treeId)
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
                    new SqlParameter("@accountID", accountId),
                    new SqlParameter("@treeID", treeId));
            }
        }

        /// <summary>
        ///  移除批次設定 (硬刪除)
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="treeId"></param>
        public void RemoveFromHealthBatchSetting(int accountId, int? treeId = null)
        {
            string sql = "";
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@accountID", accountId));

            if (treeId.HasValue)
            {
                // 移除單筆
                sql = "DELETE FROM Tree_HealthBatchSetting WHERE accountID = @accountID AND treeID = @treeID";
                parameters.Add(new SqlParameter("@treeID", treeId.Value));
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

        public List<HealthMainQueryResult> GetHealthRecordList(HealthRecordListFilter filter, int currentUserId)
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
                    COALESCE(areaInfo.city, cityInfo.city, record.cityName) AS cityName,
                    COALESCE(areaInfo.area, record.areaName) AS areaName,
            
                    /* 樹種名稱 */
                    COALESCE(species.commonName, record.speciesCommonName) AS speciesName,
            
                    record.manager

                FROM Tree_HealthRecord health
                JOIN Tree_Record record ON health.treeID = record.treeID

                /* 關聯表 */
                OUTER APPLY (SELECT TOP 1 city FROM System_Taiwan WHERE cityID = record.cityID) cityInfo
                LEFT JOIN System_Taiwan areaInfo ON areaInfo.twID = record.areaID
                LEFT JOIN Tree_Species species ON species.speciesID = record.speciesID
            ";

            // ==========================================
            // 2. 篩選條件
            // ==========================================
            whereClauses.Add("health.removeDateTime IS NULL");
            whereClauses.Add("record.removeDateTime IS NULL");

            // 必須是 editStatus = 1 (已完稿/有樹號) 才能有健檢紀錄
            whereClauses.Add("record.editStatus = 1");

            // Scope: 我的紀錄 vs 單位全部
            if (filter.scope == "My")
            {
                whereClauses.Add("health.insertAccountID = @userID");
                parameters.Add(new SqlParameter("@userID", currentUserId));
            }
            // 若是 Unit (單位全部)，不加額外過濾，顯示所有

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
                    if (dStatus.HasValue) item.dataStatus = (HealthRecordStatus)dStatus.Value;

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

        public static partial class TreeService
        {
            private const string HealthRecordSelectColumns = @"
 h.healthID, h.treeID, h.surveyDate, h.surveyor, h.dataStatus, h.memo,
 h.treeSignStatus, h.latitude, h.longitude, h.treeHeight, h.canopyArea,
 h.girth100, h.diameter100, h.girth130, h.diameter130, h.measureNote,
 h.majorDiseaseBrownRoot, h.majorDiseaseGanoderma, h.majorDiseaseWoodDecayFungus, h.majorDiseaseCanker, h.majorDiseaseOther, h.majorDiseaseOtherNote,
 h.majorPestRootTunnel, h.majorPestRootChew, h.majorPestRootLive,
 h.majorPestBaseTunnel, h.majorPestBaseChew, h.majorPestBaseLive,
 h.majorPestTrunkTunnel, h.majorPestTrunkChew, h.majorPestTrunkLive,
 h.majorPestBranchTunnel, h.majorPestBranchChew, h.majorPestBranchLive,
 h.majorPestCrownTunnel, h.majorPestCrownChew, h.majorPestCrownLive,
 h.majorPestOtherTunnel, h.majorPestOtherChew, h.majorPestOtherLive,
 h.generalPestRoot, h.generalPestBase, h.generalPestTrunk, h.generalPestBranch, h.generalPestCrown, h.generalPestOther,
 h.generalDiseaseRoot, h.generalDiseaseBase, h.generalDiseaseTrunk, h.generalDiseaseBranch, h.generalDiseaseCrown, h.generalDiseaseOther,
 h.pestOtherNote,
 h.rootDecayPercent, h.rootCavityMaxDiameter, h.rootWoundMaxDiameter, h.rootMechanicalDamage, h.rootMowingInjury, h.rootInjury, h.rootGirdling, h.rootOtherNote,
 h.baseDecayPercent, h.baseCavityMaxDiameter, h.baseWoundMaxDiameter, h.baseMechanicalDamage, h.baseMowingInjury, h.baseOtherNote,
 h.trunkDecayPercent, h.trunkCavityMaxDiameter, h.trunkWoundMaxDiameter, h.trunkMechanicalDamage, h.trunkIncludedBark, h.trunkOtherNote,
 h.branchDecayPercent, h.branchCavityMaxDiameter, h.branchWoundMaxDiameter, h.branchMechanicalDamage, h.branchIncludedBark, h.branchDrooping, h.branchOtherNote,
 h.crownLeafCoveragePercent, h.crownDeadBranchPercent, h.crownHangingBranch, h.crownOtherNote,
 h.pruningWrongDamage, h.pruningWoundHealing, h.pruningEpiphyte, h.pruningParasite, h.pruningVine, h.pruningOtherNote,
 h.supportCount, h.supportEmbedded, h.supportOtherNote,
 h.siteCementPercent, h.siteAsphaltPercent, h.sitePlanter, h.siteRecreationFacility, h.siteDebrisStack, h.siteBetweenBuildings, h.siteSoilCompaction, h.siteOverburiedSoil, h.siteOtherNote,
 h.soilPh, h.soilOrganicMatter, h.soilEc,
 h.managementStatus, h.priority, h.treatmentDescription,
 h.sourceUnit, h.sourceUnitID, h.insertAccountID, h.insertDateTime,
 h.updateAccountID, h.updateDateTime,
 t.systemTreeNo, t.agencyTreeNo, t.cityName, t.areaName,
 t.speciesCommonName";

            private static TreeHealthRecord ToHealthRecord(DataRow row)
            {
                if (row == null)
                {
                    return null;
                }

                int statusValue = GetNullableInt(row, "dataStatus") ?? 0;

                return new TreeHealthRecord
                {
                    HealthID = GetNullableInt(row, "healthID") ?? 0,
                    TreeID = GetNullableInt(row, "treeID") ?? 0,
                    SurveyDate = GetNullableDateTime(row, "surveyDate"),
                    Surveyor = GetString(row, "surveyor"),
                    DataStatus = Enum.IsDefined(typeof(HealthRecordStatus), statusValue)
                        ? (HealthRecordStatus)statusValue
                        : HealthRecordStatus.草稿,
                    Memo = GetString(row, "memo"),
                    TreeSignStatus = GetNullableInt(row, "treeSignStatus") is int signStatus ? (byte?)signStatus : null,
                    Latitude = GetNullableDecimal(row, "latitude"),
                    Longitude = GetNullableDecimal(row, "longitude"),
                    TreeHeight = GetNullableDecimal(row, "treeHeight"),
                    CanopyArea = GetNullableDecimal(row, "canopyArea"),
                    Girth100 = GetNullableDecimal(row, "girth100"),
                    Diameter100 = GetNullableDecimal(row, "diameter100"),
                    Girth130 = GetNullableDecimal(row, "girth130"),
                    Diameter130 = GetNullableDecimal(row, "diameter130"),
                    MeasureNote = GetString(row, "measureNote"),
                    MajorDiseaseBrownRoot = GetNullableBoolean(row, "majorDiseaseBrownRoot"),
                    MajorDiseaseGanoderma = GetNullableBoolean(row, "majorDiseaseGanoderma"),
                    MajorDiseaseWoodDecayFungus = GetNullableBoolean(row, "majorDiseaseWoodDecayFungus"),
                    MajorDiseaseCanker = GetNullableBoolean(row, "majorDiseaseCanker"),
                    MajorDiseaseOther = GetNullableBoolean(row, "majorDiseaseOther"),
                    MajorDiseaseOtherNote = GetString(row, "majorDiseaseOtherNote"),
                    MajorPestRootTunnel = GetNullableBoolean(row, "majorPestRootTunnel"),
                    MajorPestRootChew = GetNullableBoolean(row, "majorPestRootChew"),
                    MajorPestRootLive = GetNullableBoolean(row, "majorPestRootLive"),
                    MajorPestBaseTunnel = GetNullableBoolean(row, "majorPestBaseTunnel"),
                    MajorPestBaseChew = GetNullableBoolean(row, "majorPestBaseChew"),
                    MajorPestBaseLive = GetNullableBoolean(row, "majorPestBaseLive"),
                    MajorPestTrunkTunnel = GetNullableBoolean(row, "majorPestTrunkTunnel"),
                    MajorPestTrunkChew = GetNullableBoolean(row, "majorPestTrunkChew"),
                    MajorPestTrunkLive = GetNullableBoolean(row, "majorPestTrunkLive"),
                    MajorPestBranchTunnel = GetNullableBoolean(row, "majorPestBranchTunnel"),
                    MajorPestBranchChew = GetNullableBoolean(row, "majorPestBranchChew"),
                    MajorPestBranchLive = GetNullableBoolean(row, "majorPestBranchLive"),
                    MajorPestCrownTunnel = GetNullableBoolean(row, "majorPestCrownTunnel"),
                    MajorPestCrownChew = GetNullableBoolean(row, "majorPestCrownChew"),
                    MajorPestCrownLive = GetNullableBoolean(row, "majorPestCrownLive"),
                    MajorPestOtherTunnel = GetNullableBoolean(row, "majorPestOtherTunnel"),
                    MajorPestOtherChew = GetNullableBoolean(row, "majorPestOtherChew"),
                    MajorPestOtherLive = GetNullableBoolean(row, "majorPestOtherLive"),
                    GeneralPestRoot = GetString(row, "generalPestRoot"),
                    GeneralPestBase = GetString(row, "generalPestBase"),
                    GeneralPestTrunk = GetString(row, "generalPestTrunk"),
                    GeneralPestBranch = GetString(row, "generalPestBranch"),
                    GeneralPestCrown = GetString(row, "generalPestCrown"),
                    GeneralPestOther = GetString(row, "generalPestOther"),
                    GeneralDiseaseRoot = GetString(row, "generalDiseaseRoot"),
                    GeneralDiseaseBase = GetString(row, "generalDiseaseBase"),
                    GeneralDiseaseTrunk = GetString(row, "generalDiseaseTrunk"),
                    GeneralDiseaseBranch = GetString(row, "generalDiseaseBranch"),
                    GeneralDiseaseCrown = GetString(row, "generalDiseaseCrown"),
                    GeneralDiseaseOther = GetString(row, "generalDiseaseOther"),
                    PestOtherNote = GetString(row, "pestOtherNote"),
                    RootDecayPercent = GetNullableDecimal(row, "rootDecayPercent"),
                    RootCavityMaxDiameter = GetNullableDecimal(row, "rootCavityMaxDiameter"),
                    RootWoundMaxDiameter = GetNullableDecimal(row, "rootWoundMaxDiameter"),
                    RootMechanicalDamage = GetNullableBoolean(row, "rootMechanicalDamage"),
                    RootMowingInjury = GetNullableBoolean(row, "rootMowingInjury"),
                    RootInjury = GetNullableBoolean(row, "rootInjury"),
                    RootGirdling = GetNullableBoolean(row, "rootGirdling"),
                    RootOtherNote = GetString(row, "rootOtherNote"),
                    BaseDecayPercent = GetNullableDecimal(row, "baseDecayPercent"),
                    BaseCavityMaxDiameter = GetNullableDecimal(row, "baseCavityMaxDiameter"),
                    BaseWoundMaxDiameter = GetNullableDecimal(row, "baseWoundMaxDiameter"),
                    BaseMechanicalDamage = GetNullableBoolean(row, "baseMechanicalDamage"),
                    BaseMowingInjury = GetNullableBoolean(row, "baseMowingInjury"),
                    BaseOtherNote = GetString(row, "baseOtherNote"),
                    TrunkDecayPercent = GetNullableDecimal(row, "trunkDecayPercent"),
                    TrunkCavityMaxDiameter = GetNullableDecimal(row, "trunkCavityMaxDiameter"),
                    TrunkWoundMaxDiameter = GetNullableDecimal(row, "trunkWoundMaxDiameter"),
                    TrunkMechanicalDamage = GetNullableBoolean(row, "trunkMechanicalDamage"),
                    TrunkIncludedBark = GetNullableBoolean(row, "trunkIncludedBark"),
                    TrunkOtherNote = GetString(row, "trunkOtherNote"),
                    BranchDecayPercent = GetNullableDecimal(row, "branchDecayPercent"),
                    BranchCavityMaxDiameter = GetNullableDecimal(row, "branchCavityMaxDiameter"),
                    BranchWoundMaxDiameter = GetNullableDecimal(row, "branchWoundMaxDiameter"),
                    BranchMechanicalDamage = GetNullableBoolean(row, "branchMechanicalDamage"),
                    BranchIncludedBark = GetNullableBoolean(row, "branchIncludedBark"),
                    BranchDrooping = GetNullableBoolean(row, "branchDrooping"),
                    BranchOtherNote = GetString(row, "branchOtherNote"),
                    CrownLeafCoveragePercent = GetNullableDecimal(row, "crownLeafCoveragePercent"),
                    CrownDeadBranchPercent = GetNullableDecimal(row, "crownDeadBranchPercent"),
                    CrownHangingBranch = GetNullableBoolean(row, "crownHangingBranch"),
                    CrownOtherNote = GetString(row, "crownOtherNote"),
                    PruningWrongDamage = GetString(row, "pruningWrongDamage"),
                    PruningWoundHealing = GetNullableBoolean(row, "pruningWoundHealing"),
                    PruningEpiphyte = GetNullableBoolean(row, "pruningEpiphyte"),
                    PruningParasite = GetNullableBoolean(row, "pruningParasite"),
                    PruningVine = GetNullableBoolean(row, "pruningVine"),
                    PruningOtherNote = GetString(row, "pruningOtherNote"),
                    SupportCount = GetNullableInt(row, "supportCount"),
                    SupportEmbedded = GetNullableBoolean(row, "supportEmbedded"),
                    SupportOtherNote = GetString(row, "supportOtherNote"),
                    SiteCementPercent = GetNullableDecimal(row, "siteCementPercent"),
                    SiteAsphaltPercent = GetNullableDecimal(row, "siteAsphaltPercent"),
                    SitePlanter = GetNullableBoolean(row, "sitePlanter"),
                    SiteRecreationFacility = GetNullableBoolean(row, "siteRecreationFacility"),
                    SiteDebrisStack = GetNullableBoolean(row, "siteDebrisStack"),
                    SiteBetweenBuildings = GetNullableBoolean(row, "siteBetweenBuildings"),
                    SiteSoilCompaction = GetNullableBoolean(row, "siteSoilCompaction"),
                    SiteOverburiedSoil = GetNullableBoolean(row, "siteOverburiedSoil"),
                    SiteOtherNote = GetString(row, "siteOtherNote"),
                    SoilPh = GetNullableDecimal(row, "soilPh"),
                    SoilOrganicMatter = GetNullableDecimal(row, "soilOrganicMatter"),
                    SoilEc = GetNullableDecimal(row, "soilEc"),
                    ManagementStatus = GetString(row, "managementStatus"),
                    Priority = GetString(row, "priority"),
                    TreatmentDescription = GetString(row, "treatmentDescription"),
                    SourceUnit = GetString(row, "sourceUnit"),
                    SourceUnitID = GetNullableInt(row, "sourceUnitID"),
                    InsertAccountID = GetNullableInt(row, "insertAccountID") ?? 0,
                    InsertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue,
                    UpdateAccountID = GetNullableInt(row, "updateAccountID"),
                    UpdateDateTime = GetNullableDateTime(row, "updateDateTime"),
                    SystemTreeNo = GetString(row, "systemTreeNo"),
                    AgencyTreeNo = GetString(row, "agencyTreeNo"),
                    CityName = GetString(row, "cityName"),
                    AreaName = GetString(row, "areaName"),
                    SpeciesName = GetString(row, "speciesCommonName")
                };
            }

            private static TreeHealthPhoto ToHealthPhoto(DataRow row)
            {
                if (row == null)
                {
                    return null;
                }

                return new TreeHealthPhoto
                {
                    PhotoID = GetNullableInt(row, "photoID") ?? 0,
                    HealthID = GetNullableInt(row, "healthID") ?? 0,
                    FileName = GetString(row, "fileName"),
                    FilePath = GetString(row, "filePath"),
                    FileSize = GetNullableInt(row, "fileSize"),
                    Caption = GetString(row, "caption"),
                    InsertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue
                };
            }

            private static TreeHealthAttachment ToHealthAttachment(DataRow row)
            {
                if (row == null)
                {
                    return null;
                }

                return new TreeHealthAttachment
                {
                    AttachmentID = GetNullableInt(row, "attachmentID") ?? 0,
                    HealthID = GetNullableInt(row, "healthID") ?? 0,
                    FileName = GetString(row, "fileName"),
                    FilePath = GetString(row, "filePath"),
                    FileSize = GetNullableInt(row, "fileSize"),
                    Description = GetString(row, "description"),
                    InsertDateTime = GetNullableDateTime(row, "insertDateTime") ?? DateTime.MinValue
                };
            }

            public static List<TreeHealthRecord> GetHealthRecordsByTree(int treeId)
            {
                const string sql = @"SELECT " + HealthRecordSelectColumns + @"
FROM Tree_HealthRecord h
JOIN Tree_Record t ON h.treeID=t.treeID
WHERE h.treeID=@treeID AND h.removeDateTime IS NULL AND t.removeDateTime IS NULL
ORDER BY h.surveyDate DESC, h.healthID DESC";

                using (var da = new MS_SQL())
                {
                    var dt = da.GetDataTable(sql, new SqlParameter("@treeID", treeId));
                    return dt.Rows.Cast<DataRow>().Select(ToHealthRecord).ToList();
                }
            }

            public static TreeHealthRecord GetHealthRecord(int healthId)
            {
                const string sql = @"SELECT " + HealthRecordSelectColumns + @"
FROM Tree_HealthRecord h
JOIN Tree_Record t ON h.treeID=t.treeID
WHERE h.healthID=@id AND h.removeDateTime IS NULL AND t.removeDateTime IS NULL";

                using (var da = new MS_SQL())
                {
                    var dt = da.GetDataTable(sql, new SqlParameter("@id", healthId));
                    return dt.Rows.Count > 0 ? ToHealthRecord(dt.Rows[0]) : null;
                }
            }

            public static TreeHealthRecord GetHealthRecordByTreeAndDate(int treeId, DateTime surveyDate)
            {
                const string sql = @"SELECT TOP 1 " + HealthRecordSelectColumns + @"
FROM Tree_HealthRecord h
JOIN Tree_Record t ON h.treeID=t.treeID
WHERE h.treeID=@treeID AND h.removeDateTime IS NULL AND t.removeDateTime IS NULL AND CAST(h.surveyDate AS DATE)=@surveyDate";

                using (var da = new MS_SQL())
                {
                    var dt = da.GetDataTable(sql,
                        new SqlParameter("@treeID", treeId),
                        new SqlParameter("@surveyDate", surveyDate.Date));
                    return dt.Rows.Count > 0 ? ToHealthRecord(dt.Rows[0]) : null;
                }
            }

            public static List<TreeHealthRecord> SearchHealthRecords(TreeHealthFilter filter)
            {
                var sql = new StringBuilder();
                sql.Append("SELECT ").Append(HealthRecordSelectColumns).Append(@"
FROM Tree_HealthRecord h
JOIN Tree_Record t ON h.treeID=t.treeID
WHERE h.removeDateTime IS NULL AND t.removeDateTime IS NULL");

                var parameters = new List<SqlParameter>();

                if (filter != null)
                {
                    if (filter.CityID.HasValue)
                    {
                        sql.Append(" AND t.cityID=@cityID");
                        parameters.Add(new SqlParameter("@cityID", filter.CityID.Value));
                    }

                    if (filter.AreaID.HasValue)
                    {
                        sql.Append(" AND t.areaID=@areaID");
                        parameters.Add(new SqlParameter("@areaID", filter.AreaID.Value));
                    }

                    if (!string.IsNullOrWhiteSpace(filter.SystemTreeNo))
                    {
                        sql.Append(" AND t.systemTreeNo LIKE @systemTreeNo");
                        parameters.Add(new SqlParameter("@systemTreeNo", "%" + filter.SystemTreeNo.Trim() + "%"));
                    }

                    if (filter.SurveyDateStart.HasValue)
                    {
                        sql.Append(" AND h.surveyDate>=@surveyDateStart");
                        parameters.Add(new SqlParameter("@surveyDateStart", filter.SurveyDateStart.Value.Date));
                    }

                    if (filter.SurveyDateEnd.HasValue)
                    {
                        sql.Append(" AND h.surveyDate<=@surveyDateEnd");
                        parameters.Add(new SqlParameter("@surveyDateEnd", filter.SurveyDateEnd.Value.Date));
                    }

                    if (filter.DataStatus.HasValue)
                    {
                        sql.Append(" AND h.dataStatus=@dataStatus");
                        parameters.Add(new SqlParameter("@dataStatus", (int)filter.DataStatus.Value));
                    }
                }

                sql.Append(" ORDER BY h.surveyDate DESC, h.healthID DESC");

                using (var da = new MS_SQL())
                {
                    var dt = da.GetDataTable(sql.ToString(), parameters.ToArray());
                    return dt.Rows.Cast<DataRow>().Select(ToHealthRecord).ToList();
                }
            }

            public static int SaveHealthRecord(TreeHealthRecord record, int accountId)
            {
                if (record == null) throw new ArgumentNullException(nameof(record));

                if (record.HealthID <= 0)
                {
                    const string insertSql = @"INSERT INTO Tree_HealthRecord
(treeID, surveyDate, surveyor, dataStatus, memo, treeSignStatus, latitude, longitude, treeHeight, canopyArea,
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
 crownLeafCoveragePercent, crownDeadBranchPercent, crownHangingBranch, crownOtherNote,
 pruningWrongDamage, pruningWoundHealing, pruningEpiphyte, pruningParasite, pruningVine, pruningOtherNote,
 supportCount, supportEmbedded, supportOtherNote,
 siteCementPercent, siteAsphaltPercent, sitePlanter, siteRecreationFacility, siteDebrisStack, siteBetweenBuildings, siteSoilCompaction, siteOverburiedSoil, siteOtherNote,
 soilPh, soilOrganicMatter, soilEc,
 managementStatus, priority, treatmentDescription,
 sourceUnit, sourceUnitID, insertAccountID, insertDateTime)
OUTPUT INSERTED.healthID
VALUES
(@treeID, @surveyDate, @surveyor, @dataStatus, @memo, @treeSignStatus, @latitude, @longitude, @treeHeight, @canopyArea,
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
 @crownLeafCoveragePercent, @crownDeadBranchPercent, @crownHangingBranch, @crownOtherNote,
 @pruningWrongDamage, @pruningWoundHealing, @pruningEpiphyte, @pruningParasite, @pruningVine, @pruningOtherNote,
 @supportCount, @supportEmbedded, @supportOtherNote,
 @siteCementPercent, @siteAsphaltPercent, @sitePlanter, @siteRecreationFacility, @siteDebrisStack, @siteBetweenBuildings, @siteSoilCompaction, @siteOverburiedSoil, @siteOtherNote,
 @soilPh, @soilOrganicMatter, @soilEc,
 @managementStatus, @priority, @treatmentDescription,
 @sourceUnit, @sourceUnitID, @accountId, GETDATE())";

                    using (var da = new MS_SQL())
                    {
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@treeID", record.TreeID),
                        new SqlParameter("@accountId", accountId)
                    };
                        parameters.AddRange(GetHealthRecordFieldParameters(record));

                        return Convert.ToInt32(da.ExcuteScalar(insertSql, parameters.ToArray()));
                    }
                }
                else
                {
                    const string updateSql = @"UPDATE Tree_HealthRecord
SET surveyDate=@surveyDate,
    surveyor=@surveyor,
    dataStatus=@dataStatus,
    memo=@memo,
    treeSignStatus=@treeSignStatus,
    latitude=@latitude,
    longitude=@longitude,
    treeHeight=@treeHeight,
    canopyArea=@canopyArea,
    girth100=@girth100,
    diameter100=@diameter100,
    girth130=@girth130,
    diameter130=@diameter130,
    measureNote=@measureNote,
    majorDiseaseBrownRoot=@majorDiseaseBrownRoot,
    majorDiseaseGanoderma=@majorDiseaseGanoderma,
    majorDiseaseWoodDecayFungus=@majorDiseaseWoodDecayFungus,
    majorDiseaseCanker=@majorDiseaseCanker,
    majorDiseaseOther=@majorDiseaseOther,
    majorDiseaseOtherNote=@majorDiseaseOtherNote,
    majorPestRootTunnel=@majorPestRootTunnel,
    majorPestRootChew=@majorPestRootChew,
    majorPestRootLive=@majorPestRootLive,
    majorPestBaseTunnel=@majorPestBaseTunnel,
    majorPestBaseChew=@majorPestBaseChew,
    majorPestBaseLive=@majorPestBaseLive,
    majorPestTrunkTunnel=@majorPestTrunkTunnel,
    majorPestTrunkChew=@majorPestTrunkChew,
    majorPestTrunkLive=@majorPestTrunkLive,
    majorPestBranchTunnel=@majorPestBranchTunnel,
    majorPestBranchChew=@majorPestBranchChew,
    majorPestBranchLive=@majorPestBranchLive,
    majorPestCrownTunnel=@majorPestCrownTunnel,
    majorPestCrownChew=@majorPestCrownChew,
    majorPestCrownLive=@majorPestCrownLive,
    majorPestOtherTunnel=@majorPestOtherTunnel,
    majorPestOtherChew=@majorPestOtherChew,
    majorPestOtherLive=@majorPestOtherLive,
    generalPestRoot=@generalPestRoot,
    generalPestBase=@generalPestBase,
    generalPestTrunk=@generalPestTrunk,
    generalPestBranch=@generalPestBranch,
    generalPestCrown=@generalPestCrown,
    generalPestOther=@generalPestOther,
    generalDiseaseRoot=@generalDiseaseRoot,
    generalDiseaseBase=@generalDiseaseBase,
    generalDiseaseTrunk=@generalDiseaseTrunk,
    generalDiseaseBranch=@generalDiseaseBranch,
    generalDiseaseCrown=@generalDiseaseCrown,
    generalDiseaseOther=@generalDiseaseOther,
    pestOtherNote=@pestOtherNote,
    rootDecayPercent=@rootDecayPercent,
    rootCavityMaxDiameter=@rootCavityMaxDiameter,
    rootWoundMaxDiameter=@rootWoundMaxDiameter,
    rootMechanicalDamage=@rootMechanicalDamage,
    rootMowingInjury=@rootMowingInjury,
    rootInjury=@rootInjury,
    rootGirdling=@rootGirdling,
    rootOtherNote=@rootOtherNote,
    baseDecayPercent=@baseDecayPercent,
    baseCavityMaxDiameter=@baseCavityMaxDiameter,
    baseWoundMaxDiameter=@baseWoundMaxDiameter,
    baseMechanicalDamage=@baseMechanicalDamage,
    baseMowingInjury=@baseMowingInjury,
    baseOtherNote=@baseOtherNote,
    trunkDecayPercent=@trunkDecayPercent,
    trunkCavityMaxDiameter=@trunkCavityMaxDiameter,
    trunkWoundMaxDiameter=@trunkWoundMaxDiameter,
    trunkMechanicalDamage=@trunkMechanicalDamage,
    trunkIncludedBark=@trunkIncludedBark,
    trunkOtherNote=@trunkOtherNote,
    branchDecayPercent=@branchDecayPercent,
    branchCavityMaxDiameter=@branchCavityMaxDiameter,
    branchWoundMaxDiameter=@branchWoundMaxDiameter,
    branchMechanicalDamage=@branchMechanicalDamage,
    branchIncludedBark=@branchIncludedBark,
    branchDrooping=@branchDrooping,
    branchOtherNote=@branchOtherNote,
    crownLeafCoveragePercent=@crownLeafCoveragePercent,
    crownDeadBranchPercent=@crownDeadBranchPercent,
    crownHangingBranch=@crownHangingBranch,
    crownOtherNote=@crownOtherNote,
    pruningWrongDamage=@pruningWrongDamage,
    pruningWoundHealing=@pruningWoundHealing,
    pruningEpiphyte=@pruningEpiphyte,
    pruningParasite=@pruningParasite,
    pruningVine=@pruningVine,
    pruningOtherNote=@pruningOtherNote,
    supportCount=@supportCount,
    supportEmbedded=@supportEmbedded,
    supportOtherNote=@supportOtherNote,
    siteCementPercent=@siteCementPercent,
    siteAsphaltPercent=@siteAsphaltPercent,
    sitePlanter=@sitePlanter,
    siteRecreationFacility=@siteRecreationFacility,
    siteDebrisStack=@siteDebrisStack,
    siteBetweenBuildings=@siteBetweenBuildings,
    siteSoilCompaction=@siteSoilCompaction,
    siteOverburiedSoil=@siteOverburiedSoil,
    siteOtherNote=@siteOtherNote,
    soilPh=@soilPh,
    soilOrganicMatter=@soilOrganicMatter,
    soilEc=@soilEc,
    managementStatus=@managementStatus,
    priority=@priority,
    treatmentDescription=@treatmentDescription,
    sourceUnit=@sourceUnit,
    sourceUnitID=@sourceUnitID,
    updateAccountID=@accountId,
    updateDateTime=GETDATE()
WHERE healthID=@id AND removeDateTime IS NULL";

                    using (var da = new MS_SQL())
                    {
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@accountId", accountId),
                        new SqlParameter("@id", record.HealthID)
                    };
                        parameters.AddRange(GetHealthRecordFieldParameters(record));

                        da.ExecNonQuery(updateSql, parameters.ToArray());
                    }

                    return record.HealthID;
                }
            }

            private static IEnumerable<SqlParameter> GetHealthRecordFieldParameters(TreeHealthRecord record)
            {
                yield return new SqlParameter("@surveyDate", record.SurveyDate?.Date ?? (object)DBNull.Value);
                yield return new SqlParameter("@surveyor", ToDbValue(record.Surveyor));
                yield return new SqlParameter("@dataStatus", (int)record.DataStatus);
                yield return new SqlParameter("@memo", ToDbValue(record.Memo));
                yield return new SqlParameter("@treeSignStatus", ToDbValue(record.TreeSignStatus));
                yield return new SqlParameter("@latitude", ToDbValue(record.Latitude));
                yield return new SqlParameter("@longitude", ToDbValue(record.Longitude));
                yield return new SqlParameter("@treeHeight", ToDbValue(record.TreeHeight));
                yield return new SqlParameter("@canopyArea", ToDbValue(record.CanopyArea));
                yield return new SqlParameter("@girth100", ToDbValue(record.Girth100));
                yield return new SqlParameter("@diameter100", ToDbValue(record.Diameter100));
                yield return new SqlParameter("@girth130", ToDbValue(record.Girth130));
                yield return new SqlParameter("@diameter130", ToDbValue(record.Diameter130));
                yield return new SqlParameter("@measureNote", ToDbValue(record.MeasureNote));
                yield return new SqlParameter("@majorDiseaseBrownRoot", ToDbValue(record.MajorDiseaseBrownRoot));
                yield return new SqlParameter("@majorDiseaseGanoderma", ToDbValue(record.MajorDiseaseGanoderma));
                yield return new SqlParameter("@majorDiseaseWoodDecayFungus", ToDbValue(record.MajorDiseaseWoodDecayFungus));
                yield return new SqlParameter("@majorDiseaseCanker", ToDbValue(record.MajorDiseaseCanker));
                yield return new SqlParameter("@majorDiseaseOther", ToDbValue(record.MajorDiseaseOther));
                yield return new SqlParameter("@majorDiseaseOtherNote", ToDbValue(record.MajorDiseaseOtherNote));
                yield return new SqlParameter("@majorPestRootTunnel", ToDbValue(record.MajorPestRootTunnel));
                yield return new SqlParameter("@majorPestRootChew", ToDbValue(record.MajorPestRootChew));
                yield return new SqlParameter("@majorPestRootLive", ToDbValue(record.MajorPestRootLive));
                yield return new SqlParameter("@majorPestBaseTunnel", ToDbValue(record.MajorPestBaseTunnel));
                yield return new SqlParameter("@majorPestBaseChew", ToDbValue(record.MajorPestBaseChew));
                yield return new SqlParameter("@majorPestBaseLive", ToDbValue(record.MajorPestBaseLive));
                yield return new SqlParameter("@majorPestTrunkTunnel", ToDbValue(record.MajorPestTrunkTunnel));
                yield return new SqlParameter("@majorPestTrunkChew", ToDbValue(record.MajorPestTrunkChew));
                yield return new SqlParameter("@majorPestTrunkLive", ToDbValue(record.MajorPestTrunkLive));
                yield return new SqlParameter("@majorPestBranchTunnel", ToDbValue(record.MajorPestBranchTunnel));
                yield return new SqlParameter("@majorPestBranchChew", ToDbValue(record.MajorPestBranchChew));
                yield return new SqlParameter("@majorPestBranchLive", ToDbValue(record.MajorPestBranchLive));
                yield return new SqlParameter("@majorPestCrownTunnel", ToDbValue(record.MajorPestCrownTunnel));
                yield return new SqlParameter("@majorPestCrownChew", ToDbValue(record.MajorPestCrownChew));
                yield return new SqlParameter("@majorPestCrownLive", ToDbValue(record.MajorPestCrownLive));
                yield return new SqlParameter("@majorPestOtherTunnel", ToDbValue(record.MajorPestOtherTunnel));
                yield return new SqlParameter("@majorPestOtherChew", ToDbValue(record.MajorPestOtherChew));
                yield return new SqlParameter("@majorPestOtherLive", ToDbValue(record.MajorPestOtherLive));
                yield return new SqlParameter("@generalPestRoot", ToDbValue(record.GeneralPestRoot));
                yield return new SqlParameter("@generalPestBase", ToDbValue(record.GeneralPestBase));
                yield return new SqlParameter("@generalPestTrunk", ToDbValue(record.GeneralPestTrunk));
                yield return new SqlParameter("@generalPestBranch", ToDbValue(record.GeneralPestBranch));
                yield return new SqlParameter("@generalPestCrown", ToDbValue(record.GeneralPestCrown));
                yield return new SqlParameter("@generalPestOther", ToDbValue(record.GeneralPestOther));
                yield return new SqlParameter("@generalDiseaseRoot", ToDbValue(record.GeneralDiseaseRoot));
                yield return new SqlParameter("@generalDiseaseBase", ToDbValue(record.GeneralDiseaseBase));
                yield return new SqlParameter("@generalDiseaseTrunk", ToDbValue(record.GeneralDiseaseTrunk));
                yield return new SqlParameter("@generalDiseaseBranch", ToDbValue(record.GeneralDiseaseBranch));
                yield return new SqlParameter("@generalDiseaseCrown", ToDbValue(record.GeneralDiseaseCrown));
                yield return new SqlParameter("@generalDiseaseOther", ToDbValue(record.GeneralDiseaseOther));
                yield return new SqlParameter("@pestOtherNote", ToDbValue(record.PestOtherNote));
                yield return new SqlParameter("@rootDecayPercent", ToDbValue(record.RootDecayPercent));
                yield return new SqlParameter("@rootCavityMaxDiameter", ToDbValue(record.RootCavityMaxDiameter));
                yield return new SqlParameter("@rootWoundMaxDiameter", ToDbValue(record.RootWoundMaxDiameter));
                yield return new SqlParameter("@rootMechanicalDamage", ToDbValue(record.RootMechanicalDamage));
                yield return new SqlParameter("@rootMowingInjury", ToDbValue(record.RootMowingInjury));
                yield return new SqlParameter("@rootInjury", ToDbValue(record.RootInjury));
                yield return new SqlParameter("@rootGirdling", ToDbValue(record.RootGirdling));
                yield return new SqlParameter("@rootOtherNote", ToDbValue(record.RootOtherNote));
                yield return new SqlParameter("@baseDecayPercent", ToDbValue(record.BaseDecayPercent));
                yield return new SqlParameter("@baseCavityMaxDiameter", ToDbValue(record.BaseCavityMaxDiameter));
                yield return new SqlParameter("@baseWoundMaxDiameter", ToDbValue(record.BaseWoundMaxDiameter));
                yield return new SqlParameter("@baseMechanicalDamage", ToDbValue(record.BaseMechanicalDamage));
                yield return new SqlParameter("@baseMowingInjury", ToDbValue(record.BaseMowingInjury));
                yield return new SqlParameter("@baseOtherNote", ToDbValue(record.BaseOtherNote));
                yield return new SqlParameter("@trunkDecayPercent", ToDbValue(record.TrunkDecayPercent));
                yield return new SqlParameter("@trunkCavityMaxDiameter", ToDbValue(record.TrunkCavityMaxDiameter));
                yield return new SqlParameter("@trunkWoundMaxDiameter", ToDbValue(record.TrunkWoundMaxDiameter));
                yield return new SqlParameter("@trunkMechanicalDamage", ToDbValue(record.TrunkMechanicalDamage));
                yield return new SqlParameter("@trunkIncludedBark", ToDbValue(record.TrunkIncludedBark));
                yield return new SqlParameter("@trunkOtherNote", ToDbValue(record.TrunkOtherNote));
                yield return new SqlParameter("@branchDecayPercent", ToDbValue(record.BranchDecayPercent));
                yield return new SqlParameter("@branchCavityMaxDiameter", ToDbValue(record.BranchCavityMaxDiameter));
                yield return new SqlParameter("@branchWoundMaxDiameter", ToDbValue(record.BranchWoundMaxDiameter));
                yield return new SqlParameter("@branchMechanicalDamage", ToDbValue(record.BranchMechanicalDamage));
                yield return new SqlParameter("@branchIncludedBark", ToDbValue(record.BranchIncludedBark));
                yield return new SqlParameter("@branchDrooping", ToDbValue(record.BranchDrooping));
                yield return new SqlParameter("@branchOtherNote", ToDbValue(record.BranchOtherNote));
                yield return new SqlParameter("@crownLeafCoveragePercent", ToDbValue(record.CrownLeafCoveragePercent));
                yield return new SqlParameter("@crownDeadBranchPercent", ToDbValue(record.CrownDeadBranchPercent));
                yield return new SqlParameter("@crownHangingBranch", ToDbValue(record.CrownHangingBranch));
                yield return new SqlParameter("@crownOtherNote", ToDbValue(record.CrownOtherNote));
                yield return new SqlParameter("@pruningWrongDamage", ToDbValue(record.PruningWrongDamage));
                yield return new SqlParameter("@pruningWoundHealing", ToDbValue(record.PruningWoundHealing));
                yield return new SqlParameter("@pruningEpiphyte", ToDbValue(record.PruningEpiphyte));
                yield return new SqlParameter("@pruningParasite", ToDbValue(record.PruningParasite));
                yield return new SqlParameter("@pruningVine", ToDbValue(record.PruningVine));
                yield return new SqlParameter("@pruningOtherNote", ToDbValue(record.PruningOtherNote));
                yield return new SqlParameter("@supportCount", ToDbValue(record.SupportCount));
                yield return new SqlParameter("@supportEmbedded", ToDbValue(record.SupportEmbedded));
                yield return new SqlParameter("@supportOtherNote", ToDbValue(record.SupportOtherNote));
                yield return new SqlParameter("@siteCementPercent", ToDbValue(record.SiteCementPercent));
                yield return new SqlParameter("@siteAsphaltPercent", ToDbValue(record.SiteAsphaltPercent));
                yield return new SqlParameter("@sitePlanter", ToDbValue(record.SitePlanter));
                yield return new SqlParameter("@siteRecreationFacility", ToDbValue(record.SiteRecreationFacility));
                yield return new SqlParameter("@siteDebrisStack", ToDbValue(record.SiteDebrisStack));
                yield return new SqlParameter("@siteBetweenBuildings", ToDbValue(record.SiteBetweenBuildings));
                yield return new SqlParameter("@siteSoilCompaction", ToDbValue(record.SiteSoilCompaction));
                yield return new SqlParameter("@siteOverburiedSoil", ToDbValue(record.SiteOverburiedSoil));
                yield return new SqlParameter("@siteOtherNote", ToDbValue(record.SiteOtherNote));
                yield return new SqlParameter("@soilPh", ToDbValue(record.SoilPh));
                yield return new SqlParameter("@soilOrganicMatter", ToDbValue(record.SoilOrganicMatter));
                yield return new SqlParameter("@soilEc", ToDbValue(record.SoilEc));
                yield return new SqlParameter("@managementStatus", ToDbValue(record.ManagementStatus));
                yield return new SqlParameter("@priority", ToDbValue(record.Priority));
                yield return new SqlParameter("@treatmentDescription", ToDbValue(record.TreatmentDescription));
                yield return new SqlParameter("@sourceUnit", ToDbValue(record.SourceUnit));
                yield return new SqlParameter("@sourceUnitID", ToDbValue(record.SourceUnitID));
            }

            public static void DeleteHealthRecord(int healthId, int accountId)
            {
                const string sql = "UPDATE Tree_HealthRecord SET removeDateTime=GETDATE(), removeAccountID=@accountId WHERE healthID=@id";
                using (var da = new MS_SQL())
                {
                    da.ExecNonQuery(sql,
                        new SqlParameter("@accountId", accountId),
                        new SqlParameter("@id", healthId));
                }
            }

            public static List<TreeHealthPhoto> GetHealthPhotos(int healthId)
            {
                const string sql = "SELECT photoID, healthID, fileName, filePath, fileSize, caption, insertDateTime FROM Tree_HealthPhoto WHERE healthID=@id AND removeDateTime IS NULL ORDER BY insertDateTime DESC, photoID DESC";

                using (var da = new MS_SQL())
                {
                    var dt = da.GetDataTable(sql, new SqlParameter("@id", healthId));
                    return dt.Rows.Cast<DataRow>().Select(ToHealthPhoto).ToList();
                }
            }

            public static List<TreeHealthAttachment> GetHealthAttachments(int healthId)
            {
                const string sql = "SELECT attachmentID, healthID, fileName, filePath, fileSize, description, insertDateTime FROM Tree_HealthAttachment WHERE healthID=@id AND removeDateTime IS NULL ORDER BY insertDateTime DESC, attachmentID DESC";

                using (var da = new MS_SQL())
                {
                    var dt = da.GetDataTable(sql, new SqlParameter("@id", healthId));
                    return dt.Rows.Cast<DataRow>().Select(ToHealthAttachment).ToList();
                }
            }

            public static int InsertHealthPhoto(TreeHealthPhoto photo, int accountId)
            {
                if (photo == null) throw new ArgumentNullException(nameof(photo));

                const string sql = @"INSERT INTO Tree_HealthPhoto
(healthID, fileName, filePath, fileSize, caption, insertAccountID, insertDateTime)
OUTPUT INSERTED.photoID
VALUES
(@healthID, @fileName, @filePath, @fileSize, @caption, @accountId, GETDATE())";

                using (var da = new MS_SQL())
                {
                    return Convert.ToInt32(da.ExcuteScalar(sql,
                        new SqlParameter("@healthID", photo.HealthID),
                        new SqlParameter("@fileName", ToDbValue(photo.FileName)),
                        new SqlParameter("@filePath", ToDbValue(photo.FilePath)),
                        new SqlParameter("@fileSize", ToDbValue(photo.FileSize)),
                        new SqlParameter("@caption", ToDbValue(photo.Caption)),
                        new SqlParameter("@accountId", accountId)));
                }
            }

            public static int InsertHealthAttachment(TreeHealthAttachment attachment, int accountId)
            {
                if (attachment == null) throw new ArgumentNullException(nameof(attachment));

                const string sql = @"INSERT INTO Tree_HealthAttachment
(healthID, fileName, filePath, fileSize, description, insertAccountID, insertDateTime)
OUTPUT INSERTED.attachmentID
VALUES
(@healthID, @fileName, @filePath, @fileSize, @description, @accountId, GETDATE())";

                using (var da = new MS_SQL())
                {
                    return Convert.ToInt32(da.ExcuteScalar(sql,
                        new SqlParameter("@healthID", attachment.HealthID),
                        new SqlParameter("@fileName", ToDbValue(attachment.FileName)),
                        new SqlParameter("@filePath", ToDbValue(attachment.FilePath)),
                        new SqlParameter("@fileSize", ToDbValue(attachment.FileSize)),
                        new SqlParameter("@description", ToDbValue(attachment.Description)),
                        new SqlParameter("@accountId", accountId)));
                }
            }

            public static void DeleteHealthPhoto(int photoId, int accountId)
            {
                const string sql = "UPDATE Tree_HealthPhoto SET removeDateTime=GETDATE(), removeAccountID=@accountId WHERE photoID=@id";
                using (var da = new MS_SQL())
                {
                    da.ExecNonQuery(sql,
                        new SqlParameter("@accountId", accountId),
                        new SqlParameter("@id", photoId));
                }
            }

            public static void DeleteHealthAttachment(int attachmentId, int accountId)
            {
                const string sql = "UPDATE Tree_HealthAttachment SET removeDateTime=GETDATE(), removeAccountID=@accountId WHERE attachmentID=@id";
                using (var da = new MS_SQL())
                {
                    da.ExecNonQuery(sql,
                        new SqlParameter("@accountId", accountId),
                        new SqlParameter("@id", attachmentId));
                }
            }
        }
    }
    

   
}
