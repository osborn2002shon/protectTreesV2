<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="protectTreesV2.backstage.health.edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    健檢資料管理 / 健檢紀錄 / <asp:Literal ID="Literal_pathAction" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    <asp:Literal ID="Literal_title" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="card mb-4 shadow-sm">
        <div class="card-header bg-light">
            <h5 class="mb-0">樹籍資訊</h5>
        </div>
        <div class="card-body">
            <div class="row g-3">
                <%-- 第一列：樹號、樹種、地點 --%>
                <div class="col-md-3">
                    <label class="form-label text-muted">系統樹籍編號</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_systemTreeNo" runat="server" Text="--"></asp:Label>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label text-muted">樹種</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_speciesName" runat="server" Text="--"></asp:Label>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label text-muted">座落地點 (縣市/鄉鎮)</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_cityName" runat="server" Text="--"></asp:Label>
                        <asp:Label ID="Label_areaName" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label text-muted">管理人</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_manager" runat="server" Text="--"></asp:Label>
                    </div>
                </div>

                <%-- 分隔線 --%>
                <div class="col-12">
                    <hr class="my-2" />
                </div>

                <%-- 第二列：資料狀態 --%>
                <div class="col-12">
                    <div class="d-flex align-items-center">
                        <span class="me-2 text-muted">本筆資料編輯狀態：</span>
                        
                        <%-- 這裡用 Label，後端可以根據狀態動態改變 CssClass (例如 badge bg-primary) --%>
                        <h3>
                            <asp:Label ID="Label_recordStatus" runat="server" CssClass="badge bg-secondary" Text="新增"></asp:Label>
                        </h3>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="card shadow-sm mb-4">
        <div class="card-body">
        
            <nav>
                <div class="nav nav-tabs mb-4" id="nav-tab" role="tablist">
                    <button class="nav-link active text-dark" id="nav-general-tab" data-bs-toggle="tab" data-bs-target="#tab-general" type="button" role="tab" aria-controls="tab-general" aria-selected="true">一般調查</button>
                
                    <button class="nav-link text-dark" id="nav-pest-tab" data-bs-toggle="tab" data-bs-target="#tab-pest" type="button" role="tab" aria-controls="tab-pest" aria-selected="false">病蟲害調查</button>
                
                    <button class="nav-link text-dark" id="nav-growth-tab" data-bs-toggle="tab" data-bs-target="#tab-growth" type="button" role="tab" aria-controls="tab-growth" aria-selected="false">樹木生長外觀</button>
                
                    <button class="nav-link text-dark" id="nav-pruning-tab" data-bs-toggle="tab" data-bs-target="#tab-pruning" type="button" role="tab" aria-controls="tab-pruning" aria-selected="false">修剪與支撐</button>
                
                    <button class="nav-link text-dark" id="nav-soil-tab" data-bs-toggle="tab" data-bs-target="#tab-soil" type="button" role="tab" aria-controls="tab-soil" aria-selected="false">生育地與土壤</button>
                
                    <button class="nav-link text-dark" id="nav-diagnosis-tab" data-bs-toggle="tab" data-bs-target="#tab-diagnosis" type="button" role="tab" aria-controls="tab-diagnosis" aria-selected="false">檢查結果與評估</button>

                    <button class="nav-link text-dark" id="nav-photos-tab" data-bs-toggle="tab" data-bs-target="#tab-photos" type="button" role="tab" aria-controls="tab-photos" aria-selected="false">照片</button>

                    <button class="nav-link text-dark" id="nav-files-tab" data-bs-toggle="tab" data-bs-target="#tab-files" type="button" role="tab" aria-controls="tab-files" aria-selected="false">附件</button>
                </div>
            </nav>

            <div class="tab-content" id="nav-tabContent">
            
                <div class="tab-pane fade show active" id="tab-general" role="tabpanel" aria-labelledby="nav-general-tab">
                    <h5 class="card-title text-secondary mb-3">一般調查資料</h5>
                    <div class="alert alert-light border">一般調查欄位區域</div>
                </div>

                <div class="tab-pane fade" id="tab-pest" role="tabpanel" aria-labelledby="nav-pest-tab">
                    <h5 class="card-title text-secondary mb-3">病蟲害調查</h5>
                    <div class="alert alert-light border">病蟲害調查欄位區域</div>
                </div>

                <div class="tab-pane fade" id="tab-growth" role="tabpanel" aria-labelledby="nav-growth-tab">
                    <h5 class="card-title text-secondary mb-3">樹木生長外觀情況</h5>
                    <div class="alert alert-light border">生長外觀欄位區域</div>
                </div>

                <div class="tab-pane fade" id="tab-pruning" role="tabpanel" aria-labelledby="nav-pruning-tab">
                    <h5 class="card-title text-secondary mb-3">樹木修剪與支撐情況</h5>
                    <div class="alert alert-light border">修剪與支撐欄位區域</div>
                </div>

                <div class="tab-pane fade" id="tab-soil" role="tabpanel" aria-labelledby="nav-soil-tab">
                    <h5 class="card-title text-secondary mb-3">生育地環境與土讓檢測情況</h5>
                    <div class="alert alert-light border">生育地與土壤欄位區域</div>
                </div>

                <div class="tab-pane fade" id="tab-diagnosis" role="tabpanel" aria-labelledby="nav-diagnosis-tab">
                    <h5 class="card-title text-secondary mb-3">健康檢查結果及風險評估</h5>
                    <div class="alert alert-light border">結果與評估欄位區域</div>
                </div>

                <div class="tab-pane fade" id="tab-photos" role="tabpanel" aria-labelledby="nav-photos-tab">
                    <h5 class="card-title text-secondary mb-3">現場照片</h5>
                    <div class="alert alert-light border">照片上傳區域</div>
                </div>

                <div class="tab-pane fade" id="tab-files" role="tabpanel" aria-labelledby="nav-files-tab">
                    <h5 class="card-title text-secondary mb-3">相關附件</h5>
                    <div class="alert alert-light border">附件上傳區域</div>
                </div>

            </div>
        </div>
    </div>

    <div class="card mb-4">
        <div class="card-body">
            <div class="form-check">
                <%-- 定稿勾選框 --%>
                <asp:CheckBox ID="CheckBox_dataStatus" runat="server" CssClass="form-check-input" />
                <label class="form-check-label fw-bold" for="<%= CheckBox_dataStatus.ClientID %>">
                    是否定稿
                </label>
                
            </div>
        </div>
    </div>

    <div class="text-center mb-5">
        <%-- 儲存/新增按鈕 --%>
        <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_save_Click">
            <asp:Literal ID="Literal_btnSaveText" runat="server" Text="新增"></asp:Literal>
        </asp:LinkButton>

        <%-- 取消按鈕 --%>
        <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-secondary" OnClick="LinkButton_cancel_Click">
            取消
        </asp:LinkButton>
    </div>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
