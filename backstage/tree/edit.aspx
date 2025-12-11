<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="protectTreesV2.backstage.tree.edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    樹籍管理 / 新增或編輯樹籍
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    樹籍資料
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hfTreeID" runat="server" />
            <div class="row">
                <div class="col">
                    <h4>基本資料</h4>
                    <div class="row">
                        <div class="col">
                            系統樹籍編號：<asp:Label ID="lblSystemTreeNo" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtAgencyTreeNo" Text="機關樹木編號" />
                            <asp:TextBox ID="txtAgencyTreeNo" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtJurisdiction" Text="機關管轄編碼" />
                            <asp:TextBox ID="txtJurisdiction" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="ddlCity" Text="縣市" />
                            <asp:DropDownList ID="ddlCity" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" />
                            <asp:DropDownList ID="ddlArea" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="ddlSpecies" Text="樹種" />
                            <asp:DropDownList ID="ddlSpecies" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="ddlStatus" Text="樹籍狀態" />
                            <asp:DropDownList ID="ddlStatus" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtSurveyDate" Text="調查日期" />
                            <asp:TextBox ID="txtSurveyDate" runat="server" TextMode="Date" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtSurveyor" Text="調查人員" />
                            <asp:TextBox ID="txtSurveyor" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtAnnouncementDate" Text="公告日期" />
                            <asp:TextBox ID="txtAnnouncementDate" runat="server" TextMode="Date" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtTreeCount" Text="叢生株數" />
                            <asp:TextBox ID="txtTreeCount" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtLatitude" Text="座標(WGS84) 緯度(N)" />
                            <asp:TextBox ID="txtLatitude" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtLongitude" Text="座標(WGS84) 經度(E)" />
                            <asp:TextBox ID="txtLongitude" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="btnCoordinateTool" Text="座標轉換" />
                            <asp:Button ID="btnCoordinateTool" runat="server" Text="轉換" CausesValidation="false" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtSite" Text="坐落地點" />
                            <asp:TextBox ID="txtSite" runat="server" TextMode="MultiLine" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtManager" Text="管理人員" />
                            <asp:TextBox ID="txtManager" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtManagerContact" Text="管理人聯絡資訊" />
                            <asp:TextBox ID="txtManagerContact" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="ddlLandOwnership" Text="土地權屬" />
                            <asp:DropDownList ID="ddlLandOwnership" runat="server" />
                            <asp:TextBox ID="txtLandOwnershipNote" runat="server" placeholder="其他說明" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtFacility" Text="管理設施描述" />
                            <asp:TextBox ID="txtFacility" runat="server" TextMode="MultiLine" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtMemo" Text="其他備註" />
                            <asp:TextBox ID="txtMemo" runat="server" TextMode="MultiLine" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <h4>樹木尺寸與狀態</h4>
                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtTreeHeight" Text="樹高" />
                            <asp:TextBox ID="txtTreeHeight" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtBreastHeightDiameter" Text="胸高直徑" />
                            <asp:TextBox ID="txtBreastHeightDiameter" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtBreastHeightCircumference" Text="胸高樹圍" />
                            <asp:TextBox ID="txtBreastHeightCircumference" runat="server" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtCanopyArea" Text="樹冠投影面積" />
                            <asp:TextBox ID="txtCanopyArea" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="cblRecognition" Text="受保護認定理由" />
                            <asp:CheckBoxList ID="cblRecognition" runat="server" RepeatDirection="Vertical" />
                            <asp:TextBox ID="txtRecognitionNote" runat="server" TextMode="MultiLine" placeholder="認定理由備註說明" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtCulturalHistory" Text="文化歷史價值介紹" />
                            <asp:TextBox ID="txtCulturalHistory" runat="server" TextMode="MultiLine" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtHealth" Text="樹木健康及生育地概況" />
                            <asp:TextBox ID="txtHealth" runat="server" TextMode="MultiLine" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <asp:Button ID="btnSaveDraft" runat="server" Text="暫存草稿" OnClick="btnSaveDraft_Click" />
                    <asp:Button ID="btnSaveFinal" runat="server" Text="存檔送出" OnClick="btnSaveFinal_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="返回列表" CausesValidation="false" OnClick="btnCancel_Click" />
                    <asp:HyperLink ID="lnkUploadPhotos" runat="server" Text="樹木照片" NavigateUrl="edit_photos.aspx" />
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
