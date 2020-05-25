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
    }
}
