<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="protectTreesV2.backstage.care.edit" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style>
        .section-title {
            font-weight: 600;
            margin-bottom: 0.5rem;
        }

        .section-block {
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            padding: 16px;
            margin-bottom: 16px;
            background-color: #fff;
        }

        .section-subtitle {
            font-weight: 600;
            color: #374151;
            margin-bottom: 12px;
        }

        .option-group .form-check {
            margin-bottom: 0.35rem;
        }

        .photo-block {
            border: 1px dashed #cbd5f5;
            border-radius: 8px;
            padding: 16px;
            margin-bottom: 16px;
            background-color: #f8fafc;
        }

        .photo-block .form-label {
            font-weight: 600;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    養護資料管理 / 養護紀錄 / <asp:Literal ID="Literal_pathAction" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    <asp:Literal ID="Literal_title" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:HiddenField ID="HiddenField_treeId" runat="server" />
    <asp:HiddenField ID="HiddenField_careId" runat="server" />

    <div class="formCard card mb-4">
        <div class="card-header">樹籍資訊</div>
        <div class="card-body">
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label text-muted">系統樹籍編號</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_systemTreeNo" runat="server" Text="--"></asp:Label>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label text-muted">樹種</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_speciesName" runat="server" Text="--"></asp:Label>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label text-muted">座落地點 (縣市/鄉鎮)</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_cityName" runat="server" Text="--"></asp:Label>
                        <asp:Label ID="Label_areaName" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label text-muted">管理人</label>
                    <div class="fw-bold">
                        <asp:Label ID="Label_manager" runat="server" Text="--"></asp:Label>
                    </div>
                </div>

                <div class="col-12">
                    <hr class="my-2" />
                </div>

                <div class="col-12">
                    <div class="d-flex align-items-center">
                        <span class="me-2 text-muted">本筆資料編輯狀態：</span>
                        <h3>
                            <asp:Label ID="Label_recordStatus" runat="server" CssClass="badge bg-secondary" Text="新增"></asp:Label>
                        </h3>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="formCard card mb-4 shadow-sm">
        <div class="card-header">養護內容</div>
        <div class="card-body">
            <div class="section-block">
                <div class="section-subtitle">說明</div>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label">養護日期 <span class="text-danger">*</span></label>
                        <asp:TextBox ID="TextBox_careDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="請選擇養護日期" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">記錄人員</label>
                        <asp:TextBox ID="TextBox_recorder" runat="server" CssClass="form-control" MaxLength="100" placeholder="請輸入記錄人員" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">覆核人員</label>
                        <asp:TextBox ID="TextBox_reviewer" runat="server" CssClass="form-control" MaxLength="100" placeholder="請輸入覆核人員" />
                    </div>
                </div>
            </div>

            <div class="section-block">
                <div class="section-subtitle">生長情形概況</div>

                <div class="row mb-4">
                    <div class="col-md-2 fw-semibold">1. 樹冠枝葉</div>
                    <div class="col-md-10">
                        <asp:RadioButtonList ID="RadioButtonList_crownStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex gap-3">
                            <asp:ListItem Text="枝葉茂密無枯枝" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="有其他異狀" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                        <div id="crownOptions" class="option-group mt-2">
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_crownSeasonal" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_crownSeasonal.ClientID %>">季節性休眠落葉</label>
                            </div>
                            <div class="form-check d-flex align-items-center">
                                <asp:CheckBox ID="CheckBox_crownDeadBranch" runat="server" CssClass="form-check-input me-2" />
                                <label class="form-check-label me-2" for="<%= CheckBox_crownDeadBranch.ClientID %>">有枯枝(現存枝葉量：</label>
                                <asp:TextBox ID="TextBox_crownDeadBranchPercent" runat="server" CssClass="form-control form-control-sm w-auto" placeholder="0" />
                                <span class="ms-2">%)</span>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_crownPest" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_crownPest.ClientID %>">有明顯病蟲害(葉部有明顯蟲體或病徵)</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_crownForeign" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_crownForeign.ClientID %>">樹冠接觸電線或異物</label>
                            </div>
                            <div class="mt-2">
                                <label class="form-label">其他</label>
                                <asp:TextBox ID="TextBox_crownOtherNote" runat="server" CssClass="form-control" placeholder="請輸入其他狀況" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mb-4">
                    <div class="col-md-2 fw-semibold">2. 主莖幹</div>
                    <div class="col-md-10">
                        <asp:RadioButtonList ID="RadioButtonList_trunkStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex gap-3">
                            <asp:ListItem Text="完好健康無異狀" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="有其他異狀" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                        <div id="trunkOptions" class="option-group mt-2">
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_trunkBark" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_trunkBark.ClientID %>">樹皮破損</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_trunkDecay" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_trunkDecay.ClientID %>">莖幹損傷(腐朽中空或膨大)</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_trunkTermite" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_trunkTermite.ClientID %>">有白蟻蟻道</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_trunkLean" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_trunkLean.ClientID %>">主莖傾斜搖晃</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_trunkFungus" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_trunkFungus.ClientID %>">莖基部有真菌子實體(如靈芝)</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_trunkGummosis" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_trunkGummosis.ClientID %>">有流膠或潰瘍</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_trunkVine" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_trunkVine.ClientID %>">有纏勒植物(如雀榕或小花蔓澤蘭)</label>
                            </div>
                            <div class="mt-2">
                                <label class="form-label">其他</label>
                                <asp:TextBox ID="TextBox_trunkOtherNote" runat="server" CssClass="form-control" placeholder="請輸入其他狀況" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mb-4">
                    <div class="col-md-2 fw-semibold">3. 根部及地際部</div>
                    <div class="col-md-10">
                        <asp:RadioButtonList ID="RadioButtonList_rootStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex gap-3">
                            <asp:ListItem Text="根部完好無異狀" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="有其他異狀" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                        <div id="rootOptions" class="option-group mt-2">
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_rootDamage" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_rootDamage.ClientID %>">根部損傷</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_rootDecay" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_rootDecay.ClientID %>">根部有腐朽或可見子實體</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_rootExpose" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_rootExpose.ClientID %>">盤根或浮根</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_rootRot" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_rootRot.ClientID %>">根部潰爛</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_rootSucker" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_rootSucker.ClientID %>">大量萌櫱(不定芽)</label>
                            </div>
                            <div class="mt-2">
                                <label class="form-label">其他</label>
                                <asp:TextBox ID="TextBox_rootOtherNote" runat="server" CssClass="form-control" placeholder="請輸入其他狀況" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mb-4">
                    <div class="col-md-2 fw-semibold">4. 生育地環境</div>
                    <div class="col-md-10">
                        <asp:RadioButtonList ID="RadioButtonList_envStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex gap-3">
                            <asp:ListItem Text="良好無異狀" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="有其他異狀" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                        <div id="envOptions" class="option-group mt-2">
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_envPitSmall" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_envPitSmall.ClientID %>">樹穴過小</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_envPaved" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_envPaved.ClientID %>">遭鋪面封固(如柏油、混凝土、磚瓦等)</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_envDebris" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_envDebris.ClientID %>">有石塊或廢棄物推積</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_envSoilCover" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_envSoilCover.ClientID %>">根領覆土過高</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_envCompaction" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_envCompaction.ClientID %>">土壤壓實</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_envWater" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_envWater.ClientID %>">環境積水</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_envFacility" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_envFacility.ClientID %>">緊鄰設施或建物</label>
                            </div>
                            <div class="mt-2">
                                <label class="form-label">其他</label>
                                <asp:TextBox ID="TextBox_envOtherNote" runat="server" CssClass="form-control" placeholder="請輸入其他狀況" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-2 fw-semibold">5. 樹冠或主莖幹與鄰接物</div>
                    <div class="col-md-10">
                        <asp:RadioButtonList ID="RadioButtonList_adjacentStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex gap-3">
                            <asp:ListItem Text="無鄰接物" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="有其他異狀" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                        <div id="adjacentOptions" class="option-group mt-2">
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_adjacentBuilding" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_adjacentBuilding.ClientID %>">接觸建築物</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_adjacentWire" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_adjacentWire.ClientID %>">接觸電線或管線</label>
                            </div>
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_adjacentSignal" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= CheckBox_adjacentSignal.ClientID %>">遮蔽路燈或號誌</label>
                            </div>
                            <div class="mt-2">
                                <label class="form-label">其他</label>
                                <asp:TextBox ID="TextBox_adjacentOtherNote" runat="server" CssClass="form-control" placeholder="請輸入其他狀況" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="section-block">
                <div class="section-subtitle">養護管理作業</div>

                <div class="row mb-3">
                    <div class="col-md-2 fw-semibold">1. 危險枯枝清除</div>
                    <div class="col-md-10">
                        <div class="text-muted mb-2">受保護樹木若有枝條枯損或懸掛等危害公共安全疑慮之情形，宜立即回報並盡速清除</div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task1None" runat="server" GroupName="task1Status" Checked="true" />
                            <label class="form-check-label" for="<%= RadioButton_task1None.ClientID %>">無須處理</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task1Do" runat="server" GroupName="task1Status" />
                            <label class="form-check-label" for="<%= RadioButton_task1Do.ClientID %>">處理方式</label>
                        </div>
                        <asp:TextBox ID="TextBox_task1Note" runat="server" CssClass="form-control mt-2" placeholder="請填寫說明" />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-2 fw-semibold">2. 植栽基盤維護暨環境整理</div>
                    <div class="col-md-10">
                        <div class="text-muted mb-2">包含改良土壤結構通透氣及排水、清除過度覆土、拓展植穴空間等作業</div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task2None" runat="server" GroupName="task2Status" Checked="true" />
                            <label class="form-check-label" for="<%= RadioButton_task2None.ClientID %>">無須處理</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task2Do" runat="server" GroupName="task2Status" />
                            <label class="form-check-label" for="<%= RadioButton_task2Do.ClientID %>">處理方式</label>
                        </div>
                        <asp:TextBox ID="TextBox_task2Note" runat="server" CssClass="form-control mt-2" placeholder="請填寫說明" />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-2 fw-semibold">3. 樹木健康管理</div>
                    <div class="col-md-10">
                        <div class="text-muted mb-2">受保護樹木如有病害、蟲害等情況，須進一步診斷查明原因後再進行治療</div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task3None" runat="server" GroupName="task3Status" Checked="true" />
                            <label class="form-check-label" for="<%= RadioButton_task3None.ClientID %>">無須處理</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task3Do" runat="server" GroupName="task3Status" />
                            <label class="form-check-label" for="<%= RadioButton_task3Do.ClientID %>">處理方式</label>
                        </div>
                        <asp:TextBox ID="TextBox_task3Note" runat="server" CssClass="form-control mt-2" placeholder="請填寫說明" />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-2 fw-semibold">4. 營養評估追肥</div>
                    <div class="col-md-10">
                        <div class="text-muted mb-2">受保護樹木若經評估建議追給適量緩效性有機質肥料，宜配合調查進行</div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task4None" runat="server" GroupName="task4Status" Checked="true" />
                            <label class="form-check-label" for="<%= RadioButton_task4None.ClientID %>">無須處理</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task4Do" runat="server" GroupName="task4Status" />
                            <label class="form-check-label" for="<%= RadioButton_task4Do.ClientID %>">處理方式</label>
                        </div>
                        <asp:TextBox ID="TextBox_task4Note" runat="server" CssClass="form-control mt-2" placeholder="請填寫說明" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-2 fw-semibold">5. 安全衛生防護(非必填)</div>
                    <div class="col-md-10">
                        <div class="text-muted mb-2">當受保護樹木進行維護作業時，應設立相關安全衛生防護措施</div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task5None" runat="server" GroupName="task5Status" Checked="true" />
                            <label class="form-check-label" for="<%= RadioButton_task5None.ClientID %>">無須處理</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButton ID="RadioButton_task5Do" runat="server" GroupName="task5Status" />
                            <label class="form-check-label" for="<%= RadioButton_task5Do.ClientID %>">處理方式</label>
                        </div>
                        <asp:TextBox ID="TextBox_task5Note" runat="server" CssClass="form-control mt-2" placeholder="請填寫說明" />
                    </div>
                </div>
            </div>

            <div class="section-block">
                <div class="section-subtitle">養護照片上傳</div>
                <asp:Repeater ID="Repeater_carePhotos" runat="server" OnItemDataBound="Repeater_carePhotos_ItemDataBound">
                    <ItemTemplate>
                        <div class="photo-block">
                            <asp:HiddenField ID="HiddenField_photoId" runat="server" Value='<%# Eval("photoID") %>' />
                            <div class="row g-3">
                                <div class="col-md-4">
                                    <label class="form-label">施作項目名稱</label>
                                    <asp:TextBox ID="TextBox_itemName" runat="server" CssClass="form-control" placeholder="例如：枯枝清除" Text='<%# Eval("itemName") %>' />
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">施作前照片</label>
                                    <asp:FileUpload ID="FileUpload_beforePhoto" runat="server" CssClass="form-control" accept="image/png, image/jpeg, image/jpg" />
                                    <asp:HyperLink ID="HyperLink_beforePhoto" runat="server" CssClass="small text-decoration-none d-block mt-1" Target="_blank" />
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">施作後照片</label>
                                    <asp:FileUpload ID="FileUpload_afterPhoto" runat="server" CssClass="form-control" accept="image/png, image/jpeg, image/jpg" />
                                    <asp:HyperLink ID="HyperLink_afterPhoto" runat="server" CssClass="small text-decoration-none d-block mt-1" Target="_blank" />
                                </div>
                                <div class="col-12">
                                    <div class="form-check">
                                        <asp:CheckBox ID="CheckBox_deletePhoto" runat="server" CssClass="form-check-input" Text="刪除此筆照片" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div class="text-center">
                    <asp:Button ID="Button_addCarePhotoBlock" runat="server" Text="新增照片區塊" CssClass="btn btn-outline-primary" OnClick="Button_addCarePhotoBlock_Click" />
                </div>
            </div>

            <div class="card mb-4">
                <div class="card-body">
                    <div class="form-check">
                        <asp:CheckBox ID="CheckBox_isFinal" runat="server" />
                        <label class="form-check-label fw-bold" for="<%= CheckBox_isFinal.ClientID %>">
                            是否定稿
                        </label>
                    </div>
                </div>
            </div>

            <div class="text-center">
                <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary me-2" OnClick="LinkButton_save_Click">
                    <asp:Literal ID="Literal_btnSaveText" runat="server" Text="儲存"></asp:Literal>
                </asp:LinkButton>
                <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-secondary" OnClick="LinkButton_cancel_Click">
                    取消
                </asp:LinkButton>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlLogs" runat="server" Visible="false">
    <div class="row mt-4">
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

    <script>
        $(function () {
            function toggleOptions(listId, optionsId) {
                var $radios = $('#' + listId + ' input[type="radio"]');
                function update() {
                    var isOther = $radios.filter(':checked').val() === '2';
                    $('#' + optionsId + ' :input').prop('disabled', !isOther);
                }
                $radios.on('change', update);
                update();
            }

            toggleOptions('<%= RadioButtonList_crownStatus.ClientID %>', 'crownOptions');
            toggleOptions('<%= RadioButtonList_trunkStatus.ClientID %>', 'trunkOptions');
            toggleOptions('<%= RadioButtonList_rootStatus.ClientID %>', 'rootOptions');
            toggleOptions('<%= RadioButtonList_envStatus.ClientID %>', 'envOptions');
            toggleOptions('<%= RadioButtonList_adjacentStatus.ClientID %>', 'adjacentOptions');

            function toggleTask(noneRadioId, doRadioId, inputId) {
                var selector = '#' + noneRadioId + ', #' + doRadioId;
                function update() {
                    var isDo = $('#' + doRadioId).is(':checked');
                    $('#' + inputId).prop('disabled', !isDo);
                }
                $(selector).on('change', update);
                update();
            }

            toggleTask('<%= RadioButton_task1None.ClientID %>', '<%= RadioButton_task1Do.ClientID %>', '<%= TextBox_task1Note.ClientID %>');
            toggleTask('<%= RadioButton_task2None.ClientID %>', '<%= RadioButton_task2Do.ClientID %>', '<%= TextBox_task2Note.ClientID %>');
            toggleTask('<%= RadioButton_task3None.ClientID %>', '<%= RadioButton_task3Do.ClientID %>', '<%= TextBox_task3Note.ClientID %>');
            toggleTask('<%= RadioButton_task4None.ClientID %>', '<%= RadioButton_task4Do.ClientID %>', '<%= TextBox_task4Note.ClientID %>');
            toggleTask('<%= RadioButton_task5None.ClientID %>', '<%= RadioButton_task5Do.ClientID %>', '<%= TextBox_task5Note.ClientID %>');
        });
    </script>
</asp:Content>
