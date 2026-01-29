<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="upload.aspx.cs" Inherits="protectTreesV2.backstage.care.upload" %>
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
    養護資料管理：上傳多筆養護紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    上傳多筆養護紀錄
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
     <nav class="nav nav-tabs mb-4">
         <a class="nav-link text-dark" href="main.aspx">養護作業</a>
         <a class="nav-link text-dark" href="list.aspx">異動管理</a>
         <a class="nav-link active" href="upload.aspx">上傳多筆養護紀錄</a>
         <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆養護照片</a>
     </nav>
     <div class="row mb-4">
        <div class="col-12">
            <div class="card border-dark">
                <div class="card-header bg-white fw-bold border-bottom-0 pt-3">
                    上傳說明
                </div>
                <div class="card-body pt-0">
                    <p class="card-text">
                        請上傳填寫完成的 Excel 調查表，系統將自動解析內容。
                    </p>

                    <p class="fw-bold mb-1">勾選狀態說明：</p>
                    <ul>
                        <li>
                            <strong>未勾選任何選項（草稿模式）：</strong>
                            Excel 內僅需填寫「系統樹籍編號」與「養護日期」，其餘欄位可留空。
                        </li>
                        
                        <li>
                            <strong>勾選「若有同日資料直接覆蓋」：</strong>
                            視同正式紀錄（必填欄位不可漏填），若該日期已有資料，系統將直接覆蓋舊有紀錄。
                        </li>
                    </ul>

                    <p class="fw-bold mb-1">檔案格式限制：</p>
                    <ul>
                        <li>格式限制：僅接受 .xls 或 .xlsx 格式。</li>
                        <li>大小限制：單一檔案大小限制 30MB。</li>
                    </ul>

                    <hr class="mt-3 mb-3"/>

                    <div class="text-muted small">
                        <strong>資料覆蓋原則：</strong>
                        一個「系統樹籍編號」在同一「養護日期」下，僅會存在一筆養護紀錄。
                        系統將以本次 Excel 內容為主，一旦上傳成功，新資料將直接覆蓋系統內原有的欄位數值。
                    </div>

                </div>
            </div>
        </div>
    </div>

     <div class="row">
         <div class="col-12">

             <%-- 1. 拖曳上傳區 --%>
             <div id="batchDropArea" class="photo-drop mb-3" title="點擊選取或拖曳檔案">
                 <div class="mb-2">
                     <%-- 改成 Excel 圖示 --%>
                     <i class="fa-solid fa-file-excel fa-3x"></i>
                 </div>
                 <div class="fs-5 fw-bold">點擊選取 或 將 Excel 檔拖曳至此</div>
                 <div class="small mt-1 text-muted">
                     支援格式：.xls, .xlsx<br />
                     <span class="text-danger">※ 單一檔案限制 30MB，若超過系統將自動阻擋</span>
                 </div>
                 <%-- Accept 改為 Excel 格式 --%>
                 <asp:FileUpload ID="FileUpload_Batch" runat="server" AllowMultiple="false" CssClass="d-none" accept=".xls,.xlsx" />
             </div>

             <div id="statusBar" class="text-center">
                 <div class="d-flex justify-content-center align-items-center mb-3">
                     <i id="statusIcon" class="fa-solid fa-circle-info me-2 text-secondary"></i>
                     <span id="statusText" class="fw-bold text-dark">尚未選取檔案</span>
                     <span id="sizeText" class="text-muted small ms-2"></span>
                 </div>

                 <%-- CheckBox 區域 --%>
                 <div class="d-flex justify-content-center gap-3 mb-3">
                     <asp:CheckBox ID="CheckBox_Overwrite" runat="server" Text="若有同日資料直接覆蓋" 
                         CssClass="fw-bold text-dark" Style="cursor: pointer;" />
                 </div>

                 <div>
                     <asp:Button ID="Button_StartUpload" runat="server" Text="上傳檔案" 
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
                             <i class="fa-solid fa-list-check me-2"></i>最新處理結果明細
                         </h5>
                         <asp:Label ID="Label_LastStatus" runat="server" CssClass="badge bg-secondary rounded-pill fs-6"></asp:Label>
                     </div>
                 </div>
                 <div class="card-body p-0">
                     <div class="table-responsive gv-tb">
                         <asp:GridView ID="GridView_Detail" runat="server" 
                             CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" UseAccessibleHeader="true" OnRowCommand="GridView_Detail_RowCommand">
                             <Columns>
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

                                 <%-- 養護日期  --%>
                                 <asp:TemplateField HeaderText="養護日期" ItemStyle-Width="120px">
                                     <ItemTemplate>
                                         <%--  成功 -> 顯示 LinkButton --%>
                                         <asp:LinkButton ID="btnSurveyDate" runat="server" 
                                             Text='<%# Eval("refDate", "{0:yyyy/MM/dd}") %>'
                                             CommandName="ViewCare" 
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
                                 <asp:BoundField DataField="sourceItem" HeaderText="所在行數" />
                                 <asp:TemplateField HeaderText="處理結果">
                                     <ItemTemplate>
                                         <span class='<%# (bool)Eval("isSuccess") ? "text-success" : "text-danger fw-bold" %>'>
                                             <%# Eval("resultMsg") %>
                                         </span>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                             </Columns>
                             <EmptyDataTemplate>
                                 <div class="text-center py-4 text-muted">尚無上傳明細資料。</div>
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
                         <i class="fa-solid fa-clock-rotate-left me-2"></i>歷史匯入紀錄（近五筆）
                     </h5>
                 </div>
                 <div class="card-body p-0">
                     <div class="table-responsive gv-tb">
                         <asp:GridView ID="GridView_History" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                             <Columns>
                                 <asp:BoundField DataField="insertDateTime" HeaderText="上傳時間" DataFormatString="{0:yyyy/MM/dd HH:mm}" ItemStyle-Width="160px" />
                                 <asp:BoundField DataField="userName" HeaderText="上傳者" ItemStyle-Width="200px" />
                                 <asp:TemplateField HeaderText="總筆數" ItemStyle-Width="120px">
                                     <ItemTemplate><span class="badge bg-secondary fs-6"><%# Eval("totalCount") %></span></ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField HeaderText="成功" ItemStyle-Width="120px" >
                                     <ItemTemplate><span class="badge bg-success fs-6"><%# Eval("successCount") %></span></ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField HeaderText="失敗" ItemStyle-Width="120px">
                                     <ItemTemplate>
                                         <span class='<%# Convert.ToInt32(Eval("failCount")) > 0 ? "badge bg-danger fs-6" : "badge bg-light text-secondary border fs-6" %>'>
                                             <%# Eval("failCount") %>
                                         </span>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                             </Columns>
                             <EmptyDataTemplate>
                                 <div class="text-center py-4 text-muted">目前尚無歷史上傳紀錄。</div>
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

            // 30MB
            const maxFileSize = 30 * 1024 * 1024;

            // --- 事件監聽 ---
            $fileInput.on('click', function (e) {
                e.stopPropagation();
            });

            // 1. 點擊區塊
            $dropArea.on('click', function () {
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
                    const file = files[i];
                    const lowerName = file.name.toLowerCase();

                    // 檢查副檔名 (改為 xls, xlsx)
                    if (!lowerName.endsWith('.xls') && !lowerName.endsWith('.xlsx')) {
                        alert(`檔案「${file.name}」格式錯誤！\n僅接受 .xls 或 .xlsx 格式的 Excel 檔案。`);
                        clearAll();
                        return false;
                    }

                    // 檢查大小 (30MB)
                    if (file.size > maxFileSize) {
                        alert(`檔案「${file.name}」超過 30MB！\n系統已自動阻擋，請確認檔案內容是否過大。`);
                        clearAll();
                        return false;
                    }
                }

                updateUI(files);
                return true;
            }

            function updateUI(files) {
                const count = files ? files.length : 0;

                if (count > 0) {
                    const file = files[0];
                    const sizeMB = (file.size / (1024 * 1024)).toFixed(2);

                    $statusText.text(`已選取檔案：${file.name}`);
                    $sizeText.text(`(${sizeMB} MB)`);

                    // 變更圖示 (Excel)
                    $statusIcon.removeClass('fa-circle-info text-secondary').addClass('fa-file-excel text-success');
                    // 啟用按鈕
                    $btnUpload.removeClass('disabled');
                } else {
                    $statusText.text('尚未選取檔案');
                    $sizeText.text('');

                    // 重置圖示
                    $statusIcon.removeClass('fa-file-excel text-success').addClass('fa-circle-info text-secondary');
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
