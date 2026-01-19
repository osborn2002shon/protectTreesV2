<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="treeList.aspx.cs" Inherits="protectTreesV2.backstage.system.treeList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    樹種資料庫
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">.
    樹種資料庫
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            查詢條件
        </div>
        <div class="queryBox-body">
            <div class="row">
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="txtKeyword" Text="關鍵字" />
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control" placeholder="樹種名稱、學名、別名、備註" />
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <asp:Button ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" CssClass="btn btn-primary" />
                    <asp:Button ID="btnReset" runat="server" Text="清除條件" OnClick="btnReset_Click" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </div>

    <div class="row m-0 mt-3 mb-3 align-items-center">
        <div class="col p-0 d-flex justify-content-end align-items-center gap-2">
            <asp:Label ID="lblCount" runat="server" />
        </div>
    </div>
    <div class="container-fluid gv-tb">
        <div class="table-responsive">
            <asp:GridView ID="gvSpecies" runat="server" CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" PagerSettings-Mode="Numeric" PageButtonCount="10" OnPageIndexChanging="gvSpecies_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="commonName" ItemStyle-HorizontalAlign="Center" HeaderText="樹種名稱" />
                    <asp:TemplateField HeaderText="學名">
                        <ItemTemplate>
                            <asp:Literal ID="litScientificName" runat="server" Text='<%# FormatScientificName(Eval("scientificName") as string) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="是否為原生種">
                        <ItemTemplate>
                            <asp:Literal ID="litIsNative" runat="server" Text='<%# FormatNative(Eval("isNative")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="memo" ItemStyle-HorizontalAlign="Center" HeaderText="備註" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
