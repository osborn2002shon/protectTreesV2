<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="protectTreesV2.backstage.patrol.main" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    巡查資料管理 / 巡查紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    巡查紀錄
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <nav class="nav nav-tabs mb-4">
        <a class="nav-link active" href="main.aspx">巡查管理</a>
        <a class="nav-link text-dark" href="list.aspx">異動管理</a>
        <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆巡查照片</a>
    </nav>

    <div class="queryBox">
        <div class="queryBox-header">
            查詢條件
        </div>
        <div class="queryBox-body">
            <div class="row">
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="DropDownList_city" Text="縣市鄉鎮" />
                    <div class="d-flex gap-2">
                        <asp:DropDownList ID="DropDownList_city" runat="server" CssClass="form-select flex-fill" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_city_SelectedIndexChanged" />
                        <asp:DropDownList ID="DropDownList_area" runat="server" CssClass="form-select flex-fill" />
                    </div>
                </div>
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="DropDownList_species" Text="樹種" />
                    <asp:DropDownList ID="DropDownList_species" runat="server" CssClass="form-select" />
                </div>
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="TextBox_keyword" Text="關鍵字" />
                    <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="管理人、巡查人、樹籍編號、樹木編號、管轄編碼" />
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="RadioButtonList_queryOption" Text="查詢選項" />
                    <div class="pt-1">
                        <asp:RadioButtonList ID="RadioButtonList_queryOption" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex gap-3">
                            <asp:ListItem Text="近180天無紀錄" Value="NoRecord180" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="全部樹籍" Value="All"></asp:ListItem>
                            <asp:ListItem Text="從未有紀錄" Value="Never"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col text-center">
                    <asp:LinkButton ID="LinkButton_search" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_search_Click">查詢</asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="queryBox-footer"></div>
    </div>

    <div class="row m-0 mt-3 mb-3 align-items-center">
        <div class="col p-0 text-end">
            共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆樹籍
        </div>
    </div>

    <div class="table-responsive gv-tb">
        <asp:GridView ID="GridView_list" runat="server" CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" AllowSorting="true"
            ShowHeaderWhenEmpty="true" OnPageIndexChanging="GridView_list_PageIndexChanging" OnRowCommand="GridView_list_RowCommand" OnRowDataBound="GridView_list_RowDataBound"
            OnSorting="GridView_list_Sorting">
            <Columns>
                <asp:TemplateField HeaderText="上傳<br/>檔案製作" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton_addToUpload" runat="server"
                            CssClass="btn btn-secondary"
                            CommandName="_AddToUpload"
                            CommandArgument='<%# Eval("treeID") %>'
                            Visible='<%# !(bool)Eval("isAdded") %>'>
                            加入
                        </asp:LinkButton>

                        <asp:Label ID="Label_added" runat="server"
                            Text="已加入"
                            Visible='<%# (bool)Eval("isAdded") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="systemTreeNo" HeaderText="系統<br/>樹籍編號" SortExpression="systemTreeNo" HtmlEncode="false" />
                <asp:BoundField DataField="agencyTreeNo" HeaderText="機關<br/>樹木編號" SortExpression="agencyTreeNo" HtmlEncode="false" />

                <asp:TemplateField HeaderText="縣市鄉鎮" SortExpression="areaID">
                    <ItemTemplate>
                        <%# Eval("cityName") %><br />
                        <small class="text-muted"><%# Eval("areaName") %></small>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="speciesName" HeaderText="樹種" SortExpression="speciesName" />
                <asp:BoundField DataField="manager" HeaderText="管理人" SortExpression="manager" />

                <asp:TemplateField HeaderText="樹籍狀態" SortExpression="treeStatus">
                    <ItemTemplate>
                        <span><%# Eval("treeStatusText") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="patrolDate" HeaderText="巡查日期" SortExpression="patrolDate" DataFormatString="{0:yyyy/MM/dd}" />
                <asp:BoundField DataField="patroller" HeaderText="巡查人" SortExpression="patroller" />

                <asp:TemplateField HeaderText="巡查紀錄<br/>狀態" SortExpression="dataStatus">
                    <ItemTemplate>
                        <%# Eval("patrolRecordStatusText") %>
                        <%# Eval("lastUpdate") == null ? "" : "<div class='tbMiniInfo'>" + Eval("lastUpdate", "{0:yyyy/MM/dd HH:mm}") + "</div>" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="動作" ItemStyle-Width="250px">
                    <ItemTemplate>
                        <div class="d-flex gap-1 justify-content-center">
                            <asp:LinkButton ID="LinkButton_addRecord" runat="server"
                                CssClass="btn btn-sm btn-success"
                                Text="新增紀錄"
                                CommandName="_AddRecord"
                                CommandArgument='<%# Eval("treeID") %>' />

                            <asp:LinkButton ID="LinkButton_viewTree" runat="server"
                                CssClass="btn btn-sm btn-info text-white"
                                Text="檢視樹籍"
                                CommandName="_ViewTree"
                                CommandArgument='<%# Eval("treeID") %>' />

                            <asp:LinkButton ID="LinkButton_viewRecord" runat="server"
                                CssClass="btn btn-sm btn-primary"
                                Text="檢視紀錄"
                                CommandName="_ViewRecord"
                                CommandArgument='<%# Eval("treeID") %>' />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div class="text-center py-3 text-muted">
                    <p>查無符合條件的樹籍資料。</p>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div class="row mt-3 d-none">
        <div class="col text-center">
            <asp:LinkButton ID="LinkButton_generateExcel" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_generateExcel_Click">
               產製多筆上傳Excel
            </asp:LinkButton>

            <asp:LinkButton ID="LinkButton_clearList" runat="server"
                CssClass="btn btn-outline-danger" OnClientClick="return confirm('確定要清空所有已加入的樹籍嗎？');" OnClick="LinkButton_clearList_Click">
                清空列表
            </asp:LinkButton>
        </div>
    </div>

    <div class="table-responsive gv-tb mt-3 d-none">
        <asp:GridView ID="GridView_selectedList" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" OnRowCommand="GridView_selectedList_RowCommand">
            <Columns>
                <asp:BoundField HtmlEncode="false" DataField="systemTreeNo" HeaderText="系統<br/>樹籍編號" />
                <asp:BoundField HtmlEncode="false" DataField="agencyTreeNo" HeaderText="機關<br/>樹木編號" />
                <asp:BoundField DataField="cityName" HeaderText="縣市" />
                <asp:BoundField DataField="areaName" HeaderText="鄉鎮" />
                <asp:BoundField DataField="speciesName" HeaderText="樹種" />
                <asp:BoundField DataField="manager" HeaderText="管理人" />
                <asp:BoundField DataField="patrolDate" HeaderText="最後巡查" DataFormatString="{0:yyyy/MM/dd}" />
                <asp:BoundField DataField="patroller" HeaderText="巡查人" />
                <asp:TemplateField HeaderText="動作" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton_remove" runat="server" CssClass="btn btn-sm btn-outline-danger"
                            CommandName="_Remove" CommandArgument='<%# Eval("treeID") %>'>
                            移除
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div class="text-center py-3 text-muted">尚未加入任何樹籍。</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</asp:Content>
