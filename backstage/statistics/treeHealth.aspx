<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="treeHealth.aspx.cs" Inherits="protectTreesV2.backstage.statistics.treeHealth" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
     <script src="../../_js/highcharts.js"></script>
     <script src="../../_js/highcharts_exporting.js"></script>
     <script src="../../_js/highcharts_export-data.js"></script>
     <style>
         /* 容器設定：限制高度並允許捲動 */
         .table-container {
             max-height: 600px;
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
    各縣市健康檢查建議統計
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    各縣市健康檢查建議統計
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            統計查詢條件
        </div>
        <div class="queryBox-body">
            <div class="row g-2">
        
                <%-- 左邊區塊：起始年月 --%>
                <div class="col">
                    <label class="form-label fw-bold">起始年月</label>
                    <div class="input-group">
                        <asp:DropDownList ID="DropDownList_startYear" runat="server" CssClass="form-select"></asp:DropDownList>
                        <asp:DropDownList ID="DropDownList_startMonth" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                </div>

                <%-- 中間區塊：波浪號 --%>
                <div class="col-auto">
                    <label class="form-label d-block">&nbsp;</label> 
                    <div class="form-control-plaintext text-center fw-bold text-secondary px-0">
                        ~
                    </div>
                </div>

                <%-- 右邊區塊：結束年月 --%>
                <div class="col">
                    <label class="form-label fw-bold">結束年月</label>
                    <div class="input-group">
                        <asp:DropDownList ID="DropDownList_endYear" runat="server" CssClass="form-select"></asp:DropDownList>
                        <asp:DropDownList ID="DropDownList_endMonth" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
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
                        <h5 class="mb-0"><i class="fas fa-table me-2"></i>各縣市健康檢查建議統計表</h5>
                        <asp:LinkButton ID="LinkButton_exportExcel" runat="server" CssClass="btn btn-outline-success btn-sm" OnClick="LinkButton_exportExcel_Click">
                            <i class="fas fa-file-excel"></i> 匯出 Excel
                        </asp:LinkButton>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-container">
                            <asp:GridView ID="GridView_healthRecStats" runat="server" 
                                CssClass="table table-hover tbSticky mb-0"
                                AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" OnRowDataBound="GridView_healthRecStats_RowDataBound"
                                OnPreRender="GridView_healthRecStats_PreRender">
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="card-footer text-muted text-end">
                        <small>統計查詢時間：<asp:Literal ID="Lit_queryTime" runat="server"></asp:Literal></small>
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
                title: { text: '各縣市健康檢查建議統計百分比' },

                // : X 軸動態樣式
                xAxis: {
                    categories: categories,
                    labels: {
                        rotation: rotation, // 動態角度
                        style: {
                            fontSize: fontSize,       // 動態字體大小
                            fontFamily: 'Microsoft JhengHei' // 確保中文字體
                        },
                        step: 1 //強制顯示每一個標籤
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
                    pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y} 株</b> ({point.percentage:.1f}%)<br/>'
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
                                // 如果是 0%，回傳 null
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
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
