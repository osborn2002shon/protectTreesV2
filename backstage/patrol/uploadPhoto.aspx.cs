using protectTreesV2.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static protectTreesV2.Batch.Batch;

namespace protectTreesV2.backstage.patrol
{
    public partial class uploadPhoto : BasePage
    {
        protectTreesV2.Batch.Batch system_batch = new Batch.Batch();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }
        private void BindData()
        {
            // 取得當前使用者 ID
            var user = UserInfo.GetCurrentUser;
            int userId = user?.accountID ?? 0;

            // 綁定歷史紀錄
            var historyList = system_batch.GetBatchTaskList(enum_treeBatchType.Patrol_Photo, userId);
            GridView_History.DataSource = historyList.Take(5).ToList();
            GridView_History.DataBind();

            // 綁定最新一筆的明細 (若有歷史紀錄)
            if (historyList.Count > 0)
            {
                var latest = historyList[0];
                Label_LastStatus.Text = $"最新上傳：{latest.insertDateTime?.ToString("yyyy/MM/dd HH:mm")} (成功 {latest.successCount} / 失敗 {latest.failCount})";

                var details = system_batch.GetLatestBatchTaskLogs(latest.taskID);
                GridView_Detail.DataSource = details;
                GridView_Detail.DataBind();
            }
            else
            {
                Label_LastStatus.Text = "目前尚無上傳紀錄";
                GridView_Detail.DataSource = null;
                GridView_Detail.DataBind();
            }
        }

        protected void Button_StartUpload_Click(object sender, EventArgs e)
        {
            if (!FileUpload_Batch.HasFiles)
            {
                ShowMessage("提示", "請先選擇要上傳的檔案！");
                return;
            }
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            HttpFileCollection files = Request.Files;

            // 總表：最後要寫入資料庫的紀錄
            List<TreeBatchTaskLog> allLogList = new List<TreeBatchTaskLog>();

            // 工作佇列：只有解析成功的項目會進入此清單往下跑
            List<TreeFileInfo> processQueue = new List<TreeFileInfo>();

            // =========================================================
            // 解析與格式驗證
            // =========================================================
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFile f = files[i];

                // 略過瀏覽器空的 input
                if (string.IsNullOrEmpty(f.FileName)) continue;

                TreeBatchTaskLog myLog = new TreeBatchTaskLog
                {
                    taskID = 0,
                    sourceItem = Path.GetFileName(f.FileName),
                    isSuccess = false,
                    resultMsg = ""
                };
                allLogList.Add(myLog);

                TreeFileInfo info = new TreeFileInfo
                {
                    uploadedFile = f,
                    log = myLog
                };

                // --- 檢查 1: 空檔案 ---
                if (f.ContentLength == 0)
                {
                    info.isProcessing = false;
                    myLog.resultMsg = "失敗：檔案內容為空或損毀";
                    continue;
                }

                // --- 檢查 2: 檔案大小 (> 10MB) ---
                int maxSizeBytes = 10 * 1024 * 1024; // 10MB
                if (f.ContentLength > maxSizeBytes)
                {
                    info.isProcessing = false;
                    myLog.resultMsg = "失敗：檔案大小超過 10MB";
                    continue;
                }

                // --- 檢查 3: 副檔名 (JPG, PNG) ---
                string ext = Path.GetExtension(f.FileName).ToLower();
                string[] allowedExts = { ".jpg", ".jpeg", ".png" };
                if (!allowedExts.Contains(ext))
                {
                    info.isProcessing = false;
                    myLog.resultMsg = "失敗：檔案格式不正確";
                    continue;
                }

                // --- 檢查 4: 檔名解析 ---
                string[] parts = myLog.sourceItem.Split('_');
                if (parts.Length < 2)
                {
                    info.isProcessing = false;
                    myLog.resultMsg = "失敗：檔名格式錯誤";
                    continue;
                }

                // 解析 SystemTreeNo
                string tNo = parts[0].Trim();
                info.systemTreeNo = tNo;
                myLog.refKey = tNo;

                // 解析 Date
                string dStr = parts[1].Trim();
                if (DateTime.TryParseExact(dStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime d))
                {
                    info.checkDate = d;
                    myLog.refDate = d;
                }
                else
                {
                    info.isProcessing = false;
                    myLog.resultMsg = $"失敗：日期格式無效";
                    continue;
                }

                // 全部通過，加入處理佇列
                processQueue.Add(info);
            }

            // =========================================================
            // 資料庫批次驗證
            // =========================================================

            // 檢查樹籍編號
            if (processQueue.Count > 0)
            {
                List<string> distinctTreeNos = processQueue.Select(x => x.systemTreeNo).Distinct().ToList();
                Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(distinctTreeNos, accountID);

                foreach (var info in processQueue)
                {
                    // 檢查是否存在
                    if (treeIdMap.ContainsKey(info.systemTreeNo))
                    {
                        info.treeID = treeIdMap[info.systemTreeNo];
                    }
                    else
                    {
                        info.isProcessing = false;
                        info.log.resultMsg = $"失敗：查無系統樹籍編號";
                    }
                }
            }

            // 批次取得巡查流水號
            var activeItems = processQueue.Where(x => x.isProcessing).ToList();
            if (activeItems.Count > 0)
            {
                var queryKeys = activeItems
                    .Select(x => new TreeQueryKey { treeID = x.treeID, checkDate = x.checkDate })
                    .Distinct().ToList();

                Dictionary<string, int> healthMap = system_batch.GetPatrolIDMap(queryKeys);

                foreach (var info in activeItems)
                {
                    string key = $"{info.treeID}_{info.checkDate:yyyyMMdd}";
                    if (healthMap.ContainsKey(key))
                    {
                        info.targetID = healthMap[key];
                    }
                }
            }

            // 自動新增處理
            if (CheckBox_autoCreateDraft.Checked)
            {
                // 找出還沒有巡查資料的項目
                var toCreateList = activeItems.Where(x => x.targetID == 0).ToList();

                if (toCreateList.Count > 0)
                {
                    var createKeys = toCreateList
                        .GroupBy(x => new { x.treeID, x.checkDate })
                        .Select(g => new TreeQueryKey
                        {
                            treeID = g.Key.treeID,
                            checkDate = g.Key.checkDate
                        })
                        .ToList();
                    try
                    {
                        Dictionary<string, int> newPatrolMap = system_batch.BatchCreatePatrolRecords(createKeys, accountID);

                        foreach (var info in toCreateList)
                        {
                            // Key 組成：treeID_yyyyMMdd
                            string key = $"{info.treeID}_{info.checkDate:yyyyMMdd}";

                            if (newPatrolMap.ContainsKey(key))
                            {
                                // 成功新增
                                info.targetID = newPatrolMap[key];
                                info.log.resultMsg = "提醒：指定日期已自動新增健檢紀錄草稿";
                            }
                            else
                            {
                                // 失敗
                                info.isProcessing = false;
                                info.log.resultMsg = "失敗：自動新增失敗";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // --- 系統錯誤 ---
                        foreach (var info in toCreateList)
                        {
                            info.isProcessing = false;
                            info.log.isSuccess = false;
                            info.log.resultMsg = $"失敗：自動新增失敗";
                        }
                    }
                }
            }
            else
            {
                // 沒勾選自動新增，且沒 ID 的，直接失敗
                foreach (var info in activeItems.Where(x => x.targetID == 0))
                {
                    info.isProcessing = false;
                    info.log.resultMsg = "失敗：指定日期查無巡查紀錄";
                }
            }
            // =========================================================
            // 實體儲存與批次寫入控管 
            // =========================================================

            // 1. 先分組
            var readyToSave = processQueue
                .Where(x => x.isProcessing && x.targetID > 0)
                .GroupBy(x => x.targetID)
                .ToList();

            // 批次取得目前 DB 計數 (避免 N+1 查詢)
            List<int> distinctPatrolIDs = readyToSave.Select(g => g.Key).ToList();
            Dictionary<int, int> dbCountMap = system_batch.GetBatchPatrolPhotoCounts(distinctPatrolIDs);

            foreach (var group in readyToSave)
            {
                int pID = group.Key;
                string virtualDir = $"~/_file/patrol/img/{pID}/";
                string physicalDir = Server.MapPath(virtualDir);

                if (!Directory.Exists(physicalDir)) Directory.CreateDirectory(physicalDir);

                // 計算剩餘額度
                int currentDbCount = dbCountMap.ContainsKey(pID) ? dbCountMap[pID] : 0;
                int limit = 5;
                int remaining = limit - currentDbCount;

                // 建立本組的暫存清單 (準備批次寫入)
                List<TempFileData> batchInsertList = new List<TempFileData>();

                // 只做實體存檔 & 收集名單 ---
                foreach (var info in group)
                {
                    if (remaining > 0)
                    {
                        try
                        {
                            // 檔名處理 (Ticks + 原檔名 + 自動防重)
                            string baseFileName = $"{DateTime.Now.Ticks}_{info.uploadedFile.FileName}";
                            string fullPath = Path.Combine(physicalDir, baseFileName);
                            string finalSaveName = baseFileName;

                            int counter = 1;
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
                            string fileExt = Path.GetExtension(baseFileName);

                            while (File.Exists(fullPath))
                            {
                                finalSaveName = $"{fileNameWithoutExt}_({counter}){fileExt}";
                                fullPath = Path.Combine(physicalDir, finalSaveName);
                                counter++;
                            }

                            // 執行實體存檔
                            info.uploadedFile.SaveAs(fullPath);

                            // 加入待寫入清單 
                            batchInsertList.Add(new TempFileData
                            {
                                targetID = pID,
                                originalFileName = info.uploadedFile.FileName,
                                finalFileName = finalSaveName,
                                fullPhysicalPath = fullPath,
                                virtualPath = virtualDir + finalSaveName,
                                infoRef = info
                            });

                            // 扣除額度
                            remaining--;
                        }
                        catch (Exception ex)
                        {
                            // 實體存檔就失敗，直接標記錯誤，不用進 DB
                            info.log.isSuccess = false;
                            info.log.resultMsg = $"失敗：實體存檔錯誤 ({ex.Message})";
                        }
                    }
                    else
                    {
                        info.log.isSuccess = false;
                        info.log.resultMsg = "失敗：超過 5 張限制";
                    }
                }

                // 批次寫入資料庫 & 錯誤回滾 (Transaction) ---
                if (batchInsertList.Count > 0)
                {
                    try
                    {
                        string clientIP = Request?.UserHostAddress ?? "";

                        //批次寫入照片表
                        system_batch.BatchInsertPatrolPhotoRecords(batchInsertList, clientIP, accountID, user?.account, user?.name, user?.unitName);

                        // 寫入成功：更新 Log 狀態
                        foreach (var item in batchInsertList)
                        {
                            item.infoRef.log.isSuccess = true;
                        }
                    }
                    catch (Exception dbEx)
                    {
                        // DB 寫入失敗 -> 啟動回滾機制 (刪除檔案)
                        foreach (var item in batchInsertList)
                        {
                            try
                            {
                                if (File.Exists(item.fullPhysicalPath))
                                {
                                    File.Delete(item.fullPhysicalPath); // 刪除剛剛存的檔案
                                }
                            }
                            catch { /* 刪檔失敗忽略，避免崩潰 */ }

                            // 更新 Log 為失敗
                            item.infoRef.log.isSuccess = false;
                            item.infoRef.log.resultMsg = $"失敗：資料庫寫入錯誤";
                        }
                    }
                }
            }

            // =========================================================
            // 寫入紀錄
            // =========================================================
            int successCount = allLogList.Count(x => x.isSuccess);
            int failCount = allLogList.Count(x => !x.isSuccess);
            int totalCount = allLogList.Count;

            bool isLogSavedToDB = false;
            string dbErrorMsg = "";

            try
            {
                // 取得 Task ID
                int newBatchTaskID = system_batch.CreateBatchTask(
                    enum_treeBatchType.Patrol_Photo,
                    "批次照片上傳",
                    accountID,
                    totalCount,
                    successCount,
                    failCount
                );

                // 回填 TaskID
                foreach (var log in allLogList)
                {
                    log.taskID = newBatchTaskID;
                }

                // 批次寫入 TaskLog
                system_batch.BulkInsertTaskLogs(allLogList);

                //寫入操作紀錄
                UserLog.Insert_UserLog(accountID, UserLog.enum_UserLogItem.巡查紀錄管理, UserLog.enum_UserLogType.上傳, "批次上傳照片");

                // 標記寫入成功
                isLogSavedToDB = true;
            }
            catch (Exception ex)
            {
                isLogSavedToDB = false;
                dbErrorMsg = ex.Message;
            }

            // =========================================================
            // 結果顯示
            // =========================================================

            if (isLogSavedToDB)
            {
                //成功上傳
                ShowMessage("處理完成", $"成功：{successCount}，失敗：{failCount}");
                BindData();
            }
            else
            {

                //db紀錄寫入失敗
                ShowMessage("警告", $"照片上傳作業已執行，但「操作紀錄」寫入資料庫失敗。\n(原因：{dbErrorMsg})\n\n請參考下方列表確認結果。");
                GridView_Detail.DataSource = allLogList;
                GridView_Detail.DataBind();
            }
        }

        protected void GridView_Detail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            // 檢視樹籍資料 (ViewTree)
            if (e.CommandName == "ViewTree")
            {
                string treeNo = e.CommandArgument.ToString();

                List<string> searchList = new List<string> { treeNo };
                Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(searchList, accountID);

                if (treeIdMap.ContainsKey(treeNo))
                {
                    int treeID = treeIdMap[treeNo];

                    // 設定 Session 
                    setTreeID = treeID.ToString();

                    // 開啟新視窗
                    string targetUrl = ResolveUrl("~/Backstage/tree/detail.aspx");
                    string script = $"window.open('{targetUrl}', '_blank');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenTreeWindow", script, true);
                }
                else
                {
                    ShowMessage("提示", $"查無此樹籍資料 ({treeNo})，可能尚未建立或已被刪除。");
                }
            }
            // 檢視健檢紀錄 (ViewPatrol)
            else if (e.CommandName == "ViewPatrol")
            {
                // 解析參數：樹號,日期
                string[] args = e.CommandArgument.ToString().Split(',');
                if (args.Length < 2) return;

                string treeNo = args[0];
                string dateStr = args[1];

                if (!DateTime.TryParse(dateStr, out DateTime checkDate))
                {
                    ShowMessage("提示", "日期格式錯誤");
                    return;
                }

                // 先查 TreeID
                List<string> searchTreeList = new List<string> { treeNo };
                Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(searchTreeList, accountID);

                if (treeIdMap.ContainsKey(treeNo))
                {
                    int treeID = treeIdMap[treeNo];

                    // 組裝查詢 Key (TreeID + Date)
                    var queryKeys = new List<TreeQueryKey>
                    {
                        new TreeQueryKey { treeID = treeID, checkDate = checkDate }
                    };

                    // 反查流水號
                    Dictionary<string, int> patrolMap = system_batch.GetPatrolIDMap(queryKeys);

                    // Key 的格式是 $"{treeID}_{yyyyMMdd}"
                    string mapKey = $"{treeID}_{checkDate:yyyyMMdd}";

                    if (patrolMap.ContainsKey(mapKey))
                    {
                        int healthID = patrolMap[mapKey];

                        // 設定 Session 並跳轉
                        setPatrolID = healthID.ToString(); // 設定巡查 ID
                        setTreeID = null;                  // 清空樹木 ID 

                        // 開啟新視窗到編輯頁
                        string targetUrl = ResolveUrl("edit.aspx");
                        string script = $"window.open('{targetUrl}', '_blank');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenHealthWindow", script, true);
                    }
                    else
                    {
                        ShowMessage("提示", $"查無此日期的健檢紀錄 ({dateStr})。");
                    }
                }
                else
                {
                    ShowMessage("提示", $"查無此樹籍資料 ({treeNo})，無法查詢健檢紀錄。");
                }
            }
        }
    }
}