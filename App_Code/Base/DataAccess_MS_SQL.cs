using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
///  DB 處理
/// </summary>
/// <remarks>最後版本：2018-10-08</remarks>
namespace DataAccess
{
    public class MS_SQL : IDisposable
    {
        private string connStrName { get; set; } = "SQL_Connection";
        private System.Data.SqlClient.SqlConnection _conn = new System.Data.SqlClient.SqlConnection();
        private System.Data.SqlClient.SqlCommand _cmd = new System.Data.SqlClient.SqlCommand();
        //private int intResult;
        private System.Data.SqlClient.SqlTransaction myTranscation = null;
       // private bool _useTrans = false;
        //private string _defaultConnect

        System.Data.SqlClient.SqlConnection connection {
            get {
                this.OpenConnection();
                return this._conn;
            }
        }
        private void OpenConnection() {  if (this._conn.State == System.Data.ConnectionState.Closed) this._conn.Open();  }
        private void CloseConnection() { this._conn.Close(); }

        public MS_SQL() {
            // 取得連線字串
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings[this.connStrName].ConnectionString;
            if (string.IsNullOrEmpty(connStr)) throw new Exception("未設定連線字串：" + this.connStrName);
            this._conn = new System.Data.SqlClient.SqlConnection(connStr);
            this._cmd.Connection = this._conn;
        }
        public MS_SQL(string ConnectString) {
            _conn = new System.Data.SqlClient.SqlConnection(ConnectString);
            _cmd.Connection = this._conn;
        }
        /// <summary>
        /// 啟用 Transaction
        /// </summary>
        public void StartTransaction() {
            if (this.myTranscation == null) {
                this.OpenConnection();
                this.myTranscation = this._conn.BeginTransaction();
                //this._useTrans = true;
                this._cmd.Transaction = this.myTranscation;
            }
        }
        public void Commit() { this.myTranscation.Commit(); }
        public void RollBack() {  this.myTranscation.Rollback();  }
        private System.Data.SqlClient.SqlConnection GetConnection() { return this._conn; }
        private System.Data.SqlClient.SqlCommand GetSqlCommand(string sqlCommand, params System.Data.SqlClient.SqlParameter[] paramet) {
            this.OpenConnection();
            this._cmd.CommandText = sqlCommand;
            this._cmd.Parameters.Clear();
            if (paramet.Length > 0) this._cmd.Parameters.AddRange(paramet);
            this._cmd.CommandTimeout = 900;
            
            return this._cmd;
        }
        /// <summary>
        /// 執行 Sql Command
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parament"></param>
        /// <returns></returns>
        public int ExecNonQuery(string sqlCommand, params System.Data.SqlClient.SqlParameter[] parament) {
            System.Data.SqlClient.SqlCommand cmd = this.GetSqlCommand(sqlCommand, parament);
            return cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 取得第一行第一列資料
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parament"></param>
        /// <returns></returns>
        public object ExcuteScalar(string sqlCommand, params System.Data.SqlClient.SqlParameter[] parament) {
            System.Data.SqlClient.SqlCommand cmd = this.GetSqlCommand(sqlCommand, parament);
            return cmd.ExecuteScalar();
        }
        /// <summary>
        /// 取得 DataTable
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parament"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataTable(string sqlCommand, params System.Data.SqlClient.SqlParameter[] parament) {
            System.Data.SqlClient.SqlCommand cmd = this.GetSqlCommand(sqlCommand, parament);
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            return dt;
        }
        /// <summary>
        /// 取得 DataView
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parament"></param>
        /// <returns></returns>
        public System.Data.DataView GetDataView(string sqlCommand, params System.Data.SqlClient.SqlParameter[] parament) {
            System.Data.DataTable dt = this.GetDataTable(sqlCommand, parament);
            return new System.Data.DataView(dt);
        }
        /// <summary>
        /// 大量工作
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        public void BulkCopy(string tableName, System.Data.DataTable dt) {
            this.OpenConnection();
            using (System.Data.SqlClient.SqlBulkCopy bulk = new System.Data.SqlClient.SqlBulkCopy(
                 this._conn,
                 System.Data.SqlClient.SqlBulkCopyOptions.Default,
                 this.myTranscation)) 
            {
                bulk.BatchSize = 1000;
                bulk.DestinationTableName = tableName;
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                bulk.WriteToServer(dt);
            }
        }

        void IDisposable.Dispose()
        {
            this._cmd.Dispose();
            this._conn.Close();
            this._conn.Dispose();

        }
    }
}
