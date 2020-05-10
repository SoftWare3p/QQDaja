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
        private static StringBuilder Output = null;
        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            // todo 填充处理逻辑
            char[] spchar = { ' ' };
            string[] res = context.Message.Split(spchar, options: StringSplitOptions.RemoveEmptyEntries);
            if(res[0] == "#统计信息")
            {
                DataOprt dtoprt = new DataOprt();
                string [] result = dtoprt.queryuse();
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Text(result[0] + "打卡人数：" + result[1])
                    .Done();
            }else if (res[0] == "#打卡")
            {
                bool iserr = false;
                DataOprt oprt = new DataOprt();
                string[] str = oprt.pickdaily(context.FromQq);
                if (str[0] == "无")
                {
                    _mahuaApi.SendGroupMessage(context.FromGroup)
                        .At(context.FromQq)
                        .Text("账号尚未绑定！")
                        .Done();
                    return;
                }
                char[] spchar1 = { '[','0',']',' ','-',' ','\r','\n'};
                string[] outputR = new string[1];
                string attetion ="";
                string isright = "";
                try
                {
                    Output = new StringBuilder();
                    Process myProcess = new Process();
                    myProcess.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory+@"\ConsoleWeb.exe",str[0]+" "+str[1]+" "+context.FromQq);
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
                catch(Exception e)
                {
                    iserr = true;
                    _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Newline()
                    .Text("打卡失败，错误原因：" + e.Message.ToString())
                    .Done();
                }
                if (!iserr && isright == "无错")
                {
                    DataOprt oprt1 = new DataOprt();
                    oprt1.recordM(context.FromQq);
                    _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Newline()
                    .Text("打卡成功！\n")
                    .Text(attetion)
                    .Done();
                }
                else
                {
                    _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Newline()
                    .Text("打卡失败，错误原因：" + Output.ToString())
                    .Done();
                }
            }else if(res[0] == "#robot_path")
            {
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Text(System.Environment.CurrentDirectory)
                    .Done();
            }else if(res[0] == "#程序信息")
            {
                PluginInfo pluginInfo = new PluginInfo();
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Newline()
                    .Text("Name:"+pluginInfo.Name)
                    .Newline()
                    .Text("Version:" +pluginInfo.Version)
                    .Newline()
                    .Text("id:" +pluginInfo.Id)
                    .Newline()
                    .Text("Description:" +pluginInfo.Description)
                    .Done();        
            }
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
