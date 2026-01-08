<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="list.aspx.cs" Inherits="protectTreesV2.backstage.patrol.list" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="~/_uc/patrol/uc_patrolRecordModal.ascx" TagPrefix="uc1" TagName="uc_patrolRecordModal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <script>
        function showPatrolRecordModal() {
            var modalEl = document.getElementById('patrolRecordModal');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    巡查資料管理 / 巡查紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    異動管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <nav class="nav nav-tabs mb-4">
        <a class="nav-link text-dark" href="main.aspx">巡查管理</a>
        <a class="nav-link active" href="list.aspx">異動管理</a>
        <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆巡查照片</a>
    </nav>

    <div class="queryBox">
        <div class="queryBox-header">
            查詢條件
        </div>
        <div class="queryBox-body">
            <%-- 第一列：快速查詢 --%>
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

            <%-- 第二列：一般查詢 (縣市鄉鎮、樹種) --%>
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

            <%-- 第三列：日期區間、關鍵字 --%>
            <div class="row">
                <div class="col-md-6">
                    <label>巡查日期</label>
                    <div class="d-flex align-items-center gap-2">
                        <asp:TextBox ID="TextBox_dateStart" runat="server" CssClass="form-control" TextMode="Date" />
                        <span>~</span>
                        <asp:TextBox ID="TextBox_dateEnd" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                </div>
                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="TextBox_keyword" Text="關鍵字" />
                    <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="管理人、巡查人、樹籍編號" />
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

    <%-- 統計筆數 --%>
    <div class="row m-0 mt-3 mb-3 align-items-center">
          <div class="col p-0 text-end">
              共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆巡查紀錄
          </div>
    </div>

    <%-- 列表區塊 --%>
    <div class="table-responsive gv-tb">
        <asp:GridView ID="GridView_patrolList" runat="server"
            CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" AllowSorting="true" ShowHeaderWhenEmpty="true"
            OnPageIndexChanging="GridView_patrolList_PageIndexChanging"
            OnRowCommand="GridView_patrolList_RowCommand"
            OnSorting="GridView_patrolList_Sorting">
        
            <Columns>
                <%-- 1. 系統樹籍編號 --%>
                <asp:BoundField DataField="systemTreeNo" HeaderText="系統<br/>樹籍編號" SortExpression="systemTreeNo" HtmlEncode="false" />
            
                <%-- 2. 機關樹木編號 --%>
                <asp:BoundField DataField="agencyTreeNo" HeaderText="機關<br/>樹木編號" SortExpression="agencyTreeNo" HtmlEncode="false" />
            
                <%-- 3. 縣市鄉鎮 --%>
                <asp:TemplateField HeaderText="縣市鄉鎮" SortExpression="areaID">
                    <ItemTemplate>
                        <%# Eval("cityName") %><br />
                        <small class="text-muted"><%# Eval("areaName") %></small>
                    </ItemTemplate>
                </asp:TemplateField>

                <%-- 4. 樹種 --%>
                <asp:BoundField DataField="speciesName" HeaderText="樹種" SortExpression="speciesName" />

                <%-- 5. 管理人 --%>
                <asp:BoundField DataField="manager" HeaderText="管理人" SortExpression="manager" />

                <%-- 6. 巡查日期 --%>
                <asp:BoundField DataField="patrolDate" HeaderText="巡查日期" SortExpression="patrolDate" DataFormatString="{0:yyyy/MM/dd}" />

                <%-- 7. 巡查人 --%>
                <asp:BoundField DataField="patroller" HeaderText="巡查人" SortExpression="patroller" />

                <%-- 8. 巡查紀錄狀態 (草稿/定稿) + 最後更新時間 --%>
                <asp:TemplateField HeaderText="紀錄狀態" SortExpression="dataStatus">
                    <ItemTemplate>
                        <%# Eval("dataStatusText") %>
                        <%# Eval("lastUpdate") == null ? "" :
                            "<div class='tbMiniInfo'>" + Eval("lastUpdate", "{0:yyyy/MM/dd HH:mm}") + "</div>" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <%-- 9. 動作區塊 --%>
                <asp:TemplateField HeaderText="動作" ItemStyle-Width="180px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <div class="d-flex gap-2 justify-content-center">
                            <asp:LinkButton ID="LinkButton_viewTree" runat="server"
                                CssClass="btn btn-sm btn-info text-white"
                                Text="檢視"
                                CommandName="_ViewPatrol"
                                CommandArgument='<%# Eval("patrolID") %>' />
                            <asp:LinkButton ID="LinkButton_edit" runat="server"
                                CssClass="btn btn-sm btn-primary"
                                Text="編輯"
                                CommandName="_EditPatrol"
                                CommandArgument='<%# Eval("patrolID") %>' />
                            <asp:LinkButton ID="LinkButton_delete" runat="server"
                                Text="刪除"
                                CommandName="_DeletePatrol"
                                CommandArgument='<%# Eval("patrolID") %>'
                                CssClass='<%# Convert.ToInt32(Eval("dataStatus")) == 0 ? "btn btn-sm btn-danger" : "btn btn-sm btn-secondary disabled" %>'
                                Enabled='<%# Convert.ToInt32(Eval("dataStatus")) == 0 %>'
                                OnClientClick="return confirm('確認刪除草稿紀錄？');" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

            <EmptyDataTemplate>
                <div class="text-center py-5 text-muted">
                    <p>查無符合條件的巡查紀錄。</p>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div class="modal fade" id="patrolRecordModal" tabindex="-1" aria-hidden="true" style="color:#000;">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    巡查紀錄
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="formCard card mb-4">
                        <div class="card-header">基本資料</div>
                        <div class="card-body">
                            <div class="row g-3">
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">系統紀錄編號</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litPatrolId" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">系統樹籍編號</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litSystemTreeNo" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">機關樹木編號</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litAgencyTreeNo" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">資料狀態</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litStatus" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">所在地</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litLocation" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">樹種</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litSpecies" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">管理人</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litManager" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">最後更新</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litLastUpdate" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">巡查日期</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litPatrolDate" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-3 col-sm-6">
                                    <label class="form-label text-muted">巡查人姓名</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litPatroller" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                                <div class="col-md-6 col-sm-12">
                                    <label class="form-label text-muted">來源單位</label>
                                    <div class="fw-bold">
                                        <asp:Literal ID="litSourceUnit" runat="server" Mode="Encode" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <uc1:uc_patrolRecordModal runat="server" id="uc_patrolRecordModal" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
