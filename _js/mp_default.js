$(document).ready(function () {
    // 行動版選單切換
    $(document).on('click', '#mobileMenuBtn', function () {
        const sidebar = $('#sidebar');
        const overlay = $('#sidebarOverlay');

        if (window.innerWidth <= 768) {
            sidebar.toggleClass('show');
            overlay.toggleClass('show');
        }
    });
    // 手機版關閉按鈕
    $(document).on('click', '#mobileCloseBtn', function () {
        closeMobileMenu();
    });

    // 點擊遮罩關閉選單
    $(document).on('click', '#sidebarOverlay', function () {
        closeMobileMenu();
    });

    // 點擊選單項目後關閉選單（手機版）
    $(document).on('click', '.nav-submenu .nav-link', function () {
        if (window.innerWidth <= 768) {
            setTimeout(closeMobileMenu, 300);
        }
    });

    // 阻止側邊欄內容點擊時關閉選單
    //$('#sidebar').on('click', function (e) {
    //    e.stopPropagation();
    //});

    // 側邊欄內容點擊不關閉
    $(document).on('click', '#sidebar', function (e) {
        e.stopPropagation();
    });


    // 視窗大小改變處理
    $(window).on('resize', function () {
        if (window.innerWidth > 768) {
            closeMobileMenu();
        }
    });

    // ESC鍵關閉選單
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && window.innerWidth <= 768) {
            closeMobileMenu();
        }
    });

    // 關閉手機版選單的統一函數
    function closeMobileMenu() {
        $('#sidebar').removeClass('show');
        $('#sidebarOverlay').removeClass('show');
    }

    // 側邊欄選單項目點擊處理
    $('.nav-link').on('click', function (e) {
        if ($(this).hasClass('nav-link') && !$(this).attr('data-bs-toggle')) {
            $('.nav-submenu .nav-link').removeClass('active');
            $(this).addClass('active');
        }
    });
});

// 燈箱
function showModal() {
    var md = new bootstrap.Modal(document.getElementById('divMSG'));
    md.show();
}

// 清除控制項
function clearControl(controlId) {
    var textbox = document.getElementById(controlId);
    textbox.value = '';
    textbox.focus(); // 清除後自動聚焦
}

// 置頂
const backToTopBtn = document.getElementById('backToTop');
window.addEventListener('scroll', function () {
    //當滾動超過 300px 時顯示按鈕
    if (window.scrollY > 300) {
        backToTopBtn.classList.add('show');
    } else {
        backToTopBtn.classList.remove('show');
    }
});
backToTopBtn.addEventListener('click', function () {
    //點擊按鈕回到頂部
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
});

