using Newbe.Mahua.MahuaEvents;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMessageReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }
        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            // todo 填充处理逻辑
            char[] spchar = { ' ' };
            string[] res = context.Message.Split(spchar, options: StringSplitOptions.RemoveEmptyEntries);
            if (res[0] == "#统计信息")
            {
                DataOprt dtoprt = new DataOprt();
                string[] result = dtoprt.queryuse();
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Text(result[0] + "打卡人数：" + result[1])
                    .Done();
            }else if (res[0] == "#打卡")
            {
                string msg = Pick.pick(context.FromQq);
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Newline()
                    .Text(msg)
                    .Done();
            } else if (res[0] == "#robot_path")
            {
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Text(System.Environment.CurrentDirectory)
                    .Done();
            } else if (res[0] == "#程序信息")
            {
                PluginInfo pluginInfo = new PluginInfo();
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Newline()
                    .Text("Name:" + pluginInfo.Name)
                    .Newline()
                    .Text("Version:" + pluginInfo.Version)
                    .Newline()
                    .Text("id:" + pluginInfo.Id)
                    .Newline()
                    .Text("Description:" + pluginInfo.Description)
                    .Done();
            }
        }
    }
    
}
