using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Newbe.Mahua.Plugins.Parrot.MahuaService
{
    public interface ITickServices
    {
        Task StartAsync();
    }
    class TickServices: ITickServices
    {
        private readonly IMahuaApi _mahuaApi;
        public TickServices(IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;       
        }
        System.Timers.Timer t;
        public Task StartAsync()
        {
            t = new System.Timers.Timer(1000*60*60);//*60*60
            t.Elapsed += new System.Timers.ElapsedEventHandler(Theout);
            t.AutoReset = true;//true
            t.Enabled = true;
            _mahuaApi.SendPrivateMessage(@"1307650694")
            .Text(@"初始化成功")
            .Done();
            return Task.FromResult(0);
        }
        private static StringBuilder Output = null;
        private static void Theout(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (System.DateTime.Now.Hour >= 21)
            {
                Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt dataOprt = new Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt();
                System.Data.DataSet list = dataOprt.getPushList();
                for (int i = 0; i < list.Tables[0].Rows.Count; i++)
                {
                    string qq = list.Tables[0].Rows[i][0].ToString();
                    if (dataOprt.verQQ(qq) == -1)
                    {
                        Task.Factory.StartNew(() => {
                            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                            {
                                var api = robotSession.MahuaApi;
                                api.SendPrivateMessage(qq,"你今天尚未使用本助手打卡！");
                            } });
                    }
                }
                Task.Factory.StartNew(() => {
                    using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                    {
                        var api = robotSession.MahuaApi;
                        api.SendPrivateMessage("1307650694", "打卡提醒已经发送！");
                    }
                });
                dataOprt.Close();
            }
            else if (System.DateTime.Now.Hour == 3)
            {
                
                Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt dataOprt = new Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt();
                
                System.Data.DataSet list = dataOprt.getAutoList();
                dataOprt.Close();
                for (int i = 0; i < list.Tables[0].Rows.Count; i++)
                {
                    string qq = list.Tables[0].Rows[i][0].ToString();
                    bool iserr = false;
                    dataOprt = new Newbe.Mahua.Plugins.Parrot.MahuaEvents.DataOprt();
                    string[] str = dataOprt.pickdaily(qq);
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
                        outputR = Output.ToString().Split(spchar1, options: StringSplitOptions.RemoveEmptyEntries);
                        for (int i1 = 0; i1 < outputR.Length - 1; i1++)
                            attetion += outputR[i1];
                        isright = outputR[outputR.Length - 1];
                    }
                    catch (Exception ee)
                    {
                        iserr = true;
                        Task.Factory.StartNew(() => {
                            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                            {
                                var api = robotSession.MahuaApi;
                                api.SendPrivateMessage(qq, "打卡失败，错误原因：" + ee.Message.ToString());
                            }
                        });
                    }
                    if (!iserr && isright == "无错")
                    {
                        MahuaEvents.DataOprt oprt1 = new MahuaEvents.DataOprt();
                        oprt1.recordM(qq);
                        Task.Factory.StartNew(() => {
                            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                            {
                                var api = robotSession.MahuaApi;
                                api.SendPrivateMessage(qq, "打卡成功！\n" + attetion);
                            }
                        });
                    }
                    else
                    {
                        Task.Factory.StartNew(() => {
                            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                            {
                                var api = robotSession.MahuaApi;
                                api.SendPrivateMessage(qq, "打卡失败，错误原因：" + Output.ToString());
                            }
                        });
                    }                 
                }
            }
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
