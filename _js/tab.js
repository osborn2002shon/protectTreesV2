// 取得所有 tab 和 content 元素
const tabs = document.querySelectorAll('.tab');
const contents = document.querySelectorAll('.content');

// 為每個 tab 添加點擊事件
tabs.forEach((tab, index) => {
    tab.addEventListener('click', function () {
        // 移除所有 active class
        tabs.forEach(t => t.classList.remove('active'));
        contents.forEach(c => c.classList.remove('active'));

        // 添加 active class 到被點擊的 tab 和對應的 content
        this.classList.add('active');
        contents[index].classList.add('active');
    });
});

// 鍵盤導航支援（可選）
document.addEventListener('keydown', function (e) {
    const activeTab = document.querySelector('.tab.active');
    const activeIndex = Array.from(tabs).indexOf(activeTab);

    if (e.key === 'ArrowRight' && activeIndex < tabs.length - 1) {
        tabs[activeIndex + 1].click();
    } else if (e.key === 'ArrowLeft' && activeIndex > 0) {
        tabs[activeIndex - 1].click();
    }
});

// 淡入淡出效果
document.querySelectorAll('.tab').forEach(tab => {
    tab.addEventListener('click', () => {
        // 先清掉舊動畫
        tab.classList.remove('flash', 'fadeout');
        void tab.offsetWidth; // 強制重新觸發動畫

        // 展開（width 0 -> 100%, opacity 1）
        tab.classList.add('flash');

        // 等展開完成後才讓它淡出
        setTimeout(() => {
            tab.classList.add('fadeout');
        }, 300); // 跟 width 的 transition 一樣

        // 最後把 class 清掉，回到初始狀態
        setTimeout(() => {
            tab.classList.remove('flash', 'fadeout');
        }, 900); // 0.3 展開 + 0.6 淡出
    });
});

/*================================= TAB 2 ===========================================*/

// 取得所有 tab 和 content 元素
const tabs2 = document.querySelectorAll('.tabHItem');
const contents2 = document.querySelectorAll('.tabHContent');

// 為每個 tab 添加點擊事件
tabs2.forEach((tab, index) => {
    tab.addEventListener('click', function () {
        // 移除所有 active class
        tabs2.forEach(t => t.classList.remove('active'));
        contents2.forEach(c => c.classList.remove('active'));

        // 添加 active class 到被點擊的 tab 和對應的 content
        this.classList.add('active');
        contents2[index].classList.add('active');
    });
});

// 鍵盤導航支援（可選）
document.addEventListener('keydown', function (e) {
    const activeTab = document.querySelector('.tabHItem.active');
    const activeIndex = Array.from(tabs).indexOf(activeTab);

    if (e.key === 'ArrowRight' && activeIndex < tabs.length - 1) {
        tabs[activeIndex + 1].click();
    } else if (e.key === 'ArrowLeft' && activeIndex > 0) {
        tabs[activeIndex - 1].click();
    }
});

// 淡入淡出效果
document.querySelectorAll('.tabHItem').forEach(tab => {
    tab.addEventListener('click', () => {
        // 先清掉舊動畫
        tab.classList.remove('flash', 'fadeout');
        void tab.offsetWidth; // 強制重新觸發動畫

        // 展開（width 0 -> 100%, opacity 1）
        tab.classList.add('flash');

        // 等展開完成後才讓它淡出
        setTimeout(() => {
            tab.classList.add('fadeout');
        }, 300); // 跟 width 的 transition 一樣

        // 最後把 class 清掉，回到初始狀態
        setTimeout(() => {
            tab.classList.remove('flash', 'fadeout');
        }, 900); // 0.3 展開 + 0.6 淡出
    });
});