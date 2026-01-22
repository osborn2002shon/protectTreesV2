using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace protectTreesV2
{
    public partial class _default : System.Web.UI.Page
    {

        /// <summary>
        /// 依地區取得單位清單
        /// </summary>
        public static DataTable Get_UnitInfo_ByAreaGroup(string areaGroup)
        {
            string sqlString =
                "select * from View_UnitInfo " +
                "where areaGroup = @areaGroup " +
                "order by cityID";
            DataTable dt = new DataTable();
            using (var da = new DataAccess.MS_SQL())
            {
                dt = da.GetDataTable(
                    sqlString,
                    new SqlParameter("@areaGroup", areaGroup)
                );
            }
            return dt;
        }

        /// <summary>
        /// 繫結 Repeater
        /// </summary>
        /// <param name="areaGroup"></param>
        private void Bind_Repeater_Unit(string areaGroup)
        {
            DataTable dt = Get_UnitInfo_ByAreaGroup(areaGroup);
            Repeater_Unit.DataSource = dt;
            Repeater_Unit.DataBind();
        }

        protected void RadioButtonList_area_SelectedIndexChanged(object sender, EventArgs e)
        {
            string areaGroup = RadioButtonList_area.SelectedValue;
            Bind_Repeater_Unit(areaGroup);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string areaGroup = RadioButtonList_area.SelectedValue;
                Bind_Repeater_Unit(areaGroup);
            }
        }
    }
}