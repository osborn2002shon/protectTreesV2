using protectTreesV2.Base;
using protectTreesV2.Patrol;
using System.Collections.Generic;

namespace protectTreesV2._uc.patrol
{
    public partial class uc_patrolRecordModal : System.Web.UI.UserControl
    {
        public void BindRecord(protectTreesV2.Patrol.Patrol.PatrolRecord record, List<Patrol.Patrol.PatrolPhoto> photos)
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

        private void BindPhotos(List<Patrol.Patrol.PatrolPhoto> photos)
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

        protected string ResolvePhotoUrl(object pathObj)
        {
            var path = pathObj as string;
            var resolvedPath = VirtualPathHelper.ApplyVirtualName(path);
            return string.IsNullOrWhiteSpace(resolvedPath) ? "#" : ResolveUrl(resolvedPath);
        }

        protected string ResolvePhotoPreview(object pathObj)
        {
            var path = pathObj as string;
            var resolvedPath = VirtualPathHelper.ApplyVirtualName(path);
            return string.IsNullOrWhiteSpace(resolvedPath)
                ? "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="
                : ResolveUrl(resolvedPath);
        }

    }
}
