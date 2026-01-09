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
                LEFT JOIN User_Account u ON t.insertAccountID = u.accountID
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
        /// 取得某個使用者「最新一筆任務」的「所有 Log 明細」
        /// (用於頁面載入時直接顯示剛才或上次的執行結果)
        /// </summary>
        public List<TreeBatchTaskLog> GetLatestBatchTaskLogs(int accountID)
        {
            var logs = new List<TreeBatchTaskLog>();

            string sql = @"
                SELECT * FROM Tree_BatchTaskLog 
                WHERE taskID = (
                    SELECT TOP 1 taskID 
                    FROM Tree_BatchTask 
                    WHERE insertAccountID = @accountID 
                      AND removeDateTime IS NULL 
                    ORDER BY insertDateTime DESC
                )
                ORDER BY logID";

            using (var da = new MS_SQL())
            {
                DataTable dt = da.GetDataTable(sql, new SqlParameter("@accountID", accountID));
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
                                    @tid, @sDate, 0,  -- 0:草稿
                                    @accID, GETDATE()
                                );
                                SELECT SCOPE_IDENTITY(); -- 取回新 ID
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