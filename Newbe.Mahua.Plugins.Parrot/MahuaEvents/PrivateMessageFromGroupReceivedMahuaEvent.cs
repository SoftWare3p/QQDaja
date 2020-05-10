using Newbe.Mahua.MahuaEvents;
using System;
using System.Threading.Tasks;
namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 来自群成员的私聊消息接收事件
    /// </summary>
    public class PrivateMessageFromGroupReceivedMahuaEvent
        : IPrivateMessageFromGroupReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMessageFromGroupReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(PrivateMessageFromGroupReceivedContext context)
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
                else _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("指令错误！")
                            .Done();
            }
            else if (res[0] == "#help")
            {
                _mahuaApi.SendPrivateMessage(context.FromQq)
                 .Text("私聊发送“#账号 账号 密码”以绑定账号")
                 .Newline()
                 .Text("私聊发送“#设置 数字1 数字2 数字3 数字4 数字5”以设置3.个人健康现状 (2)现居住地状态：->(6)家庭成员状况：项目")
                 .Newline()
                 .Text("私聊发送“#解绑”以取消绑定账号")
                 .Newline()
                 .Text("在群内发送“#打卡”以打卡")
                 .Newline()
                 .Text("在群内发送#统计信息 以统计打卡信息（当天人数）")
                 .Newline()
                 .Text("在群内发送#程序信息 以查看此程序的一些没什么用的信息")
                 .Done();
            }
            else if (res[0] == "#设置")
            {
                if (res.Length == 6)
                {
                    bool isfine = true;
                    DataOprt data = new DataOprt();
                    try
                    {
                        data.Setting(context.FromQq, res, 1);
                    }
                    catch (Exception e)
                    {
                        isfine = false;
                        _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("设置失败！，原因：" + e.Message.ToCharArray())
                            .Done();
                    }
                    if(isfine)
                        _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("设置成功！")
                            .Done();
                    data.Close();
                }else
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("指令错误！")
                            .Done();
            }
        }
    }
}
