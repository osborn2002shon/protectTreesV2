using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using protectTreesV2._uc.dashboard;
using protectTreesV2.Base;
using protectTreesV2.Dashboard;
using protectTreesV2.SystemManagement;
using protectTreesV2.TreeCatalog;
using static protectTreesV2.Care.Care;
using static protectTreesV2.Health.Health;
using static protectTreesV2.Patrol.Patrol;

namespace protectTreesV2.backstage.dashboard
{
    public partial class hm : BasePage
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
            //ShowMessage("AAA", user.unitID.ToString());
            BindSummary(user.unitID);
            BindCharts(user.unitID);
        }

        private void BindSummary(int unitId)
        {
            var treeSummary = DashboardService.GetTreeRecordSummary(unitId);
            litTreeRecordTotal.Text = treeSummary.TotalCount.ToString("N0");
            litTreeRecordLastMonth.Text = treeSummary.LastMonthCount.ToString("N0");
            litTreeRecordCurrentMonth.Text = treeSummary.CurrentMonthCount.ToString("N0");

            var healthSummary = DashboardService.GetHealthRecordSummary(unitId);
            litHealthRecordTotal.Text = healthSummary.TotalCount.ToString("N0");
            litHealthRecordLastMonth.Text = healthSummary.LastMonthCount.ToString("N0");
            litHealthRecordCurrentMonth.Text = healthSummary.CurrentMonthCount.ToString("N0");

            litTreeStatusAnnounced.Text = DashboardService.GetTreeStatusCount(unitId, "已公告列管").ToString("N0");
            litTreeStatusQualified.Text = DashboardService.GetTreeStatusCount(unitId, "符合標準").ToString("N0");
            litTreeStatusOther.Text = DashboardService.GetTreeStatusCount(unitId, "其他").ToString("N0");
        }

        private void BindCharts(int unitId)
        {
            ucSpeciesStatusChart.UnitId = unitId;
            ucHealthCompletionChart.UnitId = unitId;
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