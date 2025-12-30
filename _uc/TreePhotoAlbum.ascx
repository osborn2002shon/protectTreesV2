<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TreePhotoAlbum.ascx.cs" Inherits="protectTreesV2._uc.TreePhotoAlbum" %>
<style>
    .tree-cover-image img,
    .tree-gallery-thumb img {
        object-fit: cover;
    }

    .tree-gallery-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
        gap: 0.75rem;
        padding: 0;
        margin: 0;
    }

    .tree-gallery-thumb {
        min-width: 100px;
    }

    .tree-gallery-thumb .ratio {
        border: 1px solid #e9ecef;
        border-radius: 0.5rem;
    }

    .image-wrapper {
        position: relative;
        isolation: isolate;
    }

    .image-wrapper .lazy-image {
        opacity: 0;
        filter: blur(8px);
        transition: opacity 300ms ease, filter 300ms ease;
    }

    .image-wrapper.is-loaded .lazy-image {
        opacity: 1;
        filter: blur(0);
    }

    .image-wrapper::after {
        content: "";
        position: absolute;
        inset: 0;
        z-index: 1;
        background: linear-gradient(90deg, #f8f9fa 25%, #e9ecef 50%, #f8f9fa 75%);
        background-size: 200% 100%;
        animation: shimmer 1.2s infinite;
        opacity: 0;
        transition: opacity 200ms ease;
        pointer-events: none;
    }


    .image-wrapper.is-loading::after {
        opacity: 1;
    }

    .image-wrapper .loading-indicator {
        position: absolute;
        inset: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 2;
        transition: opacity 200ms ease;
        opacity: 0;
        pointer-events: none;
    }

    .image-wrapper.is-loading .loading-indicator {
        opacity: 1;
    }

    @keyframes shimmer {
        0% {
            background-position: -200% 0;
        }

        100% {
            background-position: 200% 0;
        }
    }
</style>
<asp:Panel ID="pnlPhotoGallery" runat="server" CssClass="tree-photo-album">
    <div class="row g-3 align-items-start">
        <div class="col-12">
            <asp:Panel ID="pnlCoverPhoto" runat="server" CssClass="tree-cover-image">
                <div class="ratio ratio-4x3 rounded overflow-hidden position-relative bg-light image-wrapper is-loading">
                    <div class="loading-indicator">
                        <div class="spinner-border spinner-border-sm text-success" role="status">
                            <span class="visually-hidden">圖片載入中</span>
                        </div>
                    </div>
                    <a id="lnkCoverLightbox" runat="server" class="tree-lightbox d-block h-100 w-100">
                        <asp:Image ID="imgCover" runat="server" CssClass="w-100 h-100 lazy-image" />
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
                            <div class="ratio ratio-4x3 overflow-hidden bg-light image-wrapper is-loading">
                                <div class="loading-indicator">
                                    <div class="spinner-border spinner-border-sm text-success" role="status">
                                        <span class="visually-hidden">圖片載入中</span>
                                    </div>
                                </div>
                                <img data-src='<%# Eval("FilePath") %>' src="data:image/gif;base64,R0lGODlhAQABAAAAACw=" loading="lazy" decoding="async" class="w-100 h-100 lazy-image" alt='<%# BuildLightboxTitleFromData(Container.DataItem) %>' />
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
<script type="text/javascript">
    (function () {
        const lazyImages = Array.prototype.slice.call(document.querySelectorAll('.tree-photo-album .lazy-image'));

        if (!lazyImages.length) {
            return;
        }

        function markLoaded(img) {
            const wrapper = img.closest('.image-wrapper');
            if (wrapper) {
                wrapper.classList.remove('is-loading');
                wrapper.classList.add('is-loaded');
            }
        }

        function loadImage(img) {
            if (img.dataset.loaded === "true") {
                return;
            }

            const src = img.getAttribute('data-src') || img.getAttribute('src');
            if (!src) {
                return;
            }

            img.dataset.loaded = "true";

            const initiateLoad = function () {
                const handleComplete = function () { markLoaded(img); };
                img.addEventListener('load', handleComplete, { once: true });
                img.addEventListener('error', handleComplete, { once: true });

                if (img.getAttribute('src') !== src) {
                    img.src = src;
                } else {
                    markLoaded(img);
                }
            };

            window.setTimeout(initiateLoad, 500);
        }

        if ('IntersectionObserver' in window) {
            const observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        loadImage(entry.target);
                        observer.unobserve(entry.target);
                    }
                });
            }, { rootMargin: '120px 0px' });

            lazyImages.forEach(function (img) {
                observer.observe(img);
            });
        } else {
            lazyImages.forEach(loadImage);
        }
    })();
</script>
