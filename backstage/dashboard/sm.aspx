<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage.Master" AutoEventWireup="true" CodeBehind="sm.aspx.cs" Inherits="protectTreesV2.backstage.dashboard.sm" %>
<%@ Register Src="~/_uc/dashboard/uc_treeStatusChart.ascx" TagPrefix="uc" TagName="TreeStatusChart" %>
<%@ Register Src="~/_uc/dashboard/uc_areaGroupPieChart.ascx" TagPrefix="uc" TagName="AreaGroupPieChart" %>
<%@ Register Src="~/_uc/dashboard/uc_speciesStatusChart.ascx" TagPrefix="uc" TagName="SpeciesStatusChart" %>
<%@ Register Src="~/_uc/dashboard/uc_healthCompletionChart.ascx" TagPrefix="uc" TagName="HealthCompletionChart" %>
<%@ Register Src="~/_uc/dashboard/uc_userLoginChart.ascx" TagPrefix="uc" TagName="UserLoginChart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <script src="<%= ResolveUrl("~/_js/highcharts.js") %>"></script>
    <script src="<%= ResolveUrl("~/_js/highcharts_exporting.js") %>"></script>
    <script src="<%= ResolveUrl("~/_js/highcharts_export-data.js") %>"></script>
    <style>
        .dashboard-card .card-body {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .dashboard-card-icon {
            width: 48px;
            height: 48px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.4rem;
        }

        .dashboard-chart {
            height: 320px;
        }

        .dashboard-chart-wide {
            height: 360px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    系統資訊概況
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    系統資訊概況
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="container-fluid">
        <div class="row g-3 mb-3">
            <div class="col">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-primary text-white d-none">
                            <i class="fa-solid fa-user-clock"></i>
                        </div>
                        <div>
                            <div class="text-muted small">待審帳號</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litPendingAccounts" runat="server" />
                            </div>
                            <div class="text-muted small">
                                上月通過 <asp:Literal ID="litPendingApprovedLastMonth" runat="server" /> <br />本月通過 <asp:Literal ID="litPendingApprovedCurrentMonth" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-success text-white d-none">
                            <i class="fa-solid fa-database"></i>
                        </div>
                        <div>
                            <div class="text-muted small">全部樹籍資料</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litTreeRecordTotal" runat="server" />
                            </div>
                            <div class="text-muted small">
                                上月新增 <asp:Literal ID="litTreeRecordLastMonth" runat="server" />  <br /> 本月新增 <asp:Literal ID="litTreeRecordCurrentMonth" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-info text-white d-none">
                            <i class="fa-solid fa-clipboard-list"></i>
                        </div>
                        <div>
                            <div class="text-muted small">全部健檢紀錄</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litHealthRecordTotal" runat="server" />
                            </div>
                            <div class="text-muted small">
                                上月新增 <asp:Literal ID="litHealthRecordLastMonth" runat="server" /> <br /> 本月新增 <asp:Literal ID="litHealthRecordCurrentMonth" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-warning text-white d-none">
                            <i class="fa-solid fa-camera"></i>
                        </div>
                        <div>
                            <div class="text-muted small">全部巡查紀錄</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litPatrolRecordTotal" runat="server" />
                            </div>
                            <div class="text-muted small">
                                上月新增 <asp:Literal ID="litPatrolRecordLastMonth" runat="server" /> <br /> 本月新增 <asp:Literal ID="litPatrolRecordCurrentMonth" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-secondary text-white d-none">
                            <i class="fa-solid fa-file-lines"></i>
                        </div>
                        <div>
                            <div class="text-muted small">全部養護紀錄</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litCareRecordTotal" runat="server" />
                            </div>
                            <div class="text-muted small">
                                上月新增 <asp:Literal ID="litCareRecordLastMonth" runat="server" /> <br /> 本月新增 <asp:Literal ID="litCareRecordCurrentMonth" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3 mb-3">
            <div class="col-12 col-md-4">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-success text-white d-none">
                            <i class="fa-solid fa-tree"></i>
                        </div>
                        <div>
                            <div class="text-muted small">公告列管</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litTreeStatusAnnounced" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-primary text-white d-none">
                            <i class="fa-solid fa-tree"></i>
                        </div>
                        <div>
                            <div class="text-muted small">符合標準</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litTreeStatusQualified" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="card shadow-sm h-100 dashboard-card">
                    <div class="card-body">
                        <div class="dashboard-card-icon bg-secondary text-white d-none">
                            <i class="fa-solid fa-tree"></i>
                        </div>
                        <div>
                            <div class="text-muted small">其他</div>
                            <div class="fs-4 fw-bold">
                                <asp:Literal ID="litTreeStatusOther" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3 mb-3">
            <div class="col-12 col-lg-8">
                <div class="card shadow-sm h-100">
                    <div class="card-header">樹籍資料狀態</div>
                    <div class="card-body">
                        <uc:TreeStatusChart ID="ucTreeStatusChart" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-lg-4">
                <div class="card shadow-sm h-100">
                    <div class="card-header">各區受保護樹木數量</div>
                    <div class="card-body">
                        <uc:AreaGroupPieChart ID="ucAreaGroupPieChart" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3 mb-3">
            <div class="col-12">
                <div class="card shadow-sm">
                    <div class="card-header">受保護樹木樹種統計</div>
                    <div class="card-body">
                        <uc:SpeciesStatusChart ID="ucSpeciesStatusChart" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3 mb-3">
            <div class="col-12">
                <div class="card shadow-sm">
                    <div class="card-header">各縣市受保護樹木檢查暨風險評估完成率</div>
                    <div class="card-body">
                        <uc:HealthCompletionChart ID="ucHealthCompletionChart" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3 mb-3">
            <div class="col-12">
                <div class="card shadow-sm">
                    <div class="card-header">近 30 天使用者登入次數</div>
                    <div class="card-body">
                        <uc:UserLoginChart ID="ucUserLoginChart" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
