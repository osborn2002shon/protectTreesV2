using System;
using protectTreesV2;
using protectTreesV2.Dashboard;

namespace protectTreesV2.backstage.dashboard
{
    public partial class sm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            var user = UserInfo.GetCurrentUser;
            if (user == null)
            {
                return;
            }

            BindSummary(user.unitID);
            BindCharts(user.unitID);
        }

        private void BindSummary(int unitId)
        {
            var pendingSummary = DashboardService.GetPendingAccountSummary(unitId);
            litPendingAccounts.Text = pendingSummary.PendingCount.ToString("N0");
            litPendingApprovedLastMonth.Text = pendingSummary.LastMonthApproved.ToString("N0");
            litPendingApprovedCurrentMonth.Text = pendingSummary.CurrentMonthApproved.ToString("N0");

            var treeSummary = DashboardService.GetTreeRecordSummary(unitId);
            litTreeRecordTotal.Text = treeSummary.TotalCount.ToString("N0");
            litTreeRecordLastMonth.Text = treeSummary.LastMonthCount.ToString("N0");
            litTreeRecordCurrentMonth.Text = treeSummary.CurrentMonthCount.ToString("N0");

            var healthSummary = DashboardService.GetHealthRecordSummary(unitId);
            litHealthRecordTotal.Text = healthSummary.TotalCount.ToString("N0");
            litHealthRecordLastMonth.Text = healthSummary.LastMonthCount.ToString("N0");
            litHealthRecordCurrentMonth.Text = healthSummary.CurrentMonthCount.ToString("N0");

            var patrolSummary = DashboardService.GetPatrolRecordSummary(unitId);
            litPatrolRecordTotal.Text = patrolSummary.TotalCount.ToString("N0");
            litPatrolRecordLastMonth.Text = patrolSummary.LastMonthCount.ToString("N0");
            litPatrolRecordCurrentMonth.Text = patrolSummary.CurrentMonthCount.ToString("N0");

            var careSummary = DashboardService.GetCareRecordSummary(unitId);
            litCareRecordTotal.Text = careSummary.TotalCount.ToString("N0");
            litCareRecordLastMonth.Text = careSummary.LastMonthCount.ToString("N0");
            litCareRecordCurrentMonth.Text = careSummary.CurrentMonthCount.ToString("N0");

            litTreeStatusAnnounced.Text = DashboardService.GetTreeStatusCount(unitId, "已公告列管").ToString("N0");
            litTreeStatusQualified.Text = DashboardService.GetTreeStatusCount(unitId, "符合標準").ToString("N0");
            litTreeStatusOther.Text = DashboardService.GetTreeStatusCount(unitId, "其他").ToString("N0");
        }

        private void BindCharts(int unitId)
        {
            ucTreeStatusChart.UnitId = unitId;
            ucAreaGroupPieChart.UnitId = unitId;
            ucSpeciesStatusChart.UnitId = unitId;
            ucHealthCompletionChart.UnitId = unitId;
            ucUserLoginChart.UnitId = unitId;
        }
    }
}
