using Newbe.Mahua.MahuaEvents;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
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
        private static StringBuilder Output = null;
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
                 .Text(@"在群内\私聊发送“#打卡”以打卡")
                 .Newline()
                 .Text(@"在群内\私聊发送#统计信息 以统计打卡信息（当天人数）")
                 .Newline()
                 .Text(@"在群内\私聊发送#程序信息 以查看此程序的一些没什么用的信息")
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
                    .Newline()
                    .Text("打卡失败，错误原因：" + e.Message.ToString())
                    .Done();
                }
                if (!iserr && isright == "无错")
                {
                    DataOprt oprt1 = new DataOprt();
                    oprt1.recordM(context.FromQq);
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Newline()
                    .Text("打卡成功！\n")
                    .Text(attetion)
                    .Done();
                }
                else
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Newline()
                    .Text("打卡失败，错误原因：" + Output.ToString())
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
                    .Done();
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
