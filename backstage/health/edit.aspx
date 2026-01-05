<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="protectTreesV2.backstage.health.edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
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

        .photo-filename {
            font-size: 0.75rem;
            color: #6c757d;
            margin-bottom: 0.25rem;
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis; 
        }

        .photo-caption {
            margin-top: 4px;
            font-size: 0.9rem;
            color: #555;
            word-break: break-word;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    健檢資料管理 / 健檢紀錄 / <asp:Literal ID="Literal_pathAction" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    <asp:Literal ID="Literal_title" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="formCard card mb-4">
        <div class="card-header">樹籍資訊</div>
        <div class="card-body">
            <div class="row g-3">
                <%-- 第一列：樹號、樹種、地點 --%>
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

                <%-- 分隔線 --%>
                <div class="col-12">
                    <hr class="my-2" />
                </div>

                <%-- 第二列：資料狀態 --%>
                <div class="col-12">
                    <div class="d-flex align-items-center">
                        <span class="me-2 text-muted">本筆資料編輯狀態：</span>
                    
                        <%-- 這裡用 Label，後端可以根據狀態動態改變 CssClass (例如 badge bg-primary) --%>
                        <h3>
                            <asp:Label ID="Label_recordStatus" runat="server" CssClass="badge bg-secondary" Text="新增"></asp:Label>
                        </h3>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <ul class="nav nav-tabs mb-4" id="healthEditTabs" role="tablist">
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark active" id="tab-general" data-bs-toggle="tab" data-bs-target="#pane-general" type="button" role="tab" aria-controls="pane-general" aria-selected="true">一般調查</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-pest" data-bs-toggle="tab" data-bs-target="#pane-pest" type="button" role="tab" aria-controls="pane-pest" aria-selected="false">病蟲害調查</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-growth" data-bs-toggle="tab" data-bs-target="#pane-growth" type="button" role="tab" aria-controls="pane-growth" aria-selected="false">樹木生長外觀</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-pruning" data-bs-toggle="tab" data-bs-target="#pane-pruning" type="button" role="tab" aria-controls="pane-pruning" aria-selected="false">修剪與支撐</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-soil" data-bs-toggle="tab" data-bs-target="#pane-soil" type="button" role="tab" aria-controls="pane-soil" aria-selected="false">生育地與土壤</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-diagnosis" data-bs-toggle="tab" data-bs-target="#pane-diagnosis" type="button" role="tab" aria-controls="pane-diagnosis" aria-selected="false">檢查結果與評估</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-photos" data-bs-toggle="tab" data-bs-target="#pane-photos" type="button" role="tab" aria-controls="pane-photos" aria-selected="false">照片</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-files" data-bs-toggle="tab" data-bs-target="#pane-files" type="button" role="tab" aria-controls="pane-files" aria-selected="false">附件</button>
        </li>
    </ul>

    <div class="tab-content" id="healthEditTabContent">
    
        <div class="tab-pane fade show active" id="pane-general" role="tabpanel" aria-labelledby="tab-general">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">一般調查資料</div>
                <div class="card-body">
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">調查日期 <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_surveyDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="請選擇調查日期" />
                        </div>
                        <div class="col">
                            <label class="form-label">調查人姓名 <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_surveyor" runat="server" CssClass="form-control" MaxLength="100" placeholder="請輸入調查人姓名" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹牌 <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="DropDownList_treeSignStatus" runat="server" CssClass="form-select">
                                <asp:ListItem Text="--" Value="" />
                                <asp:ListItem Text="有" Value="1" />
                                <asp:ListItem Text="沒有" Value="2" />
                                <asp:ListItem Text="毀損" Value="3" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">座標(WGS84)：緯度(N) <span class="text-danger">*</span></label>
                            <%-- 緯度範圍 -90 到 90 --%>
                            <asp:TextBox ID="TextBox_latitude" runat="server" CssClass="form-control" TextMode="Number" step="0.000001" min="-90" max="90" placeholder="請輸入緯度" />
                        </div>
                        <div class="col">
                            <label class="form-label">座標(WGS84)：經度(E) <span class="text-danger">*</span></label>
                            <%-- 經度範圍 -180 到 180 --%>
                            <asp:TextBox ID="TextBox_longitude" runat="server" CssClass="form-control" TextMode="Number" step="0.000001" min="-180" max="180" placeholder="請輸入經度" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">樹高 (m) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_treeHeight" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="999999.99" placeholder="請輸入樹高" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹冠投影面積 (m²) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_canopyArea" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="999999.99" placeholder="請輸入樹冠投影面積" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">米圍 (1.0m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_girth100" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="999999.99" placeholder="請輸入米圍(1.0m處)" />
                        </div>
                        <div class="col">
                            <label class="form-label">米徑 (1.0m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_diameter100" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="999999.99" placeholder="請輸入米徑(1.0m處)" />
                        </div>
                        <div class="col">
                            <label class="form-label">胸圍 (1.3m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_girth130" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="999999.99" placeholder="請輸入胸圍(1.3m處)" />
                        </div>
                        <div class="col">
                            <label class="form-label">胸徑 (1.3m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_diameter130" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="999999.99" placeholder="請輸入胸徑(1.3m處)" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">備註（上移或下移實際量測高度）</label>
                            <asp:TextBox ID="TextBox_measureNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入備註" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-pest" role="tabpanel" aria-labelledby="tab-pest">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">病蟲害調查</div>
                <div class="card-body">
                    <h5 class="border-bottom pb-2 mb-3 text-primary">重大病害</h5>
                    <div class="row mb-2">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseBrownRoot" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseBrownRoot.ClientID %>">樹木褐根病</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseGanoderma" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseGanoderma.ClientID %>">靈芝/菇類</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseWoodDecay" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseWoodDecay.ClientID %>">木材腐朽菌</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseCanker" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseCanker.ClientID %>">潰瘍</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseOther" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseOther.ClientID %>">其他</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <asp:TextBox ID="TextBox_diseaseOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他重大病害備註" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary mt-4">重大蟲害 (白蟻危害)</h5>
                    <div class="table-responsive mb-3">
                        <table class="table table-bordered table-sm text-center align-middle">
                            <thead class="table-light">
                                <tr>
                                    <th style="width: 15%">部位</th>
                                    <th>白蟻蟻道</th>
                                    <th>白蟻蛀蝕</th>
                                    <th>白蟻活體</th>
                                </tr>
                            </thead>
                            <tbody>
                                <%-- 根系 --%>
                                <tr>
                                    <td class="fw-bold bg-light">根系</td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestRootTunnel" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestRootChew" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestRootLive" runat="server" /></td>
                                </tr>
                                <%-- 樹基部 --%>
                                <tr>
                                    <td class="fw-bold bg-light">樹基部</td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestBaseTunnel" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestBaseChew" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestBaseLive" runat="server" /></td>
                                </tr>
                                <%-- 主幹 --%>
                                <tr>
                                    <td class="fw-bold bg-light">主幹</td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestTrunkTunnel" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestTrunkChew" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestTrunkLive" runat="server" /></td>
                                </tr>
                                <%-- 枝幹 --%>
                                <tr>
                                    <td class="fw-bold bg-light">枝幹</td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestBranchTunnel" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestBranchChew" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestBranchLive" runat="server" /></td>
                                </tr>
                                <%-- 樹冠 --%>
                                <tr>
                                    <td class="fw-bold bg-light">樹冠</td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestCrownTunnel" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestCrownChew" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestCrownLive" runat="server" /></td>
                                </tr>
                                <%-- 其他 --%>
                                <tr>
                                    <td class="fw-bold bg-light">其他</td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestOtherTunnel" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestOtherChew" runat="server" /></td>
                                    <td><asp:CheckBox ID="CheckBox_majorPestOtherLive" runat="server" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary mt-4">一般蟲害情形</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">根系</label>
                            <asp:TextBox ID="TextBox_generalPestRoot" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入根系一般蟲害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹基部</label>
                            <asp:TextBox ID="TextBox_generalPestBase" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入樹基部一般蟲害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">主幹</label>
                            <asp:TextBox ID="TextBox_generalPestTrunk" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入主幹一般蟲害情形" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">枝幹</label>
                            <asp:TextBox ID="TextBox_generalPestBranch" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入枝幹一般蟲害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹冠</label>
                            <asp:TextBox ID="TextBox_generalPestCrown" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入樹冠一般蟲害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">其他</label>
                            <asp:TextBox ID="TextBox_generalPestOther" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他一般蟲害情形" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary mt-4">一般病害情形</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">根系</label>
                            <asp:TextBox ID="TextBox_generalDiseaseRoot" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入根系一般病害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹基部</label>
                            <asp:TextBox ID="TextBox_generalDiseaseBase" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入樹基部一般病害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">主幹</label>
                            <asp:TextBox ID="TextBox_generalDiseaseTrunk" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入主幹一般病害情形" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">枝幹</label>
                            <asp:TextBox ID="TextBox_generalDiseaseBranch" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入枝幹一般病害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹冠</label>
                            <asp:TextBox ID="TextBox_generalDiseaseCrown" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入樹冠一般病害情形" />
                        </div>
                        <div class="col">
                            <label class="form-label">其他</label>
                            <asp:TextBox ID="TextBox_generalDiseaseOther" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他一般病害情形" />
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col">
                            <label class="form-label">其他病蟲害問題詳加備註</label>
                            <asp:TextBox ID="TextBox_pestOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他病蟲害備註" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-growth" role="tabpanel" aria-labelledby="tab-growth">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">樹木生長外觀情況</div>
                <div class="card-body">
                    <h5 class="border-bottom pb-2 mb-3 text-primary">根系</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">腐朽百分比(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_rootDecayPercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入根系腐朽比例" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹洞最大直徑(m) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_rootCavityMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入根系樹洞最大直徑" />
                        </div>
                        <div class="col">
                            <label class="form-label">傷口最大直徑(m)</label>
                            <asp:TextBox ID="TextBox_rootWoundMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入根系傷口最大直徑" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_rootMechanicalDamage" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_rootMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_rootMowingInjury" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_rootMowingInjury.ClientID %>">打草傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_rootInjury" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_rootInjury.ClientID %>">根傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_rootGirdling" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_rootGirdling.ClientID %>">盤根</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他</label>
                            <asp:TextBox ID="TextBox_rootOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入根系其他狀況" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary">樹基部</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">腐朽百分比(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_baseDecayPercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入樹基部腐朽比例" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹洞最大直徑(m) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_baseCavityMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入樹基部樹洞最大直徑" />
                        </div>
                        <div class="col">
                            <label class="form-label">傷口最大直徑(m)</label>
                            <asp:TextBox ID="TextBox_baseWoundMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入樹基部傷口最大直徑" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_baseMechanicalDamage" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_baseMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_baseMowingInjury" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_baseMowingInjury.ClientID %>">打草傷</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他</label>
                            <asp:TextBox ID="TextBox_baseOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入樹基部其他狀況" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary">主幹</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">腐朽百分比(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_trunkDecayPercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入主幹腐朽比例" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹洞最大直徑(m) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_trunkCavityMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入主幹樹洞最大直徑" />
                        </div>
                        <div class="col">
                            <label class="form-label">傷口最大直徑(m)</label>
                            <asp:TextBox ID="TextBox_trunkWoundMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入主幹傷口最大直徑" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_trunkMechanicalDamage" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_trunkMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_trunkIncludedBark" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_trunkIncludedBark.ClientID %>">內生夾皮</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他</label>
                            <asp:TextBox ID="TextBox_trunkOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入主幹其他狀況" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary">枝幹</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">腐朽百分比(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_branchDecayPercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入枝幹腐朽比例" />
                        </div>
                        <div class="col">
                            <label class="form-label">樹洞最大直徑(m) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_branchCavityMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入枝幹樹洞最大直徑" />
                        </div>
                        <div class="col">
                            <label class="form-label">傷口最大直徑(m)</label>
                            <asp:TextBox ID="TextBox_branchWoundMaxDiameter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入枝幹傷口最大直徑" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_branchMechanicalDamage" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_branchMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_branchIncludedBark" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_branchIncludedBark.ClientID %>">內生夾皮</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_branchDrooping" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_branchDrooping.ClientID %>">下垂枝</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他</label>
                            <asp:TextBox ID="TextBox_branchOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入枝幹其他狀況" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary">樹冠</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">樹葉生長覆蓋度百分比(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_crownLeafCoveragePercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入樹冠葉量比例" />
                        </div>
                        <div class="col">
                            <label class="form-label">一般枯枝(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_crownDeadBranchPercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入樹冠枯枝比例" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_crownHangingBranch" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_crownHangingBranch.ClientID %>">懸掛枝</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他</label>
                            <asp:TextBox ID="TextBox_crownOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入樹冠其他狀況" />
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col">
                            <label class="form-label">其他問題詳加備註</label>
                            <asp:TextBox ID="TextBox_growthNote" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="請輸入其他問題" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-pruning" role="tabpanel" aria-labelledby="tab-pruning">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">樹木修剪與支撐情況</div>
                <div class="card-body">
                    <h5 class="border-bottom pb-2 mb-3 text-primary">樹木修剪</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">錯誤修剪傷害 <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="DropDownList_pruningWrongDamage" runat="server" CssClass="form-select">
                                <asp:ListItem Text="--" Value="" />
                                <asp:ListItem Text="截幹" Value="截幹" />
                                <asp:ListItem Text="截頂" Value="截頂" />
                                <asp:ListItem Text="不當縮剪" Value="不當縮剪" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningWoundHealing" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_pruningWoundHealing.ClientID %>">修剪傷口是否有癒合</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningEpiphyte" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_pruningEpiphyte.ClientID %>">附生植物</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningParasite" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_pruningParasite.ClientID %>">寄生植物</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningVine" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_pruningVine.ClientID %>">纏勒植物</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他修剪問題詳加備註</label>
                            <asp:TextBox ID="TextBox_pruningOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他修剪狀況" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary">樹木支撐</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <%-- supportCount int -> step="1" --%>
                            <label class="form-label">有設立支架(支) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_supportCount" runat="server" CssClass="form-control" TextMode="Number" step="1" min="0" max="999" placeholder="請輸入支架數量" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_supportEmbedded" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_supportEmbedded.ClientID %>">支架已嵌入樹皮</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他支撐問題詳加備註</label>
                            <asp:TextBox ID="TextBox_supportOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他支撐狀況" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-soil" role="tabpanel" aria-labelledby="tab-soil">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">生育地環境與土壤檢測</div>
                <div class="card-body">
                    <h5 class="border-bottom pb-2 mb-3 text-primary">生育地環境</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">水泥鋪面(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_siteCementPercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入水泥鋪面比例" />
                        </div>
                        <div class="col">
                            <label class="form-label">柏油鋪面(%) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_siteAsphaltPercent" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="100" placeholder="請輸入柏油鋪面比例" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_sitePlanter" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_sitePlanter.ClientID %>">花台</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteRecreationFacility" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_siteRecreationFacility.ClientID %>">休憩設施</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteDebrisStack" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_siteDebrisStack.ClientID %>">雜物堆置</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteBetweenBuildings" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_siteBetweenBuildings.ClientID %>">受限建物之間</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteSoilCompaction" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_siteSoilCompaction.ClientID %>">土壤受踩踏夯實</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteOverburiedSoil" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="<%= CheckBox_siteOverburiedSoil.ClientID %>">覆土過深</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col">
                            <label class="form-label">其他生育地問題備註</label>
                            <asp:TextBox ID="TextBox_siteOtherNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他生育地問題備註" />
                        </div>
                    </div>

                    <h5 class="border-bottom pb-2 mb-3 text-primary">土壤檢測</h5>
                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">土壤酸鹼度(pH值) <span class="text-danger">*</span></label>
                            <%-- pH: 0~14 --%>
                            <asp:TextBox ID="TextBox_soilPh" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="14" placeholder="請輸入土壤酸鹼度" />
                        </div>
                        <div class="col">
                            <label class="form-label">有機質含量 <span class="text-danger">*</span></label>
                            <%-- decimal(6,2) -> max 9999.99 --%>
                            <asp:TextBox ID="TextBox_soilOrganicMatter" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入有機質含量" />
                        </div>
                        <div class="col">
                            <label class="form-label">電導度(EC值) <span class="text-danger">*</span></label>
                            <%-- decimal(6,2) -> max 9999.99 --%>
                            <asp:TextBox ID="TextBox_soilEc" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" max="9999.99" placeholder="請輸入土壤電導度" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-diagnosis" role="tabpanel" aria-labelledby="tab-diagnosis">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">健康檢查結果及風險評估</div>
                <div class="card-body">
                    <h5 class="border-bottom pb-2 mb-3 text-primary">健康檢查結果及風險評估</h5>

                    <div class="row mb-3">
                        <%-- 管理情況 --%>
                        <div class="col">
                            <label class="form-label">管理情況 <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_managementStatus" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="請輸入管理情況" />
                        </div>

                        <%-- 建議處理優先順序 --%>
                        <div class="col">
                            <label class="form-label">建議處理優先順序 <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="DropDownList_priority" runat="server" CssClass="form-select">
                                <asp:ListItem Text="--" Value="" />
                                <asp:ListItem Text="緊急處理" Value="緊急處理" />
                                <asp:ListItem Text="優先處理" Value="優先處理" />
                                <asp:ListItem Text="例行養護" Value="例行養護" />
                            </asp:DropDownList>
                        </div>

                        <%-- 處理情形說明 --%>
                        <div class="col">
                            <label class="form-label">處理情形說明 <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_treatmentDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="請輸入處理情形說明" />
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col">
                            <div class="alert alert-secondary d-flex align-items-center mb-0" role="alert">
                                <i class="fa-solid fa-circle-info me-2"></i>
                                <div>
                                    本健康檢查結果之建議及評估為調查當下紀錄之情形。
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-photos" role="tabpanel" aria-labelledby="tab-photos">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">照片</div>
                <div class="card-body">
                    <%-- 隱藏欄位：前後端溝通橋樑 --%>
                    <asp:HiddenField ID="HiddenField_existingPhotosData" runat="server" />
                    <asp:HiddenField ID="HiddenField_deletedPhotoIds" runat="server" />
                    <asp:HiddenField ID="HiddenField_photoMetadata" runat="server" />

                    <%-- 1. 說明文字 --%>
                    <div class="mb-3 text-muted small">
                        <i class="fa-solid fa-circle-info me-1"></i> 可上傳多張照片，支援 JPG/PNG，單張上限 10MB。
                    </div>

                    <%-- 2. 拖曳上傳區 --%>
                    <div id="photoDropArea" class="photo-drop" title="拖曳照片到此處或點擊選擇">
                        <div class="mb-2"><i class="fa-solid fa-cloud-arrow-up fa-2x"></i></div>
                        <div>將照片拖曳到此處或點擊選擇</div>
                        <%-- FileUpload 隱藏，透過 JS 觸發 --%>
                        <asp:FileUpload ID="FileUpload_pendingPhotos" runat="server" AllowMultiple="true" CssClass="d-none" accept="image/png, image/jpeg, image/jpg" />
                    </div>

                    <%-- 3. 照片列表顯示區 --%>
                    <div id="photoList" class="photo-list"></div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-files" role="tabpanel" aria-labelledby="tab-files">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">相關附件</div>
                <div class="card-body">
                    附件區塊
                </div>
            </div>
        </div>

    </div>

    <div class="card mb-4">
        <div class="card-body">
            <div class="form-check">
                <%-- 定稿勾選框 --%>
                <asp:CheckBox ID="CheckBox_dataStatus" runat="server" CssClass="form-check-input" />
                <label class="form-check-label fw-bold" for="<%= CheckBox_dataStatus.ClientID %>">
                    是否定稿
                </label>
                
            </div>
        </div>
    </div>

    <div class="text-center mb-5">
        <%-- 儲存/新增按鈕 --%>
        <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_save_Click">
            <asp:Literal ID="Literal_btnSaveText" runat="server" Text="新增"></asp:Literal>
        </asp:LinkButton>

        <%-- 取消按鈕 --%>
        <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-secondary" OnClick="LinkButton_cancel_Click">
            取消
        </asp:LinkButton>
    </div>

    <script>
        $(document).ready(function () {
            // 物件取得
            const $dropArea = $('#photoDropArea');
            const $fileInput = $('#<%= FileUpload_pendingPhotos.ClientID %>');
            const $photoList = $('#photoList');
            const $existingDataField = $('#<%= HiddenField_existingPhotosData.ClientID %>');
            const $deletedField = $('#<%= HiddenField_deletedPhotoIds.ClientID %>');
            const $metadataField = $('#<%= HiddenField_photoMetadata.ClientID %>');

            const maxSize = 10 * 1024 * 1024;
            let existingPhotos = [];
            let newPhotos = [];
            // 用來產生新照片的暫時 ID (負數遞減)
            let tempIdCounter = -1;

            initializePhotos();

            function initializePhotos() {
                const jsonVal = $existingDataField.val();
                if (jsonVal) {
                    try {
                        existingPhotos = JSON.parse(jsonVal);
                        $.each(existingPhotos, function (i, p) {
                            p.deleted = false;
                            if (!p.caption) p.caption = '';
                        });
                    } catch (e) { console.error(e); }
                }
                renderPhotos();
            }

            function renderPhotos() {
                const activeExisting = $.grep(existingPhotos, function (p) { return !p.deleted; });
                const allPhotos = [...activeExisting, ...newPhotos];

                if (allPhotos.length === 0) {
                    $photoList.html('<div class="text-muted text-center py-3">目前沒有照片</div>');
                    updateMetadataField();
                    return;
                }

                const itemsHtml = $.map(allPhotos, function (p) {
                    const src = p.filePath || p.previewUrl;
                    const fileNameDisplay = p.file ? p.file.name : (p.fileName || '已存照片');
                    const captionVal = p.caption || '';

                    return `
                        <div class="photo-item">
                            <img src="${src}" alt="Photo" />
                            <div class="photo-actions">
                                <button type="button" class="btn btn-link btn-sm text-danger text-decoration-none p-0 btn-delete-photo" data-key="${p.key}">
                                    <i class="fa-solid fa-trash-can"></i> 刪除
                                </button>
                            </div>
                            <div class="photo-filename" title="${fileNameDisplay}">${fileNameDisplay}</div>
                            <input type="text" class="form-control form-control-sm photo-caption-input" 
                                   data-key="${p.key}" value="${captionVal}" placeholder="請輸入備註說明">
                        </div>`;
                }).join('');

                $photoList.html(`<div class="photo-grid">${itemsHtml}</div>`);
                rebuildFileInput();
                updateMetadataField();
            }

            function handleFiles(files) {
                if (!files || files.length === 0) return;

                for (let i = 0; i < files.length; i++) {
                    const file = files[i];
                    if (file.size > maxSize) { alert(`${file.name} 太大`); continue; }
                    if (newPhotos.some(np => np.file.name === file.name && np.file.size === file.size)) continue;

                    // ★ 修改點：使用負數當 Key (-1, -2, -3...)
                    // 雖然資料庫只要知道是負數就好，但在前端為了區分每一張，我們還是給它不同的負數
                    const key = tempIdCounter--;

                    const previewUrl = URL.createObjectURL(file);
                    newPhotos.push({ key: key, file: file, previewUrl: previewUrl, caption: '' });
                }
                renderPhotos();
            }

            function rebuildFileInput() {
                const dt = new DataTransfer();
                $.each(newPhotos, function (i, p) { dt.items.add(p.file); });
                $fileInput[0].files = dt.files;
            }

            // ★ 修改點：打包 JSON (移除了 isNew)
            function updateMetadataField() {
                const activeExisting = $.grep(existingPhotos, function (p) { return !p.deleted; });
                const metadata = [];

                // 1. 舊照片 (Key 是正數)
                $.each(activeExisting, function (i, p) {
                    metadata.push({
                        key: p.key,
                        caption: p.caption
                    });
                });

                // 2. 新照片 (Key 是負數，一定要傳 fileName 讓後端配對)
                $.each(newPhotos, function (i, p) {
                    metadata.push({
                        key: p.key, // 傳 -1, -2 給後端沒關係，後端只要判斷 < 0 即可
                        fileName: p.file.name,
                        caption: p.caption
                    });
                });

                $metadataField.val(JSON.stringify(metadata));
            }

            // 事件監聽
            $fileInput.on('click', function (e) { e.stopPropagation(); });
            $dropArea.on('click', function () { $fileInput.trigger('click'); });
            $fileInput.on('change', function () { handleFiles(this.files); });

            $dropArea.on('dragover', function (e) { e.preventDefault(); e.stopPropagation(); $(this).addClass('dragging'); });
            $dropArea.on('dragleave', function (e) { e.preventDefault(); e.stopPropagation(); $(this).removeClass('dragging'); });
            $dropArea.on('drop', function (e) {
                e.preventDefault(); e.stopPropagation(); $(this).removeClass('dragging');
                handleFiles(e.originalEvent.dataTransfer.files);
            });

            $photoList.on('click', '.btn-delete-photo', function (e) {
                e.stopPropagation();
                const key = $(this).data('key');
                if (!confirm('確定刪除?')) return;

                // 這裡要注意：key 可能是字串或數字，使用 == 讓它自動轉型比較保險
                const existingIndex = existingPhotos.findIndex(p => p.key == key);
                if (existingIndex !== -1) {
                    existingPhotos[existingIndex].deleted = true;
                    const deletedIds = $.map($.grep(existingPhotos, p => p.deleted), p => p.key);
                    $deletedField.val(deletedIds.join(','));
                } else {
                    newPhotos = newPhotos.filter(p => p.key != key);
                }
                renderPhotos();
            });

            $photoList.on('change input', '.photo-caption-input', function (e) {
                const key = $(this).data('key');
                const val = $(this).val();

                const existingIdx = existingPhotos.findIndex(p => p.key == key);
                if (existingIdx !== -1) {
                    existingPhotos[existingIdx].caption = val;
                } else {
                    const newIdx = newPhotos.findIndex(p => p.key == key);
                    if (newIdx !== -1) {
                        newPhotos[newIdx].caption = val;
                    }
                }
                updateMetadataField();
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
