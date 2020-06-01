using Newbe.Mahua.MahuaEvents;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Text;
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
                else if(res.Length == 4)
                {
                    DataOprt data = new DataOprt();
                    data.regaccount(context.FromQq, res[1], res[2], res[3]);
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("记录成功！")
                        .Done();
                }
                else _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("指令错误！")
                            .Done();
            }
            else if(res[0] == "#设置" && context.FromQq == "1307650694")
            {
                if(res[1] == "自动打卡")
                {
                    DataOprt oprt = new DataOprt();
                    string[] str = oprt.pickdaily(res[2]);
                    if (str[0] == "无")
                    {
                        _mahuaApi.SendPrivateMessage(res[2])
                            .Text("账号尚未绑定！")
                            .Done();
                        return;
                    }
                    oprt = new DataOprt();
                    if (oprt.SetAuto(res[2]))
                        _mahuaApi.SendPrivateMessage(res[2])
                            .Text("设定自动打卡成功！")
                            .Done();
                    else _mahuaApi.SendPrivateMessage(res[2])
                            .Text("你已经设定了自动打卡！")
                            .Done();
                    oprt.Close();
                }
                else if(res[1] == "解绑账号")
                {
                    DataOprt data = new DataOprt();
                    data.deleteAcc(res[2]);
                }
            }
            else if(res[0] == "#回复" && context.FromQq == "1307650694")
            {
                _mahuaApi.SendPrivateMessage(res[1])
                    .Text(res[2])
                    .Done();
            }
            else if (res[0] == "#反馈")
            {
                _mahuaApi.SendPrivateMessage(@"1307650694")
                    .Text(context.FromQq) .Newline()
                                .Text(context.Message)
                                .Done();
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("反馈成功！")
                    .Done();
            }
            else if (res[0] == "#help")
            {
                _mahuaApi.SendPrivateMessage(context.FromQq)
                 .Text(Pick.Help())
                 .Done();
            }
            else if(res[0] == "#自动打卡")
            {
                DataOprt oprt = new DataOprt();
                string[] str = oprt.pickdaily(context.FromQq);
                if (str[0] == "无")
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("账号尚未绑定！")
                        .Done();
                    return;
                }
                oprt = new DataOprt();
                if (oprt.SetAuto(context.FromQq))
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("设定自动打卡成功！")
                        .Done();
                else _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("你已经设定了自动打卡！")
                        .Done();
                oprt.Close();
            }
            else if (res[0] == "#取消自动")
            {
                DataOprt oprt = new DataOprt();
                if (oprt.DelAuto(context.FromQq))
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("成功取消自动打卡！")
                        .Done();
                else _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("你设定自动打卡！")
                        .Done();
                oprt.Close();
            }
            else if (res[0] == "#设置")
            {
                if (res.Length == 6)
                {
                    bool isfine = true;
                    DataOprt data = new DataOprt();
                    try
                    {
                        data.Setting(context.FromQq, res,1);
                    }
                    catch (Exception e)
                    {
                        isfine = false;
                        _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("设置失败！，原因：" + e.Message.ToCharArray())
                            .Done();
                    }
                    if (isfine)
                        _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("设置成功！")
                            .Done();
                    data.Close();
                }
                else _mahuaApi.SendPrivateMessage(context.FromQq)
                            .Text("指令错误！")
                            .Done();
            }
            else if(res[0] == "#解绑")
            {
                DataOprt data = new DataOprt();
                if(data.deleteAcc(context.FromQq) == 0)
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                           .Text("你还没有绑定账户！")
                           .Done();
                }
                else
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                           .Text("解除绑定成功！")
                           .Done();
                }
            }
            else if (res[0] == "#统计信息")
            {
                DataOprt dtoprt = new DataOprt();
                string[] result = dtoprt.queryuse();
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text(result[0] + "打卡人数：" + result[1])
                    .Done();
            }
            else if (res[0] == "#打卡")
            {
                string msg = Pick.pick(context.FromQq);
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text(msg)
                    .Done();
                return;
            }
            else if (res[0] == "#robot_path")
            {
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text(System.Environment.CurrentDirectory)
                    .Done();
            }
            else if (res[0] == "#程序信息")
            {
                PluginInfo pluginInfo = new PluginInfo();
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("Name:" + pluginInfo.Name)
                    .Newline()
                    .Text("Version:" + pluginInfo.Version)
                    .Newline()
                    .Text("id:" + pluginInfo.Id)
                    .Newline()
                    .Text("Description:" + pluginInfo.Description)
                    .Newline()
                    .Text(@"开源地址：https://github.com/SoftWare3p/QQDaja")
                    .Done();
            }
            else if (res[0] == "#注册提醒")
            {
                DataOprt oprt = new DataOprt();
                string[] str = oprt.pickdaily(context.FromQq);
                if (str[0] == "无")
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("账号尚未绑定！")
                        .Done();
                    return;
                }
                oprt = new DataOprt();
                if (oprt.RecordTips(context.FromQq))
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("注册提醒成功！")
                        .Done();
                }
                else
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                       .Text("该QQ已经注册！")
                       .Done();
                }
            }
            else if (res[0] == "#取消提醒")
            {
                DataOprt oprt = new DataOprt();
                if (oprt.DeleteTips(context.FromQq))
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("删除提醒成功！")
                        .Done();
                else _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("该QQ未曾注册提醒！")
                        .Done();
                oprt.Close();
            }
            else
                _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("指令错误！")
                        .Done();
        }
        
    }
}
