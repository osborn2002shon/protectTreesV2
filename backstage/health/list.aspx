<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="list.aspx.cs" Inherits="protectTreesV2.backstage.health.list" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="~/_uc/health/uc_healthRecordModal.ascx" TagPrefix="uc1" TagName="uc_healthRecordModal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <script>
        function showHealthRecordModal() {
            var modalEl = document.getElementById('healthRecordModal');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    健檢資料管理 / 健檢紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    異動管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <nav class="nav nav-tabs mb-4">
        <a class="nav-link text-dark" href="main.aspx">健檢紀錄</a>
        <a class="nav-link active" href="list.aspx">異動管理</a>
        <a class="nav-link text-dark" href="upload.aspx">上傳多筆健檢紀錄</a>
        <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆健檢照片</a>
        <a class="nav-link text-dark" href="uploadFile.aspx">上傳多筆健檢附件</a>
    </nav>

    <div class="queryBox">
        <div class="queryBox-header">
            查詢條件
        </div>
        <div class="queryBox-body">
            <%-- 第一列：快速查詢 (我的紀錄 / 單位全部) --%>
            <div class="row mb-3">
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="RadioButtonList_scope" Text="快速查詢" class="form-label fw-bold" />
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
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="DropDownList_city" Text="縣市鄉鎮" />
                    <div class="d-flex gap-2">
                        <asp:DropDownList ID="DropDownList_city" runat="server" CssClass="form-select flex-fill" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_city_SelectedIndexChanged" />
                        <asp:DropDownList ID="DropDownList_area" runat="server" CssClass="form-select flex-fill" />
                    </div>
                </div>
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="DropDownList_species" Text="樹種" />
                    <asp:DropDownList ID="DropDownList_species" runat="server" CssClass="form-select" data-combobox="species" data-combobox-placeholder="請輸入樹種" />
                </div>
            </div>

            <%-- 第三列：日期區間、關鍵字 --%>
            <div class="row">
                <div class="col">
                    <label>調查日期</label>
                    <div class="d-flex align-items-center gap-2">
                        <asp:TextBox ID="TextBox_dateStart" runat="server" CssClass="form-control" TextMode="Date" />
                        <span>~</span>
                        <asp:TextBox ID="TextBox_dateEnd" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                </div>
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="TextBox_keyword" Text="關鍵字" />
                    <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="管理人、調查人、樹籍編號" />
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
              共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆紀錄
          </div>
    </div>

    <%-- 列表區塊 --%>
    <div class="table-responsive gv-tb">
        <asp:GridView ID="GridView_healthList" runat="server" 
            CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" AllowSorting="true" ShowHeaderWhenEmpty="true" 
            OnPageIndexChanging="GridView_healthList_PageIndexChanging" 
            OnRowCommand="GridView_healthList_RowCommand" 
            OnSorting="GridView_healthList_Sorting">
        
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

                <%-- 5. 調查日期 (重點欄位) --%>
                <asp:BoundField DataField="surveyDate" HeaderText="調查日期" SortExpression="surveyDate" DataFormatString="{0:yyyy/MM/dd}" />

                <%-- 6. 調查人 (重點欄位) --%>
                <asp:BoundField DataField="surveyor" HeaderText="調查人" SortExpression="surveyor" />

                <%-- 7. 健檢紀錄狀態 (這裡顯示的是該筆紀錄的狀態，不是樹的狀態) --%>
                <asp:TemplateField HeaderText="紀錄狀態" SortExpression="dataStatus">
                    <ItemTemplate>
                        <%-- 狀態文字 (1=完稿, 0=草稿) --%>
                        <%# Convert.ToInt32(Eval("dataStatus")) == 1 ? "定稿" : "草稿" %>
        
                        <%-- 最後更新時間 --%>
                        <%# Eval("lastUpdate") == null ? "" : 
                            "<div class='tbMiniInfo'>" + Eval("lastUpdate", "{0:yyyy/MM/dd HH:mm}") + "</div>" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <%-- 8. 動作區塊 (針對 healthID 操作) --%>
                <asp:TemplateField HeaderText="動作" ItemStyle-Width="180px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <div class="d-flex gap-2 justify-content-center">
                            <%-- (A) 檢視 --%>
                            <asp:LinkButton ID="LinkButton_view" runat="server" 
                                CssClass="btn btn-sm btn-info text-white" 
                                Text="檢視"
                                CommandName="_ViewHealth" 
                                CommandArgument='<%# Eval("healthID") %>' />

                            <%-- (B) 編輯 --%>
                            <asp:LinkButton ID="LinkButton_edit" runat="server" 
                                CssClass="btn btn-sm btn-primary" 
                                Text="編輯"
                                CommandName="_EditHealth" 
                                CommandArgument='<%# Eval("healthID") %>' />

                            <%-- (C) 刪除 --%>
                            <asp:LinkButton ID="LinkButton_del" runat="server" 
                                Text="刪除"
                                CommandName="_DeleteHealth" 
                                CommandArgument='<%# Eval("healthID") %>'
                                CssClass='<%# Convert.ToInt32(Eval("dataStatus")) == 0 ? "btn btn-sm btn-danger" : "btn btn-sm btn-secondary disabled" %>'
                                Enabled='<%# Convert.ToInt32(Eval("dataStatus")) == 0 %>'
                                OnClientClick="return confirm('確定要刪除這筆健檢紀錄嗎？\n此動作無法復原。');" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

            <EmptyDataTemplate>
                <div class="text-center py-5 text-muted">
                    <p>查無符合條件的健檢紀錄。</p>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div class="modal fade" id="healthRecordModal" tabindex="-1" aria-hidden="true" style="color:#000;">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    健檢紀錄
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="formCard card mb-4">
                        <div class="card-header">基本資料</div>
                        <div class="card-body">
                            <div class="row g-3">
                                <%-- 系統紀錄編號 --%>
                                <div class="col-md-4 col-sm-6">
                                    <label class="form-label text-muted">系統紀錄編號</label>
                                    <div class="fw-bold ">
                                        <asp:Label ID="lblModal_healthId" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <%-- 系統樹籍編號 --%>
                                <div class="col-md-4 col-sm-6">
                                    <label class="form-label text-muted">系統樹籍編號</label>
                                    <div class="fw-bold">
                                        <asp:Label ID="lblModal_systemTreeNo" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <%-- 資料狀態 --%>
                                <div class="col-md-4 col-sm-6">
                                    <label class="form-label text-muted">資料狀態</label>
                                    <div class="fw-bold">
                                        <asp:Label ID="lblModal_status" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <%-- 所在地 --%>
                                <div class="col-md-4 col-sm-6">
                                    <label class="form-label text-muted">所在地</label>
                                    <div class="fw-bold">
                                        <asp:Label ID="lblModal_location" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <%-- 樹種及學名 --%>
                                <div class="col-md-4 col-sm-6">
                                    <label class="form-label text-muted">樹種及學名</label>
                                    <div class="fw-bold">
                                        <asp:Label ID="lblModal_species" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <%-- 最後更新 --%>
                                <div class="col-md-4 col-sm-6">
                                    <label class="form-label text-muted">最後更新</label>
                                    <div class="fw-bold">
                                        <asp:Label ID="lblModal_lastUpdate" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <%-- 備註 --%>
                              <%--  <div class="col-12">
                                    <label class="form-label text-muted">備註</label>
                                    <div class="fw-bold">
                                        <asp:Label ID="lblModal_Memo" runat="server"></asp:Label>
                                    </div>
                                </div>--%>
                            </div>
                        </div>
                    </div>

                    <uc1:uc_healthRecordModal runat="server" id="uc_healthRecordModal" />
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
