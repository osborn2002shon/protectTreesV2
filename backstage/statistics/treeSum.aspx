<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="treeSum.aspx.cs" Inherits="protectTreesV2.backstage.statistics.treeSum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <script src="../../_js/highcharts.js"></script>
    <script src="../../_js/highcharts_exporting.js"></script>
    <script src="../../_js/highcharts_export-data.js"></script>
    <style>
        /* 容器設定：限制高度並允許捲動 */
        .table-container {
            max-height: 600px; /* 超過這個高度出現卷軸 */
            overflow-y: auto;
            overflow-x: auto;
            border: 1px solid #dee2e6;
        }

        /* 表格基本樣式 */
        .tbSticky {
            width: 100%;
            border-collapse: collapse;
            white-space: nowrap; 
        }

        .tbSticky th, .tbSticky td {
            padding: 8px 12px;
            border: 1px solid #dee2e6;
            text-align: right; 
        }

        /* 第一欄 (樹種) 靠左對齊 */
        .tbSticky td:first-child {
            text-align: left;
            font-weight: bold;
            background-color: #f8f9fa;
            position: sticky;
            left: 0;
            z-index: 1; /* 確保樹種欄在水平捲動時固定 */
        }

        /* 表頭固定 (Sticky Header) */
        .tbSticky thead th {
            position: sticky;
            top: 0;
            background-color: #343a40; /* 深色表頭 */
            color: white;
            z-index: 2; /* 層級最高，蓋過第一欄 */
            text-align: center;
        }

        .tbSticky thead th:first-child {
            left: 0;
            z-index: 3;
        }

        /* 合計列樣式 (第一列資料) */
        .row-total {
            background-color: #fff3cd !important; 
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    各縣市受保護樹木樹種統計
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    各縣市受保護樹木樹種統計
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            統計查詢條件
        </div>
        <div class="queryBox-body">
            <div class="row mb-3">
                <%-- 左邊欄位：統計項目 --%>
                <div class="col text-nowrap">
                    <label class="form-label">統計項目</label>
                    <div class="d-flex align-items-center gap-3 flex-nowrap">
                        <%-- Radio: 依縣市 --%>
                        <div class="form-check m-0">
                            <asp:RadioButton ID="RadioButton_tw" runat="server" GroupName="StatItem" Text="依縣市" Checked="true" />
                        </div>

                        <%-- Radio: 依鄉鎮 --%>
                        <div class="form-check m-0">
                            <asp:RadioButton ID="RadioButton_city" runat="server" GroupName="StatItem" Text="依鄉鎮" />
                        </div>

                        <%-- 鄉鎮下拉選單 --%>
                        <asp:DropDownList ID="DropDownList_citySelect" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                </div>

                <%-- 右邊欄位：樹籍狀態 --%>
                <div class="col">
                    <label class="form-label">樹籍狀態（複選）</label>
                    <asp:CheckBoxList ID="CheckBoxList_treeStatus" runat="server" 
                        RepeatLayout="Flow" 
                        RepeatDirection="Horizontal"
                        CssClass="d-flex flex-wrap gap-3 pt-1">
                    </asp:CheckBoxList>
                </div>
            </div>

            <%-- 按鈕區塊 --%>
            <div class="row">
                <div class="col text-center">
                    <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary" OnClick="LinkButton_query_Click">查詢</asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="queryBox-footer"></div>
    </div>

    <asp:Panel ID="Panel_result" runat="server" Visible="false">
        <div class="row">
            <div class="col-12 mb-4">
                <div class="card">
                    <div class="card-body">
                        <div id="treeChart" style="width: 100%; height: 500px;"></div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-12">
                <div class="card shadow-sm">
                    <div class="card-header bg-light d-flex justify-content-between align-items-center">
                        <h5 class="mb-0"><i class="fas fa-table me-2"></i>樹種數量統計表</h5>
                        <asp:LinkButton ID="LinkButton_exportExcel" runat="server" CssClass="btn btn-outline-success btn-sm" OnClick="LinkButton_exportExcel_Click">
                            <i class="fas fa-file-excel"></i> 匯出 Excel
                        </asp:LinkButton>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-container">
                            <asp:GridView ID="GridView_SpeciesPivot" runat="server" 
                                CssClass="table table-hover tbSticky mb-0"
                                AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" OnRowDataBound="GridView_SpeciesPivot_RowDataBound"
                                OnPreRender="GridView_SpeciesPivot_PreRender">
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="card-footer text-muted text-end">
                        <small>統計查詢時間：<asp:Literal ID="Lit_QueryTime" runat="server"></asp:Literal></small>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    

    <asp:HiddenField ID="hf_ChartCategories" runat="server" />
    <asp:HiddenField ID="hf_ChartSeries" runat="server" />

    <script>
        // 定義畫圖函式
        function drawHighchart() {
            // 1. 取得資料
            var categories = JSON.parse($('#<%= hf_ChartCategories.ClientID %>').val() || "[]");
            var seriesData = JSON.parse($('#<%= hf_ChartSeries.ClientID %>').val() || "[]");

            // --- 判斷資料量，動態調整 X 軸樣式 ---
            // 如果鄉鎮數量超過 20 個 (例如高雄有 38 個)，就進入「密集模式」
            var isDense = categories.length > 23;

            // 設定字體大小：密集時 9px，一般時 12px
            var fontSize = isDense ? '10px' : '12px';

            // 設定旋轉角度：密集時 -90度(垂直)，一般時 -45度(斜向)
            var rotation = isDense ? -90 : -45;
            // ------------------------------------

            // 2. 初始化 Highcharts
            Highcharts.chart('treeChart', {
                credits: { enabled: false },

                // 匯出設定 (只保留 PNG)
                exporting: {
                    enabled: true,
                    buttons: {
                        contextButton: {
                            // 只列出 PNG 選項
                            menuItems: ['downloadPNG']
                        }
                    },
                    chartOptions: {
                        chart: {
                            backgroundColor: '#FFFFFF'
                        }
                    }
                },

                chart: {
                    type: 'column',
                    backgroundColor: null,
                    // 如果標籤轉 90 度，可能需要增加底部邊距，以免字被切掉
                    marginBottom: isDense ? 100 : undefined
                },
                title: { text: '各縣市受保護樹籍狀態百分比' },

                // : X 軸動態樣式
                xAxis: {
                    categories: categories,
                    labels: {
                        rotation: rotation, // 動態角度
                        style: {
                            fontSize: fontSize,       // 動態字體大小
                            fontFamily: 'Microsoft JhengHei' // 確保中文字體
                        },
                        step: 1 //強制顯示每一個標籤 (避免 Highcharts 自動隱藏間隔的標籤)
                    }
                },

                yAxis: {
                    min: 0,
                    max: 100,
                    title: { text: '百分比 (%)' },
                    labels: { format: '{value}%' }
                },
                tooltip: {
                    shared: true,
                    pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y} 棵</b> ({point.percentage:.1f}%)<br/>'
                },
                plotOptions: {
                    column: {
                        stacking: 'percent',
                        dataLabels: {
                            enabled: true,
                            formatter: function () {
                                if (this.percentage > 0) {
                                    return this.percentage.toFixed(1) + '%';
                                }
                                // 如果是 0%，回傳 null，Highcharts 就不會畫出這個標籤
                                return null;
                            },
                            style: {
                                fontSize: fontSize 
                            }
                        }
                    }
                },
                series: seriesData
            });
        }

        //// jQuery Document Ready (頁面第一次載入時執行)
        //$(function () {
        //    drawHighchart();
        //});

        //// 處理 UpdatePanel Partial Postback (確保按了查詢按鈕後圖表不會消失)
        //var prm = Sys.WebForms.PageRequestManager.getInstance();
        //prm.add_endRequest(function () {
        //    drawHighchart();
        //});
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
