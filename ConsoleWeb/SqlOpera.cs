using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.OleDb;
namespace ConsoleWeb
{
    class SqlOpera
    {
        private OleDbConnection AccessConnection;
        private bool isConnected;
        public SqlOpera()
        {
            isConnected = true;
            AccessConnection = new OleDbConnection(@"Provider = Microsoft.Jet.OleDb.4.0; Data Source = .\database.mdb");
            try
            {
                AccessConnection.Open();
            }
            catch (OleDbException e)
            {
                isConnected = false;
            }
        }
        public void close()
        {
            if (isConnected)
                AccessConnection.Close();
        }
        public OleDbDataReader GetReader(string safeSql)
        {
            if (!isConnected) return null;
            OleDbCommand cmd = new OleDbCommand(safeSql, AccessConnection);
            OleDbDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public DataSet GetDataSet(string safeSql)
        {
            if (!isConnected) return null;
            OleDbDataAdapter sda = new OleDbDataAdapter(safeSql, AccessConnection);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
        public bool IsConnected()
        {
            return isConnected;
        }
        //增删改返回结果
        public int ExecuteCommand(string sql)
        {
            if (!isConnected) return -1;
            OleDbCommand cmd = new OleDbCommand(sql, AccessConnection);
            int result = cmd.ExecuteNonQuery();
            return result;
        }
        //查询返回结果
        public int ExecuteSelectCommand(string sql)
        {
            OleDbCommand cmd = new OleDbCommand(sql, AccessConnection);
            int result;
            if (cmd.ExecuteScalar() != null)
                result = (int)cmd.ExecuteScalar();
            else result = -1;
            return result;
        }
    }
}
