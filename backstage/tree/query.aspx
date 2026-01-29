<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="query.aspx.cs" Inherits="protectTreesV2.backstage.tree.query" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    樹籍資料管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    樹籍資料管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            樹籍資料查詢條件
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
                    <asp:DropDownList ID="ddlEditStatus" runat="server" CssClass="form-select" />
                </div>
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="ddlTreeStatus" Text="樹籍狀態" />
                    <asp:DropDownList ID="ddlTreeStatus" runat="server" CssClass="form-select" />
                </div>
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="ddlSpecies" Text="樹種" />
                    <asp:DropDownList ID="ddlSpecies" runat="server" CssClass="form-select" data-combobox="species" data-combobox-placeholder="請輸入樹種" />
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
            <div class="row text-center">
                <div class="col">
                    <asp:LinkButton ID="LinkButton_Search" runat="server" OnClick="btnSearch_Click" CssClass="btn btn_main">
	                        <i class="fas fa-search me-1"></i>查詢
                    </asp:LinkButton>
                    <asp:LinkButton ID="LinkButton_Reset" runat="server" OnClick="btnReset_Click" CssClass="btn btn_main_line" Visible="false">
	                        <i class="fas fa-xmark me-1"></i>清除條件
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <div class="row d-flex align-items-center mt-2 mb-2">
        <div class="col-12 col-md-6 text-center text-md-start">
            <asp:LinkButton ID="LinkButton_btnAdd" runat="server"  OnClick="btnAdd_Click" CssClass="btn btn_main">
	            <i class="fas fa-file-circle-plus me-1"></i>新增樹籍
            </asp:LinkButton>
            <asp:LinkButton ID="LinkButton_btnExport" runat="server" OnClick="btnExport_Click" CssClass="btn btn_main_line">
	            <i class="fas fa-file-arrow-down me-1"></i>下載列表
            </asp:LinkButton>
        </div>
        <div class="col-12 col-md-6 text-center text-md-end">
            <asp:Label ID="lblCount" runat="server" />
        </div>
    </div>
    <div class="container-fluid gv-tb">
        <div class="table-responsive">
            <asp:GridView ID="gvTrees" runat="server" CssClass="gv" AutoGenerateColumns="false" AllowSorting="true" OnSorting="gvTrees_Sorting" OnRowCommand="gvTrees_RowCommand">
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
                    <asp:BoundField DataField="SystemTreeNo" HtmlEncode="false" HeaderText="系統樹籍編號" SortExpression="SystemTreeNo" />
                    <%--<asp:BoundField DataField="AgencyTreeNo" HeaderText="樹木編號" SortExpression="AgencyTreeNo" />
                    <asp:BoundField DataField="AgencyJurisdictionCode" HeaderText="管轄編碼" SortExpression="AgencyJurisdictionCode" />--%>
                    <asp:BoundField DataField="CityName" HeaderText="縣市" SortExpression="CityName" />
                    <asp:BoundField DataField="AreaName" HeaderText="鄉鎮" SortExpression="AreaName" />
                    <asp:BoundField DataField="SpeciesCommonName" HeaderText="樹種" SortExpression="SpeciesCommonName" />
                    <asp:BoundField DataField="SurveyDate" HeaderText="調查日期" DataFormatString="{0:yyyy/MM/dd}" SortExpression="SurveyDate" />
                    <asp:BoundField DataField="AnnouncementDisplay" HeaderText="公告日期" SortExpression="AnnouncementDate" />
                    <asp:BoundField DataField="StatusText" HeaderText="樹籍狀態" SortExpression="Status" />
                    <asp:BoundField DataField="EditStatusText" HeaderText="編輯狀態" SortExpression="EditStatus" />
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkView" runat="server" CommandName="ViewTree" CommandArgument='<%# Eval("TreeID") %>'
                                Text="檢視" Enabled='<%# ((int)Eval("EditStatus") == 1) %>' 
                                CssClass='<%# ((int)Eval("EditStatus") == 1) ? "btn btn-sm btn-outline-primary" : "btn btn-sm btn-outline-secondary disabled" %>' />
                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditTree" CommandArgument='<%# Eval("TreeID") %>' 
                                Text="編輯" CssClass="btn btn-sm btn-primary" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="queryBox mt-2">
        <div class="queryBox-header">
            快速設定樹籍狀態
            <div>透過選擇上方列表中的案件，進行快速設定樹籍狀態</div>
        </div>
        <div class="queryBox-body">
            <div class="row align-items-end">
                <div class="col">
                    <label>樹籍狀態</label>
                    <asp:DropDownList ID="ddlBulkStatus" runat="server" CssClass="form-select" />            
                </div>
                <div class="col" id="bulkAnnouncementWrapper">
                    <label>公告日期</label>
                    <asp:TextBox ID="txtBulkAnnouncement" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col">
                    <asp:LinkButton ID="LinkButton_btnApplyStatus" runat="server" CssClass="btn btn_main" OnClick="btnApplyStatus_Click">
                        <i class="fa-solid fa-floppy-disk me-1"></i>套用設定
                    </asp:LinkButton>
                </div>
            </div>            
        </div>
    </div>
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
