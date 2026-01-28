using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.Dashboard;
using protectTreesV2.SystemManagement;
using protectTreesV2.TreeCatalog;
using static protectTreesV2.Care.Care;
using static protectTreesV2.Health.Health;
using static protectTreesV2.Patrol.Patrol;

namespace protectTreesV2.backstage.dashboard
{
    public partial class tm : BasePage
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

            //ShowMessage("提示", user.unitID.ToString());
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
            ucSpeciesStatusChart.UnitId = unitId;
        }

        protected void BtnPendingAccounts_Click(object sender, EventArgs e)
        {
            var currentFilter = new AccountManageFilter
            {
                verfiyStatus = AccountVerifyStatus.尚未審核
            };
            base.RedirectState("../../backstage/system/accountManage.aspx", currentFilter);
        }

        protected void BtnTreeRecordTotal_Click(object sender, EventArgs e)
        {
            var currentFilter = new TreeFilter
            {
                EditStatus = TreeEditState.定稿
            };
            base.RedirectState("../../backstage/tree/query.aspx", currentFilter);
        }

        protected void BtnHealthRecordTotal_Click(object sender, EventArgs e)
        {
            var currentFilter = new HealthRecordListFilter
            {
                dataStatus = TreeEditState.定稿
            };
            base.RedirectState("../../backstage/health/list.aspx", currentFilter);
        }

        protected void BtnPatrolRecordTotal_Click(object sender, EventArgs e)
        {
            var currentFilter = new PatrolRecordListFilter
            {
                dataStatus = TreeEditState.定稿
            };
            base.RedirectState("../../backstage/patrol/list.aspx", currentFilter);
        }

        protected void BtnCareRecordTotal_Click(object sender, EventArgs e)
        {
            var currentFilter = new CareRecordListFilter
            {
                dataStatus = TreeEditState.定稿
            };
            base.RedirectState("../../backstage/care/list.aspx", currentFilter);
        }

        protected void BtnTreeStatusAnnounced_Click(object sender, EventArgs e)
        {
            var currentFilter = new TreeFilter
            {
                EditStatus = TreeEditState.定稿,
                Status = TreeStatus.已公告列管
            };
            base.RedirectState("../../backstage/tree/query.aspx", currentFilter);
        }

        protected void BtnTreeStatusQualified_Click(object sender, EventArgs e)
        {
            var currentFilter = new TreeFilter
            {
                EditStatus = TreeEditState.定稿,
                Status = TreeStatus.符合標準
            };
            base.RedirectState("../../backstage/tree/query.aspx", currentFilter);
        }

        protected void BtnTreeStatusOther_Click(object sender, EventArgs e)
        {
            var currentFilter = new TreeFilter
            {
                EditStatus = TreeEditState.定稿,
                Status = TreeStatus.其他
            };
            base.RedirectState("../../backstage/tree/query.aspx", currentFilter);
        }

    }
}