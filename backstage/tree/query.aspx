<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="query.aspx.cs" Inherits="protectTreesV2.backstage.tree.query" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    樹籍管理 / 樹籍資料查詢
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    樹籍資料查詢
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
<%--    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>
        <div class="queryBox">
            <div class="queryBox-header">
                查詢條件
            </div>
            <div class="queryBox-body">
                <div class="row">
                    <div class="col">
                        <asp:Label runat="server" AssociatedControlID="ddlCity" Text="縣市鄉鎮" />
                        <div class="d-flex gap-2">
                            <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" />
                            <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-select" />
                        </div>
                        
                    </div>
                    <div class="col">
                        <asp:Label runat="server" AssociatedControlID="ddlEditStatus" Text="編輯狀態" />
                        <asp:DropDownList ID="ddlEditStatus" runat="server"  CssClass="form-select" />
                    </div>
                    <div class="col">
                        <asp:Label runat="server" AssociatedControlID="ddlTreeStatus" Text="樹籍狀態" />
                        <asp:DropDownList ID="ddlTreeStatus" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col">
                        <asp:Label runat="server" AssociatedControlID="ddlSpecies" Text="樹種" />
                        <asp:DropDownList ID="ddlSpecies" runat="server" CssClass="form-select" />
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <asp:Label runat="server" AssociatedControlID="txtSurveyStart" Text="調查日期起迄" />
                        <div class="d-flex gap-2">
                            <asp:TextBox ID="txtSurveyStart" runat="server" TextMode="Date" CssClass="form-control" />
                            至
                            <asp:TextBox ID="txtSurveyEnd" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="col">
                        <asp:Label runat="server" AssociatedControlID="txtAnnouncementStart" Text="公告日期起迄" />
                        <div class="d-flex gap-2">
                            <asp:TextBox ID="txtAnnouncementStart" runat="server" TextMode="Date" CssClass="form-control" />
                            至
                            <asp:TextBox ID="txtAnnouncementEnd" runat="server" TextMode="Date" CssClass="form-control" />
                        </div>
                        
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <asp:Label runat="server" AssociatedControlID="txtKeyword" Text="關鍵字" />
                        <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control" placeholder="管理人、樹籍編號、樹木編號、管轄編碼" />
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <asp:Button ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click"  CssClass="btn btn-primary" />
                        <asp:Button ID="btnReset" runat="server" Text="清除條件" OnClick="btnReset_Click"  CssClass="btn btn-primary" />
                    </div>
                </div>
            </div>
        </div>


            <div class="row m-0 mt-3 mb-3 align-items-center">
                <div class="col p-0">
                    <asp:Button ID="btnExport" runat="server" Text="下載列表" OnClick="btnExport_Click" CssClass="btn btn-primary" />
                    <asp:Button ID="btnAdd" runat="server" Text="新增樹籍" OnClick="btnAdd_Click" CssClass="btn btn-primary"/>
                    <asp:Label ID="lblCount" runat="server" />
                </div>
            </div>
            <div class="container-fluid gv-tb">
                <div class="table-responsive">
                    <asp:GridView ID="gvTrees" runat="server"  CssClass="gv" AutoGenerateColumns="false" AllowSorting="true" OnSorting="gvTrees_Sorting" OnRowCommand="gvTrees_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="選取">
                                <HeaderTemplate>
                                    <input type="checkbox" onclick="toggleSelectAll(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" />
                                    <asp:HiddenField ID="hfTreeId" runat="server" Value='<%# Eval("TreeID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="SystemTreeNo" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" HeaderText="系統樹籍<br />編號" SortExpression="SystemTreeNo" />
                            <asp:BoundField DataField="AgencyTreeNo" ItemStyle-HorizontalAlign="Center" HeaderText="樹木編號" SortExpression="AgencyTreeNo" />
                            <asp:BoundField DataField="AgencyJurisdictionCode" ItemStyle-HorizontalAlign="Center" HeaderText="管轄編碼" SortExpression="AgencyJurisdictionCode" />
                            <asp:BoundField DataField="CityName" ItemStyle-HorizontalAlign="Center" HeaderText="縣市" SortExpression="CityName" />
                            <asp:BoundField DataField="AreaName" ItemStyle-HorizontalAlign="Center" HeaderText="鄉鎮" SortExpression="AreaName" />
                            <asp:BoundField DataField="SpeciesDisplayName" ItemStyle-HorizontalAlign="Center" HeaderText="樹種" SortExpression="SpeciesDisplayName" />
                            <asp:BoundField DataField="SurveyDate" ItemStyle-HorizontalAlign="Center" HeaderText="調查日期" DataFormatString="{0:yyyy/MM/dd}" SortExpression="SurveyDate" />
                            <asp:BoundField DataField="AnnouncementDisplay" ItemStyle-HorizontalAlign="Center" HeaderText="公告日期" SortExpression="AnnouncementDate" />
                            <asp:BoundField DataField="StatusText" ItemStyle-HorizontalAlign="Center" HeaderText="樹籍狀態" SortExpression="Status" />
                            <asp:BoundField DataField="EditStatusText" ItemStyle-HorizontalAlign="Center" HeaderText="編輯狀態" SortExpression="EditStatus" />
                            <asp:TemplateField HeaderText="操作">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditTree" CommandArgument='<%# Eval("TreeID") %>' Text="編輯" />
                                    <asp:LinkButton ID="lnkView" runat="server" CommandName="ViewTree" CommandArgument='<%# Eval("TreeID") %>' Text="檢視" Visible='<%# ((int)Eval("EditStatus") == 1) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <div class="row m-0 mt-3 mb-3 align-items-end">
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="ddlBulkStatus" Text="將勾選樹籍案件進行快速設定" />
                    <asp:DropDownList ID="ddlBulkStatus" runat="server" CssClass="form-select" />
                </div>
                <div class="col" id="bulkAnnouncementWrapper">
                    <asp:Label runat="server" AssociatedControlID="txtBulkAnnouncement" Text="公告日期" />
                    <asp:TextBox ID="txtBulkAnnouncement" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col">
                    <asp:Button ID="btnApplyStatus" runat="server" Text="套用" CssClass="btn btn-primary" OnClick="btnApplyStatus_Click" />
                </div>
            </div>
<%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
    <script type="text/javascript">
        function toggleSelectAll(master) {
            console.log('Y;');
            var grid = document.getElementById('<%= gvTrees.ClientID %>');
            if (!grid) return;
            var inputs = grid.getElementsByTagName('input');
            for (var i = 0; i < inputs.length; i++) {
                
                if (inputs[i].type === 'checkbox' && inputs[i].id.indexOf('chkSelect') > -1) {
                    inputs[i].checked = master.checked;
                }
            }
        }

        function toggleAnnouncementField() {
            var ddl = document.getElementById('<%= ddlBulkStatus.ClientID %>');
            var wrapper = document.getElementById('bulkAnnouncementWrapper');
            var announcementInput = document.getElementById('<%= txtBulkAnnouncement.ClientID %>');
            if (!ddl || !wrapper) return;

            var shouldShow = ddl.value === '1'; //已公告列管
            wrapper.style.display = shouldShow ? 'inline-block' : 'none';

            if (!shouldShow && announcementInput) {
                announcementInput.value = '';
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            var ddl = document.getElementById('<%= ddlBulkStatus.ClientID %>');
            if (ddl) {
                ddl.addEventListener('change', toggleAnnouncementField);
            }
            toggleAnnouncementField();
        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
