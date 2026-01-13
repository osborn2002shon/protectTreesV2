<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="detail.aspx.cs" Inherits="protectTreesV2.backstage.tree.detail" %>
<%@ Register Src="~/_uc/TreePhotoAlbum.ascx" TagPrefix="uc" TagName="TreePhotoAlbum" %>
<%@ Register Src="~/_uc/health/uc_healthRecordModal.ascx" TagPrefix="uc1" TagName="uc_healthRecordModal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/glightbox/dist/css/glightbox.min.css" />
    <style>
        .health-record-card.is-active {
            border-color: rgba(25, 135, 84, 0.6);
            box-shadow: 0 0 0 0.2rem rgba(25, 135, 84, 0.15);
        }
    </style>
    <script>
        function showHealthRecordModal() {
            var modalEl = document.getElementById('healthRecordModal');
            if (!modalEl) {
                return;
            }
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
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

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hfTreeID" runat="server" />
            <ul class="nav nav-tabs mb-4" id="treeDetailTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active" id="tree-detail-tab" data-bs-toggle="tab" data-bs-target="#pane-tree" type="button" role="tab" aria-controls="pane-tree" aria-selected="true">樹籍資料</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="tree-health-tab" data-bs-toggle="tab" data-bs-target="#pane-health" type="button" role="tab" aria-controls="pane-health" aria-selected="false">健檢紀錄</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link text-dark disabled" type="button" aria-disabled="true">巡查紀錄</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link text-dark disabled" type="button" aria-disabled="true">養護紀錄</button>
                </li>
            </ul>

            <div class="tab-content" id="treeDetailTabContent">
                <div class="tab-pane fade show active" id="pane-tree" role="tabpanel" aria-labelledby="tree-detail-tab">
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
                <div class="tab-pane fade" id="pane-health" role="tabpanel" aria-labelledby="tree-health-tab">
                    <div class="row g-4">
                        <div class="col-lg-5">
                            <div class="card h-100">
                                <div class="card-header d-flex align-items-center justify-content-between">
                                    <span class="fw-semibold">健檢照片</span>
                                    <span class="text-muted small">點擊照片以燈箱檢視</span>
                                </div>
                                <div class="card-body">
                                    <asp:Panel ID="pnlHealthPhotoGallery" runat="server" CssClass="tree-photo-album">
                                        <div class="row g-3 align-items-start">
                                            <div class="col-12">
                                                <asp:Panel ID="pnlHealthCoverPhoto" runat="server" CssClass="tree-cover-image">
                                                    <div class="ratio ratio-4x3 rounded overflow-hidden position-relative bg-light">
                                                        <a id="lnkHealthCoverLightbox" runat="server" class="tree-lightbox d-block h-100 w-100">
                                                            <asp:Image ID="imgHealthCover" runat="server" CssClass="w-100 h-100" />
                                                        </a>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                            <div class="col-12">
                                                <asp:Repeater ID="rptHealthGallery" runat="server">
                                                    <HeaderTemplate>
                                                        <div class="tree-gallery-grid">
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div class="tree-gallery-thumb">
                                                            <a href='<%# ResolveHealthPhotoUrl(Container.DataItem) %>' class="tree-lightbox d-block" data-gallery="health-photos" data-title='<%# BuildHealthPhotoTitleFromData(Container.DataItem) %>' data-description='<%# BuildHealthPhotoDescriptionFromData(Container.DataItem) %>'>
                                                                <div class="ratio ratio-4x3 overflow-hidden bg-light" style="min-height:100px;">
                                                                    <img src='<%# ResolveHealthPhotoUrl(Container.DataItem) %>' class="w-100 h-100" alt='<%# BuildHealthPhotoTitleFromData(Container.DataItem) %>' />
                                                                </div>
                                                            </a>
                                                        </div>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </div>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <asp:Label ID="lblNoHealthPhotos" runat="server" Text="尚無照片" CssClass="text-muted" Visible="false" />
                                    <asp:Label ID="lblHealthPhotoHint" runat="server" Text="請先選擇右側健檢紀錄以檢視照片" CssClass="text-muted" Visible="false" />
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <asp:PlaceHolder ID="phHealthRecordEmpty" runat="server" Visible="false">
                                <div class="text-muted text-center py-5">尚無健檢紀錄</div>
                            </asp:PlaceHolder>
                            <asp:Repeater ID="rptHealthRecords" runat="server" OnItemCommand="rptHealthRecords_ItemCommand">
                                <ItemTemplate>
                                    <div class='<%# GetHealthRecordCardCss(Container.DataItem) %>'>
                                        <div class="card-header d-flex align-items-center justify-content-between">
                                            <div class="fw-semibold">調查日期：<%# FormatSurveyDate(Eval("surveyDate")) %></div>
                                            <asp:LinkButton ID="btnSelectHealth" runat="server" CssClass="btn btn-outline-primary btn-sm" Text="顯示照片" CommandName="SelectHealth" CommandArgument='<%# Eval("healthID") %>' />
                                        </div>
                                        <div class="card-body">
                                            <div class="row g-3">
                                                <div class="col-md-4">
                                                    <div class="text-muted">管理情況</div>
                                                    <div class="fw-semibold"><%# FormatText(Eval("managementStatus")) %></div>
                                                </div>
                                                <div class="col-md-4">
                                                    <div class="text-muted">建議處理優先順序</div>
                                                    <div class="fw-semibold"><%# FormatText(Eval("priority")) %></div>
                                                </div>
                                                <div class="col-md-4">
                                                    <div class="text-muted">處理情形說明</div>
                                                    <div class="fw-semibold"><%# FormatText(Eval("treatmentDescription")) %></div>
                                                </div>
                                            </div>
                                            <div class="mt-3 d-flex flex-wrap gap-2">
                                                <asp:LinkButton ID="btnViewHealth" runat="server" CssClass="btn btn-sm btn-info text-white" Text="檢視報告" CommandName="ViewHealth" CommandArgument='<%# Eval("healthID") %>' />
                                                <asp:HyperLink ID="lnkHealthAttachment" runat="server" CssClass='<%# GetAttachmentButtonCss(Container.DataItem) %>' Text="附件下載" NavigateUrl='<%# GetAttachmentUrl(Container.DataItem) %>' Target="_blank" />
                                            </div>
                                        </div>
                                        <div class="card-footer text-muted">調查人：<%# FormatText(Eval("surveyor")) %></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt-4">
                <div class="col">
                    <asp:HyperLink ID="lnkBackToList" runat="server" Text="返回列表" NavigateUrl="query.aspx" CssClass="btn btn-outline-secondary" />
                </div>
            </div>
            <div class="modal fade" id="healthRecordModal" tabindex="-1" aria-hidden="true" style="color:#000;">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            健檢紀錄
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
                            <uc1:uc_healthRecordModal runat="server" id="uc_healthRecordModal" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
    </asp:UpdatePanel>

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

            function initPageComponents() {
                initLightbox();
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
