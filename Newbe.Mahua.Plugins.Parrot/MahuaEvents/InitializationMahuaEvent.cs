using Newbe.Mahua.MahuaEvents;
using System;

namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public InitializationMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Initialized(InitializedContext context)
        {
            // todo 填充处理逻辑
            System.Timers.Timer t = new System.Timers.Timer(1000 * 60 * 60);
            t.Elapsed += new System.Timers.ElapsedEventHandler(theout);
            t.AutoReset = true;
            t.Enabled = true;
        }
        private void theout(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (System.DateTime.Now.Hour >= 21)
            {
                Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt dataOprt = new DataOprt();
                System.Data.DataSet list = dataOprt.getPushList();
                for (int i = 0; i < list.Tables[0].Rows.Count; i++)
                {
                    if (dataOprt.verQQ(list.Tables[0].Rows[i][0].ToString()) >= 0)
                    {
                        _mahuaApi.SendPrivateMessage(list.Tables[0].Rows[i][0].ToString())
                            .Text("已经到" + System.DateTime.Now.Hour + "点了！")
                            .Newline()
                            .Text("你今天尚未使用本助手打卡！")
                            .Done();
                    }
                }
            }
        }
    }
}
