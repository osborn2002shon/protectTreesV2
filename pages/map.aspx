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

        @media (max-width: 768px) {
            #mapView {
                height: calc(100vh - 200px);
            }

            .map-tools {
                left: 12px;
                right: 12px;
                width: auto;
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
                            <div class="mb-3">
                                <label class="form-label">縣市</label>
                                <select class="form-select">
                                    <option>臺北市</option>
                                    <option>新北市</option>
                                    <option>臺中市</option>
                                    <option>臺南市</option>
                                    <option>高雄市</option>
                                </select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">鄉鎮區</label>
                                <select class="form-select">
                                    <option>中正區</option>
                                    <option>信義區</option>
                                    <option>大安區</option>
                                </select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">樹種</label>
                                <select class="form-select">
                                    <option>黑板樹</option>
                                    <option>榕樹</option>
                                    <option>樟樹</option>
                                </select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">樹齡</label>
                                <div class="range-input-group">
                                    <input class="form-control" type="number" placeholder="最小值" />
                                    <span class="range-separator">~</span>
                                    <input class="form-control" type="number" placeholder="最大值" />
                                </div>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">胸高直徑 (m)</label>
                                <div class="range-input-group">
                                    <input class="form-control" type="number" placeholder="最小值" />
                                    <span class="range-separator">~</span>
                                    <input class="form-control" type="number" placeholder="最大值" />
                                </div>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">胸高樹圍 (m)</label>
                                <div class="range-input-group">
                                    <input class="form-control" type="number" placeholder="最小值" />
                                    <span class="range-separator">~</span>
                                    <input class="form-control" type="number" placeholder="最大值" />
                                </div>
                            </div>
                            <div class="d-grid">
                                <button class="btn btn-primary" type="button">查詢</button>
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
    </div>

    <script>
        const loadingElement = document.getElementById("mapLoading");

        require([
            "esri/Map",
            "esri/views/MapView",
            "esri/layers/WMTSLayer",
            "esri/widgets/Zoom"
        ], (Map, MapView, WMTSLayer, Zoom) => {
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
        });
    </script>
</asp:Content>
