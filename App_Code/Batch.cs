using DataAccess;
using protectTreesV2.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using static protectTreesV2.Care.Care;
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
        /// 錯誤類型列舉 
        /// </summary>
        public enum enum_uploadErrorCategory
        {
            必填未填,  
            格式錯誤,   
            長度過長,  
            邏輯矛盾,    
            整列缺漏       
        }
        public class ValidationError
        {
     
            public enum_uploadErrorCategory category { get; set; } // 錯誤分類
            public string source { get; set; }          // 來源 
            public string message { get; set; }         // 錯誤訊息內容

            public ValidationError(enum_uploadErrorCategory category, string source, string message)
            {
                this.category = category;
                this.source = source;
                this.message = message;
            }

            public string displayMessage => $"[{source}] {message}";
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
        public class TreeFileInfo
        {
            public HttpPostedFile uploadedFile { get; set; }
            public TreeBatchTaskLog log { get; set; }
            public string systemTreeNo { get; set; }
            public DateTime checkDate { get; set; }
            public int targetID { get; set; } = 0;
            public int treeID { get; set; } = 0;
            public bool isProcessing { get; set; } = true;
            public bool isOverwriteBehavior { get; set; } = false;
        }

        /// <summary>
        /// 暫存檔案資料
        /// </summary>
        public class TempFileData
        {
            public int targetID { get; set; }
            public string originalFileName { get; set; }
            public string finalFileName { get; set; }
            public string fullPhysicalPath { get; set; }
            public string virtualPath { get; set; }
            public TreeFileInfo infoRef { get; set; }
            public bool IsOverwriteAction { get; set; } = false;
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
        /// 操作紀錄 Model
        /// </summary>
        public class TreeLog
        {
            public int logID { get; set; }
            public string functionType { get; set; }

            public int dataID { get; set; }
            public string actionType { get; set; }
            public string memo { get; set; }
            public string ipAddress { get; set; }
            public int? accountID { get; set; }
            public string account { get; set; }
            public string accountName { get; set; }
            public string accountUnit { get; set; }
            public DateTime logDateTime { get; set; } = DateTime.Now;
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
        /// 取得使用者的所有上傳紀錄
        /// </summary>
        /// <param name="batchType">批次類型</param>
        /// <param name="accountID">使用者ID</param>
        /// <returns></returns>
        public List<TreeBatchTask> GetBatchTaskList(Batch.enum_treeBatchType batchType, int accountID)
        {
            var list = new List<TreeBatchTask>();

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
                    // 處理 Enum 
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
        /// 批次新增操作紀錄 (Tree_Log)
        /// </summary>
        /// <param name="logList">紀錄列表</param>
        public void BulkInsertTreeLogs(List<TreeLog> logList)
        {
            if (logList == null || logList.Count == 0) return;

            DataTable dt = new DataTable();
            dt.Columns.Add("functionType", typeof(string));
            dt.Columns.Add("dataID", typeof(int));
            dt.Columns.Add("actionType", typeof(string));
            dt.Columns.Add("memo", typeof(string));
            dt.Columns.Add("ipAddress", typeof(string));
            dt.Columns.Add("accountID", typeof(int)); 
            dt.Columns.Add("account", typeof(string));
            dt.Columns.Add("accountName", typeof(string));
            dt.Columns.Add("accountUnit", typeof(string));
            dt.Columns.Add("logDateTime", typeof(DateTime));

            // 將 List 資料填入 DataTable
            foreach (var item in logList)
            {
                DataRow row = dt.NewRow();
                row["functionType"] = item.functionType;
                row["dataID"] = item.dataID;
                row["actionType"] = item.actionType;
                row["memo"] = item.memo ?? (object)DBNull.Value;
                row["ipAddress"] = item.ipAddress ?? (object)DBNull.Value;
                row["accountID"] = item.accountID.HasValue ? (object)item.accountID.Value : DBNull.Value;
                row["account"] = item.account ?? (object)DBNull.Value;
                row["accountName"] = item.accountName ?? (object)DBNull.Value;
                row["accountUnit"] = item.accountUnit ?? (object)DBNull.Value;
                row["logDateTime"] = item.logDateTime;

                dt.Rows.Add(row);
            }

            using (var da = new MS_SQL())
            {
                // 對應資料庫表名稱 Tree_Log
                da.BulkCopy("Tree_Log", dt);
            }
        }

        /// <summary>
        /// 取得樹籍編號對照表(區分管轄)
        /// </summary>
        /// <param name="treeNos">要檢查的樹籍編號列表</param>
        /// <returns>資料庫中符合條件的樹籍編號列表</returns>
        public Dictionary<string, int> GetTreeIDMap(List<string> treeNos, int currentUserID)
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
            parameters.Add(new SqlParameter("@currentUserID", currentUserID));

            // 條件：SystemTreeNo 在清單中 + 未刪除 (removeDateTime IS NULL) + 已定稿 (editStatus = 1)
            string inClause = string.Join(",", paramNames);

            string sql = $@"
                SELECT systemTreeNo , treeID
                FROM Tree_Record 
                WHERE removeDateTime IS NULL 
                  AND editStatus = 1 
                  AND systemTreeNo IN ({inClause})
                  AND Tree_Record.areaID IN (
                  SELECT map.twID 
                      FROM System_UnitCityMapping map
                      INNER JOIN System_UserAccount u ON u.unitID = map.unitID
                      WHERE u.accountID = @currentUserID
                  )";

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
        /// 批次取得健檢流水號
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

                conditions.Add($" (treeID = {pTreeID} AND surveyDate = {pDate}) ");
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
            // A. 定義欄位
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
            dt.Columns.Add("crownLeafCoveragePercent", typeof(string));
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

        /// <summary>
        /// 批次更新健康紀錄
        /// </summary>
        public void BulkUpdateHealthRecords(List<TreeHealthRecord> list, bool updateStatus, string ipAddress, int accountID,string account, string accountName,string accountUnit)
        {
            if (list == null || list.Count == 0) return;

            // 產生 Update 用的 DataTable 
            System.Data.DataTable dt = CreateHealthRecordDataTable(list, forUpdate: true);

            // ===============================================
            // 準備 Log 的 DataTable
            // ===============================================
            System.Data.DataTable dtLog = new System.Data.DataTable();
                dtLog.Columns.Add("functionType", typeof(string));
                dtLog.Columns.Add("dataID", typeof(int));
                dtLog.Columns.Add("actionType", typeof(string));
                dtLog.Columns.Add("memo", typeof(string));
                dtLog.Columns.Add("ipAddress", typeof(string));
                dtLog.Columns.Add("accountID", typeof(int));
                dtLog.Columns.Add("account", typeof(string));
                dtLog.Columns.Add("accountName", typeof(string));
                dtLog.Columns.Add("accountUnit", typeof(string));
                dtLog.Columns.Add("logDateTime", typeof(DateTime));

            DateTime now = DateTime.Now;

            // 填寫 Log 資料
            foreach (var item in list)
            {
                System.Data.DataRow rowLog = dtLog.NewRow();
                rowLog["functionType"] = protectTreesV2.TreeLog.LogFunctionTypes.Health.ToString();
                rowLog["dataID"] = item.healthID; 
                rowLog["actionType"] = "批次上傳";
                rowLog["memo"] = "編輯健檢";
                rowLog["ipAddress"] = ipAddress;
                rowLog["accountID"] = accountID;
                rowLog["account"] = account;
                rowLog["accountName"] = accountName;
                rowLog["accountUnit"] = accountUnit;
                rowLog["logDateTime"] = now;
                dtLog.Rows.Add(rowLog);
            }

            using (var da = new MS_SQL())
            {
                try
                {
                    // 開啟交易 (確保 Update 和 Log 同步成功)
                    da.StartTransaction();

                    // ===============================================
                    // 建立暫存表
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
                            [crownLeafCoveragePercent] [nvarchar](50) NULL,
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
                    // Step C: 從暫存表更新主表
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
                            T.updateAccountID = S.updateAccountID,
                            T.updateDateTime = GETDATE()
                        FROM Tree_HealthRecord T
                        INNER JOIN #TempHealthUpdate S ON T.healthID = S.healthID;

                        DROP TABLE #TempHealthUpdate;
                    ";

                    // 執行 Update
                    da.ExecNonQuery(updateSql, new System.Data.SqlClient.SqlParameter("@UpdateStatus", updateStatus ? 1 : 0));

                    // ===============================================
                    // 批次寫入 Log 
                    // ===============================================
                    if (dtLog.Rows.Count > 0)
                    {
                        da.BulkCopy("Tree_Log", dtLog);
                    }

                    // 全部成功，提交交易
                    da.Commit();
                }
                catch (Exception ex)
                {
                    da.RollBack();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批次新增健康紀錄
        /// </summary>
        /// <param name="list">要新增的資料清單)</param>
        /// <param name="accountID">操作者 ID</param>
        /// <param name="accountName">操作者姓名</param>
        /// <param name="ipAddress">來源 IP</param>
        public void BulkInsertHealthRecords(List<TreeHealthRecord> list, string ipAddress, int accountID, string account, string accountName, string accountUnit)
        {
            if (list == null || list.Count == 0) return;

            // 準備 Log 的 DataTable
            System.Data.DataTable dtLog = new System.Data.DataTable();
            dtLog.Columns.Add("functionType", typeof(string));
            dtLog.Columns.Add("dataID", typeof(int));
            dtLog.Columns.Add("actionType", typeof(string));
            dtLog.Columns.Add("memo", typeof(string));
            dtLog.Columns.Add("ipAddress", typeof(string));
            dtLog.Columns.Add("accountID", typeof(int));
            dtLog.Columns.Add("account", typeof(string));
            dtLog.Columns.Add("accountName", typeof(string));
            dtLog.Columns.Add("accountUnit", typeof(string));
            dtLog.Columns.Add("logDateTime", typeof(DateTime));

            DateTime now = DateTime.Now;

            using (var da = new MS_SQL())
            {
                try
                {
                    da.StartTransaction();

                    string sqlInsert = @"
                        INSERT INTO Tree_HealthRecord 
                        (
                            /* [基本資料] */
                            treeID, surveyDate, surveyor, dataStatus, memo, treeSignStatus,
                    
                            /* [量測資料] */
                            latitude, longitude, treeHeight, canopyArea, 
                            girth100, diameter100, girth130, diameter130, measureNote,

                            /* [主要病害] */
                            majorDiseaseBrownRoot, majorDiseaseGanoderma, majorDiseaseWoodDecayFungus, 
                            majorDiseaseCanker, majorDiseaseOther, majorDiseaseOtherNote,

                            /* [主要害蟲] */
                            majorPestRootTunnel, majorPestRootChew, majorPestRootLive,
                            majorPestBaseTunnel, majorPestBaseChew, majorPestBaseLive,
                            majorPestTrunkTunnel, majorPestTrunkChew, majorPestTrunkLive,
                            majorPestBranchTunnel, majorPestBranchChew, majorPestBranchLive,
                            majorPestCrownTunnel, majorPestCrownChew, majorPestCrownLive,
                            majorPestOtherTunnel, majorPestOtherChew, majorPestOtherLive,

                            /* [一般病蟲害描述] */
                            generalPestRoot, generalPestBase, generalPestTrunk, 
                            generalPestBranch, generalPestCrown, generalPestOther,
                            generalDiseaseRoot, generalDiseaseBase, generalDiseaseTrunk, 
                            generalDiseaseBranch, generalDiseaseCrown, generalDiseaseOther,
                            pestOtherNote,

                            /* [根部細節] */
                            rootDecayPercent, rootCavityMaxDiameter, rootWoundMaxDiameter,
                            rootMechanicalDamage, rootMowingInjury, rootInjury, rootGirdling, rootOtherNote,

                            /* [基部細節] */
                            baseDecayPercent, baseCavityMaxDiameter, baseWoundMaxDiameter,
                            baseMechanicalDamage, baseMowingInjury, baseOtherNote,

                            /* [樹幹細節] */
                            trunkDecayPercent, trunkCavityMaxDiameter, trunkWoundMaxDiameter,
                            trunkMechanicalDamage, trunkIncludedBark, trunkOtherNote,

                            /* [枝條細節] */
                            branchDecayPercent, branchCavityMaxDiameter, branchWoundMaxDiameter,
                            branchMechanicalDamage, branchIncludedBark, branchDrooping, branchOtherNote,

                            /* [樹冠與生長] */
                            crownLeafCoveragePercent, crownDeadBranchPercent, crownHangingBranch, crownOtherNote,
                            growthNote,

                            /* [修剪與維護] */
                            pruningWrongDamage, pruningWoundHealing, pruningEpiphyte, 
                            pruningParasite, pruningVine, pruningOtherNote,
                            supportCount, supportEmbedded, supportOtherNote,

                            /* [棲地環境] */
                            siteCementPercent, siteAsphaltPercent, sitePlanter, 
                            siteRecreationFacility, siteDebrisStack, siteBetweenBuildings,
                            siteSoilCompaction, siteOverburiedSoil, siteOtherNote,

                            /* [土壤與管理] */
                            soilPh, soilOrganicMatter, soilEc,
                            managementStatus, priority, treatmentDescription,

                            /* [系統欄位] */
                            insertAccountID, insertDateTime
                        )
                        VALUES 
                        (
                            /* [基本資料] */
                            @treeID, @surveyDate, @surveyor, @dataStatus, @memo, @treeSignStatus,

                            /* [量測資料] */
                            @latitude, @longitude, @treeHeight, @canopyArea, 
                            @girth100, @diameter100, @girth130, @diameter130, @measureNote,

                            /* [主要病害] */
                            @majorDiseaseBrownRoot, @majorDiseaseGanoderma, @majorDiseaseWoodDecayFungus, 
                            @majorDiseaseCanker, @majorDiseaseOther, @majorDiseaseOtherNote,

                            /* [主要害蟲] */
                            @majorPestRootTunnel, @majorPestRootChew, @majorPestRootLive,
                            @majorPestBaseTunnel, @majorPestBaseChew, @majorPestBaseLive,
                            @majorPestTrunkTunnel, @majorPestTrunkChew, @majorPestTrunkLive,
                            @majorPestBranchTunnel, @majorPestBranchChew, @majorPestBranchLive,
                            @majorPestCrownTunnel, @majorPestCrownChew, @majorPestCrownLive,
                            @majorPestOtherTunnel, @majorPestOtherChew, @majorPestOtherLive,

                            /* [一般病蟲害描述] */
                            @generalPestRoot, @generalPestBase, @generalPestTrunk, 
                            @generalPestBranch, @generalPestCrown, @generalPestOther,
                            @generalDiseaseRoot, @generalDiseaseBase, @generalDiseaseTrunk, 
                            @generalDiseaseBranch, @generalDiseaseCrown, @generalDiseaseOther,
                            @pestOtherNote,

                            /* [根部細節] */
                            @rootDecayPercent, @rootCavityMaxDiameter, @rootWoundMaxDiameter,
                            @rootMechanicalDamage, @rootMowingInjury, @rootInjury, @rootGirdling, @rootOtherNote,

                            /* [基部細節] */
                            @baseDecayPercent, @baseCavityMaxDiameter, @baseWoundMaxDiameter,
                            @baseMechanicalDamage, @baseMowingInjury, @baseOtherNote,

                            /* [樹幹細節] */
                            @trunkDecayPercent, @trunkCavityMaxDiameter, @trunkWoundMaxDiameter,
                            @trunkMechanicalDamage, @trunkIncludedBark, @trunkOtherNote,

                            /* [枝條細節] */
                            @branchDecayPercent, @branchCavityMaxDiameter, @branchWoundMaxDiameter,
                            @branchMechanicalDamage, @branchIncludedBark, @branchDrooping, @branchOtherNote,

                            /* [樹冠與生長] */
                            @crownLeafCoveragePercent, @crownDeadBranchPercent, @crownHangingBranch, @crownOtherNote,
                            @growthNote,

                            /* [修剪與維護] */
                            @pruningWrongDamage, @pruningWoundHealing, @pruningEpiphyte, 
                            @pruningParasite, @pruningVine, @pruningOtherNote,
                            @supportCount, @supportEmbedded, @supportOtherNote,

                            /* [棲地環境] */
                            @siteCementPercent, @siteAsphaltPercent, @sitePlanter, 
                            @siteRecreationFacility, @siteDebrisStack, @siteBetweenBuildings,
                            @siteSoilCompaction, @siteOverburiedSoil, @siteOtherNote,

                            /* [土壤與管理] */
                            @soilPh, @soilOrganicMatter, @soilEc,
                            @managementStatus, @priority, @treatmentDescription,

                            /* [系統欄位] */
                            @acc, @time
                        );
                
                        SELECT CAST(SCOPE_IDENTITY() AS int); 
                    ";

                    foreach (var item in list)
                    {
                        List<System.Data.SqlClient.SqlParameter> p = new List<System.Data.SqlClient.SqlParameter>();

                        // --- 基本資料 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@treeID", item.treeID));
                        p.Add(new System.Data.SqlClient.SqlParameter("@surveyDate", item.surveyDate));
                        p.Add(new System.Data.SqlClient.SqlParameter("@surveyor", item.surveyor ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@dataStatus", item.dataStatus)); // 預設 0
                        p.Add(new System.Data.SqlClient.SqlParameter("@memo", item.memo ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@treeSignStatus", item.treeSignStatus ?? (object)DBNull.Value));

                        // --- 量測 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@latitude", item.latitude ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@longitude", item.longitude ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@treeHeight", item.treeHeight ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@canopyArea", item.canopyArea ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@girth100", item.girth100 ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@diameter100", item.diameter100 ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@girth130", item.girth130 ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@diameter130", item.diameter130 ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@measureNote", item.measureNote ?? (object)DBNull.Value));

                        // --- 主要病害 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorDiseaseBrownRoot", item.majorDiseaseBrownRoot ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorDiseaseGanoderma", item.majorDiseaseGanoderma ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorDiseaseWoodDecayFungus", item.majorDiseaseWoodDecayFungus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorDiseaseCanker", item.majorDiseaseCanker ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorDiseaseOther", item.majorDiseaseOther ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorDiseaseOtherNote", item.majorDiseaseOtherNote ?? (object)DBNull.Value));

                        // --- 主要害蟲 (簡化寫法，請確保物件有這些屬性) ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestRootTunnel", item.majorPestRootTunnel ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestRootChew", item.majorPestRootChew ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestRootLive", item.majorPestRootLive ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestBaseTunnel", item.majorPestBaseTunnel ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestBaseChew", item.majorPestBaseChew ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestBaseLive", item.majorPestBaseLive ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestTrunkTunnel", item.majorPestTrunkTunnel ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestTrunkChew", item.majorPestTrunkChew ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestTrunkLive", item.majorPestTrunkLive ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestBranchTunnel", item.majorPestBranchTunnel ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestBranchChew", item.majorPestBranchChew ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestBranchLive", item.majorPestBranchLive ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestCrownTunnel", item.majorPestCrownTunnel ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestCrownChew", item.majorPestCrownChew ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestCrownLive", item.majorPestCrownLive ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestOtherTunnel", item.majorPestOtherTunnel ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestOtherChew", item.majorPestOtherChew ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@majorPestOtherLive", item.majorPestOtherLive ?? (object)DBNull.Value));

                        // --- 一般病蟲害 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalPestRoot", item.generalPestRoot ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalPestBase", item.generalPestBase ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalPestTrunk", item.generalPestTrunk ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalPestBranch", item.generalPestBranch ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalPestCrown", item.generalPestCrown ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalPestOther", item.generalPestOther ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalDiseaseRoot", item.generalDiseaseRoot ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalDiseaseBase", item.generalDiseaseBase ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalDiseaseTrunk", item.generalDiseaseTrunk ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalDiseaseBranch", item.generalDiseaseBranch ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalDiseaseCrown", item.generalDiseaseCrown ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@generalDiseaseOther", item.generalDiseaseOther ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@pestOtherNote", item.pestOtherNote ?? (object)DBNull.Value));

                        // --- 根部 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootDecayPercent", item.rootDecayPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootCavityMaxDiameter", item.rootCavityMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootWoundMaxDiameter", item.rootWoundMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootMechanicalDamage", item.rootMechanicalDamage ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootMowingInjury", item.rootMowingInjury ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootInjury", item.rootInjury ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootGirdling", item.rootGirdling ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootOtherNote", item.rootOtherNote ?? (object)DBNull.Value));

                        // --- 基部 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@baseDecayPercent", item.baseDecayPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@baseCavityMaxDiameter", item.baseCavityMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@baseWoundMaxDiameter", item.baseWoundMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@baseMechanicalDamage", item.baseMechanicalDamage ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@baseMowingInjury", item.baseMowingInjury ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@baseOtherNote", item.baseOtherNote ?? (object)DBNull.Value));

                        // --- 樹幹 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkDecayPercent", item.trunkDecayPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkCavityMaxDiameter", item.trunkCavityMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkWoundMaxDiameter", item.trunkWoundMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkMechanicalDamage", item.trunkMechanicalDamage ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkIncludedBark", item.trunkIncludedBark ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkOtherNote", item.trunkOtherNote ?? (object)DBNull.Value));

                        // --- 枝條 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@branchDecayPercent", item.branchDecayPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@branchCavityMaxDiameter", item.branchCavityMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@branchWoundMaxDiameter", item.branchWoundMaxDiameter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@branchMechanicalDamage", item.branchMechanicalDamage ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@branchIncludedBark", item.branchIncludedBark ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@branchDrooping", item.branchDrooping ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@branchOtherNote", item.branchOtherNote ?? (object)DBNull.Value));

                        // --- 樹冠與生長 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownLeafCoveragePercent", item.crownLeafCoveragePercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownDeadBranchPercent", item.crownDeadBranchPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownHangingBranch", item.crownHangingBranch ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownOtherNote", item.crownOtherNote ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@growthNote", item.growthNote ?? (object)DBNull.Value));

                        // --- 修剪與維護 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@pruningWrongDamage", item.pruningWrongDamage ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@pruningWoundHealing", item.pruningWoundHealing ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@pruningEpiphyte", item.pruningEpiphyte ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@pruningParasite", item.pruningParasite ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@pruningVine", item.pruningVine ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@pruningOtherNote", item.pruningOtherNote ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@supportCount", item.supportCount ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@supportEmbedded", item.supportEmbedded ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@supportOtherNote", item.supportOtherNote ?? (object)DBNull.Value));

                        // --- 棲地環境 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteCementPercent", item.siteCementPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteAsphaltPercent", item.siteAsphaltPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@sitePlanter", item.sitePlanter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteRecreationFacility", item.siteRecreationFacility ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteDebrisStack", item.siteDebrisStack ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteBetweenBuildings", item.siteBetweenBuildings ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteSoilCompaction", item.siteSoilCompaction ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteOverburiedSoil", item.siteOverburiedSoil ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@siteOtherNote", item.siteOtherNote ?? (object)DBNull.Value));

                        // --- 土壤與管理 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@soilPh", item.soilPh ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@soilOrganicMatter", item.soilOrganicMatter ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@soilEc", item.soilEc ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@managementStatus", item.managementStatus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@priority", item.priority ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@treatmentDescription", item.treatmentDescription ?? (object)DBNull.Value));

                        // --- 系統 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@acc", accountID));
                        p.Add(new System.Data.SqlClient.SqlParameter("@time", now));

                        // 執行 SQL
                        object result = da.ExcuteScalar(sqlInsert, p.ToArray());

                        if (result != null && int.TryParse(result.ToString(), out int newHealthID))
                        {
                            // 建立 Log 
                            System.Data.DataRow rowLog = dtLog.NewRow();
                            rowLog["functionType"] = protectTreesV2.TreeLog.LogFunctionTypes.Health.ToString();
                            rowLog["dataID"] = newHealthID;
                            rowLog["actionType"] = "批次上傳";
                            rowLog["memo"] = "新增健檢";
                            rowLog["ipAddress"] = ipAddress;
                            rowLog["accountID"] = accountID;
                            rowLog["account"] = account;
                            rowLog["accountName"] = accountName;
                            rowLog["accountUnit"] = accountUnit;
                            rowLog["logDateTime"] = now;
                            dtLog.Rows.Add(rowLog);
                        }
                    }

                    // 批次寫入 Log
                    if (dtLog.Rows.Count > 0)
                    {
                        da.BulkCopy("Tree_Log", dtLog);
                    }

                    da.Commit();
                }
                catch (Exception ex)
                {
                    da.RollBack();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批次新增健檢紀錄 
        /// </summary>
        /// <param name="keys">要新增的鍵值 (TreeID + Date)</param>
        /// <param name="accountID">建立者 ID</param>
        /// <returns>Map: "treeID_yyyyMMdd" -> 新產生的 HealthID</returns>
        public Dictionary<string, int> BatchCreateHealthRecords(List<TreeQueryKey> keys, int accountID)
        {
            Dictionary<string, int> newMap = new Dictionary<string, int>();
            if (keys == null || keys.Count == 0) return newMap;

            using (var da = new MS_SQL())
            {
                try
                {
                    // 開啟交易
                    da.StartTransaction();

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
                
                        /* 取回新 ID (建議轉型為 int 避免 decimal 問題) */
                        SELECT CAST(SCOPE_IDENTITY() AS int); 
                    ";

                    foreach (var k in keys)
                    {
                        List<System.Data.SqlClient.SqlParameter> p = new List<System.Data.SqlClient.SqlParameter>();
                        p.Add(new System.Data.SqlClient.SqlParameter("@tid", k.treeID));
                        p.Add(new System.Data.SqlClient.SqlParameter("@sDate", k.checkDate));
                        p.Add(new System.Data.SqlClient.SqlParameter("@accID", accountID));

                        object result = da.ExcuteScalar(sql, p.ToArray());

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

                    // 全部成功，提交交易
                    da.Commit();
                }
                catch (Exception ex)
                {
                    // 發生錯誤，回滾
                    da.RollBack();
                    throw ex; 
                }
            }

            return newMap;
        }

        /// <summary>
        /// 批次取得指定健檢紀錄照片數量 
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
                parameters.Add(new SqlParameter(pName, healthIDs[i]));
            }

            sb.Append(string.Join(",", paramNames));
            sb.Append(") GROUP BY healthID");

            using (var da = new MS_SQL())
            {
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
        public void BatchInsertHealthPhotoRecords(List<TempFileData> photos, string ipAddress, int accountID,string account, string accountName,string accountUnit)
        {
            if (photos == null || photos.Count == 0) return;

            // 準備照片資料表
            DataTable dtPhoto = new DataTable();
            dtPhoto.Columns.Add("healthID", typeof(int));
            dtPhoto.Columns.Add("fileName", typeof(string));
            dtPhoto.Columns.Add("filePath", typeof(string));
            dtPhoto.Columns.Add("fileSize", typeof(int));
            dtPhoto.Columns.Add("caption", typeof(string));
            dtPhoto.Columns.Add("insertAccountID", typeof(int));
            dtPhoto.Columns.Add("insertDateTime", typeof(DateTime));

            // 準備 Log 資料表 
            DataTable dtLog = new DataTable();
            dtLog.Columns.Add("functionType", typeof(string));
            dtLog.Columns.Add("dataID", typeof(int));
            dtLog.Columns.Add("actionType", typeof(string));
            dtLog.Columns.Add("memo", typeof(string));
            dtLog.Columns.Add("ipAddress", typeof(string));
            dtLog.Columns.Add("accountID", typeof(int));
            dtLog.Columns.Add("account", typeof(string));
            dtLog.Columns.Add("accountName", typeof(string));
            dtLog.Columns.Add("accountUnit", typeof(string));
            dtLog.Columns.Add("logDateTime", typeof(DateTime));

            DateTime now = DateTime.Now;

            // ==========================================
            // 填寫照片資料 (每張都要寫)
            // ==========================================
            foreach (var p in photos)
            {
                DataRow row = dtPhoto.NewRow();
                row["healthID"] = p.targetID;
                row["fileName"] = p.originalFileName;
                row["filePath"] = p.virtualPath;

                int fSize = (p.infoRef != null && p.infoRef.uploadedFile != null)
                            ? p.infoRef.uploadedFile.ContentLength : 0;
                row["fileSize"] = fSize;
                row["caption"] = DBNull.Value;
                row["insertAccountID"] = accountID;
                row["insertDateTime"] = now;
                dtPhoto.Rows.Add(row);
            }

            // ==========================================
            // B: 填寫 Log 資料 (依 HealthID 分組)
            // ==========================================

            // 使用 LINQ 依 healthID 分組
            var groupedPhotos = photos.GroupBy(p => p.targetID);

            foreach (var group in groupedPhotos)
            {
                int currentHealthID = group.Key;
                int count = group.Count();

                // 組合備註文字：顯示數量與檔名 (例如：批次上傳 3 張：a.jpg, b.jpg, c.jpg)
                string fileNames = string.Join("、", group.Select(x => x.originalFileName));
                string memoStr = $"批次上傳 {count} 張照片";

                DataRow rowLog = dtLog.NewRow();
                rowLog["functionType"] = protectTreesV2.TreeLog.LogFunctionTypes.Health.ToString();
                rowLog["dataID"] = currentHealthID;
                rowLog["actionType"] = "批次上傳";
                rowLog["memo"] = memoStr;
                rowLog["ipAddress"] = ipAddress;
                rowLog["accountID"] = accountID;
                rowLog["account"] = account;
                rowLog["accountName"] = accountName;
                rowLog["accountUnit"] = accountUnit;
                rowLog["logDateTime"] = now;
                dtLog.Rows.Add(rowLog);
            }

            using (var db = new MS_SQL())
            {
                try
                {
                    db.StartTransaction();

                    if (dtPhoto.Rows.Count > 0)
                        db.BulkCopy("Tree_HealthPhoto", dtPhoto);

                    if (dtLog.Rows.Count > 0)
                        db.BulkCopy("Tree_Log", dtLog);

                    db.Commit();
                }
                catch (Exception ex)
                {
                    db.RollBack();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批次取得指定健檢流水號擁有的附件檔案數量
        /// </summary>
        public Dictionary<int, int> GetBatchHealthAttachmentCounts(List<int> healthIDs)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            if (healthIDs == null || healthIDs.Count == 0) return map;

            // 動態組建 SQL
            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                SELECT healthID, COUNT(*) as Cnt 
                FROM Tree_HealthAttachment 
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

            using (var da = new MS_SQL())
            {
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
        /// 批次覆蓋/新增健檢附件並寫入操作 Log
        /// </summary>
        /// <param name="insertList">要新增的附件清單</param>
        /// <param name="accountID">操作者 ID </param>
        /// <param name="accountName">操作者姓名 </param>
        /// <param name="isOverwrite">是否執行覆蓋 (軟刪除)</param>
        public void BatchReplaceHealthDocs(List<TempFileData> insertList, string ipAddress, int accountID, string account, string accountName, string accountUnit, bool isOverwrite)
        {
            if (insertList == null || insertList.Count == 0) return;

            //檔案欄位
            DataTable dtAttach = new DataTable();
            dtAttach.Columns.Add("healthID", typeof(int));
            dtAttach.Columns.Add("fileName", typeof(string));
            dtAttach.Columns.Add("filePath", typeof(string));
            dtAttach.Columns.Add("fileSize", typeof(int));
            dtAttach.Columns.Add("description", typeof(string));
            dtAttach.Columns.Add("insertAccountID", typeof(int));
            dtAttach.Columns.Add("insertDateTime", typeof(DateTime));

            //log欄位
            DataTable dtLog = new DataTable();
            dtLog.Columns.Add("functionType", typeof(string));
            dtLog.Columns.Add("dataID", typeof(int));
            dtLog.Columns.Add("actionType", typeof(string));
            dtLog.Columns.Add("memo", typeof(string));
            dtLog.Columns.Add("ipAddress", typeof(string));
            dtLog.Columns.Add("accountID", typeof(int));
            dtLog.Columns.Add("account", typeof(string));
            dtLog.Columns.Add("accountName", typeof(string));
            dtLog.Columns.Add("accountUnit", typeof(string));
            dtLog.Columns.Add("logDateTime", typeof(DateTime));

            // 填寫資料 
            DateTime now = DateTime.Now;

            foreach (var item in insertList)
            {
                // --- A. 填寫附件資料 ---
                DataRow rowAttach = dtAttach.NewRow();
                rowAttach["healthID"] = item.targetID;
                rowAttach["fileName"] = item.originalFileName;
                rowAttach["filePath"] = item.virtualPath;

                int fSize = (item.infoRef != null && item.infoRef.uploadedFile != null)
                            ? item.infoRef.uploadedFile.ContentLength : 0;
                rowAttach["fileSize"] = fSize;
                rowAttach["description"] = "批次上傳";
                rowAttach["insertAccountID"] = accountID;
                rowAttach["insertDateTime"] = now;
                dtAttach.Rows.Add(rowAttach);

                // --- B. 填寫 Log 資料 ---
                DataRow rowLog = dtLog.NewRow();
                rowLog["functionType"] = protectTreesV2.TreeLog.LogFunctionTypes.Health.ToString();
                rowLog["dataID"] = item.targetID;
                rowLog["actionType"] = "批次上傳";
                rowLog["memo"] = item.IsOverwriteAction ? "覆蓋附件" : "上傳附件";
                rowLog["ipAddress"] = ipAddress;
                rowLog["accountID"] = accountID;
                rowLog["account"] = account;
                rowLog["accountName"] = accountName;
                rowLog["accountUnit"] = accountUnit;
                rowLog["logDateTime"] = now;
                dtLog.Rows.Add(rowLog);
            }

            // 開始執行資料庫交易
            using (var db = new MS_SQL())
            {
                try
                {
                    //開啟交易
                    db.StartTransaction();

                    // 軟刪除 
                    if (isOverwrite)
                    {
                        // 取出所有涉及的 HealthID
                        var distinctIDs = insertList.Select(x => x.targetID).Distinct().ToList();
                        if (distinctIDs.Count > 0)
                        {
                            string idList = string.Join(",", distinctIDs);
                            string sqlUpdate = $@"
                                UPDATE Tree_HealthAttachment 
                                SET removeDateTime = GETDATE(), removeAccountID = @acc
                                WHERE healthID IN ({idList}) AND removeDateTime IS NULL";

                            db.ExecNonQuery(sqlUpdate, new System.Data.SqlClient.SqlParameter("@acc", accountID));
                        }
                    }

                    // 批次寫入附件
                    if (dtAttach.Rows.Count > 0)
                    {
                        db.BulkCopy("Tree_HealthAttachment", dtAttach);
                    }

                    if (dtLog.Rows.Count > 0)
                    {
                        db.BulkCopy("Tree_Log", dtLog);
                    }

                    // 全部成功，提交
                    db.Commit();
                }
                catch (Exception ex)
                {
                    db.RollBack();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批次取得養護記錄流水號
        /// </summary>
        /// <param name="keys">查詢條件 (TreeID + CheckDate)</param>
        public Dictionary<string, int> GetCareIDMap(List<TreeQueryKey> keys)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            if (keys == null || keys.Count == 0) return map;

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                SELECT treeID, careDate, careID 
                FROM Tree_CareRecord
                WHERE removeDateTime IS NULL 
                  AND (
            ");

            List<SqlParameter> parameters = new List<SqlParameter>();
            List<string> conditions = new List<string>();

            for (int i = 0; i < keys.Count; i++)
            {
                string pTreeID = $"@t{i}";
                string pDate = $"@d{i}";

                // 條件直接對應 treeID
                conditions.Add($" (treeID = {pTreeID} AND careDate = {pDate}) ");

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
                    DateTime dbDate = Convert.ToDateTime(row["careDate"]);
                    int dbCarethID = Convert.ToInt32(row["careID"]);

                    string key = $"{dbTreeID}_{dbDate:yyyyMMdd}";

                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, dbCarethID);
                    }
                }
            }

            return map;
        }
        /// <summary>
        /// 將 CareRecord 轉為 DataTable
        /// </summary>
        private System.Data.DataTable CreateCareRecordDataTable(List<CareRecord> list, bool forUpdate)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            // ==========================================
            // A. 定義欄位 (對應 Tree_CareRecord)
            // ==========================================

            // 如果是更新模式，必須包含 PK
            if (forUpdate) dt.Columns.Add("careID", typeof(int));

            // 基本資料
            dt.Columns.Add("treeID", typeof(int));
            dt.Columns.Add("careDate", typeof(DateTime));
            dt.Columns.Add("recorder", typeof(string));
            dt.Columns.Add("reviewer", typeof(string));
            dt.Columns.Add("dataStatus", typeof(byte)); // tinyint 對應 byte

            // 樹冠枝葉 (Crown)
            dt.Columns.Add("crownStatus", typeof(byte));
            dt.Columns.Add("crownSeasonalDormant", typeof(bool));
            dt.Columns.Add("crownDeadBranch", typeof(bool));
            dt.Columns.Add("crownDeadBranchPercent", typeof(decimal));
            dt.Columns.Add("crownPest", typeof(bool));
            dt.Columns.Add("crownForeignObject", typeof(bool));
            dt.Columns.Add("crownOtherNote", typeof(string));

            // 主莖幹 (Trunk)
            dt.Columns.Add("trunkStatus", typeof(byte));
            dt.Columns.Add("trunkBarkDamage", typeof(bool));
            dt.Columns.Add("trunkDecay", typeof(bool));
            dt.Columns.Add("trunkTermiteTrail", typeof(bool));
            dt.Columns.Add("trunkLean", typeof(bool));
            dt.Columns.Add("trunkFungus", typeof(bool));
            dt.Columns.Add("trunkGummosis", typeof(bool));
            dt.Columns.Add("trunkVine", typeof(bool));
            dt.Columns.Add("trunkOtherNote", typeof(string));

            // 根部 (Root)
            dt.Columns.Add("rootStatus", typeof(byte));
            dt.Columns.Add("rootDamage", typeof(bool));
            dt.Columns.Add("rootDecay", typeof(bool));
            dt.Columns.Add("rootExpose", typeof(bool));
            dt.Columns.Add("rootRot", typeof(bool));
            dt.Columns.Add("rootSucker", typeof(bool));
            dt.Columns.Add("rootOtherNote", typeof(string));

            // 生育地環境 (Env)
            dt.Columns.Add("envStatus", typeof(byte));
            dt.Columns.Add("envPitSmall", typeof(bool));
            dt.Columns.Add("envPaved", typeof(bool));
            dt.Columns.Add("envDebris", typeof(bool));
            dt.Columns.Add("envSoilCover", typeof(bool));
            dt.Columns.Add("envCompaction", typeof(bool));
            dt.Columns.Add("envWaterlog", typeof(bool));
            dt.Columns.Add("envNearFacility", typeof(bool));
            dt.Columns.Add("envOtherNote", typeof(string));

            // 鄰接物 (Adjacent)
            dt.Columns.Add("adjacentStatus", typeof(byte));
            dt.Columns.Add("adjacentBuilding", typeof(bool));
            dt.Columns.Add("adjacentWire", typeof(bool));
            dt.Columns.Add("adjacentSignal", typeof(bool));
            dt.Columns.Add("adjacentOtherNote", typeof(string));

            // 養護作業項目 (Tasks)
            dt.Columns.Add("task1Status", typeof(byte));
            dt.Columns.Add("task1Note", typeof(string));
            dt.Columns.Add("task2Status", typeof(byte));
            dt.Columns.Add("task2Note", typeof(string));
            dt.Columns.Add("task3Status", typeof(byte));
            dt.Columns.Add("task3Note", typeof(string));
            dt.Columns.Add("task4Status", typeof(byte));
            dt.Columns.Add("task4Note", typeof(string));
            dt.Columns.Add("task5Status", typeof(byte));
            dt.Columns.Add("task5Note", typeof(string));

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

                if (forUpdate) row["careID"] = item.careID;

                row["treeID"] = item.treeID;
                // Date 欄位要注意 Nullable 轉換
                row["careDate"] = item.careDate ?? (object)DBNull.Value;
                row["recorder"] = item.recorder ?? (object)DBNull.Value;
                row["reviewer"] = item.reviewer ?? (object)DBNull.Value;
                row["dataStatus"] = item.dataStatus; // int 轉 byte, 假設範圍正確

                // Crown
                row["crownStatus"] = item.crownStatus ?? (object)DBNull.Value;
                row["crownSeasonalDormant"] = item.crownSeasonalDormant ?? (object)DBNull.Value;
                row["crownDeadBranch"] = item.crownDeadBranch ?? (object)DBNull.Value;
                row["crownDeadBranchPercent"] = item.crownDeadBranchPercent ?? (object)DBNull.Value;
                row["crownPest"] = item.crownPest ?? (object)DBNull.Value;
                row["crownForeignObject"] = item.crownForeignObject ?? (object)DBNull.Value;
                row["crownOtherNote"] = item.crownOtherNote ?? (object)DBNull.Value;

                // Trunk
                row["trunkStatus"] = item.trunkStatus ?? (object)DBNull.Value;
                row["trunkBarkDamage"] = item.trunkBarkDamage ?? (object)DBNull.Value;
                row["trunkDecay"] = item.trunkDecay ?? (object)DBNull.Value;
                row["trunkTermiteTrail"] = item.trunkTermiteTrail ?? (object)DBNull.Value;
                row["trunkLean"] = item.trunkLean ?? (object)DBNull.Value;
                row["trunkFungus"] = item.trunkFungus ?? (object)DBNull.Value;
                row["trunkGummosis"] = item.trunkGummosis ?? (object)DBNull.Value;
                row["trunkVine"] = item.trunkVine ?? (object)DBNull.Value;
                row["trunkOtherNote"] = item.trunkOtherNote ?? (object)DBNull.Value;

                // Root
                row["rootStatus"] = item.rootStatus ?? (object)DBNull.Value;
                row["rootDamage"] = item.rootDamage ?? (object)DBNull.Value;
                row["rootDecay"] = item.rootDecay ?? (object)DBNull.Value;
                row["rootExpose"] = item.rootExpose ?? (object)DBNull.Value;
                row["rootRot"] = item.rootRot ?? (object)DBNull.Value;
                row["rootSucker"] = item.rootSucker ?? (object)DBNull.Value;
                row["rootOtherNote"] = item.rootOtherNote ?? (object)DBNull.Value;

                // Env
                row["envStatus"] = item.envStatus ?? (object)DBNull.Value;
                row["envPitSmall"] = item.envPitSmall ?? (object)DBNull.Value;
                row["envPaved"] = item.envPaved ?? (object)DBNull.Value;
                row["envDebris"] = item.envDebris ?? (object)DBNull.Value;
                row["envSoilCover"] = item.envSoilCover ?? (object)DBNull.Value;
                row["envCompaction"] = item.envCompaction ?? (object)DBNull.Value;
                row["envWaterlog"] = item.envWaterlog ?? (object)DBNull.Value;
                row["envNearFacility"] = item.envNearFacility ?? (object)DBNull.Value;
                row["envOtherNote"] = item.envOtherNote ?? (object)DBNull.Value;

                // Adjacent
                row["adjacentStatus"] = item.adjacentStatus ?? (object)DBNull.Value;
                row["adjacentBuilding"] = item.adjacentBuilding ?? (object)DBNull.Value;
                row["adjacentWire"] = item.adjacentWire ?? (object)DBNull.Value;
                row["adjacentSignal"] = item.adjacentSignal ?? (object)DBNull.Value;
                row["adjacentOtherNote"] = item.adjacentOtherNote ?? (object)DBNull.Value;

                // Tasks
                row["task1Status"] = item.task1Status ?? (object)DBNull.Value;
                row["task1Note"] = item.task1Note ?? (object)DBNull.Value;
                row["task2Status"] = item.task2Status ?? (object)DBNull.Value;
                row["task2Note"] = item.task2Note ?? (object)DBNull.Value;
                row["task3Status"] = item.task3Status ?? (object)DBNull.Value;
                row["task3Note"] = item.task3Note ?? (object)DBNull.Value;
                row["task4Status"] = item.task4Status ?? (object)DBNull.Value;
                row["task4Note"] = item.task4Note ?? (object)DBNull.Value;
                row["task5Status"] = item.task5Status ?? (object)DBNull.Value;
                row["task5Note"] = item.task5Note ?? (object)DBNull.Value;

                // System
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
        /// <summary>
        /// 批次新增養護紀錄 
        /// </summary>
        /// <param name="list">要新增的資料清單</param>
        /// <param name="accountID">操作者 ID</param>
        /// <param name="accountName">操作者姓名</param>
        /// <param name="ipAddress">來源 IP</param>
        public void BulkInsertCareRecords(List<CareRecord> list, string ipAddress, int accountID, string account, string accountName, string accountUnit)
        {
            if (list == null || list.Count == 0) return;

            // 準備 Log 的 DataTable
            System.Data.DataTable dtLog = new System.Data.DataTable();
            dtLog.Columns.Add("functionType", typeof(string));
            dtLog.Columns.Add("dataID", typeof(int));
            dtLog.Columns.Add("actionType", typeof(string));
            dtLog.Columns.Add("memo", typeof(string));
            dtLog.Columns.Add("ipAddress", typeof(string));
            dtLog.Columns.Add("accountID", typeof(int));
            dtLog.Columns.Add("account", typeof(string));
            dtLog.Columns.Add("accountName", typeof(string));
            dtLog.Columns.Add("accountUnit", typeof(string));
            dtLog.Columns.Add("logDateTime", typeof(DateTime));

            DateTime now = DateTime.Now;

            using (var da = new MS_SQL())
            {
                try
                {
                    // 開啟交易
                    da.StartTransaction();

                    string sqlInsert = @"
                        INSERT INTO Tree_CareRecord 
                        (
                            /* [基本資料] */
                            treeID, careDate, recorder, reviewer, dataStatus,

                            /* [樹冠狀態] */
                            crownStatus, crownSeasonalDormant, crownDeadBranch, crownDeadBranchPercent,
                            crownPest, crownForeignObject, crownOtherNote,

                            /* [主莖幹狀態] */
                            trunkStatus, trunkBarkDamage, trunkDecay, trunkTermiteTrail,
                            trunkLean, trunkFungus, trunkGummosis, trunkVine, trunkOtherNote,

                            /* [根部狀態] */
                            rootStatus, rootDamage, rootDecay, rootExpose,
                            rootRot, rootSucker, rootOtherNote,

                            /* [生育地環境] */
                            envStatus, envPitSmall, envPaved, envDebris,
                            envSoilCover, envCompaction, envWaterlog, envNearFacility, envOtherNote,

                            /* [鄰接物] */
                            adjacentStatus, adjacentBuilding, adjacentWire, adjacentSignal, adjacentOtherNote,

                            /* [維護作業項目] */
                            task1Status, task1Note,
                            task2Status, task2Note,
                            task3Status, task3Note,
                            task4Status, task4Note,
                            task5Status, task5Note,

                            /* [系統欄位] */
                            insertAccountID, insertDateTime
                        )
                        VALUES 
                        (
                            /* [基本資料] */
                            @treeID, @careDate, @recorder, @reviewer, @dataStatus,

                            /* [樹冠狀態] */
                            @crownStatus, @crownSeasonalDormant, @crownDeadBranch, @crownDeadBranchPercent,
                            @crownPest, @crownForeignObject, @crownOtherNote,

                            /* [主莖幹狀態] */
                            @trunkStatus, @trunkBarkDamage, @trunkDecay, @trunkTermiteTrail,
                            @trunkLean, @trunkFungus, @trunkGummosis, @trunkVine, @trunkOtherNote,

                            /* [根部狀態] */
                            @rootStatus, @rootDamage, @rootDecay, @rootExpose,
                            @rootRot, @rootSucker, @rootOtherNote,

                            /* [生育地環境] */
                            @envStatus, @envPitSmall, @envPaved, @envDebris,
                            @envSoilCover, @envCompaction, @envWaterlog, @envNearFacility, @envOtherNote,

                            /* [鄰接物] */
                            @adjacentStatus, @adjacentBuilding, @adjacentWire, @adjacentSignal, @adjacentOtherNote,

                            /* [維護作業項目] */
                            @task1Status, @task1Note,
                            @task2Status, @task2Note,
                            @task3Status, @task3Note,
                            @task4Status, @task4Note,
                            @task5Status, @task5Note,

                            /* [系統欄位] */
                            @acc, @time
                        );
                
                        SELECT CAST(SCOPE_IDENTITY() AS int); 
                    ";

                    foreach (var item in list)
                    {
                        List<System.Data.SqlClient.SqlParameter> p = new List<System.Data.SqlClient.SqlParameter>();

                        // --- 基本資料 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@treeID", item.treeID));
                        p.Add(new System.Data.SqlClient.SqlParameter("@careDate", item.careDate));
                        p.Add(new System.Data.SqlClient.SqlParameter("@recorder", item.recorder ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@reviewer", item.reviewer ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@dataStatus", item.dataStatus)); 

                        // --- 樹冠狀態 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownStatus", item.crownStatus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownSeasonalDormant", item.crownSeasonalDormant ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownDeadBranch", item.crownDeadBranch ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownDeadBranchPercent", item.crownDeadBranchPercent ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownPest", item.crownPest ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownForeignObject", item.crownForeignObject ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@crownOtherNote", item.crownOtherNote ?? (object)DBNull.Value));

                        // --- 主莖幹狀態 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkStatus", item.trunkStatus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkBarkDamage", item.trunkBarkDamage ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkDecay", item.trunkDecay ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkTermiteTrail", item.trunkTermiteTrail ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkLean", item.trunkLean ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkFungus", item.trunkFungus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkGummosis", item.trunkGummosis ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkVine", item.trunkVine ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@trunkOtherNote", item.trunkOtherNote ?? (object)DBNull.Value));

                        // --- 根部狀態 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootStatus", item.rootStatus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootDamage", item.rootDamage ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootDecay", item.rootDecay ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootExpose", item.rootExpose ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootRot", item.rootRot ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootSucker", item.rootSucker ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@rootOtherNote", item.rootOtherNote ?? (object)DBNull.Value));

                        // --- 生育地環境 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@envStatus", item.envStatus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envPitSmall", item.envPitSmall ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envPaved", item.envPaved ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envDebris", item.envDebris ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envSoilCover", item.envSoilCover ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envCompaction", item.envCompaction ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envWaterlog", item.envWaterlog ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envNearFacility", item.envNearFacility ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@envOtherNote", item.envOtherNote ?? (object)DBNull.Value));

                        // --- 鄰接物 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@adjacentStatus", item.adjacentStatus ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@adjacentBuilding", item.adjacentBuilding ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@adjacentWire", item.adjacentWire ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@adjacentSignal", item.adjacentSignal ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@adjacentOtherNote", item.adjacentOtherNote ?? (object)DBNull.Value));

                        // --- 維護作業項目 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@task1Status", item.task1Status ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task1Note", item.task1Note ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task2Status", item.task2Status ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task2Note", item.task2Note ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task3Status", item.task3Status ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task3Note", item.task3Note ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task4Status", item.task4Status ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task4Note", item.task4Note ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task5Status", item.task5Status ?? (object)DBNull.Value));
                        p.Add(new System.Data.SqlClient.SqlParameter("@task5Note", item.task5Note ?? (object)DBNull.Value));

                        // --- 系統欄位 ---
                        p.Add(new System.Data.SqlClient.SqlParameter("@acc", accountID));
                        p.Add(new System.Data.SqlClient.SqlParameter("@time", now));

                        // 執行 SQL 並取得 ID
                        object result = da.ExcuteScalar(sqlInsert, p.ToArray());

                        if (result != null && int.TryParse(result.ToString(), out int newCareID))
                        {
                            // 建立 Log 
                            System.Data.DataRow rowLog = dtLog.NewRow();
                            rowLog["functionType"] = protectTreesV2.TreeLog.LogFunctionTypes.Care;
                            rowLog["dataID"] = newCareID;    
                            rowLog["actionType"] = "批次上傳";
                            rowLog["memo"] = "新增養護";
                            rowLog["ipAddress"] = ipAddress;
                            rowLog["accountID"] = accountID;
                            rowLog["account"] = account;
                            rowLog["accountName"] = accountName;
                            rowLog["accountUnit"] = accountUnit;
                            rowLog["logDateTime"] = now;
                            dtLog.Rows.Add(rowLog);
                        }
                    }

                    // 批次寫入 Log
                    if (dtLog.Rows.Count > 0)
                    {
                        da.BulkCopy("Tree_Log", dtLog);
                    }

                    // 提交交易
                    da.Commit();
                }
                catch (Exception ex)
                {
                    da.RollBack();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批次更新養護紀錄 
        /// </summary>
        public void BulkUpdateCareRecords(List<CareRecord> list, string ipAddress, int accountID, string account, string accountName, string accountUnit)
        {
            if (list == null || list.Count == 0) return;

            // 產生 Update 用的 DataTable
            System.Data.DataTable dt = CreateCareRecordDataTable(list, forUpdate: true);

            // ===============================================
            // 準備 Log 的 DataTable
            // ===============================================
            System.Data.DataTable dtLog = new System.Data.DataTable();
            dtLog.Columns.Add("functionType", typeof(string));
            dtLog.Columns.Add("dataID", typeof(int));
            dtLog.Columns.Add("actionType", typeof(string));
            dtLog.Columns.Add("memo", typeof(string));
            dtLog.Columns.Add("ipAddress", typeof(string));
            dtLog.Columns.Add("accountID", typeof(int));
            dtLog.Columns.Add("account", typeof(string));
            dtLog.Columns.Add("accountName", typeof(string));
            dtLog.Columns.Add("accountUnit", typeof(string));
            dtLog.Columns.Add("logDateTime", typeof(DateTime));


            DateTime now = DateTime.Now;

            // 填寫 Log 資料
            foreach (var item in list)
            {
                System.Data.DataRow rowLog = dtLog.NewRow();
                rowLog["functionType"] = protectTreesV2.TreeLog.LogFunctionTypes.Care; 
                rowLog["dataID"] = item.careID;  
                rowLog["actionType"] = "批次上傳";
                rowLog["memo"] = "更新養護";
                rowLog["ipAddress"] = ipAddress;
                rowLog["accountID"] = accountID;
                rowLog["account"] = account;
                rowLog["accountName"] = accountName;
                rowLog["accountUnit"] = accountUnit;
                rowLog["logDateTime"] = now;
                dtLog.Rows.Add(rowLog);
            }

            using (var da = new MS_SQL())
            {
                try
                {
                    // 開啟交易
                    da.StartTransaction();

                    // ===============================================
                    // 建立暫存表
                    // ===============================================
                    string createTempSql = @"
                        IF OBJECT_ID('tempdb..#TempCareUpdate') IS NOT NULL DROP TABLE #TempCareUpdate;

                        CREATE TABLE #TempCareUpdate (
                            [careID] [int] NOT NULL,
                            [treeID] [int] NULL,
                            [careDate] [date] NULL,
                            [recorder] [nvarchar](100) NULL,
                            [reviewer] [nvarchar](100) NULL,
                            [dataStatus] [tinyint] NULL,

                            [crownStatus] [tinyint] NULL,
                            [crownSeasonalDormant] [bit] NULL,
                            [crownDeadBranch] [bit] NULL,
                            [crownDeadBranchPercent] [decimal](5, 2) NULL,
                            [crownPest] [bit] NULL,
                            [crownForeignObject] [bit] NULL,
                            [crownOtherNote] [nvarchar](200) NULL,

                            [trunkStatus] [tinyint] NULL,
                            [trunkBarkDamage] [bit] NULL,
                            [trunkDecay] [bit] NULL,
                            [trunkTermiteTrail] [bit] NULL,
                            [trunkLean] [bit] NULL,
                            [trunkFungus] [bit] NULL,
                            [trunkGummosis] [bit] NULL,
                            [trunkVine] [bit] NULL,
                            [trunkOtherNote] [nvarchar](200) NULL,

                            [rootStatus] [tinyint] NULL,
                            [rootDamage] [bit] NULL,
                            [rootDecay] [bit] NULL,
                            [rootExpose] [bit] NULL,
                            [rootRot] [bit] NULL,
                            [rootSucker] [bit] NULL,
                            [rootOtherNote] [nvarchar](200) NULL,

                            [envStatus] [tinyint] NULL,
                            [envPitSmall] [bit] NULL,
                            [envPaved] [bit] NULL,
                            [envDebris] [bit] NULL,
                            [envSoilCover] [bit] NULL,
                            [envCompaction] [bit] NULL,
                            [envWaterlog] [bit] NULL,
                            [envNearFacility] [bit] NULL,
                            [envOtherNote] [nvarchar](200) NULL,

                            [adjacentStatus] [tinyint] NULL,
                            [adjacentBuilding] [bit] NULL,
                            [adjacentWire] [bit] NULL,
                            [adjacentSignal] [bit] NULL,
                            [adjacentOtherNote] [nvarchar](200) NULL,

                            [task1Status] [tinyint] NULL,
                            [task1Note] [nvarchar](500) NULL,
                            [task2Status] [tinyint] NULL,
                            [task2Note] [nvarchar](500) NULL,
                            [task3Status] [tinyint] NULL,
                            [task3Note] [nvarchar](500) NULL,
                            [task4Status] [tinyint] NULL,
                            [task4Note] [nvarchar](500) NULL,
                            [task5Status] [tinyint] NULL,
                            [task5Note] [nvarchar](500) NULL,

                            [updateAccountID] [int] NULL,
                            [updateDateTime] [datetime] NULL
                        );
                    ";
                    da.ExecNonQuery(createTempSql);

                    // ===============================================
                    // Step B: 批次寫入暫存表 - [保持不動]
                    // ===============================================
                    da.BulkCopy("#TempCareUpdate", dt);

                    // ===============================================
                    // Step C: 從暫存表更新主表 (列出所有欄位) - [保持不動]
                    // ===============================================
                    string updateSql = @"
                    UPDATE T
                    SET 
                        T.treeID = S.treeID,
                        T.careDate = S.careDate,
                        T.recorder = S.recorder,
                        T.reviewer = S.reviewer,
                        /*T.dataStatus = CASE WHEN @UpdateStatus = 1 THEN S.dataStatus ELSE T.dataStatus END,*/

                        T.crownStatus = S.crownStatus,
                        T.crownSeasonalDormant = S.crownSeasonalDormant,
                        T.crownDeadBranch = S.crownDeadBranch,
                        T.crownDeadBranchPercent = S.crownDeadBranchPercent,
                        T.crownPest = S.crownPest,
                        T.crownForeignObject = S.crownForeignObject,
                        T.crownOtherNote = S.crownOtherNote,

                        T.trunkStatus = S.trunkStatus,
                        T.trunkBarkDamage = S.trunkBarkDamage,
                        T.trunkDecay = S.trunkDecay,
                        T.trunkTermiteTrail = S.trunkTermiteTrail,
                        T.trunkLean = S.trunkLean,
                        T.trunkFungus = S.trunkFungus,
                        T.trunkGummosis = S.trunkGummosis,
                        T.trunkVine = S.trunkVine,
                        T.trunkOtherNote = S.trunkOtherNote,

                        T.rootStatus = S.rootStatus,
                        T.rootDamage = S.rootDamage,
                        T.rootDecay = S.rootDecay,
                        T.rootExpose = S.rootExpose,
                        T.rootRot = S.rootRot,
                        T.rootSucker = S.rootSucker,
                        T.rootOtherNote = S.rootOtherNote,

                        T.envStatus = S.envStatus,
                        T.envPitSmall = S.envPitSmall,
                        T.envPaved = S.envPaved,
                        T.envDebris = S.envDebris,
                        T.envSoilCover = S.envSoilCover,
                        T.envCompaction = S.envCompaction,
                        T.envWaterlog = S.envWaterlog,
                        T.envNearFacility = S.envNearFacility,
                        T.envOtherNote = S.envOtherNote,

                        T.adjacentStatus = S.adjacentStatus,
                        T.adjacentBuilding = S.adjacentBuilding,
                        T.adjacentWire = S.adjacentWire,
                        T.adjacentSignal = S.adjacentSignal,
                        T.adjacentOtherNote = S.adjacentOtherNote,

                        T.task1Status = S.task1Status,
                        T.task1Note = S.task1Note,
                        T.task2Status = S.task2Status,
                        T.task2Note = S.task2Note,
                        T.task3Status = S.task3Status,
                        T.task3Note = S.task3Note,
                        T.task4Status = S.task4Status,
                        T.task4Note = S.task4Note,
                        T.task5Status = S.task5Status,
                        T.task5Note = S.task5Note,

                        T.updateAccountID = S.updateAccountID,
                        T.updateDateTime = GETDATE()
                    FROM Tree_CareRecord T
                    INNER JOIN #TempCareUpdate S ON T.careID = S.careID;

                    DROP TABLE #TempCareUpdate;
                ";

                    // 執行 Update
                    da.ExecNonQuery(updateSql);

                    // ===============================================
                    // 批次寫入 Log
                    // ===============================================
                    if (dtLog.Rows.Count > 0)
                    {
                        da.BulkCopy("Tree_Log", dtLog);
                    }

                    // 全部成功，提交交易
                    da.Commit();
                }
                catch (Exception ex)
                {
                    da.RollBack();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批次取得巡查流水號
        /// </summary>
        /// <param name="keys">查詢條件 (TreeID + PatrolDate)</param>
        public Dictionary<string, int> GetPatrolIDMap(List<TreeQueryKey> keys)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            if (keys == null || keys.Count == 0) return map;

            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                SELECT treeID, patrolDate, patrolID 
                FROM Tree_PatrolRecord
                WHERE removeDateTime IS NULL 
                AND (
            ");

            List<SqlParameter> parameters = new List<SqlParameter>();
            List<string> conditions = new List<string>();

            for (int i = 0; i < keys.Count; i++)
            {
                string pTreeID = $"@t{i}";
                string pDate = $"@d{i}";

                conditions.Add($" (treeID = {pTreeID} AND patrolDate = {pDate}) ");

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

                    DateTime dbDate = Convert.ToDateTime(row["patrolDate"]);
                    int dbPatrolID = Convert.ToInt32(row["patrolID"]);

                    // 組合 Key
                    string key = $"{dbTreeID}_{dbDate:yyyyMMdd}";

                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, dbPatrolID);
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// 批次新增巡查紀錄 
        /// </summary>
        /// <param name="keys">要新增的鍵值 (TreeID + Date)</param>
        /// <param name="accountID">建立者 ID</param>
        /// <returns>Map: "treeID_yyyyMMdd" -> 新產生的 PatrolID</returns>
        public Dictionary<string, int> BatchCreatePatrolRecords(List<TreeQueryKey> keys, int accountID)
        {
            Dictionary<string, int> newMap = new Dictionary<string, int>();
            if (keys == null || keys.Count == 0) return newMap;

            using (var da = new MS_SQL())
            {
                try
                {
                    // 開啟交易
                    da.StartTransaction();

                    // SQL 語句
                    string sql = @"
                        INSERT INTO Tree_PatrolRecord 
                        (
                            treeID, patrolDate, dataStatus, 
                            hasPublicSafetyRisk, /* 必填欄位，需給預設值 */
                            insertAccountID, insertDateTime
                        ) 
                        VALUES 
                        (
                            @tid, @pDate, 0,  /* 0:草稿 */
                            0,                /* 0:預設無風險 */
                            @accID, GETDATE()
                        );
                
                        /* 取回新 ID */
                        SELECT CAST(SCOPE_IDENTITY() AS int); 
                    ";

                    foreach (var k in keys)
                    {
                        List<System.Data.SqlClient.SqlParameter> p = new List<System.Data.SqlClient.SqlParameter>();
                        p.Add(new System.Data.SqlClient.SqlParameter("@tid", k.treeID));
                        p.Add(new System.Data.SqlClient.SqlParameter("@pDate", k.checkDate)); // 假設您的 QueryKey 日期屬性名為 checkDate
                        p.Add(new System.Data.SqlClient.SqlParameter("@accID", accountID));

                        object result = da.ExcuteScalar(sql, p.ToArray());

                        if (result != null && result != DBNull.Value)
                        {
                            int newID = Convert.ToInt32(result);
                            // 組合 Key: TreeID_yyyyMMdd
                            string key = $"{k.treeID}_{k.checkDate:yyyyMMdd}";

                            if (!newMap.ContainsKey(key))
                            {
                                newMap.Add(key, newID);
                            }
                        }
                    }

                    // 全部成功，提交交易
                    da.Commit();
                }
                catch (Exception ex)
                {
                    // 發生錯誤，回滾
                    da.RollBack();
                    throw ex;
                }
            }

            return newMap;
        }

        /// <summary>
        /// 批次取得指定巡查紀錄照片數量 
        /// </summary>
        /// <param name="patrolIDs">巡查紀錄 ID 清單</param>
        public Dictionary<int, int> GetBatchPatrolPhotoCounts(List<int> patrolIDs)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            if (patrolIDs == null || patrolIDs.Count == 0) return map;

            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                SELECT patrolID, COUNT(*) as Cnt 
                FROM Tree_PatrolPhoto 
                WHERE removeDateTime IS NULL 
                  AND patrolID IN (
            ");

            List<SqlParameter> parameters = new List<SqlParameter>();
            List<string> paramNames = new List<string>();

            for (int i = 0; i < patrolIDs.Count; i++)
            {
                string pName = $"@p{i}";
                paramNames.Add(pName);
                parameters.Add(new SqlParameter(pName, patrolIDs[i]));
            }

            sb.Append(string.Join(",", paramNames));
            sb.Append(") GROUP BY patrolID");

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sb.ToString(), parameters.ToArray());

                foreach (DataRow row in dt.Rows)
                {
                    int pID = Convert.ToInt32(row["patrolID"]);
                    int count = Convert.ToInt32(row["Cnt"]);

                    if (!map.ContainsKey(pID))
                    {
                        map.Add(pID, count);
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// 批次寫入巡查照片紀錄
        /// </summary>
        public void BatchInsertPatrolPhotoRecords(List<TempFileData> photos, string ipAddress, int accountID, string account, string accountName, string accountUnit)
        {
            if (photos == null || photos.Count == 0) return;

            // 準備照片資料表
            DataTable dtPhoto = new DataTable();
            dtPhoto.Columns.Add("patrolID", typeof(int)); 
            dtPhoto.Columns.Add("fileName", typeof(string));
            dtPhoto.Columns.Add("filePath", typeof(string));
            dtPhoto.Columns.Add("fileSize", typeof(int));
            dtPhoto.Columns.Add("caption", typeof(string));
            dtPhoto.Columns.Add("insertAccountID", typeof(int));
            dtPhoto.Columns.Add("insertDateTime", typeof(DateTime));

            // 準備 Log 資料表 
            DataTable dtLog = new DataTable();
            dtLog.Columns.Add("functionType", typeof(string));
            dtLog.Columns.Add("dataID", typeof(int));
            dtLog.Columns.Add("actionType", typeof(string));
            dtLog.Columns.Add("memo", typeof(string));
            dtLog.Columns.Add("ipAddress", typeof(string));
            dtLog.Columns.Add("accountID", typeof(int));
            dtLog.Columns.Add("account", typeof(string));
            dtLog.Columns.Add("accountName", typeof(string));
            dtLog.Columns.Add("accountUnit", typeof(string));
            dtLog.Columns.Add("logDateTime", typeof(DateTime));

            DateTime now = DateTime.Now;

            // ==========================================
            // 填寫照片資料
            // ==========================================
            foreach (var p in photos)
            {
                DataRow row = dtPhoto.NewRow();
                row["patrolID"] = p.targetID;
                row["fileName"] = p.originalFileName;
                row["filePath"] = p.virtualPath;

                int fSize = (p.infoRef != null && p.infoRef.uploadedFile != null)
                            ? p.infoRef.uploadedFile.ContentLength : 0;
                row["fileSize"] = fSize;
                row["caption"] = DBNull.Value;
                row["insertAccountID"] = accountID;
                row["insertDateTime"] = now;
                dtPhoto.Rows.Add(row);
            }

            // ==========================================
            // 填寫 Log 資料 (依 PatrolID 分組)
            // ==========================================
            var groupedPhotos = photos.GroupBy(p => p.targetID);

            foreach (var group in groupedPhotos)
            {
                int currentPatrolID = group.Key;
                int count = group.Count();

                // 組合備註
                string memoStr = $"批次上傳 {count} 張照片";

                DataRow rowLog = dtLog.NewRow();
                rowLog["functionType"] = protectTreesV2.TreeLog.LogFunctionTypes.Patrol;
                rowLog["dataID"] = currentPatrolID;
                rowLog["actionType"] = "批次上傳";
                rowLog["memo"] = memoStr;
                rowLog["ipAddress"] = ipAddress;
                rowLog["accountID"] = accountID;
                rowLog["account"] = account;
                rowLog["accountName"] = accountName;
                rowLog["accountUnit"] = accountUnit;
                rowLog["logDateTime"] = now;
                dtLog.Rows.Add(rowLog);
            }

            // ==========================================
            // 執行寫入
            // ==========================================
            using (var db = new MS_SQL())
            {
                try
                {
                    db.StartTransaction();

                    if (dtPhoto.Rows.Count > 0)
                        db.BulkCopy("Tree_PatrolPhoto", dtPhoto);

                    if (dtLog.Rows.Count > 0)
                        db.BulkCopy("Tree_Log", dtLog);

                    db.Commit();
                }
                catch (Exception ex)
                {
                    db.RollBack();
                    throw ex;
                }
            }
        }
    }
}