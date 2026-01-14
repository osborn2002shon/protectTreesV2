using NPOI.SS.UserModel;
using protectTreesV2.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.SS.UserModel; 
using NPOI.XSSF.UserModel; 
using NPOI.HSSF.UserModel; 
using static protectTreesV2.Batch.Batch;
using static protectTreesV2.Health.Health;
using NPOI.HSSF.Record.Chart;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.health
{
    public partial class upload : protectTreesV2.Base.BasePage
    {
        protectTreesV2.Batch.Batch system_batch = new Batch.Batch();

        public class ImportTreeDataModel
        {
            public TreeBatchTaskLog log { get; set; }    
            public TreeHealthRecord record { get; set; } 
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

            public TreeHealthRecord parsedData { get; set; } = new TreeHealthRecord();

            public DateTime? surveyDate { get; set; }
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
            var user = UserService.GetCurrentUser();
            int userId = user?.userID ?? 0;

            // 綁定歷史紀錄
            var historyList = system_batch.GetBatchTaskList(enum_treeBatchType.Health_Record, userId);
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
            // 檢視樹籍資料 (ViewTree)
            if (e.CommandName == "ViewTree")
            {
                string treeNo = e.CommandArgument.ToString();

                List<string> searchList = new List<string> { treeNo };
                Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(searchList);

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
            // 檢視健檢紀錄 (ViewHealth)
            else if (e.CommandName == "ViewHealth")
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
                Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(searchTreeList);

                if (treeIdMap.ContainsKey(treeNo))
                {
                    int treeID = treeIdMap[treeNo];

                    // 2組裝查詢 Key (TreeID + Date)
                    var queryKeys = new List<TreeQueryKey>
                    {
                        new TreeQueryKey { treeID = treeID, checkDate = checkDate }
                    };

                    // 反查 HealthID
                    Dictionary<string, int> healthMap = system_batch.GetHealthIDMap(queryKeys);

                    // Key 的格式通常是 $"{treeID}_{yyyyMMdd}"
                    string mapKey = $"{treeID}_{checkDate:yyyyMMdd}";

                    if (healthMap.ContainsKey(mapKey))
                    {
                        int healthID = healthMap[mapKey];

                        // 設定 Session 並跳轉
                        setHealthID = healthID.ToString(); // 設定健檢 ID
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
        protected void Button_StartUpload_Click(object sender, EventArgs e)
        {
            // ==========================================
            // 1. 基礎檢查 (檔案存在、格式、大小)
            // ==========================================
            var user = UserService.GetCurrentUser();
            int accountID = user?.userID ?? 0;

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
                string relativeFolder = $"~/_file/health/upload/{accountID}/{yyyyMM}/";
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

            // ==========================================
            // 3. 定義檢查規則
            // ==========================================
            var sheetRules = new Dictionary<string, (int TargetRow, string[] Cols)>
    {
        { "基本資料", (0, new[] {
            "調查記數", "樹籍編號", "機關樹木編號", "座落地點縣市", "座落地點鄉鎮",
            "樹種", "調查日期", "調查人", "樹牌", "樹高(m)",
            "樹冠投影面積(m2)", "N(緯度)", "E(經度)", "米圍(1.0m處)", "米徑(1.0m處)",
            "胸圍(1.3m處)", "胸徑(1.3m處)", "備註(上移或下移實際量測高度)"
        })},
        { "病蟲害調查", (1, new[] {
            "調查記數", "樹籍編號", "樹種",
            "重大病害樹木褐根病 / 靈芝 / 木材腐朽菌 / 潰瘍 / 其他(____)",
            "根系", "樹基部", "主幹", "枝幹", "樹冠", "其他",
            "根系", "樹基部", "主幹", "枝幹", "樹冠", "其他",
            "根系", "樹基部", "主幹", "枝幹", "樹冠", "其他",
            "其他病蟲害問題詳加備註"
        })},
        { "樹木生長外觀情況", (1, new[] {
            "調查記數", "樹籍編號", "樹種",
            "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "打草傷", "根傷", "盤根", "其他",
            "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "打草傷", "其他",
            "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "內生夾皮", "其他",
            "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "內生夾皮", "下垂枝", "其他",
            "樹葉生長覆蓋度百分比%", "一般枯枝%", "懸掛枝", "其他",
            "其他問題詳加備註"
        })},
        { "樹木修剪與支撐情況", (1, new[] {
            "調查記數", "樹籍編號", "樹種",
            "錯誤修剪傷害", "修剪傷口是否有癒合", "附生植物", "寄生植物", "纏勒性植物", "其他修剪問題詳加備註",
            "有設立支架（單位為支，請輸入數字）", "支架已嵌入樹皮", "其他支撐問題詳加備註"
        })},
        { "生育地環境與土讓檢測情況", (1, new[] {
            "調查記數", "樹籍編號", "樹種",
            "水泥鋪面%", "柏油鋪面%", "花台", "休憩設施(桌椅)", "雜物堆置",
            "受限建物之間", "土壤受踩踏夯實", "覆土過深", "其他生育地問題詳加備註",
            "土壤酸鹼度(pH值)", "有機質含量", "電導度(EC值)"
        })},
        { "健康檢查結果及風險評估", (0, new[] {
            "調查記數", "樹籍編號", "樹種", "管理情況", "建議處理優先順序", "處理情形說明"
        })}
    };

            // ==========================================
            // 4. 開始解析與處理
            // ==========================================
            // 所有的 Log 
            List<TreeBatchTaskLog> allLogList = new List<TreeBatchTaskLog>();
            List<ImportTreeDataModel> validDataList = new List<ImportTreeDataModel>();

            // 業務邏輯旗標
            bool isSetFinal = CheckBox_SetFinal.Checked;   // 定稿
            bool isOverwrite = CheckBox_Overwrite.Checked; // 覆蓋
            bool isStrictMode = isSetFinal || isOverwrite; // 必填檢查

            // 重複檢查 (同檔內)
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
                        if (!descText.Contains("填寫說明") ) 
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
                    // 4.2 讀取資料 (Data Parsing)
                    // ---------------------------------------------------------
                    var pestMap = ScanSideSheet(workbook, "病蟲害調查", 3, 0, allLogList);
                    var appearanceMap = ScanSideSheet(workbook, "樹木生長外觀情況", 3, 0, allLogList);
                    var pruningMap = ScanSideSheet(workbook, "樹木修剪與支撐情況", 3, 0, allLogList);
                    var soilMap = ScanSideSheet(workbook, "生育地環境與土讓檢測情況", 3, 0, allLogList);
                    var riskMap = ScanSideSheet(workbook, "健康檢查結果及風險評估", 2, 0, allLogList);

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
                                resultMsg = $"失敗：調查日期格式錯誤"
                            });
                            continue; 
                        }

                        // 民國年自動校正 (ROC Logic)
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
                        BasicInfoParseResult basicResult = ValidateBasicInfo(row, isStrictMode, validSurveyDate);

                        if (basicResult.errors.Count > 0) rowErrors.AddRange(basicResult.errors);
                        if (basicResult.surveyDate.HasValue) myLog.refDate = basicResult.surveyDate.Value;

                        TreeHealthRecord draftRecord = basicResult.parsedData;

                        // 領取附屬資料
                        IRow pestRow = GetAndRemove(pestMap, linkKey);
                        IRow appRow = GetAndRemove(appearanceMap, linkKey);
                        IRow prunRow = GetAndRemove(pruningMap, linkKey);
                        IRow soilRow = GetAndRemove(soilMap, linkKey);
                        IRow riskRow = GetAndRemove(riskMap, linkKey);

                        // --- 附屬頁籤檢查 ---
                        if (pestRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, pestRow, "病蟲害調查", rowErrors);
                            var errs = ParseAndValidatePestInfo(pestRow, draftRecord, isStrictMode);
                            if (errs.Count > 0) rowErrors.AddRange(errs);
                        }
                        if (appRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, appRow, "樹木生長外觀情況", rowErrors);
                            var errs = ParseAndValidateAppearance(appRow, draftRecord, isStrictMode);
                            if (errs.Count > 0) rowErrors.AddRange(errs);
                        }
                        if (prunRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, prunRow, "樹木修剪與支撐情況", rowErrors);
                            var errs = ParseAndValidatePruning(prunRow, draftRecord, isStrictMode);
                            if (errs.Count > 0) rowErrors.AddRange(errs);
                        }
                        if (soilRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, soilRow, "生育地環境與土讓檢測情況", rowErrors);
                            var errs = ParseAndValidateSoil(soilRow, draftRecord, isStrictMode);
                            if (errs.Count > 0) rowErrors.AddRange(errs);
                        }
                        if (riskRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, riskRow, "健康檢查結果及風險評估", rowErrors);
                            var errs = ParseAndValidateRisk(riskRow, draftRecord, isStrictMode);
                            if (errs.Count > 0) rowErrors.AddRange(errs);
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
                            draftRecord.dataStatus = isSetFinal ? 1 : 0;

                            validDataList.Add(new ImportTreeDataModel
                            {
                                log = myLog,
                                record = draftRecord
                            }); ;
                        }
                    } // End Loop

                    // 處理孤兒資料
                    ProcessOrphans(pestMap, "病蟲害調查", allLogList);
                    ProcessOrphans(appearanceMap, "樹木生長外觀情況", allLogList);
                    ProcessOrphans(pruningMap, "樹木修剪與支撐情況", allLogList);
                    ProcessOrphans(soilMap, "生育地環境與土讓檢測情況", allLogList);
                    ProcessOrphans(riskMap, "健康檢查結果及風險評估", allLogList);

                } // End Using FileStream

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
                    List<string> distinctTreeNos = validDataList.Select(x => x.record.systemTreeNo).Distinct().ToList();
                    Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(distinctTreeNos);

                    for (int i = validDataList.Count - 1; i >= 0; i--)
                    {
                        var item = validDataList[i];
                        string tNo = item.record.systemTreeNo;

                        if (treeIdMap.ContainsKey(tNo))
                        {
                            item.record.treeID = treeIdMap[tNo];
                        }
                        else
                        {
                            item.log.isSuccess = false;
                            item.log.resultMsg = $"失敗：查無系統樹籍編號 ({tNo})";
                            validDataList.RemoveAt(i);
                        }
                    }

                    if (validDataList.Count > 0)
                    {
                        var queryKeys = validDataList.Select(x => new TreeQueryKey
                        {
                            treeID = x.record.treeID,
                            checkDate = x.record.surveyDate.Value
                        }).Distinct().ToList();

                        Dictionary<string, int> healthMap = system_batch.GetHealthIDMap(queryKeys);

                        for (int i = validDataList.Count - 1; i >= 0; i--)
                        {
                            var item = validDataList[i];
                            string key = $"{item.record.treeID}_{item.record.surveyDate:yyyyMMdd}";

                            if (healthMap.ContainsKey(key))
                            {
                                int oldHealthID = healthMap[key];
                                if (isOverwrite)
                                {
                                    item.record.healthID = oldHealthID;
                                    item.log.resultMsg += "提醒：已覆蓋同日調查資料";
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

                var updateModels = validDataList.Where(x => x.record.healthID > 0).ToList();
                var insertModels = validDataList.Where(x => x.record.healthID == 0).ToList();

                // -----------------------------------------------------
                // 執行更新 
                // -----------------------------------------------------
                if (updateModels.Count > 0)
                {
                    try
                    {
                        // 1. 準備純資料 List 給 Service 用
                        var recordsToUpdate = updateModels.Select(x => x.record).ToList();

                        // 2. 設定欄位
                        foreach (var rec in recordsToUpdate)
                        {
                            rec.updateAccountID = accountID;
                            rec.updateDateTime = DateTime.Now;
                            if (isSetFinal) rec.dataStatus = 1;
                        }

                        // 3. 執行 SQL
                        system_batch.BulkUpdateHealthRecords(recordsToUpdate, updateStatus: isSetFinal);

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
                        // 1. 準備純資料 List
                        var recordsToInsert = insertModels.Select(x => x.record).ToList();

                        // 2. 設定欄位
                        foreach (var rec in recordsToInsert)
                        {
                            rec.insertAccountID = accountID;
                            rec.insertDateTime = DateTime.Now;
                        }

                        // 3. 執行 SQL
                        system_batch.BulkInsertHealthRecords(recordsToInsert);

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
                        enum_treeBatchType.Health_Record,
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
        private BasicInfoParseResult ValidateBasicInfo(IRow row, bool isSetFinal, DateTime confirmedDate)
        {
            var result = new BasicInfoParseResult();
            var errors = result.errors;
            var data = result.parsedData; 

            // ---------------------------------------------------------
            // 樹籍編號 (Col 1)
            // ---------------------------------------------------------
            string valTreeNo = GetCellValue(row, 1);
            data.systemTreeNo = valTreeNo;

            // ---------------------------------------------------------
            // 調查日期 (Col 6)
            // ---------------------------------------------------------
            data.surveyDate = confirmedDate;
            result.surveyDate = confirmedDate;

            // ---------------------------------------------------------
            // 調查人 (Col 7)
            // ---------------------------------------------------------
            string valSurveyor = GetCellValue(row, 7);
            if (isSetFinal && string.IsNullOrEmpty(valSurveyor))
            {
                errors.Add("調查人 (必填)");
            }
            else if (!string.IsNullOrEmpty(valSurveyor) && valSurveyor.Length > 50)
            {
                errors.Add("調查人 (長度須小於50字)");
            }
            else
            {
                data.surveyor = valSurveyor;
            }

            // ---------------------------------------------------------
            // 樹牌狀態 (Col 8) - Enum 轉換
            // ---------------------------------------------------------
            string valTreeSign = GetCellValue(row, 8);
            if (string.IsNullOrEmpty(valTreeSign))
            {
                if (isSetFinal) errors.Add("樹牌狀態 (必填)");
            }
            else
            {
                switch (valTreeSign.Trim())
                {
                    case "有":
                        data.treeSignStatus = (int)enum_treeSignStatus.有; break;
                    case "沒有":
                        data.treeSignStatus = (int)enum_treeSignStatus.沒有; break;
                    case "毀損":
                        data.treeSignStatus = (int)enum_treeSignStatus.毀損; break;
                    default: errors.Add($"樹牌狀態 (選項錯誤: {valTreeSign})"); break;
                }
            }

            // ---------------------------------------------------------
            // 數值檢查 (樹高, 面積, 經緯度)
            // ---------------------------------------------------------
            // 樹高 (Col 9)
            string valHeight = GetCellValue(row, 9);
            if (string.IsNullOrEmpty(valHeight))
            {
                if (isSetFinal) errors.Add("樹高 (必填)");
            }
            else
            {
                if (!decimal.TryParse(valHeight, out decimal h)) errors.Add("樹高 (格式錯誤)");
                else if (h < 0 || h > 999999.99m) errors.Add("樹高 (須介於 0~999999.99)");
                else data.treeHeight = h;
            }

            // 樹冠面積 (Col 10)
            string valArea = GetCellValue(row, 10);
            if (string.IsNullOrEmpty(valArea))
            {
                if (isSetFinal) errors.Add("樹冠投影面積 (必填)");
            }
            else
            {
                if (!decimal.TryParse(valArea, out decimal a)) errors.Add("樹冠投影面積 (格式錯誤)");
                else if (a < 0 || a > 999999.99m) errors.Add("樹冠投影面積 (須介於 0~999999.99)");
                else data.canopyArea = a;
            }

            // 緯度 (Col 11)
            string valLat = GetCellValue(row, 11);
            if (string.IsNullOrEmpty(valLat))
            {
                if (isSetFinal) errors.Add("緯度 (必填)");
            }
            else
            {
                if (!decimal.TryParse(valLat, out decimal lat)) errors.Add("緯度 (格式錯誤)");
                else if (lat < -90 || lat > 90) errors.Add("緯度 (超出範圍 -90~90)");
                else data.latitude = lat;
            }

            // 經度 (Col 12)
            string valLon = GetCellValue(row, 12);
            if (string.IsNullOrEmpty(valLon))
            {
                if (isSetFinal) errors.Add("經度 (必填)");
            }
            else
            {
                if (!decimal.TryParse(valLon, out decimal lon)) errors.Add("經度 (格式錯誤)");
                else if (lon < -180 || lon > 180) errors.Add("經度 (超出範圍 -180~180)");
                else data.longitude = lon;
            }

            // ---------------------------------------------------------
            // 多值欄位 (米圍, 米徑, 胸圍, 胸徑)
            // ---------------------------------------------------------
            // 定義一個小的本地檢查 Action 避免重複代碼
            void CheckMultiVal(int colIdx, string name, Action<string> setProp)
            {
                string val = GetCellValue(row, colIdx);
                if (string.IsNullOrEmpty(val))
                {
                    if (isSetFinal) errors.Add($"{name} (必填)");
                }
                else
                {
                    var parts = val.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    bool isAllNum = true;
                    foreach (var p in parts)
                    {
                        if (!decimal.TryParse(p.Trim(), out _)) { isAllNum = false; break; }
                    }

                    if (!isAllNum) errors.Add($"{name} (含有非數字格式)");
                    else setProp(val); // 格式正確才賦值
                }
            }

            CheckMultiVal(13, "米圍", v => data.girth100 = v);
            CheckMultiVal(14, "米徑", v => data.diameter100 = v);
            CheckMultiVal(15, "胸圍", v => data.girth130 = v);
            CheckMultiVal(16, "胸徑", v => data.diameter130 = v);

            return result;
        }

        /// <summary>
        /// 解析並驗證 [病蟲害調查] 資料，直接填入 record
        /// </summary>
        private List<string> ParseAndValidatePestInfo(IRow row, TreeHealthRecord record, bool isStrictMode)
        {
            List<string> errors = new List<string>();

            if (row == null)
            {
                // 嚴格模式下，若認為病蟲害必填可解開註解
                // if (isStrictMode) errors.Add("缺病蟲害調查資料");
                return errors;
            }

            // =============================================================
            // 1. 重大病害 (Col 3) - 多選字串解析 -> Bit 欄位
            // =============================================================
            string valDisease = GetCellValue(row, 3);

            if (!string.IsNullOrEmpty(valDisease))
            {
                var parts = valDisease.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rawPart in parts)
                {
                    string part = rawPart.Trim();

                    if (part == "樹木褐根病") record.majorDiseaseBrownRoot = true;
                    else if (part == "靈芝") record.majorDiseaseGanoderma = true;
                    else if (part == "木材腐朽菌") record.majorDiseaseWoodDecayFungus = true; 
                    else if (part == "潰瘍") record.majorDiseaseCanker = true;
                    else if (part.StartsWith("其他"))
                    {
                        record.majorDiseaseOther = true;

                        // 解析括號內容： 其他(內容)
                        string note = "";
                        int start = part.IndexOf('(');
                        int end = part.LastIndexOf(')');
                        if (start == -1) { start = part.IndexOf('（'); end = part.LastIndexOf('）'); } 

                        if (start > -1 && end > start)
                        {
                            note = part.Substring(start + 1, end - start - 1);
                        }

                        // 填入 DB [majorDiseaseOtherNote]
                        record.majorDiseaseOtherNote = note;
                    }
                    else
                    {
                        errors.Add($"重大病害 (選項不合法: {part})，若為其他請填寫格式：其他(說明)");
                    }
                }
            }

            // =============================================================
            // 重大蟲害 (Col 4~9) - 文字解析 -> 對應部位的 Bit 欄位
            // =============================================================
            // 資料庫設計是每個部位有三個 Bit: Tunnel(穿鑿), Chew(啃食), Live(活蟲)
            // Excel 欄位對應: 4=根系, 5=樹基, 6=主幹, 7=枝幹, 8=樹冠, 9=其他

            var allowedTermiteValues = new HashSet<string> { "白蟻蟻道", "白蟻蛀蝕", "白蟻活體" };

            // 定義一個 Action 來處理單一部位的邏輯
            void CheckAndSetTermite(int col, string locationName,
                                    Action<bool> setTunnel, Action<bool> setChew, Action<bool> setLive)
            {
                string val = GetCellValue(row, col);
                if (!string.IsNullOrEmpty(val))
                {
                    // 支援多選嗎?假設一格只填一種狀態，若有多種需 Split
                    // 這裡假設是單選，或是用逗號分隔
                    var states = val.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var stateRaw in states)
                    {
                        string state = stateRaw.Trim();
                        if (state == "白蟻蟻道") setTunnel(true);
                        else if (state == "白蟻蛀蝕") setChew(true);
                        else if (state == "白蟻活體") setLive(true);
                        else
                        {
                            errors.Add($"重大蟲害-{locationName} (內容不合法: {state})，僅限填寫：白蟻蟻道、白蟻蛀蝕、白蟻活體");
                        }
                    }
                }
            }

            // 依序檢查各部位並填入 DB 對應欄位
            CheckAndSetTermite(4, "根系", v => record.majorPestRootTunnel = v, v => record.majorPestRootChew = v, v => record.majorPestRootLive = v);
            CheckAndSetTermite(5, "樹基", v => record.majorPestBaseTunnel = v, v => record.majorPestBaseChew = v, v => record.majorPestBaseLive = v);
            CheckAndSetTermite(6, "樹幹", v => record.majorPestTrunkTunnel = v, v => record.majorPestTrunkChew = v, v => record.majorPestTrunkLive = v);
            CheckAndSetTermite(7, "枝條", v => record.majorPestBranchTunnel = v, v => record.majorPestBranchChew = v, v => record.majorPestBranchLive = v);
            CheckAndSetTermite(8, "樹冠", v => record.majorPestCrownTunnel = v, v => record.majorPestCrownChew = v, v => record.majorPestCrownLive = v);
            CheckAndSetTermite(9, "其他", v => record.majorPestOtherTunnel = v, v => record.majorPestOtherChew = v, v => record.majorPestOtherLive = v);

            // =============================================================
            // 一般蟲害 (Col 10~15) - 純文字
            // =============================================================
            // 依序讀取並填入 DB 對應欄位
            record.generalPestRoot = GetCellValue(row, 10);
            record.generalPestBase = GetCellValue(row, 11);
            record.generalPestTrunk = GetCellValue(row, 12);
            record.generalPestBranch = GetCellValue(row, 13);
            record.generalPestCrown = GetCellValue(row, 14);
            record.generalPestOther = GetCellValue(row, 15);

            // =============================================================
            // 一般病害 (Col 16~21) - 純文字
            // =============================================================
            record.generalDiseaseRoot = GetCellValue(row, 16);
            record.generalDiseaseBase = GetCellValue(row, 17);
            record.generalDiseaseTrunk = GetCellValue(row, 18);
            record.generalDiseaseBranch = GetCellValue(row, 19);
            record.generalDiseaseCrown = GetCellValue(row, 20);
            record.generalDiseaseOther = GetCellValue(row, 21);

            // =============================================================
            // 其他備註 (Col 22) 
            // =============================================================
            record.pestOtherNote = GetCellValue(row, 22);

            return errors;
        }

        /// <summary>
        /// 解析並驗證 [樹木生長外觀情況]，直接填入 record
        /// </summary>
        private List<string> ParseAndValidateAppearance(IRow row, TreeHealthRecord record, bool isStrictMode)
        {
            List<string> errors = new List<string>();

            if (row == null) return errors;

            // =========================================================================
            // 1. 根系 (Root) - Index 3~10
            // 標頭: 腐朽百分比%, 樹洞最大直徑(m), 傷口最大直徑(m), 機具損傷, 打草傷, 根傷, 盤根, 其他
            // =========================================================================
            record.rootDecayPercent = ParseDecimalColumn(row, 3, "根系-腐朽百分比%", isStrictMode, 0, 100, errors);
            record.rootCavityMaxDiameter = ParseDecimalColumn(row, 4, "根系-樹洞最大直徑(m)", false, 0, 9999.99m, errors);
            record.rootWoundMaxDiameter = ParseDecimalColumn(row, 5, "根系-傷口最大直徑(m)", false, 0, 9999.99m, errors);

            // 單選狀態 (只允許 有/無)
            record.rootMechanicalDamage = ParseBoolColumn(row, 6, "根系-機具損傷", errors, isStrictMode);
            record.rootMowingInjury = ParseBoolColumn(row, 7, "根系-打草傷", errors, isStrictMode);
            record.rootInjury = ParseBoolColumn(row, 8, "根系-根傷", errors, isStrictMode);
            record.rootGirdling = ParseBoolColumn(row, 9, "根系-盤根", errors, isStrictMode); 

            // 備註
            record.rootOtherNote = GetCellValue(row, 10);

            // =========================================================================
            // 2. 樹基部 (Base) - Index 11~16
            // 標頭: 腐朽百分比%, 樹洞最大直徑(m), 傷口最大直徑(m), 機具損傷, 打草傷, 其他
            // =========================================================================
            record.baseDecayPercent = ParseDecimalColumn(row, 11, "樹基部-腐朽百分比%", isStrictMode, 0, 100, errors);
            record.baseCavityMaxDiameter = ParseDecimalColumn(row, 12, "樹基部-樹洞最大直徑(m)", false, 0, 9999.99m, errors);
            record.baseWoundMaxDiameter = ParseDecimalColumn(row, 13, "樹基部-傷口最大直徑(m)", false, 0, 9999.99m, errors);

            record.baseMechanicalDamage = ParseBoolColumn(row, 14, "樹基部-機具損傷", errors, isStrictMode);
            record.baseMowingInjury = ParseBoolColumn(row, 15, "樹基部-打草傷", errors, isStrictMode);

            record.baseOtherNote = GetCellValue(row, 16);

            // =========================================================================
            // 3. 主幹 (Trunk) - Index 17~22
            // 標頭: 腐朽百分比%, 樹洞最大直徑(m), 傷口最大直徑(m), 機具損傷, 內生夾皮, 其他
            // =========================================================================
            record.trunkDecayPercent = ParseDecimalColumn(row, 17, "主幹-腐朽百分比%", isStrictMode, 0, 100, errors);
            record.trunkCavityMaxDiameter = ParseDecimalColumn(row, 18, "主幹-樹洞最大直徑(m)", false, 0, 9999.99m, errors);
            record.trunkWoundMaxDiameter = ParseDecimalColumn(row, 19, "主幹-傷口最大直徑(m)", false, 0, 9999.99m, errors);

            record.trunkMechanicalDamage = ParseBoolColumn(row, 20, "主幹-機具損傷", errors, isStrictMode);
            record.trunkIncludedBark = ParseBoolColumn(row, 21, "主幹-內生夾皮", errors, isStrictMode);

            record.trunkOtherNote = GetCellValue(row, 22);

            // =========================================================================
            // 4. 枝幹 (Branch) - Index 23~29
            // 標頭: 腐朽百分比%, 樹洞最大直徑(m), 傷口最大直徑(m), 機具損傷, 內生夾皮, 下垂枝, 其他
            // =========================================================================
            record.branchDecayPercent = ParseDecimalColumn(row, 23, "枝幹-腐朽百分比%", isStrictMode, 0, 100, errors);
            record.branchCavityMaxDiameter = ParseDecimalColumn(row, 24, "枝幹-樹洞最大直徑(m)", false, 0, 9999.99m, errors);
            record.branchWoundMaxDiameter = ParseDecimalColumn(row, 25, "枝幹-傷口最大直徑(m)", false, 0, 9999.99m, errors);

            record.branchMechanicalDamage = ParseBoolColumn(row, 26, "枝幹-機具損傷", errors, isStrictMode);
            record.branchIncludedBark = ParseBoolColumn(row, 27, "枝幹-內生夾皮", errors, isStrictMode);
            record.branchDrooping = ParseBoolColumn(row, 28, "枝幹-下垂枝", errors, isStrictMode);

            record.branchOtherNote = GetCellValue(row, 29);

            // =========================================================================
            // 5. 樹冠 (Crown) - Index 30~33
            // 標頭: 樹葉生長覆蓋度百分比%, 一般枯枝%, 懸掛枝, 其他
            // =========================================================================
            record.crownLeafCoveragePercent = ParseDecimalColumn(row, 30, "樹冠-樹葉生長覆蓋度百分比%", isStrictMode, 0, 100, errors);
            record.crownDeadBranchPercent = ParseDecimalColumn(row, 31, "樹冠-一般枯枝%", isStrictMode, 0, 100, errors);

            record.crownHangingBranch = ParseBoolColumn(row, 32, "樹冠-懸掛枝", errors, isStrictMode);

            record.crownOtherNote = GetCellValue(row, 33);

            // =========================================================================
            // 6. 最後 (Other) - Index 34
            // 標頭: 其他問題詳加備註
            // =========================================================================
            record.growthNote = GetCellValue(row, 34);

            return errors;
        }
        /// <summary>
        /// 解析並驗證 [樹木修剪與支撐情況]，直接填入 record
        /// </summary>
        private List<string> ParseAndValidatePruning(IRow row, TreeHealthRecord record, bool isStrictMode)
        {
            List<string> errors = new List<string>();

            if (row == null) return errors;

            // =========================================================================
            // 1. 修剪情況 (Pruning) - Index 3~8
            // =========================================================================

            // A. 錯誤修剪傷害 (Enum 字串驗證) - Index 3
            // DB 欄位是 nvarchar，但內容必須限制在 Enum 範圍內
            string valDamage = GetCellValue(row, 3);
            if (!string.IsNullOrEmpty(valDamage))
            {
                valDamage = valDamage.Trim();
                // 檢查輸入文字是否在 Enum 定義中 (截幹, 截頂, 不當縮剪)
                if (Enum.IsDefined(typeof(enum_pruningDamageType), valDamage))
                {
                    record.pruningWrongDamage = valDamage;
                }
                else
                {
                    errors.Add($"錯誤修剪傷害 (選項錯誤: {valDamage})");
                }
            }
            // 若為 null 或空字串，視為無錯誤修剪，不報錯 (除非這是必填，看您需求)

            // B. 修剪傷口癒合 (Bool) - Index 4
            record.pruningWoundHealing = ParseBoolColumn(row, 4, "修剪傷口是否有癒合", errors, isStrictMode);

            // C. 附生植物 (Bool) - Index 5
            record.pruningEpiphyte = ParseBoolColumn(row, 5, "樹幹附生植物", errors, isStrictMode);

            // D. 寄生植物 (Bool) - Index 6
            record.pruningParasite = ParseBoolColumn(row, 6, "寄生植物情形", errors, isStrictMode);

            // E. 纏勒性植物 (Bool) - Index 7
            record.pruningVine = ParseBoolColumn(row, 7, "蔓藤纏繞情形", errors, isStrictMode);

            // F. 修剪其他說明 (String) - Index 8
            record.pruningOtherNote = GetCellValue(row, 8);

            // =========================================================================
            // 2. 支撐情況 (Support) - Index 9~11
            // =========================================================================

            // G. 支架數量 (Int) - Index 9
            // 假設範圍 0~100，若非嚴格模式則非必填
            record.supportCount = ParseIntColumn(row, 9, "支撐設施數量", false, 0, int.MaxValue, errors);

            // H. 支架嵌入 (Bool) - Index 10
            record.supportEmbedded = ParseBoolColumn(row, 10, "支撐設施嵌入樹幹", errors, isStrictMode);

            // I. 支撐其他說明 (String) - Index 11
            record.supportOtherNote = GetCellValue(row, 11);

            return errors;
        }

        /// <summary>
        /// 解析並驗證 [生育地環境與土讓檢測情況]，直接填入 record
        /// </summary>
        private List<string> ParseAndValidateSoil(IRow row, TreeHealthRecord record, bool isStrictMode)
        {
            List<string> errors = new List<string>();

            if (row == null) return errors;

            // =========================================================================
            // 1. 生育地環境 - 鋪面比例 (Index 3~4)
            // 標頭: 水泥鋪面%, 柏油鋪面%
            // =========================================================================
            record.siteCementPercent = ParseDecimalColumn(row, 3, "水泥鋪面%", isStrictMode, 0, 100, errors);
            record.siteAsphaltPercent = ParseDecimalColumn(row, 4, "柏油鋪面%", isStrictMode, 0, 100, errors);

            // =========================================================================
            // 2. 生育地環境 - 單選題 (Index 5~10)
            // 標頭: 花台, 休憩設施(桌椅), 雜物堆置, 受限建物之間, 土壤受踩踏夯實, 覆土過深
            // 規則: 嚴格限制填寫 "有" 或 "無"
            // =========================================================================
            record.sitePlanter = ParseBoolColumn(row, 5, "花台", errors, isStrictMode);
            record.siteRecreationFacility = ParseBoolColumn(row, 6, "休憩設施(桌椅)", errors, isStrictMode);
            record.siteDebrisStack = ParseBoolColumn(row, 7, "雜物堆置", errors, isStrictMode);
            record.siteBetweenBuildings = ParseBoolColumn(row, 8, "受限建物之間", errors, isStrictMode);
            record.siteSoilCompaction = ParseBoolColumn(row, 9, "土壤受踩踏夯實", errors, isStrictMode);
            record.siteOverburiedSoil = ParseBoolColumn(row, 10, "覆土過深", errors, isStrictMode);

            // =========================================================================
            // 3. 其他備註 (Index 11)
            // 標頭: 其他生育地問題詳加備註
            // =========================================================================
            record.siteOtherNote = GetCellValue(row, 11);

            // =========================================================================
            // 4. 土壤檢測 (Index 12~14)
            // 標頭: 土壤酸鹼度(pH值), 有機質含量, 電導度(EC值)
            // 規則: 文字欄位，直接讀取
            // =========================================================================
            record.soilPh = GetCellValue(row, 12);
            record.soilOrganicMatter = GetCellValue(row, 13);
            record.soilEc = GetCellValue(row, 14);

            return errors;
        }

        /// <summary>
        /// 解析並驗證 [健康檢查結果及風險評估]，直接填入 record
        /// </summary>
        private List<string> ParseAndValidateRisk(IRow row, TreeHealthRecord record, bool isStrictMode)
        {
            List<string> errors = new List<string>();

            if (row == null) return errors;

            // =========================================================================
            // 1. 管理狀態說明 (Index 3)
            // =========================================================================
            record.managementStatus = GetCellValue(row, 3);

            // =========================================================================
            // 2. 建議處理優先順序 (Index 4) - Enum 檢查
            // Enum: 緊急處理, 優先處理, 例行養護
            // =========================================================================
            string valPriority = GetCellValue(row, 4);

            if (string.IsNullOrEmpty(valPriority))
            {
                // 定稿模式必填檢查
                if (isStrictMode) errors.Add("建議處理優先順序 (必填)");
            }
            else
            {
                valPriority = valPriority.Trim();

                // 檢查文字是否符合 Enum 定義
                if (Enum.IsDefined(typeof(enum_treatmentPriority), valPriority))
                {
                    record.priority = valPriority; // 直接存文字
                }
                else
                {
                    errors.Add($"建議處理優先順序 (選項錯誤: {valPriority})");
                }
            }

            // =========================================================================
            // 3. 處置或維護建議 (Index 5)
            // =========================================================================
            record.treatmentDescription = GetCellValue(row, 5);

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
        /// 驗證並解析整數欄位 (處理必填、範圍、非整數格式)
        /// </summary>
        private int? ParseIntColumn(IRow row, int col, string fieldName, bool isRequired, int min, int max, List<string> errors)
        {
            string val = GetCellValue(row, col);

            if (string.IsNullOrEmpty(val))
            {
                if (isRequired) errors.Add($"{fieldName} (必填)");
                return null;
            }

            // 移除可能的小數點 (有些 Excel 即使輸入 1 也會變成 1.0)
            if (double.TryParse(val, out double d))
            {
                // 檢查是否為整數 (例如 1.5 就是錯的)
                if (d % 1 != 0)
                {
                    errors.Add($"{fieldName} (必須為整數)");
                    return null;
                }
                val = ((int)d).ToString();
            }
            else
            {
                errors.Add($"{fieldName} (格式錯誤)");
                return null;
            }

            if (!int.TryParse(val, out int num))
            {
                errors.Add($"{fieldName} (格式錯誤)");
                return null;
            }

            if (num < min || num > max)
            {
                errors.Add($"{fieldName} (須介於 {min}~{max})");
                return null;
            }

            return num;
        }
        /// <summary>
        /// 驗證並解析數值欄位 (處理百分比、範圍、必填)
        /// </summary>
        private decimal? ParseDecimalColumn(IRow row, int col, string fieldName, bool isRequired, decimal min, decimal max, List<string> errors)
        {
            string val = GetCellValue(row, col);

            // 自動移除百分比符號與空白
            if (!string.IsNullOrEmpty(val))
                val = val.Replace("%", "").Trim();

            if (string.IsNullOrEmpty(val))
            {
                if (isRequired)
                    errors.Add($"{fieldName} (必填)");
                return null; // 空值回傳 null
            }

            if (!decimal.TryParse(val, out decimal num))
            {
                errors.Add($"{fieldName} (格式錯誤)");
                return null;
            }

            if (num < min || num > max)
            {
                errors.Add($"{fieldName} (須介於 {min}~{max})");
                // 雖然超出範圍，但如果是因為打錯數字，通常還是回傳 null 比較安全，避免髒資料寫入
                return null;
            }

            return num; // 驗證成功，回傳數值
        }

        /// <summary>
        /// 驗證並解析布林欄位 (有/無)
        /// </summary>
        /// <param name="isStrictMode">是否為嚴格模式 (定稿或覆蓋時為 true)</param>
        private bool? ParseBoolColumn(IRow row, int col, string fieldName, List<string> errors, bool isStrictMode)
        {
            string val = GetCellValue(row, col);

            // 空值檢查
            if (string.IsNullOrEmpty(val))
            {
                // 只有在嚴格模式下，未填寫才算錯誤
                if (isStrictMode)
                {
                    errors.Add($"{fieldName} (必填)");
                }

                // 不論是否嚴格，空值都回傳 null
                return null;
            }

            val = val.Trim(); // 去除空白

            if (val == "有") return true;
            if (val == "無") return false;

            // 2. 內容錯誤 (不管是不是嚴格模式，填錯字就是錯)
            errors.Add($"{fieldName} (選項錯誤: {val})");
            return null;
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

        
    }
}