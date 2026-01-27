<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_public.Master" AutoEventWireup="true" CodeBehind="analysis.aspx.cs" Inherits="protectTreesV2.pages.analysis" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <script src="https://code.highcharts.com/highcharts.js"></script>
    <script src="https://code.highcharts.com/modules/heatmap.js"></script>
    <script src="https://code.highcharts.com/modules/treemap.js"></script>
    <script src="https://code.highcharts.com/modules/exporting.js"></script>
    <script src="https://code.highcharts.com/modules/export-data.js"></script>
    <script src="https://code.highcharts.com/modules/accessibility.js"></script>
    <script src="https://code.highcharts.com/modules/sankey.js"></script>
    <style type="text/css">
        .stats-container {
            /*max-width: 1200px;*/
            margin: 0 auto;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 50px 30px;
            padding:20px 0px;
        }

        .stat-card {
            position: relative;
            background: rgba(255, 255, 255, 0.95);
            padding: 40px 15px;
            border: 10px solid transparent;
            background: 
                linear-gradient(white, white) padding-box,
                linear-gradient(90deg,rgba(29, 105, 196, 0.2) 0%, rgba(102, 189, 189, 0.2) 100%) border-box;
            border-radius: 65% 35% 70% 30% / 40% 60% 40% 60%;
            animation: morph 8s ease-in-out infinite;
            /*box-shadow: 0 10px 30px rgba(0,0,0,0.15);*/            
            text-align:center !important;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .color2 {
            background: 
                linear-gradient(white, white) padding-box,
                linear-gradient(90deg,rgba(29, 105, 196, 0.5) 0%, rgba(102, 189, 189, 0.5) 100%) border-box;        
        }

        .stat-card:hover {
            transform: translateY(-5px);
            box-shadow: rgba(100, 100, 111, 0.2) 0px 7px 29px 0px;
        }

        .stat-card:nth-child(1) {
            animation-delay: 0s;
        }

        .stat-card:nth-child(2) {
            animation-delay: -2s;
        }

        .stat-card:nth-child(3) {
            animation-delay: -4s;
        }

        .stat-card:nth-child(4) {
            animation-delay: -6s;
        }

        .stat-card:nth-child(5) {
            animation-delay: -1s;
        }

        .stat-card:nth-child(6) {
            animation-delay: -3s;
        }

        .stat-card:nth-child(7) {
            animation-delay: -5s;
        }

        @keyframes morph {
            0%, 100% {
                border-radius: 65% 35% 70% 30% / 40% 60% 40% 60%;
            }
            25% {
                border-radius: 35% 65% 30% 70% / 60% 40% 60% 40%;
            }
            50% {
                border-radius: 70% 30% 60% 40% / 35% 65% 35% 65%;
            }
            75% {
                border-radius: 30% 70% 40% 60% / 65% 35% 65% 35%;
            }
        }

        .stat-icon {
            /*font-size: 48px;
            margin-bottom: 15px;
            display: block;*/
        }

        .stat-number {
            font-size: 3rem;
            font-weight: 800;
            color: #1D69C4;
            margin: 20px 0px;
            line-height: 1;
        }

        .stat-label {
            font-size: 1.5rem;
            color: #666;
            font-weight: 500;
        }

        .stat-description {
            font-size: 1rem;
            color: #999;
            line-height: 1.4;
        }

        .stat-unit {
            font-size:1rem;
            margin-left:3px;
        }

        @media (max-width: 768px) {
            .stats-container {
                grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                gap: 25px;
            }

            .stat-card {
                padding: 30px 20px;
            }

            .stat-number {
                font-size: 42px;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-house"></i> > 保護成果
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="stats-container">
        <div class="stat-card">
            <div class="stat-label">保護數量</div>
            <div class="stat-number"><asp:Literal ID="Literal_TotalProtected" runat="server" /><span class="stat-unit">案</span></div>
            <div class="stat-description">全台列管保護的珍貴樹木</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">樹木種類</div>
            <div class="stat-number"><asp:Literal ID="Literal_SpeciesCount" runat="server" /><span class="stat-unit">種</span></div>
            <div class="stat-description">包含台灣特有種與外來種</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">數量最多樹種</div>
            <div class="stat-number"><asp:Literal ID="Literal_MostSpeciesCount" runat="server" /><span class="stat-unit">案</span></div>
            <div class="stat-description"><asp:Literal ID="Literal_MostSpeciesName" runat="server" /></div>
        </div>
        <div class="stat-card">
            <div class="stat-label">數量最少樹種</div>
            <div class="stat-number"><asp:Literal ID="Literal_LeastSpeciesCount" runat="server" /><span class="stat-unit">案</span></div>
            <div class="stat-description"><asp:Literal ID="Literal_LeastSpeciesName" runat="server" /></div>
        </div>
        <div class="stat-card">
            <div class="stat-label">涵蓋縣市</div>
            <div class="stat-number"><asp:Literal ID="Literal_CityCount" runat="server" /><span class="stat-unit">個</span></div>
            <div class="stat-description">遍布全台</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">涵蓋鄉鎮</div>
            <div class="stat-number"><asp:Literal ID="Literal_AreaCount" runat="server" /><span class="stat-unit">個</span></div>
            <div class="stat-description">深入鄉里</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">種類最豐富</div>
            <div class="stat-number"><asp:Literal ID="Literal_MostSpeciesCityCount" runat="server" /><span class="stat-unit">種</span></div>
            <div class="stat-description"><asp:Literal ID="Literal_MostSpeciesCityName" runat="server" /></div>
        </div>
        <div class="stat-card">
            <div class="stat-label">保護數量最多</div>
            <div class="stat-number"><asp:Literal ID="Literal_MostProtectedCityCount" runat="server" /><span class="stat-unit">案</span></div>
            <div class="stat-description"><asp:Literal ID="Literal_MostProtectedCityName" runat="server" /></div>
        </div>
    </div>
    <div class="chartBox">
        <div class="blockTitle">
            <span>各縣市受保護樹木數量</span>
        </div>
        <div class="mt-5 mb-5">
            <asp:RadioButtonList ID="RadioButtonList_c1Type" runat="server" CssClass="rbl" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                <asp:ListItem>依縣市順序排</asp:ListItem>
                <asp:ListItem Selected="True">從多到少排</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="cityChart"></div>
    </div>
    <div class="chartBox">
        <div class="blockTitle">
            <span>
                指定縣市受保護樹木分析
            </span>
        </div>

                <div class="row">
                    <div class="col-12 col-md-6">
                        <asp:DropDownList ID="DropDownList_city" runat="server" CssClass="form-select m-0" AutoPostBack="true" />
                    </div>
                    <div class="col-12 col-md-6">

                    </div>
                </div>
                <div class="row">
                    <div class="col-12 col-md-6">
                        <table class="tb">
                            <tr>
                                <th>樹種</th>
                                <th>數量</th>
                                <th>分布</th>
                            </tr>
                            <asp:Repeater ID="Repeater_citySpecies" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("SpeciesName") %></td>
                                        <td><%# Eval("TreeCount") %></td>
                                        <td><%# Eval("AreaNames") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>
                    <div class="col-12 col-md-6">
                        <div id="speciesChart"></div>
                    </div>
                </div>
                <script type="text/javascript">
                    const data2 = <%= SpeciesChartDataJson %>;

                    Highcharts.chart('speciesChart', {
                        credits: { enabled: false },
                        exporting: { enabled: false },
                        chart: {
                            type: 'pie',
                            backgroundColor: 'transparent',
                            height: '100%'
                        },
                        title: {
                            text: null
                        },
                        plotOptions: {
                            pie: {
                                allowPointSelect: true,
                                cursor: 'pointer',
                                dataLabels: {
                                    enabled: true,
                                    format: '<b>{point.name}</b><br/>{point.percentage:.1f}%',
                                    distance: 20,
                                    style: {
                                        fontSize: '16px'
                                    }
                                },
                                showInLegend: true
                            }
                        },
                        series: [{
                            name: '受保護樹木數量',
                            data: data2,
                            //colors: ['rgba(102, 189, 189, 0.2)','rgba(29, 105, 196, 0.2)']
                            //colors: ['#5470C6', '#91CC75', '#FAC858', '#EE6666', '#73C0DE', '#3BA272', '#FC8452', '#9A60B4', '#EA7CCC']
                            colors: [
                                'rgba(84, 112, 198, 0.5)',   // #5470C6
                                'rgba(145, 204, 117, 0.5)',  // #91CC75
                                'rgba(250, 200, 88, 0.5)',   // #FAC858
                                'rgba(238, 102, 102, 0.5)',  // #EE6666
                                'rgba(115, 192, 222, 0.5)',  // #73C0DE
                                'rgba(59, 162, 114, 0.5)',   // #3BA272
                                'rgba(252, 132, 82, 0.5)',   // #FC8452
                                'rgba(154, 96, 180, 0.5)',   // #9A60B4
                                'rgba(234, 124, 204, 0.5)'   // #EA7CCC
                            ]
                            //colors: ['#A8DADC', '#F4A261', '#E76F51', '#2A9D8F', '#E9C46A', '#F07167', '#00AFB9', '#B5A6C9']
                            //colors: ['#5470C6', '#91CC75', '#FAC858', '#EE6666', '#73C0DE', '#3BA272', '#FC8452', '#9A60B4', '#EA7CCC'],
                        }],
                        tooltip: {
                            pointFormat: '<b>{point.percentage:.1f}%</b><br/>數量: <b>{point.y}</b> 棵'
                        },
                        legend: {
                            layout: 'horizontal',
                            align: 'center',
                            verticalAlign: 'bottom'
                        },
                        credits: {
                            enabled: false
                        }
                    });
                </script>

        
    </div>
    <script type="text/javascript">
        // 模擬老樹數量資料
        const data = <%= CityChartDataJson %>;

        Highcharts.chart('cityChart', {
            credits: { enabled: false },
            exporting: { enabled: false },
            chart: {
                type: 'column',
                backgroundColor: 'transparent',
                height: 400
            },
            title: {
                text: null
            },
            xAxis: {
                type: 'category',
                labels: {
                    rotation: -45,
                    style: {
                        fontSize: '16px'
                    }
                }
            },
            yAxis: {
                title: {
                    text: '受保護樹木數量'
                },
                min: 0
            },
            plotOptions: {
                column: {
                    //colorByPoint: true,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y}'
                    }
                }
            },
            series: [{
                name: '受保護樹木數量',
                data: data,
                //colors: ['#198754', '#20c997', '#28a745', '#17a2b8', '#6f42c1', '#e83e8c', '#fd7e14', '#ffc107']
                color: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, 'rgba(29, 105, 196, 0.5)'], // 红色
                        [1, 'rgba(102, 189, 189, 0.5)']  // 绿色
                    ]
                }
            }],
            tooltip: {
                pointFormat: '<b>{point.y}</b> 棵樹'
            },
            legend: {
                enabled: false
            }
        });

    </script>
</asp:Content>
