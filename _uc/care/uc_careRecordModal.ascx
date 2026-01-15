<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_careRecordModal.ascx.cs" Inherits="protectTreesV2._uc.care.uc_careRecordModal" %>
<link rel="stylesheet" href="https://unpkg.com/img-comparison-slider@8/dist/styles.css" />
<style>
    .care-compare {
        width: 100%;
        aspect-ratio: 4 / 3;
        background: #000;
        border-radius: 0.5rem;
        overflow: hidden;
        border: 1px solid #dee2e6;
        box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
    }

    .care-compare img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        object-position: center;
    }
</style>
<div class="modalForm">
    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="true">
        <div class="text-center text-muted py-4">尚未載入養護紀錄。</div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phContent" runat="server" Visible="false">
        <div class="formCard card mb-4">
            <div class="card-header">養護說明</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">系統紀錄編號</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litCareId" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">資料狀態</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litStatus" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">養護日期</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litCareDate" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">記錄人員</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litRecorder" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">覆核人員</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litReviewer" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <label class="form-label text-muted">最後更新</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litLastUpdate" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="formCard card mb-4">
            <div class="card-header">生長情形概況</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-12">
                        <label class="form-label text-muted">樹冠枝葉</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litCrownSummary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">主莖幹</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTrunkSummary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">根部及地際部</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litRootSummary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">生育地環境</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litEnvSummary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">樹冠或主莖幹與鄰接物</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litAdjacentSummary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="formCard card mb-4">
            <div class="card-header">養護管理作業</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-12">
                        <label class="form-label text-muted">1. 危險枯枝清除</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTask1Summary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">2. 植栽基盤維護暨環境整理</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTask2Summary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">3. 樹木健康管理</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTask3Summary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">4. 營養評估追肥</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTask4Summary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">5. 安全衛生防護</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litTask5Summary" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="formCard card mb-4">
            <div class="card-header">養護照片</div>
            <div class="card-body">
                <asp:PlaceHolder ID="phPhotoEmpty" runat="server" Visible="false">
                    <div class="text-muted">無照片。</div>
                </asp:PlaceHolder>
                <asp:Repeater ID="rptPhotos" runat="server">
                    <HeaderTemplate>
                        <div class="row g-3">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="col-12">
                            <div class="border rounded p-3">
                                <div class="row g-3">
                                    <div class="col-12">
                                        <label class="form-label text-muted">施作項目名稱</label>
                                        <div class="fw-bold"><%# FormatText(Eval("itemName") as string) %></div>
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label text-muted">施作前後照片對照</label>
                                        <img-comparison-slider class="care-compare">
                                            <img slot="first" src='<%# ResolvePhotoPreview(Eval("beforeFilePath")) %>' alt='<%# FormatText(Eval("beforeFileName") as string) %>' />
                                            <img slot="second" src='<%# ResolvePhotoPreview(Eval("afterFilePath")) %>' alt='<%# FormatText(Eval("afterFileName") as string) %>' />
                                        </img-comparison-slider>
                                        <div class="row g-2 mt-2">
                                            <div class="col-md-6">
                                                <label class="form-label text-muted mb-1">施作前照片</label>
                                                <div class="small text-truncate">
                                                    <a href='<%# ResolvePhotoUrl(Eval("beforeFilePath")) %>' target="_blank" rel="noopener" title='<%# FormatText(Eval("beforeFileName") as string) %>'>
                                                        <%# FormatText(Eval("beforeFileName") as string) %>
                                                    </a>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <label class="form-label text-muted mb-1">施作後照片</label>
                                                <div class="small text-truncate">
                                                    <a href='<%# ResolvePhotoUrl(Eval("afterFilePath")) %>' target="_blank" rel="noopener" title='<%# FormatText(Eval("afterFileName") as string) %>'>
                                                        <%# FormatText(Eval("afterFileName") as string) %>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </asp:PlaceHolder>
</div>
<script defer src="https://unpkg.com/img-comparison-slider@8/dist/index.js"></script>
