<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="protectTreesV2.backstage.health.main" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    健檢資料管理 / 健檢紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    健檢紀錄
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <nav class="nav nav-tabs mb-4">
        <a class="nav-link active" href="main.aspx">健檢紀錄</a>
        <a class="nav-link text-dark" href="list.aspx">異動管理</a>
        <a class="nav-link text-dark" href="upload.aspx">上傳多筆健檢紀錄</a>
        <a class="nav-link text-dark" href="uploadPhoto.aspx">上傳多筆健檢照片</a>
        <a class="nav-link text-dark" href="uploadFile.aspx">上傳多筆健檢附件</a>
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
                    <asp:DropDownList ID="DropDownList_species" runat="server" CssClass="form-select" data-combobox="species" data-combobox-placeholder="請輸入樹種" />
                </div>
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="TextBox_keyword" Text="關鍵字" />
                    <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="管理人、調查人、樹籍編號、樹木編號、管轄編碼" />
                </div>
            </div>

           <div class="row">
                <div class="col">
                    <asp:Label runat="server" AssociatedControlID="CheckBox_onlyNoRecord" Text="查詢選項" />
                    <div class="d-flex flex-wrap align-items-center gap-3 pt-1">
                        <%-- CheckBox 1: 僅撈取無健檢紀錄樹籍 --%>
                        <div class="form-check form-check-inline m-0">
                            <asp:CheckBox ID="CheckBox_onlyNoRecord" runat="server" Text="僅撈取無健檢紀錄樹籍" />
                        </div>

                        <%-- CheckBox 2: 包含草稿資料 --%>
                        <div class="form-check form-check-inline m-0">
                            <asp:CheckBox ID="CheckBox_includeDraft" runat="server" Text="包含草稿資料" />
                        </div>
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
                <%-- 1. 上傳檔案製作 --%>
                <asp:TemplateField HeaderText="上傳<br/>檔案製作" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" >
                    <ItemTemplate>
                        <%-- 情境 A: 還沒加入 (顯示按鈕) --%>
                        <asp:LinkButton ID="LinkButton_addToUpload" runat="server" 
                            CssClass="btn btn-secondary" 
                            CommandName="_AddToUpload" 
                            CommandArgument='<%# Eval("treeID") %>'
                            Visible='<%# !(bool)Eval("isAdded") %>' > 
                            加入
                        </asp:LinkButton>

                        <%-- 情境 B: 已經加入 (顯示文字) --%>
                        <asp:Label ID="Label_added" runat="server" 
                            Text="已加入" 
                            Visible='<%# (bool)Eval("isAdded") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <%-- 2. 系統樹籍編號 --%>
                <asp:BoundField DataField="systemTreeNo" HeaderText="系統<br/>樹籍編號" SortExpression="systemTreeNo" HtmlEncode="false" />
            
                <%-- 3. 機關樹木編號 --%>
                <asp:BoundField DataField="agencyTreeNo" HeaderText="機關<br/>樹木編號" SortExpression="agencyTreeNo" HtmlEncode="false" />
            
                <%-- 4. 縣市鄉鎮 --%>
                <asp:TemplateField HeaderText="縣市鄉鎮" SortExpression="areaID">
                    <ItemTemplate>
                        <%# Eval("cityName") %><br />
                        <small class="text-muted"><%# Eval("areaName") %></small>
                    </ItemTemplate>
                </asp:TemplateField>

                <%-- 5. 樹種 --%>
                <asp:BoundField DataField="speciesName" HeaderText="樹種" SortExpression="speciesName" />

                <%-- 6. 管理人 --%>
                <asp:BoundField DataField="manager" HeaderText="管理人" SortExpression="manager" />

                <%-- 7. 樹籍狀態 --%>
                <asp:TemplateField HeaderText="樹籍狀態" SortExpression="treeStatus">
                    <ItemTemplate>
                        <span >
                         <%# Eval("treeStatusText") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <%-- [新增] 8. 調查日期 (來自 Tree_HealthRecord) --%>
                <asp:BoundField DataField="surveyDate" HeaderText="調查日期" SortExpression="surveyDate" DataFormatString="{0:yyyy/MM/dd}" />

                <%-- [新增] 9. 調查人 (來自 Tree_HealthRecord) --%>
                <asp:BoundField DataField="surveyor" HeaderText="調查人" SortExpression="surveyor" />

                <%-- [新增] 10. 健檢紀錄狀態 (來自 Tree_HealthRecord 的 dataStatus) --%>
                <asp:TemplateField HeaderText="健檢紀錄<br/>狀態" SortExpression="dataStatus">
                    <ItemTemplate>
                        <%-- 如果沒有 healthID (無紀錄) 顯示 "--"，否則判斷狀態 (假設 1=完稿, 0=草稿) --%>
                        <%# Eval("healthID") == null ? "--" : (Convert.ToInt32(Eval("dataStatus")) == 1 ? "定稿" : "草稿") %>
        
                        <%-- 邏輯 B: 時間區塊 --%>
                        <%-- 如果 lastUpdate 不是 null，就組出那個 div，否則顯示空字串 --%>
                        <%# Eval("lastUpdate") == null ? "" : 
                            "<div class='tbMiniInfo'>" + Eval("lastUpdate", "{0:yyyy/MM/dd HH:mm}") + "</div>" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <%-- 11. 動作區塊 --%>
                <asp:TemplateField HeaderText="動作" ItemStyle-Width="250px">
                    <ItemTemplate>
                        <div class="d-flex gap-1 justify-content-center">
                            <%-- (A) 新增紀錄 --%>
                            <asp:LinkButton ID="LinkButton_addRecord" runat="server" 
                                CssClass="btn btn-sm btn-success" 
                                Text="新增紀錄"
                                CommandName="_AddRecord" 
                                CommandArgument='<%# Eval("treeID") %>' />

                            <%-- (B) 檢視樹籍 --%>
                            <asp:LinkButton ID="LinkButton_viewTree" runat="server" 
                                CssClass="btn btn-sm btn-info text-white" 
                                Text="檢視樹籍"
                                CommandName="_ViewTree" 
                                CommandArgument='<%# Eval("treeID") %>' />

                            <%-- (C) 檢視紀錄 --%>
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

    <div class="row mt-3">
        <div class="col text-center">
            <%-- 產製 Excel 按鈕 --%>
        <asp:LinkButton ID="LinkButton_generateExcel" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_generateExcel_Click">
           產製多筆上傳Excel
        </asp:LinkButton>

        <%-- 清空列表 按鈕 --%>
        <asp:LinkButton ID="LinkButton_clearList" runat="server" 
            CssClass="btn btn-outline-danger" OnClientClick="return confirm('確定要清空所有已加入的樹籍嗎？');" OnClick="LinkButton_clearList_Click">
            清空列表
        </asp:LinkButton>
        </div>
    </div>

    <div class="table-responsive gv-tb mt-3">
        <asp:GridView ID="GridView_selectedList" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" OnRowCommand="GridView_selectedList_RowCommand">
            <Columns>
                <%-- 1. 系統樹籍編號 --%>
            <asp:BoundField DataField="systemTreeNo" HeaderText="系統<br>樹籍編號" HtmlEncode="false" />

                <%-- 2. 機關樹木編號 --%>
                <asp:BoundField DataField="agencyTreeNo" HeaderText="機關<br>樹木編號" HtmlEncode="false" />

                <%-- 3. 機關管轄編號 --%>
                <asp:BoundField DataField="agencyJurisdictionCode" HeaderText="機關<br>管轄編號" HtmlEncode="false" />

                <%-- 4. 樹種 --%>
                <asp:BoundField DataField="speciesName" HeaderText="樹種" />

                <%-- 5. 管理人 --%>
                <asp:BoundField DataField="manager" HeaderText="管理人" />

                <%-- 6. 動作 (移除按鈕) --%>
                <asp:TemplateField HeaderText="動作" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton_remove" runat="server" 
                            CssClass="btn btn-danger" 
                            Text="移除" 
                            CommandName="_Remove" 
                            CommandArgument='<%# Eval("treeID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

            <EmptyDataTemplate>
                <div class="text-center py-3 text-muted">
                    尚未加入任何樹籍。
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
