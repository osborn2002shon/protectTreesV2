<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_public.Master" AutoEventWireup="true" CodeBehind="about.aspx.cs" Inherits="protectTreesV2.pages.about" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-house"></i> > 說明介紹
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="tabBox">
        <div class="tab active"><span>保護緣起</span></div>
        <div class="tab"><span>認定標準</span></div>
    </div>
    <div class="contentBox">
		<div class="content active">
            <p>隨著城市發展與土地開發加速，許多位於非森林區域的老樹與重要樹木，長期面臨不當修剪、砍伐與生育環境破壞的風險。</p>
            <p>這些樹木不只是綠意，更承載著地方的歷史記憶、生態價值與居民情感。</p>
            <p>為了守護這些珍貴的自然與文化資產，政府修正相關法令，建立「受保護樹木」制度，透過普查、認定與公告，讓具有生態、景觀、文化或歷史意義的樹木能被看見、被珍惜，也被妥善照顧，讓樹木與土地的故事能持續在生活中延續。</p>
            <div>
                <a href="pages/#.aspx" class="btn_link" data-bs-toggle="modal" data-bs-target="#exampleModal">點我看更詳細的介紹</a>
            </div>
		</div>
		<div class="content">
            <p>
                森林以外之群生竹木、行道樹或單株樹木具有下列各款情形之一，經直轄市、縣（市）主管機關認定並公告者，為受保護樹木：
            </p>
            <ul class="aboutList mb-0">
                <li>樹齡達一百年以上。</li>
                <li>離地一點三公尺處（以下簡稱胸高），闊葉樹之樹幹胸高直徑達一點五公尺以上或胸高樹圍達四點七公尺以上；<br />
                    針葉樹之樹幹胸高直徑達零點七五公尺以上或胸高樹圍達二點四公尺以上。</li>
                <li>樹冠投影面積達四百平方公尺以上。</li>
                <li>樹木生育地，形成具生物多樣性豐富之生態環境。</li>
                <li>為區域具地理上代表性樹木。</li>
                <li>具重大美學欣賞價值之景觀。</li>
                <li>與當地居民生活、情感、祭祀、民俗或信仰具有重大連結性。</li>
                <li>與重大歷史事件具有關聯性。</li>
                <li>具有人文、科學研究及自然教育價值。</li>
                <li>當地居民之共同記憶場域。</li>
                <li>具有其他重要意義。</li>
            </ul>
		</div>
    </div>
    <div class="modal fade" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">詳細介紹</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p><strong>近</strong>年經濟發展快速，土地開法，非森林區域之樹木，常招致不當修剪、砍乏、生育地破壞、移植的案件，樹保團體抗議事件層出不窮。爰，104年7月1日總統華總一義字第10400075321號令公布修正森林法，增訂有關<span>森林以外樹木</span>保護法令，其中森林法第38條之2第1項規定，<span>地方主管機關應對轄區內樹木進行普查，具有生態、生物、地理、景觀、文化、歷史、教育、研究、社區及其他重要意義之群生竹木、行道樹或單株樹木，經地方主管機關認定為受保護樹木，應予造冊並公告之</span>。並於同條第3項規定，普查方法及受保護樹木之認定標準，由中央主管機關定之。農委會已訂定「森林以外之樹木普查方法及受保護樹木認定標準」，全文計11條，並於105年5月27日發布施行，未來經地方主管政府公告之受保護樹木，應優先加強保護，維持樹冠之自然生長及樹木品質，定期健檢養護並保護樹木生長環境。</p>
                    <p><strong>本</strong>標準重點為訂定受保護樹木之認定條件標準及程序，其中屬客觀標準之生物意義者為樹齡100年以上、闊葉樹之樹幹胸高（離地130公分）直徑達1.5公尺以上或胸高樹圍達4.7公尺以上、針葉樹之樹幹胸高直徑達0.75公尺以上或胸高樹圍達2.4公尺以上、樹冠投影面積達400平方公尺以上。針對具生態、生物、地理、景觀、文化、歷史、教育、研究、社區等重要意義之樹木，屬於不確定法律概念，則參考實務執行情形，歸納出各認定意義之標準，如<span>南投縣集集鎮綠色隧道，屬「具重大美學欣賞價值之景觀」者；屏東縣墾丁國家森林遊樂區內之銀葉板根、高雄縣旗津區之海茄苳屬「為區域具地理上代表性樹木」者；臺東縣延平鄉老茄苳為卑南族祭儀之守靈者、臺中市大里區樹王里之樹王公，屬「與當地居民生活、情感、祭祀、民俗或信仰具有重大連結性」者；而高雄市六龜區之克蘭樹，為十分罕見之老樹種類，且為臺灣原生種之民俗植物，排灣族常以其製造繩索、刀鞘，亦可製造農漁具，則屬「具有人文、科學研究及自然教育價值」者</span>。必要時直轄市、縣（市）主管機關可依本標準規定組成審議會審查認定之。</p>
                    <p><strong>直</strong>轄市、縣(市)主管機關每五年應至少就轄區內森林以外之群生竹木、行道樹或單株樹木辦理普查一次。為有公眾參與機會，受保護樹木除由直轄市、縣（市）主管機關主動調查及提報審查，<span>任何人得提供相關佐證資料向樹木所在地之直轄市、縣（市）主管機關提報，進行受保護樹木之認定程序</span>，以免有遺珠之憾。</p>
                    <p><strong>經</strong>公告之受保護樹木，地方主管機關依據森林法第38條之2及第38條之3規定，應優先加強保護，維持樹冠之自然生長及樹木品質，定期健檢養護並保護樹木生長環境，於機關專屬網頁定期公布其現況；此外，土地開發利用範圍內，有經公告之受保護樹木，應以原地保留為原則；非經地方主管機關許可，不得任意砍伐、移植、修剪或以其他方式破壞，並應維護其良好生長環境。違者可依同法第56條規定，處新臺幣12萬元以上60萬元以下罰鍰。</p>
                    <p class="mb-0 text-end">
                        <a href="https://www.moa.gov.tw/ws.php?id=2505146" target="_blank">農政與農情/105年7月(第289期)/森林以外之樹木普查方法及受保護樹木認定標準簡介</a>
                    </p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    <script src="../_js/tab.js"></script>
</asp:Content>
