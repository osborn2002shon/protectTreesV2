<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_login.Master" AutoEventWireup="true" CodeBehind="regVerify.aspx.cs" Inherits="protectTreesV2.regVerify" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    信箱驗證結果
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_body" runat="server">
    <asp:MultiView ID="MultiView_main" runat="server">
        <asp:View ID="View_def" runat="server">
            <div class="text-center">
                <div class="text-success" style="font-size:2rem;"><i class="fa-solid fa-check"></i></div>
                您的電子信箱<br />
                <asp:Label ID="Label_email" runat="server"  CssClass="start"></asp:Label><br />
                驗證結果為<span class="start">通過</span><br />
                待管理者審核通過即可使用本系統<br />
                <a href="login.aspx" class="btn-cancel mt-2" title="返回登入">返回登入</a>
            </div>
        </asp:View>
        <asp:View ID="View_err" runat="server">
            <div class="text-center">
                <div class="text-danger" style="font-size:2rem;"><i class="fa-solid fa-xmark"></i></div>
                <asp:Label ID="Label_errMsg" runat="server"></asp:Label>
                <br />
                <a href="reg.aspx" class="forgot-password me-3"><i class="fa-solid fa-registered me-1"></i>帳號申請</a>
            </div>
        </asp:View>
    </asp:MultiView>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_modal_title" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_modal_body" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_modal_footer" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_jsBottom" runat="server">
</asp:Content>
