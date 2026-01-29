using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess;

namespace protectTreesV2.Base
{
    /// <summary>
    /// 單位與其對應權限
    /// </summary>
    public class OrgUnit
    {
        public int unitID { get; set; }
        public string unitName { get; set; }
        public int auTypeID { get; set; }
    }

    /// <summary>
    /// 提供取得單位資訊的服務
    /// </summary>
    public static class OrgUnitService
    {
        /// <summary>
        /// 取得所有可申請的單位清單
        /// </summary>
        public static List<OrgUnit> GetUnits()
        {
            var list = new List<OrgUnit>();
            using (var da = new MS_SQL())
            {
                string sql = "SELECT unitID, unitName, auTypeID FROM System_UserUnit ORDER BY unitID";
                var dt = da.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new OrgUnit
                    {
                        unitID = Convert.ToInt32(row["unitID"]),
                        unitName = row["unitName"].ToString(),
                        auTypeID = Convert.ToInt32(row["auTypeID"])
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 依據單位編號取得單位資訊
        /// </summary>
        public static OrgUnit GetUnit(int unitID)
        {
            using (var da = new MS_SQL())
            {
                string sql = "SELECT TOP 1 unitID, unitName, auTypeID FROM System_UserUnit WHERE unitID=@id";
                var dt = da.GetDataTable(sql, new SqlParameter("@id", unitID));
                if (dt.Rows.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    return new OrgUnit
                    {
                        unitID = Convert.ToInt32(row["unitID"]),
                        unitName = row["unitName"].ToString(),
                        auTypeID = Convert.ToInt32(row["auTypeID"])
                    };
                }
            }
            return null;
        }
    }
}
