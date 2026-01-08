<%@ Page Title="" Language="C#" MasterPageFile="~/_mp/mp_backstage_def.master" AutoEventWireup="true" CodeFile="TreeManage_Care.aspx.cs" Inherits="Backstage_TreeManage_Care" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" Runat="Server">
    <link href="../../_css/cust_dashboard.css" rel="stylesheet" />
    <link href="../../_css/cust_modal.css" rel="stylesheet" />
    <link href="../../_css/cust_beforafter.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_title" Runat="Server">
    樹籍資料管理：養護紀錄（編輯）
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_content" Runat="Server">
    <div class="row">
        <div class="col">
            <div class="item">
                <img src="../../_img/icon/database.png" /><br />
                <a href="TreeManage_Detail.aspx">樹籍資料</a>
            </div>
        </div>
        <div class="col">
            <div class="item">
                <img src="../../_img/icon/health-check2.png" /><br />
                <a href="TreeManage_Health_List.aspx">健檢紀錄</a>
            </div>
        </div>
        <div class="col">
            <div class="item">
                <img src="../../_img/icon/picture.png" /><br />
                <a href="TreeManage_Patrol_List.aspx">巡查紀錄</a>
            </div>
        </div>
        <div class="col">
            <div class="item">
                <img src="../../_img/icon/sign.png" /><br />
                <a href="TreeManage_Care_List.aspx">養護紀錄</a>
            </div>
        </div>
    </div>
    <div class="pageBlock mt-3">
        <div class="boxTitle">基本資料</div>
        <div class="row">
            <div class="col">
                <label for="location">縣市鄉鎮</label>
                <div>臺北市中正區</div>
            </div>
            <div class="col">
                <label for="tree_id">系統樹籍編號</label>
                <div style="color:#ffc0cb">A0001</div>
            </div>
            <div class="col">
                <label for="agency_code">機關管轄編碼</label>
                <div>--</div>
            </div>
            <div class="col">
                <label for="agency_tree_id">機關樹木編號</label>
                <div>珍貴樹木584</div>
            </div>
        </div>
        <div class="row mt-4">
            <div class="col">
                <label for="location">樹種及學名</label>
                <div>榕樹 Ficus microcarpa</div>
            </div>
            <div class="col">
                <label for="agency_tree_id">數量</label>
                <div>1</div>
            </div>
            <div class="col">
                <label for="tree_id">樹籍狀態</label>
                <div>已公告列管</div>
            </div>
            <div class="col">
                <label for="agency_code">公告日期</label>
                <div>2025/05/01</div>
            </div>
        </div>
    </div>
    <div class="pageBlock mt-3">
        <div class="boxTitle">養護紀錄</div>
        <div class="row">
            <div class="col text-center">
                <a href="TreeManage_Care_New.aspx" class="btn_action">新增養護紀錄</a>
            </div>
        </div>
        <div class="table-responsive">
            <table class="tb">
                <tr>
                    <th>系統紀錄編號</th>
                    <th>養護日期</th>
                    <th>紀錄人員</th>
                    <th>覆核人員</th>
                    <th>資料狀態</th>
                    <th>最後編輯時間</th>
                    <th>動作</th>
                </tr>
                <tr>
                    <td>5</td>
                    <td><a href="#">2025/12/01</a></td>
                    <td>毛詹苔</td>
                    <td>林美美</td>
                    <td style="color:#00ffff">草稿</td>
                    <td>2025/01/03 14:20</td>
                    <td>
                        <button class="btn_action" type="button" data-bs-toggle="modal" data-bs-target="#batchUpdateModal">檢視</button>
                        <a href="TreeManage_Care.aspx" class="btn_action">編輯</a>
                    </td>
                </tr>
                <tr>
                    <td>4</td>
                    <td><a href="#">2025/11/01</a></td>
                    <td>毛詹苔</td>
                    <td>林美美</td>
                    <td style="color:#f6ad49">完稿</td>
                    <td>2025/01/03 14:20</td>
                    <td>
                        <button class="btn_action" type="button" data-bs-toggle="modal" data-bs-target="#batchUpdateModal">檢視</button>
                        <a href="TreeManage_Care.aspx" class="btn_action">編輯</a>
                    </td>
                </tr>
                <tr>
                    <td>3</td>
                    <td><a href="#">2025/10/01</a></td>
                    <td>毛詹苔</td>
                    <td>林美美</td>
                    <td style="color:#f6ad49">完稿</td>
                    <td>2025/01/03 14:20</td>
                    <td>
                        <button class="btn_action" type="button" data-bs-toggle="modal" data-bs-target="#batchUpdateModal">檢視</button>
                        <a href="TreeManage_Care.aspx" class="btn_action">編輯</a>
                    </td>
                </tr>
                <tr>
                    <td>2</td>
                    <td><a href="#">2025/09/01</a></td>
                    <td>毛詹苔</td>
                    <td>林美美</td>
                    <td style="color:#f6ad49">完稿</td>
                    <td>2025/01/03 14:20</td>
                    <td>
                        <button class="btn_action" type="button" data-bs-toggle="modal" data-bs-target="#batchUpdateModal">檢視</button>
                        <a href="TreeManage_Care.aspx" class="btn_action">編輯</a>
                    </td>
                </tr>
                <tr>
                    <td>1</td>
                    <td><a href="#">2025/08/01</a></td>
                    <td>毛詹苔</td>
                    <td>林美美</td>
                    <td style="color: #f6ad49">完稿</td>
                    <td>2025/01/03 14:20</td>
                    <td>
                        <button class="btn_action" type="button" data-bs-toggle="modal" data-bs-target="#batchUpdateModal">檢視</button>
                        <a href="TreeManage_Care.aspx" class="btn_action">編輯</a>
                    </td>
                </tr>
            </table>
            <div class="tbFooter">
                <a href="#">1</a>
                <a href="#">2</a>
                <a href="#">3</a>
                <a href="#">4</a>
                <a href="#">5</a>
            </div>
        </div>
    </div>
    <div class="pageBlock mt-3">
        <div class="boxTitle">養護內容</div>
        <div class="boxTitle_mini">說明</div>
        <div class="row">
            <div class="col">
                <label for="care_date">養護日期</label>
                <input type="date" id="care_date" name="care_date" class="w-100" />
            </div>
             <div class="col">
                 <label>記錄人員</label><input type="text" class="w-100" />
             </div>
             <div class="col">
                 <label>覆核人員</label><input type="text" class="w-100" />
             </div>
        </div>
        <div class="boxTitle_mini mt-4">生長情形概況</div>
         <div class="row mt-4">
             <div class="col-2">
                 1. 樹冠枝葉
             </div>
             <div class="col">
                 <div>
                    <label class="me-2"><input type="radio" name="crown" value="normal" checked="checked" /> 枝葉茂密無枯枝</label>
                    <label class="me-2"><input type="radio" name="crown" value="other" /> 有其他異狀</label>
                 </div>
                 <div class="checkbox-div">
                     <input type="checkbox" name="criteria" value="age_100"><span>季節性休眠落葉</span>
                 </div>
                 <div class="checkbox-div">
                     <input type="checkbox" name="criteria" value="age_100"><span>有枯枝(現存枝葉量：<input type="text" class="w-25" disabled style="width:80px" />%)</span>
                 </div>
                 <div class="checkbox-div">
                     <input type="checkbox" name="criteria" value="age_100"><span>有明顯病蟲害(葉部有明顯蟲體或病徵)</span>
                 </div>
                 <div class="checkbox-div">
                     <input type="checkbox" name="criteria" value="age_100"><span>樹冠接觸電線或異物</span>
                 </div>
                 <br />
                 <div class="checkbox-div">
                     <input type="checkbox" name="criteria" value="age_100"><span>其他：</span>
                 </div>
                 <input type="text" class="w-100">
             </div>
         </div>
         <div class="row mt-4">
            <div class="col-2">
                2. 主莖幹
            </div>
            <div class="col">
                <div>
                   <label class="me-2"><input type="radio" name="crown" value="normal" checked="checked" /> 完好健康無異狀</label>
                   <label class="me-2"><input type="radio" name="crown" value="other" /> 有其他異狀</label>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>樹皮破損</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>莖幹損傷(腐朽中空或膨大)</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>有白蟻蟻道</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>主莖傾斜搖晃</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>莖基部有真菌子實體(如靈芝)</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>有流膠或潰瘍</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>有纏勒植物(如雀榕或小花蔓澤蘭)</span>
                </div>
                <br />
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>其他：</span>
                </div>
                <input type="text" class="w-100">
            </div>
        </div>
         <div class="row mt-4">
            <div class="col-2">
                3. 根部及地際部
            </div>
            <div class="col">
                <div>
                   <label class="me-2"><input type="radio" name="crown" value="normal" checked="checked" /> 根部完好無異狀</label>
                   <label class="me-2"><input type="radio" name="crown" value="other" /> 有其他異狀</label>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>根部損傷</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>根部有腐朽或可見子實體</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>盤根或浮根</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>根部潰爛</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>大量萌櫱(不定芽)</span>
                </div>
                <br />
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>其他：</span>
                </div>
                <input type="text" class="w-100">
            </div>
        </div>
         <div class="row mt-4">
            <div class="col-2">
                4. 生育地環境
            </div>
            <div class="col">
                <div>
                    <label class="me-2"><input type="radio" name="crown" value="normal" checked="checked" /> 良好無異狀</label>
                    <label class="me-2"><input type="radio" name="crown" value="other" /> 有其他異狀</label>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>樹穴過小</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>遭鋪面封固(如柏油、混凝土、磚瓦等)</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>有石塊或廢棄物推積</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>根領覆土過高</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>土壤壓實</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>環境積水</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>緊鄰設施或建物</span>
                </div>
                <br />
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>其他：</span>
                </div>
                <input type="text" class="w-100">
            </div>
        </div>
         <div class="row mt-4">
            <div class="col-2">
                5. 樹冠或主莖幹與鄰接物
            </div>
            <div class="col">
                <div>
                    <label class="me-2"><input type="radio" name="crown" value="normal" checked="checked" /> 無鄰接物</label>
                    <label class="me-2"><input type="radio" name="crown" value="other" /> 有其他異狀</label>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>接觸建築物</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>接觸電線或管線</span>
                </div>
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>遮蔽路燈或號誌</span>
                </div>
                <br />
                <div class="checkbox-div">
                    <input type="checkbox" name="criteria" value="age_100"><span>其他：</span>
                </div>
                <input type="text" class="w-100">
            </div>
        </div>
        <div class="boxTitle_mini mt-4">養護管理作業</div>
        <div class="row mt-2">
            <div class="col-2">
                 <strong>1. 危險枯枝清除</strong>
            </div>
            <div class="col">
                <span>受保護樹木若有枝條枯損或懸掛等危害公共安全疑慮之情形，宜立即回報並盡速清除</span><br />
                完成情形：
                <label class="me-2"><input type="radio" name="task1" checked="checked" /> 無須處理</label>
                <label><input type="radio" name="task1" /> 處理方式(請填寫說明：<input type="text" style="width:500px" />)</label>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-2">
                 <strong>2. 植栽基盤維護暨環境整理</strong>
            </div>
            <div class="col">
                <span>包含改良土壤結構通透氣及排水、清除過度覆土、拓展植穴空間等作業</span><br />
                完成情形：
                <label class="me-2"><input type="radio" name="task2" checked="checked" /> 無須處理</label>
                <label><input type="radio" name="task2" /> 處理方式(請填寫說明：<input type="text" style="width:500px" />)</label>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-2">
                 <strong>3. 樹木健康管理</strong>
            </div>
            <div class="col">
                <span>受保護樹木如有病害、蟲害等情況，須進一步診斷查明原因後再進行治療</span><br />
                完成情形：
                <label class="me-2"><input type="radio" name="task3" checked="checked" /> 無須處理</label>
                <label><input type="radio" name="task3" /> 處理方式(請填寫說明：<input type="text" style="width:500px" />)</label>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-2">
                 <strong>4. 營養評估追肥</strong>
            </div>
            <div class="col">
                <span>受保護樹木若經評估建議追給適量緩效性有機質肥料，宜配合調查進行</span><br />
                完成情形：
                <label class="me-2"><input type="radio" name="task4" checked="checked" /> 無須處理</label>
                <label><input type="radio" name="task4" /> 處理方式(請填寫說明：<input type="text" style="width:500px" />)</label>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-2">
                 <strong>5. 安全衛生防護(非必填)</strong>
            </div>
            <div class="col">
                <span>當受保護樹木進行維護作業時，應設立相關安全衛生防護措施</span><br />
                完成情形：
                <label class="me-2"><input type="radio" name="task5" checked="checked" /> 無須處理</label>
                <label><input type="radio" name="task5" /> 處理方式(請填寫說明：<input type="text" style="width:500px" />)</label>
            </div>
        </div>

        <%--<div class="row">
            <div class="col">
                備註：<br />
                1.紀錄表使用方式請見後方說明。<br />
                2.受保護樹木如有嚴重危害公共安全之風險或其他特殊緊急狀況時應立即回報。
            </div>
        </div>--%>

        <div class="boxTitle_mini mt-4 mb-2">養護照片上傳</div>
        <div id="uploadPhotoBlocks"></div>
        <div class="row">
            <div class="col text-center">
                <button type="button" id="addUploadPhotoBlock" class="btn_action">新增照片區塊</button>
            </div>
        </div>        
        <div class="row mt-4">
            <div class="col text-center">
                <asp:Button ID="Button_save_pass" runat="server" Text="儲存" CssClass="btn_action" />
                <asp:Button ID="Button_save_temp" runat="server" Text="暫存" CssClass="btn_action" />
                <asp:Button ID="Button_delete" runat="server" Text="刪除" CssClass="btn_action" />
                <asp:Button ID="Button_cancel" runat="server" Text="返回列表" CssClass="btn_action" />
            </div>
        </div>
    </div>
    <div class="pageBlock mt-3">
        <div class="row">
            <div class="boxTitle">操作說明</div>
            <div class="col">
                1. 養護管理頻度建議每年至少1次，除參照本表項目查填外，亦應針對各項目之作前、施作後狀態拍照記錄。<br />
                2. 針對健康有疑慮或具有危害公共安全風險之受保護樹木，建議養護管理作業頻度為每6個月至少1次。<br />
                3. 受保護樹木如有嚴重危害公共安全之風險或其他特殊緊急狀況時應立即回報。<br />
                4. 同一天(養護日期)僅允許一筆養護紀錄存在。<br />
            </div>
        </div>
    </div>
    <div class="pageBlock mt-3">
        <div class="row">
            <div class="boxTitle">資料操作紀錄</div>
        </div>        
        <div class="table-responsive">
            <table class="tb">
                <tr>
                    <th>操作時間</th>
                    <th>動作類型</th>
                    <th>使用者帳號</th>
                    <th>使用者姓名</th>
                    <th>使用者單位</th>
                    <th>IP位置</th>            
                </tr>
                <tr><td>2024/08/21 18:35</td><td>編輯</td><td>moumou</td><td>毛詹苔</td><td>臺北市政府</td><td>192.168.0.1</td></tr>
                <tr><td>2024/02/16 18:35</td><td>編輯</td><td>meimei</td><td>林美美</td><td>臺北市政府</td><td>192.168.0.1</td></tr>
                <tr><td>2024/02/15 18:35</td><td>編輯</td><td>meimei</td><td>林美美</td><td>臺北市政府</td><td>192.168.0.1</td></tr>
                <tr><td>2024/01/01 18:35</td><td>新增</td><td>admin</td><td>陳大華</td><td>臺北市政府</td><td>192.168.0.1</td></tr>
            </table>
        </div>
    </div>
    <script>
        $(function () {
            function setupToggle(radioName, optsId) {
                function update() {
                    if ($('input[name="' + radioName + '"]:checked').val() === 'other') {
                        $('#' + optsId + ' input').prop('disabled', false);
                    } else {
                        $('#' + optsId + ' input').prop('disabled', true);
                    }
                }
                $('input[name="' + radioName + '"]').on('change', update);
                update();
            }
            setupToggle('crown', 'crown_opts');
            setupToggle('trunk', 'trunk_opts');
            setupToggle('root', 'root_opts');
            setupToggle('env', 'env_opts');
            setupToggle('adjacent', 'adjacent_opts');

            
        });
    </script>
    <script src="../../_js/cust/image-upload.js"></script>

    <!-- Modal -->
    
    <div class="modalStyle modalForm">
        <!-- Modal -->
        <div class="modal fade" id="batchUpdateModal" tabindex="-1" aria-labelledby="batchUpdateModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        養護紀錄
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="boxTitle_mini">說明</div>
                        <div class="row">
                            <div class="col"><span>養護日期</span>2025/12/01</div>
                            <div class="col"><span>記錄人員</span>毛詹苔</div>
                            <div class="col"><span>覆核人員</span>林美美</div>
                        </div>                    
                        <div class="boxTitle_mini mt-4">生長情形概況</div>
                        <div class="row mt-2">
                            <div class="col-3">
                                1. 樹冠枝葉
                            </div>
                            <div class="col-9">
                                □枝葉茂密無枯枝<br />
                                ■有其他異狀：<br />
                                <div class="ms-3">
                                    ■季節性休眠落葉<br />
                                    ■有枯枝(現存枝葉量：50%)<br />
                                    □有明顯病蟲害(葉部有明顯蟲體或病徵)<br />
                                    □樹冠接觸電線或異物<br />
                                    □其他：
                                </div>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                2. 主莖幹
                            </div>
                            <div class="col-9">
                                □枝葉茂密無枯枝<br />
                                ■有其他異狀：<br />
                                <div class="ms-3">
                                    □完好健康無異狀<br />
                                    ■有其他異狀：<br />
                                    <div class="ms-3">
                                        □樹皮破損<br />
                                        □莖幹損傷(腐朽中空或膨大)<br />
                                        □有白蟻蟻道<br />
                                        □主莖傾斜搖晃<br />
                                        □莖基部有真菌子實體(如靈芝)<br />
                                        □有流膠或潰瘍<br />
                                        □有纏勒植物(如雀榕或小花蔓澤蘭)<br />
                                        ■其他：不知道為什麼有一個大洞，已採樣化驗。
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                3. 根部及地際部
                            </div>
                            <div class="col-9">
                                ■根部完好無異狀<br />
                                □有其他異狀：<br />
                                <div class="ms-3">
                                    □根部損傷<br />
                                    □根部有腐朽或可見子實體<br />
                                    □盤根或浮根<br />
                                    □根部潰爛<br />
                                    □大量萌櫱(不定芽)<br />
                                    □其他：
                                </div>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                4. 生育地環境
                            </div>
                            <div class="col-9">
                                □良好無異狀<br />
                                ■有其他異狀：<br />
                                <div class="ms-3">
                                    ■樹穴過小<br />
                                    ■遭鋪面封固(如柏油、混凝土、磚瓦等)<br />
                                    ■有石塊或廢棄物推積<br />
                                    □根領覆土過高<br />
                                    □土壤壓實<br />
                                    □環境積水<br />
                                    □緊鄰設施或建物<br />
                                    □其他：
                                </div>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                5. 樹冠或主莖幹與鄰接物
                            </div>
                            <div class="col-9">
                                □無鄰接物<br />
                                ■有其他異狀：<br />
                                <div class="ms-3">
                                    □接觸建築物<br />
                                    □接觸電線或管線<br />
                                    ■遮蔽路燈或號誌<br />
                                    □其他：
                                </div>
                            </div>
                        </div>
                        <div class="boxTitle_mini mt-4">養護管理作業</div>
                        <div class="row mt-2">
                            <div class="col-3">
                                1. 危險枯枝清除
                            </div>
                            <div class="col-9">
                                <span>受保護樹木若有枝條枯損或懸掛等危害公共安全疑慮之情形，宜立即回報並盡速清除</span>
                                完成情形：■無須處理 □處理方式(請填寫說明：)
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                2. 植栽基盤維護暨環境整理
                            </div>
                            <div class="col-9">
                                <span>包含改良土壤結構通透氣及排水、清除過度覆土、拓展植穴空間等作業</span>
                                完成情形：■無須處理 □處理方式(請填寫說明：)
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                3. 樹木健康管理
                            </div>
                            <div class="col-9">
                                <span>受保護樹木如有病害、蟲害等情況，須進一步診斷查明原因後再進行治療</span>
                                完成情形：■無須處理 □處理方式(請填寫說明：)
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                4. 營養評估追肥
                            </div>
                            <div class="col-9">
                                <span>受保護樹木若經評估建議追給適量緩效性有機質肥料，宜配合調查進行</span>
                                完成情形：■無須處理 □處理方式(請填寫說明：)
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-3">
                                5. 安全衛生防護(非必填)
                            </div>
                            <div class="col-9">
                                <span>當受保護樹木進行維護作業時，應設立相關安全衛生防護措施</span>
                                完成情形：■無須處理 □處理方式(請填寫說明：)
                            </div>
                        </div>
                        <div class="boxTitle_mini mt-4">施作項目照片</div>
                        施作項目1<br />
                        <div class="row mb-2">                        
                            <div class="col">
                                <img src="../../_img/before.jpg" class="w-100" alt="前">
                            </div>
                            <div class="col">
                                <img src="../../_img/after.jpg" class="w-100" alt="後">
                            </div>
                        </div>
                        施作項目2<br />
                        <div class="row">
                            <div class="col">
                                <img src="../../_img/before.jpg" class="w-100" alt="前">
                            </div>
                            <div class="col">
                                <img src="../../_img/after.jpg" class="w-100" alt="後">
                            </div>
                        </div>
                        <%--<div class="text-center">作業項目1（施作前／後）</div>
                        <div class="container mt-4 mb-2">
                            <div style="margin: 0 auto; width: 400px;">
                                <div class="before-after-container">
                                    <div class="before-after-wrapper">
                                        <img src="../../_img/before.jpg" class="before-image" alt="前">
                                    </div>
                                    <img src="../../_img/after.jpg" class="after-image" alt="後" />
                                    <div class="before-after-slider">
                                        <div class="before-after-handle"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="text-center">作業項目2（施作前／後）</div>
                        <div class="container mt-4 mb-2">
                            <div style="margin: 0 auto; width: 400px;">
                                <div class="before-after-container2">
                                    <div class="before-after-wrapper2">
                                        <img src="../../_img/before.jpg" class="before-image" alt="前">
                                    </div>
                                    <img src="../../_img/after.jpg" class="after-image" alt="後" />
                                    <div class="before-after-slider2">
                                        <div class="before-after-handle"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="text-center">作業項目3（施作前／後）</div>
                        <div class="container mt-4">
                            <div style="margin: 0 auto; width: 400px;">
                                <div class="before-after-container3">
                                    <div class="before-after-wrapper3">
                                        <img src="../../_img/before.jpg" class="before-image" alt="前">
                                    </div>
                                    <img src="../../_img/after.jpg" class="after-image" alt="後" />
                                    <div class="before-after-slider3">
                                        <div class="before-after-handle"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>--%>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-bs-dismiss="modal">
                            關閉
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="../../_js/cust/beforAfter.js"></script>
</asp:Content>

