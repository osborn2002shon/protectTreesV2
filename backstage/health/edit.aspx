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
            transition: all 0.2s;
            background-color: transparent;
        }

        .photo-drop:hover, .photo-drop.dragging {
            background-color: #f8f9fa; 
            border-color: #6c757d;     
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


        .file-drop {
            border: 2px dashed #6c757d;
            border-radius: 6px;
            padding: 16px;
            text-align: center;
            color: #6c757d;
            cursor: pointer;
            margin-bottom: 12px;
            background-color: transparent;
            transition: all 0.2s;
        }
        .file-drop:hover, .file-drop.dragging {
            background-color: #f8f9fa;
            border-color: #6c757d;
        }

        .file-card {
            border: 1px solid #e2e2e2;
            border-radius: 6px;
            padding: 12px; 
            background: #fff;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .file-info {
            display: flex;
            align-items: center;
            gap: 12px;
        }
    
        .file-icon {
            font-size: 1.8rem;
            color: #198754; 
        }

        .file-name {
            font-weight: 600;
            color: #333;
            font-size: 0.95rem;
            margin-bottom: 2px;
        }

        .file-meta {
            font-size: 0.75rem;
            color: #6c757d;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    健檢資料管理：<asp:Literal ID="Literal_pathAction" runat="server"></asp:Literal>
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
        <div class="card-footer"></div>
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
                            <asp:DropDownList ID="DropDownList_treeSignStatus" runat="server" CssClass="form-select"></asp:DropDownList>
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
                        <%-- 米圍 --%>
                        <div class="col">
                            <label class="form-label">米圍 (1.0m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_girth100" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入米圍" />
                        </div>
                        <%-- 米徑 --%>
                        <div class="col">
                            <label class="form-label">米徑 (1.0m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_diameter100" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入米徑" />
                        </div>
                        <%-- 胸圍 --%>
                        <div class="col">
                            <label class="form-label">胸圍 (1.3m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_girth130" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入胸圍" />
                        </div>
                        <%-- 胸徑 --%>
                        <div class="col">
                            <label class="form-label">胸徑 (1.3m處) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_diameter130" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入胸徑" />
                        </div>
                    </div>

                     <div class="row">
                         <div class="col">
                             <div class="text-muted small mb-2">
                                 <i class="fa-solid fa-circle-info me-1"></i> 若有多筆數據，請以 <strong>"," (半形逗號)</strong> 進行區隔，例：4.2, 4.8。
                             </div>
                         </div>
                     </div>

                    <div class="row mb-3">
                        <div class="col">
                            <label class="form-label">備註（上移或下移實際量測高度）</label>
                            <asp:TextBox ID="TextBox_measureNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入備註" />
                        </div>
                    </div>
                </div>
                <div class="card-footer"></div>
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
                                    <asp:CheckBox ID="CheckBox_diseaseBrownRoot" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseBrownRoot.ClientID %>">樹木褐根病</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseGanoderma" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseGanoderma.ClientID %>">靈芝/菇類</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseWoodDecay" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseWoodDecay.ClientID %>">木材腐朽菌</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseCanker" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_diseaseCanker.ClientID %>">潰瘍</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_diseaseOther" runat="server" />
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
                <div class="card-footer"></div>
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
                                    <asp:CheckBox ID="CheckBox_rootMechanicalDamage" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_rootMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_rootMowingInjury" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_rootMowingInjury.ClientID %>">打草傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_rootInjury" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_rootInjury.ClientID %>">根傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_rootGirdling" runat="server" />
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
                                    <asp:CheckBox ID="CheckBox_baseMechanicalDamage" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_baseMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_baseMowingInjury" runat="server" />
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
                                    <asp:CheckBox ID="CheckBox_trunkMechanicalDamage" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_trunkMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_trunkIncludedBark" runat="server"/>
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
                                    <asp:CheckBox ID="CheckBox_branchMechanicalDamage" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_branchMechanicalDamage.ClientID %>">機具損傷</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_branchIncludedBark" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_branchIncludedBark.ClientID %>">內生夾皮</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_branchDrooping" runat="server"/>
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
                            <label class="form-label">樹葉生長覆蓋度百分比 <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="DropDownList_crownLeafCoveragePercent" runat="server" CssClass="form-select"></asp:DropDownList>
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
                                    <asp:CheckBox ID="CheckBox_crownHangingBranch" runat="server" />
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
                            <asp:TextBox ID="TextBox_growthNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入其他問題" />
                        </div>
                    </div>
                </div>
                <div class="card-footer"></div>
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
                            <asp:DropDownList ID="DropDownList_pruningWrongDamage" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="d-flex flex-wrap gap-4 align-items-center">
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningWoundHealing" runat="server"/>
                                    <label class="form-check-label" for="<%= CheckBox_pruningWoundHealing.ClientID %>">修剪傷口是否有癒合</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningEpiphyte" runat="server"/>
                                    <label class="form-check-label" for="<%= CheckBox_pruningEpiphyte.ClientID %>">附生植物</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningParasite" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_pruningParasite.ClientID %>">寄生植物</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_pruningVine" runat="server" />
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
                                    <asp:CheckBox ID="CheckBox_supportEmbedded" runat="server" />
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
                <div class="card-footer"></div>
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
                                    <asp:CheckBox ID="CheckBox_sitePlanter" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_sitePlanter.ClientID %>">花台</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteRecreationFacility" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_siteRecreationFacility.ClientID %>">休憩設施</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteDebrisStack" runat="server"/>
                                    <label class="form-check-label" for="<%= CheckBox_siteDebrisStack.ClientID %>">雜物堆置</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteBetweenBuildings" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_siteBetweenBuildings.ClientID %>">受限建物之間</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteSoilCompaction" runat="server" />
                                    <label class="form-check-label" for="<%= CheckBox_siteSoilCompaction.ClientID %>">土壤受踩踏夯實</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="CheckBox_siteOverburiedSoil" runat="server" />
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
                        <%-- 1. 土壤酸鹼度 --%>
                        <div class="col">
                            <label class="form-label">土壤酸鹼度(pH值) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_soilPh" runat="server" CssClass="form-control" MaxLength="50" placeholder="請輸入土壤酸鹼度" />
                        </div>

                        <%-- 2. 有機質含量 --%>
                        <div class="col">
                            <label class="form-label">有機質含量 <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_soilOrganicMatter" runat="server" CssClass="form-control" MaxLength="50" placeholder="請輸入有機質含量" />
                        </div>

                        <%-- 3. 電導度 --%>
                        <div class="col">
                            <label class="form-label">電導度(EC值) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_soilEc" runat="server" CssClass="form-control" MaxLength="50" placeholder="請輸入土壤電導度" />
                        </div>
                    </div>
                </div>
                <div class="card-footer"></div>
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
                            <asp:DropDownList ID="DropDownList_priority" runat="server" CssClass="form-select"></asp:DropDownList>
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
                <div class="card-footer"></div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-photos" role="tabpanel" aria-labelledby="tab-photos">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">照片</div>
                <div class="card-body">
                    <%-- 隱藏欄位 --%>
                    <asp:HiddenField ID="HiddenField_existingPhotosData" runat="server" />
                    <asp:HiddenField ID="HiddenField_deletedPhotoIds" runat="server" />
                    <asp:HiddenField ID="HiddenField_photoMetadata" runat="server" />

                    <div class="mb-3 text-muted small">
                        <i class="fa-solid fa-circle-info me-1"></i> 最多可上傳 <strong>5</strong> 張照片，支援 JPG/PNG，單張上限 10MB。
                    </div>

                    <%-- 拖曳上傳區 --%>
                    <div id="photoDropArea" class="photo-drop" title="拖曳照片到此處或點擊選擇">
                        <div class="mb-2"><i class="fa-solid fa-cloud-arrow-up fa-2x"></i></div>
                        <div>將照片拖曳到此處或點擊選擇</div>
                        <%-- FileUpload 隱藏，透過 JS 觸發 --%>
                        <asp:FileUpload ID="FileUpload_pendingPhotos" runat="server" AllowMultiple="true" CssClass="d-none" accept="image/png, image/jpeg, image/jpg" />
                    </div>

                    <%-- 照片列表顯示區 --%>
                    <div id="photoList" class="photo-list"></div>
                </div>
            </div>
        </div>

        <div class="tab-pane fade" id="pane-files" role="tabpanel" aria-labelledby="tab-files">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">相關附件</div>
                <div class="card-body">
                    <%-- 隱藏欄位 --%>
                    <%-- 1. 存現有檔案資訊 (JSON: { fileName: 'report.zip', filePath: '...' }) --%>
                    <asp:HiddenField ID="HiddenField_existingFileData" runat="server" />
                    <%-- 2. 標記是否刪除了舊檔案 (true/false) --%>
                    <asp:HiddenField ID="HiddenField_isFileDeleted" runat="server" Value="false" />

                    <div class="mb-3 text-muted small">
                        <i class="fa-solid fa-circle-info me-1"></i> 僅限上傳單一 <strong>.zip</strong> 壓縮檔，大小限制 <strong>30MB</strong>。
                    </div>

                    <%-- 狀態 A: 拖曳上傳區 (預設顯示) --%>
                    <div id="fileDropArea" class="file-drop" title="拖曳ZIP檔案到此處或點擊選擇">
                        <div class="mb-2"><i class="fa-solid fa-file-zipper fa-2x"></i></div>
                        <div>拖曳ZIP檔案到此處或點擊選擇</div>
                        <asp:FileUpload ID="FileUpload_attachment" runat="server" CssClass="d-none" accept=".zip,application/zip,application/x-zip-compressed" />
                    </div>

                    <%-- 狀態 B: 檔案顯示區 (預設隱藏) --%>
                    <div id="fileDisplayArea" class="d-none">
                        <div class="file-card">
                            <div class="file-info">
                                <div class="file-icon">
                                    <i class="fa-solid fa-file-zipper"></i>
                                </div>
                                <div>
                                    <div id="fileNameDisplay" class="file-name text-break"></div>
                                    <div id="fileStatusDisplay" class="file-meta">已上傳</div>
                                </div>
                            </div>
                            <button type="button" class="btn btn-outline-danger btn-sm" id="btnDeleteFile">
                                <i class="fa-solid fa-trash-can me-1"></i> 移除
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <div class="modal fade" id="imagePreviewModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg"> 
            <div class="modal-content bg-transparent border-0 shadow-none">
                <div class="modal-body p-0 text-center position-relative">
                    <%-- 關閉按鈕 --%>
                    <button type="button" class="btn-close btn-close-white position-absolute top-0 end-0 m-3" data-bs-dismiss="modal" aria-label="Close"></button>
                    <%-- 圖片容器 --%>
                    <img id="modalPreviewImage" src="" class="img-fluid rounded shadow" style="max-height: 85vh;" alt="Preview">
                </div>
            </div>
        </div>
    </div>

    <div class="text-center mb-5">
        <%-- 定稿勾選框 --%>
        <asp:CheckBox ID="CheckBox_dataStatus" runat="server"  />
        <label class="form-check-label" for="<%= CheckBox_dataStatus.ClientID %>">
            儲存為定稿
        </label>
        <br />
        定稿視為正式資料，無法退回草稿狀態，並將開放後台使用者查詢檢視
        <br />
        <%-- 儲存/新增按鈕 --%>
        <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn_main" OnClick="LinkButton_save_Click">
            <i class="fa-solid fa-floppy-disk me-1"></i><asp:Literal ID="Literal_btnSaveText" runat="server" Text="新增"></asp:Literal>
        </asp:LinkButton>
        <%-- 取消按鈕 --%>
        <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn_main_line" OnClick="LinkButton_cancel_Click">
            <i class="fa-solid fa-xmark me-1"></i>取消
        </asp:LinkButton>
    </div>

        <asp:Panel ID="Panel_logs" runat="server" Visible="false">
            <div class="row">
                <div class="col">
                    <div class="formCard card">
                        <div class="card-header">編輯紀錄</div>
                        <div class="card-body">
                            <asp:GridView ID="GridView_logs" runat="server" AutoGenerateColumns="false" CssClass="tb" AllowPaging="true" PageSize="5" OnPageIndexChanging="gvLogs_PageIndexChanging"  ShowHeaderWhenEmpty="true" >
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
                                 <EmptyDataTemplate>
                                     <div class="text-center py-3 text-muted">
                                         <p>尚無編輯紀錄。</p>
                                     </div>
                                 </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                        <div class="card-footer"></div>
                    </div>
                </div>
            </div>
        </asp:Panel>

   <script>
       $(document).ready(function () {
           // ==========================================================================
           // 樹木照片處理邏輯 (Photos)
           // ==========================================================================

           // 1. 取得物件
            const $photoDropArea = $('#photoDropArea');
            const $photoInput = $('#<%= FileUpload_pendingPhotos.ClientID %>');
            const $photoList = $('#photoList');
            const $existingPhotoData = $('#<%= HiddenField_existingPhotosData.ClientID %>');
            const $deletedPhotoField = $('#<%= HiddenField_deletedPhotoIds.ClientID %>');
            const $metadataField = $('#<%= HiddenField_photoMetadata.ClientID %>');

            // 設定
            const photoMaxSize = 10 * 1024 * 1024; // 10MB
            const maxPhotos = 5;
            let existingPhotos = [];
            let newPhotos = [];
            let tempIdCounter = -1; // 用來產生新照片的暫時 ID

            // 初始化照片
            initializePhotos();

            function initializePhotos() {
                const jsonVal = $existingPhotoData.val();
                if (jsonVal) {
                    try {
                        existingPhotos = JSON.parse(jsonVal);
                        $.each(existingPhotos, function (i, p) {
                            p.deleted = false;
                            if (!p.caption) p.caption = '';
                        });
                    } catch (e) { console.error('Photos JSON Error', e); }
                }
                renderPhotos();
            }

            function renderPhotos() {
                const activeExisting = $.grep(existingPhotos, function (p) { return !p.deleted; });
                const allPhotos = [ ...newPhotos,...activeExisting];

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
                        <img src="${src}" alt="Photo" 
                                class="rounded mb-2" 
                                style="width: 100%; height: 100px; object-fit: cover; cursor: pointer;" 
                                onclick="showImageModal('${src}')" />
    
                        <div class="photo-actions">
                                <button type="button" class="btn btn-link btn-sm text-danger text-decoration-none p-0 btn-delete-photo" data-key="${p.key}">
                                    <i class="fa-solid fa-trash-can"></i> 刪除
                                </button>
                        </div>
                        <div class="photo-filename text-truncate" title="${fileNameDisplay}">${fileNameDisplay}</div>
                        <input type="text" class="form-control form-control-sm photo-caption" 
                                data-key="${p.key}" value="${captionVal}" placeholder="請輸入備註說明">
                    </div>`;
                }).join('');

                $photoList.html(`<div class="photo-grid">${itemsHtml}</div>`);
                rebuildPhotoInput();
                updateMetadataField();
           }

           window.showImageModal = function (src) {
               $('#modalPreviewImage').attr('src', src);
               var myModal = new bootstrap.Modal(document.getElementById('imagePreviewModal'));
               myModal.show();
           }

            function handlePhotoFiles(files) {
                if (!files || files.length === 0) return;

                let currentCount = existingPhotos.filter(p => !p.deleted).length + newPhotos.length;

                // 檢查照片數量
                if (currentCount + files.length > maxPhotos) {
                    alert(`照片數量限制為 ${maxPhotos} 張。\n目前已有 ${currentCount} 張，您試圖加入 ${files.length} 張。`);
                    // 清空 input 讓使用者可以重選
                    $('#<%= FileUpload_pendingPhotos.ClientID %>').val('');
                    return;
                }

                for (let i = files.length - 1; i >= 0; i--) {
                    const file = files[i];
                    if (file.size > photoMaxSize) { alert(`${file.name} 太大(超過10MB)，已略過`); continue; }
                    if (newPhotos.some(np => np.file.name === file.name && np.file.size === file.size)) continue;

                    const key = tempIdCounter--;
                    const previewUrl = URL.createObjectURL(file);
                    newPhotos.unshift({ key: key, file: file, previewUrl: previewUrl, caption: '' });
                    currentCount++;
                }
                renderPhotos();
            }

            function rebuildPhotoInput() {
                const dt = new DataTransfer();
                $.each(newPhotos, function (i, p) { dt.items.add(p.file); });
                $photoInput[0].files = dt.files;
            }

            function updateMetadataField() {
                const activeExisting = $.grep(existingPhotos, function (p) { return !p.deleted; });
                const metadata = [];

                // 舊照片
                $.each(activeExisting, function (i, p) {
                    metadata.push({ key: p.key, caption: p.caption });
                });
                // 新照片
                $.each(newPhotos, function (i, p) {
                    metadata.push({ key: p.key, fileName: p.file.name, caption: p.caption });
                });

                $metadataField.val(JSON.stringify(metadata));
            }

            // --- 照片區事件監聽 ---
            $photoInput.on('click', function (e) { e.stopPropagation(); });
            $photoDropArea.on('click', function () { $photoInput.trigger('click'); });
            $photoInput.on('change', function () { handlePhotoFiles(this.files); });

            $photoDropArea.on('dragover', function (e) { e.preventDefault(); e.stopPropagation(); $(this).addClass('dragging'); });
            $photoDropArea.on('dragleave', function (e) { e.preventDefault(); e.stopPropagation(); $(this).removeClass('dragging'); });
            $photoDropArea.on('drop', function (e) {
                e.preventDefault(); e.stopPropagation(); $(this).removeClass('dragging');
                handlePhotoFiles(e.originalEvent.dataTransfer.files);
            });

            $photoList.on('click', '.btn-delete-photo', function (e) {
                e.stopPropagation();
                const key = $(this).data('key');
                if (!confirm('確定刪除這張照片嗎?')) return;

                const existingIndex = existingPhotos.findIndex(p => p.key == key);
                if (existingIndex !== -1) {
                    existingPhotos[existingIndex].deleted = true;
                    const deletedIds = $.map($.grep(existingPhotos, p => p.deleted), p => p.key);
                    $deletedPhotoField.val(deletedIds.join(','));
                } else {
                    newPhotos = newPhotos.filter(p => p.key != key);
                }
                renderPhotos();
            });

            $photoList.on('change input', '.photo-caption', function (e) {
                const key = $(this).data('key');
                const val = $(this).val();
                const existingIdx = existingPhotos.findIndex(p => p.key == key);
                if (existingIdx !== -1) {
                    existingPhotos[existingIdx].caption = val;
                } else {
                    const newIdx = newPhotos.findIndex(p => p.key == key);
                    if (newIdx !== -1) newPhotos[newIdx].caption = val;
                }
                updateMetadataField();
            });


            // ==========================================================================
            // 相關附件處理邏輯 (Files - ZIP Only)
            // ==========================================================================
        
            // 1. 取得物件
            const $fileDropArea = $('#fileDropArea');
            const $fileDisplayArea = $('#fileDisplayArea');
            const $attachInput = $('#<%= FileUpload_attachment.ClientID %>');
            const $existingFileField = $('#<%= HiddenField_existingFileData.ClientID %>');
            const $fileDeletedField = $('#<%= HiddenField_isFileDeleted.ClientID %>');

            const $fileNameDisplay = $('#fileNameDisplay');
            const $fileStatusDisplay = $('#fileStatusDisplay');
            const $btnDeleteFile = $('#btnDeleteFile');

            // 設定
            const attachMaxSize = 30 * 1024 * 1024; // 30MB
            let existingFile = null;
            let newAttachment = null;

            // 初始化附件
            initializeAttachment();

            function initializeAttachment() {
                const jsonVal = $existingFileField.val();
                if (jsonVal) {
                    try {
                        existingFile = JSON.parse(jsonVal);
                    } catch (e) { console.error('File JSON Error', e); }
                }
                updateFileUI();
            }

            function updateFileUI() {
                if (newAttachment) {
                    // --- 情境 A：使用者剛選取了新檔案 (還沒存檔) ---
                    $fileDropArea.addClass('d-none');
                    $fileDisplayArea.removeClass('d-none');

                    // 建立一個暫時的 Blob URL 讓使用者可以點擊下載自己剛選的檔案
                    const tempUrl = URL.createObjectURL(newAttachment);

                    // 生成連結 HTML
                    const linkHtml = `<a href="${tempUrl}" download="${newAttachment.name}" class="fw-bold text-decoration-none text-dark hover-primary" target="_blank">
                                    ${newAttachment.name}
                                  </a>`;

                    $('#fileNameDisplay').html(linkHtml); 
                    const sizeMB = (newAttachment.size / (1024 * 1024)).toFixed(2);
                    $('#fileStatusDisplay').text(`準備上傳 (${sizeMB} MB)`).addClass('text-success').removeClass('text-muted');

                } else if (existingFile && $fileDeletedField.val() !== 'true') {
                    // 資料庫裡的舊檔案 ---
                    $fileDropArea.addClass('d-none');
                    $fileDisplayArea.removeClass('d-none');
                    const downloadLink = existingFile.filePath;

                    const linkHtml = `<a href="${downloadLink}" download="${existingFile.fileName}" class="fw-bold text-decoration-none text-dark hover-primary" target="_blank">
                        ${existingFile.fileName}
                      </a>`;

                    $('#fileNameDisplay').html(linkHtml); // 塞入連結

                    $('#fileStatusDisplay').text('已存檔').addClass('text-muted').removeClass('text-success');

                } else {
                    // 無檔案 ---
                    $fileDropArea.removeClass('d-none');
                    $fileDisplayArea.addClass('d-none');
                    $attachInput.val(''); // 清空 Input
                }
            }

            function handleAttachment(file) {
                if (!file) return;
                // 檢查副檔名
                if (!file.name.toLowerCase().endsWith('.zip')) {
                    alert('僅支援 .zip 格式的壓縮檔');
                    return;
                }
                // 檢查大小
                if (file.size > attachMaxSize) {
                    alert(`檔案大小超過 30MB 限制`);
                    return;
                }

                newAttachment = file;
                // 若有上傳新檔，視為覆蓋舊檔 (或重置刪除狀態)
                $fileDeletedField.val('false');
                updateFileUI();
            }

            // --- 附件區事件監聽 ---

            // 阻止 Input 冒泡 (防止無窮迴圈)
            $attachInput.on('click', function (e) { e.stopPropagation(); });

            // 點擊區塊
            $fileDropArea.on('click', function () { $attachInput.trigger('click'); });

            // Input 改變
            $attachInput.on('change', function () {
                if (this.files && this.files.length > 0) {
                    handleAttachment(this.files[0]);
                }
            });

            // 拖曳事件
            $fileDropArea.on('dragover', function (e) { e.preventDefault(); e.stopPropagation(); $(this).addClass('dragging'); });
            $fileDropArea.on('dragleave', function (e) { e.preventDefault(); e.stopPropagation(); $(this).removeClass('dragging'); });
            $fileDropArea.on('drop', function (e) {
                e.preventDefault(); e.stopPropagation(); $(this).removeClass('dragging');

                // 取得 DataTransfer 物件 (這是為了最後要塞回 input 用的)
                const dt = e.originalEvent.dataTransfer;
                const files = dt.files;

                if (files.length > 0) {
                    const droppedFile = files[0];
                    handleAttachment(droppedFile);
                    if (newAttachment === droppedFile) {
                        // 驗證成功
                        $attachInput[0].files = files;
                    }
                }
            });

            // 刪除按鈕
            $btnDeleteFile.on('click', function () {
                if (!confirm('確定要移除此附件嗎？')) return;

                if (newAttachment) {
                    newAttachment = null;
                    $attachInput.val('');

                    if (existingFile) {
                        $fileDeletedField.val('true');
                    }
                } else if (existingFile) {
                    $fileDeletedField.val('true');
                }
                updateFileUI();
            });


           $('#<%= LinkButton_save.ClientID %>').on('click', function () {
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
