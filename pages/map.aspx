<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_publicMap.Master" AutoEventWireup="true" CodeBehind="map.aspx.cs" Inherits="protectTreesV2.pages.map1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <link rel="stylesheet" href="https://js.arcgis.com/4.29/esri/themes/light/main.css" />
    <style>
        :root {
            --map-panel-bg: rgba(255, 255, 255, 0.9);
            --map-panel-border: rgba(148, 163, 184, 0.35);
            --map-panel-shadow: 0 18px 45px rgba(15, 23, 42, 0.15);
            --map-accent: #1d4ed8;
            --map-muted: #64748b;
        }

        #mapPage {
            position: relative;
            width: 100vw;
            margin-left: calc(50% - 50vw);
            background: #f8fafc;
        }

        #mapView {
            width: 100%;
            height: calc(100vh - 170px);
            min-height: 560px;
            margin:0;
        }

        .map-loading {
            position: absolute;
            inset: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: column;
            gap: 12px;
            background: rgba(248, 250, 252, 0.95);
            z-index: 30;
            transition: opacity 0.3s ease, visibility 0.3s ease;
        }

        .map-loading.is-hidden {
            opacity: 0;
            visibility: hidden;
        }

        .map-loading .spinner {
            width: 52px;
            height: 52px;
            border-radius: 50%;
            border: 5px solid rgba(37, 99, 235, 0.15);
            border-top-color: var(--map-accent);
            animation: spin 0.9s linear infinite;
        }

        .map-loading span {
            font-weight: 600;
            color: #1f2937;
            letter-spacing: 0.02em;
        }

        @keyframes spin {
            to {
                transform: rotate(360deg);
            }
        }

        .map-tools {
            position: absolute;
            top: 20px;
            left: 20px;
            width: min(360px, calc(100% - 40px));
            z-index: 20;
        }

        .map-tools .accordion-item {
            background: var(--map-panel-bg);
            border: 1px solid var(--map-panel-border);
            border-radius: 16px;
            overflow: hidden;
            box-shadow: var(--map-panel-shadow);
        }

        .map-tools .accordion-item + .accordion-item {
            margin-top: 14px;
        }

        .map-tools .accordion-button {
            background: transparent;
            font-weight: 600;
            color: #0f172a;
            padding: 16px 18px;
            box-shadow: none;
        }

        .map-tools .accordion-button:not(.collapsed) {
            color: var(--map-accent);
        }

        .map-tools .accordion-body {
            padding: 16px 18px 20px;
            color: #0f172a;
        }

        .map-tools label {
            font-weight: 600;
            color: #1f2937;
        }

        .map-tools .form-control,
        .map-tools .form-select {
            border-radius: 10px;
            border-color: rgba(148, 163, 184, 0.5);
        }

        .map-tools .form-control:focus,
        .map-tools .form-select:focus {
            border-color: var(--map-accent);
            box-shadow: 0 0 0 0.2rem rgba(29, 78, 216, 0.15);
        }

        .map-tools .btn-primary {
            background: var(--map-accent);
            border-color: var(--map-accent);
        }

        .map-tools .btn-outline-secondary {
            border-radius: 10px;
        }

        .map-tools .range-input-group {
            display: grid;
            grid-template-columns: 1fr auto 1fr;
            gap: 8px;
            align-items: center;
        }

        .map-tools .range-separator {
            color: var(--map-muted);
            font-weight: 600;
        }

        .map-tools .map-filter-scroll {
            max-height: 200px;
            overflow-y: auto;
            padding-right: 6px;
        }

        .map-pill {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 6px 12px;
            border-radius: 999px;
            background: rgba(29, 78, 216, 0.1);
            color: var(--map-accent);
            font-weight: 600;
            font-size: 0.85rem;
        }

        .map-note {
            font-size: 0.85rem;
            color: var(--map-muted);
            line-height: 1.5;
        }

        .map-tree-panel {
            position: absolute;
            top: 20px;
            right: 20px;
            width: min(360px, calc(100% - 40px));
            max-height: calc(100vh - 220px);
            background: var(--map-panel-bg);
            border: 1px solid var(--map-panel-border);
            border-radius: 18px;
            box-shadow: var(--map-panel-shadow);
            z-index: 20;
            display: flex;
            flex-direction: column;
            transition: opacity 0.2s ease, transform 0.2s ease, visibility 0.2s ease;
        }

        .map-tree-panel.is-hidden {
            opacity: 0;
            visibility: hidden;
            transform: translateX(16px);
            pointer-events: none;
        }

        .map-tree-panel .is-hidden {
            display: none;
        }

        .map-tree-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 16px 18px 10px;
            border-bottom: 1px solid rgba(148, 163, 184, 0.25);
        }

        .map-tree-header h3 {
            margin: 0;
            font-size: 1rem;
            font-weight: 600;
            color: #0f172a;
        }

        .map-tree-count {
            font-size: 0.85rem;
            color: var(--map-muted);
        }

        .map-tree-close {
            border: none;
            background: transparent;
            color: var(--map-muted);
            font-size: 1.1rem;
            padding: 4px;
        }

        .map-tree-close:hover {
            color: var(--map-accent);
        }

        .map-tree-content {
            overflow-y: auto;
            padding: 12px 16px 18px;
            display: block;
        }

        .map-tree-detail {
            display: grid;
            gap: 14px;
        }

        .map-tree-detail-section {
            border: 1px solid rgba(148, 163, 184, 0.25);
            border-radius: 12px;
            padding: 12px 14px;
            background: rgba(248, 250, 252, 0.7);
        }

        .map-tree-detail-section h4 {
            margin: 0 0 10px;
            font-size: 0.95rem;
            font-weight: 600;
            color: #1f2937;
        }

        .map-tree-detail-grid {
            display: grid;
            gap: 10px;
        }

        .map-tree-detail-item {
            display: grid;
            grid-template-columns: 110px 1fr;
            gap: 8px;
            font-size: 0.95rem;
        }

        .map-tree-detail-item dt {
            margin: 0;
            font-weight: 600;
            color: #1f2937;
        }

        .map-tree-detail-item dd {
            margin: 0;
            color: #0f172a;
            word-break: break-word;
        }

        .map-tree-detail-item dd .map-note {
            margin: 0;
        }

        .map-tree-detail-photo {
            width: 100%;
            border-radius: 12px;
            border: 1px solid rgba(148, 163, 184, 0.3);
            object-fit: cover;
            max-height: 220px;
        }

        .map-tree-detail-criteria {
            padding-left: 18px;
            margin: 0;
        }

        .map-tree-detail-criteria li {
            margin-bottom: 6px;
            line-height: 1.4;
        }

        .map-tree-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 1rem;
            color: #0f172a;
        }

        .map-tree-table th,
        .map-tree-table td {
            padding: 8px 10px;
            border-bottom: 1px solid rgba(148, 163, 184, 0.25);
            vertical-align: middle;
        }

        .map-tree-table th {
            font-weight: 600;
            color: #1e293b;
            text-align: left;
            background: rgba(226, 232, 240, 0.6);
            position: sticky;
            top: 0;
            z-index: 1;
        }

        .map-tree-table tbody tr:hover {
            background: rgba(226, 232, 240, 0.35);
        }

        .map-tree-locate {
            border-radius: 999px;
            padding: 4px 10px;
            font-size: 0.8rem;
            white-space: nowrap;
        }

        .map-tree-pagination {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 10px 16px 14px;
            border-top: 1px solid rgba(148, 163, 184, 0.2);
            gap: 8px;
        }

        .map-tree-pagination .btn {
            border-radius: 999px;
            padding: 4px 12px;
            font-size: 0.85rem;
        }

        .map-tree-pagination .map-page-info {
            font-size: 0.85rem;
            color: var(--map-muted);
        }

        @media (max-width: 768px) {
            #mapView {
                height: calc(100vh - 200px);
            }

            .map-tools {
                left: 12px;
                right: 12px;
                width: auto;
            }

            .map-tree-panel {
                right: 12px;
                left: 12px;
                width: auto;
                max-height: calc(100vh - 240px);
            }
        }
    </style>
    <script src="https://js.arcgis.com/4.29/"></script>
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
                                <button class="btn btn-primary" type="button" id="btnLocate">座標定位</button>
                                <button class="btn btn-outline-secondary" type="button" id="btnGeolocate">目前位置</button>
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
                                    <select id="filterSpecies" class="form-select"></select>
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
                            <div class="mb-3">
                                <span class="map-pill"><i class="fa-solid fa-map"></i> 目前底圖</span>
                            </div>
                            <select id="basemapSelect" class="form-select mb-3">
                                <option value="NLSC_1">臺灣通用版電子地圖</option>
                                <option value="NLSC_3">臺灣通用版正射影像</option>
                                <option value="streets">ArcGIS 街道圖</option>
                                <option value="topo-vector">ArcGIS 地形圖</option>
                                <option value="hybrid">ArcGIS 衛星影像</option>
                            </select>
                            <div class="map-note">可切換政府圖資或 ArcGIS 底圖，適合不同查詢需求。</div>
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
    </div>
    <asp:HiddenField ID="TreeDataJson" runat="server" />

    <script>
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
            map.add(graphicsLayer);

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
                url: "/_img/icon/pin.png",
                width: "30px",
                height: "30px"
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
            let treeListEntries = [];
            let treeListPage = 1;
            const treeListPageSize = 10;

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
                const photoUrl = normalizePhotoUrl(tree.PhotoUrl);

                treeListContent.innerHTML = `
                    <div class="map-tree-detail">
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
                        <div class="map-tree-detail-section">
                            <h4>照片</h4>
                            ${photoUrl ? `<img class="map-tree-detail-photo" src="${photoUrl}" alt="${tree.SystemTreeNo || "樹木"}" />` : "<div class=\"map-note\">無照片</div>"}
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
                    view.popup.open({
                        features: [entry.graphic],
                        location: { latitude: entry.latitude, longitude: entry.longitude }
                    });
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

            renderTrees(treeData);
        });
    </script>
</asp:Content>
