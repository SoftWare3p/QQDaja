using Newbe.Mahua.MahuaEvents;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
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
        private static StringBuilder Output = null;
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
                    data.regaccount(context.FromQq, res[1], res[2], context.FromGroup);
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
                 .Text("私聊发送“#账号 教务处账号 密码”以绑定账号")
                 .Newline()
                 .Text("私聊发送“#设置 数字1 数字2 数字3 数字4 数字5”以设置个人健康现状的部分项目（详细请参见此QQ的空间说说）")
                 .Newline()
                 .Text("私聊发送“#解绑”以取消绑定账号")
                 .Newline()
                 .Text("私聊发送“#注册提醒”，如果你当天未使用机器人打卡，机器人将在21点后提醒你，私聊发送“#取消提醒”可取消")
                 .Newline()
                 .Text("私聊发送“#自动打卡”，机器人会在当天中午12点打卡，私聊发送“#取消自动”可取消")
                 .Newline()
                 .Text(@"在群内\私聊发送“#打卡”以打卡")
                 .Newline()
                 .Text(@"在群内\私聊发送#统计信息 以统计打卡信息（当天人数）")
                 .Newline()
                 .Text(@"在群内\私聊发送#程序信息 以查看此程序的一些没什么用的信息")
                 .Done();
            }
            else if (res[0] == "#自动打卡")
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
                        data.Setting(context.FromQq, res, 1);
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
                } else _mahuaApi.SendPrivateMessage(context.FromQq)
                             .Text("指令错误！")
                             .Done();
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
                bool iserr = false;
                DataOprt oprt = new DataOprt();
                string[] str = oprt.pickdaily(context.FromQq);
                if (str[0] == "无")
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("账号尚未绑定！")
                        .Done();
                    return;
                }
                char[] spchar1 = { '[', '0', ']', ' ', '-', ' ', '\r', '\n' };
                string[] outputR = new string[1];
                string attetion = "";
                string isright = "";
                try
                {
                    Output = new StringBuilder();
                    Process myProcess = new Process();
                    myProcess.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + @"\ConsoleWeb.exe", str[0] + " " + str[1] + " " + context.FromQq);
                    myProcess.StartInfo.RedirectStandardOutput = true;
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.OutputDataReceived += myOutputHandler;
                    myProcess.Start();
                    myProcess.BeginOutputReadLine();
                    myProcess.WaitForExit();
                    outputR = Output.ToString().Split(spchar1, options: StringSplitOptions.RemoveEmptyEntries); ;
                    for (int i = 0; i < outputR.Length - 1; i++)
                        attetion += outputR[i];
                    isright = outputR[outputR.Length - 1];
                }
                catch (Exception e)
                {
                    iserr = true;
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("打卡失败，错误原因：" + e.Message.ToString())
                    .Text("\n 请翻阅此QQ的空间了解更新信息~")
                    .Done();
                }
                if (!iserr && isright == "无错")
                {
                    DataOprt oprt1 = new DataOprt();
                    oprt1.recordM(context.FromQq);
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("打卡成功！\n")
                    .Text(attetion)
                    .Text("\n 请翻阅此QQ的空间了解更新信息~")
                    .Done();
                }
                else
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("打卡失败，错误原因：" + Output.ToString())
                    .Text("\n 请翻阅此QQ的空间了解更新信息~")
                    .Done();
                }
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
            else if (res[0] == "#群内查询")
            {
                DataOprt oprt1 = new DataOprt();
                string msg = oprt1.queryGroup(context.FromGroup);
                msg = "当日未打卡QQ:" + msg;
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text(msg)
                    .Done();
                oprt1.Close();
            }
            else if(res[0] == "#注册提醒")
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
            else if (res[0] == "#删除提醒")
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
        private static void myOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the sort command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {

                // Add the text to the collected output.
                Output.Append(Environment.NewLine +
                    $"[{0}] - {outLine.Data}");
            }
        }
    }
}
