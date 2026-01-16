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
<%--    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>
            <asp:HiddenField ID="hfTreeID" runat="server" />
            <asp:HiddenField ID="hfDeletedPhotos" runat="server" />
            <asp:HiddenField ID="hfCoverPhoto" runat="server" />
            <asp:HiddenField ID="hfNewPhotoKeys" runat="server" />
            <asp:HiddenField ID="hfIsFinal" runat="server" />
            <div class="d-flex flex-wrap align-items-center justify-content-between mb-3">
                <div class="d-flex flex-wrap align-items-center gap-3">
                    <asp:Label ID="lblEditMode" runat="server" CssClass="fw-semibold" />
                    <asp:Label ID="lblTopSystemTreeNo" runat="server" CssClass="text-muted" />
                </div>
            </div>
            <ul class="nav nav-tabs mb-4" id="treeEditTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link text-dark active" id="tab-basic" data-bs-toggle="tab" data-bs-target="#pane-basic" type="button" role="tab" aria-controls="pane-basic" aria-selected="true">基本資料</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link text-dark" id="tab-survey" data-bs-toggle="tab" data-bs-target="#pane-survey" type="button" role="tab" aria-controls="pane-survey" aria-selected="false">調查資料</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link text-dark" id="tab-photos" data-bs-toggle="tab" data-bs-target="#pane-photos" type="button" role="tab" aria-controls="pane-photos" aria-selected="false">樹木照片</button>
                </li>
            </ul>
            <div class="tab-content mt-3" id="treeEditTabContent">
                <div class="tab-pane fade show active" id="pane-basic" role="tabpanel" aria-labelledby="tab-basic">
                    <div class="row">
                        <div class="col">
                            <div class="formCard card">
                                <div class="card-header">基本資料</div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtJurisdiction" Text="機關管轄編碼" /><asp:TextBox ID="txtJurisdiction" runat="server" CssClass="form-control" placeholder="A12345" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtAgencyTreeNo" Text="機關樹木編號" /><asp:TextBox ID="txtAgencyTreeNo" runat="server" CssClass="form-control" placeholder="T-2024-001" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="ddlSpecies" Text="樹種及學名" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:DropDownList ID="ddlSpecies" runat="server" CssClass="form-select" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtTreeCount" Text="數量" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtTreeCount" runat="server" TextMode="Number" CssClass="form-control" placeholder="3" />
                                            <small class="form-text text-muted">當他為群生竹木請填寫實際株(棵)數</small>
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="ddlCity" Text="縣市鄉鎮" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <div class="d-flex gap-2">
                                                <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" />
                                                <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-select" />
                                            </div>
                                        </div>
                                        
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtSite" Text="坐落地點" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtSite" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="台北市中正區中山南路1號" />
                                        </div>
                                    </div>
                                    <div class="row align-items-end">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtLatitude" Text="座標(WGS84) 緯度(N)" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtLatitude" runat="server" TextMode="Number" Step="0.000001" CssClass="form-control" placeholder="25.032969" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtLongitude" Text="座標(WGS84) 經度(E)" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtLongitude" runat="server" TextMode="Number" Step="0.000001" CssClass="form-control" placeholder="121.565418" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="btnCoordinateTool" Text="座標轉換" />
                                            <asp:Button ID="btnCoordinateTool" runat="server" Text="座標轉換" CausesValidation="false" OnClientClick="openCoordinateModal(); return false;" CssClass="btn btn-primary" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <small class="form-text text-muted">請輸入精度最多至第六位的經緯度座標</small>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-2">
                                            <asp:Label runat="server" AssociatedControlID="ddlLandOwnership" Text="土地權屬" />
                                            <asp:DropDownList ID="ddlLandOwnership" runat="server" CssClass="form-select" />
                                        </div>
                                        <div class="col">
                                            土地權屬備註
                                            <asp:TextBox ID="txtLandOwnershipNote" runat="server" placeholder="與鄰地共用" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtManager" Text="管理人" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtManager" runat="server" CssClass="form-control" placeholder="王小明" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtManagerContact" Text="管理人聯絡電話" />
                                            <asp:TextBox ID="txtManagerContact" runat="server" CssClass="form-control" placeholder="0900123456" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtFacility" Text="管理設施描述" />
                                            <asp:TextBox ID="txtFacility" runat="server" CssClass="form-control" placeholder="周邊設置護欄" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="ddlStatus" Text="樹籍狀態" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select" />
                                        </div>
                                        <div class="col" id="announcementDateContainer">
                                            <asp:Label runat="server" AssociatedControlID="txtAnnouncementDate" Text="公告日期" /><span class="text-danger required-marker required-final" data-requirement="announcement">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtAnnouncementDate" runat="server" TextMode="Date" CssClass="form-control" placeholder="2024-05-15" />
                                        </div>
                                    </div>
                                    
                                    
                                    
                                    
                                    <div class="row" id="recognitionContainer">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="cblRecognition" Text="受保護認定理由" /><span class="text-danger required-marker required-final" data-requirement="recognition">*</span><i class="fa-solid fa-eye"></i>
                                            <asp:CheckBoxList ID="cblRecognition" runat="server" RepeatDirection="Vertical" />
                                        </div>
                                    </div>
                                        <div class="col">
                                            <div class="row">
                                                <div class="col">
                                                    <asp:Label runat="server" AssociatedControlID="txtRecognitionNote" Text="認定理由備註說明" />
                                                    <asp:TextBox ID="txtRecognitionNote" runat="server" TextMode="MultiLine" Height="150" placeholder="因具地方文化意義" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:Label runat="server" AssociatedControlID="txtCulturalHistory" Text="文化歷史價值介紹" /><span class="text-danger required-marker required-final">*</span><i class="fa-solid fa-eye"></i>
                                                    <asp:TextBox ID="txtCulturalHistory" runat="server" TextMode="MultiLine" Height="150" CssClass="form-control" placeholder="日治時期種植，陪伴社區成長" />
                                                </div>
                                            </div>
                                        </div>
                                   
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane fade" id="pane-survey" role="tabpanel" aria-labelledby="tab-survey">
                    <div class="row">
                        <div class="col">
                            <div class="formCard card">
                                <div class="card-header">調查資料</div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtSurveyDate" Text="調查日期" /><span class="text-danger required-marker required-final">*</span>
                                            <asp:TextBox ID="txtSurveyDate" runat="server" TextMode="Date" CssClass="form-control" placeholder="2024-05-01" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtSurveyor" Text="調查人姓名" />
                                            <asp:TextBox ID="txtSurveyor" runat="server" CssClass="form-control" placeholder="林大華" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtEstimatedPlantingYear" Text="推估種植年間" />
                                            <asp:TextBox ID="txtEstimatedPlantingYear" runat="server" CssClass="form-control" placeholder="約民國70年" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtEstimatedAgeNote" Text="推估年齡備註" />
                                            <asp:TextBox ID="txtEstimatedAgeNote" runat="server" CssClass="form-control" placeholder="樹齡推估依據老照片" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtGroupGrowthInfo" Text="群生竹木或行道樹生長資訊" />
                                            <asp:TextBox ID="txtGroupGrowthInfo" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="行道樹排列整齊，覆蓋完整" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtTreeHeight" Text="樹高" /><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtTreeHeight" runat="server" TextMode="Number" CssClass="form-control" placeholder="12.5" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtBreastHeightDiameter" Text="胸高直徑" /><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtBreastHeightDiameter" runat="server" TextMode="Number" CssClass="form-control" placeholder="35.2" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtBreastHeightCircumference" Text="胸高樹圍" /><i class="fa-solid fa-eye"></i>
                                            <asp:TextBox ID="txtBreastHeightCircumference" runat="server" TextMode="Number" CssClass="form-control" placeholder="110" />
                                        </div>
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtCanopyArea" Text="樹冠投影面積" />
                                            <asp:TextBox ID="txtCanopyArea" runat="server" TextMode="Number" CssClass="form-control" placeholder="45" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtHealth" Text="樹木健康及生育地概況" />
                                            <asp:TextBox ID="txtHealth" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="樹勢旺盛，葉片茂密" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtEpiphyteDescription" Text="附生植物概況" />
                                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                                <span>狀況說明：</span>
                                                <asp:TextBox ID="txtEpiphyteDescription" runat="server" CssClass="form-control" placeholder="少量山蘇附生" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtParasiteDescription" Text="寄生植物概況" />
                                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                                <span>狀況說明：</span>
                                                <asp:TextBox ID="txtParasiteDescription" runat="server" CssClass="form-control" placeholder="無明顯寄生" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtClimbingPlantDescription" Text="纏勒植物概況" />
                                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                                <span>狀況說明：</span>
                                                <asp:TextBox ID="txtClimbingPlantDescription" runat="server" CssClass="form-control" placeholder="近根處有少量藤蔓" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:Label runat="server" AssociatedControlID="txtMemo" Text="其他備註" />
                                            <asp:TextBox ID="txtMemo" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="維護時需封鎖人行道" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane fade" id="pane-photos" role="tabpanel" aria-labelledby="tab-photos">
                    <div class="row">
                        <div class="col">
                            <div class="formCard card">
                                <div class="card-header">樹木照片<i class="fa-solid fa-eye"></i> (最多 5 張，每張 10MB)</div>
                                <div class="card-body">
                                    <div id="photoDropArea" class="photo-drop" title="拖曳照片到此處或點擊選擇">
                                        將照片拖曳到此處或點擊選擇
                                        <asp:FileUpload ID="fuPendingPhotos" runat="server" AllowMultiple="true" CssClass="d-none" accept="image/*" />
                                    </div>
                                    <div id="photoList" class="photo-list"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <div class="formCard card">
                        <div class="card-header">操作說明</div>
                        <div class="card-body">
                            1. 樹籍狀態若為「已公告列管」，則「公告日期」為必填項目，並且此樹將顯示於前台可被民眾查看。<br />
                            2. 標示為<i class="fa-solid fa-eye"></i>之項目，填打內容將顯示於前台可被民眾查看。
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <div class="formCard card">
                        <div class="card-footer m-0 mt-3">
                            <div class="d-flex flex-wrap align-items-center gap-3">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkFinalConfirm" runat="server" />
                                    <asp:Label runat="server" AssociatedControlID="chkFinalConfirm" CssClass="form-check-label" Text="確認為定稿" /><br />
                                    定稿後無法回復為草稿，定稿後會開放全部使用者可檢視
                                </div>
                                <asp:Button ID="btnSave" runat="server" Text="儲存" OnClick="btnSave_Click" OnClientClick="return confirmSave();" CssClass="btn btn-primary" />
                                <asp:Button ID="btnCancel" runat="server" Text="取消" CausesValidation="false" OnClick="btnCancel_Click" OnClientClick="return confirmNavigate();" CssClass="btn btn-outline-secondary" />
                                <asp:HyperLink Visible="false" ID="lnkUploadPhotos" runat="server" Text="樹木照片" NavigateUrl="edit_photos.aspx" CssClass="btn btn-link" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
       <%-- </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSaveDraft" />
            <asp:PostBackTrigger ControlID="btnSaveFinal" />
        </Triggers>
    </asp:UpdatePanel>--%>

    <asp:Panel ID="pnlLogs" runat="server" Visible="false">
    <div class="row">
        <div class="col">
            <div class="formCard card">
                <div class="card-header">編輯紀錄</div>
                <div class="card-body">
                    <asp:Label ID="lblLogEmpty" runat="server" Text="尚無編輯紀錄" Visible="false" />
                    <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="false" CssClass="gv" AllowPaging="true" PageSize="5" OnPageIndexChanging="gvLogs_PageIndexChanging">
                        <PagerSettings Mode="Numeric" />
                        <Columns>
                            <asp:BoundField DataField="ActionType" HeaderText="動作" />
                            <asp:BoundField DataField="Memo" HeaderText="說明" />
                            <asp:BoundField DataField="LogDateTime" HeaderText="時間" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:TemplateField HeaderText="帳號">
                                <ItemTemplate>
                                    <%# Eval("AccountName") %> (<%# Eval("Account") %>) <%# Eval("AccountUnit") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

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
                        <input type="number" class="form-control" id="twd97X" placeholder="303000" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label" for="twd97Y">97座標 Y（北向）</label>
                        <input type="number" class="form-control" id="twd97Y" placeholder="2765000" />
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
        const maxPhotos = 5;
        const maxSize = 10 * 1024 * 1024;
        const initialPhotos = <%= PhotoJson %> || [];
        const photoListElement = document.getElementById('photoList');
        const dropArea = document.getElementById('photoDropArea');
        const fileInput = document.getElementById('<%= fuPendingPhotos.ClientID %>');
        const deletedField = document.getElementById('<%= hfDeletedPhotos.ClientID %>');
        const coverField = document.getElementById('<%= hfCoverPhoto.ClientID %>');
        const newKeysField = document.getElementById('<%= hfNewPhotoKeys.ClientID %>');
        let existingPhotos = initialPhotos
            .filter(p => !p.isTemp)
            .map(p => ({
                key: `existing-${p.photoId}`,
                photoId: p.photoId,
                filePath: p.filePath,
                caption: p.caption,
                isCover: p.isCover,
                deleted: false
            }));
        let newPhotos = initialPhotos
            .filter(p => p.isTemp)
            .map(p => ({
                key: p.key || `temp-${Date.now()}`,
                fileName: p.fileName || p.caption || '',
                filePath: p.filePath,
                previewUrl: p.filePath,
                caption: p.caption || p.fileName || '',
                isTemp: true
            }));

        function initializePhotos() {
            const allPhotos = getAllActivePhotos();
            const existingCover = coverField.value;
            if (existingCover && allPhotos.some(p => p.key === existingCover)) {
                renderPhotos();
                return;
            }

            const cover = allPhotos.find(p => p.isCover && !p.deleted) || allPhotos[0];
            coverField.value = cover ? cover.key : '';
            renderPhotos();
        }

        function getAllActivePhotos() {
            const activeExisting = existingPhotos.filter(p => !p.deleted);
            return [...activeExisting, ...newPhotos];
        }

        function renderPhotos() {
            const allPhotos = getAllActivePhotos();
            if (allPhotos.length === 0) {
                photoListElement.innerHTML = '<div class="text-muted">尚未選擇照片</div>';
                updateHiddenFields();
                return;
            }

            const coverValue = coverField.value;
            const items = allPhotos.map(p => {
                const isCover = coverValue === p.key;
                const src = p.filePath || p.previewUrl;
                const caption = p.caption || p.fileName || p.file?.name || '';
                return `
                    <div class="photo-item">
                        <img src="${src}" alt="${caption}" />
                        <div class="photo-actions">
                            <label>
                                <input type="radio" name="coverPhoto" value="${p.key}" ${isCover ? 'checked' : ''} onchange="setCoverPhoto('${p.key}')" />
                                設為封面
                            </label>
                            <button type="button" class="btn btn-link" onclick="removePhoto('${p.key}')">刪除</button>
                        </div>
                        <div class="photo-caption">${caption}</div>
                    </div>`;
            });

            photoListElement.innerHTML = `<div class="photo-grid">${items.join('')}</div>`;
            updateHiddenFields();
        }

        function setCoverPhoto(key) {
            coverField.value = key;
            markDirty();
        }

        function removePhoto(key) {
            const existing = existingPhotos.find(p => p.key === key);
            if (existing) {
                existing.deleted = true;
                deletedField.value = existingPhotos.filter(p => p.deleted).map(p => p.photoId).join(',');
            } else {
                newPhotos = newPhotos.filter(p => p.key !== key);
                rebuildFileInput();
            }

            if (coverField.value === key) {
                const next = existingPhotos.find(p => !p.deleted) || newPhotos[0];
                coverField.value = next ? next.key : '';
            }

            markDirty();
            renderPhotos();
        }

        function handleFiles(files) {
            if (!files || files.length === 0) return;

            const activeExisting = existingPhotos.filter(p => !p.deleted).length;
            if (activeExisting + newPhotos.length + files.length > maxPhotos) {
                alert('每棵樹最多保留 5 張照片');
                return;
            }

            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (file.size > maxSize) {
                    alert(`${file.name} 超過 10MB 限制，請重新選擇`);
                    continue;
                }

                const key = `new-${Date.now()}-${i}`;
                const previewUrl = URL.createObjectURL(file);
                newPhotos.push({ key, file, previewUrl, caption: file.name, fileName: file.name });
            }

            rebuildFileInput();

            if (!coverField.value) {
                const firstNew = newPhotos[0];
                if (firstNew) {
                    coverField.value = firstNew.key;
                }
            }

            markDirty();
            renderPhotos();
        }

        function rebuildFileInput() {
            const dataTransfer = new DataTransfer();
            newPhotos.forEach(p => {
                if (p.file) {
                    dataTransfer.items.add(p.file);
                }
            });
            fileInput.files = dataTransfer.files;
        }

        function updateHiddenFields() {
            deletedField.value = existingPhotos.filter(p => p.deleted).map(p => p.photoId).join(',');
            const allPhotos = getAllActivePhotos();
            if (!coverField.value || !allPhotos.some(p => p.key === coverField.value)) {
                const next = allPhotos[0];
                coverField.value = next ? next.key : '';
            }
            newKeysField.value = newPhotos.map(p => p.key).join(',');
        }

        dropArea.addEventListener('click', function () {
            fileInput.click();
        });

        dropArea.addEventListener('dragover', function (e) {
            e.preventDefault();
            dropArea.classList.add('dragging');
        });

        dropArea.addEventListener('dragleave', function () {
            dropArea.classList.remove('dragging');
        });

        dropArea.addEventListener('drop', function (e) {
            e.preventDefault();
            dropArea.classList.remove('dragging');
            handleFiles(e.dataTransfer.files);
        });

        fileInput.addEventListener('change', function () {
            handleFiles(fileInput.files);
            fileInput.value = '';
        });

        function toggleAnnouncementDate(selectedValue) {
            const container = document.getElementById('announcementDateContainer');
            if (!container) return;
            container.style.display = selectedValue === '1' ? '' : 'none';
        }

        function toggleRecognition(selectedValue) {
            const container = document.getElementById('recognitionContainer');
            if (!container) return;
            container.style.display = selectedValue === '1' ? '' : 'none';
        }

        document.addEventListener('DOMContentLoaded', function () {
            initializePhotos();
            setupDirtyTracking();
            setupNavigationGuards();
            updateRequiredIndicators();
            const statusDropdown = document.getElementById('<%= ddlStatus.ClientID %>');
            if (statusDropdown) {
                toggleAnnouncementDate(statusDropdown.value);
                toggleRecognition(statusDropdown.value);
                statusDropdown.addEventListener('change', function () {
                    toggleAnnouncementDate(statusDropdown.value);
                    toggleRecognition(statusDropdown.value);
                    updateRequiredIndicators();
                });
            }
            const finalCheckbox = document.getElementById('<%= chkFinalConfirm.ClientID %>');
            if (finalCheckbox) {
                finalCheckbox.addEventListener('change', function () {
                    updateRequiredIndicators();
                    markDirty();
                });
            }
            setupPostbackBypass();
        });
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
            markDirty();

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

        let isDirty = false;
        let allowUnload = false;

        function markDirty() {
            isDirty = true;
        }

        function requiresFinalValidation() {
            const finalCheckbox = document.getElementById('<%= chkFinalConfirm.ClientID %>');
            const finalHidden = document.getElementById('<%= hfIsFinal.ClientID %>');
            return (finalCheckbox && finalCheckbox.checked) || (finalHidden && finalHidden.value === '1');
        }

        function updateRequiredIndicators() {
            const requiresFinal = requiresFinalValidation();
            const statusDropdown = document.getElementById('<%= ddlStatus.ClientID %>');
            const statusValue = statusDropdown ? statusDropdown.value : '';
            document.querySelectorAll('.required-final').forEach(el => {
                const requirement = el.getAttribute('data-requirement');
                let visible = requiresFinal;
                if (requirement === 'announcement' || requirement === 'recognition') {
                    visible = requiresFinal && statusValue === '已公告列管';
                }
                el.style.display = visible ? 'inline' : 'none';
            });
        }

        function setupDirtyTracking() {
            const watched = document.querySelectorAll('input, textarea, select');
            watched.forEach(el => {
                el.addEventListener('change', markDirty);
                el.addEventListener('input', markDirty);
            });
        }

        function setupPostbackBypass() {
            const postbackIds = [
                '<%= ddlCity.ClientID %>',
                '<%= ddlArea.ClientID %>'
            ];
            postbackIds.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    element.addEventListener('change', function () {
                        allowUnload = true;
                    });
                }
            });
        }

        function setupNavigationGuards() {
            window.addEventListener('beforeunload', function (e) {
                if (isDirty && !allowUnload) {
                    e.preventDefault();
                    e.returnValue = '資料尚未儲存，是否確認離開？';
                }
            });

            document.addEventListener('click', function (e) {
                const anchor = e.target.closest('a');
                if (anchor && anchor.getAttribute('href') && anchor.getAttribute('href') !== '#') {
                    if (isDirty && !allowUnload) {
                        const canLeave = confirmNavigate();
                        if (!canLeave) {
                            e.preventDefault();
                        }
                    } else {
                        allowUnload = true;
                    }
                }
            });
        }

        function confirmNavigate() {
            if (!isDirty) {
                return true;
            }
            const leave = confirm('資料尚未儲存，是否確認離開？');
            if (leave) {
                allowUnload = true;
            }
            return leave;
        }

        function confirmSave() {
            const proceed = confirm('確認要儲存目前的資料嗎？');
            if (proceed) {
                allowUnload = true;
                isDirty = false;
            }
            return proceed;
        }
    </script>
    <style>
        .photo-drop {
            border: 2px dashed #6c757d;
            border-radius: 6px;
            padding: 16px;
            text-align: center;
            color: #6c757d;
            cursor: pointer;
            margin-bottom: 12px;
        }

        .photo-drop.dragging {
            background-color: #f8f9fa;
        }

        .photo-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
            gap: 12px;
        }

        .photo-item {
            border: 1px solid #e2e2e2;
            border-radius: 6px;
            padding: 8px;
            background: #fff;
        }

        .photo-item img {
            width: 100%;
            height: 140px;
            object-fit: cover;
            border-radius: 4px;
        }

        .photo-actions {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-top: 6px;
        }

        .photo-actions .btn-link {
            padding: 0;
            text-decoration: underline;
        }

        .photo-caption {
            margin-top: 4px;
            font-size: 0.9rem;
            color: #555;
            word-break: break-word;
        }

        .required-marker {
            display: none;
            margin-right: 4px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
