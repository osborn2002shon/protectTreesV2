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
                            <asp:TextBox ID="txtTreeCount" runat="server" TextMode="Number" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtLatitude" Text="座標(WGS84) 緯度(N)" />
                            <asp:TextBox ID="txtLatitude" runat="server" TextMode="Number" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtLongitude" Text="座標(WGS84) 經度(E)" />
                            <asp:TextBox ID="txtLongitude" runat="server" TextMode="Number" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="btnCoordinateTool" Text="座標轉換" />
                            <asp:Button ID="btnCoordinateTool" runat="server" Text="座標轉換" CausesValidation="false" OnClientClick="openCoordinateModal(); return false;" />
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
                            <asp:TextBox ID="txtTreeHeight" runat="server" TextMode="Number" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtBreastHeightDiameter" Text="胸高直徑" />
                            <asp:TextBox ID="txtBreastHeightDiameter" runat="server" TextMode="Number" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtBreastHeightCircumference" Text="胸高樹圍" />
                            <asp:TextBox ID="txtBreastHeightCircumference" runat="server" TextMode="Number" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="txtCanopyArea" Text="樹冠投影面積" />
                            <asp:TextBox ID="txtCanopyArea" runat="server" TextMode="Number" />
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

    <div class="modal fade" id="coordinateModal" tabindex="-1" aria-labelledby="coordinateModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="coordinateModalLabel">座標轉換</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="twd97X">97座標 X（東向）</label>
                        <input type="number" class="form-control" id="twd97X" placeholder="例如：303000" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label" for="twd97Y">97座標 Y（北向）</label>
                        <input type="number" class="form-control" id="twd97Y" placeholder="例如：2765000" />
                    </div>
                    <div class="alert alert-secondary" role="alert">
                        請輸入TWD97二度分帶(121E)平面座標，系統會轉換為WGS84經緯度並填入表單。
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">關閉</button>
                    <button type="button" class="btn btn-primary" onclick="convertCoordinate();">轉換並填入</button>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        const latitudeInputId = '<%= txtLatitude.ClientID %>';
    const longitudeInputId = '<%= txtLongitude.ClientID %>';

        function openCoordinateModal() {
            const modalElement = document.getElementById('coordinateModal');
            const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
            modal.show();
        }

        function convertCoordinate() {
            const xInput = document.getElementById('twd97X');
            const yInput = document.getElementById('twd97Y');
            const x = parseFloat(xInput.value);
            const y = parseFloat(yInput.value);

            if (isNaN(x) || isNaN(y)) {
                alert('請輸入有效的97座標數值');
                return;
            }

            const result = twd97ToWgs84(x, y);
            document.getElementById(latitudeInputId).value = result.lat.toFixed(6);
            document.getElementById(longitudeInputId).value = result.lon.toFixed(6);

            const modalElement = document.getElementById('coordinateModal');
            const modal = bootstrap.Modal.getInstance(modalElement);
            modal.hide();
        }

        function twd97ToWgs84(x, y) {
            const a = 6378137.0;
            const b = 6356752.314245;
            const lon0 = 121 * Math.PI / 180;
            const k0 = 0.9999;
            const dx = 250000;
            const dy = 0;

            const e = Math.sqrt(1 - Math.pow(b, 2) / Math.pow(a, 2));
            const e2 = Math.pow(e, 2) / (1 - Math.pow(e, 2));

            const xAdjusted = x - dx;
            const yAdjusted = y - dy;

            const M = yAdjusted / k0;
            const mu = M / (a * (1.0 - Math.pow(e, 2) / 4.0 - 3 * Math.pow(e, 4) / 64.0 - 5 * Math.pow(e, 6) / 256.0));

            const e1 = (1.0 - Math.sqrt(1.0 - Math.pow(e, 2))) / (1.0 + Math.sqrt(1.0 - Math.pow(e, 2)));

            const J1 = (3 * e1 / 2 - 27 * Math.pow(e1, 3) / 32.0);
            const J2 = (21 * Math.pow(e1, 2) / 16 - 55 * Math.pow(e1, 4) / 32.0);
            const J3 = (151 * Math.pow(e1, 3) / 96.0);
            const J4 = (1097 * Math.pow(e1, 4) / 512.0);

            const fp = mu + J1 * Math.sin(2 * mu) + J2 * Math.sin(4 * mu) + J3 * Math.sin(6 * mu) + J4 * Math.sin(8 * mu);

            const sinFp = Math.sin(fp);
            const cosFp = Math.cos(fp);
            const tanFp = Math.tan(fp);

            const C1 = e2 * Math.pow(cosFp, 2);
            const T1 = Math.pow(tanFp, 2);
            const R1 = a * (1 - Math.pow(e, 2)) / Math.pow(1 - Math.pow(e, 2) * Math.pow(sinFp, 2), 1.5);
            const N1 = a / Math.sqrt(1 - Math.pow(e, 2) * Math.pow(sinFp, 2));
            const D = xAdjusted / (N1 * k0);

            const lat = fp - (N1 * tanFp / R1) * (Math.pow(D, 2) / 2 - (5 + 3 * T1 + 10 * C1 - 4 * Math.pow(C1, 2) - 9 * e2) * Math.pow(D, 4) / 24 + (61 + 90 * T1 + 298 * C1 + 45 * Math.pow(T1, 2) - 3 * Math.pow(C1, 2) - 252 * e2) * Math.pow(D, 6) / 720);

            const lon = lon0 + (D - (1 + 2 * T1 + C1) * Math.pow(D, 3) / 6 + (5 - 2 * C1 + 28 * T1 - 3 * Math.pow(C1, 2) + 8 * e2 + 24 * Math.pow(T1, 2)) * Math.pow(D, 5) / 120) / cosFp;

            return {
                lat: lat * 180 / Math.PI,
                lon: lon * 180 / Math.PI
            };
        }
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>


