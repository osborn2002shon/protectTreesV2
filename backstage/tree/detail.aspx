<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="detail.aspx.cs" Inherits="protectTreesV2.backstage.tree.detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/glightbox/dist/css/glightbox.min.css" />
    <style>
        .tree-cover-image img,
        .tree-gallery-thumb img {
            object-fit: cover;
        }

        .tree-cover-badge {
            top: 12px;
            left: 12px;
        }

        .tree-gallery-thumb .card {
            transition: transform 0.15s ease, box-shadow 0.15s ease;
        }

        .tree-gallery-thumb .card:hover {
            transform: translateY(-3px);
            box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.08);
        }
    </style>
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
            <ul class="nav nav-tabs mb-3" id="treeDetailTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <a class="nav-link active" aria-current="page" href="#">樹籍資料</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link disabled" href="#" tabindex="-1" aria-disabled="true">健檢紀錄</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link disabled" href="#" tabindex="-1" aria-disabled="true">巡查紀錄</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link disabled" href="#" tabindex="-1" aria-disabled="true">養護紀錄</a>
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
                                    <asp:Panel ID="pnlPhotoGallery" runat="server">
                                        <div class="row g-3 align-items-start">
                                            
                                            <div class="col-lg-12">
                                                <asp:Panel ID="pnlCoverPhoto" runat="server" CssClass="tree-cover-image">
                                                    <div class="ratio ratio-4x3 rounded overflow-hidden position-relative bg-light mb-3">
                                                        <a id="lnkCoverLightbox" runat="server" class="tree-lightbox d-block h-100 w-100">
                                                            <asp:Image ID="imgCover" runat="server" CssClass="w-100 h-100" />
                                                        </a>
                                                        
                                                    </div>
                                                    
                                                    <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
                                                        
                                                        <asp:Label ID="lblCoverCaption" runat="server" CssClass="fw-semibold" />
                                                        <asp:Label ID="lblCoverUploadTime" runat="server" CssClass="text-muted small" />
                                                        
                                                    </div>
                                                </asp:Panel>
                                                
                                            </div>
                                            
                                            <div class="col-lg-12">
                                                <div class="row row-cols-1 row-cols-sm-2 g-3">
                                                    <asp:Repeater ID="rptGallery" runat="server">
                                                        <ItemTemplate>
                                                            <div class="col tree-gallery-thumb">
                                                                <div class="card h-100 border-0 shadow-sm">
                                                                    <a href='<%# Eval("FilePath") %>' class="tree-lightbox" data-gallery="tree-photos" data-title='<%# BuildLightboxTitleFromData(Container.DataItem) %>' data-description='<%# BuildLightboxDescriptionAttributeFromData(Container.DataItem) %>'>
                                                                        <div class="ratio ratio-4x3 overflow-hidden rounded-top">
                                                                            <img src='<%# Eval("FilePath") %>' class="w-100 h-100" alt='<%# BuildLightboxTitleFromData(Container.DataItem) %>' />
                                                                        </div>
                                                                    </a>
                                                                    <div class="p-2">
                                                                        <div class="d-flex align-items-center justify-content-between gap-2">
                                                                            <span class="fw-semibold text-truncate"><%# BuildPhotoCaptionFromData(Container.DataItem) %></span>
                                                                            <asp:Label runat="server" Visible='<%# (bool)Eval("IsCover") %>' CssClass="badge bg-primary">封面</asp:Label>
                                                                        </div>
                                                                        <div class="text-muted small">上傳：<%# BuildUploadTimeDisplayFromData(Container.DataItem) %></div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <asp:Label ID="lblNoPhotos" runat="server" Text="尚無照片" CssClass="text-muted" Visible="false" />
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
                                            <div class="row mb-2">
                                                <div class="col">
                                                    <small class="form-text text-muted">請輸入精度最多至第六位的經緯度座標</small>
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
                <div class="row mt-4">
                    <div class="col">
                        <asp:HyperLink ID="lnkBackToList" runat="server" Text="返回列表" NavigateUrl="query.aspx" CssClass="btn btn-outline-secondary" />
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

            document.addEventListener('DOMContentLoaded', initLightbox);
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initLightbox);
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
