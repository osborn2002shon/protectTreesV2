<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="protectTreesV2.backstage.system.profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style>
       
        .password-toggle {
            position: absolute;
            right: 15px;        
            top: 50%;            
            transform: translateY(-50%);
            background: none;
            border: none;
            color: #6c757d;
            font-size: 1rem;
            padding: 5px;
            cursor: pointer;
            transition: color 0.3s ease;
            z-index: 10;
        }

          .password-toggle:hover {
              color: #006948;
          }

          .password-toggle:focus {
              outline: none;
              color: #006948;
          }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    我的帳號管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    我的帳號管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <%-- 1. 基本資料區塊 (全唯讀，無 Footer) --%>
    <div class="card formCard mb-4">
        <div class="card-header">
            基本資料
        </div>
        <div class="card-body">
            <div class="row mb-3">
                <%-- 第一列：帳號 / 姓名 --%>
                <div class="col-md-6">
                    <label class="form-label">電子信箱 (登入帳號)</label>
                    <asp:Label ID="Label_account" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
                <div class="col-md-6">
                    <label class="form-label">姓名</label>
                    <asp:Label ID="Label_name" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
            </div>

            <div class="row mb-3">
                <%-- 第二列：所屬單位 / 系統權限 --%>
                <div class="col-md-6">
                    <label class="form-label">所屬單位</label>
                    <asp:Label ID="Label_unit" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
                <div class="col-md-6">
                    <label class="form-label">系統權限</label>
                    <asp:Label ID="Label_role" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
            </div>

            <div class="row">
                <%-- 第三列：最後登入時間 --%>
                <div class="col-md-6">
                    <label class="form-label">最後登入時間</label>
                    <asp:Label ID="Label_lastLogin" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
            </div>
        </div>
    </div>

    <%-- 2. 密碼變更區塊  --%>
    <asp:Panel ID="Panel_passwordChange" runat="server" CssClass="card formCard">
        <div class="card-header">
            密碼變更
        </div>
        <div class="card-body">
            <div class="row mb-3">
                <div class="col-12">
                    <label class="form-label">密碼最後變更時間</label>
                    <asp:Label ID="Label_passwordChangedDate" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6 mb-3">
                    <label class="form-label">輸入新密碼 <span class="text-danger">*</span></label>
                    <div class="position-relative"> <asp:TextBox ID="TextBox_newPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="請輸入新密碼"></asp:TextBox>
                        <button type="button" class="password-toggle" id="toggleNewPwd">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                </div>

                <div class="col-md-6 mb-3">
                    <label class="form-label">確認新密碼 <span class="text-danger">*</span></label>
                    <div class="position-relative"> <asp:TextBox ID="TextBox_confirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="請再次輸入密碼"></asp:TextBox>
                        <button type="button" class="password-toggle" id="toggleConfirmPwd">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer text-center">
            <asp:LinkButton ID="LinkButton_changePassword" runat="server" CssClass="btn btn-warning" OnClick="LinkButton_changePassword_Click">
                <i class="fas fa-save me-1"></i>變更密碼
            </asp:LinkButton>
        </div>
    </asp:Panel>
    <script>
        $(document).ready(function () {
            // 綁定 "新密碼" 的眼睛按鈕
            $('#toggleNewPwd').on('click', function () {
                togglePasswordVisibility($(this), $('#<%= TextBox_newPassword.ClientID %>'));
            });

            // 綁定 "確認密碼" 的眼睛按鈕
            $('#toggleConfirmPwd').on('click', function () {
                togglePasswordVisibility($(this), $('#<%= TextBox_confirmPassword.ClientID %>'));
            });

            // 共用的切換顯示函式
            function togglePasswordVisibility(btn, inputField) {
                const icon = btn.find('i');

                if (inputField.attr('type') === 'password') {
                    inputField.attr('type', 'text');
                    icon.removeClass('fa-eye').addClass('fa-eye-slash');
                } else {
                    inputField.attr('type', 'password');
                    icon.removeClass('fa-eye-slash').addClass('fa-eye');
                }
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
