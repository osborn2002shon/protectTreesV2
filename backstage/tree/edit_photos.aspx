<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="edit_photos.aspx.cs" Inherits="protectTreesV2.backstage.tree.edit_photos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    樹籍管理 / 樹木照片
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    樹木照片
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:HiddenField ID="hfTreeID" runat="server" />
    <div class="row">
        <div class="col">
            <asp:Label ID="lblTreeInfo" runat="server" />
        </div>
    </div>
    <div class="row">
        <div class="col">
            <asp:Label runat="server" AssociatedControlID="fuPhotos" Text="樹木照片 (最多 5 張，每張 5MB)" />
            <asp:FileUpload ID="fuPhotos" runat="server" AllowMultiple="true" />
            <asp:Button ID="btnUpload" runat="server" Text="上傳" OnClick="btnUpload_Click" />
            <asp:Button ID="btnBack" runat="server" Text="返回樹籍" CausesValidation="false" OnClick="btnBack_Click" />
        </div>
    </div>
    <div class="row">
        <div class="col">
            <asp:Repeater ID="rptPhotos" runat="server" OnItemCommand="rptPhotos_ItemCommand">
                <HeaderTemplate>
                    <div class="row">
                        <div class="col">縮圖</div>
                        <div class="col">說明</div>
                        <div class="col">封面</div>
                        <div class="col">操作</div>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="row">
                        <div class="col">
                            <asp:Image runat="server" ImageUrl='<%# ResolvePhotoPath(Eval("FilePath")) %>' Width="150" />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" Text='<%# Eval("Caption") %>' />
                        </div>
                        <div class="col">
                            <asp:Label runat="server" Text='<%# (bool)Eval("IsCover") ? "是" : "否" %>' />
                        </div>
                        <div class="col">
                            <asp:LinkButton runat="server" CommandName="cover" CommandArgument='<%# Eval("PhotoID") %>' Text="設為封面" />
                            <asp:LinkButton runat="server" CommandName="delete" CommandArgument='<%# Eval("PhotoID") %>' Text="刪除" OnClientClick="return confirm('確認刪除？');" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
