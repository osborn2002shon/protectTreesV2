<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_login.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="protectTreesV2.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    登入
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_body" runat="server">
    <div>
        <div class="form-floating">
            帳　號（電子信箱）<br />
            <asp:TextBox ID="TextBox_ac" runat="server" placeholder="輸入帳號（電子信箱）"></asp:TextBox>
        </div>
        <div class="form-floating">
            密　碼
            <i class="bi bi-eye-slash eyeBtn" id="eye"></i><br />
            <asp:TextBox ID="TextBox_pw" runat="server" placeholder="輸入密碼" TextMode="Password"></asp:TextBox>
        </div>
        <div class="form-floating">
            驗證碼<br />
            <asp:TextBox ID="TextBox_Captcha" runat="server" Width="140px" MaxLength="4" autocomplete="off" placeholder="輸入驗證碼"></asp:TextBox>
            <asp:Image ID="Image_Captcha" runat="server" ImageUrl="~/_uc/captcha.ashx" Height="30"/>
        </div>
        <asp:Label ID="Label_msg" runat="server" CssClass="text-warning"></asp:Label>
        <div class="form-floating">
            <asp:Button ID="Button_login" runat="server" Text="登入" cssClass="btn_action w-100 py-2" OnClick="Button_login_Click" /><br />
            <div class="text-end">
                <i class="bi bi-person-plus-fill"></i>
                <asp:LinkButton ID="LinkButton_regist" runat="server" Text="帳號申請" CssClass="txt_info me-3" PostBackUrl="Register"></asp:LinkButton>
                <i class="bi bi-magic"></i>
                <asp:LinkButton ID="LinkButton_fpw" runat="server" Text="忘記密碼" CssClass="txt_info me-3" PostBackUrl="ForgotPassword"></asp:LinkButton>
                <i class="bi bi-magic"></i>
                <asp:LinkButton ID="LinkButton_fac" runat="server" Text="忘記帳號" CssClass="txt_info" PostBackUrl="ForgotAccount"></asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>
