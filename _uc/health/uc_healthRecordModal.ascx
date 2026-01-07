<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_healthRecordModal.ascx.cs" Inherits="protectTreesV2._uc.health.uc_healthRecordModal" %>
<div class="modalForm">
    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="true">
        <div class="text-center text-muted py-4">尚未載入健檢紀錄。</div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phContent" runat="server" Visible="false">
        <%-- 1. 基本資料 --%>
        <div class="formCard card mb-4">
            <div class="card-header">基本資料</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">系統紀錄編號</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litHealthId" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">系統樹籍編號</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litSystemTreeNo" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">資料狀態</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litStatus" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">所在地</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litLocation" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">樹種及學名</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litSpecies" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">樹牌</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTreeSign" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">最後更新</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litLastUpdate" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">調查日期</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litSurveyDate" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">調查人姓名</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litSurveyor" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">備註</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litMemo" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- 2. 一般調查 --%>
        <div class="formCard card mb-4">
            <div class="card-header">一般調查</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">座標(WGS84)：緯度(N)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litLatitude" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">座標(WGS84)：經度(E)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litLongitude" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">樹高 (m)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTreeHeight" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">樹冠投影面積 (m²)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litCanopyArea" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">米圍 (1.0m處)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litGirth100" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">米徑 (1.0m處)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litDiameter100" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">胸圍 (1.3m處)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litGirth130" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">胸徑 (1.3m處)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litDiameter130" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">備註 (上移或下移實際量測高度)</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litMeasureNote" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- 3. 病蟲害調查 --%>
        <div class="formCard card mb-4">
            <div class="card-header">病蟲害調查</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-12">
                        <label class="form-label text-muted">重大病害</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litMajorDisease" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-12">
                        <hr class="my-1" /> <%-- 分隔線，可選 --%>
                        <label class="form-label text-muted">重大蟲害</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litMajorPest" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-12">
                        <hr class="my-1" />
                        <label class="form-label text-muted">一般蟲害</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litGeneralPest" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-12">
                        <hr class="my-1" />
                        <label class="form-label text-muted">一般病害</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litGeneralDisease" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-12">
                        <hr class="my-1" />
                        <label class="form-label text-muted">其他病蟲害備註</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litPestOtherNote" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- 4. 樹木生長外觀情況 --%>
        <div class="formCard card mb-4">
            <div class="card-header">樹木生長外觀情況</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">根系</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litRootStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">樹基部</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litBaseStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">主幹</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTrunkStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">枝幹</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litBranchStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">樹冠</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litCrownStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- 5. 樹木修剪與支撐情況 --%>
        <div class="formCard card mb-4">
            <div class="card-header">樹木修剪與支撐情況</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">樹木修剪情況</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litPruningStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">樹木支撐情況</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litSupportStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- 6. 生育地環境與土壤檢測情況 --%>
        <div class="formCard card mb-4">
            <div class="card-header">生育地環境與土壤檢測情況</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">生育地環境</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litSiteStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                    <div class="col-md-6 col-12">
                        <label class="form-label text-muted">土壤檢測</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litSoilStatus" runat="server" Mode="PassThrough" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- 7. 健康檢查結果及風險評估 --%>
        <div class="formCard card mb-4">
            <div class="card-header">健康檢查結果及風險評估</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">管理情況</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litManagementStatus" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">建議處理優先順序</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litPriority" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">處理情形說明</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTreatmentDescription" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12 text-muted small mt-3">
                        <i class="fas fa-info-circle"></i> 本健康檢查結果之建議及評估為調查當下紀錄之情形。
                    </div>
                </div>
            </div>
        </div>

        <%-- 8. 附件與照片 --%>
        <div class="formCard card mb-4">
            <div class="card-header">附件與照片</div>
            <div class="card-body">
                <%-- 附件區塊 --%>
                <div class="mb-4">
                    <label class="form-label fw-bold mb-2">附件列表</label>
                    <asp:PlaceHolder ID="phAttachmentEmpty" runat="server" Visible="false">
                        <div class="text-muted">無附件。</div>
                    </asp:PlaceHolder>
                    <asp:Repeater ID="rptAttachments" runat="server">
                        <HeaderTemplate>
                            <ul class="list-unstyled mb-0">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="mb-1">
                                <a href='<%# string.IsNullOrEmpty(Eval("FilePath") as string) ? "#" : ResolveUrl(Eval("FilePath").ToString()) %>' target="_blank" rel="noopener">
                                    <i class="fas fa-paperclip me-1"></i> <%# Eval("FileName") %>
                                </a>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <hr />

                <%-- 照片區塊 --%>
                <div class="mt-4 mb-3">
                     <label class="form-label fw-bold mb-2">照片列表</label>
                    <asp:PlaceHolder ID="phPhotoEmpty" runat="server" Visible="false">
                        <div class="text-muted">無照片。</div>
                    </asp:PlaceHolder>
                    <asp:Repeater ID="rptPhotos" runat="server">
                        <HeaderTemplate>
                            <div class="row g-3">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="col-lg-3 col-md-4 col-sm-6 col-12">
                                <a href='<%# string.IsNullOrEmpty(Eval("FilePath") as string) ? "#" : ResolveUrl(Eval("FilePath").ToString()) %>' target="_blank" rel="noopener">
                                    <img src='<%# string.IsNullOrEmpty(Eval("FilePath") as string) ? "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==" : ResolveUrl(Eval("FilePath").ToString()) %>' alt='<%# Eval("FileName") %>' class="img-fluid rounded border shadow-sm" />
                                </a>
                                <div class="small text-muted mt-2 text-truncate" title='<%# Eval("FileName") %>'><%# Eval("FileName") %></div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</div>