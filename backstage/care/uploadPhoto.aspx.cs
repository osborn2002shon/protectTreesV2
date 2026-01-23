using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static protectTreesV2.Batch.Batch;

namespace protectTreesV2.backstage.care
{
    public partial class uploadPhoto : protectTreesV2.Base.BasePage
    {
        protectTreesV2.Batch.Batch system_batch = new Batch.Batch();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
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
            var historyList = system_batch.GetBatchTaskList(enum_treeBatchType.Care_Photo, userId);
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

            // 用來暫存解析結果：Key = 完整檔名
            Dictionary<string, LocalPhotoMeta> metaMap = new Dictionary<string, LocalPhotoMeta>();

            // 一致性檢查表
            // Key = "樹籍_日期_組別" (例如: A001_20250101_01)
            // Value = "第一次讀到的項目名稱" (例如: 修剪)
            Dictionary<string, string> consistencyCheckMap = new Dictionary<string, string>();

            //新增的草稿ID
            HashSet<int> newlyCreatedIDs = new HashSet<int>();
            List<int> idsToRollback = new List<int>();

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
                if (parts.Length < 5)
                {
                    info.isProcessing = false;
                    myLog.resultMsg = "失敗：檔名格式錯誤";
                    continue;
                }

                // 檢查重複檔名 (同一批次不允許同名檔案)
                if (metaMap.ContainsKey(f.FileName))
                {
                    info.isProcessing = false;
                    myLog.resultMsg = "失敗：重複的檔案名稱";
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

                // 解析組別
                string pGroup = parts[2].Trim();

                // 解析項目名稱
                string pItemName = parts[3].Trim();
                if (string.IsNullOrEmpty(pGroup) || string.IsNullOrEmpty(pItemName))
                {
                    info.isProcessing = false; 
                    myLog.resultMsg = "失敗：組別或名稱無法辨識"; 
                    continue;
                }

                // 解析類型
                string pTypeStr = parts[4].Trim();
                if (pTypeStr != "1" && pTypeStr != "2")
                {
                    info.isProcessing = false;
                    myLog.resultMsg = "失敗：類型錯誤"; 
                    continue;
                }

                //檢查組別名稱是否不一致
                string uniqueGroupKey = $"{tNo}_{dStr}_{pGroup}";
                bool isConflict = false;

                if (consistencyCheckMap.ContainsKey(uniqueGroupKey))
                {
                    string existingName = consistencyCheckMap[uniqueGroupKey];
                    if (existingName != pItemName)
                    {
                        isConflict = true;
                        info.isProcessing = false;
                        myLog.resultMsg = $"失敗：同組別項目名稱衝突 (應為 '{existingName}')";

                        // 回頭砍掉「之前」加入同組別的檔案
                        var previousFiles = processQueue
                            .Where(x => x.isProcessing
                                     && metaMap.ContainsKey(x.uploadedFile.FileName)
                                     && metaMap[x.uploadedFile.FileName].groupCode == pGroup
                                     && x.systemTreeNo == tNo
                                     && $"{x.checkDate:yyyyMMdd}" == dStr)
                            .ToList();

                        foreach (var prev in previousFiles)
                        {
                            prev.isProcessing = false;
                            prev.log.isSuccess = false;
                            prev.log.resultMsg = $"失敗：同組別項目名稱衝突 (與檔案 {f.FileName} 名稱不符)";
                        }
                    }
                }
                else
                {
                    consistencyCheckMap.Add(uniqueGroupKey, pItemName);
                }

                // 如果發生衝突，當下的檔案就不加入 Queue 了
                if (isConflict)
                {
                    continue;
                }

                // 存入 Map
                metaMap[f.FileName] = new LocalPhotoMeta
                {
                    groupCode = pGroup,
                    itemName = pItemName,
                    type = int.Parse(pTypeStr)
                };

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

            // 批次取得養護流水號
            var activeItems = processQueue.Where(x => x.isProcessing).ToList();
            if (activeItems.Count > 0)
            {
                var queryKeys = activeItems
                    .Select(x => new TreeQueryKey { treeID = x.treeID, checkDate = x.checkDate })
                    .Distinct().ToList();

                Dictionary<string, int> carehMap = system_batch.GetCareIDMap(queryKeys);

                foreach (var info in activeItems)
                {
                    string key = $"{info.treeID}_{info.checkDate:yyyyMMdd}";
                    if (carehMap.ContainsKey(key))
                    {
                        info.targetID = carehMap[key];
                    }
                }
            }

            // 自動新增處理
            if (CheckBox_autoCreateDraft.Checked)
            {
                // 找出還沒有養護資料的項目
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
                        Dictionary<string, int> newPatrolMap = system_batch.BatchCreateCareRecords(createKeys, accountID);

                        foreach (var info in toCreateList)
                        {
                            // Key 組成：treeID_yyyyMMdd
                            string key = $"{info.treeID}_{info.checkDate:yyyyMMdd}";

                            if (newPatrolMap.ContainsKey(key))
                            {
                                // 成功新增
                                info.targetID = newPatrolMap[key];
                                info.log.resultMsg = "提醒：指定日期已自動新增養護紀錄草稿";

                                //加入白名單
                                newlyCreatedIDs.Add(info.targetID);
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
            var readyGroups = processQueue
            .Where(x => x.isProcessing && x.targetID > 0)
            .GroupBy(x => x.targetID)
            .ToList();

            List<int> distinctCareIDs = readyGroups.Select(g => g.Key).ToList();

            // 撈取照片組數
            Dictionary<int, int> dbCountMap = system_batch.GetBatchCareCounts(distinctCareIDs);

            foreach (var group in readyGroups)
            {
                int cID = group.Key;
                string virtualDir = $"~/_file/care/img/{cID}/";
                string physicalDir = Server.MapPath(virtualDir);
                if (!Directory.Exists(physicalDir)) Directory.CreateDirectory(physicalDir);

                // --- 計算額度 ---
                int currentDbCount = dbCountMap.ContainsKey(cID) ? dbCountMap[cID] : 0;
                int limit = 5;
                int remainingQuota = limit - currentDbCount;

                // 將本次上傳的檔案，依照 GroupCode 再次分組
                // 因為同一個 GroupCode (例如 01) 的 1.jpg 和 2.jpg 算作「同一組新資料」
                var subGroups = group
                    .GroupBy(f => metaMap[f.uploadedFile.FileName].groupCode)
                    .ToList();

                List<TempFileData> batchInsertList = new List<TempFileData>();

                foreach (var subGroup in subGroups)
                {
                    // 檢查這一組能不能塞進去
                    if (remainingQuota > 0)
                    {
                        // 扣除額度
                        remainingQuota--;

                        foreach (var info in subGroup) // 處理這一組的 Before/After 檔案
                        {
                            try
                            {
                                // --- 流水號存檔 ---
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

                                info.uploadedFile.SaveAs(fullPath);

                                batchInsertList.Add(new TempFileData
                                {
                                    targetID = cID,
                                    originalFileName = info.uploadedFile.FileName,
                                    finalFileName = finalSaveName,
                                    fullPhysicalPath = fullPath,
                                    virtualPath = virtualDir + finalSaveName,
                                    infoRef = info
                                });
                            }
                            catch (Exception ex)
                            {
                                info.log.isSuccess = false; 
                                info.log.resultMsg = $"失敗：實體存檔錯誤";
                            }
                        }
                    }
                    else
                    {
                        // 超過額度
                        foreach (var info in subGroup)
                        {
                            info.log.isSuccess = false;
                            info.log.resultMsg = "失敗：組數已達上限";
                        }
                    }
                }

                // --- 批次寫入資料庫 ---
                if (batchInsertList.Count > 0)
                {
                    try
                    {
                        string clientIP = Request?.UserHostAddress ?? "";

                        // 批次新增
                        system_batch.BatchInsertCarePhotoRecords(
                            batchInsertList, metaMap, clientIP, accountID, user?.account, user?.name, user?.unitName
                        );

                        foreach (var item in batchInsertList) item.infoRef.log.isSuccess = true;
                    }
                    catch (Exception)
                    {
                        // 回滾刪檔
                        foreach (var item in batchInsertList)
                        {
                            try { 
                                if (File.Exists(item.fullPhysicalPath))
                                    File.Delete(item.fullPhysicalPath); 
                            } 
                            catch { }
                            item.infoRef.log.isSuccess = false; 
                            item.infoRef.log.resultMsg = "失敗：資料庫寫入錯誤";
                        }

                        // 如果是剛新增的錯誤要刪除
                        if (newlyCreatedIDs.Contains(cID))
                        {
                            idsToRollback.Add(cID);
                        }
                    }
                }
            }

            // 執行批次清理錯誤的草稿
            if (idsToRollback.Count > 0)
            {
                try { system_batch.BatchDeleteCareRecords(idsToRollback); }
                catch { /* 清理失敗不影響主要結果，僅忽略 */ }
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
            // 檢視健檢紀錄 (ViewCare)
            else if (e.CommandName == "ViewCare")
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
                    Dictionary<string, int> carehMap = system_batch.GetCareIDMap(queryKeys);

                    // Key 的格式是 $"{treeID}_{yyyyMMdd}"
                    string mapKey = $"{treeID}_{checkDate:yyyyMMdd}";

                    if (carehMap.ContainsKey(mapKey))
                    {
                        int careID = carehMap[mapKey];

                        // 設定 Session 並跳轉
                        setCareID = careID.ToString();  // 設定養護 ID
                        setTreeID = null;               // 清空樹木 ID 

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