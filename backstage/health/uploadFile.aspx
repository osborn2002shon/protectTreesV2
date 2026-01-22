<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="uploadFile.aspx.cs" Inherits="protectTreesV2.backstage.health.uploadFile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
       <style>

       /* 拖曳區樣式 */
       .photo-drop {
           border: 2px dashed #6c757d;
           border-radius: 6px;
           padding: 40px 20px;
           text-align: center;
           color: #6c757d;
           cursor: pointer;
           background-color: #fff;
           transition: all 0.2s;
       }

       .photo-drop:hover, .photo-drop.dragging {
           background-color: #f8f9fa; 
           border-color: #6c757d;     
       }

       /* 狀態提示區塊 */
       .upload-status-bar {
           background-color: #f8f9fa;
           border: 1px solid #dee2e6;
           border-radius: 6px;
           padding: 10px 15px;
           margin-top: 15px;
           display: flex;
           justify-content: space-between;
           align-items: center;
       }

       .gv-tb {
           max-height: 500px;  
           overflow-y: auto;   
           position: relative; 
       }

       .gv-tb .gv th {
           position: sticky;   
           top: 0;            
           z-index: 10;
           background-color: #fff; 
       }
   </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    健檢資料管理 / 健檢紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    上傳多筆健檢附件
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <nav class="nav nav-tabs mb-4">
        <a class="nav-link text-dark" href="main.aspx">健檢紀錄</a>
        <a class="nav-link text-dark" href="list.aspx">異動管理</a>
        <a class="nav-link text-dark" href="upload.aspx">上傳多筆健檢紀錄</a>
        <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆健檢照片</a>
        <a class="nav-link active" href="uploadFile.aspx">上傳多筆健檢附件</a>
    </nav>
    <div class="row mb-4">
        <div class="col-12">
            <div class="card border-dark">
                <div class="card-header bg-white fw-bold border-bottom-0 pt-3">
                    上傳說明
                </div>
                <div class="card-body pt-0">
                    <p class="card-text">
                        請依照系統命名規則重新命名文件檔案後上傳，系統將自動解析檔名並關聯至對應的調查資料。
                    </p>

                    <p class="fw-bold mb-1">命名規則與範例：</p>
                    <ul>
                        <li>
                            <strong>命名規則：</strong>系統樹籍編號_調查日期_xxxx.zip
                            <br /><span class="text-muted small">（xxxx 可為任意流水號或自訂註記）</span>
                        </li>
                        <li>
                            <strong>範例說明：</strong>
                            若要上傳「系統樹籍編號 A0001」於「2025年1月1日」的文件，請將檔案命名為 <code>A0001_20250101_001.zip</code>。
                        </li>
                    </ul>

                    <p class="fw-bold mb-1">勾選狀態說明：</p>
                    <ul>
                        <li>
                            <strong>勾選「指定日期若無紀錄則自動新增一筆（草稿）」：</strong>
                            若系統中查無該「樹籍編號」與「調查日期」的對應紀錄，系統將自動建立一筆新的草稿資料以利歸檔文件。
                        </li>
                        <li>
                            <strong>勾選「若附件已存在則取代已上傳之附件」：</strong>
                            若該筆紀錄已存在附件，系統將直接以新檔案取代舊檔；若未勾選此項且紀錄中已有附件，系統將會阻擋該筆上傳。
                        </li>
                    </ul>

                    <p class="fw-bold mb-1">檔案格式限制：</p>
                    <ul>
                        <li>格式限制：僅接受 .zip 格式。</li>
                        <li>大小限制：單一檔案大小限制 30MB。</li>
                        <li>
                            <strong>數量限制：</strong>每筆健檢紀錄只會有一個附件。
                            <br />
                            <span class="text-muted small">（若該筆紀錄已存在附件且未勾選「覆蓋」，系統將阻擋上傳）</span>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-12">
       
            <%-- 1. 拖曳上傳區 --%>
            <div id="batchDropArea" class="photo-drop mb-3" title="點擊選取或拖曳檔案">
                <div class="mb-2">
                    <i class="fa-solid fa-cloud-arrow-up fa-3x"></i>
                </div>
                <div class="fs-5 fw-bold">點擊選取 或 將文件拖曳至此</div>
                <div class="small mt-1 text-muted">
                    支援多檔選取 (ZIP)<br/>
                    <span class="text-danger">※ 單一檔案限制 30MB，若超過系統將自動阻擋</span>
                </div>
                <%-- 只接受 zip --%>
                <asp:FileUpload ID="FileUpload_Batch" runat="server" AllowMultiple="true" CssClass="d-none" accept=".zip,application/zip,application/x-zip-compressed" />
            </div>

            <div id="statusBar" class="text-center">
                <div class="d-flex justify-content-center align-items-center mb-3">
                    <i id="statusIcon" class="fa-solid fa-circle-info me-2 text-secondary"></i>
                    <span id="statusText" class="fw-bold text-dark">尚未選取檔案</span>
                    <span id="sizeText" class="text-muted small ms-2"></span>
                </div>
                <div class="d-flex justify-content-center flex-wrap gap-3 mb-3">
                        <%-- 選項 1: 自動新增草稿 --%>
                        <asp:CheckBox ID="CheckBox_autoCreateDraft" runat="server" 
                            Text="指定日期若無紀錄則自動新增一筆（草稿）" 
                            CssClass="fw-bold text-dark text-nowrap" 
                            Style="cursor: pointer;" />

                        <%-- 選項 2: 覆蓋舊檔 --%>
                        <asp:CheckBox ID="CheckBox_OverwriteExisting" runat="server" 
                            Text="若附件已存在則取代已上傳之附件" 
                            CssClass="fw-bold text-dark text-nowrap" 
                            Style="cursor: pointer;" />

                    </div>
                <div>
                    <asp:Button ID="Button_StartUpload" runat="server" Text="上傳文件" 
                        CssClass="btn btn-primary disabled" 
                        OnClick="Button_StartUpload_Click" 
                        ClientIDMode="Static" />
                </div>
            </div>

        </div>
    </div>

    <hr class="my-5" />

    <div class="row mb-4">
        <div class="col-12">
            <div class="card shadow-sm">
                <div class="card-header bg-white py-3">
                    <div class="d-flex justify-content-between align-items-center">
                        <h5 class="m-0 fw-bold">
                            <i class="fa-solid fa-list-check me-2"></i>最新上傳結果明細
                        </h5>
                        <asp:Label ID="Label_LastStatus" runat="server" CssClass="badge bg-secondary rounded-pill fs-6"></asp:Label>
                    </div>
                </div>
                <div class="card-body p-0">
                    <div class="table-responsive gv-tb">
                        <asp:GridView ID="GridView_Detail" runat="server" 
                            CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" UseAccessibleHeader="true" OnRowCommand="GridView_Detail_RowCommand">
                       
                            <Columns>
                                <%-- 狀態圖示 --%>
                                <asp:TemplateField HeaderText="狀態" ItemStyle-Width="80px">
                                    <ItemTemplate>
                                        <%# (bool)Eval("isSuccess") 
                                            ? "<i class='fa-solid fa-circle-check text-success fs-5'></i>" 
                                            : "<i class='fa-solid fa-circle-xmark text-danger fs-5'></i>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- 樹籍編號 (成功顯示連結，失敗顯示純文字) --%>
                                 <asp:TemplateField HeaderText="樹籍編號" ItemStyle-Width="120px">
                                     <ItemTemplate>
                                         <%-- 成功 -> 顯示 LinkButton --%>
                                         <asp:LinkButton ID="btnTreeNo" runat="server" 
                                             Text='<%# Eval("refKey") %>'
                                             CommandName="ViewTree" 
                                             CommandArgument='<%# Eval("refKey") %>'
                                             CssClass="text-decoration-underline text-primary"
                                             Visible='<%# (bool)Eval("isSuccess") %>'>
                                         </asp:LinkButton>

                                         <%-- 失敗 -> 顯示 Label (純文字) --%>
                                         <asp:Label ID="lblTreeNo" runat="server" 
                                             Text='<%# Eval("refKey") %>'
                                             Visible='<%# !(bool)Eval("isSuccess") %>'>
                                         </asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>

                                 <%-- 調查日期  --%>
                                 <asp:TemplateField HeaderText="調查日期" ItemStyle-Width="120px">
                                     <ItemTemplate>
                                         <%--  成功 -> 顯示 LinkButton --%>
                                         <asp:LinkButton ID="btnSurveyDate" runat="server" 
                                             Text='<%# Eval("refDate", "{0:yyyy/MM/dd}") %>'
                                             CommandName="ViewHealth" 
                                             CommandArgument='<%# Eval("refKey") + "," + Eval("refDate", "{0:yyyy/MM/dd}") %>'
                                             CssClass="text-decoration-underline text-primary"
                                             Visible='<%# (bool)Eval("isSuccess") %>'>
                                         </asp:LinkButton>

                                         <%--  失敗 -> 顯示 Label (純文字) --%>
                                         <asp:Label ID="lblSurveyDate" runat="server" 
                                             Text='<%# Eval("refDate", "{0:yyyy/MM/dd}") %>'
                                             Visible='<%# !(bool)Eval("isSuccess") %>'>
                                         </asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>

                                <%-- 來源項目 (檔名) --%>
                                <asp:BoundField DataField="sourceItem" HeaderText="檔案名稱" />

                                <%-- 處理結果訊息 (失敗原因) --%>
                                <asp:TemplateField HeaderText="處理結果">
                                    <ItemTemplate>
                                        <span class='<%# (bool)Eval("isSuccess") ? "text-success" : "text-danger fw-bold" %>'>
                                            <%# Eval("resultMsg") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <EmptyDataTemplate>
                                <div class="text-center py-4 text-muted">
                                    尚無上傳明細資料。
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-12">
            <div class="card shadow-sm">
                <div class="card-header bg-white py-3">
                    <h5 class="m-0 fw-bold text-dark">
                        <i class="fa-solid fa-clock-rotate-left me-2"></i>歷史上傳紀錄（近五筆）
                    </h5>
                </div>
                <div class="card-body p-0">
                    <div class="table-responsive gv-tb">
                        <asp:GridView ID="GridView_History" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                            <Columns>
                                <%-- 1. 上傳時間 --%>
                                <asp:BoundField DataField="insertDateTime" HeaderText="上傳時間" 
                                    DataFormatString="{0:yyyy/MM/dd HH:mm}" ItemStyle-Width="160px" />

                                <%-- 2. 上傳者 --%>
                                <asp:BoundField DataField="userName" HeaderText="上傳者" ItemStyle-Width="200px" />

                                <%-- 3. 總筆數  --%>
                                <asp:TemplateField HeaderText="總筆數" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <span class="badge bg-secondary fs-6"><%# Eval("totalCount") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- 4. 成功筆數 --%>
                                <asp:TemplateField HeaderText="成功" ItemStyle-Width="120px" >
                                    <ItemTemplate>
                                        <span class="badge bg-success fs-6"><%# Eval("successCount") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- 5. 失敗筆數  --%>
                                <asp:TemplateField HeaderText="失敗" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <%-- 如果失敗數大於0，顯示紅色，否則顯示淺灰色或隱藏 --%>
                                        <span class='<%# Convert.ToInt32(Eval("failCount")) > 0 ? "badge bg-danger fs-6" : "badge bg-light text-secondary border fs-6" %>'>
                                            <%# Eval("failCount") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center py-4 text-muted">
                                    目前尚無歷史上傳紀錄。
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
    $(document).ready(function () {
        // --- 變數 ---
        const $dropArea = $('#batchDropArea');
        const $fileInput = $('#<%= FileUpload_Batch.ClientID %>');
       
        // UI 元件
        const $statusText = $('#statusText');
        const $sizeText = $('#sizeText');
        const $statusIcon = $('#statusIcon');
        const $btnUpload = $('#Button_StartUpload');
       
        // 修改：限制改為 30MB
        const maxFileSize = 30 * 1024 * 1024; // 30MB

        // --- 事件監聽 ---
        $fileInput.on('click', function (e) {
            e.stopPropagation();
        });
        // 1. 點擊區塊 -> 觸發 File Input
        $dropArea.on('click', function () {
            // 點擊後重新選取，自然會覆蓋舊的，不需額外清除
            $fileInput.trigger('click');
        });

        // 2. 檔案選擇後處理
        $fileInput.on('change', function () {
            validateAndSetFiles(this.files);
        });

        // 3. 拖曳效果
        $dropArea.on('dragover', function (e) {
            e.preventDefault(); e.stopPropagation();
            $(this).addClass('dragging');
        });
        $dropArea.on('dragleave', function (e) {
            e.preventDefault(); e.stopPropagation();
            $(this).removeClass('dragging');
        });
       
        // 4. 拖曳放下 (Drop)
        $dropArea.on('drop', function (e) {
            e.preventDefault(); e.stopPropagation();
            $(this).removeClass('dragging');
           
            const dt = e.originalEvent.dataTransfer;
            if (dt.files && dt.files.length > 0) {
                if (validateAndSetFiles(dt.files)) {
                    $fileInput[0].files = dt.files;
                }
            }
        });

        // --- 邏輯函數 ---

        function clearAll() {
            $fileInput.val(''); 
            updateUI([]); 
        }

        function validateAndSetFiles(files) {
            if (!files || files.length === 0) {
                clearAll();
                return false;
            }

            for (let i = 0; i < files.length; i++) {
                // 檢查檔案大小
                if (files[i].size > maxFileSize) {
                    alert(`檔案「${files[i].name}」超過 30MB！\n系統已自動阻擋，請重新選取符合大小的檔案。`);
                    clearAll();
                    return false;
                }
                // 可以在這裡加一個檢查副檔名的邏輯，但為了不更動太多邏輯，目前主要依賴 accept 屬性
            }

            updateUI(files);
            return true;
        }

        function updateUI(files) {
            const count = files ? files.length : 0;
           
            if (count > 0) {
                let totalBytes = 0;
                for (let i = 0; i < count; i++) {
                    totalBytes += files[i].size;
                }
                const sizeMB = (totalBytes / (1024 * 1024)).toFixed(2);

                $statusText.text(`已選取 ${count} 個檔案`);
                $sizeText.text(`(共 ${sizeMB} MB)`);
               
                // 變更圖示
                $statusIcon.removeClass('fa-circle-info text-secondary').addClass('fa-check-circle text-success');
                // 啟用按鈕
                $btnUpload.removeClass('disabled');
            } else {
                $statusText.text('尚未選取檔案');
                $sizeText.text('');
               
                // 重置圖示
                $statusIcon.removeClass('fa-check-circle text-success').addClass('fa-circle-info text-secondary');
                // 禁用按鈕
                $btnUpload.addClass('disabled');
            }
        }
    });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
