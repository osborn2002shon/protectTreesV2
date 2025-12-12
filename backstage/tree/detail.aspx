<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="detail.aspx.cs" Inherits="protectTreesV2.backstage.tree.detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
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
            <div class="row">
                <div class="col">
                    <h4>基本資料</h4>
                    <div class="row">
                        <div class="col">系統樹籍編號：<asp:Label ID="lblSystemTreeNo" runat="server"></asp:Label></div>
                        <div class="col">機關樹木編號：<asp:Label ID="lblAgencyTreeNo" runat="server" /></div>
                        <div class="col">機關管轄編碼：<asp:Label ID="lblJurisdiction" runat="server" /></div>
                    </div>
                    <div class="row">
                        <div class="col">縣市：<asp:Label ID="lblCity" runat="server" /></div>
                        <div class="col">行政區：<asp:Label ID="lblArea" runat="server" /></div>
                        <div class="col">樹種：<asp:Label ID="lblSpecies" runat="server" /></div>
                    </div>
                    <div class="row">
                        <div class="col">樹籍狀態：<asp:Label ID="lblStatus" runat="server" /></div>
                        <div class="col">編輯狀態：<asp:Label ID="lblEditStatus" runat="server" /></div>
                        <div class="col">調查日期：<asp:Label ID="lblSurveyDate" runat="server" /></div>
                    </div>
                    <div class="row">
                        <div class="col">公告日期：<asp:Label ID="lblAnnouncementDate" runat="server" /></div>
                        <div class="col">管理人員：<asp:Label ID="lblManager" runat="server" /></div>
                        <div class="col">管理人聯絡資訊：<asp:Label ID="lblManagerContact" runat="server" /></div>
                    </div>
                    <div class="row">
                        <div class="col">坐落地點：<asp:Label ID="lblSite" runat="server" /></div>
                        <div class="col">土地權屬：<asp:Label ID="lblLandOwnership" runat="server" /></div>
                        <div class="col">土地權屬備註：<asp:Label ID="lblLandOwnershipNote" runat="server" /></div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <h4>尺寸與狀態</h4>
                    <div class="row">
                        <div class="col">叢生株數：<asp:Label ID="lblTreeCount" runat="server" /></div>
                        <div class="col">樹高：<asp:Label ID="lblTreeHeight" runat="server" /></div>
                        <div class="col">胸高直徑：<asp:Label ID="lblBreastHeightDiameter" runat="server" /></div>
                        <div class="col">胸高樹圍：<asp:Label ID="lblBreastHeightCircumference" runat="server" /></div>
                        <div class="col">樹冠投影面積：<asp:Label ID="lblCanopyArea" runat="server" /></div>
                    </div>
                    <div class="row">
                        <div class="col">座標(WGS84) 緯度(N)：<asp:Label ID="lblLatitude" runat="server" /></div>
                        <div class="col">座標(WGS84) 經度(E)：<asp:Label ID="lblLongitude" runat="server" /></div>
                        <div class="col">公告是否公開：<asp:Label ID="lblIsAnnounced" runat="server" /></div>
                    </div>
                    <div class="row">
                        <div class="col">受保護認定理由：<asp:Literal ID="ltlRecognition" runat="server" Mode="PassThrough" /></div>
                        <div class="col">認定理由備註：<asp:Label ID="lblRecognitionNote" runat="server" /></div>
                        <div class="col">文化歷史價值介紹：<asp:Label ID="lblCulturalHistory" runat="server" /></div>
                    </div>
                    <div class="row">
                        <div class="col">樹木健康及生育地概況：<asp:Label ID="lblHealth" runat="server" /></div>
                        <div class="col">其他備註：<asp:Label ID="lblMemo" runat="server" /></div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <h4>照片</h4>
                    <asp:Repeater ID="rptPhotos" runat="server">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col">
                                    <asp:Image runat="server" ImageUrl='<%# Eval("FilePath") %>' Width="200" />
                                </div>
                                <div class="col">
                                    <asp:Label runat="server" Text='<%# Eval("Caption") %>' />
                                    <asp:Label runat="server" Text='<%# (bool)Eval("IsCover") ? " (封面)" : string.Empty %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <asp:HyperLink ID="lnkBackToList" runat="server" Text="返回列表" NavigateUrl="query.aspx" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
