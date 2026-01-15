using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using protectTreesV2.Base;
using protectTreesV2.TreeCatalog;

namespace protectTreesV2.backstage.tree
{
    public partial class edit_photos : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int treeId;
                if (!int.TryParse(setTreeID, out treeId) || treeId <= 0)
                {
                    Response.Redirect("query.aspx");
                    return;
                }

                hfTreeID.Value = treeId.ToString();
                BindTree();
                BindPhotos();
            }
        }

        private void BindTree()
        {
            int treeId = int.Parse(hfTreeID.Value);
            var tree = TreeService.GetTree(treeId);
            if (tree == null)
            {
                Response.Redirect("query.aspx");
                return;
            }

            lblTreeInfo.Text = $"樹籍編號：{tree.SystemTreeNo ?? "(未編號)"}，樹種：{tree.SpeciesDisplayName}";
        }

        private void BindPhotos()
        {
            int treeId = int.Parse(hfTreeID.Value);
            var photos = TreeService.GetPhotos(treeId);
            rptPhotos.DataSource = photos;
            rptPhotos.DataBind();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            int treeId = int.Parse(hfTreeID.Value);
            var existing = TreeService.GetPhotos(treeId);
            if (!fuPhotos.HasFiles)
            {
                ShowMessage("提示", "請選擇要上傳的照片", "info");
                return;
            }

            var files = fuPhotos.PostedFiles;
            if (files.Count > 5)
            {
                ShowMessage("限制", "一次最多上傳 5 張照片", "warning");
                return;
            }

            if (existing.Count + files.Count > 5)
            {
                ShowMessage("限制", "每棵樹最多保留 5 張照片", "warning");
                return;
            }

            const int maxSize = 5 * 1024 * 1024;
            var user = UserInfo.GetCurrentUser;
            int accountId = user?.accountID ?? 0;
            bool hasCover = existing.Any(p => p.IsCover);
            string uploadFolder = Server.MapPath($"~/upload/tree/{treeId}/");
            Directory.CreateDirectory(uploadFolder);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.ContentLength > maxSize)
                {
                    ShowMessage("限制", $"{file.FileName} 超過 5MB，未上傳", "warning");
                    continue;
                }

                string fileName = Path.GetFileName(file.FileName);
                string savedName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{fileName}";
                string physicalPath = Path.Combine(uploadFolder, savedName);
                file.SaveAs(physicalPath);

                var photo = new TreePhoto
                {
                    TreeID = treeId,
                    FileName = fileName,
                    FilePath = $"/upload/tree/{treeId}/{savedName}",
                    Caption = fileName,
                    IsCover = !hasCover && existing.Count == 0 && i == 0
                };

                int photoId = TreeService.InsertPhoto(photo, accountId);
                if (photo.IsCover)
                {
                    TreeService.SetCoverPhoto(treeId, photoId, accountId);
                    hasCover = true;
                }
            }

            BindPhotos();
            ShowMessage("完成", "照片上傳完成", "success");
        }

        protected void rptPhotos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int treeId = int.Parse(hfTreeID.Value);
            var user = UserInfo.GetCurrentUser;
            int accountId = user?.accountID ?? 0;
            int photoId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "cover")
            {
                TreeService.SetCoverPhoto(treeId, photoId, accountId);
                BindPhotos();
            }
            else if (e.CommandName == "delete")
            {
                var photo = TreeService.GetPhotos(treeId).FirstOrDefault(p => p.PhotoID == photoId);
                TreeService.DeletePhoto(photoId, accountId);
                if (photo != null)
                {
                    string physical = Server.MapPath(photo.FilePath);
                    if (File.Exists(physical))
                    {
                        File.Delete(physical);
                    }
                }
                BindPhotos();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("edit.aspx");
        }
    }
}
