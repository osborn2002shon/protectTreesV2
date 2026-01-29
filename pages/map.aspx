
<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_publicMap.Master" AutoEventWireup="true" CodeBehind="map.aspx.cs" Inherits="protectTreesV2.pages.map1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <link rel="stylesheet" href="https://js.arcgis.com/4.29/esri/themes/light/main.css" />
    <script src="https://js.arcgis.com/4.29/"></script>
    <link href="../_css/mp_public_map.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_map" runat="server">
    <div id="mapPage">
        <div id="mapView"></div>
        <div id="mapLoading" class="map-loading" aria-live="polite">
            <div class="spinner" role="status" aria-label="地圖載入中"></div>
            <span>地圖載入中...</span>
        </div>
        <div class="map-tools">
            <div class="accordion" id="mapToolsAccordion">
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingLocate">
                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseLocate" aria-expanded="true" aria-controls="collapseLocate">
                            <i class="fa-solid fa-location-crosshairs me-2"></i>地圖定位
                        </button>
                    </h2>
                    <div id="collapseLocate" class="accordion-collapse collapse show" aria-labelledby="headingLocate" data-bs-parent="#mapToolsAccordion">
                        <div class="accordion-body">
                            <div class="mb-3">
                                <label class="form-label" for="inputLat">緯度</label>
                                <input id="inputLat" class="form-control" type="number" step="any" placeholder="例：23.6978" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label" for="inputLng">經度</label>
                                <input id="inputLng" class="form-control" type="number" step="any" placeholder="例：120.9605" />
                            </div>
                            <div class="d-grid gap-2">
                                <button class="btn btn-primary" type="button" id="btnLocate">輸入經緯度座標定位</button>
                                <button class="btn btn-outline-warning" type="button" id="btnGeolocate">使用目前位置定位</button>
                            </div>
                            <p class="map-note mt-3 mb-0">建議輸入 WGS84 經緯度即可快速定位。</p>
                        </div>
                    </div>
                </div>
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingFilter">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseFilter" aria-expanded="false" aria-controls="collapseFilter">
                            <i class="fa-solid fa-filter me-2"></i>篩選條件
                        </button>
                    </h2>
                    <div id="collapseFilter" class="accordion-collapse collapse" aria-labelledby="headingFilter" data-bs-parent="#mapToolsAccordion">
                        <div class="accordion-body">
                            <div class="map-filter-scroll">
                                <div class="mb-3">
                                    <label class="form-label" for="filterCity">縣市</label>
                                    <select id="filterCity" class="form-select"></select>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label" for="filterArea">鄉鎮區</label>
                                    <select id="filterArea" class="form-select"></select>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label" for="filterSpecies">樹種</label>
                                    <select id="filterSpecies" class="form-select" data-combobox="species" data-combobox-placeholder="請輸入樹種"></select>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">樹齡</label>
                                    <div class="range-input-group">
                                        <input id="filterAgeMin" class="form-control" type="number" placeholder="最小值" />
                                        <span class="range-separator">~</span>
                                        <input id="filterAgeMax" class="form-control" type="number" placeholder="最大值" />
                                    </div>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">胸高直徑 (cm)</label>
                                    <div class="range-input-group">
                                        <input id="filterDiameterMin" class="form-control" type="number" placeholder="最小值" />
                                        <span class="range-separator">~</span>
                                        <input id="filterDiameterMax" class="form-control" type="number" placeholder="最大值" />
                                    </div>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">胸高樹圍 (cm)</label>
                                    <div class="range-input-group">
                                        <input id="filterCircumferenceMin" class="form-control" type="number" placeholder="最小值" />
                                        <span class="range-separator">~</span>
                                        <input id="filterCircumferenceMax" class="form-control" type="number" placeholder="最大值" />
                                    </div>
                                </div>
                            </div>
                            <div class="d-grid">
                                <button class="btn btn-primary" type="button" id="btnFilterApply">查詢</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingBasemap">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseBasemap" aria-expanded="false" aria-controls="collapseBasemap">
                            <i class="fa-solid fa-layer-group me-2"></i>底圖設定
                        </button>
                    </h2>
                    <div id="collapseBasemap" class="accordion-collapse collapse" aria-labelledby="headingBasemap" data-bs-parent="#mapToolsAccordion">
                        <div class="accordion-body">
                            <select id="basemapSelect" class="form-select mb-3">
                                <option value="NLSC_1">臺灣通用版電子地圖</option>
                                <option value="NLSC_3">臺灣通用版正射影像</option>
                                <%--<option value="streets">ArcGIS 街道圖</option>
                                <option value="topo-vector">ArcGIS 地形圖</option>
                                <option value="hybrid">ArcGIS 衛星影像</option>--%>
                            </select>
                            <%--<div class="map-note">可切換政府圖資或 ArcGIS 底圖，適合不同查詢需求。</div>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="treeListPanel" class="map-tree-panel is-hidden" aria-hidden="true">
            <div class="map-tree-header">
                <div>
                    <h3>樹籍資料</h3>
                    <div class="map-tree-count" id="treeListCount">0 筆</div>
                </div>
                <button class="map-tree-close" type="button" id="treeListClose" aria-label="關閉樹籍資料清單">
                    <i class="fa-solid fa-xmark"></i>
                </button>
            </div>
            <div class="map-tree-content" id="treeListContent"></div>
            <div class="map-tree-pagination" id="treeListPagination">
                <button class="btn btn-outline-secondary btn-sm" type="button" id="treeListPrev">上一頁</button>
                <span class="map-page-info" id="treeListPageInfo">第 1 頁 / 共 1 頁</span>
                <button class="btn btn-outline-secondary btn-sm" type="button" id="treeListNext">下一頁</button>
            </div>
        </div>
        <div id="photoLightbox" class="photo-lightbox" aria-hidden="true">
            <div class="photo-lightbox-dialog" role="dialog" aria-modal="true" aria-label="照片相簿">
                <div class="photo-lightbox-header">
                    <span>照片相簿</span>
                    <button class="photo-lightbox-close" type="button" id="photoLightboxClose" aria-label="關閉相簿">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div class="photo-lightbox-body">
                    <button class="photo-lightbox-nav prev" type="button" id="photoLightboxPrev" aria-label="上一張">
                        <i class="fa-solid fa-chevron-left"></i>
                    </button>
                    <img class="photo-lightbox-image" id="photoLightboxImage" alt="照片預覽" />
                    <button class="photo-lightbox-nav next" type="button" id="photoLightboxNext" aria-label="下一張">
                        <i class="fa-solid fa-chevron-right"></i>
                    </button>
                </div>
                <div class="photo-lightbox-thumbs" id="photoLightboxThumbs"></div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="TreeDataJson" runat="server" />
    <script type="text/javascript">
        const loadingElement = document.getElementById("mapLoading");
        const treeDataInput = document.getElementById("<%= TreeDataJson.ClientID %>");
        const treeData = treeDataInput && treeDataInput.value ? JSON.parse(treeDataInput.value) : [];

        require([
            "esri/Map",
            "esri/views/MapView",
            "esri/layers/WMTSLayer",
            "esri/widgets/Zoom",
            "esri/Graphic",
            "esri/layers/GraphicsLayer"
        ], (Map, MapView, WMTSLayer, Zoom, Graphic, GraphicsLayer) => {
            const baseMapLayer = new WMTSLayer({
                url: "https://wmts.nlsc.gov.tw/wmts",
                activeLayer: { id: "EMAP" },
                title: "通用版電子地圖"
            });

            const orthoLayer = new WMTSLayer({
                url: "https://wmts.nlsc.gov.tw/wmts",
                activeLayer: { id: "PHOTO2" },
                title: "正射影像圖(臺灣通用)"
            });

            orthoLayer.visible = false;

            const map = new Map({
                basemap: "streets",
                layers: [baseMapLayer, orthoLayer]
            });

            const graphicsLayer = new GraphicsLayer();
            const highlightLayer = new GraphicsLayer();
            map.add(graphicsLayer);
            map.add(highlightLayer);

            const view = new MapView({
                container: "mapView",
                map: map,
                center: [120.9605, 23.6978],
                zoom: 7,
                ui: { components: ["attribution"] },
                constraints: {
                    minZoom: 5,
                    maxZoom: 19
                }
            });

            const zoomWidget = new Zoom({
                view: view
            });
            view.ui.add(zoomWidget, "bottom-right");
            view.popup.autoOpenEnabled = false;

            view.when(() => {
                loadingElement.classList.add("is-hidden");
            }).catch(() => {
                loadingElement.classList.add("is-hidden");
            });

            const locateButton = document.getElementById("btnLocate");
            const geoButton = document.getElementById("btnGeolocate");
            const latInput = document.getElementById("inputLat");
            const lngInput = document.getElementById("inputLng");

            locateButton.addEventListener("click", () => {
                const lat = parseFloat(latInput.value);
                const lng = parseFloat(lngInput.value);

                if (Number.isNaN(lat) || Number.isNaN(lng)) {
                    alert("請輸入有效的緯度與經度。\n例如：23.6978, 120.9605");
                    return;
                }

                view.goTo({
                    center: [lng, lat],
                    zoom: 14
                });
            });

            geoButton.addEventListener("click", () => {
                if (!navigator.geolocation) {
                    alert("瀏覽器不支援定位功能。");
                    return;
                }

                navigator.geolocation.getCurrentPosition(
                    (position) => {
                        const lat = Number(position.coords.latitude.toFixed(6));
                        const lng = Number(position.coords.longitude.toFixed(6));
                        latInput.value = lat;
                        lngInput.value = lng;
                        view.goTo({ center: [lng, lat], zoom: 15 });
                    },
                    (error) => {
                        alert("無法取得目前位置：" + error.message);
                    }
                );
            });

            const basemapSelect = document.getElementById("basemapSelect");
            basemapSelect.addEventListener("change", (event) => {
                const selection = event.target.value;

                if (selection === "NLSC_1") {
                    baseMapLayer.visible = true;
                    orthoLayer.visible = false;
                    map.basemap = "streets";
                    return;
                }

                if (selection === "NLSC_3") {
                    baseMapLayer.visible = false;
                    orthoLayer.visible = true;
                    map.basemap = "streets";
                    return;
                }

                baseMapLayer.visible = false;
                orthoLayer.visible = false;
                map.basemap = selection;
            });

            const treeSymbol = {
                type: "picture-marker",
                url: "../_img/icon/pin.png",
                width: "30px",
                height: "30px"
            };

            const highlightSymbol = {
                type: "simple-marker",
                color: [0, 0, 0, 0],
                size: 40,
                outline: {
                    color: [37, 99, 235, 0.9],
                    width: 3
                }
            };

            const popupTemplate = {
                title: "{systemTreeNo}",
                content: [{
                    type: "fields",
                    fieldInfos: [
                        { fieldName: "species", label: "樹種" },
                        { fieldName: "city", label: "縣市" },
                        { fieldName: "area", label: "鄉鎮區" },
                        { fieldName: "age", label: "樹齡", format: { digitSeparator: true } },
                        { fieldName: "diameter", label: "胸高直徑 (cm)" },
                        { fieldName: "circumference", label: "胸高樹圍 (cm)" }
                    ]
                }]
            };

            const parseNumber = (value) => {
                if (value === null || value === undefined || value === "") {
                    return null;
                }
                const parsed = Number(String(value).replace(/,/g, ""));
                return Number.isFinite(parsed) ? parsed : null;
            };

            const matchesRange = (value, min, max) => {
                if (value === null) {
                    return min === null && max === null;
                }
                if (min !== null && value < min) {
                    return false;
                }
                if (max !== null && value > max) {
                    return false;
                }
                return true;
            };

            const treeListPanel = document.getElementById("treeListPanel");
            const treeListContent = document.getElementById("treeListContent");
            const treeListCount = document.getElementById("treeListCount");
            const treeListClose = document.getElementById("treeListClose");
            const treeListPagination = document.getElementById("treeListPagination");
            const treeListPrev = document.getElementById("treeListPrev");
            const treeListNext = document.getElementById("treeListNext");
            const treeListPageInfo = document.getElementById("treeListPageInfo");
            const photoLightbox = document.getElementById("photoLightbox");
            const photoLightboxImage = document.getElementById("photoLightboxImage");
            const photoLightboxThumbs = document.getElementById("photoLightboxThumbs");
            const photoLightboxClose = document.getElementById("photoLightboxClose");
            const photoLightboxPrev = document.getElementById("photoLightboxPrev");
            const photoLightboxNext = document.getElementById("photoLightboxNext");
            let treeListEntries = [];
            let treeListPage = 1;
            const treeListPageSize = 10;
            let currentPhotoUrls = [];
            let currentPhotoIndex = 0;

            const setTreeListOpen = (open) => {
                if (!treeListPanel) {
                    return;
                }
                treeListPanel.classList.toggle("is-hidden", !open);
                treeListPanel.setAttribute("aria-hidden", open ? "false" : "true");
            };

            const updatePaginationState = () => {
                if (!treeListPagination || !treeListPrev || !treeListNext || !treeListPageInfo) {
                    return;
                }
                const totalPages = Math.max(1, Math.ceil(treeListEntries.length / treeListPageSize));
                const currentPage = Math.min(treeListPage, totalPages);
                treeListPage = currentPage;
                treeListPageInfo.textContent = `第 ${currentPage} 頁 / 共 ${totalPages} 頁`;
                treeListPrev.disabled = currentPage <= 1;
                treeListNext.disabled = currentPage >= totalPages;
            };

            const normalizePhotoUrl = (url) => {
                if (!url) {
                    return null;
                }
                if (url.startsWith("http://") || url.startsWith("https://") || url.startsWith("/")) {
                    return url;
                }
                return `/${url}`;
            };

            const parsePhotoUrls = (rawUrls) => {
                if (!rawUrls) {
                    return [];
                }
                return String(rawUrls)
                    .split(/[|,;\n]+/)
                    .map((item) => item.trim())
                    .filter(Boolean)
                    .map((item) => normalizePhotoUrl(item))
                    .filter(Boolean);
            };

            const setLightboxOpen = (open) => {
                if (!photoLightbox) {
                    return;
                }
                photoLightbox.classList.toggle("is-open", open);
                photoLightbox.setAttribute("aria-hidden", open ? "false" : "true");
                if (!open && photoLightboxImage) {
                    photoLightboxImage.src = "";
                }
            };

            const updateLightboxView = (index) => {
                if (!photoLightboxImage || !photoLightboxThumbs) {
                    return;
                }
                if (currentPhotoUrls.length === 0) {
                    return;
                }
                currentPhotoIndex = Math.max(0, Math.min(index, currentPhotoUrls.length - 1));
                const currentUrl = currentPhotoUrls[currentPhotoIndex];
                photoLightboxImage.src = currentUrl;
                photoLightboxImage.alt = `照片 ${currentPhotoIndex + 1}`;
                photoLightboxThumbs.innerHTML = currentPhotoUrls
                    .map((url, idx) => {
                        const activeClass = idx === currentPhotoIndex ? "is-active" : "";
                        return `
                            <button class="photo-lightbox-thumb ${activeClass}" type="button" data-index="${idx}">
                                <img src="${url}" alt="縮圖 ${idx + 1}" loading="lazy" />
                            </button>
                        `;
                    })
                    .join("");
                if (photoLightboxPrev) {
                    photoLightboxPrev.disabled = currentPhotoIndex <= 0;
                }
                if (photoLightboxNext) {
                    photoLightboxNext.disabled = currentPhotoIndex >= currentPhotoUrls.length - 1;
                }
            };

            const openLightbox = (photoUrls, index) => {
                if (!photoUrls || photoUrls.length === 0) {
                    return;
                }
                currentPhotoUrls = photoUrls;
                setLightboxOpen(true);
                updateLightboxView(index);
            };

            const buildRecognitionList = (htmlContent) => {
                if (!htmlContent) {
                    return "<span class=\"map-note\">無資料</span>";
                }
                const sanitized = String(htmlContent).replace(/\n/g, "<br />");
                const items = sanitized
                    .split(/<br\s*\/?\s*>/i)
                    .map((item) => item.trim())
                    .filter(Boolean);
                if (items.length === 0) {
                    return "<span class=\"map-note\">無資料</span>";
                }
                const listItems = items.map((item) => `<li>${item}</li>`).join("");
                return `<ul class=\"map-tree-detail-criteria\">${listItems}</ul>`;
            };

            const renderTrees = (records) => {
                graphicsLayer.removeAll();
                highlightLayer.removeAll();
                const entries = [];
                records.forEach((tree) => {
                    const latitude = parseNumber(tree.Latitude);
                    const longitude = parseNumber(tree.Longitude);
                    if (latitude === null || longitude === null) {
                        return;
                    }

                    const graphic = new Graphic({
                        geometry: {
                            type: "point",
                            latitude,
                            longitude
                        },
                        symbol: treeSymbol,
                        attributes: {
                            systemTreeNo: tree.SystemTreeNo || "樹木資料",
                            species: tree.Species || "—",
                            city: tree.City || "—",
                            area: tree.Area || "—",
                            age: tree.Age ?? "—",
                            diameter: tree.BreastHeightDiameter || "—",
                            circumference: tree.BreastHeightCircumference || "—"
                        },
                        popupTemplate
                    });
                    graphicsLayer.add(graphic);
                    entries.push({
                        tree,
                        graphic,
                        latitude,
                        longitude
                    });
                });
                treeListEntries = entries;
                treeListPage = 1;
                return entries;
            };

            const highlightTree = (entry) => {
                if (!entry) {
                    highlightLayer.removeAll();
                    return;
                }
                highlightLayer.removeAll();
                const highlightGraphic = new Graphic({
                    geometry: {
                        type: "point",
                        latitude: entry.latitude,
                        longitude: entry.longitude
                    },
                    symbol: highlightSymbol
                });
                highlightLayer.add(highlightGraphic);
            };

            const citySelect = document.getElementById("filterCity");
            const areaSelect = document.getElementById("filterArea");
            const speciesSelect = document.getElementById("filterSpecies");
            const ageMinInput = document.getElementById("filterAgeMin");
            const ageMaxInput = document.getElementById("filterAgeMax");
            const diameterMinInput = document.getElementById("filterDiameterMin");
            const diameterMaxInput = document.getElementById("filterDiameterMax");
            const circumferenceMinInput = document.getElementById("filterCircumferenceMin");
            const circumferenceMaxInput = document.getElementById("filterCircumferenceMax");
            const filterButton = document.getElementById("btnFilterApply");

            const updateTreeList = (entries) => {
                if (!treeListContent || !treeListCount) {
                    return;
                }
                treeListContent.innerHTML = "";
                treeListCount.textContent = `${entries.length} 筆`;

                if (entries.length === 0) {
                    const emptyState = document.createElement("div");
                    emptyState.className = "map-note";
                    emptyState.textContent = "目前篩選條件下沒有可顯示的樹籍資料。";
                    treeListContent.appendChild(emptyState);
                    if (treeListPagination) {
                        treeListPagination.classList.add("is-hidden");
                    }
                    return;
                }

                if (treeListPagination) {
                    treeListPagination.classList.remove("is-hidden");
                }

                const startIndex = (treeListPage - 1) * treeListPageSize;
                const pageEntries = entries.slice(startIndex, startIndex + treeListPageSize);
                const table = document.createElement("table");
                table.className = "map-tree-table";
                const thead = document.createElement("thead");
                const headRow = document.createElement("tr");
                ["樹籍編號", "縣市", "鄉鎮", "樹種", "定位"].forEach((label) => {
                    const th = document.createElement("th");
                    th.textContent = label;
                    headRow.appendChild(th);
                });
                thead.appendChild(headRow);
                table.appendChild(thead);

                const tbody = document.createElement("tbody");
                pageEntries.forEach((entry, index) => {
                    const row = document.createElement("tr");

                    const treeIdCell = document.createElement("td");
                    treeIdCell.textContent = entry.tree.SystemTreeNo || "—";
                    row.appendChild(treeIdCell);

                    const cityCell = document.createElement("td");
                    cityCell.textContent = entry.tree.City || "未知縣市";
                    row.appendChild(cityCell);

                    const areaCell = document.createElement("td");
                    areaCell.textContent = entry.tree.Area || "未知鄉鎮區";
                    row.appendChild(areaCell);

                    const speciesCell = document.createElement("td");
                    speciesCell.textContent = entry.tree.Species || "未知樹種";
                    row.appendChild(speciesCell);

                    const locateCell = document.createElement("td");
                    const locateButton = document.createElement("button");
                    locateButton.type = "button";
                    locateButton.className = "btn btn-outline-primary btn-sm map-tree-locate";
                    locateButton.textContent = "定位";
                    locateButton.dataset.index = String(startIndex + index);
                    locateCell.appendChild(locateButton);
                    row.appendChild(locateCell);

                    tbody.appendChild(row);
                });
                table.appendChild(tbody);
                treeListContent.appendChild(table);
                updatePaginationState();
            };

            const renderTreeDetail = (entry) => {
                if (!treeListContent) {
                    return;
                }
                const tree = entry.tree || {};
                const latText = entry.latitude !== null && entry.latitude !== undefined ? entry.latitude : "—";
                const lngText = entry.longitude !== null && entry.longitude !== undefined ? entry.longitude : "—";
                const scientificName = tree.SpeciesScientificName ? ` (${tree.SpeciesScientificName})` : "";
                const speciesDisplay = tree.Species ? `${tree.Species}${scientificName}` : "—";
                const treeCount = tree.TreeCount ?? "—";
                const locationDisplay = [tree.City, tree.Area].filter(Boolean).join(" ") || "—";
                const siteDisplay = tree.Site || "—";
                const managerDisplay = tree.Manager || "—";
                const statusDisplay = tree.TreeStatus || "—";
                const announcementDisplay = tree.AnnouncementDate || "—";
                const culturalDisplay = tree.CulturalHistoryIntro || "—";
                const heightDisplay = tree.TreeHeight || "—";
                const diameterDisplay = tree.BreastHeightDiameter || "—";
                const circumferenceDisplay = tree.BreastHeightCircumference || "—";
                const photoUrls = parsePhotoUrls(tree.PhotoUrls || tree.PhotoUrl);
                currentPhotoUrls = photoUrls;
                currentPhotoIndex = 0;
                const photoHtml = photoUrls.length
                    ? `<div class="map-tree-detail-photos">${photoUrls
                        .map((url, index) => `<img class="map-tree-detail-photo" src="${url}" alt="${tree.SystemTreeNo || "樹木"} 照片 ${index + 1}" loading="lazy" data-index="${index}" />`)
                        .join("")}</div>`
                    : "<div class=\"map-note\">無照片</div>";

                treeListContent.innerHTML = `
                    <div class="map-tree-detail">
                        <div class="map-tree-detail-section">
                            <h4>照片</h4>
                            ${photoHtml}
                        </div>
                        <div class="map-tree-detail-section">
                            <h4>基本資訊</h4>
                            <dl class="map-tree-detail-grid">
                                <div class="map-tree-detail-item">
                                    <dt>樹種及學名</dt>
                                    <dd>${speciesDisplay}</dd>
                                </div>
                                <div class="map-tree-detail-item">
                                    <dt>數量</dt>
                                    <dd>${treeCount}</dd>
                                </div>
                                <div class="map-tree-detail-item">
                                    <dt>縣市鄉鎮</dt>
                                    <dd>${locationDisplay}</dd>
                                </div>
                                <div class="map-tree-detail-item">
                                    <dt>坐落地點</dt>
                                    <dd>${siteDisplay}</dd>
                                </div>
                                <div class="map-tree-detail-item">
                                    <dt>座標經緯度</dt>
                                    <dd>${latText}, ${lngText}</dd>
                                </div>
                            </dl>
                        </div>
                        <div class="d-grid">
                            <button class="btn btn-outline-primary map-tree-detail-toggle" type="button" id="treeDetailToggle" data-expanded="false">
                                顯示詳細資料
                            </button>
                        </div>
                        <div class="map-tree-detail-extra is-hidden">
                            <div class="map-tree-detail-section">
                                <h4>詳細資訊</h4>
                                <dl class="map-tree-detail-grid">
                                    <div class="map-tree-detail-item">
                                        <dt>管理人</dt>
                                        <dd>${managerDisplay}</dd>
                                    </div>
                                    <div class="map-tree-detail-item">
                                        <dt>樹籍狀態</dt>
                                        <dd>${statusDisplay}</dd>
                                    </div>
                                    <div class="map-tree-detail-item">
                                        <dt>公告日期</dt>
                                        <dd>${announcementDisplay}</dd>
                                    </div>
                                </dl>
                            </div>
                            <div class="map-tree-detail-section">
                                <h4>受保護認定理由</h4>
                                ${buildRecognitionList(tree.RecognitionReasonsHtml)}
                            </div>
                            <div class="map-tree-detail-section">
                                <h4>文化歷史價值介紹</h4>
                                <div>${culturalDisplay}</div>
                            </div>
                            <div class="map-tree-detail-section">
                                <h4>生長資訊</h4>
                                <dl class="map-tree-detail-grid">
                                    <div class="map-tree-detail-item">
                                        <dt>樹高</dt>
                                        <dd>${heightDisplay}</dd>
                                    </div>
                                    <div class="map-tree-detail-item">
                                        <dt>胸高直徑</dt>
                                        <dd>${diameterDisplay}</dd>
                                    </div>
                                    <div class="map-tree-detail-item">
                                        <dt>胸高樹圍</dt>
                                        <dd>${circumferenceDisplay}</dd>
                                    </div>
                                </dl>
                            </div>
                        </div>
                        <div class="d-grid">
                            <button class="btn btn-outline-primary" type="button" id="treeDetailBack">
                                返回清單
                            </button>
                        </div>
                    </div>
                `;
            };

            const getDistinctValues = (records, selector) => {
                const values = new Set();
                records.forEach((record) => {
                    const value = selector(record);
                    if (value) {
                        values.add(value);
                    }
                });
                return Array.from(values).sort((a, b) => a.localeCompare(b, "zh-Hant"));
            };

            const buildSelectOptions = (selectElement, values) => {
                selectElement.innerHTML = "";
                const defaultOption = document.createElement("option");
                defaultOption.value = "";
                defaultOption.textContent = "全部";
                selectElement.appendChild(defaultOption);
                values.forEach((value) => {
                    const option = document.createElement("option");
                    option.value = value;
                    option.textContent = value;
                    selectElement.appendChild(option);
                });
            };

            const refreshAreaOptions = () => {
                const selectedCity = citySelect.value;
                const areas = getDistinctValues(
                    treeData.filter((tree) => !selectedCity || tree.City === selectedCity),
                    (tree) => tree.Area
                );
                buildSelectOptions(areaSelect, areas);
            };

            const initializeFilters = () => {
                buildSelectOptions(citySelect, getDistinctValues(treeData, (tree) => tree.City));
                buildSelectOptions(speciesSelect, getDistinctValues(treeData, (tree) => tree.Species));
                refreshAreaOptions();
            };

            const applyFilters = () => {
                const selectedCity = citySelect.value;
                const selectedArea = areaSelect.value;
                const selectedSpecies = speciesSelect.value;
                const ageMin = parseNumber(ageMinInput.value);
                const ageMax = parseNumber(ageMaxInput.value);
                const diameterMin = parseNumber(diameterMinInput.value);
                const diameterMax = parseNumber(diameterMaxInput.value);
                const circumferenceMin = parseNumber(circumferenceMinInput.value);
                const circumferenceMax = parseNumber(circumferenceMaxInput.value);

                const filtered = treeData.filter((tree) => {
                    if (selectedCity && tree.City !== selectedCity) {
                        return false;
                    }
                    if (selectedArea && tree.Area !== selectedArea) {
                        return false;
                    }
                    if (selectedSpecies && tree.Species !== selectedSpecies) {
                        return false;
                    }

                    const ageValue = tree.Age !== null && tree.Age !== undefined ? Number(tree.Age) : null;
                    if (!matchesRange(ageValue, ageMin, ageMax)) {
                        return false;
                    }

                    const diameterValue = parseNumber(tree.BreastHeightDiameter);
                    if (!matchesRange(diameterValue, diameterMin, diameterMax)) {
                        return false;
                    }

                    const circumferenceValue = parseNumber(tree.BreastHeightCircumference);
                    if (!matchesRange(circumferenceValue, circumferenceMin, circumferenceMax)) {
                        return false;
                    }

                    return true;
                });

                const entries = renderTrees(filtered);
                updateTreeList(entries);
                setTreeListOpen(true);
            };

            if (citySelect && areaSelect && speciesSelect) {
                initializeFilters();
                citySelect.addEventListener("change", () => {
                    const currentArea = areaSelect.value;
                    refreshAreaOptions();
                    if (currentArea) {
                        areaSelect.value = currentArea;
                    }
                });
            }

            if (filterButton) {
                filterButton.addEventListener("click", applyFilters);
            }

            if (treeListContent) {
                treeListContent.addEventListener("click", (event) => {
                    const button = event.target.closest(".map-tree-locate");
                    if (!button) {
                        return;
                    }
                    const index = Number(button.dataset.index);
                    const entry = treeListEntries[index];
                    if (!entry) {
                        return;
                    }
                    view.goTo({ center: [entry.longitude, entry.latitude], zoom: 16 });
                    highlightTree(entry);
                    renderTreeDetail(entry);
                    if (treeListCount) {
                        treeListCount.textContent = entry.tree.SystemTreeNo || "樹木資料";
                    }
                    if (treeListPagination) {
                        treeListPagination.classList.add("is-hidden");
                    }
                });
            }

            if (treeListContent) {
                treeListContent.addEventListener("click", (event) => {
                    const backButton = event.target.closest("#treeDetailBack");
                    if (!backButton) {
                        return;
                    }
                    updateTreeList(treeListEntries);
                });
            }

            if (treeListContent) {
                treeListContent.addEventListener("click", (event) => {
                    const toggleButton = event.target.closest("#treeDetailToggle");
                    if (!toggleButton) {
                        return;
                    }
                    const detailSection = treeListContent.querySelector(".map-tree-detail-extra");
                    if (!detailSection) {
                        return;
                    }
                    const isExpanded = toggleButton.dataset.expanded === "true";
                    detailSection.classList.toggle("is-hidden", isExpanded);
                    toggleButton.dataset.expanded = isExpanded ? "false" : "true";
                    toggleButton.textContent = isExpanded ? "顯示詳細資料" : "收合詳細資料";
                });
            }

            if (treeListContent) {
                treeListContent.addEventListener("click", (event) => {
                    const photo = event.target.closest(".map-tree-detail-photo");
                    if (!photo) {
                        return;
                    }
                    const index = Number(photo.dataset.index || 0);
                    openLightbox(currentPhotoUrls, index);
                });
            }

            if (treeListPrev && treeListNext) {
                treeListPrev.addEventListener("click", () => {
                    if (treeListPage > 1) {
                        treeListPage -= 1;
                        updateTreeList(treeListEntries);
                    }
                });
                treeListNext.addEventListener("click", () => {
                    const totalPages = Math.max(1, Math.ceil(treeListEntries.length / treeListPageSize));
                    if (treeListPage < totalPages) {
                        treeListPage += 1;
                        updateTreeList(treeListEntries);
                    }
                });
            }

            if (treeListClose) {
                treeListClose.addEventListener("click", () => {
                    setTreeListOpen(false);
                });
            }

            if (photoLightbox) {
                photoLightbox.addEventListener("click", (event) => {
                    const thumb = event.target.closest(".photo-lightbox-thumb");
                    if (thumb && thumb.dataset.index) {
                        updateLightboxView(Number(thumb.dataset.index));
                        return;
                    }
                    if (event.target === photoLightbox) {
                        setLightboxOpen(false);
                    }
                });
            }

            if (photoLightboxClose) {
                photoLightboxClose.addEventListener("click", () => setLightboxOpen(false));
            }

            if (photoLightboxPrev) {
                photoLightboxPrev.addEventListener("click", () => {
                    updateLightboxView(currentPhotoIndex - 1);
                });
            }

            if (photoLightboxNext) {
                photoLightboxNext.addEventListener("click", () => {
                    updateLightboxView(currentPhotoIndex + 1);
                });
            }

            document.addEventListener("keydown", (event) => {
                if (event.key === "Escape" && photoLightbox && photoLightbox.classList.contains("is-open")) {
                    setLightboxOpen(false);
                }
            });

            renderTrees(treeData);
        });
    </script>
</asp:Content>
