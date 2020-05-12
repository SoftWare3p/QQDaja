using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ConsoleWeb
{
    class DataOprt
    {
        SqlOpera sqlO;
        public DataOprt()
        {
            sqlO = new SqlOpera();
        }
        public string[] Setting(string qq)
        {
            string[] res = new string[5];
            if(sqlO.ExecuteSelectCommand(@"Select * from setting where QQid = '"+ qq+"';") >= 0)
            {
                DataSet dataSet = sqlO.GetDataSet(@"Select * from setting where QQid = '" + qq + "';");
                res[0] = dataSet.Tables[0].Rows[0]["sF5"].ToString();
                res[1] = dataSet.Tables[0].Rows[0]["sF6"].ToString();
                res[2] = dataSet.Tables[0].Rows[0]["sF7"].ToString();
                res[3] = dataSet.Tables[0].Rows[0]["sF8"].ToString();
                res[4] = dataSet.Tables[0].Rows[0]["sF9"].ToString();
            }
            else
            {
                res[0] = "1"; res[1] = "5"; res[2] = "1"; res[3] = "1"; res[4] = "1";
                Setting(qq, res);
            }
            return res;
        }
        public void Setting(string qq, string[] res)
        {
            Setting(qq, res, 0);
        }
        public void Setting(string qq, string [] res,int index)
        {
            if (sqlO.ExecuteSelectCommand(@"Select * from setting where QQid = '" + qq + "';") >= 0)
                sqlO.ExecuteCommand(@"update setting set sF5 = '" + res[index+0] + "', sF6 = '" + res[index +1] + "',sF7 = '" + res[index +2] + "',sF8 = '" + res[index +3] + "', sF9 = '" + res[index +4] +"' where QQid = '" + qq + "';");
            else
                sqlO.ExecuteCommand(@"insert into setting (QQid,sF5,sf6,sF7,sF8,sF9) values('"+qq+"','"+ res[index + 0] + "','"+ res[index + 1] + "','"+ res[index + 2] + "','"+ res[index + 3] + "','"+ res[index + 4] + "');");
        }
        public void Close()
        {
            sqlO.close();
        }
    }
}
