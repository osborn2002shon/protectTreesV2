<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_login.Master" AutoEventWireup="true" CodeBehind="reg.aspx.cs" Inherits="protectTreesV2.reg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <script src="_js/jquery.mask.js"></script>
    <style>
        .info {
            font-size:0.85rem;
            color:#7a9578;
            text-align:left;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%= TextBox_mobile.ClientID %>').mask("0000-000-000", { placeholder: "0000-000-000" });
        });        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    帳號申請
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_body" runat="server">
    <div>
        <div class="mb-3">
            <span class="contItem"><i class="fas fa-envelope me-2"></i>電子信箱<span class="start">*</span></span>
            <asp:TextBox ID="TextBox_email" runat="server" CssClass="form-control" placeholder="請輸入電子信箱作為您的登入帳號" autocomplete="off"></asp:TextBox>
        </div>
        <div class="mb-3">
            <span class="contItem"><i class="fas fa-user me-2"></i>姓名<span class="start">*</span></span>
            <asp:TextBox ID="TextBox_name" runat="server" CssClass="form-control" placeholder="請輸入您的真實姓名供承辦人員進行確認" autocomplete="off"></asp:TextBox>
        </div>
        <div class="mb-3">
            <span class="contItem"><i class="fas fa-mobile me-2"></i>手機<span class="start">*</span></span>
            <asp:TextBox ID="TextBox_mobile" runat="server" CssClass="form-control" placeholder="請輸入您的手機號碼供承辦人員確認聯繫" autocomplete="off"></asp:TextBox>
        </div>
        
        <div class="mb-3">            
            <span class="contItem"><i class="fas fa-suitcase me-2"></i>單位類型與單位名稱<span class="start">*</span></span>
            <div class="d-flex gap-2">
                <asp:DropDownList ID="DropDownList_unitGroup" runat="server" CssClass="form-select flex-fill" AutoPostBack="true"
                    OnSelectedIndexChanged="DropDownList_unitGroup_SelectedIndexChanged"></asp:DropDownList>
                <asp:DropDownList ID="DropDownList_unitName" runat="server" CssClass="form-select flex-fill"></asp:DropDownList>     
            </div>
            <div class="info">若您為樹木權管、養護單位或廠商，請選擇您所屬的縣市單位</div>
        </div>        
        <div class="mb-3">
            <span class="contItem"><i class="fas fa-pen me-2"></i>單位補充說明</span>
            <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control" placeholder="請輸入欲補充說明之任職單位" autocomplete="off"></asp:TextBox>
            <div class="info">若您為樹木權管、養護單位或廠商，建議補充說明</div>
        </div>
        <div class="form-floating">
            <asp:Button ID="Button_submitCheck" runat="server" Text="送出申請" CssClass="btn-def btn-login mb-2" OnClick="Button_submitCheck_Click" />
            <a href="login.aspx" class="btn-def btn-cancel" title="返回登入">返回登入</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_modal_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_modal_body" runat="server">
    <asp:MultiView ID="MultiView_main" runat="server">
        <asp:View ID="View_info" runat="server">
            <ul class="ul">
                <li>點選下方【驗證電子信箱】後，請在30分鐘內依照信上指示完成帳號申請。</li>
                <li>帳號申請送出後須等待所屬機關承辦人審核，若有疑問可洽系統管理者確認。</li>
                <li>審核結果會以電子信件方式通知，審核通過後請透過信上密碼進行登入，並於初次登入時變更密碼。</li>
            </ul>
            <div class="text-center">
                <asp:Button ID="Button_submit" runat="server" Text="驗證電子信箱" CssClass="btn-login" OnClick="Button_submit_Click" />
            </div>
        </asp:View>
        <asp:View ID="View_msg" runat="server">
            <asp:Label ID="Label_msg" runat="server" ForeColor="Tomato"></asp:Label>
        </asp:View>
    </asp:MultiView>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_modal_footer" runat="server">    
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_jsBottom" runat="server">
</asp:Content>
