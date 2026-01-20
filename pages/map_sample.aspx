<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_publicMap.Master" AutoEventWireup="true" CodeBehind="map_sample.aspx.cs" Inherits="protectTreesV2.pages.map" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style type="text/css">
        #viewDiv {
            /*margin:0 auto;*/
            /*height:calc(100vh - 210px);*/
            /*width:100vw;*/
            height: calc(100vh - 93px);
            /*width:100%;*/
        }

        .stickySave {
            position: fixed;
            background-color: rgba(255, 255, 255, 0.9);
            width: 270px;
            border: 5px solid #000;
            border-top-left-radius: 15px;
            border-bottom-left-radius: 15px;
            box-shadow: 10px 5px 5px rgba(0, 0, 0, 0.3);
            top: 150px;
            /*right: -220px;*/
            right: -5px;
            text-align: center;
            z-index: 99999;
            padding: 10px 15px;
        }
    </style>
    <link rel="stylesheet" href="https://js.arcgis.com/4.24/esri/themes/light/main.css" />
    <script src="https://js.arcgis.com/4.24/"></script>
    <script>
        //$(document).ready(function () {
        //    $(".stickySave").hover(
        //        function () {
        //            // mouse enter
        //            $(this).stop().animate({
        //                right: '-5px'
        //            }, 500); //0.5秒
        //        },
        //        function () {
        //            // mouse leave
        //            $(this).stop().animate({
        //                right: '-220px'
        //            }, 500); //0.5秒
        //        }
        //    );
        //});
    </script>
    <script>
        var proxy = "https://www.i-forcetech.com/proxy/proxy.ashx?";
        require([
            "esri/Map",
            "esri/views/SceneView",
            "esri/views/MapView",
            "esri/layers/WMSLayer",
            "esri/layers/WMTSLayer",
            "esri/layers/TileLayer",
            "esri/layers/MapImageLayer",
            "esri/widgets/LayerList",
            "esri/widgets/Zoom",
            "esri/Graphic",
            "esri/layers/GraphicsLayer"], (
                Map,
                SceneView,
                MapView,
                WMSLayer,
                WMTSLayer,
                TileLayer,
                MapImageLayer,
                LayerList,
                Zoom,
                Graphic, GraphicsLayer
            ) => {

            //https://maps.nlsc.gov.tw/S09SOA/
            var NLSC_1 = new WMTSLayer({
                url: "https://wmts.nlsc.gov.tw/wmts",
                activeLayer: { id: "EMAP", },
                title: '通用版電子地圖'
            });

            var NLSC_3 = new WMTSLayer({
                url: "https://wmts.nlsc.gov.tw/wmts",
                activeLayer: { id: "PHOTO2" },
                title: '正射影像圖(臺灣通用)'
            });

            var NLSC_4 = new WMTSLayer({
                url: "https://wmts.nlsc.gov.tw/wmts",
                activeLayer: { id: "CITY" },
                title: '縣市界'
            });

            var NLSC_5 = new WMTSLayer({
                url: "https://wmts.nlsc.gov.tw/wmts",
                activeLayer: { id: "TOWN" },
                title: '鄉鎮區界',
                minScale: 500000,
                maxScale: 1,
            });

            map = new Map({
                basemap: "satellite"
            });

            map.add(NLSC_1, 1); //正射影像圖(臺灣通用)
            map.add(NLSC_3, 1); //正射影像圖(臺灣通用)
            /*
            map.add(NLSC_4, 8); //縣市界
            map.add(NLSC_5, 9); //鄉鎮區界
            */

            // 隱藏自定義圖層，等用戶選擇後顯示
            //NLSC_1.visible = false;
            NLSC_3.visible = false;

            view = new MapView({
                container: "viewDiv",
                map: map,
                zoom: 18,
                center: [121.50995183094452, 25.031511865096132],
                ui: {
                    components: ["attribution"] // removes default widgets except for attribution
                }
            });

            /* 把ZOOM換位置 + 上面除了要引用widgets，也記得要先移除原本的widgets */
            var zoom = new Zoom({
                view: view,
                layout: "vertical"
            });
            view.ui.add(zoom, "bottom-right");


            view.when(function () {
                //view.goTo(extent);
                layerList = new LayerList({
                    view: view,
                    container: 'layerList'
                });
            });

            /* 單一個點 */
            //const simpleMarkerSymbol = {
            //    type: "simple-marker",
            //    color: [255, 32, 80],  // Orange
            //    outline: {
            //        color: [255, 255, 255], // White
            //        width: 1
            //    }
            //};
            //const point = { //Create a point
            //    type: "point",
            //    longitude: 121.50995183094452,
            //    latitude: 25.031511865096132
            //};
            //const pointGraphic = new Graphic({
            //    geometry: point,
            //    symbol: simpleMarkerSymbol
            //});
            //view.graphics.add(pointGraphic);

            // 創建 GraphicsLayer 並添加到地圖
            const graphicsLayer = new GraphicsLayer();
            map.add(graphicsLayer);

            // 定義多個記號的位置
            const locations = [
                { longitude: 121.50980, latitude: 25.03140, name: "A12345" },
                { longitude: 121.50996, latitude: 25.03144, name: "A12346" },
                { longitude: 121.51015, latitude: 25.03149, name: "A12347" }
            ];
            // 為每個位置創建 Graphic
            locations.forEach(location => {
                const point = {
                    type: "point",
                    longitude: location.longitude,
                    latitude: location.latitude
                };

                const markerSymbol = {
                    type: "simple-marker",
                    color: [255, 0, 0],
                    outline: {
                        color: [255, 255, 255],
                        width: 1
                    }
                };

                //const markerSymbol = {
                //    type: "picture-marker", // 使用圖片標記
                //    url: "../_img/pin.png", // 圖片的 URL
                //    width: "24px", // 設定圖片寬度
                //    height: "24px" // 設定圖片高度
                //};

                const pointGraphic = new Graphic({
                    geometry: point,
                    symbol: markerSymbol,
                    attributes: {
                        name: location.name
                    }
                });

                // 添加到 GraphicsLayer
                graphicsLayer.add(pointGraphic);
            });

            // 添加點擊事件來觸發 Bootstrap 的 modal
            view.on("click", function (event) {
                view.hitTest(event).then(function (response) {
                    const graphic = response.results[0]?.graphic;
                    if (graphic) {
                        // 將位置信息添加到 modal 中
                        document.getElementById("locationInfo").textContent = graphic.attributes.name;

                        // 顯示 Bootstrap modal
                        const myModal = new bootstrap.Modal(document.getElementById("exampleModal"));
                        myModal.show();
                    }
                });
            });

            // 切換底圖的函數
            function switchBasemap() {
                const selectedBasemap = document.getElementById("basemapDropdown").value;
                //根據選取的值切換內建底圖
                //map.basemap = selectedBasemap; 

                // 如果選擇的是預設底圖
                if (["satellite", "topo", "streets"].includes(selectedBasemap)) {
                    map.basemap = selectedBasemap; // 切換底圖
                    NLSC_1.visible = false;
                    NLSC_3.visible = false;
                } else if (selectedBasemap === "NLSC_1") {
                    //map.basemap = "satellite"; // 保留主要底圖
                    NLSC_1.visible = true;
                    NLSC_3.visible = false;
                } else if (selectedBasemap === "NLSC_3") {
                    //map.basemap = "satellite"; // 保留主要底圖
                    NLSC_1.visible = false;
                    NLSC_3.visible = true;
                }
            }
            // 綁定按鈕事件
            document.getElementById("switchBasemapBtn").addEventListener("click", switchBasemap);

        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_map" runat="server">
    <div class="row p-0 m-0" style="background-color: #e1e1e1;">
        <div class="col-2 p-2">
            <div class="accordion" id="accordionExample">
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapse0" aria-expanded="true" aria-controls="collapseZero">
                            <i class="bi bi-geo-alt me-2"></i>地圖定位
                        </button>
                    </h2>
                    <div id="collapse0" class="accordion-collapse collapse show" data-bs-parent="#accordionExample">
                        <div class="accordion-body">
                            <%--1. TOGS門牌定位
                            2. 經緯度座標定位
                            3. 目前位置--%>
                            <!-- 座標系統選擇 -->
                            <div class="form-group mb-2" style="display:none;">
                                <label><strong>座標系統</strong></label>
                                <div class="radio-group">
                                    <label>
                                        <input type="radio" name="system" value="WGS84" checked> WGS84
                                    </label>
                                    <label>
                                        <input type="radio" name="system" value="TWD97"> TWD97
                                    </label>
                                </div>
                                <div class="info-box" id="systemInfo" style="font-size:0.9rem;color:silver;display:none">
                                    WGS84 (世界大地測量系統)<br>
                                    格式: 十進制度數<br>
                                    範例: 緯度 25.033611, 經度 121.565000 (台北101)
                                </div>
                            </div>

                            <!-- 座標輸入 -->
                            <div class="form-group mb-2">
                                <label for="latitude" id="coord1Label"><strong>緯度</strong></label>
                                <input type="number" id="latitude" step="any" placeholder="例：25.033611" class="form-control">
                            </div>

                            <div class="form-group mb-2">
                                <label for="longitude" id="coord2Label"><strong>經度</strong></label>
                                <input type="number" id="longitude" step="any" placeholder="例：121.565000" class="form-control">
                            </div>

                            <!-- 按鈕 -->
                            <div class="row mb-2">
                                <div class="col">
                                    <button class="form-control btn btn-primary" onclick="searchCoordinates()">座標定位</button>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                   <button class="form-control btn btn-warning" onclick="getCurrentLocation()">目前位置</button>
                                </div>
                            </div>
                            <button class="btn-convert" onclick="convertCoordinates()" style="display:none">
                                🔄 座標轉換
                            </button>

                            <!-- 目前位置顯示 -->
                            <div class="current-location" id="locationInfo2" style="display:none">
                                <strong>目前位置：</strong><br>
                                <span id="currentCoords"></span>
                            </div>

                            <!-- 轉換結果顯示 -->
                            <div class="current-location" id="convertResult" style="display:none">
                                <strong>轉換結果：</strong><br>
                                <span id="convertedCoords"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse1" aria-expanded="false" aria-controls="collapseOne">
                            <i class="bi bi-search me-2"></i>篩選條件
                        </button>
                    </h2>
                    <div id="collapse1" class="accordion-collapse collapse" data-bs-parent="#accordionExample">
                        <div class="accordion-body">
                            <div class="row mb-2">
                                <div class="col">
                                    <strong>縣市</strong>
                                    <asp:DropDownList ID="DropDownList3" runat="server" CssClass="form-select form-floating">
                                        <asp:ListItem Selected="True">台北市</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row mb-2">
                                <div class="col-12">
                                    <strong>鄉鎮</strong>
                                    <asp:DropDownList ID="DropDownList4" runat="server" CssClass="form-select">
                                        <asp:ListItem Selected="True">中正區</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row mb-2">
                                <div class="col-12">
                                    <strong>樹種</strong>
                                    <asp:DropDownList ID="DropDownList2" runat="server" CssClass="form-select">
                                        <asp:ListItem>黑板樹</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row mb-2">
                                <div class="col-12">
                                    <strong>樹齡</strong>
                                    <div class="range-input-group">
                                        <input type="text" class="form-control range-input" placeholder="最小值">
                                        <span class="range-separator">~</span>
                                        <input type="text" class="form-control range-input" placeholder="最大值">
                                    </div>
                                </div>
                            </div>
                            <div class="row mb-2">
                                <div class="col-12">
                                    <strong>胸高直徑(m)</strong>
                                    <div class="range-input-group">
                                        <input type="text" class="form-control range-input" placeholder="最小值">
                                        <span class="range-separator">~</span>
                                        <input type="text" class="form-control range-input" placeholder="最大值">
                                    </div>
                                </div>
                            </div>
                            <div class="row mb-2">
                                <div class="col-12">
                                    <strong>胸高樹圍(m)</strong>
                                    <div class="range-input-group">
                                        <input type="text" class="form-control range-input" placeholder="最小值">
                                        <span class="range-separator">~</span>
                                        <input type="text" class="form-control range-input" placeholder="最大值">
                                    </div>
                                </div>
                            </div>
                            <div class="row mb-2">
                                <div class="col">
                                    <input type="button" value="查詢" class="form-control btn btn-primary" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <input type="button" value="列表" class="form-control btn btn-warning" data-bs-toggle="modal" data-bs-target="#exampleModal_2" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse2" aria-expanded="false" aria-controls="collapseTwo">
                            <i class="bi bi-layers-half me-2"></i>底圖設定
                        </button>
                    </h2>
                    <div id="collapse2" class="accordion-collapse collapse" data-bs-parent="#accordionExample">
                        <div class="accordion-body">
                            <select id="basemapDropdown" class="form-select form-floating mb-2">
                                <option value="NLSC_1">臺灣通用版電子地圖</option>
                                <option value="NLSC_3">臺灣通用版正射影像</option>
                                <%--<option value="satellite">ArcGIS預設衛星圖</option>
                                <option value="topo">ArcGIS預設地形圖</option>
                                <option value="streets">ArcGIS預設街道圖</option>--%>
                            </select>
                            <input type="button" value="切換" class="form-control btn btn-primary" id="switchBasemapBtn" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-10" id="viewDiv"></div>
    </div>
    <div class="stickySave" style="display: none">
        ...
    </div>

    <div class="modal fade" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">樹籍編號 <span id="locationInfo"></span></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-4">
                            <img src="../../_img/olive-tree-3579922_1280.jpg" class="w-100" /></div>
                        <div class="col-4">
                            <img src="../../_img/olive-tree-3579922_1280.jpg" class="w-100" /></div>
                        <div class="col-4">
                            <img src="../../_img/olive-tree-3579922_1280.jpg" class="w-100" /></div>
                    </div>
                    <%--<div>※示意用照片非黑板樹。</div>--%>
                    <table class="table">
                        <tr>
                            <th>樹籍編號</th>
                            <td>A12345</td>
                            <th>公告日期</th>
                            <td>2020/05/08</td>
                            <th>數量</th>
                            <td>1</td>
                        </tr>
                        <tr>
                            <th>縣市</th>
                            <td colspan="2">臺北市</td>
                            <th>鄉鎮</th>
                            <td colspan="2">中山區</td>
                        </tr>
                        <tr>
                            <th>坐落地點</th>
                            <td colspan="5">臺北市中山區長安東路二段169-6號</td>
                        </tr>
                        <tr>
                            <th>樹種</th>
                            <td>黑板樹</td>
                            <th>學名</th>
                            <td colspan="4" style="font-style: italic">Alstonia scholaris</td>
                        </tr>
                        <tr>
                            <th>樹高</th>
                            <td>0.82公尺</td>
                            <th>胸高直徑</th>
                            <td>0.82公尺</td>
                            <th>胸高樹圍</th>
                            <td>2.57公尺</td>
                        </tr>
                        <tr>
                            <th>推估種植年間</th>
                            <td colspan="2">--</td>
                            <th>推估樹齡備註</th>
                            <td colspan="2">100</td>
                        </tr>
                        <%--<tr>
                            <th>管理人</th>
                            <td colspan="5">大武科技股份有限公司</td>
                        </tr>
                        <tr>
                            <th>認定理由</th>
                            <td colspan="5">樹齡達一百年以上、為區域具地理上代表性樹木</td>
                        </tr>
                        <tr>
                            <th>文化歷史價值介紹</th>
                            <td colspan="5">黑板樹又稱象皮樹、燈架樹、糖膠樹、乳木、魔神樹等，為夾竹桃科雞骨常山屬的模式種。<br />
                                常綠大喬木，其樹體高大，為具有明顯主幹的喬木，株高15~25公尺，樹冠傘蓋狀，樹幹挺直灰褐色、枝輪生、具白色乳液。<br />
                                葉序為單葉輪生，小葉4~10枚，葉面深綠、葉背淺白色。花是黃白色，花冠徑8~12公分。
                            </td>
                        </tr>--%>
                        <%--<tr>
                            <th>健檢中心建議處理優先順序</th>
                            <td colspan="5">緊急處理（2025/03/16）</td>
                        </tr>--%>
                    </table>
                    <div style="width:130px;margin:0 auto;padding:10px 20px; background-color:antiquewhite;border-radius:5px;text-align:center;">
                        查看更多
                    </div>
                </div>
                <div class="modal-footer">
                    以上資訊由各主管機關自行維護，如有疑義請可洽<a href="http://localhost:46559/Backstage/Prototype_public/About.aspx#block3" target="_blank">機關</a>聯繫。
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="exampleModal_2" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel_2">查詢結果 共11筆</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="table-wrapper">
                        <table class="tb_modal">
                            <thead>
                                <tr>
                                    <th>樹籍編號</th>
                                    <th>縣市</th>
                                    <th>鄉鎮</th>
                                    <th>樹種</th>
                                    <th>定位</th>
                                    <%--<th>樹齡</th>
                                    <th>胸高直徑(m)</th>
                                    <th>胸高樹圍(m)</th>--%>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>A0001</td>
                                    <td>台北市</td>
                                    <td>中正區</td>
                                    <td>榕樹</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--二二八和平公園內北側步道旁--%></td>
                                    <%--<td>120年</td>
                                    <td>1.8</td>
                                    <td>5.65</td>--%>
                                </tr>
                                <tr>
                                    <td>A0002</td>
                                    <td>台北市</td>
                                    <td>大安區</td>
                                    <td>樟樹</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--大安森林公園音樂台前廣場--%></td>
                                    <%--<td>85年</td>
                                    <td>1.5</td>
                                    <td>4.71</td>--%>
                                </tr>
                                <tr>
                                    <td>F0003</td>
                                    <td>新北市</td>
                                    <td>板橋區</td>
                                    <td>茄苳</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--板橋435藝文特區兒童玩具博物館前--%></td>
                                    <%--<td>150年</td>
                                    <td>2.1</td>
                                    <td>6.59</td>--%>
                                </tr>
                                <tr>
                                    <td>H0004</td>
                                    <td>桃園市</td>
                                    <td>中壢區</td>
                                    <td>龍眼</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--中壢區公所對面中正公園內--%></td>
                                    <%--<td>95年</td>
                                    <td>1.3</td>
                                    <td>4.08</td>--%>
                                </tr>
                                <tr>
                                    <td>B0005</td>
                                    <td>台中市</td>
                                    <td>西屯區</td>
                                    <td>榕樹</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--逢甲夜市文華路入口廣場--%></td>
                                    <%--<td>110年</td>
                                    <td>1.9</td>
                                    <td>5.97</td>--%>
                                </tr>
                                <tr>
                                    <td>D0006</td>
                                    <td>台南市</td>
                                    <td>中西區</td>
                                    <td>相思樹</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--赤崁樓文昌閣後方庭園--%></td>
                                    <%--<td>180年</td>
                                    <td>2.3</td>
                                    <td>7.22</td>--%>
                                </tr>
                                <tr>
                                    <td>E0007</td>
                                    <td>高雄市</td>
                                    <td>三民區</td>
                                    <td>樟樹</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--高雄火車站前廣場東側綠地--%></td>
                                    <%--<td>75年</td>
                                    <td>1.4</td>
                                    <td>4.40</td>--%>
                                </tr>
                                <tr>
                                    <td>G0008</td>
                                    <td>宜蘭縣</td>
                                    <td>宜蘭市</td>
                                    <td>茄苳</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--宜蘭運動公園游泳池旁步道--%></td>
                                    <%--<td>130年</td>
                                    <td>1.7</td>
                                    <td>5.34</td>--%>
                                </tr>
                                <tr>
                                    <td>U0009</td>
                                    <td>花蓮縣</td>
                                    <td>花蓮市</td>
                                    <td>榕樹</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--花蓮文化創意產業園區主建築前--%></td>
                                    <%--<td>140年</td>
                                    <td>2.0</td>
                                    <td>6.28</td>--%>
                                </tr>
                                <tr>
                                    <td>V0010</td>
                                    <td>台東縣</td>
                                    <td>台東市</td>
                                    <td>龍眼</td>
                                    <td><i class="bi bi-geo-alt"></i> <%--台東森林公園黑森林步道中段--%></td>
                                    <%--<td>105年</td>
                                    <td>1.6</td>
                                    <td>5.03</td>--%>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="text-center mt-2">
                        第 <select style="width:50px;text-align:center">
                            <option value="value">1</option>
                            <option value="value">2</option>
                        </select> 頁
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    
    
    <style>
        .accordion-body {
            font-size: 1rem;
        }

        .tb_modal {
            width: 100%;
            border-collapse: collapse;
            /*font-size: 14px;*/
            background: white;
        }

        .tb_modal th {
            background: #42593f;
            color: white;
            padding: 12px 10px;
            text-align: center;
            font-weight: 600;
            border: none;
            /*font-size: 13px;*/
        }

        .tb_modal td {
            padding: 10px;
            text-align: center;
            border-bottom: 1px solid #e9ecef;
            vertical-align: middle;
        }

        .tb_modal tr:nth-child(even) {
            background-color: #f8f9fa;
        }

        .tb_modal tr:hover {
            background-color: rgba(66, 89, 63, 0.1);
            transition: background-color 0.3s ease;
        }

        /* 響應式設計 */
        @media (max-width: 768px) {
            .tb_modal {
                font-size: 12px;
            }

            .tb_modal th, .tb_modal td {
                padding: 8px 6px;
            }
        }

        /* 表格滾動 */
        .table-wrapper {
            overflow-x: auto;
        }
    </style>

    <script>
        // 座標系統資訊
        const systemInfo = {
            WGS84: 'WGS84 (世界大地測量系統)<br>格式: 十進制度數 (經緯度)<br>範例: 緯度 25.033611, 經度 121.565000 (台北101)',
            TWD97: 'TWD97 (台灣大地基準)<br>格式: TM2度分帶投影座標 (公尺)<br>範例: X: 306173, Y: 2770285 (台北101)'
        };

        // TWD97 轉換參數 (台灣地區近似參數)
        const TWD97_PARAMS = {
            a: 6378137.0,           // 長半軸
            f: 1 / 298.257223563,     // 扁率
            e2: 0.00669438002290,   // 第一偏心率平方
            k0: 0.9999,             // 比例因子
            lon0: 121 * Math.PI / 180,  // 中央經線 121度
            lat0: 0,                // 原點緯度
            x0: 250000,             // 東偏移量
            y0: 0                   // 北偏移量
        };

        // 監聽座標系統選擇
        document.querySelectorAll('input[name="system"]').forEach(radio => {
            radio.addEventListener('change', function () {
                const newSystem = this.value;
                const val1 = parseFloat(document.getElementById('latitude').value);
                const val2 = parseFloat(document.getElementById('longitude').value);

                // 如果有輸入座標值，進行自動轉換
                if (!isNaN(val1) && !isNaN(val2) && val1 && val2) {
                    autoConvertCoordinates(newSystem, val1, val2);
                }

                // 更新介面
                document.getElementById('systemInfo').innerHTML = systemInfo[newSystem];
                updateInputLabels(newSystem);

                // 清除轉換結果顯示
                document.getElementById('convertResult').style.display = 'none';
            });
        });

        // 自動轉換座標
        function autoConvertCoordinates(targetSystem, val1, val2) {
            let convertedCoords;

            if (targetSystem === 'TWD97') {
                // 從 WGS84 轉到 TWD97
                convertedCoords = wgs84ToTwd97(val1, val2);
                document.getElementById('latitude').value = convertedCoords.x;
                document.getElementById('longitude').value = convertedCoords.y;
            } else {
                // 從 TWD97 轉到 WGS84
                convertedCoords = twd97ToWgs84(val1, val2);
                document.getElementById('latitude').value = convertedCoords.lat;
                document.getElementById('longitude').value = convertedCoords.lon;
            }

            // 顯示轉換提示
            showAutoConvertMessage(targetSystem);
        }

        // 顯示自動轉換提示
        function showAutoConvertMessage(targetSystem) {
            const message = '已自動轉換為 ' + targetSystem + ' 格式';

            // 創建提示元素
            const alertDiv = document.createElement('div');
            alertDiv.innerHTML = '✅ ' + message;
            alertDiv.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                background: #28a745;
                color: white;
                padding: 10px 15px;
                border-radius: 5px;
                z-index: 1000;
                font-size: 14px;
                box-shadow: 0 2px 5px rgba(0,0,0,0.2);
            `;

            document.body.appendChild(alertDiv);

            // 3秒後自動移除
            setTimeout(() => {
                if (alertDiv.parentNode) {
                    alertDiv.parentNode.removeChild(alertDiv);
                }
            }, 3000);
        }

        // 更新輸入欄位標籤
        function updateInputLabels(system) {
            const coord1Label = document.getElementById('coord1Label');
            const coord2Label = document.getElementById('coord2Label');
            const latInput = document.getElementById('latitude');
            const lngInput = document.getElementById('longitude');

            if (system === 'WGS84') {
                coord1Label.innerHTML = '<strong>緯度</strong>';
                coord2Label.innerHTML = '<strong>經度</strong>';
                latInput.placeholder = '例：25.033611';
                lngInput.placeholder = '例：121.565000';
            } else {
                coord1Label.innerHTML = '<strong>X座標(公尺)</strong>';
                coord2Label.innerHTML = '<strong>Y座標(公尺)</strong>';
                latInput.placeholder = '例：306173';
                lngInput.placeholder = '例：2770285';
            }
        }

        // WGS84 經緯度轉 TWD97 TM2 座標
        function wgs84ToTwd97(lat, lng) {
            const deg2rad = Math.PI / 180;
            const a = 6378137.0;           // 長半軸
            const f = 1 / 298.257223563;     // 扁率
            const k0 = 0.9999;             // 比例因子
            const lon0 = 121 * deg2rad;    // 中央經線 121度
            const x0 = 250000;             // 東偏移量
            const y0 = 0;                  // 北偏移量

            lat = lat * deg2rad;
            lng = lng * deg2rad;

            const e2 = 2 * f - f * f;
            const e = Math.sqrt(e2);
            const n = f / (2 - f);

            const A = a * (1 - n + (5 / 4) * (n * n - n * n * n) + (81 / 64) * (Math.pow(n, 4) - Math.pow(n, 5)));
            const B = (3 * a * n / 2) * (1 - n + (7 / 8) * (n * n - n * n * n) + (55 / 64) * Math.pow(n, 4));
            const C = (15 * a * n * n / 16) * (1 - n + (3 / 4) * (n * n - n * n * n));
            const D = (35 * a * n * n * n / 48) * (1 - n + (11 / 16) * n * n);
            const E = (315 * a * Math.pow(n, 4) / 512) * (1 - n);

            const S = A * lat - B * Math.sin(2 * lat) + C * Math.sin(4 * lat) - D * Math.sin(6 * lat) + E * Math.sin(8 * lat);

            const nu = a / Math.sqrt(1 - e2 * Math.sin(lat) * Math.sin(lat));
            const p = lng - lon0;
            const A1 = S * k0;
            const A2 = nu * Math.sin(lat) * Math.cos(lat) * k0 / 2;
            const A3 = nu * Math.sin(lat) * Math.pow(Math.cos(lat), 3) * k0 * (5 - Math.pow(Math.tan(lat), 2) + 9 * e2 * Math.pow(Math.cos(lat), 2)) / 24;
            const A4 = nu * Math.cos(lat) * k0;
            const A5 = nu * Math.pow(Math.cos(lat), 3) * k0 * (1 - Math.pow(Math.tan(lat), 2) + e2 * Math.pow(Math.cos(lat), 2)) / 6;

            const x = x0 + A4 * p + A5 * Math.pow(p, 3);
            const y = y0 + A1 + A2 * p * p + A3 * Math.pow(p, 4);

            return { x: Math.round(x), y: Math.round(y) };
        }

        // TWD97 TM2 座標轉 WGS84 經緯度 (簡化版本)
        function twd97ToWgs84(x, y) {
            // 這是簡化的反算公式，實際專案建議使用 proj4js
            const deg2rad = Math.PI / 180;
            const rad2deg = 180 / Math.PI;
            const a = 6378137.0;
            const f = 1 / 298.257223563;
            const k0 = 0.9999;
            const lon0 = 121;
            const x0 = 250000;
            const y0 = 0;

            // 簡化的迭代反算
            let lat = y / (a * k0) * rad2deg;
            let lng = (x - x0) / (a * k0) * rad2deg + lon0;

            // 基本修正
            lat = lat + 0.0002;
            lng = lng - 0.0001;

            return { lat: parseFloat(lat.toFixed(6)), lon: parseFloat(lng.toFixed(6)) };
        }

        // 座標轉換功能
        function convertCoordinates() {
            const val1 = parseFloat(document.getElementById('latitude').value);
            const val2 = parseFloat(document.getElementById('longitude').value);
            const currentSystem = document.querySelector('input[name="system"]:checked').value;

            if (!val1 || !val2 || isNaN(val1) || isNaN(val2)) {
                alert('請先輸入有效的座標值');
                return;
            }

            let convertedCoords;
            let targetSystem;
            let resultText;

            if (currentSystem === 'WGS84') {
                // WGS84 轉 TWD97
                convertedCoords = wgs84ToTwd97(val1, val2);
                targetSystem = 'TWD97';
                resultText = targetSystem + ' TM2 座標<br>' +
                    'X: ' + convertedCoords.x + ' 公尺<br>' +
                    'Y: ' + convertedCoords.y + ' 公尺';
            } else {
                // TWD97 轉 WGS84
                convertedCoords = twd97ToWgs84(val1, val2);
                targetSystem = 'WGS84';
                resultText = targetSystem + ' 經緯度座標<br>' +
                    '緯度: ' + convertedCoords.lat + '<br>' +
                    '經度: ' + convertedCoords.lon;
            }

            // 顯示轉換結果
            document.getElementById('convertedCoords').innerHTML = resultText;
            document.getElementById('convertResult').style.display = 'block';

            console.log('座標轉換結果:', {
                original: { system: currentSystem, val1: val1, val2: val2 },
                converted: { system: targetSystem, coords: convertedCoords }
            });
        }

        // 取得目前位置
        function getCurrentLocation() {
            const currentSystem = document.querySelector('input[name="system"]:checked').value;

            if (currentSystem === 'TWD97') {
                alert('目前位置功能僅支援 WGS84 格式，請先切換到 WGS84');
                return;
            }

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function (position) {
                        const lat = position.coords.latitude.toFixed(6);
                        const lng = position.coords.longitude.toFixed(6);

                        document.getElementById('latitude').value = lat;
                        document.getElementById('longitude').value = lng;

                        document.getElementById('currentCoords').innerHTML =
                            '緯度: ' + lat + '<br>經度: ' + lng;
                        document.getElementById('locationInfo2').style.display = 'block';

                        // 清除轉換結果
                        document.getElementById('convertResult').style.display = 'none';
                    },
                    function (error) {
                        alert('無法取得位置：' + error.message);
                    }
                );
            } else {
                alert('瀏覽器不支援地理位置功能');
            }
        }

        // 執行座標查詢
        function searchCoordinates() {
            const lat = document.getElementById('latitude').value;
            const lng = document.getElementById('longitude').value;
            const system = document.querySelector('input[name="system"]:checked').value;

            if (!lat || !lng) {
                alert('請輸入完整的經緯度座標');
                return;
            }

            // 這裡可以加入實際的查詢邏輯
            alert('查詢參數：\n座標系統: ' + system + '\n緯度: ' + lat + '\n經度: ' + lng);

            console.log({
                system: system,
                latitude: parseFloat(lat),
                longitude: parseFloat(lng)
            });
        }
    </script>
</asp:Content>
