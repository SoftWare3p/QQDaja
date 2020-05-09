using Newbe.Mahua.MahuaEvents;
using System;
using System.Threading.Tasks;
namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class PrivateMessageFromFriendReceivedMahuaEvent
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMessageFromFriendReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {
            // todo 填充处理逻辑
            char[] spchar = { ' ' };
            string[] res = context.Message.Split(spchar, options: StringSplitOptions.RemoveEmptyEntries);
            if (res[0] == "#账号")
            {
                if (res.Length == 3)
                {
                    DataOprt data = new DataOprt();
                    data.regaccount(context.FromQq, res[1], res[2]);
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("记录成功！")
                        .Done();
                }
            }
            else if (res[0] == "#help")
            {
                _mahuaApi.SendPrivateMessage(context.FromQq)
                 .Text("私聊发送“#账号 账号 密码”以绑定账号")
                 .Newline()
                 .Text("在群内发送“#打卡”以打卡")
                 .Newline()
                 .Text("在群内发送#统计信息 以统计打卡信息（当天人数）")
                 .Done();
            }
        }
    }
}
