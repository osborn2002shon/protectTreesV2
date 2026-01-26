<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="Tree.aspx.cs" Inherits="protectTreesV2.backstage.Manage.Tree" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
<style type="text/css">
    .tb th, .tb td { text-align: center; }
    .tb td.text-start { text-align: left; }
    .photo-card {
        border: 1px solid #ccc;
        border-radius: 6px;
        padding: 10px;
        margin-bottom: 10px;
        text-align: center;
    }

    .photo-card img {
        max-width: 200px;
        max-height: 200px;
        object-fit: cover;
    }

    .photo-card .actions {
        margin-top: 8px;
    }

    .photo-card .badge {
        display: inline-block;
        padding: 2px 8px;
        background-color: #198754;
        color: #fff;
        border-radius: 12px;
        font-size: 0.75rem;
    }

    .field-readonly {
        background-color: #f5f5f5;
    }

    .form-grid label {
        font-weight: 500;
    }

    .form-grid .row + .row {
        margin-top: 1rem;
    }

    .checkbox-list {
        border: 1px solid #e0e0e0;
        padding: 10px;
        border-radius: 6px;
    }

    .modal-overlay {
        position: fixed;
        inset: 0;
        background-color: rgba(0, 0, 0, 0.5);
        display: none;
        align-items: center;
        justify-content: center;
        z-index: 1050;
    }

    .modal-overlay.active {
        display: flex;
    }

    .modal-dialog-custom {
        background-color: #fff;
        border-radius: 8px;
        max-width: 520px;
        width: 100%;
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
    }

    .modal-dialog-custom .modal-header,
    .modal-dialog-custom .modal-footer {
        padding: 12px 20px;
        border-bottom: 1px solid #e5e5e5;
    }

    .modal-dialog-custom .btn-close {
        background: none;
        border: none;
        font-size: 1.5rem;
        line-height: 1;
        cursor: pointer;
    }

    .modal-dialog-custom .modal-footer {
        border-top: 1px solid #e5e5e5;
        border-bottom: none;
        display: flex;
        justify-content: flex-end;
        gap: 10px;
    }

    .modal-dialog-custom .modal-body {
        padding: 20px;
    }

    .log-pager {
        display: flex;
        justify-content: space-between;
        align-items: center;
        flex-wrap: wrap;
        gap: 12px;
        margin-top: 12px;
    }

    .log-pager-info {
        font-size: 0.95rem;
        color: #555;
    }

    .pager-buttons {
        display: flex;
        align-items: center;
        gap: 8px;
        flex-wrap: wrap;
    }

    .pager-link {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 6px 12px;
        border: 1px solid #ced4da;
        border-radius: 4px;
        background-color: #fff;
        color: #333;
        text-decoration: none;
        cursor: pointer;
        transition: background-color 0.2s ease, color 0.2s ease, border-color 0.2s ease;
    }

    .pager-link:hover {
        background-color: #0d6efd;
        color: #fff;
        border-color: #0d6efd;
    }

    .pager-link.active {
        background-color: #0d6efd;
        color: #fff;
        border-color: #0b5ed7;
        cursor: default;
    }

    .photo-preview .photo-item {
        border: 1px solid #2a2a2a;
        border-radius: 6px;
        padding: 10px;
        width: 220px;
        position: relative;
        background-color: #000;
        color: #fff;
    }

    .photo-preview .photo-item img {
        width: 100%;
        height: 150px;
        object-fit: cover;
        border-radius: 4px;
        background-color: #000;
    }

    .photo-preview .photo-item .photo-fields {
        margin-top: 8px;
        text-align: left;
    }

    .photo-preview .photo-item .btn-close {
        position: absolute;
        top: 6px;
        right: 6px;
        background-color: rgba(255, 255, 255, 0.25);
        color: #fff;
        border-radius: 50%;
        width: 24px;
        height: 24px;
        line-height: 24px;
        text-align: center;
        border: none;
        font-size: 14px;
        cursor: pointer;
    }

    .photo-preview .photo-item .btn-close:hover {
        background-color: rgba(0, 0, 0, 0.8);
    }

    .dropzone {
        border: 2px dashed #ccc;
        border-radius: 6px;
        padding: 20px;
        text-align: center;
        cursor: pointer;
        transition: background-color 0.2s ease, border-color 0.2s ease;
    }

    .dropzone.dragover {
        border-color: #0d6efd;
        background-color: #f0f8ff;
    }
     </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:MultiView ID="mvMain" runat="server" ActiveViewIndex="0">
        <asp:View ID="viewList" runat="server">
            <div class="row">
                <div class="col text-end">
                    <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn_def" OnClick="btnAdd_Click"><i class="bi bi-plus-circle"></i> 新增樹籍</asp:LinkButton>
                    <asp:LinkButton ID="btnExport" runat="server" CssClass="btn_def" OnClick="btnExport_Click"><i class="bi bi-download"></i> 下載列表</asp:LinkButton>
                </div>
            </div>
            <div class="pageBlock mt-3">
                <div class="row">
                    <div class="col">
                        <span>縣市鄉鎮</span>
                        <div class="d-flex gap-2">
                            <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" AutoPostBack="true" />
                            <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-select" />
                        </div>
                    </div>
                    <div class="col">
                        <span>編輯狀態</span>
                        <asp:DropDownList ID="ddlEditStatus" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col">
                        <span>樹籍狀態</span>
                        <asp:DropDownList ID="ddlTreeStatus" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col">
                        <span>樹種</span>
                        <asp:DropDownList ID="ddlSpecies" runat="server" CssClass="form-select" data-combobox="species" data-combobox-placeholder="請輸入樹種" />
                    </div>
                </div>
                <div class="row mt-2">
                    <div class="col">
                        <span>調查日期起訖</span>
                        <div class="d-flex gap-2">
                            <asp:TextBox ID="txtSurveyStart" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:TextBox ID="txtSurveyEnd" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                    </div>
                    <div class="col">
                        <span>公告日期起訖</span>
                        <div class="d-flex gap-2">
                            <asp:TextBox ID="txtAnnouncementStart" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:TextBox ID="txtAnnouncementEnd" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                    </div>
                    <div class="col">
                        <span>關鍵字</span>
                        <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control" placeholder="管理人、樹籍編號、樹木編號、管轄編碼" />
                    </div>
                </div>
                <div class="row mt-4">
                    <div class="col">
                        <asp:LinkButton ID="btnQuery" runat="server" CssClass="btn_action w-100" OnClick="btnQuery_Click">查詢</asp:LinkButton>
                    </div>
                </div>
            </div>
            <div class="pageBlock mt-3">
                <div class="text-end" style="font-size:1rem">
                    樹籍資料查詢結果 <asp:Label ID="lblResultCount" runat="server" Text="0" /> 筆
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvTrees" runat="server" AutoGenerateColumns="false" CssClass="tb" AllowPaging="true" PageSize="20"
                        DataKeyNames="TreeID,SourceUnit" OnPageIndexChanging="gvTrees_PageIndexChanging" OnRowCommand="gvTrees_RowCommand"
                        OnRowDataBound="gvTrees_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="40px">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelectAll_CheckedChanged" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" />
                                    <asp:HiddenField ID="hfTreeID" runat="server" Value='<%# Eval("TreeID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="系統<br />樹籍編號">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="OpenTree" CommandArgument='<%# Eval("TreeID") %>' Text='<%# Eval("SystemTreeNo") %>' CssClass="link-primary" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>機關<br />樹木編號</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("AgencyTreeNo") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>機關<br />管轄編碼</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("AgencyJurisdictionCode") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="縣市 / 鄉鎮">
                                <ItemTemplate>
                                    <div><%# Eval("CityName") %></div>
                                    <div><%# Eval("AreaName") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="SpeciesCommonName" HeaderText="樹種" />
                            <asp:BoundField DataField="Manager" HeaderText="管理人" />
                            <asp:BoundField DataField="SurveyDate" DataFormatString="{0:yyyy/MM/dd}" HeaderText="調查日期" />
                            <asp:BoundField DataField="AnnouncementDate" DataFormatString="{0:yyyy/MM/dd}" HeaderText="公告日期" />
                            <asp:TemplateField HeaderText="樹籍狀態">
                                <ItemTemplate>
                                    <span class="status" runat="server" id="litStatus" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="編輯狀態">
                                <ItemTemplate>
                                    <span class="edit-status" runat="server" id="litEditStatus" />
                                    <div class="tbMiniInfo"><%# Eval("UpdateDateTime", "{0:yyyy/MM/dd HH:mm}") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:Panel ID="pnlBatchStatus" runat="server" CssClass="pageBlock mt-2">
                    將本頁勾選樹籍變更狀態為
                    <asp:DropDownList ID="ddlBatchStatus" runat="server" CssClass="form-select d-inline-block" Width="180px" />
                    <span id="batchAnnouncementWrapper">
                        公告日期 <asp:TextBox ID="txtBatchAnnouncementDate" runat="server" TextMode="Date" CssClass="form-control d-inline-block" Width="160px" />
                    </span>
                    <asp:LinkButton ID="btnBatchStatus" runat="server" CssClass="btn_action" OnClick="btnBatchStatus_Click">執行</asp:LinkButton>
                </asp:Panel>
            </div>
        </asp:View>
        <asp:View ID="viewDetail" runat="server">
            <asp:HiddenField ID="hfTreeID" runat="server" />
            <asp:HiddenField ID="hfSourceUnit" runat="server" />
            <asp:Panel ID="pnlDetailNavigation" runat="server" CssClass="row">
                <div class="col">
                    <div class="item">
                        <img src="../../_img/icon/database.png" /><br />
                        <asp:HyperLink ID="lnkReturnToDetail" runat="server" Text="樹籍資料" CssClass="btn-link" />
                    </div>
                </div>
                <div class="col">
                    <div class="item">
                        <img src="../../_img/icon/health-check2.png" /><br />
                        <asp:HyperLink ID="lnkHealth" runat="server" Text="健檢紀錄" />
                    </div>
                </div>
                <div class="col">
                    <div class="item">
                        <img src="../../_img/icon/picture.png" /><br />
                        <asp:HyperLink ID="lnkPatrol" runat="server" Text="巡查紀錄" />
                    </div>
                </div>
                <div class="col">
                    <div class="item">
                        <img src="../../_img/icon/sign.png" /><br />
                        <asp:HyperLink ID="lnkCare" runat="server" Text="養護紀錄" />
                    </div>
                </div>
            </asp:Panel>
            <div class="pageBlock mt-3">
                <asp:Label ID="lblDetailNotice" runat="server" CssClass="text-danger"></asp:Label>
                <div class="boxTitle">樹籍資料</div>
                <div class="boxTitle_mini">基本資料</div>
                <div class="form-grid">
                    <div class="row">
                        <div class="col">
                            <label for="txtSurveyDate">調查日期</label>
                            <asp:TextBox ID="txtSurveyDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="col">
                            <label for="txtSurveyor">調查人姓名</label>
                            <asp:TextBox ID="txtSurveyor" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>縣市 / 鄉鎮市區</label>
                            <div class="d-flex gap-2">
                                <asp:DropDownList ID="ddlDetailCity" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDetailCity_SelectedIndexChanged" />
                                <asp:DropDownList ID="ddlDetailArea" runat="server" CssClass="form-select" />
                            </div>
                        </div>
                        <div class="col">
                            <label for="txtSystemTreeNo">系統樹籍編號</label>
                            <asp:TextBox ID="txtSystemTreeNo" runat="server" CssClass="form-control" ReadOnly="true" />
                            <small class="text-muted">儲存後自動產生</small>
                        </div>
                        <div class="col">
                            <label for="txtJurisdictionCode">機關管轄編碼</label>
                            <asp:TextBox ID="txtJurisdictionCode" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col">
                            <label for="txtAgencyTreeNo">機關樹木編號</label>
                            <asp:TextBox ID="txtAgencyTreeNo" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label for="ddlDetailSpecies">樹種及學名</label>
                            <asp:DropDownList ID="ddlDetailSpecies" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col">
                            <label for="txtTreeCount">數量</label>
                            <asp:TextBox ID="txtTreeCount" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                        <div class="col">
                            <label for="ddlDetailStatus">樹籍狀態</label>
                            <asp:DropDownList ID="ddlDetailStatus" runat="server" CssClass="form-select" />
                        </div>
                        <asp:Panel ID="pnlAnnouncementDate" runat="server" CssClass="col">
                            <label for="txtDetailAnnouncementDate">公告日期</label>
                            <asp:TextBox ID="txtDetailAnnouncementDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </asp:Panel>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label for="ddlDetailEditStatus">編輯狀態</label>
                            <asp:DropDownList ID="ddlDetailEditStatus" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col">
                            <label for="txtManager">管理人</label>
                            <asp:TextBox ID="txtManager" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col">
                            <label for="txtManagerContact">管理人聯絡電話</label>
                            <asp:TextBox ID="txtManagerContact" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <label for="txtSite">坐落地點</label>
                            <asp:TextBox ID="txtSite" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label for="txtLatitude">座標(WGS84)：緯度(N)</label>
                            <asp:TextBox ID="txtLatitude" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-4">
                            <label for="txtLongitude">座標(WGS84)：經度(E)</label>
                            <asp:TextBox ID="txtLongitude" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-4">
                            <label class="d-block">座標轉換工具</label>
                            <button type="button" id="btnOpenCoordinate" class="btn_def w-100">開啟座標轉換工具</button>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label for="txtLandOwnership">土地權屬</label>
                            <asp:TextBox ID="txtLandOwnership" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-9">
                            <label for="txtLandNote">土地權屬備註</label>
                            <asp:TextBox ID="txtLandNote" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <label for="txtFacility">管理設施描述</label>
                            <asp:TextBox ID="txtFacility" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label for="txtMemo">備註</label>
                            <asp:TextBox ID="txtMemo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-6">
                            <label>認定理由</label>
                            <div class="checkbox-list">
                                <asp:CheckBoxList ID="cblCriteria" runat="server" RepeatDirection="Vertical" />
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="mb-3">
                                <label for="txtRecognitionNote">認定理由備註說明</label>
                                <asp:TextBox ID="txtRecognitionNote" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" Height="120" />
                            </div>
                            <div>
                                <label for="txtCulturalHistory">文化歷史價值介紹</label>
                                <asp:TextBox ID="txtCulturalHistory" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" Height="120" />
                            </div>
                        </div>
                    </div>
                    <div class="boxTitle_mini mt-4">調查資料</div>
                    <div class="row">
                        <div class="col-md-6">
                            <label for="txtEstimatedPlantingYear">推估種植年間</label>
                            <asp:TextBox ID="txtEstimatedPlantingYear" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label for="txtEstimatedAgeNote">推估樹齡備註</label>
                            <asp:TextBox ID="txtEstimatedAgeNote" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label for="txtGroupGrowthInfo">群生竹木或行道樹生長資訊</label>
                            <asp:TextBox ID="txtGroupGrowthInfo" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label for="txtTreeHeight">樹高</label>
                            <asp:TextBox ID="txtTreeHeight" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-3">
                            <label for="txtBreastHeightDiameter">胸高直徑</label>
                            <asp:TextBox ID="txtBreastHeightDiameter" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-3">
                            <label for="txtBreastHeightCircumference">胸高樹圍</label>
                            <asp:TextBox ID="txtBreastHeightCircumference" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-3">
                            <label for="txtCanopyProjectionArea">樹冠投影面積</label>
                            <asp:TextBox ID="txtCanopyProjectionArea" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label for="txtHealthCondition">樹木健康及生育地概況</label>
                            <asp:TextBox ID="txtHealthCondition" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>附生植物概況</label>
                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                <asp:CheckBox ID="chkHasEpiphyte" runat="server" Text="有" />
                                <span>狀況說明：</span>
                                <asp:TextBox ID="txtEpiphyteDescription" runat="server" CssClass="form-control flex-grow-1" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>寄生植物概況</label>
                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                <asp:CheckBox ID="chkHasParasite" runat="server" Text="有" />
                                <span>狀況說明：</span>
                                <asp:TextBox ID="txtParasiteDescription" runat="server" CssClass="form-control flex-grow-1" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>纏勒植物概況</label>
                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                <asp:CheckBox ID="chkHasClimbingPlant" runat="server" Text="有" />
                                <span>狀況說明：</span>
                                <asp:TextBox ID="txtClimbingPlantDescription" runat="server" CssClass="form-control flex-grow-1" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label for="txtSurveyOtherNote">其他備註</label>
                            <asp:TextBox ID="txtSurveyOtherNote" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="pageBlock mt-3">
                <div class="boxTitle">樹籍照片</div>
                <asp:Panel ID="pnlPhotoUpload" runat="server">
                    <div class="mb-2 small">拖拉或點擊下方區塊以選擇照片，單張預覽最大 200px × 200px。</div>
                    <asp:FileUpload ID="fuPhoto" runat="server" CssClass="d-none" AllowMultiple="true" />
                    <div id="photoDrop" class="dropzone">拖曳照片到此或點擊選擇</div>
                    <div class="mt-2 text-muted small">可為每張照片設定說明，單次上傳總大小上限 <%= PhotoUploadLimitDisplay %>。</div>
                </asp:Panel>
                <div id="photoPreview" class="d-flex flex-wrap gap-3 mt-3 photo-preview"></div>
                <asp:PlaceHolder ID="phPhotoList" runat="server" Visible="false">
                    <asp:Repeater ID="rptPhotos" runat="server" OnItemCommand="rptPhotos_ItemCommand" OnItemDataBound="rptPhotos_ItemDataBound">
                        <ItemTemplate>
                            <div class="photo-card">
                                <img src='<%# ResolveUrl(Eval("FilePath").ToString()) %>' alt='<%# Eval("Caption") %>' />
                                <div class="mt-2"><%# Eval("Caption") %></div>
                                <asp:HiddenField ID="hfPhotoCaption" runat="server" Value='<%# Eval("Caption") %>' />
                                <asp:Panel ID="pnlCover" runat="server" Visible='<%# Convert.ToBoolean(Eval("IsCover")) %>'>
                                    <span class="badge">封面</span>
                                </asp:Panel>
                                <div class="actions">
                                    <asp:LinkButton ID="btnSetCover" runat="server" CommandName="SetCover" CommandArgument='<%# Eval("PhotoID") %>' CssClass="btn_action btn-sm">設為封面</asp:LinkButton>
                                    <asp:LinkButton ID="btnRemovePhoto" runat="server" CommandName="DeletePhoto" CommandArgument='<%# Eval("PhotoID") %>' CssClass="btn_danger btn-sm" OnClientClick="return confirm('確認刪除照片？');">刪除</asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:PlaceHolder>
                <asp:Panel ID="pnlPhotosEmpty" runat="server" CssClass="alert alert-info mt-3" Visible="false">
                    目前沒有已上傳的樹籍照片。
                </asp:Panel>
            </div>
            <div class="pageBlock mt-3 text-end">
                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn_action" OnClick="btnSave_Click">儲存</asp:LinkButton>
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn_def" OnClick="btnBack_Click" CausesValidation="false">返回列表</asp:LinkButton>
            </div>
            <div class="pageBlock mt-3">
                <div class="boxTitle">資料操作紀錄</div>
                <asp:PlaceHolder ID="phLogList" runat="server" Visible="false">
                    <asp:Repeater ID="rptLogs" runat="server">
                        <HeaderTemplate>
                            <div class="table-responsive">
                                <table class="tb">
                                    <tr>
                                        <th>操作時間</th>
                                        <th>動作類型</th>
                                        <th>使用者帳號</th>
                                        <th>使用者姓名</th>
                                        <th>使用者單位</th>
                                        <th>IP 位置</th>
                                        <th>備註</th>
                                    </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("LogDateTime", "{0:yyyy/MM/dd HH:mm}") %></td>
                                        <td><%# Eval("ActionType") %></td>
                                        <td><%# Eval("Account") %></td>
                                        <td><%# Eval("AccountName") %></td>
                                        <td><%# Eval("AccountUnit") %></td>
                                        <td><%# Eval("IPAddress") %></td>
                                        <td class="text-start"><%# Eval("Memo") %></td>
                                    </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </table>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="phLogPager" runat="server" Visible="false">
                        <div class="log-pager">
                            <span class="log-pager-info">
                                <asp:Literal ID="litLogSummary" runat="server" />
                            </span>
                            <div class="pager-buttons">
                                <asp:Repeater ID="rptLogPager" runat="server" OnItemCommand="rptLogPager_ItemCommand">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnPage" runat="server" CssClass='<%# Convert.ToBoolean(Eval("IsCurrent")) ? "pager-link active" : "pager-link" %>' CommandName="Page" CommandArgument='<%# Eval("Index") %>' CausesValidation="false"><%# Eval("Text") %></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
                <asp:Panel ID="pnlLogsEmpty" runat="server" CssClass="alert alert-info mt-3" Visible="false">
                    目前尚無資料操作紀錄。
                </asp:Panel>
            </div>
        </asp:View>

    </asp:MultiView>

    <div id="coordinateModal" class="modal-overlay" aria-hidden="true">
        <div class="modal-dialog-custom" role="dialog" aria-modal="true" aria-labelledby="coordinateModalTitle">
            <div class="modal-header d-flex justify-content-between align-items-center">
                <h5 id="coordinateModalTitle" class="mb-0">座標轉換工具</h5>
                <button type="button" class="btn-close" aria-label="Close" data-close="coordinateModal">&times;</button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="modalTwd97X" class="form-label">TWD97 X</label>
                    <input type="text" id="modalTwd97X" class="form-control" />
                </div>
                <div class="mb-3">
                    <label for="modalTwd97Y" class="form-label">TWD97 Y</label>
                    <input type="text" id="modalTwd97Y" class="form-control" />
                </div>
                <div class="d-flex gap-2 flex-wrap">
                    <button type="button" id="btnModalTwdToWgs" class="btn_action">97 轉經緯度</button>
                    <button type="button" id="btnModalWgsToTwd" class="btn_def" style="display:none">經緯度轉 97</button>
                </div>
                <div class="row mt-3 g-2">
                    <div class="col-md-6">
                        <label for="modalLatitude" class="form-label">緯度 (N)</label>
                        <input type="text" id="modalLatitude" class="form-control" readonly />
                    </div>
                    <div class="col-md-6">
                        <label for="modalLongitude" class="form-label">經度 (E)</label>
                        <input type="text" id="modalLongitude" class="form-control" readonly />
                    </div>
                </div>
                <div class="text-muted small mt-2">轉換後的經緯度會自動帶入表單欄位。</div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn_def" data-close="coordinateModal">關閉</button>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        (function () {
            function $(id) { return document.getElementById(id); }

            function toggleAnnouncement() {
            
                var ddl = $("<%= ddlDetailStatus.ClientID %>");
                var panel = $("<%= pnlAnnouncementDate.ClientID %>");
                if (!ddl || !panel) return;
                var shouldShow = ddl.value === "1";
                panel.style.display = shouldShow ? "" : "none";
            }

            function toggleBatchAnnouncement() {
                var ddl = $("<%= ddlBatchStatus.ClientID %>");
                var wrapper = $("batchAnnouncementWrapper");
                if (!ddl || !wrapper) return;
                var shouldShow = ddl.value === "1";
                wrapper.style.display = shouldShow ? "" : "none";
            }

            function initPhotoDropZone() {
                var input = $("<%= fuPhoto.ClientID %>");
                var drop = $("photoDrop");
                var preview = $("photoPreview");
                if (!input || !drop || !preview || !window.DataTransfer) return;

                var dt = new DataTransfer();
                var metadata = new Map();

                function getFileKey(file) {
                    return [file.name, file.size, file.lastModified].join("_");
                }

                function captureMetadata() {
                    var current = new Map();
                    Array.from(preview.querySelectorAll(".photo-item")).forEach(function (item) {
                        var key = item.getAttribute("data-file-key");
                        if (!key) return;
                        var captionInput = item.querySelector("[data-role='photoCaption']");
                        current.set(key, {
                            caption: captionInput ? captionInput.value : ""
                        });
                    });
                    metadata = current;
                }

                function refreshPreview() {
                    captureMetadata();
                    preview.innerHTML = "";
                    Array.from(dt.files).forEach(function (file, idx) {
                        var key = getFileKey(file);
                        var data = metadata.get(key) || {};
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            var card = document.createElement("div");
                            card.className = "photo-item";
                            card.setAttribute("data-file-key", key);

                            var remove = document.createElement("button");
                            remove.type = "button";
                            remove.className = "btn-close";
                            remove.setAttribute("aria-label", "remove");
                            remove.textContent = "×";
                            remove.addEventListener("click", function () {
                                var removeIndex = Array.from(dt.files).findIndex(function (f) { return getFileKey(f) === key; });
                                if (removeIndex >= 0) {
                                    dt.items.remove(removeIndex);
                                }
                                metadata.delete(key);
                                refreshPreview();
                            });

                            var img = document.createElement("img");
                            img.src = e.target.result;
                            img.alt = file.name;

                            var fields = document.createElement("div");
                            fields.className = "photo-fields";

                            var captionLabel = document.createElement("label");
                            captionLabel.className = "form-label small mb-1";
                            captionLabel.textContent = "照片說明";

                            var captionInput = document.createElement("input");
                            captionInput.type = "text";
                            captionInput.name = "photoCaptionList_" + idx;
                            captionInput.setAttribute("data-role", "photoCaption");
                            captionInput.className = "form-control form-control-sm";
                            var defaultCaption = data.caption || file.name.replace(/\.[^/.]+$/, "");
                            captionInput.value = defaultCaption;
                            captionInput.addEventListener("input", function () {
                                var meta = metadata.get(key) || {};
                                meta.caption = captionInput.value;
                                metadata.set(key, meta);
                            });

                            fields.appendChild(captionLabel);
                            fields.appendChild(captionInput);

                            card.appendChild(remove);
                            card.appendChild(img);
                            card.appendChild(fields);
                            preview.appendChild(card);

                            var meta = metadata.get(key) || {};
                            meta.caption = captionInput.value;
                            metadata.set(key, meta);
                        };
                        reader.readAsDataURL(file);
                    });

                    input.files = dt.files;
                }

                function getCurrentTotalSize() {
                    return Array.from(dt.files).reduce(function (sum, file) { return sum + (file.size || 0); }, 0);
                }

                var maxTotalBytes = parseInt(input.getAttribute("data-max-total-bytes"), 10);
                if (isNaN(maxTotalBytes)) {
                    maxTotalBytes = 0;
                }
                var maxTotalDisplay = input.getAttribute("data-max-total-display") || "";

                function handleFiles(fileList) {
                    var images = Array.from(fileList).filter(function (file) {
                        return file && file.type && file.type.startsWith("image/");
                    });

                    if (images.length === 0) {
                        return;
                    }

                    if (maxTotalBytes > 0) {
                        var currentTotal = getCurrentTotalSize();
                        var incomingTotal = images.reduce(function (sum, file) { return sum + (file.size || 0); }, 0);
                        if (currentTotal + incomingTotal > maxTotalBytes) {
                            var message = "選擇的照片總大小超過限制";
                            if (maxTotalDisplay) {
                                message += "（" + maxTotalDisplay + "）";
                            }
                            message += "，請分批上傳。";
                            alert(message);
                            return;
                        }
                    }

                    images.forEach(function (file) {
                        dt.items.add(file);
                    });
                    refreshPreview();
                }

                drop.addEventListener("dragover", function (e) {
                    e.preventDefault();
                    drop.classList.add("dragover");
                });
                drop.addEventListener("dragleave", function (e) {
                    e.preventDefault();
                    drop.classList.remove("dragover");
                });
                drop.addEventListener("drop", function (e) {
                    e.preventDefault();
                    drop.classList.remove("dragover");
                    handleFiles(e.dataTransfer.files);
                });
                drop.addEventListener("click", function () {
                    input.click();
                });

                input.addEventListener("change", function () {
                    handleFiles(this.files);
                    this.value = "";
                });
            }

            function initCoordinateTool() {
                var openButton = $("btnOpenCoordinate");
                var modal = $("coordinateModal");
                var latInput = $("<%= txtLatitude.ClientID %>");
                var lngInput = $("<%= txtLongitude.ClientID %>");
                var modalLat = $("modalLatitude");
                var modalLng = $("modalLongitude");
                var modalX = $("modalTwd97X");
                var modalY = $("modalTwd97Y");
                var btnTwdToWgs = $("btnModalTwdToWgs");
                var btnWgsToTwd = $("btnModalWgsToTwd");

                if (!openButton || !modal || !latInput || !lngInput || !modalLat || !modalLng || !modalX || !modalY || !btnTwdToWgs || !btnWgsToTwd) return;

                function openModal() {
                    modal.classList.add("active");
                    modal.setAttribute("aria-hidden", "false");
                }

                function closeModal() {
                    modal.classList.remove("active");
                    modal.setAttribute("aria-hidden", "true");
                }

                function populateFromWgs() {
                    var lat = parseFloat(latInput.value);
                    var lon = parseFloat(lngInput.value);
                    if (!isNaN(lat) && !isNaN(lon)) {
                        modalLat.value = lat.toFixed(6);
                        modalLng.value = lon.toFixed(6);
                        var result = wgs84ToTwd97(lat, lon);
                        modalX.value = result.x.toFixed(3);
                        modalY.value = result.y.toFixed(3);
                    } else {
                        modalLat.value = latInput.value;
                        modalLng.value = lngInput.value;
                        modalX.value = "";
                        modalY.value = "";
                    }
                }

                openButton.addEventListener("click", function () {
                    populateFromWgs();
                    openModal();
                    modalX.focus();
                });

                Array.from(modal.querySelectorAll('[data-close="coordinateModal"]')).forEach(function (btn) {
                    btn.addEventListener("click", function () {
                        closeModal();
                    });
                });

                modal.addEventListener("click", function (e) {
                    if (e.target === modal) {
                        closeModal();
                    }
                });

                document.addEventListener("keydown", function (e) {
                    if (e.key === "Escape" && modal.classList.contains("active")) {
                        closeModal();
                    }
                });

                btnTwdToWgs.addEventListener("click", function () {
                    var x = parseFloat(modalX.value);
                    var y = parseFloat(modalY.value);
                    if (isNaN(x) || isNaN(y)) {
                        alert("請輸入有效的TWD97座標");
                        return;
                    }
                    var result = twd97ToWgs84(x, y);
                    modalLat.value = result.lat.toFixed(6);
                    modalLng.value = result.lon.toFixed(6);
                    latInput.value = modalLat.value;
                    lngInput.value = modalLng.value;
                    closeModal();
                });

                btnWgsToTwd.addEventListener("click", function () {
                    var lat = parseFloat(latInput.value);
                    var lon = parseFloat(lngInput.value);
                    if (isNaN(lat) || isNaN(lon)) {
                        alert("請輸入有效的經緯度");
                        return;
                    }
                    var result = wgs84ToTwd97(lat, lon);
                    modalX.value = result.x.toFixed(3);
                    modalY.value = result.y.toFixed(3);
                    modalLat.value = lat.toFixed(6);
                    modalLng.value = lon.toFixed(6);
                });
            }

            function wgs84ToTwd97(lat, lon) {
                var a = 6378137.0;
                var b = 6356752.314245;
                var lon0 = 121 * Math.PI / 180;
                var k0 = 0.9999;
                var dx = 250000;
                var dy = 0;
                var e = Math.sqrt(1 - Math.pow(b, 2) / Math.pow(a, 2));
                var e2 = (Math.pow(e, 2)) / (1 - Math.pow(e, 2));

                lat = lat * Math.PI / 180;
                lon = lon * Math.PI / 180;

                var N = a / Math.sqrt(1 - Math.pow(e * Math.sin(lat), 2));
                var T = Math.pow(Math.tan(lat), 2);
                var C = e2 * Math.pow(Math.cos(lat), 2);
                var A = Math.cos(lat) * (lon - lon0);

                var M = a * ((1 - Math.pow(e, 2) / 4 - 3 * Math.pow(e, 4) / 64 - 5 * Math.pow(e, 6) / 256) * lat
                    - (3 * Math.pow(e, 2) / 8 + 3 * Math.pow(e, 4) / 32 + 45 * Math.pow(e, 6) / 1024) * Math.sin(2 * lat)
                    + (15 * Math.pow(e, 4) / 256 + 45 * Math.pow(e, 6) / 1024) * Math.sin(4 * lat)
                    - (35 * Math.pow(e, 6) / 3072) * Math.sin(6 * lat));

                var x = dx + k0 * N * (A + (1 - T + C) * Math.pow(A, 3) / 6
                    + (5 - 18 * T + Math.pow(T, 2) + 72 * C - 58 * e2) * Math.pow(A, 5) / 120);
                var y = dy + k0 * (M + N * Math.tan(lat) * (Math.pow(A, 2) / 2
                    + (5 - T + 9 * C + 4 * Math.pow(C, 2)) * Math.pow(A, 4) / 24
                    + (61 - 58 * T + Math.pow(T, 2) + 600 * C - 330 * e2) * Math.pow(A, 6) / 720));

                return { x: x, y: y };
            }

            function twd97ToWgs84(x, y) {
                var a = 6378137.0;
                var b = 6356752.314245;
                var lon0 = 121 * Math.PI / 180;
                var k0 = 0.9999;
                var dx = 250000;
                var dy = 0;
                var e = Math.sqrt(1 - Math.pow(b, 2) / Math.pow(a, 2));
                var e2 = (Math.pow(e, 2)) / (1 - Math.pow(e, 2));

                x = x - dx;
                y = y - dy;

                var M = y / k0;
                var mu = M / (a * (1 - Math.pow(e, 2) / 4 - 3 * Math.pow(e, 4) / 64 - 5 * Math.pow(e, 6) / 256));
                var e1 = (1 - Math.sqrt(1 - Math.pow(e, 2))) / (1 + Math.sqrt(1 - Math.pow(e, 2)));

                var J1 = (3 * e1 / 2 - 27 * Math.pow(e1, 3) / 32);
                var J2 = (21 * Math.pow(e1, 2) / 16 - 55 * Math.pow(e1, 4) / 32);
                var J3 = (151 * Math.pow(e1, 3) / 96);
                var J4 = (1097 * Math.pow(e1, 4) / 512);

                var fp = mu + J1 * Math.sin(2 * mu) + J2 * Math.sin(4 * mu) + J3 * Math.sin(6 * mu) + J4 * Math.sin(8 * mu);

                var C1 = e2 * Math.pow(Math.cos(fp), 2);
                var T1 = Math.pow(Math.tan(fp), 2);
                var R1 = a * (1 - Math.pow(e, 2)) / Math.pow(1 - Math.pow(e * Math.sin(fp), 2), 1.5);
                var N1 = a / Math.sqrt(1 - Math.pow(e * Math.sin(fp), 2));
                var D = x / (N1 * k0);

                var Q1 = N1 * Math.tan(fp) / R1;
                var Q2 = (Math.pow(D, 2) / 2);
                var Q3 = (5 + 3 * T1 + 10 * C1 - 4 * Math.pow(C1, 2) - 9 * e2) * Math.pow(D, 4) / 24;
                var Q4 = (61 + 90 * T1 + 298 * C1 + 45 * Math.pow(T1, 2) - 252 * e2 - 3 * Math.pow(C1, 2)) * Math.pow(D, 6) / 720;
                var lat = fp - Q1 * (Q2 - Q3 + Q4);

                var Q5 = D;
                var Q6 = (1 + 2 * T1 + C1) * Math.pow(D, 3) / 6;
                var Q7 = (5 - 2 * C1 + 28 * T1 - 3 * Math.pow(C1, 2) + 8 * e2 + 24 * Math.pow(T1, 2)) * Math.pow(D, 5) / 120;
                var lon = lon0 + (Q5 - Q6 + Q7) / Math.cos(fp);

                return { lat: lat * 180 / Math.PI, lon: lon * 180 / Math.PI };
            }

            document.addEventListener("DOMContentLoaded", function () {
                toggleAnnouncement();
                toggleBatchAnnouncement();
                initPhotoDropZone();
                initCoordinateTool();

                var ddlStatus = $("<%= ddlDetailStatus.ClientID %>");
                if (ddlStatus) {
                    ddlStatus.addEventListener("change", toggleAnnouncement);
                }
                var ddlBatch = $("<%= ddlBatchStatus.ClientID %>");
                if (ddlBatch) {
                    ddlBatch.addEventListener("change", toggleBatchAnnouncement);
                }
            });
        })();
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
