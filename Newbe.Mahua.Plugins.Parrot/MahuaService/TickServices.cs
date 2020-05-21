using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Newbe.Mahua.Plugins.Parrot.MahuaService
{
    public interface ITickServices
    {
        Task StartAsync();
    }
    class TickServices: ITickServices
    {
        private readonly IMahuaApi _mahuaApi;
        public TickServices(IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;       
        }
        System.Timers.Timer t;
        public Task StartAsync()
        {
            t = new System.Timers.Timer(1000*60*60);//*60*60
            t.Elapsed += new System.Timers.ElapsedEventHandler(Theout);
            t.AutoReset = true;//true
            t.Enabled = true;
            _mahuaApi.SendPrivateMessage(@"1307650694")
            .Text(@"初始化成功")
            .Done();
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
                    if (dataOprt.verQQ(list.Tables[0].Rows[i][0].ToString()) == -1)
                    {
                        Task.Factory.StartNew(() => {
                            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                            {
                                var api = robotSession.MahuaApi;
                                api.SendPrivateMessage(list.Tables[0].Rows[i][0].ToString(), "你今天尚未使用本助手打卡！");
                            } });
                    }
                }
                Task.Factory.StartNew(() => {
                    using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                    {
                        var api = robotSession.MahuaApi;
                        api.SendPrivateMessage("1307650694", "我就看看提醒灵不灵" + System.DateTime.Now.Hour.ToString());
                    }
                });
                dataOprt.Close();
            } 
        }
    }
}
