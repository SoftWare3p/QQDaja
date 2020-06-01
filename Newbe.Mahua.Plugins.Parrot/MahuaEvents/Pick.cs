using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    class Pick
    {
        static object locker = new object();
        private static StringBuilder Output = null;
        public static string pick(string qq)
        {
            string result = "";
            bool iserr = false;
            lock (locker)
            {
                DataOprt oprt = new DataOprt();
                string[] str = oprt.pickdaily(qq);
                if (str[0] == "无")
                {
                    result = "账号尚未绑定！";
                    return result;
                }
                char[] spchar1 = { '[', '0', ']', ' ', '-', ' ', '\r', '\n' };
                string[] outputR = new string[1];
                string attetion = "";
                string isright = "";
                try
                {
                    Output = new StringBuilder();
                    Process myProcess = new Process();
                    myProcess.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + @"\ConsoleWeb.exe", str[0] + " " + str[1] + " " + qq);
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
                    result = "打卡失败，错误原因：" + e.Message.ToString() + "\n 请翻阅此QQ的空间了解更新信息~";
                }
                if (!iserr && isright == "无错")
                {
                    DataOprt oprt1 = new DataOprt();
                    oprt1.recordM(qq);
                    result = "打卡成功！\n" + attetion + "\n 请翻阅此QQ的空间了解更新信息~";
                }
                else
                {
                    result = "打卡失败，错误原因：" + Output.ToString() + "\n 请翻阅此QQ的空间了解更新信息~";
                }
            }
            return result;
        }
        private static void myOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the sort command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {

                // Add the text to the collected output.
                Output.Append(Environment.NewLine +
                    $"[{0}] - {outLine.Data}");
            }
        }
        public static string Help()
        {
            return "私聊发送“#账号 教务处账号 密码”（如 #账号 2018051041 JD098432a）以绑定账号\n"+"私聊发送“#账号 教务处账号 密码 群号”以在好友私聊模式下绑定来自某个群成员的账号\n"
                +"私聊发送“#设置 数字1 数字2 数字3 数字4 数字5”以设置个人健康现状的部分项目（详细请参见此QQ的空间说说）\n"+"私聊发送“#解绑”以取消绑定账号\n"
                 +"私聊发送“#注册提醒”，如果你当天未使用机器人打卡，机器人将在21点后提醒你，私聊发送“#取消提醒”可取消\n"
                 +"私聊发送“#自动打卡”，机器人会在当天中午12点打卡，私聊发送“#取消自动”可取消\n"
                 +"私聊发送“#反馈 反馈信息” 可发送反馈\n"
                 +"在群内/私聊发送“#打卡”以打卡\n"
                 + "在群内/私聊发送#统计信息 以统计打卡信息（当天人数）\n" + "在群内/私聊发送#程序信息 以查看此程序的一些没什么用的信息";
        }
    }
}
