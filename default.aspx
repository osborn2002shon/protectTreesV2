<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="protectTreesV2._default" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>受保護樹木資訊網</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Mono:wght@100..900&family=Noto+Sans+TC:wght@100..900&display=swap" rel="stylesheet" />
    <script src="_js/bootstrap.bundle.js"></script>
    <script src="_js/jquery-3.7.1.js"></script>
    <link href="_css/bootstrap.css" rel="stylesheet" />
    <link href="_css/fontawesome-all.css" rel="stylesheet" />
    <link href="_css/mp_public_menu.css" rel="stylesheet" />
    <link href="_css/mp_public_main.css" rel="stylesheet" />
    <link href="_css/mp_public_control.css" rel="stylesheet" />
    <link href="_css/mp_public_default.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">        
        <asp:ScriptManager ID="ScriptManager_main" runat="server"></asp:ScriptManager>
        <header class="container-fluid p-0 m-0">
            <nav class="div_header navbar navbar-expand-lg">
                <div class="container-fluid">
                    <!-- 網站標題 -->
                    <div class="d-none d-md-block">
                        <a class="navbar-brand" href="default.aspx"><img src="_img/logo.png"/>受保護樹木資訊網</a>
                    </div>
                    <div class="d-block d-md-none">
                        <a class="navbar-brand" href="default.aspx"><img src="_img/logo_mini.png"/>受保護樹木資訊網</a>
                    </div>
                    
                    <!-- 手機版漢堡按鈕 -->
                    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" 
                            aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                        <span class="navbar-toggler-icon"></span>
                    </button>            
                    <!-- 選單項目 -->
                    <div class="collapse navbar-collapse" id="navbarNav">
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item">
                                <a class="nav-link highlight" href="pages/about.aspx">說明介紹</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link highlight" href="pages/analysis.aspx">保護成果</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link highlight" href="pages/map.aspx">樹木地圖</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link highlight" href="pages/link.aspx">其他資源</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
        </header>    
        <main class="container-fluid p-0 m-0">
            <div class="div_banner">
                <div class="div_banner_title d-none d-md-block">
                    <h1>讓珍貴的樹木，持續成為城市的風景</h1>
                    <span style="font-size:1.5rem">一起守護街角的綠蔭，讓下一代抬頭就能看見</span>
                </div>
                <div class="div_banner_title_s d-block d-md-none">
                    <h1>讓珍貴的樹木<br />持續成為城市的風景</h1>
                    <span style="font-size:1.25rem">一起守護街角的綠蔭<br />讓下一代抬頭就能看見</span>
                </div>
            </div>
            <div class="div_item p-5">
                <div class="container itemBox">
                    <div class="item">
                        <img src="_img/icon/faq.png" />
                        <div>保護緣起、認定標準<br />說明介紹</div>
                        <a href="pages/about.aspx" title="開始了解" class="btn_link">前往了解</a>
                    </div>
                    <div class="item">
                        <img src="_img/icon/analysis.png" />
                        <div>受保護樹木<br />統計與概況</div>
                        <a href="pages/analysis.aspx" title="查看統計資訊" class="btn_link">查看資訊</a>
                    </div>
                    <div class="item">
                        <img src="_img/icon/street-map.png" />
                        <div>查詢受保護樹木<br />位置與基本資料</div>
                        <a href="pages/map.aspx" title="開啟地圖" class="btn_link">開啟地圖</a>
                    </div>
                    <div class="item">
                        <img src="_img/icon/search.png" />
                        <div>法規文件下載<br />與相關資源查詢</div>
                        <a href="pages/link.aspx" title="查看資源" class="btn_link">前往運用</a>
                    </div>
                </div>
            </div>
            <div class="div_faq p-5">
                <div class="blockTitle">
                    <span>常見問答</span>
                </div>
                <div class="container">
                    <div class="accordion" id="accordionExample">
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapse1" aria-expanded="true" aria-controls="collapseOne">
                                    Ｑ１：這些受保護樹木和縣市政府自治條例的受保護樹木有什麼不一樣？
                                </button>
                            </h2>
                            <div id="collapse1" class="accordion-collapse collapse show" data-bs-parent="#accordionExample">
                                <div class="accordion-body">
                                    兩者確實不一樣唷！...
                                </div>
                            </div>
                        </div>
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse2" aria-expanded="false" aria-controls="collapseTwo">
                                    Ｑ２：我家附近有樹木快倒了感覺好危險，我應該要和誰聯繫？
                                </button>
                            </h2>
                            <div id="collapse2" class="accordion-collapse collapse" data-bs-parent="#accordionExample">
                                <div class="accordion-body">
                                    需要先判斷該樹是否為私人持有，可聯絡OOO....
                                </div>
                            </div>
                        </div>
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse3" aria-expanded="false" aria-controls="collapseThree">
                                    Ｑ３：我要怎麼知道我家旁邊這棵樹是否為受保護樹木？
                                </button>
                            </h2>
                            <div id="collapse3" class="accordion-collapse collapse" data-bs-parent="#accordionExample">
                                <div class="accordion-body">
                                    確認是否有掛牌、和機關連繫諮詢、訪問里長...
                                </div>
                            </div>
                        </div>
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse4" aria-expanded="false" aria-controls="collapseThree">
                                    Ｑ４：我知道有一顆樹已存活超過百年，並且有豐富的故事，我要怎麼替它申請成為受保護樹木？
                                </button>
                            </h2>
                            <div id="collapse4" class="accordion-collapse collapse" data-bs-parent="#accordionExample">
                                <div class="accordion-body">
                                    ...
                                </div>
                            </div>
                        </div>
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse5" aria-expanded="false" aria-controls="collapseThree">
                                    Ｑ５：我家有一顆受保護樹木因為土地要賣了須要砍掉，我可以砍掉嗎？我要聯繫什麼單位？
                                </button>
                            </h2>
                            <div id="collapse5" class="accordion-collapse collapse" data-bs-parent="#accordionExample">
                                <div class="accordion-body">
                                    ...
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="div_unit p-5">
                <div class="blockTitle">
                    <span>聯絡窗口</span>
                </div>
                <div class="container">
                    <asp:UpdatePanel ID="UpadtePanel_main" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col">
                                    <asp:RadioButtonList ID="RadioButtonList_area" runat="server" CssClass="rbl"
                                        RepeatDirection="Horizontal" RepeatLayout="Flow"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="RadioButtonList_area_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">北部</asp:ListItem>
                                        <asp:ListItem>中部</asp:ListItem>
                                        <asp:ListItem>南部</asp:ListItem>
                                        <asp:ListItem>東部</asp:ListItem>
                                        <asp:ListItem>外島</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="row g-3 mt-1">
                                <asp:Repeater ID="Repeater_Unit" runat="server">
                                    <ItemTemplate>
                                        <div class="col-12 col-sm-6 col-md-4 col-lg-3">
                                            <div class="card h-100 card_cust">
                                                <div class="card-header">
                                                    <%# Eval("unitName") %>
                                                </div>
                                                <div class="card-body">
                                                    <%# Eval("contectName") %><br />
                                                    <%# Eval("contectTel") %><br />
                                                    <span style="color:#808080;font-size:1rem">
                                                        <%# Eval("contectAddress") %>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </main>
        <div class="div_footer_img">
            <img src="_img/icon/forest2.png"/>
        </div>
        <footer class="div_footer container-fluid">
            <div class="text-center">
                Copyright © 2025 Forestry and Nature Conservation Agency<br />
                電話：(02)2351-5441<br />
                地址：100024 臺北市中正區杭州南路一段2號<br />
            </div>
        </footer>        
    </form>
    <!-- 回到頂部按鈕 -->
    <button class="back-to-top" id="backToTop">
        <i class="fa-solid fa-arrow-up"></i>
    </button>
    <script type="text/javascript">
        //頁面載入動畫
        $(window).on('load', function() {
            $('.div_banner_title').hide().fadeIn(2000);
        });

        // 取得按鈕元素
        const backToTopBtn = document.getElementById('backToTop');

        // 監聽滾動事件
        window.addEventListener('scroll', function () {
            // 當滾動超過 300px 時顯示按鈕
            if (window.scrollY > 300) {
                backToTopBtn.classList.add('show');
            } else {
                backToTopBtn.classList.remove('show');
            }
        });

        // 點擊按鈕回到頂部
        backToTopBtn.addEventListener('click', function () {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    </script>
</body>
</html>
