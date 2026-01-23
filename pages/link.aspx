<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_public.Master" AutoEventWireup="true" CodeBehind="link.aspx.cs" Inherits="protectTreesV2.pages.link" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-house"></i> > 其他資源
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="tabBox">
        <div class="tab active"><span>外部連結</span></div>
        <div class="tab"><span>檔案下載</span></div>
    </div>
    <div class="contentBox">
		<div class="content active">
            <div class="linkBox">
                <a class="link" target="_blank" href='https://law.moj.gov.tw/LawClass/LawAll.aspx?pcode=M0040047'>
                    <span><i class="fa-solid fa-hashtag me-1"></i>全國法規資料庫</span><br />森林以外之樹木普查方法及受保護樹木認定標準
                </a>
                <a class="link" target="_blank" href='https://tree.tfri.gov.tw/'>
                    <span><i class="fa-solid fa-hashtag me-1"></i>農業部林業試驗所</span><br />全國種樹諮詢中心</a>
                <a class="link" target="_blank" href='https://oldtree.life/'>
                    <span><i class="fa-solid fa-hashtag me-1"></i>老樹報報</span><br />全國城鄉老樹資源地圖</a>
            </div>
		</div>
		<div class="content">
            <div class="linkBox">
                <a class="link" target="_blank" href='1.pdf'><i class="fa-solid fa-download me-1"></i>非中央受保護樹木資源專區.pdf</a>
            </div>            
		</div>
    </div>
    <script src="../_js/tab.js"></script>
</asp:Content>
