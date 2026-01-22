using NPOI.SS.UserModel;
using protectTreesV2.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using static protectTreesV2.Batch.Batch;
using static protectTreesV2.Care.Care;

namespace protectTreesV2.backstage.care
{
    public partial class upload : BasePage
    {
        protectTreesV2.Batch.Batch system_batch = new Batch.Batch();
        public class ImportTreeDataModel
        {
            public TreeBatchTaskLog log { get; set; }
            public Care.Care.CareRecord record { get; set; }
            public string systemTreeNo { get; set; }
        }

        public class SideRowInfo
        {
            public IRow row { get; set; }
            public int rowIndex { get; set; } // 原始 Excel 列號 (0-based)
            public string linkKey { get; set; } // 調查記數
        }

        public class BasicInfoParseResult
        {
            public List<string> errors { get; set; } = new List<string>();

            public Care.Care.CareRecord parsedData { get; set; } = new Care.Care.CareRecord();

            public DateTime? careDate { get; set; }
        }

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
            var historyList = system_batch.GetBatchTaskList(enum_treeBatchType.Care_Record, userId);
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
                Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(searchTreeList,accountID);

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
        protected void Button_StartUpload_Click(object sender, EventArgs e)
        {
            // ==========================================
            // 1. 基礎檢查 (檔案存在、格式、大小)
            // ==========================================
            var user = UserInfo.GetCurrentUser;
            int accountID = user?.accountID ?? 0;

            if (!FileUpload_Batch.HasFile)
            {
                ShowMessage("提示", "請先選取檔案！");
                return;
            }

            string fileName = FileUpload_Batch.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();

            if (fileExt != ".xls" && fileExt != ".xlsx")
            {
                ShowMessage("提示", "檔案格式錯誤！僅接受 .xls 或 .xlsx 格式。");
                return;
            }

            // 30MB 限制
            int maxFileSize = 30 * 1024 * 1024;
            if (FileUpload_Batch.PostedFile.ContentLength > maxFileSize)
            {
                ShowMessage("提示", "檔案大小超過 30MB 限制！");
                return;
            }

            // ==========================================
            // 2. 檔案儲存 (保留檔案，不刪除)
            // ==========================================
            string saveFullPath = "";
            try
            {
                string yyyyMM = DateTime.Now.ToString("yyyyMM");
                string relativeFolder = $"~/_file/care/upload/{accountID}/{yyyyMM}/";
                string physicalFolder = Server.MapPath(relativeFolder);

                if (!Directory.Exists(physicalFolder))
                {
                    Directory.CreateDirectory(physicalFolder);
                }

                string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
                string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string baseFileName = $"{timeStamp}_{fileNameNoExt}";
                string finalFileName = baseFileName + fileExt;

                saveFullPath = Path.Combine(physicalFolder, finalFileName);
                int counter = 1;
                while (File.Exists(saveFullPath))
                {
                    finalFileName = $"{baseFileName}({counter}){fileExt}";
                    saveFullPath = Path.Combine(physicalFolder, finalFileName);
                    counter++;
                }

                FileUpload_Batch.SaveAs(saveFullPath);
            }
            catch (Exception ex)
            {
                ShowMessage("提示", $"檔案上傳失敗：{ex.Message}");
                return;
            }

            var sheetRules = new Dictionary<string, (int TargetRow, string[] Cols)>
{
            // Sheet 1: 基本資料 (單層標題，檢查第 0 列)
            { "基本資料", (0, new[] {
                "調查記數", "樹籍編號", "機關樹木編號", "座落地點縣市", "座落地點鄉鎮",
                "樹種", "養護日期", "記錄人員", "覆核人員"
            })},

            // Sheet 2: 生長情形概況 (雙層標題，檢查第 1 列的子項目)
            { "生長情形概況", (1, new[] {

                "調查記數", "樹籍編號", "樹種",

                // 樹冠枝葉
                "枝葉茂密無枯枝", "季節性休眠落葉", "有枯枝", "有明顯病蟲害", "樹冠接觸電線或異物", "其他",

                // 主莖幹
                "完好健康無異狀", "樹皮破損", "莖幹損傷", "有白蟻蟻道", "主莖傾斜搖晃",
                "莖基部有真菌子實體", "有流膠或潰瘍", "有纏勒植物", "其他",

                // 根部及地際部
                "根部完好無異狀", "根部損傷", "根部有腐朽或可見子實體", "盤根或浮根",
                "根部潰爛", "大量萌櫱(不定芽)", "其他",

                // 生育地環境
                "良好無異狀", "樹穴過小", "遭鋪面封固", "有石塊或廢棄物堆積",
                "土壤壓實", "環境積水", "緊鄰設施或建物", "其他",

                 // 樹冠或主莖幹與鄰接物
                "無鄰接物", "接觸建築物", "接觸電線或管線", "遮蔽路燈或號誌","其他"
            })},

            // Sheet 3: 養護作業管理 (單層標題，檢查第 0 列)
            { "養護作業管理", (0, new[] {
                "調查記數", "樹籍編號", "樹種",
                "危險枯枝清除", "植栽基盤維護暨環境整理", "樹木健康管理",
                "營養評估追肥", "安全衛生防護"
            })}
        };

            // ==========================================
            // 開始解析與處理
            // ==========================================
            // 所有的 Log 
            List<TreeBatchTaskLog> allLogList = new List<TreeBatchTaskLog>();
            List<ImportTreeDataModel> validDataList = new List<ImportTreeDataModel>();

            // 業務邏輯旗標
            bool isOverwrite = CheckBox_Overwrite.Checked; 

            // 重複檢查 
            HashSet<string> internalDuplicateCheck = new HashSet<string>();

            try
            {
                IWorkbook workbook = null;
                using (FileStream fs = new FileStream(saveFullPath, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(fs);

                    // ---------------------------------------------------------
                    // 4.1 格式檢查 (Format Validation)
                    // ---------------------------------------------------------
                    bool isFormatValid = true;
                    foreach (var rule in sheetRules)
                    {
                        string sheetName = rule.Key;
                        int targetRowIdx = rule.Value.TargetRow;
                        string[] expectedCols = rule.Value.Cols;

                        ISheet sheet = workbook.GetSheet(sheetName);
                        if (sheet == null) { isFormatValid = false; break; }

                        IRow targetRow = sheet.GetRow(targetRowIdx);
                        if (targetRow == null) { isFormatValid = false; break; }

                        // 檢查說明列 (簡單檢查是否為正確的範本)
                        IRow descRow = sheet.GetRow(targetRowIdx + 1);
                        if (descRow == null) { isFormatValid = false; break; }
                        ICell descCell = descRow.GetCell(0);
                        string descText = (descCell == null) ? "" : descCell.ToString();
                        if (!descText.Contains("填寫說明"))
                        {
                            isFormatValid = false; break;
                        }

                        // 檢查欄位名稱
                        for (int i = 0; i < expectedCols.Length; i++)
                        {
                            string actualValue = "";
                            // 往上查找 Loop (處理合併儲存格)
                            for (int r = targetRowIdx; r >= 0; r--)
                            {
                                IRow currentRow = sheet.GetRow(r);
                                if (currentRow != null)
                                {
                                    ICell cell = currentRow.GetCell(i);
                                    string val = (cell == null) ? "" : cell.ToString().Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
                                    if (!string.IsNullOrEmpty(val))
                                    {
                                        actualValue = val;
                                        break;
                                    }
                                }
                            }
                            string expectedRaw = expectedCols[i].Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
                            if (!actualValue.Equals(expectedRaw, StringComparison.OrdinalIgnoreCase))
                            {
                                isFormatValid = false;
                                break;
                            }
                        }
                        if (!isFormatValid) break;
                    }

                    if (!isFormatValid)
                    {
                        // 格式錯誤：不刪檔，直接回傳
                        ShowMessage("提示", "EXCEL欄位格式錯誤，請檢查EXCEL欄位是否與範本相符");
                        return;
                    }

                    // ---------------------------------------------------------
                    // 讀取資料 
                    // ---------------------------------------------------------
                    var growthMap = ScanSideSheet(workbook, "生長情形概況", 3, 0, allLogList);
                    var maintenanceMap = ScanSideSheet(workbook, "養護作業管理", 2, 0, allLogList);

                    ISheet mainSheet = workbook.GetSheet("基本資料");

                    for (int r = 2; r <= mainSheet.LastRowNum; r++)
                    {
                        IRow row = mainSheet.GetRow(r);
                        if (row == null) continue;

                        string linkKey = GetCellValue(row, 0);
                        string treeNo = GetCellValue(row, 1);
                        string valDateStr = GetCellValue(row, 6);

                        // 空行判斷
                        bool hasData = false;
                        for (int c = 0; c < row.LastCellNum; c++)
                        {
                            if (!string.IsNullOrEmpty(GetCellValue(row, c))) { hasData = true; break; }
                        }

                        if (string.IsNullOrEmpty(linkKey) || string.IsNullOrEmpty(treeNo) || string.IsNullOrEmpty(valDateStr))
                        {
                            if (hasData)
                            {
                                allLogList.Add(new TreeBatchTaskLog
                                {
                                    taskID = 0,
                                    sourceItem = $"基本資料-第{r + 1}列",
                                    refKey = treeNo,
                                    isSuccess = false,
                                    resultMsg = "失敗：必要欄位未填寫"
                                });
                            }
                            continue;
                        }

                        DateTime validSurveyDate;
                        if (!DateTime.TryParse(valDateStr, out validSurveyDate))
                        {
                            allLogList.Add(new TreeBatchTaskLog
                            {
                                taskID = 0,
                                sourceItem = $"基本資料-第{r + 1}列",
                                refKey = treeNo,
                                isSuccess = false,
                                resultMsg = $"失敗：養護日期格式錯誤"
                            });
                            continue;
                        }

                        // 民國年自動校正 
                        if (validSurveyDate.Year < 1911)
                        {
                            validSurveyDate = validSurveyDate.AddYears(1911);
                        }

                        TreeBatchTaskLog myLog = new TreeBatchTaskLog
                        {
                            taskID = 0,
                            sourceItem = $"基本資料-第{r + 1}列",
                            refKey = treeNo,
                            isSuccess = false,
                            resultMsg = ""
                        };
                        allLogList.Add(myLog);

                        List<string> rowErrors = new List<string>();
                        BasicInfoParseResult basicResult = ValidateBasicInfo(row, isOverwrite, validSurveyDate);

                        if (basicResult.errors.Count > 0) rowErrors.AddRange(basicResult.errors);
                        if (basicResult.careDate.HasValue) myLog.refDate = basicResult.careDate.Value;

                        Care.Care.CareRecord draftRecord = basicResult.parsedData;

                        // 領取附屬資料
                        IRow growthRow = GetAndRemove(growthMap, linkKey);
                        IRow maintenanceRow = GetAndRemove(maintenanceMap, linkKey);

                        // --- 附屬頁籤檢查 ---
                        // 生長情形概況 
                        if (growthRow != null) 
                        {
                            // 檢查樹號是否一致
                            CheckTreeNoConsistency(treeNo, growthRow, "生長情形概況", rowErrors);

                            // 解析並驗證
                            var errs = ParseAndValidateGrowthCondition(growthRow, draftRecord, isOverwrite);
                            if (errs.Count > 0) rowErrors.AddRange(errs);
                        }
                        else
                        {
                            rowErrors.Add("缺漏資料：[生長情形概況] 頁籤中找不到此調查記數的資料。");
                        }

                        // 養護作業管理 
                        if (maintenanceRow != null) 
                        {
                            // 檢查樹號是否一致
                            CheckTreeNoConsistency(treeNo, maintenanceRow, "養護作業管理", rowErrors);

                            // 解析並驗證 
                            var errs = ParseAndValidateMaintenance(maintenanceRow, draftRecord, isOverwrite);
                            if (errs.Count > 0) rowErrors.AddRange(errs);
                        }
                        else
                        {
                            rowErrors.Add("缺漏資料：[養護作業管理] 頁籤中找不到此調查記數的資料 (若無作業請保留樹號列，內容留空)。");
                        }


                        // --- 重複資料檢查 (同檔案內) ---
                        if (rowErrors.Count == 0)
                        {
                            string uniqueKey = $"{treeNo}_{myLog.refDate:yyyyMMdd}";
                            if (internalDuplicateCheck.Contains(uniqueKey))
                            {
                                rowErrors.Add("Excel內包含重複的[樹號+日期]資料");
                            }
                            else
                            {
                                internalDuplicateCheck.Add(uniqueKey);
                            }
                        }

                        // --- 總結單列結果 ---
                        if (rowErrors.Count > 0)
                        {
                            //myLog.resultMsg = "失敗：資料提供不全 (" + string.Join("、", rowErrors) + ")";
                            myLog.resultMsg = "失敗：資料提供不全 ";
                            myLog.isSuccess = false;
                        }
                        else
                        {
                            myLog.isSuccess = true;
                            myLog.resultMsg = "";
                            //draftRecord.dataStatus =  0;

                            validDataList.Add(new ImportTreeDataModel
                            {
                                log = myLog,
                                record = draftRecord,
                                systemTreeNo = treeNo
                            }); ;
                        }
                    } // End Loop

                    // 處理孤兒資料
                    ProcessOrphans(growthMap, "生長情形概況", allLogList);
                    ProcessOrphans(maintenanceMap, "養護作業管理", allLogList);

                } 

                // ==========================================
                // 資料庫比對與寫入
                // ==========================================

                // excel沒有資料
                if (allLogList.Count == 0)
                {
                    ShowMessage("提示", "檔案中沒有讀取到任何資料列。");
                    return;
                }

                // 資料庫比對 
                if (validDataList.Count > 0)
                {
                    List<string> distinctTreeNos = validDataList.Select(x => x.systemTreeNo).Distinct().ToList();
                    Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(distinctTreeNos, accountID);

                    for (int i = validDataList.Count - 1; i >= 0; i--)
                    {
                        var item = validDataList[i];
                        string tNo = item.systemTreeNo;

                        if (treeIdMap.ContainsKey(tNo))
                        {
                            item.record.treeID = treeIdMap[tNo];
                        }
                        else
                        {
                            item.log.isSuccess = false;
                            item.log.resultMsg = $"失敗：查無系統樹籍編號";
                            validDataList.RemoveAt(i);
                        }
                    }

                    if (validDataList.Count > 0)
                    {
                        var queryKeys = validDataList.Select(x => new TreeQueryKey
                        {
                            treeID = x.record.treeID,
                            checkDate = x.record.careDate.Value
                        }).Distinct().ToList();

                        Dictionary<string, int> careMap = system_batch.GetCareIDMap(queryKeys);

                        for (int i = validDataList.Count - 1; i >= 0; i--)
                        {
                            var item = validDataList[i];
                            string key = $"{item.record.treeID}_{item.record.careDate:yyyyMMdd}";

                            if (careMap.ContainsKey(key))
                            {
                                int oldCareID = careMap[key];
                                if (isOverwrite)
                                {
                                    item.record.careID = oldCareID;
                                    item.log.resultMsg += "提醒：已覆蓋同日養護資料";
                                }
                                else
                                {
                                    item.log.isSuccess = false;
                                    item.log.resultMsg = "失敗：同日已有紀錄但未覆蓋";
                                    validDataList.RemoveAt(i);
                                }
                            }
                        }
                    }
                }

                var updateModels = validDataList.Where(x => x.record.careID > 0).ToList();
                var insertModels = validDataList.Where(x => x.record.careID == 0).ToList();

                // -----------------------------------------------------
                // 執行更新 
                // -----------------------------------------------------
                string clientIP = Request?.UserHostAddress ?? "";
                if (updateModels.Count > 0)
                {
                    try
                    {
                        var recordsToUpdate = updateModels.Select(x => x.record).ToList();
                        foreach (var rec in recordsToUpdate)
                        {
                            rec.updateAccountID = accountID;
                            rec.updateDateTime = DateTime.Now;
                        }

                        // 執行 SQL
                        system_batch.BulkUpdateCareRecords(recordsToUpdate, clientIP, accountID, user?.account, user?.name, user?.unitName);

                        // 成功：保持原本 Log 的 isSuccess = true
                    }
                    catch (Exception ex)
                    {
                        // 失敗：把這批「原本以為會成功」的 Log 全部改成失敗
                        foreach (var item in updateModels)
                        {
                            item.log.isSuccess = false;
                            item.log.resultMsg = $"失敗：資料庫寫入錯誤";
                        }
                    }
                }

                // -----------------------------------------------------
                //  執行新增 
                // -----------------------------------------------------
                if (insertModels.Count > 0)
                {
                    try
                    {
                        var recordsToInsert = insertModels.Select(x => x.record).ToList();
                        foreach (var rec in recordsToInsert)
                        {
                            rec.insertAccountID = accountID;
                            rec.insertDateTime = DateTime.Now;
                        }
                        system_batch.BulkInsertCareRecords(recordsToInsert, clientIP, accountID, user?.account, user?.name, user?.unitName);

                        // 成功：保持原本 Log 的 isSuccess = true
                    }
                    catch (Exception ex)
                    {
                        // 失敗：把這批 Log 全部改成失敗
                        foreach (var item in insertModels)
                        {
                            item.log.isSuccess = false;
                            item.log.resultMsg = $"失敗：資料庫寫入錯誤";
                        }
                    }
                }

                int finalSuccess = allLogList.Count(x => x.isSuccess);
                int finalFail = allLogList.Count(x => !x.isSuccess);
                int finalTotal = allLogList.Count;

                bool logSaved = false;
                string logError = "";

                try
                {
                    // 建立 Task
                    int newBatchTaskID = system_batch.CreateBatchTask(
                        enum_treeBatchType.Care_Record,
                        fileName,
                        accountID,
                        finalTotal,
                        finalSuccess,
                        finalFail
                    );

                    // 回填 TaskID
                    foreach (var log in allLogList)
                    {
                        log.taskID = newBatchTaskID;
                    }

                    // 寫入 DB
                    system_batch.BulkInsertTaskLogs(allLogList);

                    //寫入操作紀錄
                    UserLog.Insert_UserLog(accountID, UserLog.enum_UserLogItem.養護紀錄管理, UserLog.enum_UserLogType.上傳, "批次上傳養護紀錄");

                    logSaved = true;
                }
                catch (Exception ex)
                {
                    logSaved = false;
                    logError = ex.Message;
                }

                // =====================================================
                // Step E: 顯示結果
                // =====================================================

                if (logSaved)
                {
                    ShowMessage("處理完成", $"成功：{finalSuccess}，失敗：{finalFail}");
                    // 如果 Log 存成功，可以直接 Bind DB
                    BindData();
                }
                else
                {
                    // 如果 Log 存失敗 ，顯示記憶體中的結果
                    ShowMessage("警告", $"資料處理結束，但紀錄寫入失敗 ({logError})。\n請參考下方列表。");
                    GridView_Detail.DataSource = allLogList;
                    GridView_Detail.DataBind();
                }

            }
            catch (Exception ex)
            {
                ShowMessage("提示", $"系統發生錯誤：{ex.Message}");
            }
        }

        /// <summary>
        /// 掃描附屬頁籤並建立索引
        /// </summary>
        private Dictionary<string, SideRowInfo> ScanSideSheet(IWorkbook wb, string sheetName, int startRow, int keyCol, List<TreeBatchTaskLog> logList)
        {
            var map = new Dictionary<string, SideRowInfo>();
            ISheet sheet = wb.GetSheet(sheetName);
            if (sheet == null) return map;

            for (int r = startRow; r <= sheet.LastRowNum; r++)
            {
                IRow row = sheet.GetRow(r);
                if (row == null) continue;

                string linkKey = GetCellValue(row, keyCol);
                string treeNo = GetCellValue(row, 1);

                bool hasOtherData = false;

                // row.LastCellNum 是這一列最後一個 Cell 的編號 (+1)
                // 從頭掃到尾，看有沒有任何一格有字
                for (int c = 0; c < row.LastCellNum; c++)
                {
                    if (c == keyCol) continue; // 跳過 Key 欄位本身不檢查

                    string cellVal = GetCellValue(row, c);
                    if (!string.IsNullOrEmpty(cellVal))
                    {
                        hasOtherData = true;
                        break; // 只要找到一格有字，就確定這行有資料
                    }
                }

                if (string.IsNullOrEmpty(linkKey))
                {
                    if (hasOtherData)
                    {
                        logList.Add(new TreeBatchTaskLog
                        {
                            taskID = 0,
                            sourceItem = $"{sheetName}-第{r + 1}列",
                            refKey = "",
                            isSuccess = false,
                            resultMsg = "失敗：無法對應資料"
                        });
                    }
                    continue;
                }

                // 5. 合法資料 -> 加入字典
                // 防止 Excel 內部 Key 重複 (例如兩行都是 "1")，只取第一筆，或您可以選擇這裡也報錯
                if (map.ContainsKey(linkKey))
                {
                    // 抓出「上一筆」佔據這個 Key 的是哪一列，方便使用者比對
                    int prevRowIndex = map[linkKey].rowIndex + 1;

                    // 記錄失敗 Log
                    logList.Add(new TreeBatchTaskLog
                    {
                        taskID = 0,
                        sourceItem = $"{sheetName}-第{r + 1}列,(記數:{linkKey})", // 這一列 (後到的)
                        refKey = treeNo,
                        isSuccess = false,

                        resultMsg = $"失敗：調查記數重複 (此記數已存在於第 {prevRowIndex} 列)"
                    });

                    // 既然重複了，這筆資料就不加入 Dictionary
                }
                else
                {
                    // 合法且唯一
                    map.Add(linkKey, new SideRowInfo
                    {
                        row = row,
                        rowIndex = r,
                        linkKey = linkKey
                    });
                }
            }
            return map;
        }

        /// <summary>
        /// 驗證基本資料並回傳解析後的物件
        /// </summary>
        /// <param name="isStrictMode">是否為嚴格模式</param>
        /// <param name="confirmedDate">外層已解析成功的養護日期</param>
        private BasicInfoParseResult ValidateBasicInfo(IRow row, bool isStrictMode, DateTime confirmedDate)
        {
            var result = new BasicInfoParseResult();
            var errors = result.errors;
            var data = result.parsedData; 

            // ---------------------------------------------------------
            // 養護日期 (Col 6) - 必填 (由外層傳入)
            // ---------------------------------------------------------
            data.careDate = confirmedDate;
            result.careDate = confirmedDate; 

            // ---------------------------------------------------------
            // 記錄人員 (Col 7) - 嚴格模式必填，限 100 字
            // ---------------------------------------------------------
            string valRecorder = GetCellValue(row, 7);

            if (string.IsNullOrEmpty(valRecorder))
            {
                // 只有在嚴格模式 (覆蓋) 下，才視為錯誤
                if (isStrictMode)
                {
                    errors.Add("記錄人員 (必填)");
                }
            }
            else
            {
                // 有填寫就要檢查長度
                if (valRecorder.Length > 100)
                {
                    errors.Add($"記錄人員 (長度限制 100 字)");
                }
                else
                {
                    data.recorder = valRecorder;
                }
            }

            // ---------------------------------------------------------
            // 覆核人員 (Col 8) - 選填，但若有填需限 100 字
            // ---------------------------------------------------------
            string valReviewer = GetCellValue(row, 8);

            if (!string.IsNullOrEmpty(valReviewer))
            {
                // 只要有填寫，不管是不是嚴格模式，都要檢查長度格式
                if (valReviewer.Length > 100)
                {
                    errors.Add($"覆核人員 (長度限制 100 字，目前: {valReviewer.Length})");
                }
                else
                {
                    data.reviewer = valReviewer;
                }
            }

            return result;
        }

        /// <summary>
        /// 解析並驗證「生長情形概況」 (呼叫外部 Helper 版)
        /// </summary>
        private List<string> ParseAndValidateGrowthCondition(IRow row, CareRecord data, bool isStrictMode)
        {
            List<string> errors = new List<string>();

            // =================================================================
            // 1. 樹冠枝葉
            // =================================================================
            ProcessGroup(row, errors, isStrictMode, "樹冠枝葉", 3, 4, 8, new[] { 5, 8 },
            (col, val) =>
            {
                if (col == 4) data.crownSeasonalDormant = true;

                if (col == 5) // 枯枝百分比
                {
                    string numStr = val.Replace("%", "").Trim();
                    if (decimal.TryParse(numStr, out decimal d))
                    {
                        if (d < 0 || d > 100) errors.Add("樹冠枝葉-有枯枝 (%)：數值須介於 0 ~ 100");
                        else
                        {
                            data.crownDeadBranch = true;
                            data.crownDeadBranchPercent = Math.Round(d, 2);
                        }
                    }
                    else errors.Add("樹冠枝葉-有枯枝：格式錯誤，請輸入數字");
                }

                if (col == 6) data.crownPest = true;
                if (col == 7) data.crownForeignObject = true;

                if (col == 8)
                {
                    if (CheckTextLength(errors, val, 200, "樹冠枝葉-其他"))
                        data.crownOtherNote = val;
                }
            },
            status => data.crownStatus = status);


            // =================================================================
            // 2. 主莖幹
            // =================================================================
            ProcessGroup(row, errors, isStrictMode, "主莖幹", 9, 10, 17, new[] { 17 },
            (col, val) =>
            {
                if (col == 10) data.trunkBarkDamage = true;
                if (col == 11) data.trunkDecay = true;
                if (col == 12) data.trunkTermiteTrail = true;
                if (col == 13) data.trunkLean = true;
                if (col == 14) data.trunkFungus = true;
                if (col == 15) data.trunkGummosis = true;
                if (col == 16) data.trunkVine = true;

                if (col == 17)
                {
                    if (CheckTextLength(errors, val, 200, "主莖幹-其他"))
                        data.trunkOtherNote = val;
                }
            },
            status => data.trunkStatus = status);


            // =================================================================
            // 3. 根部及地際部
            // =================================================================
            ProcessGroup(row, errors, isStrictMode, "根部及地際部", 18, 19, 24, new[] { 24 },
            (col, val) =>
            {
                if (col == 19) data.rootDamage = true;
                if (col == 20) data.rootDecay = true;
                if (col == 21) data.rootExpose = true;
                if (col == 22) data.rootRot = true;
                if (col == 23) data.rootSucker = true;

                if (col == 24)
                {
                    if (CheckTextLength(errors, val, 200, "根部-其他"))
                        data.rootOtherNote = val;
                }
            },
            status => data.rootStatus = status);


            // =================================================================
            // 4. 生育地環境
            // =================================================================
            ProcessGroup(row, errors, isStrictMode, "生育地環境", 25, 26, 32, new[] { 32 },
            (col, val) =>
            {
                if (col == 26) data.envPitSmall = true;
                if (col == 27) data.envPaved = true;
                if (col == 28) data.envDebris = true;
                if (col == 29) data.envCompaction = true;
                if (col == 30) data.envWaterlog = true;
                if (col == 31) data.envNearFacility = true;

                if (col == 32)
                {
                    if (CheckTextLength(errors, val, 200, "生育地-其他"))
                        data.envOtherNote = val;
                }
            },
            status => data.envStatus = status);


            // =================================================================
            // 5. 鄰接物
            // =================================================================
            ProcessGroup(row, errors, isStrictMode, "鄰接物", 33, 34, 37, new[] { 37 },
            (col, val) =>
            {
                if (col == 34) data.adjacentBuilding = true;
                if (col == 35) data.adjacentWire = true;
                if (col == 36) data.adjacentSignal = true;

                if (col == 37)
                {
                    if (CheckTextLength(errors, val, 200, "鄰接物-其他"))
                        data.adjacentOtherNote = val;
                }
            },
            status => data.adjacentStatus = status);

            return errors;
        }
        /// <summary>
        /// 解析並驗證「養護作業管理」 (呼叫外部 Helper 版)
        /// </summary>
        private List<string> ParseAndValidateMaintenance(IRow row, CareRecord data, bool isStrictMode)
        {
            List<string> errors = new List<string>();

           
            // 3: 危險枯枝清除
            // 4: 植栽基盤維護暨環境整理
            // 5: 樹木健康管理
            // 6: 營養評估追肥
            // 7: 安全衛生防護

            ProcessMaintenanceTask(row, errors, "危險枯枝清除", 3, (status, note) => {
                data.task1Status = status;
                data.task1Note = note;
            });

            ProcessMaintenanceTask(row, errors, "植栽基盤維護暨環境整理", 4, (status, note) => {
                data.task2Status = status;
                data.task2Note = note;
            });

            ProcessMaintenanceTask(row, errors, "樹木健康管理", 5, (status, note) => {
                data.task3Status = status;
                data.task3Note = note;
            });

            ProcessMaintenanceTask(row, errors, "營養評估追肥", 6, (status, note) => {
                data.task4Status = status;
                data.task4Note = note;
            });

            ProcessMaintenanceTask(row, errors, "安全衛生防護", 7, (status, note) => {
                data.task5Status = status;
                data.task5Note = note;
            });

            return errors;
        }
        private string GetCellValue(IRow row, int index)
        {
            if (row == null) return "";
            var cell = row.GetCell(index);
            if (cell == null) return "";

            // 處理公式或其他型態
            if (cell.CellType == CellType.Formula)
            {
                try { return cell.NumericCellValue.ToString(); }
                catch { return cell.StringCellValue.Trim(); }
            }
            return cell.ToString().Trim();
        }
        private IRow GetAndRemove(Dictionary<string, SideRowInfo> map, string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            if (map.ContainsKey(key))
            {
                var info = map[key];
                map.Remove(key);
                return info.row;
            }
            return null;
        }
        private void CheckTreeNoConsistency(string mainTreeNo, IRow subRow, string sheetName, List<string> errorList)
        {
            if (subRow == null) return;

            // 假設附屬頁籤樹號都在 Index 1
            string subTreeNo = GetCellValue(subRow, 1);

            if (!string.Equals(mainTreeNo, subTreeNo, StringComparison.OrdinalIgnoreCase))
            {
                errorList.Add($"[{sheetName}]樹號不符({subTreeNo})");
            }
        }
        /// <summary>
        /// 處理孤兒資料 (將字典中剩餘的資料轉為失敗 Log)
        /// </summary>
        private void ProcessOrphans(Dictionary<string, SideRowInfo> map, string sheetName, List<TreeBatchTaskLog> logs)
        {
            foreach (var kvp in map)
            {
                var info = kvp.Value;
                string orphanTreeNo = GetCellValue(info.row, 1);

                logs.Add(new TreeBatchTaskLog
                {
                    taskID = 0,
                    sourceItem = $"{sheetName}-第{info.rowIndex + 1}列",
                    refKey = orphanTreeNo,
                    isSuccess = false,
                    resultMsg = "失敗：無對應資料 (在基本資料頁籤中找不到對應的調查記數)"
                });
            }
        }

        private bool CheckTextLength(List<string> errors, string val, int limit, string fieldName)
        {
            if (val.Length > limit)
            {
                errors.Add($"[{fieldName}] 內容過長：限制 {limit} 字");
                return false;
            }
            return true;
        }

       
        private void ProcessGroup(
            IRow row,
            List<string> errors,
            bool isStrictMode,
            string groupName,
            int normalCol,
            int abnormalStart,
            int abnormalEnd,
            int[] exceptionCols,
            Action<int, string> mappingAction,
            Action<int> setStatusAction
        )
        {
            var validBoolValues = new HashSet<string> { "是", "否" }; // 定義在裡面或拉成全域變數皆可
            bool isNormal = false;
            bool hasAbnormal = false;

            // 1. 檢查「正常」欄位
            string valNormal = GetCellValue(row, normalCol);
            if (!string.IsNullOrEmpty(valNormal))
            {
                if (!validBoolValues.Contains(valNormal))
                {
                    errors.Add($"[{groupName}] 正常欄位格式錯誤");
                }
                else if (valNormal == "是")
                {
                    isNormal = true;
                }
            }

            // 2. 檢查「異常」欄位群
            for (int c = abnormalStart; c <= abnormalEnd; c++)
            {
                string val = GetCellValue(row, c);
                if (string.IsNullOrEmpty(val)) continue;

                bool isException = exceptionCols != null && exceptionCols.Contains(c);

                // 一般欄位檢查 (只能填 是/否)
                if (!isException)
                {
                    if (!validBoolValues.Contains(val))
                    {
                        errors.Add($"[{groupName}] 選項格式錯誤 (第{c + 1}欄)");
                        continue;
                    }
                }

                // 只有填 "否" 時跳過 (不算異常)
                if (val == "否") continue;

                // 填 "是" 或 "特例內容" -> 視為異常
                hasAbnormal = true;
                mappingAction(c, val);
            }

            // 3. 狀態判定
            if (isNormal && hasAbnormal)
            {
                errors.Add($"[{groupName}] 邏輯錯誤：不可同時勾選「正常(是)」與「異常項目」。");
            }
            else if (hasAbnormal) setStatusAction(2); // 異常
            else if (isNormal) setStatusAction(1);    // 良好
            else
            {
                if (isStrictMode) errors.Add($"[{groupName}] 未填寫：請至少勾選一項狀態。");
                else setStatusAction(0); // 未填
            }
        }

        private void ProcessMaintenanceTask(IRow row,List<string> errors,string taskName,int colIndex,Action<int, string> mappingAction)
        {
            // 1. 讀取 Excel 內容
            string val = GetCellValue(row, colIndex);

            // 2. 判斷邏輯
            if (string.IsNullOrEmpty(val))
            {
               
                mappingAction(0, null);
            }
            else
            {
                // --- 情況 B: 有填寫 ---
                // 使用共用的 CheckTextLength 檢查長度 (限制 500 字)
                if (CheckTextLength(errors, val, 500, taskName))
                {
                    // 檢查通過 -> Status = 1 (完成), Note = 內容
                    mappingAction(1, val);
                }
              
            }
        }
    }
}