using protectTreesV2.Patrol;
using System.Collections.Generic;

namespace protectTreesV2._uc.patrol
{
    public partial class uc_patrolRecordModal : System.Web.UI.UserControl
    {
        public void BindRecord(Patrol.PatrolRecord record, List<Patrol.PatrolPhoto> photos)
        {
            if (record == null)
            {
                ClearContent();
                phContent.Visible = false;
                phEmpty.Visible = true;
                return;
            }

            phContent.Visible = true;
            phEmpty.Visible = false;

            litMemo.Text = FormatText(record.memo);
            litRisk.Text = record.hasPublicSafetyRisk ? "是" : "否";

            BindPhotos(photos);
        }

        private void BindPhotos(List<Patrol.PatrolPhoto> photos)
        {
            if (photos == null || photos.Count == 0)
            {
                rptPhotos.DataSource = null;
                rptPhotos.DataBind();
                phPhotoEmpty.Visible = true;
                return;
            }

            phPhotoEmpty.Visible = false;
            rptPhotos.DataSource = photos;
            rptPhotos.DataBind();
        }

        private void ClearContent()
        {
            litRisk.Text = string.Empty;
            litMemo.Text = string.Empty;

            BindPhotos(null);
        }

        private static string FormatText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "--" : value.Trim();
        }

    }
}
