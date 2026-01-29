<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="accountManage.aspx.cs" Inherits="protectTreesV2.backstage.system.accountManage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <script type="text/javascript">
        function confirmVerifyStatusChange() {
            var original = document.getElementById('<%= HiddenField_verifyStatusOriginal.ClientID %>').value;
            var selected = document.querySelector('input[name="<%= RadioButtonList_verifyStatus.UniqueID %>"]:checked');
            if (!selected) {
                return true;
            }

            var current = selected.value;
            if (original !== current) {
                return confirm('審核狀態變更後會寄送通知信，確定要儲存嗎？');
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    系統帳號管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    系統帳號管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:MultiView ID="MultiView_main" runat="server">
        <asp:View ID="View_list" runat="server">
            <asp:Panel ID="Panel_permissionMessage" runat="server" CssClass="alert alert-warning" Visible="false">
                您目前沒有帳號管理權限，請聯絡系統管理者協助。
            </asp:Panel>

            <div class="queryBox">
                <div class="queryBox-header">
                    查詢條件
                </div>
                <div class="queryBox-body">
                    <div class="row mb-3">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="DropDownList_accountType" Text="帳號類型" />
                            <asp:DropDownList ID="DropDownList_accountType" runat="server" CssClass="form-select">
                                <asp:ListItem Text="全部" Value="" />
                                <asp:ListItem Text="一般帳號" Value="default" />
                                <asp:ListItem Text="SSO帳號" Value="sso" />
                            </asp:DropDownList>
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="DropDownList_status" Text="帳號狀態" />
                            <asp:DropDownList ID="DropDownList_status" runat="server" CssClass="form-select">
                                <asp:ListItem Text="全部" Value="" />
                                <asp:ListItem Text="啟用" Value="1" />
                                <asp:ListItem Text="停用" Value="0" />
                            </asp:DropDownList>
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="DropDownList_verifyStatus" Text="審核狀態" />
                            <asp:DropDownList ID="DropDownList_verifyStatus" runat="server" CssClass="form-select">
                                <asp:ListItem Text="全部" Value="" />
                                <asp:ListItem Text="待審" Value="pending" />
                                <asp:ListItem Text="通過" Value="1" />
                                <asp:ListItem Text="駁回" Value="0" />
                            </asp:DropDownList>
                        </div>
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="DropDownList_role" Text="系統權限別" />
                            <asp:DropDownList ID="DropDownList_role" runat="server" CssClass="form-select">
                                <asp:ListItem Text="全部" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col">
                            <asp:Label runat="server" AssociatedControlID="TextBox_keyword" Text="關鍵字" />
                            <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="帳號、姓名、單位" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col text-center">
                            <asp:LinkButton ID="LinkButton_search" runat="server" CssClass="btn btn_main" OnClick="LinkButton_search_Click">
                                <i class="fas fa-search me-1"></i>查詢
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
                <div class="queryBox-footer"></div>
            </div>
            <div class="row d-flex align-items-center mt-2 mb-2">
                <div class="col-12 col-md-6 text-center text-md-start">
                    <asp:LinkButton ID="LinkButton_add" runat="server" CssClass="btn btn_main" OnClick="LinkButton_add_Click">
                        <i class="fas fa-file-circle-plus me-1"></i>新增帳號
                    </asp:LinkButton>
                    <asp:LinkButton ID="LinkButton_exportExcel" runat="server" CssClass="btn btn_main_line" OnClick="LinkButton_exportExcel_Click">
                        <i class="fas fa-file-arrow-down"></i>下載列表
                    </asp:LinkButton>
                </div>
                <div class="col-12 col-md-6 text-center text-md-end">
                    共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 個帳號
                </div>
            </div>
            <div class="table-responsive gv-tb">
                <asp:GridView ID="GridView_accountList" runat="server"
                    CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
                    ShowHeaderWhenEmpty="true" OnPageIndexChanging="GridView_accountList_PageIndexChanging"
                    OnRowCommand="GridView_accountList_RowCommand" OnRowDataBound="GridView_accountList_RowDataBound"
                    DataKeyNames="accountID,accountType,verifyStatus,isActive,auTypeID">
                    <Columns>
                        <asp:BoundField DataField="account" HeaderText="帳號" />
                        <asp:TemplateField HeaderText="帳號類型">
                            <ItemTemplate>
                                <%# GetAccountTypeText(Eval("accountType")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="系統權限別">
                            <ItemTemplate>
                                <%# Eval("auTypeName") %><br />
                                <small class="text-muted"><%# Eval("unitName") %></small>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="帳號狀態">
                            <ItemTemplate>
                                <%# GetActiveStatusText(Eval("isActive")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="審核狀態">
                            <ItemTemplate>
                                <%# GetVerifyStatusText(Eval("verifyStatus")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="最後登入時間">
                            <ItemTemplate>
                                <%# FormatDateTime(Eval("lastLoginDateTime")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="動作" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton_edit" runat="server"
                                    CssClass="btn btn-sm btn-primary"
                                    Text="編輯"
                                    CommandName="EditAccount"
                                    CommandArgument='<%# Eval("accountID") %>' />
                                <asp:LinkButton ID="LinkButton_toggle" runat="server"
                                    CssClass="btn btn-sm btn-danger"
                                    Text="啟用"
                                    CommandName="ToggleActive"
                                    CommandArgument='<%# Eval("accountID") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center py-5 text-muted">
                            <p>查無符合條件的帳號。</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </asp:View>

        <asp:View ID="View_edit" runat="server">
            <div class="queryBox">
                <div class="queryBox-header">
                    帳號資料
                </div>
                <div class="queryBox-body">
                    <asp:HiddenField ID="HiddenField_accountId" runat="server" />
                    <asp:HiddenField ID="HiddenField_editMode" runat="server" />
                    <asp:HiddenField ID="HiddenField_verifyStatusOriginal" runat="server" />

                    <div class="row mb-3">
                        <div class="col">
                            <span class="contItem"><i class="fas fa-envelope me-2"></i>電子信箱<span class="start">*</span></span>
                            <asp:Label ID="Label_account" runat="server" CssClass="form-control-plaintext" />
                            <asp:TextBox ID="TextBox_account" runat="server" CssClass="form-control" placeholder="請輸入電子信箱作為登入帳號" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col">
                            <span class="contItem"><i class="fas fa-user me-2"></i>姓名<span class="start">*</span></span>
                            <asp:TextBox ID="TextBox_name" runat="server" CssClass="form-control" placeholder="請輸入姓名" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col">
                            <span class="contItem"><i class="fas fa-mobile me-2"></i>手機<span class="start">*</span></span>
                            <asp:TextBox ID="TextBox_mobile" runat="server" CssClass="form-control" placeholder="請輸入手機號碼" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col">
                            <span class="contItem"><i class="fas fa-suitcase me-2"></i>單位類型與單位名稱<span class="start">*</span></span>
                            <div class="d-flex gap-2">
                                <asp:DropDownList ID="DropDownList_unitGroup" runat="server" CssClass="form-select flex-fill" AutoPostBack="true"
                                    OnSelectedIndexChanged="DropDownList_unitGroup_SelectedIndexChanged"></asp:DropDownList>
                                <asp:DropDownList ID="DropDownList_unitName" runat="server" CssClass="form-select flex-fill"></asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col">
                            <span class="contItem"><i class="fas fa-pen me-2"></i>單位補充說明</span>
                            <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control" placeholder="請輸入補充說明" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col">
                            <span class="contItem"><i class="fas fa-clipboard-check me-2"></i>審核狀態<span class="start">*</span></span>
                            <asp:RadioButtonList ID="RadioButtonList_verifyStatus" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="審核通過" Value="1" />
                                <asp:ListItem Text="審核駁回" Value="0" />
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col text-center">
                            <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_save_Click"
                                OnClientClick="return confirmVerifyStatusChange();">
                                儲存
                            </asp:LinkButton>
                            <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-outline-secondary ms-2" OnClick="LinkButton_cancel_Click">
                                取消
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
                <div class="queryBox-footer"></div>
            </div>
        </asp:View>
    </asp:MultiView>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
