using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace Newbe.Mahua.Plugins.Parrot.MahuaService
{
    public interface ITickServices
    {
        Task StartAsync();
    }
    class TickServices: ITickServices
    {
        public TickServices()
        {  
        }
        System.Timers.Timer t;
        public Task StartAsync()
        {
            t = new System.Timers.Timer(1000*60*60);//*60*60
            t.Elapsed += new System.Timers.ElapsedEventHandler(Theout);
            t.AutoReset = true;//true
            t.Enabled = true;
            Task.Factory.StartNew(() => {
                using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                {
                    var api = robotSession.MahuaApi;
                    api.SendPrivateMessage(@"1307650694", @"初始化成功");
                }
            });
            return Task.FromResult(0);
        }
        private static void Theout(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (System.DateTime.Now.Hour >= 21)
            {
                Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt dataOprt = new Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt();
                System.Data.DataSet list = dataOprt.getPushList();
                for (int i = 0; i < list.Tables[0].Rows.Count; i++)
                {
                    string qq = list.Tables[0].Rows[i][0].ToString();
                    if (dataOprt.verQQ(qq) == -1)
                    {
                        Task.Factory.StartNew(() => {
                            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                            {
                                var api = robotSession.MahuaApi;
                                api.SendPrivateMessage(qq,"你今天尚未使用本助手打卡！");
                            } });
                    }
                }
                Task.Factory.StartNew(() => {
                    using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                    {
                        var api = robotSession.MahuaApi;
                        api.SendPrivateMessage("1307650694", "打卡提醒已经发送！");
                    }
                });
                dataOprt.Close();
            }
            else if (System.DateTime.Now.Hour == 11)
            {
                Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt dataOprt = new Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt();
                System.Data.DataSet list = dataOprt.getAutoList();
                dataOprt.Close();
                for (int i = 0; i < list.Tables[0].Rows.Count; i++)
                {
                    string qq = list.Tables[0].Rows[i][0].ToString();
                    string msg = MahuaEvents.Pick.pick(qq);
                    Task.Factory.StartNew(() => {
                        using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                            {
                                var api = robotSession.MahuaApi;
                                api.SendPrivateMessage(qq, msg);
                            }
                        });
                    if (((i + 1) % 5 == 0) && (i != list.Tables[0].Rows.Count - 1))
                    {
                        Thread.Sleep(1000 * 60);
                    }
                }
            }
        }
    }
}
