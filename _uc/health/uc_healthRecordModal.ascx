<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_healthRecordModal.ascx.cs" Inherits="protectTreesV2._uc.health.uc_healthRecordModal" %>
<div class="modalForm">
    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="true">
        <div class="text-center text-muted py-4">尚未載入健檢紀錄。</div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phContent" runat="server" Visible="false">
        <div class="boxTitle_mini">基本資料</div>
        <div class="row g-3">
            <div class="col-md-4 col-sm-6">
                <span>系統紀錄編號</span>
                <div class="fw-semibold"><asp:Literal ID="litHealthId" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>系統樹籍編號</span>
                <div class="fw-semibold text-primary"><asp:Literal ID="litSystemTreeNo" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>資料狀態</span>
                <div><asp:Literal ID="litStatus" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>所在地</span>
                <div><asp:Literal ID="litLocation" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>樹種及學名</span>
                <div><asp:Literal ID="litSpecies" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>樹牌</span>
                <div><asp:Literal ID="litTreeSign" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>最後更新</span>
                <div><asp:Literal ID="litLastUpdate" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>調查日期</span>
                <div><asp:Literal ID="litSurveyDate" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>調查人姓名</span>
                <div><asp:Literal ID="litSurveyor" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-12">
                <span>備註</span>
                <div><asp:Literal ID="litMemo" runat="server" Mode="Encode" /></div>
            </div>
        </div>

        <div class="boxTitle_mini mt-4">一般調查</div>
        <div class="row g-3">
            <div class="col-md-3 col-sm-6">
                <span>座標(WGS84)：緯度(N)</span>
                <div><asp:Literal ID="litLatitude" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-3 col-sm-6">
                <span>座標(WGS84)：經度(E)</span>
                <div><asp:Literal ID="litLongitude" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-3 col-sm-6">
                <span>樹高 (m)</span>
                <div><asp:Literal ID="litTreeHeight" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-3 col-sm-6">
                <span>樹冠投影面積 (m²)</span>
                <div><asp:Literal ID="litCanopyArea" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-3 col-sm-6">
                <span>米圍 (1.0m處)</span>
                <div><asp:Literal ID="litGirth100" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-3 col-sm-6">
                <span>米徑 (1.0m處)</span>
                <div><asp:Literal ID="litDiameter100" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-3 col-sm-6">
                <span>胸圍 (1.3m處)</span>
                <div><asp:Literal ID="litGirth130" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-3 col-sm-6">
                <span>胸徑 (1.3m處)</span>
                <div><asp:Literal ID="litDiameter130" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-12">
                <span>備註 (上移或下移實際量測高度)</span>
                <div><asp:Literal ID="litMeasureNote" runat="server" Mode="Encode" /></div>
            </div>
        </div>

        <div class="boxTitle_mini mt-4">病蟲害調查</div>
        <div class="row g-3">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">重大病害</div>
            <div class="col">
                <div><asp:Literal ID="litMajorDisease" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">重大蟲害</div>
            <div class="col">
                <div><asp:Literal ID="litMajorPest" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">一般蟲害</div>
            <div class="col">
                <div><asp:Literal ID="litGeneralPest" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">一般病害</div>
            <div class="col">
                <div><asp:Literal ID="litGeneralDisease" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">其他病蟲害備註</div>
            <div class="col">
                <div><asp:Literal ID="litPestOtherNote" runat="server" Mode="Encode" /></div>
            </div>
        </div>

        <div class="boxTitle_mini mt-4">樹木生長外觀情況</div>
        <div class="row g-3">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">根系</div>
            <div class="col">
                <div><asp:Literal ID="litRootStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">樹基部</div>
            <div class="col">
                <div><asp:Literal ID="litBaseStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">主幹</div>
            <div class="col">
                <div><asp:Literal ID="litTrunkStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">枝幹</div>
            <div class="col">
                <div><asp:Literal ID="litBranchStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">樹冠</div>
            <div class="col">
                <div><asp:Literal ID="litCrownStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>

        <div class="boxTitle_mini mt-4">樹木修剪與支撐情況</div>
        <div class="row g-3">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">樹木修剪情況</div>
            <div class="col">
                <div><asp:Literal ID="litPruningStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">樹木支撐情況</div>
            <div class="col">
                <div><asp:Literal ID="litSupportStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>

        <div class="boxTitle_mini mt-4">生育地環境與土壤檢測情況</div>
        <div class="row g-3">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">生育地環境</div>
            <div class="col">
                <div><asp:Literal ID="litSiteStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>
        <div class="row g-3 mt-2">
            <div class="col-lg-2 col-md-3 col-sm-4 fw-semibold">土壤檢測</div>
            <div class="col">
                <div><asp:Literal ID="litSoilStatus" runat="server" Mode="PassThrough" /></div>
            </div>
        </div>

        <div class="boxTitle_mini mt-4">健康檢查結果及風險評估</div>
        <div class="row g-3">
            <div class="col-md-4 col-sm-6">
                <span>管理情況</span>
                <div><asp:Literal ID="litManagementStatus" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>建議處理優先順序</span>
                <div><asp:Literal ID="litPriority" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-md-4 col-sm-6">
                <span>處理情形說明</span>
                <div><asp:Literal ID="litTreatmentDescription" runat="server" Mode="Encode" /></div>
            </div>
            <div class="col-12 text-muted small">
                本健康檢查結果之建議及評估為調查當下紀錄之情形。
            </div>
        </div>

        <div class="boxTitle_mini mt-4">附件</div>
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
                        <%# Eval("FileName") %>
                    </a>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>

        <div class="boxTitle_mini mt-4">照片</div>
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
                        <img src='<%# string.IsNullOrEmpty(Eval("FilePath") as string) ? "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==" : ResolveUrl(Eval("FilePath").ToString()) %>' alt='<%# Eval("FileName") %>' class="img-fluid rounded border" />
                    </a>
                    <div class="small text-muted mt-2 text-truncate" title='<%# Eval("FileName") %>'><%# Eval("FileName") %></div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </asp:PlaceHolder>
</div>