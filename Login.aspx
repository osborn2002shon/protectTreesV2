<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_login.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="protectTreesV2.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    登入
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_body" runat="server">
    <div class="form-floating">
        <asp:TextBox ID="TextBox_ac" runat="server" CssClass="form-control" placeholder="請輸入帳號" TabIndex="1"></asp:TextBox>
        <asp:Label ID="Label_ac" runat="server" AssociatedControlID="TextBox_ac">
            <i class="fas fa-user me-2"></i>電子信箱
        </asp:Label>
    </div>
                    
    <div class="form-floating position-relative">
        <asp:TextBox ID="TextBox_pw" runat="server" CssClass="form-control" placeholder="請輸入密碼" TextMode="Password" TabIndex="2"></asp:TextBox>
        <asp:Label ID="Label_pw" runat="server" AssociatedControlID="TextBox_pw">
            <i class="fas fa-lock me-2"></i>密碼
        </asp:Label>
        <button type="button" class="btn password-toggle" id="passwordToggle">
            <i class="fas fa-eye"></i>
        </button>
    </div>

    <div class="form-floating position-relative">
        <asp:TextBox ID="TextBox_Captcha" runat="server" class="form-control" placeholder="請輸入驗證碼" autocomplete="off" TabIndex="3" MaxLength="4"></asp:TextBox>
        <asp:Label ID="Label_Captcha" runat="server" AssociatedControlID="TextBox_Captcha">
            <i class="fa-solid fa-hashtag me-2"></i>驗證碼
        </asp:Label>
        <asp:Image ID="Image_Captcha" runat="server" ImageUrl="~/_uc/captcha.ashx" Height="38" CssClass="captcha-image"/>                    
    </div>

    <asp:Button ID="Button_login" runat="server" CssClass="btn-login mb-3" Text="登入" OnClick="Button_login_Click" />
    <div class="text-center">
        <a href="reg.aspx" class="link me-3"><i class="fa-solid fa-registered me-1"></i>帳號申請</a>
        <a href="fpw.aspx" class="link me-3"><i class="fa-solid fa-key me-1"></i>忘記密碼</a>
        <a href="fac.aspx" class="link"><i class="fa-solid fa-question me-1"></i>忘記帳號</a>        
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_modal_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_modal_body" runat="server">
    <asp:Label ID="Label_msg" runat="server"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_modal_footer" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_jsBottom" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            //密碼顯示/隱藏切換
            $('#passwordToggle').on('click', function() {
                const passwordInput = $('#<%= TextBox_pw.ClientID%>');
                const toggleIcon = $(this).find('i');
                
                if (passwordInput.attr('type') === 'password') {
                    passwordInput.attr('type', 'text');
                    toggleIcon.removeClass('fa-eye').addClass('fa-eye-slash');
                } else {
                    passwordInput.attr('type', 'password');
                    toggleIcon.removeClass('fa-eye-slash').addClass('fa-eye');
                }
            });
            
            //輸入框焦點效果
            $('.form-control').on('focus', function() {
                $(this).parent().addClass('focused');
            }).on('blur', function() {
                if (!$(this).val()) {
                    $(this).parent().removeClass('focused');
                }
            });
            
            //Enter鍵快速登入
            $(document).on('keypress', function(e) {
                if (e.which === 13 && !$('#loginForm').find('button[type="submit"]').prop('disabled')) {
                    $('#loginForm').submit();
                }
            });
        });
        
        //頁面載入動畫
        //$(window).on('load', function() {
        //    $('.login-card').hide().fadeIn(800);
        //});
    </script>
</asp:Content>
