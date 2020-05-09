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
            DataSet dts = sqlO.GetDataSet(@"Select login_name, Login_password from account where QQid = '"+ qq +"';");
            if (dts.Tables[0].Rows.Count > 0)
            {
                str[0] = dts.Tables[0].Rows[0][0].ToString();
                str[1] = dts.Tables[0].Rows[0][1].ToString();
            }
            else str[0] = "无";
            sqlO.close();
            return str;
        }
        public void regaccount(string qq,string name,string pwd)
        {
            if (sqlO.ExecuteSelectCommand(@"select * from account where QQid = '" + qq + "';") == 1)
            {
                sqlO.ExecuteCommand(@"update account set login_name = '"+name+ "', Login_password = '"+pwd +"' where QQid = '" + qq +"';");
            }
            else
            {
                sqlO.ExecuteCommand(@"insert into account (QQid, login_name, Login_password) values('" + qq + "','" + name + "','" + pwd+"');");
            }
            sqlO.close();
        }
        public string[] queryuse()
        {
            string[] str = new string[2];
            str[0] = DateTime.Now.ToLongDateString();
            if (sqlO.ExecuteSelectCommand(@"select * from sum_acc where sum_date = '" + DateTime.Now.ToLongDateString() + "';") == 1)
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
            if (sqlO.ExecuteSelectCommand(@"select * from pick_acc where sum_date = '" + DateTime.Now.ToLongDateString() + "'and QQid ='"+ qq + "';") == 1)
            {
                sqlO.close();
                return;
            }
            else
            {
                sqlO.ExecuteSelectCommand(@"insert into pick_acc (sum_date,QQid) values('" + DateTime.Now.ToLongDateString()+"','"+qq+"');");
                if (sqlO.ExecuteSelectCommand(@"select * from sum_acc where sum_date = '" + DateTime.Now.ToLongDateString() + "';") != 1)
                {
                    sqlO.ExecuteCommand(@"insert into sum_acc (sum_date, sums) values('" + DateTime.Now.ToLongDateString() + "','1');");
                    sqlO.close();
                    return;
                }
                int nums = int.Parse(sqlO.GetDataSet(@"select sums from sum_acc where (sum_date = '" + DateTime.Now.ToLongDateString() + "')").Tables[0].Rows[0][0].ToString())+1;
                sqlO.ExecuteCommand(@"update sum_acc set sums = '"+ nums +"' where (sum_date = '" + DateTime.Now.ToLongDateString() + "');");
                sqlO.close();
            }
        }
    }
}
