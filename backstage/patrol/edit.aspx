<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="protectTreesV2.backstage.patrol.edit" MaintainScrollPositionOnPostback="true" %>
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
    巡查資料管理 / 巡查紀錄 / <asp:Literal ID="Literal_pathAction" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    <asp:Literal ID="Literal_title" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <nav class="nav nav-tabs mb-4">
        <a class="nav-link text-dark" href="main.aspx">巡查管理</a>
        <a class="nav-link text-dark" href="list.aspx">異動管理</a>
        <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆巡查照片</a>
    </nav>

    <asp:HiddenField ID="HiddenField_treeId" runat="server" />
    <asp:HiddenField ID="HiddenField_patrolId" runat="server" />

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

    <ul class="nav nav-tabs mb-4" id="patrolEditTabs" role="tablist">
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark active" id="tab-general" data-bs-toggle="tab" data-bs-target="#pane-general" type="button" role="tab" aria-controls="pane-general" aria-selected="true">一般調查</button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link text-dark" id="tab-photos" data-bs-toggle="tab" data-bs-target="#pane-photos" type="button" role="tab" aria-controls="pane-photos" aria-selected="false">照片</button>
        </li>
    </ul>

    <div class="tab-content" id="patrolEditTabContent">
        <div class="tab-pane fade show active" id="pane-general" role="tabpanel" aria-labelledby="tab-general">
            <div class="formCard card mb-4 shadow-sm">
                <div class="card-header">一般調查資料</div>
                <div class="card-body">
                    <div class="row mb-3">
                        <div class="col-md-4">
                            <label class="form-label">巡查日期 <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_patrolDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="請選擇巡查日期" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">巡查人姓名 <span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_patroller" runat="server" CssClass="form-control" MaxLength="100" placeholder="請輸入巡查人姓名" />
                        </div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">巡查備註</label>
                        <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="請輸入巡查備註" />
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_isFinal" runat="server" />
                                <label class="form-check-label fw-bold" for="<%= CheckBox_isFinal.ClientID %>">
                                    是否定稿
                                </label>
                            </div>
                        </div>
                        <div class="col-md-8">
                            <div class="form-check">
                                <asp:CheckBox ID="CheckBox_hasPublicSafetyRisk" runat="server" />
                                <label class="form-check-label fw-bold" for="<%= CheckBox_hasPublicSafetyRisk.ClientID %>">
                                    是否有危害公共安全風險或緊急狀況
                                </label>
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
                    <asp:HiddenField ID="HiddenField_existingPhotosData" runat="server" />
                    <asp:HiddenField ID="HiddenField_deletedPhotoIds" runat="server" />
                    <asp:HiddenField ID="HiddenField_photoMetadata" runat="server" />

                    <div class="mb-3 text-muted small">
                        <i class="fa-solid fa-circle-info me-1"></i> 最多可上傳 <strong>5</strong> 張照片，支援 JPG/PNG，單張上限 10MB。
                    </div>

                    <div id="photoDropArea" class="photo-drop" title="拖曳照片到此處或點擊選擇">
                        <div class="mb-2"><i class="fa-solid fa-cloud-arrow-up fa-2x"></i></div>
                        <div>將照片拖曳到此處或點擊選擇</div>
                        <asp:FileUpload ID="FileUpload_pendingPhotos" runat="server" AllowMultiple="true" CssClass="d-none" accept="image/png, image/jpeg, image/jpg" />
                    </div>

                    <div id="photoList" class="photo-list"></div>
                </div>
            </div>
        </div>
    </div>

    <div class="text-center mb-5">
        <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary me-2" OnClick="LinkButton_save_Click">
            <asp:Literal ID="Literal_btnSaveText" runat="server" Text="新增"></asp:Literal>
        </asp:LinkButton>

        <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-secondary" OnClick="LinkButton_cancel_Click">
            取消
        </asp:LinkButton>
    </div>

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

    <script>
        function confirmPatrolRisk() {
            var isFinal = document.getElementById('<%= CheckBox_isFinal.ClientID %>').checked;
            var hasRisk = document.getElementById('<%= CheckBox_hasPublicSafetyRisk.ClientID %>').checked;
            if (isFinal && hasRisk) {
                return confirm('此次巡查將會寄信給相關主管機關，請確認是否送出？');
            }
            return true;
        }

        $(document).ready(function () {
            const $dropArea = $('#photoDropArea');
            const $fileInput = $('#<%= FileUpload_pendingPhotos.ClientID %>');
            const $photoList = $('#photoList');
            const $existingDataField = $('#<%= HiddenField_existingPhotosData.ClientID %>');
            const $deletedField = $('#<%= HiddenField_deletedPhotoIds.ClientID %>');
            const $metadataField = $('#<%= HiddenField_photoMetadata.ClientID %>');

            const maxSize = 10 * 1024 * 1024;
            const maxPhotos = 5;
            let existingPhotos = [];
            let newPhotos = [];
            let tempIdCounter = -1;

            initializePhotos();

            function initializePhotos() {
                const jsonVal = $existingDataField.val();
                if (jsonVal) {
                    try {
                        const parsed = JSON.parse(jsonVal);
                        $.each(parsed, function (i, p) {
                            if (p.isTemp) {
                                newPhotos.push({
                                    key: p.key,
                                    fileName: p.fileName,
                                    previewUrl: p.filePath,
                                    caption: p.caption || '',
                                    isTemp: true
                                });
                                tempIdCounter = Math.min(tempIdCounter, p.key - 1);
                            } else {
                                p.deleted = false;
                                if (!p.caption) p.caption = '';
                                existingPhotos.push(p);
                            }
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

                const currentCount = existingPhotos.filter(p => !p.deleted).length + newPhotos.length;
                if (currentCount + files.length > maxPhotos) {
                    alert(`照片數量限制為 ${maxPhotos} 張。\n目前已有 ${currentCount} 張，您試圖加入 ${files.length} 張。`);
                    $fileInput.val('');
                    return;
                }

                for (let i = 0; i < files.length; i++) {
                    const file = files[i];
                    if (file.size > maxSize) { alert(`${file.name} 太大`); continue; }
                    if (newPhotos.some(np => np.file && np.file.name === file.name && np.file.size === file.size)) continue;

                    const key = tempIdCounter--;
                    const previewUrl = URL.createObjectURL(file);
                    newPhotos.push({ key: key, file: file, previewUrl: previewUrl, caption: '' });
                }
                renderPhotos();
            }

            function rebuildFileInput() {
                const dt = new DataTransfer();
                $.each(newPhotos, function (i, p) {
                    if (p.file) {
                        dt.items.add(p.file);
                    }
                });
                $fileInput[0].files = dt.files;
            }

            function updateMetadataField() {
                const activeExisting = $.grep(existingPhotos, function (p) { return !p.deleted; });
                const metadata = [];

                $.each(activeExisting, function (i, p) {
                    metadata.push({
                        key: p.key,
                        caption: p.caption
                    });
                });

                $.each(newPhotos, function (i, p) {
                    metadata.push({
                        key: p.key,
                        fileName: p.file ? p.file.name : (p.fileName || ''),
                        caption: p.caption
                    });
                });

                $metadataField.val(JSON.stringify(metadata));
            }

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

            $photoList.on('change input', '.photo-caption-input', function () {
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
