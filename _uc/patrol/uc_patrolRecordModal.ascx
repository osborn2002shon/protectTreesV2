<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_patrolRecordModal.ascx.cs" Inherits="protectTreesV2._uc.patrol.uc_patrolRecordModal" %>
<div class="modalForm">
    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="true">
        <div class="text-center text-muted py-4">尚未載入巡查紀錄。</div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phContent" runat="server" Visible="false">
        <div class="formCard card mb-4">
            <div class="card-header">巡查紀錄</div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-4 col-sm-6">
                        <label class="form-label text-muted">是否有危害公共安全風險</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litRisk" runat="server" Mode="Encode" />
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label text-muted">巡查備註</label>
                        <div class="fw-bold">
                            <asp:Literal ID="litMemo" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="formCard card mb-4">
            <div class="card-header">照片</div>
            <div class="card-body">
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
                                <img src='<%# string.IsNullOrEmpty(Eval("FilePath") as string) ? "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==" : ResolveUrl(Eval("FilePath").ToString()) %>' alt='<%# Eval("FileName") %>' class="img-fluid rounded border shadow-sm" />
                            </a>
                            <div class="small text-muted mt-2 text-truncate" title='<%# Eval("FileName") %>'><%# Eval("FileName") %></div>
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
