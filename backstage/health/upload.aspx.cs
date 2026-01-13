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
        protected void Button_StartUpload_Click(object sender, EventArgs e)
        {

            var user = UserService.GetCurrentUser();
            int accountID = user?.userID ?? 0;

            // 1. 基礎檢查：是否有選取檔案
            if (!FileUpload_Batch.HasFile)
            {
                ShowMessage("提示","請先選取檔案！");
                return;
            }

            // 2. 檢查副檔名 (Excel 格式)
            string fileName = FileUpload_Batch.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();

            if (fileExt != ".xls" && fileExt != ".xlsx")
            {
                ShowMessage("提示", "檔案格式錯誤！僅接受 .xls 或 .xlsx 格式。");
                return;
            }

            // 3. 檢查檔案大小 (30MB)
            // 30MB = 30 * 1024 * 1024 bytes
            int maxFileSize = 30 * 1024 * 1024;
            if (FileUpload_Batch.PostedFile.ContentLength > maxFileSize)
            {
                ShowMessage("提示", "檔案大小超過 30MB 限制！");
                return;
            }

            try
            {
               
                string yyyyMM = DateTime.Now.ToString("yyyyMM");

                // 實體路徑: ...\_file\health\upload\5\202601\
                string relativeFolder = $"~/_file/health/upload/{accountID}/{yyyyMM}/";
                string physicalFolder = Server.MapPath(relativeFolder);

                // 如果資料夾不存在則建立
                if (!Directory.Exists(physicalFolder))
                {
                    Directory.CreateDirectory(physicalFolder);
                }


                //取得檔名
                string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
                string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string baseFileName = $"{timeStamp}_{fileNameNoExt}";
                string finalFileName = baseFileName + fileExt;

                // 組合完整實體路徑
                string saveFullPath = Path.Combine(physicalFolder, finalFileName);
                int counter = 1;
                while (File.Exists(saveFullPath))
                {
                    finalFileName = $"{baseFileName}({counter}){fileExt}";
                    saveFullPath = Path.Combine(physicalFolder, finalFileName);
                    counter++;
                }

                FileUpload_Batch.SaveAs(saveFullPath);

                // Key: 頁籤名稱
                // Value: (TargetRow: 檢查哪一列, Cols: 欄位順序陣列)
                var sheetRules = new Dictionary<string, (int TargetRow, string[] Cols)>
                {
                    // 1. 基本資料 (TargetRow = 0)
                    { "基本資料", (0, new[] {
                        "調查記數", "樹籍編號", "機關樹木編號", "座落地點縣市", "座落地點鄉鎮",
                        "樹種", "調查日期", "調查人", "樹牌", "樹高(m)",
                        "樹冠投影面積(m2)", "N(緯度)", "E(經度)", "米圍(1.0m處)", "米徑(1.0m處)",
                        "胸圍(1.3m處)", "胸徑(1.3m處)", "備註(上移或下移實際量測高度)"
                    })},

                    // 2. 病蟲害調查 (TargetRow = 1，因為 Row 0 是大標題)
                    { "病蟲害調查", (1, new[] {
                        "調查記數", "樹籍編號", "樹種", 
                         // 重大病害
                        "重大病害樹木褐根病 / 靈芝 / 木材腐朽菌 / 潰瘍 / 其他(____)", 
                          // 重大蟲害
                        "根系", "樹基部", "主幹", "枝幹", "樹冠", "其他",
                        // 一般蟲害
                        "根系", "樹基部", "主幹", "枝幹", "樹冠", "其他",
                        // 一般病害
                        "根系", "樹基部", "主幹", "枝幹", "樹冠", "其他",
                       "其他病蟲害問題詳加備註",


                    })},

                    // 3. 樹木生長外觀情況 (TargetRow = 1)
                    // 注意：這裡欄位名稱重複很多次(腐朽百分比)，程式會依順序檢查，順序不能錯
                    { "樹木生長外觀情況", (1, new[] {
                        "調查記數", "樹籍編號", "樹種",
                         // 根系
                        "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "打草傷", "根傷", "盤根", "其他",
                        // 樹基部
                        "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "打草傷", "其他",
                          // 主幹
                        "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "內生夾皮", "其他",
                        // 枝幹
                        "腐朽百分比%", "樹洞最大直徑(m)", "傷口最大直徑(m)", "機具損傷", "內生夾皮", "下垂枝", "其他",
                        // 樹冠
                        "樹葉生長覆蓋度百分比%", "一般枯枝%", "懸掛枝", "其他",
                        // 最後
                        "其他問題詳加備註"
                    })},

                    // 4. 樹木修剪與支撐情況 (TargetRow = 1)
                    { "樹木修剪與支撐情況", (1, new[] {
                        "調查記數", "樹籍編號", "樹種",
                        // 樹木(近期)修剪情況
                        "錯誤修剪傷害", "修剪傷口是否有癒合", "附生植物", "寄生植物", "纏勒性植物", "其他修剪問題詳加備註",
                        // 樹木支撐情況 (注意：括號與換行會在檢查時被 Replace 掉，所以這裡字串要寫對)
                        "有設立支架（單位為支，請輸入數字）", "支架已嵌入樹皮", "其他支撐問題詳加備註"
                    })},

                    // 5. 生育地環境與土讓檢測情況 (TargetRow = 1)
                    { "生育地環境與土讓檢測情況", (1, new[] {
                        "調查記數", "樹籍編號", "樹種",
                          // 生育地環境
                        "水泥鋪面%", "柏油鋪面%", "花台", "休憩設施(桌椅)", "雜物堆置",
                        "受限建物之間", "土壤受踩踏夯實", "覆土過深", "其他生育地問題詳加備註",
                        // 土壤檢測
                        "土壤酸鹼度(pH值)", "有機質含量", "電導度(EC值)",
                      
                        
                    })},

                    // 6. 健康檢查結果及風險評估 (TargetRow = 0，只有一行)
                    { "健康檢查結果及風險評估", (0, new[] {
                        "調查記數", "樹籍編號", "樹種", "管理情況", "建議處理優先順序", "處理情形說明"
                    })}
                };

                // ==========================================
                // NPOI 格式檢查邏輯 (通用)
                // ==========================================
                IWorkbook workbook = null;
                bool isFormatValid = true;

                using (FileStream fs = new FileStream(saveFullPath, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(fs);

                    foreach (var rule in sheetRules)
                    {
                        string sheetName = rule.Key;
                        int targetRowIdx = rule.Value.TargetRow;
                        string[] expectedCols = rule.Value.Cols;

                        // A. 檢查頁籤
                        ISheet sheet = workbook.GetSheet(sheetName);
                        if (sheet == null)
                        {
                            isFormatValid = false;
                            // System.Diagnostics.Debug.WriteLine($"缺少頁籤: {sheetName}");
                            break;
                        }

                        // B. 檢查標題列
                        IRow targetRow = sheet.GetRow(targetRowIdx);
                        if (targetRow == null)
                        {
                            isFormatValid = false;
                            break;
                        }

                        int descRowIndex = targetRowIdx + 1;
                        IRow descRow = sheet.GetRow(descRowIndex);

                        if (descRow == null)
                        {
                            // 如果連下一列都沒有，代表使用者把說明列刪了，或檔案不完整
                            isFormatValid = false;
                            // Debug: System.Diagnostics.Debug.WriteLine($"頁籤[{sheetName}] 缺少說明列");
                            break;
                        }
                        ICell descCell = descRow.GetCell(0);
                        string descText = (descCell == null) ? "" : descCell.ToString();
                        if (!descText.Contains("填寫說明") && !descText.Contains("調查記數"))
                        {
                            isFormatValid = false;
                            // Debug: System.Diagnostics.Debug.WriteLine($"頁籤[{sheetName}] 說明列驗證失敗，內容為: {descText}");
                            break;
                        }

                        // C. 檢查欄位 (包含往上查找邏輯)
                        for (int i = 0; i < expectedCols.Length; i++)
                        {
                            string actualValue = "";

                            // 往上查找 Loop (從 TargetRow 往上找到 Row 0)
                            for (int r = targetRowIdx; r >= 0; r--)
                            {
                                IRow currentRow = sheet.GetRow(r);
                                if (currentRow != null)
                                {
                                    ICell cell = currentRow.GetCell(i);
                                    // 讀取值並去除空白、換行
                                    string val = (cell == null) ? "" : cell.ToString().Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
                                    if (!string.IsNullOrEmpty(val))
                                    {
                                        actualValue = val;
                                        break; // 找到有字的就停止
                                    }
                                }
                            }

                            // 為了比對方便，預期字串也做同樣的正規化處理
                            string expectedRaw = expectedCols[i].Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");

                            if (!actualValue.Equals(expectedRaw, StringComparison.OrdinalIgnoreCase))
                            {
                                isFormatValid = false;
                                // Debug 用：印出哪一欄錯了
                                // System.Diagnostics.Debug.WriteLine($"頁籤[{sheetName}] 第{i+1}欄錯誤: 預期[{expectedRaw}] 實際[{actualValue}]");
                                break;
                            }
                        }

                        if (!isFormatValid) break;
                    }
                }

                // ==========================================
                // 檢查欄位結果處理
                // ==========================================
                if (!isFormatValid)
                {
                    // --- 修改建議：不要刪除檔案，改為註解掉 ---
                    /* try
                    {
                        if (workbook != null) workbook.Close();
                        File.Delete(saveFullPath); // 建議保留，方便日後查修
                    }
                    catch { }
                    */

                    if (workbook != null) workbook.Close();
                    ShowMessage("提示", "EXCEL欄位格式錯誤，請檢查EXCEL欄位");
                    return;
                }

                //準備匯入資料

                // 所有的 Log 
                List<TreeBatchTaskLog> allLogList = new List<TreeBatchTaskLog>();

                List<ImportTreeDataModel> validDataList = new List<ImportTreeDataModel>();

                // 業務邏輯旗標
                bool isSetFinal = CheckBox_SetFinal.Checked;   // 定稿
                bool isOverwrite = CheckBox_Overwrite.Checked; // 覆蓋
                bool isStrictMode = isSetFinal || isOverwrite; // 必填檢查

                // 用來確保同一份檔案中，不會出現兩筆「同樹號 + 同日期」
                HashSet<string> internalDuplicateCheck = new HashSet<string>();

                using (FileStream fs = new FileStream(saveFullPath, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(fs);

                    //檢查其他頁籤的資料
                    var pestMap = ScanSideSheet(workbook, "病蟲害調查", 3, 0, allLogList);
                    var appearanceMap = ScanSideSheet(workbook, "樹木生長外觀情況", 3, 0, allLogList);
                    var pruningMap = ScanSideSheet(workbook, "樹木修剪與支撐情況", 3, 0, allLogList);
                    var soilMap = ScanSideSheet(workbook, "生育地環境與土讓檢測情況", 3, 0, allLogList);
                    var riskMap = ScanSideSheet(workbook, "健康檢查結果及風險評估", 2, 0, allLogList);

                    // =========================================================================
                    // 讀取 [基本資料]
                    // =========================================================================
                    ISheet mainSheet = workbook.GetSheet("基本資料");

                    // 資料從 Row 2 開始 (Row 0=標題, Row 1=說明)
                    for (int r = 2; r <= mainSheet.LastRowNum; r++)
                    {
                        IRow row = mainSheet.GetRow(r);
                        if (row == null) continue;

                        // 取得 Key 
                        string linkKey = GetCellValue(row, 0); // 調查記數 (Col 0)
                        string treeNo = GetCellValue(row, 1);  // 樹籍編號 (Col 1)

                        // 全欄位掃描 (判斷這行是不是真的有資料)
                        bool hasData = false;
                        for (int c = 0; c < row.LastCellNum; c++)
                        {
                            if (!string.IsNullOrEmpty(GetCellValue(row, c)))
                            {
                                hasData = true;
                                break;
                            }
                        }
                        // 空行與漏填 Key 判斷
                        if (string.IsNullOrEmpty(linkKey))
                        {
                            // 如果沒記數，但這行有其他資料 
                            if (hasData)
                            {
                                allLogList.Add(new TreeBatchTaskLog
                                {
                                    taskID = 0,
                                    sourceItem = $"基本資料-第{r + 1}列",
                                    refKey = treeNo,
                                    isSuccess = false,
                                    resultMsg = "失敗：資料提供不全 (調查記數為必填)"
                                });
                            }
                            continue; // 跳過無法處理的行
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

                        // =================================================================
                        // 欄位驗證
                        // =================================================================
                        List<string> rowErrors = new List<string>();

                        BasicInfoParseResult basicResult = ValidateBasicInfo(row, isStrictMode);

                        // 收集基本資料錯誤
                        if (basicResult.errors.Count > 0)
                        {
                            rowErrors.AddRange(basicResult.errors);
                        }

                        // 設定 Log 日期 (如果有解出來的話)
                        if (basicResult.surveyDate.HasValue)
                        {
                            myLog.refDate = basicResult.surveyDate.Value;
                        }

                        TreeHealthRecord draftRecord = basicResult.parsedData;

                        IRow pestRow = GetAndRemove(pestMap, linkKey);
                        IRow appRow = GetAndRemove(appearanceMap, linkKey);
                        IRow prunRow = GetAndRemove(pruningMap, linkKey);
                        IRow soilRow = GetAndRemove(soilMap, linkKey);
                        IRow riskRow = GetAndRemove(riskMap, linkKey);
                        if (pestRow != null)
                        {
                            //  一致性檢查 (確認樹號沒貼錯)
                            CheckTreeNoConsistency(treeNo, pestRow, "病蟲害調查", rowErrors);

                            // 解析 + 驗證 + 填入
                            List<string> pestErrors = ParseAndValidatePestInfo(pestRow, draftRecord, isStrictMode);

                            if (pestErrors.Count > 0) rowErrors.AddRange(pestErrors);
                        }

                        // --- 外觀 ---
                        if (appRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, appRow, "樹木生長外觀情況", rowErrors);
                            List<string> appErrors = ParseAndValidateAppearance(appRow, draftRecord, isStrictMode);
                            if (appErrors.Count > 0) rowErrors.AddRange(appErrors);
                        }

                        // --- 修剪 ---
                        if (prunRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, prunRow, "樹木修剪與支撐情況", rowErrors);
                            List<string> prunErrors = ParseAndValidatePruning(prunRow, draftRecord, isStrictMode);
                            if (prunErrors.Count > 0) rowErrors.AddRange(prunErrors);
                        }

                        // --- 生育地 ---
                        if (soilRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, soilRow, "生育地環境與土讓檢測情況", rowErrors);
                            List<string> soilErrors = ParseAndValidateSoil(soilRow, draftRecord, isStrictMode);
                            if (soilErrors.Count > 0) rowErrors.AddRange(soilErrors);
                        }

                        // --- 風險 ---
                        if (riskRow != null)
                        {
                            CheckTreeNoConsistency(treeNo, riskRow, "健康檢查結果及風險評估", rowErrors);

                            List<string> riskErrors = ParseAndValidateRisk(riskRow, draftRecord, isStrictMode);
                            if (riskErrors.Count > 0) rowErrors.AddRange(riskErrors);
                        }

                        
                        if (rowErrors.Count == 0)
                        {
                            //重複資料檢查
                            // 組合唯一鍵值：樹號 + 日期
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

                        if (rowErrors.Count > 0)
                        {
                            // 失敗：串接所有錯誤訊息
                            myLog.resultMsg = "失敗：資料提供不全 (" + string.Join("、", rowErrors) + ")";
                            myLog.isSuccess = false;
                        }
                        else
                        {
                            // 成功：標記 Log 為成功
                            myLog.isSuccess = true;
                            myLog.resultMsg = "";

                            // 成功後，將 draftRecord 的狀態設好
                            // (注意：draftRecord 裡面的資料在前面各步驟都已經填好了)
                            draftRecord.dataStatus = isSetFinal ? 1 : 0;

                            // 加入「待匯入清單」
                            validDataList.Add(new ImportTreeDataModel
                            {
                                log = myLog,
                                record = draftRecord
                            });
                        }

                    }

                    //其他頁籤沒有對應到的資料檢查
                    ProcessOrphans(pestMap, "病蟲害調查", allLogList);
                    ProcessOrphans(appearanceMap, "樹木生長外觀情況", allLogList);
                    ProcessOrphans(pruningMap, "樹木修剪與支撐情況", allLogList);
                    ProcessOrphans(soilMap, "生育地環境與土讓檢測情況", allLogList);
                    ProcessOrphans(riskMap, "健康檢查結果及風險評估", allLogList);
                }

                if (allLogList.Count == 0)
                {
                    ShowMessage("提示", "檔案中沒有讀取到任何資料列。");
                }

                if (validDataList.Count > 0)
                {

                    List<string> distinctTreeNos = validDataList
                        .Select(x => x.record.systemTreeNo)
                        .Distinct()
                        .ToList();

                    Dictionary<string, int> treeIdMap = system_batch.GetTreeIDMap(distinctTreeNos);

                    // 倒序迴圈
                    for (int i = validDataList.Count - 1; i >= 0; i--)
                    {
                        var item = validDataList[i];
                        string tNo = item.record.systemTreeNo;

                        if (treeIdMap.ContainsKey(tNo))
                        {
                            // 查到了，填入 TreeID
                            item.record.treeID = treeIdMap[tNo];
                        }
                        else
                        {
                            // 查無此樹
                            item.log.isSuccess = false;
                            item.log.resultMsg = $"失敗：查無系統樹籍編號 ({tNo})";

                            // 從待存檔清單中移除
                            validDataList.RemoveAt(i);
                        }
                    }

                    if (validDataList.Count > 0)
                    {
                        // 檢查有無健檢資料
                        var queryKeys = validDataList
                            .Select(x => new TreeQueryKey
                            {
                                treeID = x.record.treeID,
                                checkDate = x.record.surveyDate.Value
                            })
                            .Distinct() 
                            .ToList();

                        // 2. 呼叫 Service 取得 Map (Key format: $"{TreeID}_{yyyyMMdd}")
                        Dictionary<string, int> healthMap = system_batch.GetHealthIDMap(queryKeys);

                        // 3. 再次倒序迴圈檢查重複與覆蓋邏輯
                        for (int i = validDataList.Count - 1; i >= 0; i--)
                        {
                            var item = validDataList[i];

                            // 組合 Key 來比對 (與 Service 回傳的 Key 格式要一致)
                            string key = $"{item.record.treeID}_{item.record.surveyDate:yyyyMMdd}";

                            if (healthMap.ContainsKey(key))
                            {
                                int oldHealthID = healthMap[key];

                                if (isOverwrite)
                                {
                                    // 情況 1: 勾選覆蓋 -> 標記為 Update (填入舊 ID)
                                    item.record.healthID = oldHealthID;

                                    // 提示訊息
                                    item.log.resultMsg += "提醒：已覆蓋同日調查資料";
                                }
                                else
                                {
                                    // 情況 2: 未勾選覆蓋 -> 視為失敗
                                    item.log.isSuccess = false;
                                    item.log.resultMsg = "失敗：同日已有紀錄但未覆蓋";

                                    // 踢除，不存檔
                                    validDataList.RemoveAt(i);
                                }
                            }
                            
                        }
                    }

                    if (validDataList.Count > 0)
                    {
                        // 分流 Insert 和 Update
                        var updateList = validDataList.Where(x => x.record.healthID > 0).Select(x => x.record).ToList();
                        var insertList = validDataList.Where(x => x.record.healthID == 0).Select(x => x.record).ToList();

                        // 執行更新 (Update)
                        if (updateList.Count > 0)
                        {
                            foreach (var rec in updateList)
                            {
                                rec.updateAccountID = accountID;
                                rec.updateDateTime = DateTime.Now;
                                // 如果有勾定稿，物件狀態設為 1；沒勾則保持 0 
                                if (isSetFinal) rec.dataStatus = 1;
                            }

                            // 如果 isSetFinal 為 false，Update 時就會自動忽略 dataStatus 欄位
                            system_batch.BulkUpdateHealthRecords(updateList, updateStatus: isSetFinal);
                        }

                        // 執行新增
                        if (insertList.Count > 0)
                        {
                            foreach (var rec in insertList)
                            {
                                rec.insertAccountID = accountID;
                                rec.insertDateTime = DateTime.Now;
                            }
                            system_batch.BulkInsertHealthRecords(insertList);
                        }
                    }
                }

                int successCount = allLogList.Count(x => x.isSuccess);
                int failCount = allLogList.Count(x => !x.isSuccess);
                int totalCount = allLogList.Count;

                int newBatchTaskID = system_batch.CreateBatchTask(enum_treeBatchType.Health_Record,fileName, accountID, totalCount,successCount,failCount);

                // 回填 TaskID 到每一筆 Log 明細，並寫入 DB
                foreach (var log in allLogList)
                {
                    log.taskID = newBatchTaskID;
                }

                system_batch.BulkInsertTaskLogs(allLogList);

                // 顯示結果
                ShowMessage("處理完成", $"成功：{successCount}，失敗：{failCount}");
                BindData();
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
        private BasicInfoParseResult ValidateBasicInfo(IRow row, bool isSetFinal)
        {
            var result = new BasicInfoParseResult();
            var errors = result.errors;
            var data = result.parsedData; 

            // ---------------------------------------------------------
            // 樹籍編號 (Col 1)
            // ---------------------------------------------------------
            string valTreeNo = GetCellValue(row, 1);
            if (string.IsNullOrEmpty(valTreeNo))
            {
                errors.Add("樹籍編號 (未填)");
            }
            else
            {
                data.systemTreeNo = valTreeNo;
            }

            // ---------------------------------------------------------
            // 調查日期 (Col 6)
            // ---------------------------------------------------------
            string valDate = GetCellValue(row, 6);

            if (string.IsNullOrEmpty(valDate))
            {
                errors.Add("調查日期 (未填)");
            }
            else
            {
                // 嘗試解析日期
                if (DateTime.TryParse(valDate, out DateTime d))
                {
                    // 自動校正民國年 
                    if (d.Year < 1911)
                    {
                        d = d.AddYears(1911);
                    }


                    data.surveyDate = d;
                    result.surveyDate = d; 
                }
                else
                {
                    errors.Add($"調查日期 (格式錯誤: {valDate})");
                }
            }

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
            record.rootMechanicalDamage = ParseBoolColumn(row, 6, "根系-機具損傷", errors);
            record.rootMowingInjury = ParseBoolColumn(row, 7, "根系-打草傷", errors);
            record.rootInjury = ParseBoolColumn(row, 8, "根系-根傷", errors);
            record.rootGirdling = ParseBoolColumn(row, 9, "根系-盤根", errors); // 對應到 rootGirdling

            // 備註
            record.rootOtherNote = GetCellValue(row, 10);

            // =========================================================================
            // 2. 樹基部 (Base) - Index 11~16
            // 標頭: 腐朽百分比%, 樹洞最大直徑(m), 傷口最大直徑(m), 機具損傷, 打草傷, 其他
            // =========================================================================
            record.baseDecayPercent = ParseDecimalColumn(row, 11, "樹基部-腐朽百分比%", isStrictMode, 0, 100, errors);
            record.baseCavityMaxDiameter = ParseDecimalColumn(row, 12, "樹基部-樹洞最大直徑(m)", false, 0, 9999.99m, errors);
            record.baseWoundMaxDiameter = ParseDecimalColumn(row, 13, "樹基部-傷口最大直徑(m)", false, 0, 9999.99m, errors);

            record.baseMechanicalDamage = ParseBoolColumn(row, 14, "樹基部-機具損傷", errors);
            record.baseMowingInjury = ParseBoolColumn(row, 15, "樹基部-打草傷", errors);

            record.baseOtherNote = GetCellValue(row, 16);

            // =========================================================================
            // 3. 主幹 (Trunk) - Index 17~22
            // 標頭: 腐朽百分比%, 樹洞最大直徑(m), 傷口最大直徑(m), 機具損傷, 內生夾皮, 其他
            // =========================================================================
            record.trunkDecayPercent = ParseDecimalColumn(row, 17, "主幹-腐朽百分比%", isStrictMode, 0, 100, errors);
            record.trunkCavityMaxDiameter = ParseDecimalColumn(row, 18, "主幹-樹洞最大直徑(m)", false, 0, 9999.99m, errors);
            record.trunkWoundMaxDiameter = ParseDecimalColumn(row, 19, "主幹-傷口最大直徑(m)", false, 0, 9999.99m, errors);

            record.trunkMechanicalDamage = ParseBoolColumn(row, 20, "主幹-機具損傷", errors);
            record.trunkIncludedBark = ParseBoolColumn(row, 21, "主幹-內生夾皮", errors);

            record.trunkOtherNote = GetCellValue(row, 22);

            // =========================================================================
            // 4. 枝幹 (Branch) - Index 23~29
            // 標頭: 腐朽百分比%, 樹洞最大直徑(m), 傷口最大直徑(m), 機具損傷, 內生夾皮, 下垂枝, 其他
            // =========================================================================
            record.branchDecayPercent = ParseDecimalColumn(row, 23, "枝幹-腐朽百分比%", isStrictMode, 0, 100, errors);
            record.branchCavityMaxDiameter = ParseDecimalColumn(row, 24, "枝幹-樹洞最大直徑(m)", false, 0, 9999.99m, errors);
            record.branchWoundMaxDiameter = ParseDecimalColumn(row, 25, "枝幹-傷口最大直徑(m)", false, 0, 9999.99m, errors);

            record.branchMechanicalDamage = ParseBoolColumn(row, 26, "枝幹-機具損傷", errors);
            record.branchIncludedBark = ParseBoolColumn(row, 27, "枝幹-內生夾皮", errors);
            record.branchDrooping = ParseBoolColumn(row, 28, "枝幹-下垂枝", errors);

            record.branchOtherNote = GetCellValue(row, 29);

            // =========================================================================
            // 5. 樹冠 (Crown) - Index 30~33
            // 標頭: 樹葉生長覆蓋度百分比%, 一般枯枝%, 懸掛枝, 其他
            // =========================================================================
            record.crownLeafCoveragePercent = ParseDecimalColumn(row, 30, "樹冠-樹葉生長覆蓋度百分比%", isStrictMode, 0, 100, errors);
            record.crownDeadBranchPercent = ParseDecimalColumn(row, 31, "樹冠-一般枯枝%", isStrictMode, 0, 100, errors);

            record.crownHangingBranch = ParseBoolColumn(row, 32, "樹冠-懸掛枝", errors);

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
                    errors.Add($"錯誤修剪傷害 (內容不合法: {valDamage})，請填寫：截幹、截頂、不當縮剪");
                }
            }
            // 若為 null 或空字串，視為無錯誤修剪，不報錯 (除非這是必填，看您需求)

            // B. 修剪傷口癒合 (Bool) - Index 4
            record.pruningWoundHealing = ParseBoolColumn(row, 4, "修剪傷口是否有癒合", errors);

            // C. 附生植物 (Bool) - Index 5
            record.pruningEpiphyte = ParseBoolColumn(row, 5, "樹幹附生植物", errors);

            // D. 寄生植物 (Bool) - Index 6
            record.pruningParasite = ParseBoolColumn(row, 6, "寄生植物情形", errors);

            // E. 纏勒性植物 (Bool) - Index 7
            record.pruningVine = ParseBoolColumn(row, 7, "蔓藤纏繞情形", errors);

            // F. 修剪其他說明 (String) - Index 8
            record.pruningOtherNote = GetCellValue(row, 8);

            // =========================================================================
            // 2. 支撐情況 (Support) - Index 9~11
            // =========================================================================

            // G. 支架數量 (Int) - Index 9
            // 假設範圍 0~100，若非嚴格模式則非必填
            record.supportCount = ParseIntColumn(row, 9, "支撐設施數量", false, 0, int.MaxValue, errors);

            // H. 支架嵌入 (Bool) - Index 10
            record.supportEmbedded = ParseBoolColumn(row, 10, "支撐設施嵌入樹幹", errors);

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
            record.sitePlanter = ParseBoolColumn(row, 5, "花台", errors);
            record.siteRecreationFacility = ParseBoolColumn(row, 6, "休憩設施(桌椅)", errors);
            record.siteDebrisStack = ParseBoolColumn(row, 7, "雜物堆置", errors);
            record.siteBetweenBuildings = ParseBoolColumn(row, 8, "受限建物之間", errors);
            record.siteSoilCompaction = ParseBoolColumn(row, 9, "土壤受踩踏夯實", errors);
            record.siteOverburiedSoil = ParseBoolColumn(row, 10, "覆土過深", errors);

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
                    errors.Add($"建議處理優先順序 (內容不合法: {valPriority})，請填寫：緊急處理、優先處理、例行養護");
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
        private bool? ParseBoolColumn(IRow row, int col, string fieldName, List<string> errors)
        {
            string val = GetCellValue(row, col);

            if (string.IsNullOrEmpty(val))
            {
                return null; // 沒填視為 Null
            }

            val = val.Trim(); // 去除空白

            if (val == "有") return true;
            if (val == "無") return false;

            // 嚴格限制只能填這兩個字
            errors.Add($"{fieldName} (內容不合法: {val})，請填寫：有/無");
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
                    sourceItem = $"{sheetName}-第{info.rowIndex + 1}列 (記數:{info.linkKey})",
                    refKey = orphanTreeNo,
                    isSuccess = false,
                    resultMsg = "失敗：無對應資料 (在基本資料頁籤中找不到對應的調查記數)"
                });
            }
        }
    }
}