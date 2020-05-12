using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    class DataOprt
    {
        SqlOpera sqlO;
        public DataOprt()
        {
            sqlO = new SqlOpera();
        }
        public string[] pickdaily(string qq)
        {
            string[] str = new string[2];
            DataSet dts = sqlO.GetDataSet(@"Select login_name, Login_password from account where QQid = '" + qq + "';");
            if (dts.Tables[0].Rows.Count > 0)
            {
                str[0] = dts.Tables[0].Rows[0][0].ToString();
                str[1] = dts.Tables[0].Rows[0][1].ToString();
            }
            else str[0] = "无";
            sqlO.close();
            return str;
        }
        public void regaccount(string qq, string name, string pwd)
        {
            if (sqlO.ExecuteSelectCommand(@"select * from account where QQid = '" + qq + "';") >= 0)
            {
                sqlO.ExecuteCommand(@"update account set login_name = '" + name + "', Login_password = '" + pwd + "' where QQid = '" + qq + "';");
            }
            else
            {
                sqlO.ExecuteCommand(@"insert into account (QQid, login_name, Login_password) values('" + qq + "','" + name + "','" + pwd + "');");
            }
            if (sqlO.ExecuteSelectCommand(@"select * from account1 where QQid = '" + qq + "';") >= 0)
            {
                sqlO.ExecuteCommand(@"update account1 set login_name = '" + name + "', Login_password = '" + pwd + "' where QQid = '" + qq + "';");
            }
            else
            {
                sqlO.ExecuteCommand(@"insert into account1 (QQid, login_name, Login_password) values('" + qq + "','" + name + "','" + pwd + "');");
            }
            sqlO.close();
        }
        public string[] queryuse()
        {
            string[] str = new string[2];
            str[0] = DateTime.Now.ToLongDateString();
            if (sqlO.ExecuteSelectCommand(@"select * from sum_acc where sum_date = '" + DateTime.Now.ToLongDateString() + "';") >= 0)
            {
                DataSet dtset = sqlO.GetDataSet(@"select sums from sum_acc where sum_date = '" + DateTime.Now.ToLongDateString() + "';");
                str[1] = dtset.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                sqlO.ExecuteCommand(@"insert into sum_acc (sum_date, sums) values('" + DateTime.Now.ToLongDateString() + "','0');");
                str[1] = "0";
            }
            sqlO.close();
            return str;
        }
        public void recordM(string qq)
        {
            if (sqlO.ExecuteSelectCommand(@"select * from pick_acc where sum_date = '" + DateTime.Now.ToLongDateString() + "'and QQid ='" + qq + "';") >= 0)
            {
                sqlO.close();
                return;
            }
            else
            {
                sqlO.ExecuteSelectCommand(@"insert into pick_acc (sum_date,QQid) values('" + DateTime.Now.ToLongDateString() + "','" + qq + "');");
                if (sqlO.ExecuteSelectCommand(@"select * from sum_acc where sum_date = '" + DateTime.Now.ToLongDateString() + "';") < 0)
                {
                    sqlO.ExecuteCommand(@"insert into sum_acc (sum_date, sums) values('" + DateTime.Now.ToLongDateString() + "','1');");
                    sqlO.close();
                    return;
                }
                else
                {
                    int nums = int.Parse(sqlO.GetDataSet(@"select sums from sum_acc where (sum_date = '" + DateTime.Now.ToLongDateString() + "')").Tables[0].Rows[0][0].ToString()) + 1;
                    sqlO.ExecuteCommand(@"update sum_acc set sums = '" + nums + "' where (sum_date = '" + DateTime.Now.ToLongDateString() + "');");
                }
                sqlO.close();
            }
        }
        public string[] Setting(string qq)
        {
            string[] res = new string[5];
            if (sqlO.ExecuteSelectCommand(@"Select * from setting where QQid = '" + qq + "';") >= 0)
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
        public void Setting(string qq, string[] res, int index)
        {
            res[index + 0] = (int.Parse(res[index + 0])).ToString();
            res[index + 1] = (int.Parse(res[index + 1])).ToString();
            res[index + 2] = (int.Parse(res[index + 2])).ToString();
            res[index + 3] = (int.Parse(res[index + 3])).ToString();
            res[index + 4] = (int.Parse(res[index + 4])).ToString();
            if (sqlO.ExecuteSelectCommand(@"Select * from setting where QQid = '" + qq + "';") >= 0)
                sqlO.ExecuteCommand(@"update setting set sF5 = '" + res[index + 0] + "', sF6 = '" + res[index + 1] + "',sF7 = '" + res[index + 2] + "',sF8 = '" + res[index + 3] + "', sF9 = '" + res[index + 4] + "' where QQid = '" + qq + "';");
            else
                sqlO.ExecuteCommand(@"insert into setting (QQid,sF5,sf6,sF7,sF8,sF9) values('" + qq + "','" + res[index + 0] + "','" + res[index + 1] + "','" + res[index + 2] + "','" + res[index + 3] + "','" + res[index + 4] + "');");
        }
        public void Close()
        {
            sqlO.close();
        }
        public int deleteAcc(string qq)
        {
            return sqlO.ExecuteCommand(@"delete from account where QQid = '" + qq + "';");
        }
    }
}
