<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="detail.aspx.cs" Inherits="protectTreesV2.backstage.tree.detail" %>
<%@ Register Src="~/_uc/TreePhotoAlbum.ascx" TagPrefix="uc" TagName="TreePhotoAlbum" %>
<%@ Register Src="~/_uc/health/uc_healthRecordModal.ascx" TagPrefix="uc" TagName="HealthRecordModal" %>
<%@ Register Src="~/_uc/patrol/uc_patrolRecordModal.ascx" TagPrefix="uc" TagName="PatrolRecordModal" %>
<%@ Register Src="~/_uc/care/uc_careRecordModal.ascx" TagPrefix="uc" TagName="CareRecordModal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/glightbox/dist/css/glightbox.min.css" />
    <style>
        .health-record-card,
        .patrol-record-card {
            cursor: pointer;
            transition: box-shadow 0.2s ease, border-color 0.2s ease, background-color 0.2s ease;
        }

        .patrol-record-row.is-selected {
            background-color: #f0fbf4;
        }

        .health-record-card.is-selected,
        .patrol-record-card.is-selected {
            border: 2px solid #198754;
            box-shadow: 0 0.75rem 1.5rem rgba(25, 135, 84, 0.2);
            background-color: #f0fbf4;
        }

        .care-record-row.is-selected {
            background-color: #f0fbf4;
        }
    </style>
    <script>
        function showRecordModal() {
            var modalEl = document.getElementById('recordModal');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }

        function showHealthTab() {
            var tabEl = document.getElementById('tree-health-tab');
            if (!tabEl) return;
            var tab = bootstrap.Tab.getOrCreateInstance(tabEl);
            tab.show();
        }

        function showPatrolTab() {
            var tabEl = document.getElementById('tree-patrol-tab');
            if (!tabEl) return;
            var tab = bootstrap.Tab.getOrCreateInstance(tabEl);
            tab.show();
        }

        function showCareTab() {
            var tabEl = document.getElementById('tree-care-tab');
            if (!tabEl) return;
            var tab = bootstrap.Tab.getOrCreateInstance(tabEl);
            tab.show();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    樹籍管理 / 樹籍檢視
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    樹籍檢視
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">


            <asp:HiddenField ID="hfTreeID" runat="server" />
            <asp:HiddenField ID="hfSelectedHealthId" runat="server" />
            <asp:HiddenField ID="hfSelectedPatrolId" runat="server" />
            <asp:HiddenField ID="hfSelectedCareId" runat="server" />
            <ul class="nav nav-tabs mb-4" id="treeDetailTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <a class="nav-link text-dark active" id="tree-detail-tab" data-bs-toggle="tab" href="#pane-tree" role="tab" aria-controls="pane-tree" aria-selected="true">樹籍資料</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link text-dark" id="tree-health-tab" data-bs-toggle="tab" href="#pane-health" role="tab" aria-controls="pane-health" aria-selected="false">健檢紀錄</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link text-dark" id="tree-patrol-tab" data-bs-toggle="tab" href="#pane-patrol" role="tab" aria-controls="pane-patrol" aria-selected="false">巡查紀錄</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link text-dark" id="tree-care-tab" data-bs-toggle="tab" href="#pane-care" role="tab" aria-controls="pane-care" aria-selected="false">養護紀錄</a>
                </li>
            </ul>

            <div class="tab-content" id="treeDetailTabContent">
                <div class="tab-pane show active" id="pane-tree" role="tabpanel" aria-labelledby="tree-detail-tab">
                    <div class="row g-4">
                        <div class="col-lg-5">
                            <div class="card h-100">
                                <div class="card-header d-flex align-items-center justify-content-between">
                                    <span class="fw-semibold">樹木照片</span>
                                    <span class="text-muted small">點擊照片以燈箱檢視</span>
                                </div>
                                <div class="card-body">
                                    <uc:TreePhotoAlbum ID="treePhotoAlbum" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <div class="accordion" id="treeDetailAccordion">
                                <div class="accordion-item">
                                    <h2 class="accordion-header" id="headingBasic">
                                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseBasic" aria-expanded="true" aria-controls="collapseBasic">
                                            基本資料
                                        </button>
                                    </h2>
                                    <div id="collapseBasic" class="accordion-collapse collapse show" aria-labelledby="headingBasic" data-bs-parent="#treeDetailAccordion">
                                        <div class="accordion-body">
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">系統樹籍編號</div>
                                                    <asp:Label ID="lblSystemTreeNo" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col d-none">
                                                    <div class="text-muted">編輯狀態</div>
                                                    <asp:Label ID="lblEditStatus" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">機關管轄編碼</div>
                                                    <asp:Label ID="lblJurisdiction" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">機關樹木編號</div>
                                                    <asp:Label ID="lblAgencyTreeNo" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">樹種及學名</div>
                                                    <asp:Label ID="lblSpecies" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">數量</div>
                                                    <asp:Label ID="lblTreeCount" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">縣市鄉鎮</div>
                                                    <span class="fw-semibold d-block"><asp:Label ID="lblCity" runat="server" /> <asp:Label ID="lblArea" runat="server" /></span>
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">坐落地點</div>
                                                    <asp:Label ID="lblSite" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3 align-items-end">
                                                <div class="col">
                                                    <div class="text-muted">座標(WGS84) 緯度(N)</div>
                                                    <asp:Label ID="lblLatitude" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">座標(WGS84) 經度(E)</div>
                                                    <asp:Label ID="lblLongitude" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col-3">
                                                    <div class="text-muted">土地權屬</div>
                                                    <asp:Label ID="lblLandOwnership" runat="server" CssClass="d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">土地權屬備註</div>
                                                    <asp:Label ID="lblLandOwnershipNote" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">管理人</div>
                                                    <asp:Label ID="lblManager" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">管理人聯絡電話</div>
                                                    <asp:Label ID="lblManagerContact" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">管理設施描述</div>
                                                    <asp:Label ID="lblFacility" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">樹籍狀態</div>
                                                    <asp:Label ID="lblStatus" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                            </div>
                                            <asp:Panel ID="pnlAnnouncementSection" runat="server">
                                                <div class="row mb-3">
                                                    <div class="col">
                                                        <div class="text-muted">公告日期</div>
                                                        <asp:Label ID="lblAnnouncementDate" runat="server" CssClass="fw-semibold d-block" />
                                                    </div>
                                                </div>
                                                <div class="row mb-3">
                                                    <div class="col">
                                                        <div class="text-muted">受保護認定理由</div>
                                                        <asp:Literal ID="ltlRecognition" runat="server" Mode="PassThrough" />
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">認定理由備註說明</div>
                                                    <asp:Label ID="lblRecognitionNote" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">文化歷史價值介紹</div>
                                                    <asp:Label ID="lblCulturalHistory" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="accordion-item">
                                    <h2 class="accordion-header" id="headingSurvey">
                                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseSurvey" aria-expanded="false" aria-controls="collapseSurvey">
                                            調查資料
                                        </button>
                                    </h2>
                                    <div id="collapseSurvey" class="accordion-collapse collapse" aria-labelledby="headingSurvey" data-bs-parent="#treeDetailAccordion">
                                        <div class="accordion-body">
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">調查日期</div>
                                                    <asp:Label ID="lblSurveyDate" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">調查人姓名</div>
                                                    <asp:Label ID="lblSurveyor" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">推估種植年間</div>
                                                    <asp:Label ID="lblEstimatedPlantingYear" runat="server" CssClass="d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">推估年齡備註</div>
                                                    <asp:Label ID="lblEstimatedAgeNote" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">群生竹木或行道樹生長資訊</div>
                                                    <asp:Label ID="lblGroupGrowthInfo" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">樹高</div>
                                                    <asp:Label ID="lblTreeHeight" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">胸高直徑</div>
                                                    <asp:Label ID="lblBreastHeightDiameter" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">胸高樹圍</div>
                                                    <asp:Label ID="lblBreastHeightCircumference" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                                <div class="col">
                                                    <div class="text-muted">樹冠投影面積</div>
                                                    <asp:Label ID="lblCanopyArea" runat="server" CssClass="fw-semibold d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">樹木健康及生育地概況</div>
                                                    <asp:Label ID="lblHealth" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">附生植物概況</div>
                                                    <asp:Label ID="lblEpiphyteDescription" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">寄生植物概況</div>
                                                    <asp:Label ID="lblParasiteDescription" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">纏勒植物概況</div>
                                                    <asp:Label ID="lblClimbingPlantDescription" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col">
                                                    <div class="text-muted">其他備註</div>
                                                    <asp:Label ID="lblMemo" runat="server" CssClass="d-block" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane" id="pane-health" role="tabpanel" aria-labelledby="tree-health-tab">
                    <div class="row g-4">
                        <div class="col-lg-5">
                            <div class="card h-100">
                                <div class="card-header d-flex align-items-center justify-content-between">
                                    <span class="fw-semibold">健檢照片</span>
                                    <span class="text-muted small">選擇右側紀錄以切換，點擊照片以燈箱檢視</span>
                                </div>
                                <div class="card-body">
                                    <uc:TreePhotoAlbum ID="healthPhotoAlbum" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <asp:Panel ID="pnlHealthRecordEmpty" runat="server" Visible="false" CssClass="text-center text-muted py-5">
                                查無健檢紀錄。
                            </asp:Panel>
                            <asp:Repeater ID="rptHealthRecords" runat="server" OnItemCommand="rptHealthRecords_ItemCommand" OnItemDataBound="rptHealthRecords_ItemDataBound">
                                <HeaderTemplate>
                                    <div class="row g-3">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="col-12 col-lg-6">
                                        <asp:Panel ID="pnlHealthCard" runat="server" CssClass="card health-record-card h-100">
                                            <div class="card-header d-flex align-items-center justify-content-between">
                                                <asp:LinkButton ID="btnSelectHealth" runat="server" CssClass="fw-semibold text-decoration-none" CommandName="SelectHealth" CommandArgument='<%# Eval("HealthId") %>'>
                                                    <%# Eval("SurveyDateDisplay") %>
                                                </asp:LinkButton>
                                                <asp:Label ID="lblHealthSelectionHint" runat="server" CssClass="text-muted small" Text="點選切換照片" />
                                            </div>
                                            <div class="card-body">
                                                <div class="row g-3">
                                                    <div class="col-12">
                                                        <div class="text-muted small">管理情況</div>
                                                        <div class="fw-semibold"><%# Eval("ManagementStatusDisplay") %></div>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <div class="text-muted small">建議處理優先順序</div>
                                                        <div class="fw-semibold"><%# Eval("PriorityDisplay") %></div>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <div class="text-muted small">處理情形說明</div>
                                                        <div class="fw-semibold"><%# Eval("TreatmentDescriptionDisplay") %></div>
                                                    </div>
                                                </div>
                                                <div class="d-flex flex-wrap gap-2 mt-3">
                                                    <asp:LinkButton ID="btnViewReport" runat="server" CssClass="btn btn-sm btn-primary" CommandName="ViewReport" CommandArgument='<%# Eval("HealthId") %>' Text="檢視報告" />
                                                    <div class="dropdown">
                                                        <button id="btnAttachmentToggle" runat="server" type="button" class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">附件下載</button>
                                                        <ul class="dropdown-menu">
                                                            <asp:Repeater ID="rptHealthAttachments" runat="server">
                                                                <ItemTemplate>
                                                                    <li>
                                                                        <a class="dropdown-item" href='<%# ResolveUrl(Eval("filePath").ToString()) %>' target="_blank" rel="noopener">
                                                                            <%# Eval("fileName") %>
                                                                        </a>
                                                                    </li>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </ul>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="card-footer text-muted small text-end">
                                                調查人：<%# Eval("SurveyorDisplay") %>｜最後更新：<%# Eval("LastUpdateDisplay") %>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="pane-patrol" role="tabpanel" aria-labelledby="tree-patrol-tab">
                    <div class="row g-4">
                        <div class="col-lg-5">
                            <div class="card h-100">
                                <div class="card-header d-flex align-items-center justify-content-between">
                                    <span class="fw-semibold">巡查照片</span>
                                    <span class="text-muted small">選擇右側紀錄以切換，點擊照片以燈箱檢視</span>
                                </div>
                                <div class="card-body">
                                    <uc:TreePhotoAlbum ID="patrolPhotoAlbum" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <asp:Panel ID="pnlPatrolRecordEmpty" runat="server" Visible="false" CssClass="text-center text-muted py-5">
                                查無巡查紀錄。
                            </asp:Panel>
                            <div class="card mb-3">
                                <div class="card-body">
                                    <div class="row g-3 align-items-end">
                                        <div class="col-md-3">
                                            <label class="form-label">年度篩選</label>
                                            <asp:DropDownList ID="ddlPatrolYear" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPatrolYear_SelectedIndexChanged" />
                                        </div>
                                        <div class="col-md-3">
                                            <label class="form-label">月份篩選</label>
                                            <asp:DropDownList ID="ddlPatrolMonth" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPatrolMonth_SelectedIndexChanged" />
                                        </div>
                                        <div class="col-md-3">
                                            <label class="form-label">關鍵字篩選</label>
                                            <asp:TextBox ID="txtPatrolKeyword" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtPatrolKeyword_TextChanged" />
                                        </div>
                                        <div class="col-md-3 text-md-end">
                                            <span class="text-muted">資料總筆數：</span>
                                            <asp:Label ID="lblPatrolRecordTotal" runat="server" CssClass="fw-semibold" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <asp:GridView ID="gvPatrolRecords" runat="server" CssClass="table table-bordered align-middle" AutoGenerateColumns="false" OnRowCommand="gvPatrolRecords_RowCommand" OnRowDataBound="gvPatrolRecords_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="巡查日期">
                                        <ItemTemplate>
                                            <%# Eval("PatrolDateDisplay") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="調查人">
                                        <ItemTemplate>
                                            <%# Eval("PatrollerDisplay") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="巡查備註">
                                        <ItemTemplate>
                                            <%# Eval("MemoDisplay") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="風險狀況">
                                        <ItemTemplate>
                                            <%# Eval("RiskDisplay") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="照片">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnSelectPatrol" runat="server" CssClass="btn btn-sm btn-primary" CommandName="SelectPatrol" CommandArgument='<%# Eval("PatrolId") %>' Text="檢視" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="pane-care" role="tabpanel" aria-labelledby="tree-care-tab">
                    <div class="row g-4">
                        <div class="col-lg-5">
                            <div class="card h-100">
                                <div class="card-header d-flex align-items-center justify-content-between">
                                    <span class="fw-semibold">養護照片</span>
                                    <span class="text-muted small">選擇右側紀錄以切換，點擊照片以燈箱檢視</span>
                                </div>
                                <div class="card-body">
                                    <uc:TreePhotoAlbum ID="carePhotoAlbum" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <asp:Panel ID="pnlCareRecordEmpty" runat="server" Visible="false" CssClass="text-center text-muted py-5">
                                查無養護紀錄。
                            </asp:Panel>
                            <div class="card mb-3">
                                <div class="card-body">
                                    <div class="row g-3 align-items-end">
                                        <div class="col-md-3">
                                            <label class="form-label">年度篩選</label>
                                            <asp:DropDownList ID="ddlCareYear" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCareYear_SelectedIndexChanged" />
                                        </div>
                                        <div class="col-md-3">
                                            <label class="form-label">月份篩選</label>
                                            <asp:DropDownList ID="ddlCareMonth" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCareMonth_SelectedIndexChanged" />
                                        </div>
                                        <div class="col-md-3">
                                            <label class="form-label">關鍵字篩選</label>
                                            <asp:TextBox ID="txtCareKeyword" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtCareKeyword_TextChanged" />
                                        </div>
                                        <div class="col-md-3 text-md-end">
                                            <span class="text-muted">資料總筆數：</span>
                                            <asp:Label ID="lblCareRecordTotal" runat="server" CssClass="fw-semibold" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <asp:GridView ID="gvCareRecords" runat="server" CssClass="table table-bordered align-middle" AutoGenerateColumns="false" OnRowCommand="gvCareRecords_RowCommand" OnRowDataBound="gvCareRecords_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="養護日期">
                                        <ItemTemplate>
                                            <%# Eval("CareDateDisplay") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="施作項目">
                                        <ItemTemplate>
                                            <div class="text-wrap">
                                                <asp:Literal ID="ltlCareItems" runat="server" Mode="PassThrough" Text='<%# Eval("CareItemDisplay") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="記錄人員">
                                        <ItemTemplate>
                                            <%# Eval("RecorderDisplay") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="覆核人員">
                                        <ItemTemplate>
                                            <%# Eval("ReviewerDisplay") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="照片">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnSelectCare" runat="server" CssClass="btn btn-sm btn-primary" CommandName="SelectCare" CommandArgument='<%# Eval("CareId") %>' Text="檢視" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="內容">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnViewCareReport" runat="server" CssClass="btn btn-sm btn-primary" CommandName="ViewReport" CommandArgument='<%# Eval("CareId") %>' Text="檢視" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mt-4">
                <div class="col">
                    <asp:HyperLink ID="lnkBackToList" runat="server" Text="返回列表" NavigateUrl="query.aspx" CssClass="btn btn-outline-secondary" />
                </div>
            </div>

            <div class="modal fade" id="recordModal" tabindex="-1" aria-hidden="true" style="color: #000;">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <asp:Literal ID="ltlRecordModalTitle" runat="server" Text="健檢紀錄" />
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="formCard card mb-4">
                                <div class="card-header">基本資料</div>
                                <div class="card-body">
                                    <div class="row g-3">
                                        <div class="col-md-4 col-sm-6">
                                            <label class="form-label text-muted">系統紀錄編號</label>
                                            <div class="fw-bold">
                                                <asp:Label ID="lblModal_healthId" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6">
                                            <label class="form-label text-muted">系統樹籍編號</label>
                                            <div class="fw-bold">
                                                <asp:Label ID="lblModal_systemTreeNo" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6">
                                            <label class="form-label text-muted">資料狀態</label>
                                            <div class="fw-bold">
                                                <asp:Label ID="lblModal_status" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6">
                                            <label class="form-label text-muted">所在地</label>
                                            <div class="fw-bold">
                                                <asp:Label ID="lblModal_location" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6">
                                            <label class="form-label text-muted">樹種及學名</label>
                                            <div class="fw-bold">
                                                <asp:Label ID="lblModal_species" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6">
                                            <label class="form-label text-muted">最後更新</label>
                                            <div class="fw-bold">
                                                <asp:Label ID="lblModal_lastUpdate" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <asp:PlaceHolder ID="phHealthRecordModal" runat="server">
                                <uc:HealthRecordModal runat="server" ID="uc_healthRecordModal" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phPatrolRecordModal" runat="server" Visible="false">
                                <uc:PatrolRecordModal runat="server" ID="uc_patrolRecordModal" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phCareRecordModal" runat="server" Visible="false">
                                <uc:CareRecordModal runat="server" ID="uc_careRecordModal" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>



    <script src="https://cdn.jsdelivr.net/npm/glightbox/dist/js/glightbox.min.js"></script>
    <script type="text/javascript">
        (function () {
            var lightboxInstance;
            function initLightbox() {
                if (lightboxInstance) {
                    lightboxInstance.destroy();
                }
                lightboxInstance = GLightbox({
                    selector: '.tree-lightbox',
                    loop: true,
                    touchNavigation: true
                });
            }

            function initHealthRecordCards() {
                document.querySelectorAll('.health-record-card').forEach(function (card) {
                    if (card.dataset.healthCardBound) {
                        return;
                    }
                    card.dataset.healthCardBound = 'true';
                    card.addEventListener('click', function (event) {
                        if (event.target.closest('a, button, .dropdown-menu, input, label')) {
                            return;
                        }
                        var targetId = card.getAttribute('data-select-target');
                        if (!targetId) {
                            return;
                        }
                        var targetLink = document.getElementById(targetId);
                        if (targetLink) {
                            targetLink.click();
                        }
                    });
                });
            }

            function initPatrolRecordCards() {
                document.querySelectorAll('.patrol-record-card').forEach(function (card) {
                    if (card.dataset.patrolCardBound) {
                        return;
                    }
                    card.dataset.patrolCardBound = 'true';
                    card.addEventListener('click', function (event) {
                        if (event.target.closest('a, button, .dropdown-menu, input, label')) {
                            return;
                        }
                        var targetId = card.getAttribute('data-select-target');
                        if (!targetId) {
                            return;
                        }
                        var targetLink = document.getElementById(targetId);
                        if (targetLink) {
                            targetLink.click();
                        }
                    });
                });
            }

            function initPageComponents() {
                initLightbox();
                initHealthRecordCards();
                initPatrolRecordCards();
            }

            document.addEventListener('DOMContentLoaded', initPageComponents);
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initPageComponents);
            }
        })();
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
