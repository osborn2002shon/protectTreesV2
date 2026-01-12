<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="list.aspx.cs" Inherits="protectTreesV2.backstage.care.list" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    養護資料管理 / 養護紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    異動管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <nav class="nav nav-tabs mb-4">
        <a class="nav-link text-dark" href="main.aspx">養護管理</a>
        <a class="nav-link active" href="list.aspx">異動管理</a>
        <a class="nav-link text-dark" href="upload.aspx">上傳多筆養護紀錄</a>
        <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆養護照片</a>
    </nav>

    <div class="queryBox">
        <div class="queryBox-header">
            查詢條件
        </div>
        <div class="queryBox-body">
            <div class="row mb-3">
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="RadioButtonList_scope" Text="快速查詢" CssClass="form-label fw-bold" />
                    <div class="d-flex align-items-center">
                        <asp:RadioButtonList ID="RadioButtonList_scope" runat="server"
                            RepeatDirection="Horizontal" RepeatLayout="Flow"
                            CssClass="d-flex gap-3">
                            <asp:ListItem Value="My">我的紀錄</asp:ListItem>
                            <asp:ListItem Value="Unit" Selected="True">單位全部</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>

            <div class="row mb-2">
                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="DropDownList_city" Text="縣市鄉鎮" />
                    <div class="d-flex gap-2">
                        <asp:DropDownList ID="DropDownList_city" runat="server" CssClass="form-select flex-fill" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_city_SelectedIndexChanged" />
                        <asp:DropDownList ID="DropDownList_area" runat="server" CssClass="form-select flex-fill" />
                    </div>
                </div>
                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="DropDownList_species" Text="樹種" />
                    <asp:DropDownList ID="DropDownList_species" runat="server" CssClass="form-select" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <label>養護日期</label>
                    <div class="d-flex align-items-center gap-2">
                        <asp:TextBox ID="TextBox_dateStart" runat="server" CssClass="form-control" TextMode="Date" />
                        <span>~</span>
                        <asp:TextBox ID="TextBox_dateEnd" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                </div>
                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="TextBox_keyword" Text="關鍵字" />
                    <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="管理人、記錄人員、覆核人員、樹籍編號" />
                </div>
            </div>

            <div class="row mt-4">
                <div class="col text-center">
                    <asp:LinkButton ID="LinkButton_search" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_search_Click">
                         查詢
                    </asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="queryBox-footer"></div>
    </div>

    <div class="row m-0 mt-3 mb-3 align-items-center">
          <div class="col p-0 text-end">
              共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆養護紀錄
          </div>
    </div>

    <div class="table-responsive gv-tb">
        <asp:GridView ID="GridView_careList" runat="server"
            CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" AllowSorting="true" ShowHeaderWhenEmpty="true"
            OnPageIndexChanging="GridView_careList_PageIndexChanging"
            OnRowCommand="GridView_careList_RowCommand"
            OnSorting="GridView_careList_Sorting">

            <Columns>
                <asp:BoundField DataField="systemTreeNo" HeaderText="系統<br/>樹籍編號" SortExpression="systemTreeNo" HtmlEncode="false" />
                <asp:BoundField DataField="agencyTreeNo" HeaderText="機關<br/>樹木編號" SortExpression="agencyTreeNo" HtmlEncode="false" />

                <asp:TemplateField HeaderText="縣市鄉鎮" SortExpression="areaID">
                    <ItemTemplate>
                        <%# Eval("cityName") %><br />
                        <small class="text-muted"><%# Eval("areaName") %></small>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="speciesName" HeaderText="樹種" SortExpression="speciesName" />
                <asp:BoundField DataField="careDate" HeaderText="養護日期" SortExpression="careDate" DataFormatString="{0:yyyy/MM/dd}" />
                <asp:BoundField DataField="recorder" HeaderText="記錄人員" SortExpression="recorder" />
                <asp:BoundField DataField="reviewer" HeaderText="覆核人員" SortExpression="reviewer" />

                <asp:TemplateField HeaderText="紀錄狀態" SortExpression="dataStatus">
                    <ItemTemplate>
                        <%# Convert.ToInt32(Eval("dataStatus")) == 1 ? "定稿" : "草稿" %>
                        <%# Eval("lastUpdate") == null ? "" :
                            "<div class='tbMiniInfo'>" + Eval("lastUpdate", "{0:yyyy/MM/dd HH:mm}") + "</div>" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="動作" ItemStyle-Width="180px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <div class="d-flex gap-2 justify-content-center">
                            <asp:LinkButton ID="LinkButton_view" runat="server"
                                CssClass="btn btn-sm btn-info text-white"
                                Text="檢視"
                                CommandName="_ViewCare"
                                CommandArgument='<%# Eval("careID") %>' />
                            <asp:LinkButton ID="LinkButton_edit" runat="server"
                                CssClass="btn btn-sm btn-primary"
                                Text="編輯"
                                CommandName="_EditCare"
                                CommandArgument='<%# Eval("careID") %>' />
                            <asp:LinkButton ID="LinkButton_delete" runat="server"
                                Text="刪除"
                                CommandName="_DeleteCare"
                                CommandArgument='<%# Eval("careID") %>'
                                CssClass='<%# Convert.ToInt32(Eval("dataStatus")) == 0 ? "btn btn-sm btn-danger" : "btn btn-sm btn-secondary disabled" %>'
                                Enabled='<%# Convert.ToInt32(Eval("dataStatus")) == 0 %>'
                                OnClientClick="return confirm('確認刪除草稿紀錄？');" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

            <EmptyDataTemplate>
                <div class="text-center py-5 text-muted">
                    <p>查無符合條件的養護紀錄。</p>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</asp:Content>
