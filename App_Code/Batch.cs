using DataAccess;
using protectTreesV2.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using static protectTreesV2.Batch.Batch;
using static protectTreesV2.Health.Health;

namespace protectTreesV2.Batch
{
    public class Batch
    {
        public enum enum_treeBatchType
        {
            Health_Record, // 健檢紀錄
            Health_Photo,  // 健檢照片
            Health_File,   // 健檢附件
            Patrol_Photo,  // 巡查照片
            Care_Record,   // 養護紀錄
            Care_Photo     // 養護照片
        }
        /// <summary>
        /// DB 查詢用的複合鍵
        /// </summary>
        public class TreeQueryKey
        {
            public int treeID { get; set; }
            public DateTime checkDate { get; set; }
        }

        /// <summary>
        /// 處理過程中的暫存物件
        /// </summary>
        public class TreePhotoInfo
        {
            public HttpPostedFile uploadedFile { get; set; }
            public TreeBatchTaskLog log { get; set; }
            public string systemTreeNo { get; set; }
            public DateTime checkDate { get; set; }
            public int healthID { get; set; } = 0;
            public int treeID { get; set; } = 0;
            public bool isProcessing { get; set; } = true;
        }

        /// <summary>
        /// 暫存照片資料
        /// </summary>
        public class TempPhotoData
        {
            public int healthID { get; set; }
            public string originalFileName { get; set; }
            public string finalFileName { get; set; }
            public string fullPhysicalPath { get; set; }
            public string virtualPath { get; set; }
            public TreePhotoInfo infoRef { get; set; }
        }

        /// <summary>
        /// 批次主表 Model
        /// </summary>
        public class TreeBatchTask
        {
            public int taskID { get; set; }
            public Batch.enum_treeBatchType batchType { get; set; }

            public string taskName { get; set; }
            public int totalCount { get; set; }
            public int successCount { get; set; }
            public int failCount { get; set; }
            public string memo { get; set; }
            public int insertAccountID { get; set; }
            public DateTime? insertDateTime { get; set; }
            public string userName { get; set; }
        }

        /// <summary>
        /// 批次明細 Model
        /// </summary>
        public class TreeBatchTaskLog
        {
            public int logID { get; set; }
            public int taskID { get; set; }
            public string refKey { get; set; }    // 樹籍編號
            public DateTime? refDate { get; set; } // 調查日期
            public string sourceItem { get; set; }
            public bool isSuccess { get; set; }
            public string resultMsg { get; set; }
        }

        /// <summary>
        /// 新增批次主表 
        /// </summary>
        /// <param name="batchType">任務類型</param>
        /// <param name="taskName">任務名稱/檔名</param>
        /// <param name="insertAccountID">上傳者ID</param>
        /// <param name="total">總筆數</param>
        /// <param name="success">成功筆數</param>
        /// <param name="fail">失敗筆數</param>
        /// <returns>回傳新產生的 TaskID</returns>
        public int CreateBatchTask(enum_treeBatchType batchType, string taskName, int insertAccountID, int total, int success, int fail)
        {
            string sql = @"
                INSERT INTO Tree_BatchTask (
                    batchType, taskName, totalCount, successCount, failCount, 
                    insertAccountID, insertDateTime
                ) 
                VALUES (
                    @batchType, @taskName, @total, @success, @fail, 
                    @insertAccountID, GETDATE()
                );
                SELECT SCOPE_IDENTITY();";

            using (var da = new MS_SQL())
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@batchType", batchType.ToString()),
                    new SqlParameter("@taskName", taskName ?? (object)DBNull.Value),
                    new SqlParameter("@total", total),
                    new SqlParameter("@success", success),
                    new SqlParameter("@fail", fail),
                    new SqlParameter("@insertAccountID", insertAccountID)
                };

                object result = da.ExcuteScalar(sql, parameters);
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        /// <summary>
        /// 新增多筆 Log 明細
        /// </summary>
        public void BulkInsertTaskLogs(List<TreeBatchTaskLog> logList)
        {
            if (logList == null || logList.Count == 0) return;

            // 建立 DataTable 
            DataTable dt = new DataTable();
            dt.Columns.Add("taskID", typeof(int));
            dt.Columns.Add("refKey", typeof(string));
            dt.Columns.Add("refDate", typeof(DateTime));
            dt.Columns.Add("sourceItem", typeof(string));
            dt.Columns.Add("isSuccess", typeof(bool));
            dt.Columns.Add("resultMsg", typeof(string));

            // 將 List 資料填入 DataTable
            foreach (var item in logList)
            {
                DataRow row = dt.NewRow();
                row["taskID"] = item.taskID;
                row["refKey"] = item.refKey ?? (object)DBNull.Value;
                row["refDate"] = item.refDate.HasValue ? (object)item.refDate.Value : DBNull.Value;
                row["sourceItem"] = item.sourceItem ?? (object)DBNull.Value;
                row["isSuccess"] = item.isSuccess;
                row["resultMsg"] = item.resultMsg ?? (object)DBNull.Value;
                dt.Rows.Add(row);
            }

            using (var da = new MS_SQL())
            {
                da.BulkCopy("Tree_BatchTaskLog", dt);
            }
        }

        /// <summary>
        /// 取得某個使用者的所有上傳紀錄 (歷史列表) - 依照類型篩選
        /// </summary>
        /// <param name="batchType">批次類型</param>
        /// <param name="accountID">使用者ID</param>
        /// <returns></returns>
        public List<TreeBatchTask> GetBatchTaskList(Batch.enum_treeBatchType batchType, int accountID)
        {
            var list = new List<TreeBatchTask>();

            // [修改 1] SQL 增加 batchType 篩選條件
            string sql = @"
                SELECT t.*, u.name AS userName 
                FROM Tree_BatchTask t
                LEFT JOIN System_UserAccount u ON t.insertAccountID = u.accountID
                WHERE t.insertAccountID = @accountID 
                  AND t.batchType = @batchType
                  AND t.removeDateTime IS NULL 
                ORDER BY t.insertDateTime DESC";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@accountID", accountID));
            parameters.Add(new SqlParameter("@batchType", batchType.ToString()));

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, parameters.ToArray());

                foreach (DataRow row in dt.Rows)
                {
                    // 1. 處理 Enum (先安全取得字串，再嘗試解析)
                    Batch.enum_treeBatchType typeEnum = Batch.enum_treeBatchType.Health_Record; // 預設值
                    string typeStr = DataRowHelper.GetString(row, "batchType");

                    if (!string.IsNullOrEmpty(typeStr))
                    {
                        try
                        {
                            typeEnum = (Batch.enum_treeBatchType)Enum.Parse(typeof(Batch.enum_treeBatchType), typeStr);
                        }
                        catch { /* 解析失敗維持預設值 */ }
                    }

                    list.Add(new TreeBatchTask
                    {
                        taskID = DataRowHelper.GetNullableInt(row, "taskID") ?? 0,
                        batchType = typeEnum,
                        taskName = DataRowHelper.GetString(row, "taskName") ?? "",
                        totalCount = DataRowHelper.GetNullableInt(row, "totalCount") ?? 0,
                        successCount = DataRowHelper.GetNullableInt(row, "successCount") ?? 0,
                        failCount = DataRowHelper.GetNullableInt(row, "failCount") ?? 0,
                        memo = DataRowHelper.GetString(row, "memo") ?? "",
                        insertAccountID = DataRowHelper.GetNullableInt(row, "insertAccountID") ?? 0,
                        insertDateTime = DataRowHelper.GetNullableDateTime(row, "insertDateTime"),
                        userName = DataRowHelper.GetString(row, "userName") ?? ""
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 取得某一筆log明細
        /// </summary>
        public List<TreeBatchTaskLog> GetLatestBatchTaskLogs(int taskID)
        {
            var logs = new List<TreeBatchTaskLog>();

            string sql = @"
                SELECT * FROM Tree_BatchTaskLog 
                WHERE taskID = (
                    SELECT TOP 1 taskID 
                    FROM Tree_BatchTask 
                    WHERE taskID = @taskID 
                      AND removeDateTime IS NULL 
                    ORDER BY insertDateTime DESC
                )
                ORDER BY logID";

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@taskID", taskID));
                foreach (DataRow row in dt.Rows)
                {
                    logs.Add(new TreeBatchTaskLog
                    {
                        logID = DataRowHelper.GetNullableInt(row, "logID") ?? 0,
                        taskID = DataRowHelper.GetNullableInt(row, "taskID") ?? 0,
                        refKey = DataRowHelper.GetString(row, "refKey") ?? "",
                        refDate = DataRowHelper.GetNullableDateTime(row, "refDate"),
                        sourceItem = DataRowHelper.GetString(row, "sourceItem") ?? "",
                        isSuccess = DataRowHelper.GetBoolean(row, "isSuccess", false),
                        resultMsg = DataRowHelper.GetString(row, "resultMsg") ?? ""
                    });
                }
            }
            return logs;
        }

        /// <summary>
        /// 取得樹籍編號對照表
        /// </summary>
        /// <param name="treeNos">要檢查的樹籍編號列表</param>
        /// <returns>資料庫中符合條件的樹籍編號列表</returns>
        public Dictionary<string, int> GetTreeIDMap(List<string> treeNos)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            if (treeNos == null || treeNos.Count == 0) return map;

            // 動態建立 SQL 參數 (處理 IN 查詢)
            List<string> paramNames = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            for (int i = 0; i < treeNos.Count; i++)
            {
                string pName = $"@p{i}";
                paramNames.Add(pName);
                parameters.Add(new SqlParameter(pName, treeNos[i]));
            }

            // 條件：SystemTreeNo 在清單中 + 未刪除 (removeDateTime IS NULL) + 已定稿 (editStatus = 1)
            string inClause = string.Join(",", paramNames);

            string sql = $@"
                SELECT systemTreeNo , treeID
                FROM Tree_Record 
                WHERE removeDateTime IS NULL 
                  AND editStatus = 1 
                  AND systemTreeNo IN ({inClause})";

            // 呼叫資料庫
            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    string tNo = row["systemTreeNo"].ToString();
                    int tID = Convert.ToInt32(row["treeID"]);

                    if (!map.ContainsKey(tNo))
                    {
                        map.Add(tNo, tID);
                    }
                }
            }
            return map;
        }

        /// <summary>
        /// 批次取得 HealthID
        /// </summary>
        /// <param name="keys">查詢條件 (TreeID + CheckDate)</param>
        public Dictionary<string, int> GetHealthIDMap(List<TreeQueryKey> keys)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            if (keys == null || keys.Count == 0) return map;

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                SELECT treeID, surveyDate, healthID 
                FROM Tree_HealthRecord
                WHERE removeDateTime IS NULL 
                  AND (
            ");

            List<SqlParameter> parameters = new List<SqlParameter>();
            List<string> conditions = new List<string>();

            for (int i = 0; i < keys.Count; i++)
            {
                string pTreeID = $"@t{i}";
                string pDate = $"@d{i}";

                // [改] 條件直接對應 treeID (int)
                conditions.Add($" (treeID = {pTreeID} AND surveyDate = {pDate}) ");

                // [改] 傳入 int 型別的 treeID
                parameters.Add(new SqlParameter(pTreeID, keys[i].treeID));
                parameters.Add(new SqlParameter(pDate, keys[i].checkDate));
            }

            sb.Append(string.Join(" OR ", conditions));
            sb.Append(" )");

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sb.ToString(), parameters.ToArray());

                foreach (DataRow row in dt.Rows)
                {
                    int dbTreeID = Convert.ToInt32(row["treeID"]);
                    DateTime dbDate = Convert.ToDateTime(row["surveyDate"]);
                    int dbHealthID = Convert.ToInt32(row["healthID"]);

                    string key = $"{dbTreeID}_{dbDate:yyyyMMdd}";

                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, dbHealthID);
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// 將 TreeHealthRecord 轉為 DataTable
        /// </summary>
        private System.Data.DataTable CreateHealthRecordDataTable(List<TreeHealthRecord> list, bool forUpdate)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            // ==========================================
            // A. 定義欄位 (共約 100+ 個欄位)
            // ==========================================
            if (forUpdate) dt.Columns.Add("healthID", typeof(int));

            // 樹籍與基本
            dt.Columns.Add("treeID", typeof(int));
            dt.Columns.Add("surveyDate", typeof(DateTime));
            dt.Columns.Add("surveyor", typeof(string));
            dt.Columns.Add("dataStatus", typeof(byte));
            dt.Columns.Add("memo", typeof(string));
            dt.Columns.Add("treeSignStatus", typeof(byte));

            // 規格
            dt.Columns.Add("latitude", typeof(decimal));
            dt.Columns.Add("longitude", typeof(decimal));
            dt.Columns.Add("treeHeight", typeof(decimal));
            dt.Columns.Add("canopyArea", typeof(decimal));
            dt.Columns.Add("girth100", typeof(string));
            dt.Columns.Add("diameter100", typeof(string));
            dt.Columns.Add("girth130", typeof(string));
            dt.Columns.Add("diameter130", typeof(string));
            dt.Columns.Add("measureNote", typeof(string));

            // 主要病害
            dt.Columns.Add("majorDiseaseBrownRoot", typeof(bool));
            dt.Columns.Add("majorDiseaseGanoderma", typeof(bool));
            dt.Columns.Add("majorDiseaseWoodDecayFungus", typeof(bool));
            dt.Columns.Add("majorDiseaseCanker", typeof(bool));
            dt.Columns.Add("majorDiseaseOther", typeof(bool));
            dt.Columns.Add("majorDiseaseOtherNote", typeof(string));

            // 主要蟲害 (根/基/幹/枝/冠/其他 * 3種狀態)
            dt.Columns.Add("majorPestRootTunnel", typeof(bool));
            dt.Columns.Add("majorPestRootChew", typeof(bool));
            dt.Columns.Add("majorPestRootLive", typeof(bool));
            dt.Columns.Add("majorPestBaseTunnel", typeof(bool));
            dt.Columns.Add("majorPestBaseChew", typeof(bool));
            dt.Columns.Add("majorPestBaseLive", typeof(bool));
            dt.Columns.Add("majorPestTrunkTunnel", typeof(bool));
            dt.Columns.Add("majorPestTrunkChew", typeof(bool));
            dt.Columns.Add("majorPestTrunkLive", typeof(bool));
            dt.Columns.Add("majorPestBranchTunnel", typeof(bool));
            dt.Columns.Add("majorPestBranchChew", typeof(bool));
            dt.Columns.Add("majorPestBranchLive", typeof(bool));
            dt.Columns.Add("majorPestCrownTunnel", typeof(bool));
            dt.Columns.Add("majorPestCrownChew", typeof(bool));
            dt.Columns.Add("majorPestCrownLive", typeof(bool));
            dt.Columns.Add("majorPestOtherTunnel", typeof(bool));
            dt.Columns.Add("majorPestOtherChew", typeof(bool));
            dt.Columns.Add("majorPestOtherLive", typeof(bool));

            // 一般病蟲害
            dt.Columns.Add("generalPestRoot", typeof(string));
            dt.Columns.Add("generalPestBase", typeof(string));
            dt.Columns.Add("generalPestTrunk", typeof(string));
            dt.Columns.Add("generalPestBranch", typeof(string));
            dt.Columns.Add("generalPestCrown", typeof(string));
            dt.Columns.Add("generalPestOther", typeof(string));
            dt.Columns.Add("generalDiseaseRoot", typeof(string));
            dt.Columns.Add("generalDiseaseBase", typeof(string));
            dt.Columns.Add("generalDiseaseTrunk", typeof(string));
            dt.Columns.Add("generalDiseaseBranch", typeof(string));
            dt.Columns.Add("generalDiseaseCrown", typeof(string));
            dt.Columns.Add("generalDiseaseOther", typeof(string));
            dt.Columns.Add("pestOtherNote", typeof(string));

            // 細部檢測 - 根部
            dt.Columns.Add("rootDecayPercent", typeof(decimal));
            dt.Columns.Add("rootCavityMaxDiameter", typeof(decimal));
            dt.Columns.Add("rootWoundMaxDiameter", typeof(decimal));
            dt.Columns.Add("rootMechanicalDamage", typeof(bool));
            dt.Columns.Add("rootMowingInjury", typeof(bool));
            dt.Columns.Add("rootInjury", typeof(bool));
            dt.Columns.Add("rootGirdling", typeof(bool));
            dt.Columns.Add("rootOtherNote", typeof(string));

            // 細部檢測 - 基部
            dt.Columns.Add("baseDecayPercent", typeof(decimal));
            dt.Columns.Add("baseCavityMaxDiameter", typeof(decimal));
            dt.Columns.Add("baseWoundMaxDiameter", typeof(decimal));
            dt.Columns.Add("baseMechanicalDamage", typeof(bool));
            dt.Columns.Add("baseMowingInjury", typeof(bool));
            dt.Columns.Add("baseOtherNote", typeof(string));

            // 細部檢測 - 樹幹
            dt.Columns.Add("trunkDecayPercent", typeof(decimal));
            dt.Columns.Add("trunkCavityMaxDiameter", typeof(decimal));
            dt.Columns.Add("trunkWoundMaxDiameter", typeof(decimal));
            dt.Columns.Add("trunkMechanicalDamage", typeof(bool));
            dt.Columns.Add("trunkIncludedBark", typeof(bool));
            dt.Columns.Add("trunkOtherNote", typeof(string));

            // 細部檢測 - 枝條
            dt.Columns.Add("branchDecayPercent", typeof(decimal));
            dt.Columns.Add("branchCavityMaxDiameter", typeof(decimal));
            dt.Columns.Add("branchWoundMaxDiameter", typeof(decimal));
            dt.Columns.Add("branchMechanicalDamage", typeof(bool));
            dt.Columns.Add("branchIncludedBark", typeof(bool));
            dt.Columns.Add("branchDrooping", typeof(bool));
            dt.Columns.Add("branchOtherNote", typeof(string));

            // 細部檢測 - 樹冠 & 生長
            dt.Columns.Add("crownLeafCoveragePercent", typeof(decimal));
            dt.Columns.Add("crownDeadBranchPercent", typeof(decimal));
            dt.Columns.Add("crownHangingBranch", typeof(bool));
            dt.Columns.Add("crownOtherNote", typeof(string));
            dt.Columns.Add("growthNote", typeof(string));

            // 修剪與支撐
            dt.Columns.Add("pruningWrongDamage", typeof(string));
            dt.Columns.Add("pruningWoundHealing", typeof(bool));
            dt.Columns.Add("pruningEpiphyte", typeof(bool));
            dt.Columns.Add("pruningParasite", typeof(bool));
            dt.Columns.Add("pruningVine", typeof(bool));
            dt.Columns.Add("pruningOtherNote", typeof(string));
            dt.Columns.Add("supportCount", typeof(int));
            dt.Columns.Add("supportEmbedded", typeof(bool));
            dt.Columns.Add("supportOtherNote", typeof(string));

            // 棲地與土壤
            dt.Columns.Add("siteCementPercent", typeof(decimal));
            dt.Columns.Add("siteAsphaltPercent", typeof(decimal));
            dt.Columns.Add("sitePlanter", typeof(bool));
            dt.Columns.Add("siteRecreationFacility", typeof(bool));
            dt.Columns.Add("siteDebrisStack", typeof(bool));
            dt.Columns.Add("siteBetweenBuildings", typeof(bool));
            dt.Columns.Add("siteSoilCompaction", typeof(bool));
            dt.Columns.Add("siteOverburiedSoil", typeof(bool));
            dt.Columns.Add("siteOtherNote", typeof(string));
            dt.Columns.Add("soilPh", typeof(string));
            dt.Columns.Add("soilOrganicMatter", typeof(string));
            dt.Columns.Add("soilEc", typeof(string));

            // 管理建議
            dt.Columns.Add("managementStatus", typeof(string));
            dt.Columns.Add("priority", typeof(string));
            dt.Columns.Add("treatmentDescription", typeof(string));
            dt.Columns.Add("sourceUnit", typeof(string));
            dt.Columns.Add("sourceUnitID", typeof(int));

            // 系統資訊
            if (forUpdate)
            {
                dt.Columns.Add("updateAccountID", typeof(int));
                dt.Columns.Add("updateDateTime", typeof(DateTime));
            }
            else
            {
                dt.Columns.Add("insertAccountID", typeof(int));
                dt.Columns.Add("insertDateTime", typeof(DateTime));
            }

            // ==========================================
            // B. 填入資料
            // ==========================================
            foreach (var item in list)
            {
                System.Data.DataRow row = dt.NewRow();

                if (forUpdate) row["healthID"] = item.healthID;

                row["treeID"] = item.treeID;
                row["surveyDate"] = item.surveyDate ?? (object)DBNull.Value;
                row["surveyor"] = item.surveyor ?? (object)DBNull.Value;
                row["dataStatus"] = item.dataStatus;
                row["memo"] = item.memo ?? (object)DBNull.Value;
                row["treeSignStatus"] = item.treeSignStatus ?? (object)DBNull.Value;

                row["latitude"] = item.latitude ?? (object)DBNull.Value;
                row["longitude"] = item.longitude ?? (object)DBNull.Value;
                row["treeHeight"] = item.treeHeight ?? (object)DBNull.Value;
                row["canopyArea"] = item.canopyArea ?? (object)DBNull.Value;
                row["girth100"] = item.girth100 ?? (object)DBNull.Value;
                row["diameter100"] = item.diameter100 ?? (object)DBNull.Value;
                row["girth130"] = item.girth130 ?? (object)DBNull.Value;
                row["diameter130"] = item.diameter130 ?? (object)DBNull.Value;
                row["measureNote"] = item.measureNote ?? (object)DBNull.Value;

                row["majorDiseaseBrownRoot"] = item.majorDiseaseBrownRoot ?? (object)DBNull.Value;
                row["majorDiseaseGanoderma"] = item.majorDiseaseGanoderma ?? (object)DBNull.Value;
                row["majorDiseaseWoodDecayFungus"] = item.majorDiseaseWoodDecayFungus ?? (object)DBNull.Value;
                row["majorDiseaseCanker"] = item.majorDiseaseCanker ?? (object)DBNull.Value;
                row["majorDiseaseOther"] = item.majorDiseaseOther ?? (object)DBNull.Value;
                row["majorDiseaseOtherNote"] = item.majorDiseaseOtherNote ?? (object)DBNull.Value;

                row["majorPestRootTunnel"] = item.majorPestRootTunnel ?? (object)DBNull.Value;
                row["majorPestRootChew"] = item.majorPestRootChew ?? (object)DBNull.Value;
                row["majorPestRootLive"] = item.majorPestRootLive ?? (object)DBNull.Value;
                row["majorPestBaseTunnel"] = item.majorPestBaseTunnel ?? (object)DBNull.Value;
                row["majorPestBaseChew"] = item.majorPestBaseChew ?? (object)DBNull.Value;
                row["majorPestBaseLive"] = item.majorPestBaseLive ?? (object)DBNull.Value;
                row["majorPestTrunkTunnel"] = item.majorPestTrunkTunnel ?? (object)DBNull.Value;
                row["majorPestTrunkChew"] = item.majorPestTrunkChew ?? (object)DBNull.Value;
                row["majorPestTrunkLive"] = item.majorPestTrunkLive ?? (object)DBNull.Value;
                row["majorPestBranchTunnel"] = item.majorPestBranchTunnel ?? (object)DBNull.Value;
                row["majorPestBranchChew"] = item.majorPestBranchChew ?? (object)DBNull.Value;
                row["majorPestBranchLive"] = item.majorPestBranchLive ?? (object)DBNull.Value;
                row["majorPestCrownTunnel"] = item.majorPestCrownTunnel ?? (object)DBNull.Value;
                row["majorPestCrownChew"] = item.majorPestCrownChew ?? (object)DBNull.Value;
                row["majorPestCrownLive"] = item.majorPestCrownLive ?? (object)DBNull.Value;
                row["majorPestOtherTunnel"] = item.majorPestOtherTunnel ?? (object)DBNull.Value;
                row["majorPestOtherChew"] = item.majorPestOtherChew ?? (object)DBNull.Value;
                row["majorPestOtherLive"] = item.majorPestOtherLive ?? (object)DBNull.Value;

                row["generalPestRoot"] = item.generalPestRoot ?? (object)DBNull.Value;
                row["generalPestBase"] = item.generalPestBase ?? (object)DBNull.Value;
                row["generalPestTrunk"] = item.generalPestTrunk ?? (object)DBNull.Value;
                row["generalPestBranch"] = item.generalPestBranch ?? (object)DBNull.Value;
                row["generalPestCrown"] = item.generalPestCrown ?? (object)DBNull.Value;
                row["generalPestOther"] = item.generalPestOther ?? (object)DBNull.Value;

                row["generalDiseaseRoot"] = item.generalDiseaseRoot ?? (object)DBNull.Value;
                row["generalDiseaseBase"] = item.generalDiseaseBase ?? (object)DBNull.Value;
                row["generalDiseaseTrunk"] = item.generalDiseaseTrunk ?? (object)DBNull.Value;
                row["generalDiseaseBranch"] = item.generalDiseaseBranch ?? (object)DBNull.Value;
                row["generalDiseaseCrown"] = item.generalDiseaseCrown ?? (object)DBNull.Value;
                row["generalDiseaseOther"] = item.generalDiseaseOther ?? (object)DBNull.Value;
                row["pestOtherNote"] = item.pestOtherNote ?? (object)DBNull.Value;

                row["rootDecayPercent"] = item.rootDecayPercent ?? (object)DBNull.Value;
                row["rootCavityMaxDiameter"] = item.rootCavityMaxDiameter ?? (object)DBNull.Value;
                row["rootWoundMaxDiameter"] = item.rootWoundMaxDiameter ?? (object)DBNull.Value;
                row["rootMechanicalDamage"] = item.rootMechanicalDamage ?? (object)DBNull.Value;
                row["rootMowingInjury"] = item.rootMowingInjury ?? (object)DBNull.Value;
                row["rootInjury"] = item.rootInjury ?? (object)DBNull.Value;
                row["rootGirdling"] = item.rootGirdling ?? (object)DBNull.Value;
                row["rootOtherNote"] = item.rootOtherNote ?? (object)DBNull.Value;

                row["baseDecayPercent"] = item.baseDecayPercent ?? (object)DBNull.Value;
                row["baseCavityMaxDiameter"] = item.baseCavityMaxDiameter ?? (object)DBNull.Value;
                row["baseWoundMaxDiameter"] = item.baseWoundMaxDiameter ?? (object)DBNull.Value;
                row["baseMechanicalDamage"] = item.baseMechanicalDamage ?? (object)DBNull.Value;
                row["baseMowingInjury"] = item.baseMowingInjury ?? (object)DBNull.Value;
                row["baseOtherNote"] = item.baseOtherNote ?? (object)DBNull.Value;

                row["trunkDecayPercent"] = item.trunkDecayPercent ?? (object)DBNull.Value;
                row["trunkCavityMaxDiameter"] = item.trunkCavityMaxDiameter ?? (object)DBNull.Value;
                row["trunkWoundMaxDiameter"] = item.trunkWoundMaxDiameter ?? (object)DBNull.Value;
                row["trunkMechanicalDamage"] = item.trunkMechanicalDamage ?? (object)DBNull.Value;
                row["trunkIncludedBark"] = item.trunkIncludedBark ?? (object)DBNull.Value;
                row["trunkOtherNote"] = item.trunkOtherNote ?? (object)DBNull.Value;

                row["branchDecayPercent"] = item.branchDecayPercent ?? (object)DBNull.Value;
                row["branchCavityMaxDiameter"] = item.branchCavityMaxDiameter ?? (object)DBNull.Value;
                row["branchWoundMaxDiameter"] = item.branchWoundMaxDiameter ?? (object)DBNull.Value;
                row["branchMechanicalDamage"] = item.branchMechanicalDamage ?? (object)DBNull.Value;
                row["branchIncludedBark"] = item.branchIncludedBark ?? (object)DBNull.Value;
                row["branchDrooping"] = item.branchDrooping ?? (object)DBNull.Value;
                row["branchOtherNote"] = item.branchOtherNote ?? (object)DBNull.Value;

                row["crownLeafCoveragePercent"] = item.crownLeafCoveragePercent ?? (object)DBNull.Value;
                row["crownDeadBranchPercent"] = item.crownDeadBranchPercent ?? (object)DBNull.Value;
                row["crownHangingBranch"] = item.crownHangingBranch ?? (object)DBNull.Value;
                row["crownOtherNote"] = item.crownOtherNote ?? (object)DBNull.Value;
                row["growthNote"] = item.growthNote ?? (object)DBNull.Value;

                row["pruningWrongDamage"] = item.pruningWrongDamage ?? (object)DBNull.Value;
                row["pruningWoundHealing"] = item.pruningWoundHealing ?? (object)DBNull.Value;
                row["pruningEpiphyte"] = item.pruningEpiphyte ?? (object)DBNull.Value;
                row["pruningParasite"] = item.pruningParasite ?? (object)DBNull.Value;
                row["pruningVine"] = item.pruningVine ?? (object)DBNull.Value;
                row["pruningOtherNote"] = item.pruningOtherNote ?? (object)DBNull.Value;

                row["supportCount"] = item.supportCount ?? (object)DBNull.Value;
                row["supportEmbedded"] = item.supportEmbedded ?? (object)DBNull.Value;
                row["supportOtherNote"] = item.supportOtherNote ?? (object)DBNull.Value;

                row["siteCementPercent"] = item.siteCementPercent ?? (object)DBNull.Value;
                row["siteAsphaltPercent"] = item.siteAsphaltPercent ?? (object)DBNull.Value;
                row["sitePlanter"] = item.sitePlanter ?? (object)DBNull.Value;
                row["siteRecreationFacility"] = item.siteRecreationFacility ?? (object)DBNull.Value;
                row["siteDebrisStack"] = item.siteDebrisStack ?? (object)DBNull.Value;
                row["siteBetweenBuildings"] = item.siteBetweenBuildings ?? (object)DBNull.Value;
                row["siteSoilCompaction"] = item.siteSoilCompaction ?? (object)DBNull.Value;
                row["siteOverburiedSoil"] = item.siteOverburiedSoil ?? (object)DBNull.Value;
                row["siteOtherNote"] = item.siteOtherNote ?? (object)DBNull.Value;

                row["soilPh"] = item.soilPh ?? (object)DBNull.Value;
                row["soilOrganicMatter"] = item.soilOrganicMatter ?? (object)DBNull.Value;
                row["soilEc"] = item.soilEc ?? (object)DBNull.Value;

                row["managementStatus"] = item.managementStatus ?? (object)DBNull.Value;
                row["priority"] = item.priority ?? (object)DBNull.Value;
                row["treatmentDescription"] = item.treatmentDescription ?? (object)DBNull.Value;
                row["sourceUnit"] = item.sourceUnit ?? (object)DBNull.Value;
                row["sourceUnitID"] = item.sourceUnitID ?? (object)DBNull.Value;

                if (forUpdate)
                {
                    row["updateAccountID"] = item.updateAccountID ?? (object)DBNull.Value;
                    row["updateDateTime"] = DateTime.Now;
                }
                else
                {
                    row["insertAccountID"] = item.insertAccountID;
                    row["insertDateTime"] = DateTime.Now;
                }

                dt.Rows.Add(row);
            }

            return dt;
        }

        public void BulkUpdateHealthRecords(List<TreeHealthRecord> list, bool updateStatus)
        {
            if (list == null || list.Count == 0) return;

            // 1. 產生 DataTable
            System.Data.DataTable dt = CreateHealthRecordDataTable(list, forUpdate: true);

            // 2. 使用您的 MS_SQL 類別
            using (var da = new MS_SQL())
            {
                // ===============================================
                // Step A: 建立暫存表 (包含所有欄位)
                // ===============================================
                string createTempSql = @"
            IF OBJECT_ID('tempdb..#TempHealthUpdate') IS NOT NULL DROP TABLE #TempHealthUpdate;

            CREATE TABLE #TempHealthUpdate (
                [healthID] [int] NOT NULL,
                [treeID] [int] NULL,
                [surveyDate] [date] NULL,
                [surveyor] [nvarchar](100) NULL,
                [dataStatus] [tinyint] NULL,
                [memo] [nvarchar](max) NULL,
                [treeSignStatus] [tinyint] NULL,
                [latitude] [decimal](10, 6) NULL,
                [longitude] [decimal](10, 6) NULL,
                [treeHeight] [decimal](8, 2) NULL,
                [canopyArea] [decimal](8, 2) NULL,
                [girth100] [nvarchar](200) NULL,
                [diameter100] [nvarchar](200) NULL,
                [girth130] [nvarchar](200) NULL,
                [diameter130] [nvarchar](200) NULL,
                [measureNote] [nvarchar](200) NULL,
                [majorDiseaseBrownRoot] [bit] NULL,
                [majorDiseaseGanoderma] [bit] NULL,
                [majorDiseaseWoodDecayFungus] [bit] NULL,
                [majorDiseaseCanker] [bit] NULL,
                [majorDiseaseOther] [bit] NULL,
                [majorDiseaseOtherNote] [nvarchar](200) NULL,
                [majorPestRootTunnel] [bit] NULL,
                [majorPestRootChew] [bit] NULL,
                [majorPestRootLive] [bit] NULL,
                [majorPestBaseTunnel] [bit] NULL,
                [majorPestBaseChew] [bit] NULL,
                [majorPestBaseLive] [bit] NULL,
                [majorPestTrunkTunnel] [bit] NULL,
                [majorPestTrunkChew] [bit] NULL,
                [majorPestTrunkLive] [bit] NULL,
                [majorPestBranchTunnel] [bit] NULL,
                [majorPestBranchChew] [bit] NULL,
                [majorPestBranchLive] [bit] NULL,
                [majorPestCrownTunnel] [bit] NULL,
                [majorPestCrownChew] [bit] NULL,
                [majorPestCrownLive] [bit] NULL,
                [majorPestOtherTunnel] [bit] NULL,
                [majorPestOtherChew] [bit] NULL,
                [majorPestOtherLive] [bit] NULL,
                [generalPestRoot] [nvarchar](200) NULL,
                [generalPestBase] [nvarchar](200) NULL,
                [generalPestTrunk] [nvarchar](200) NULL,
                [generalPestBranch] [nvarchar](200) NULL,
                [generalPestCrown] [nvarchar](200) NULL,
                [generalPestOther] [nvarchar](200) NULL,
                [generalDiseaseRoot] [nvarchar](200) NULL,
                [generalDiseaseBase] [nvarchar](200) NULL,
                [generalDiseaseTrunk] [nvarchar](200) NULL,
                [generalDiseaseBranch] [nvarchar](200) NULL,
                [generalDiseaseCrown] [nvarchar](200) NULL,
                [generalDiseaseOther] [nvarchar](200) NULL,
                [pestOtherNote] [nvarchar](200) NULL,
                [rootDecayPercent] [decimal](5, 2) NULL,
                [rootCavityMaxDiameter] [decimal](6, 2) NULL,
                [rootWoundMaxDiameter] [decimal](6, 2) NULL,
                [rootMechanicalDamage] [bit] NULL,
                [rootMowingInjury] [bit] NULL,
                [rootInjury] [bit] NULL,
                [rootGirdling] [bit] NULL,
                [rootOtherNote] [nvarchar](200) NULL,
                [baseDecayPercent] [decimal](5, 2) NULL,
                [baseCavityMaxDiameter] [decimal](6, 2) NULL,
                [baseWoundMaxDiameter] [decimal](6, 2) NULL,
                [baseMechanicalDamage] [bit] NULL,
                [baseMowingInjury] [bit] NULL,
                [baseOtherNote] [nvarchar](200) NULL,
                [trunkDecayPercent] [decimal](5, 2) NULL,
                [trunkCavityMaxDiameter] [decimal](6, 2) NULL,
                [trunkWoundMaxDiameter] [decimal](6, 2) NULL,
                [trunkMechanicalDamage] [bit] NULL,
                [trunkIncludedBark] [bit] NULL,
                [trunkOtherNote] [nvarchar](200) NULL,
                [branchDecayPercent] [decimal](5, 2) NULL,
                [branchCavityMaxDiameter] [decimal](6, 2) NULL,
                [branchWoundMaxDiameter] [decimal](6, 2) NULL,
                [branchMechanicalDamage] [bit] NULL,
                [branchIncludedBark] [bit] NULL,
                [branchDrooping] [bit] NULL,
                [branchOtherNote] [nvarchar](200) NULL,
                [crownLeafCoveragePercent] [decimal](5, 2) NULL,
                [crownDeadBranchPercent] [decimal](5, 2) NULL,
                [crownHangingBranch] [bit] NULL,
                [crownOtherNote] [nvarchar](200) NULL,
                [growthNote] [nvarchar](200) NULL,
                [pruningWrongDamage] [nvarchar](50) NULL,
                [pruningWoundHealing] [bit] NULL,
                [pruningEpiphyte] [bit] NULL,
                [pruningParasite] [bit] NULL,
                [pruningVine] [bit] NULL,
                [pruningOtherNote] [nvarchar](200) NULL,
                [supportCount] [int] NULL,
                [supportEmbedded] [bit] NULL,
                [supportOtherNote] [nvarchar](200) NULL,
                [siteCementPercent] [decimal](5, 2) NULL,
                [siteAsphaltPercent] [decimal](5, 2) NULL,
                [sitePlanter] [bit] NULL,
                [siteRecreationFacility] [bit] NULL,
                [siteDebrisStack] [bit] NULL,
                [siteBetweenBuildings] [bit] NULL,
                [siteSoilCompaction] [bit] NULL,
                [siteOverburiedSoil] [bit] NULL,
                [siteOtherNote] [nvarchar](200) NULL,
                [soilPh] [nvarchar](200) NULL,
                [soilOrganicMatter] [nvarchar](200) NULL,
                [soilEc] [nvarchar](200) NULL,
                [managementStatus] [nvarchar](max) NULL,
                [priority] [nvarchar](50) NULL,
                [treatmentDescription] [nvarchar](max) NULL,
                [sourceUnit] [nvarchar](200) NULL,
                [sourceUnitID] [int] NULL,
                [updateAccountID] [int] NULL,
                [updateDateTime] [datetime] NULL
            );
        ";
                da.ExecNonQuery(createTempSql);

                // ===============================================
                // Step B: 批次寫入暫存表
                // ===============================================
                da.BulkCopy("#TempHealthUpdate", dt);

                // ===============================================
                // Step C: 從暫存表更新主表 (列出所有欄位)
                // ===============================================
                string updateSql = @"
            UPDATE T
            SET 
                T.treeID = S.treeID,
                T.surveyDate = S.surveyDate,
                T.surveyor = S.surveyor,
                T.dataStatus = CASE WHEN @UpdateStatus = 1 THEN S.dataStatus ELSE T.dataStatus END,
                T.memo = S.memo,
                T.treeSignStatus = S.treeSignStatus,
                T.latitude = S.latitude,
                T.longitude = S.longitude,
                T.treeHeight = S.treeHeight,
                T.canopyArea = S.canopyArea,
                T.girth100 = S.girth100,
                T.diameter100 = S.diameter100,
                T.girth130 = S.girth130,
                T.diameter130 = S.diameter130,
                T.measureNote = S.measureNote,

                T.majorDiseaseBrownRoot = S.majorDiseaseBrownRoot,
                T.majorDiseaseGanoderma = S.majorDiseaseGanoderma,
                T.majorDiseaseWoodDecayFungus = S.majorDiseaseWoodDecayFungus,
                T.majorDiseaseCanker = S.majorDiseaseCanker,
                T.majorDiseaseOther = S.majorDiseaseOther,
                T.majorDiseaseOtherNote = S.majorDiseaseOtherNote,

                T.majorPestRootTunnel = S.majorPestRootTunnel,
                T.majorPestRootChew = S.majorPestRootChew,
                T.majorPestRootLive = S.majorPestRootLive,
                T.majorPestBaseTunnel = S.majorPestBaseTunnel,
                T.majorPestBaseChew = S.majorPestBaseChew,
                T.majorPestBaseLive = S.majorPestBaseLive,
                T.majorPestTrunkTunnel = S.majorPestTrunkTunnel,
                T.majorPestTrunkChew = S.majorPestTrunkChew,
                T.majorPestTrunkLive = S.majorPestTrunkLive,
                T.majorPestBranchTunnel = S.majorPestBranchTunnel,
                T.majorPestBranchChew = S.majorPestBranchChew,
                T.majorPestBranchLive = S.majorPestBranchLive,
                T.majorPestCrownTunnel = S.majorPestCrownTunnel,
                T.majorPestCrownChew = S.majorPestCrownChew,
                T.majorPestCrownLive = S.majorPestCrownLive,
                T.majorPestOtherTunnel = S.majorPestOtherTunnel,
                T.majorPestOtherChew = S.majorPestOtherChew,
                T.majorPestOtherLive = S.majorPestOtherLive,

                T.generalPestRoot = S.generalPestRoot,
                T.generalPestBase = S.generalPestBase,
                T.generalPestTrunk = S.generalPestTrunk,
                T.generalPestBranch = S.generalPestBranch,
                T.generalPestCrown = S.generalPestCrown,
                T.generalPestOther = S.generalPestOther,

                T.generalDiseaseRoot = S.generalDiseaseRoot,
                T.generalDiseaseBase = S.generalDiseaseBase,
                T.generalDiseaseTrunk = S.generalDiseaseTrunk,
                T.generalDiseaseBranch = S.generalDiseaseBranch,
                T.generalDiseaseCrown = S.generalDiseaseCrown,
                T.generalDiseaseOther = S.generalDiseaseOther,
                T.pestOtherNote = S.pestOtherNote,

                T.rootDecayPercent = S.rootDecayPercent,
                T.rootCavityMaxDiameter = S.rootCavityMaxDiameter,
                T.rootWoundMaxDiameter = S.rootWoundMaxDiameter,
                T.rootMechanicalDamage = S.rootMechanicalDamage,
                T.rootMowingInjury = S.rootMowingInjury,
                T.rootInjury = S.rootInjury,
                T.rootGirdling = S.rootGirdling,
                T.rootOtherNote = S.rootOtherNote,

                T.baseDecayPercent = S.baseDecayPercent,
                T.baseCavityMaxDiameter = S.baseCavityMaxDiameter,
                T.baseWoundMaxDiameter = S.baseWoundMaxDiameter,
                T.baseMechanicalDamage = S.baseMechanicalDamage,
                T.baseMowingInjury = S.baseMowingInjury,
                T.baseOtherNote = S.baseOtherNote,

                T.trunkDecayPercent = S.trunkDecayPercent,
                T.trunkCavityMaxDiameter = S.trunkCavityMaxDiameter,
                T.trunkWoundMaxDiameter = S.trunkWoundMaxDiameter,
                T.trunkMechanicalDamage = S.trunkMechanicalDamage,
                T.trunkIncludedBark = S.trunkIncludedBark,
                T.trunkOtherNote = S.trunkOtherNote,

                T.branchDecayPercent = S.branchDecayPercent,
                T.branchCavityMaxDiameter = S.branchCavityMaxDiameter,
                T.branchWoundMaxDiameter = S.branchWoundMaxDiameter,
                T.branchMechanicalDamage = S.branchMechanicalDamage,
                T.branchIncludedBark = S.branchIncludedBark,
                T.branchDrooping = S.branchDrooping,
                T.branchOtherNote = S.branchOtherNote,

                T.crownLeafCoveragePercent = S.crownLeafCoveragePercent,
                T.crownDeadBranchPercent = S.crownDeadBranchPercent,
                T.crownHangingBranch = S.crownHangingBranch,
                T.crownOtherNote = S.crownOtherNote,

                T.growthNote = S.growthNote,

                T.pruningWrongDamage = S.pruningWrongDamage,
                T.pruningWoundHealing = S.pruningWoundHealing,
                T.pruningEpiphyte = S.pruningEpiphyte,
                T.pruningParasite = S.pruningParasite,
                T.pruningVine = S.pruningVine,
                T.pruningOtherNote = S.pruningOtherNote,

                T.supportCount = S.supportCount,
                T.supportEmbedded = S.supportEmbedded,
                T.supportOtherNote = S.supportOtherNote,

                T.siteCementPercent = S.siteCementPercent,
                T.siteAsphaltPercent = S.siteAsphaltPercent,
                T.sitePlanter = S.sitePlanter,
                T.siteRecreationFacility = S.siteRecreationFacility,
                T.siteDebrisStack = S.siteDebrisStack,
                T.siteBetweenBuildings = S.siteBetweenBuildings,
                T.siteSoilCompaction = S.siteSoilCompaction,
                T.siteOverburiedSoil = S.siteOverburiedSoil,
                T.siteOtherNote = S.siteOtherNote,

                T.soilPh = S.soilPh,
                T.soilOrganicMatter = S.soilOrganicMatter,
                T.soilEc = S.soilEc,

                T.managementStatus = S.managementStatus,
                T.priority = S.priority,
                T.treatmentDescription = S.treatmentDescription,
                T.sourceUnit = S.sourceUnit,
                T.sourceUnitID = S.sourceUnitID,

                T.updateAccountID = S.updateAccountID,
                T.updateDateTime = GETDATE()
            FROM Tree_HealthRecord T
            INNER JOIN #TempHealthUpdate S ON T.healthID = S.healthID;

            DROP TABLE #TempHealthUpdate;
        ";

                // Step D: 執行 Update
                da.ExecNonQuery(updateSql, new System.Data.SqlClient.SqlParameter("@UpdateStatus", updateStatus ? 1 : 0));
            }
        }

        /// <summary>
        /// 批次新增健康紀錄
        /// </summary>
        /// <param name="list">要新增的資料清單</param>
        public void BulkInsertHealthRecords(List<TreeHealthRecord> list)
        {
            if (list == null || list.Count == 0) return;

            // 1. 產生 DataTable (Insert 模式)
            System.Data.DataTable dt = CreateHealthRecordDataTable(list, forUpdate: false);

            using (var da = new MS_SQL())
            {
                da.BulkCopy("Tree_HealthRecord", dt);
            }
        }

        /// <summary>
        /// 批次新增健檢紀錄 (直接使用 treeID 寫入)
        /// </summary>
        /// <param name="keys">要新增的鍵值 (TreeID + Date)</param>
        /// <param name="accountID">建立者 ID</param>
        /// <returns>Map: "treeID_yyyyMMdd" -> 新產生的 HealthID</returns>
        public Dictionary<string, int> BatchCreateHealthRecords(List<TreeQueryKey> keys, int accountID)
        {
            Dictionary<string, int> newMap = new Dictionary<string, int>();
            if (keys == null || keys.Count == 0) return newMap;
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SQL_Connection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr)) 
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var k in keys)
                        {
                            string sql = @"
                                INSERT INTO Tree_HealthRecord 
                                (
                                    treeID, surveyDate, dataStatus, 
                                    insertAccountID, insertDateTime
                                ) 
                                VALUES 
                                (
                                    @tid, @sDate, 0,  /* 0:草稿 */
                                    @accID, GETDATE()
                                );
                                SELECT SCOPE_IDENTITY(); /* 取回新 ID */
                            ";

                            using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                            {
                                // 參數直接給 int
                                cmd.Parameters.AddWithValue("@tid", k.treeID);
                                cmd.Parameters.AddWithValue("@sDate", k.checkDate);
                                cmd.Parameters.AddWithValue("@accID", accountID);

                                object result = cmd.ExecuteScalar();

                                if (result != null && result != DBNull.Value)
                                {
                                    int newID = Convert.ToInt32(result);
                                    string key = $"{k.treeID}_{k.checkDate:yyyyMMdd}";

                                    if (!newMap.ContainsKey(key))
                                    {
                                        newMap.Add(key, newID);
                                    }
                                }
                            }
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return newMap;
        }

        /// <summary>
        /// 批次取得指定 HealthID 目前擁有的照片數量 
        /// </summary>
        public Dictionary<int, int> GetBatchHealthPhotoCounts(List<int> healthIDs)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            if (healthIDs == null || healthIDs.Count == 0) return map;

            // 1. 動態組建 SQL
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                SELECT healthID, COUNT(*) as Cnt 
                FROM Tree_HealthPhoto 
                WHERE removeDateTime IS NULL 
                  AND healthID IN (
            ");

            List<SqlParameter> parameters = new List<SqlParameter>();
            List<string> paramNames = new List<string>();

            for (int i = 0; i < healthIDs.Count; i++)
            {
                string pName = $"@p{i}";
                paramNames.Add(pName);
                // 參數化查詢，防止 SQL Injection
                parameters.Add(new SqlParameter(pName, healthIDs[i]));
            }

            sb.Append(string.Join(",", paramNames));
            sb.Append(") GROUP BY healthID");

            // 2. 使用 MS_SQL Helper 執行查詢
            using (var da = new MS_SQL())
            {
                // 呼叫 GetDataTable (傳入 SQL 和 參數陣列)
                DataTable dt = da.GetDataTable(sb.ToString(), parameters.ToArray());

                foreach (DataRow row in dt.Rows)
                {
                    int hID = Convert.ToInt32(row["healthID"]);
                    int count = Convert.ToInt32(row["Cnt"]);

                    if (!map.ContainsKey(hID))
                    {
                        map.Add(hID, count);
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// 批次寫入健檢照片紀錄 
        /// </summary>
        /// <param name="photos">準備寫入的照片清單</param>
        /// <param name="accountID">上傳者 ID</param>
        public void BatchInsertHealthPhotoRecords(List<TempPhotoData> photos, int accountID)
        {
            if (photos == null || photos.Count == 0) return;

            DataTable dt = new DataTable();
            dt.Columns.Add("healthID", typeof(int));
            dt.Columns.Add("fileName", typeof(string));
            dt.Columns.Add("filePath", typeof(string));
            dt.Columns.Add("fileSize", typeof(int));
            dt.Columns.Add("caption", typeof(string)); 
            dt.Columns.Add("insertAccountID", typeof(int));
            dt.Columns.Add("insertDateTime", typeof(DateTime));

            // 填入資料
            foreach (var p in photos)
            {
                DataRow row = dt.NewRow();
                row["healthID"] = p.healthID;
                row["fileName"] = p.originalFileName;

                // 組合虛擬路徑
                row["filePath"] = p.virtualPath;

                // 檔案大小
                int fSize = (p.infoRef != null && p.infoRef.uploadedFile != null)
                            ? p.infoRef.uploadedFile.ContentLength
                            : 0;
                row["fileSize"] = fSize;
                row["caption"] = DBNull.Value;
                row["insertAccountID"] = accountID;
                row["insertDateTime"] = DateTime.Now; 

                dt.Rows.Add(row);
            }

            using (var da = new MS_SQL())
            {
                da.BulkCopy("Tree_HealthPhoto", dt);
            }
        }

    }
}