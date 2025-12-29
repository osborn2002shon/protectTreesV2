<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TreePhotoAlbum.ascx.cs" Inherits="protectTreesV2._uc.TreePhotoAlbum" %>
<style>
    .tree-cover-image img,
    .tree-gallery-thumb img {
        object-fit: cover;
    }

    .tree-loading {
        position: relative;
        background: #f1f3f5;
        overflow: hidden;
    }

    .tree-loading::before {
        content: "";
        position: absolute;
        inset: 0;
        background: linear-gradient(90deg, rgba(241, 243, 245, 0) 0%, rgba(222, 226, 230, 0.8) 50%, rgba(241, 243, 245, 0) 100%);
        transform: translateX(-100%);
        animation: tree-loading-shimmer 1.2s infinite;
    }

    .tree-loading img {
        opacity: 0;
        transition: opacity 0.3s ease;
    }

    .tree-loading.is-loaded img {
        opacity: 1;
    }

    .tree-loading.is-loaded::before {
        opacity: 0;
        animation: none;
    }

    @keyframes tree-loading-shimmer {
        100% {
            transform: translateX(100%);
        }
    }

    .tree-gallery-grid {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 0.75rem;
        padding: 0;
        margin: 0;
    }

    .tree-gallery-thumb {
        min-width: 0;
    }

    .tree-gallery-thumb .ratio {
        border: 1px solid #e9ecef;
        border-radius: 0.5rem;
    }
</style>
<asp:Panel ID="pnlPhotoGallery" runat="server" CssClass="tree-photo-album">
    <div class="row g-3 align-items-start">
        <div class="col-12">
            <asp:Panel ID="pnlCoverPhoto" runat="server" CssClass="tree-cover-image">
                <div class="ratio ratio-4x3 rounded overflow-hidden position-relative bg-light tree-loading">
                    <a id="lnkCoverLightbox" runat="server" class="tree-lightbox d-block h-100 w-100">
                        <asp:Image ID="imgCover" runat="server" CssClass="w-100 h-100" />
                    </a>
                </div>
            </asp:Panel>
        </div>

        <div class="col-12">
            <asp:Repeater ID="rptGallery" runat="server">
                <HeaderTemplate>
                    <div class="tree-gallery-grid">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="tree-gallery-thumb">
                        <a href='<%# Eval("FilePath") %>' class="tree-lightbox d-block h-100" data-gallery="tree-photos" data-title='<%# BuildLightboxTitleFromData(Container.DataItem) %>' data-description='<%# BuildLightboxDescriptionAttributeFromData(Container.DataItem) %>'>
                            <div class="ratio ratio-4x3 overflow-hidden bg-light tree-loading">
                                <img src='<%# Eval("FilePath") %>' loading="lazy" class="w-100 h-100" alt='<%# BuildLightboxTitleFromData(Container.DataItem) %>' />
                            </div>
                        </a>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Panel>
<asp:Label ID="lblNoPhotos" runat="server" Text="尚無照片" CssClass="text-muted" Visible="false" />
<script>
    (function () {
        function markLoaded(container) {
            container.classList.add('is-loaded');
        }

        function handleContainer(container) {
            var img = container.querySelector('img');
            if (!img) return;

            if (img.complete && img.naturalWidth > 0) {
                markLoaded(container);
                return;
            }

            img.addEventListener('load', function () {
                markLoaded(container);
            }, { once: true });

            img.addEventListener('error', function () {
                markLoaded(container);
            }, { once: true });
        }

        document.addEventListener('DOMContentLoaded', function () {
            var containers = document.querySelectorAll('.tree-loading');
            containers.forEach(handleContainer);
        });
    })();
</script>
