using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2._uc
{
    public partial class TreePhotoAlbum : UserControl
    {
        private const string TransparentPixel = "data:image/gif;base64,R0lGODlhAQABAAAAACw=";
        public IEnumerable<TreePhoto> Photos { get; private set; } = Enumerable.Empty<TreePhoto>();
        public string GalleryName { get; set; } = "tree-photos";
        protected string GalleryNameValue => string.IsNullOrWhiteSpace(GalleryName) ? "tree-photos" : GalleryName;

        public void SetPhotos(IEnumerable<TreePhoto> photos)
        {
            Photos = photos ?? Enumerable.Empty<TreePhoto>();
            BindPhotoGallery();
        }

        private void BindPhotoGallery()
        {
            var photoList = Photos?.ToList() ?? new List<TreePhoto>();
            if (!photoList.Any())
            {
                pnlPhotoGallery.Visible = false;
                lblNoPhotos.Visible = true;
                return;
            }

            var coverPhoto = photoList.FirstOrDefault(p => p.IsCover) ?? photoList.FirstOrDefault();

            if (coverPhoto != null)
            {
                imgCover.ImageUrl = TransparentPixel;
                imgCover.AlternateText = BuildLightboxTitle(coverPhoto);
                imgCover.Attributes["loading"] = "lazy";
                imgCover.Attributes["decoding"] = "async";
                imgCover.Attributes["data-src"] = coverPhoto.FilePath;

                lnkCoverLightbox.HRef = coverPhoto.FilePath;
                lnkCoverLightbox.Attributes["data-gallery"] = GalleryNameValue;
                lnkCoverLightbox.Attributes["data-title"] = BuildLightboxTitle(coverPhoto);
                lnkCoverLightbox.Attributes["data-description"] = BuildLightboxDescriptionAttribute(coverPhoto);
            }

            var galleryPhotos = photoList
                .Where(p => coverPhoto == null || p.PhotoID != coverPhoto.PhotoID)
                .ToList();

            rptGallery.DataSource = galleryPhotos;
            rptGallery.DataBind();
            rptGallery.Visible = galleryPhotos.Any();

            pnlPhotoGallery.Visible = true;
            lblNoPhotos.Visible = false;
        }

        protected string BuildPhotoCaption(TreePhoto photo)
        {
            if (photo == null) return "未命名照片";
            return string.IsNullOrWhiteSpace(photo.Caption) ? "未命名照片" : photo.Caption;
        }

        protected string BuildUploadTimeDisplay(TreePhoto photo)
        {
            if (photo == null || photo.InsertDateTime == default)
            {
                return "未知時間";
            }

            return photo.InsertDateTime.ToString("yyyy/MM/dd HH:mm");
        }

        protected string BuildLightboxTitle(TreePhoto photo)
        {
            if (photo == null) return string.Empty;
            var caption = BuildPhotoCaption(photo);
            return photo.IsCover ? $"{caption}（封面）" : caption;
        }

        protected string BuildLightboxDescriptionAttribute(TreePhoto photo)
        {
            if (photo == null) return string.Empty;
            var caption = BuildPhotoCaption(photo);
            var uploadTime = BuildUploadTimeDisplay(photo);
            var description = $"{caption}｜上傳：{uploadTime}";
            return HttpUtility.HtmlAttributeEncode(description);
        }

        protected string BuildLightboxTitleFromData(object dataItem)
        {
            return BuildLightboxTitle(dataItem as TreePhoto);
        }

        protected string BuildLightboxDescriptionAttributeFromData(object dataItem)
        {
            return BuildLightboxDescriptionAttribute(dataItem as TreePhoto);
        }
    }
}
