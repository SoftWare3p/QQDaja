using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ConsoleWeb
{
    class Program
    {

        static void Main(string[] args)
        {
            bool isr = true;
            try
            {
                WebVis web = new WebVis();
                web.daka(args);
            }
            catch (Exception e)
            {
                isr = false;
                Console.WriteLine(e.Message.ToString());
            }
            if (isr) Console.WriteLine("无错");
        }
        //}
        class WebVis
        {
            private static AutoResetEvent DocComplete = new AutoResetEvent(false);
            string source;
            public static void initWebVisit()
            {
                var settings = new CefSettings()
                {
                    //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                    Locale = "zh-CN",
                    BrowserSubprocessPath = @"\CefSharp.BrowserSubprocess.exe",
                    CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
                };
                CefSharp.Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }
            public ChromiumWebBrowser browser;
            async void browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
            {
                source = await browser.GetSourceAsync();
                DocComplete.Set();
            }
            void getNearest()
            {
                int index = source.IndexOf(@">&nbsp;</td>");
                if(index == -1)
                {
                    Console.WriteLine(@"没有遗漏打卡的记录");
                    return;
                }
                string res = source.Substring(index);
                index = res.IndexOf(@"疫情防控——师生健康状态采集");
                res = res.Substring(index-4,4);
                Console.WriteLine(@"你最近一次遗漏打卡的时间是在" + res[0]+res[1]+"月"+res[2]+res[3]+"日");
            }
            public void daka(string[] str)
            {
                //initWebVisit();
                browser = new ChromiumWebBrowser("http://login.cuit.edu.cn/Login/xLogin/Login.asp");
                browser.FrameLoadEnd += browser_FrameLoadEnd;
                DocComplete.WaitOne();
                string script = @"FmLgn.txtId.value = '" + str[0] + "'; FmLgn.txtMM.value = '" + str[1] + "'; FmLgn.txtVC.value = '1';FmLgn.submit();";
                browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
                DocComplete.WaitOne();
                DocComplete.WaitOne();
                DocComplete.WaitOne();
                browser.Load("http://jszx-jxpt.cuit.edu.cn/Jxgl/Xs/netks/sj.asp");
                DocComplete.WaitOne();
                getNearest();
                script = @"document.getElementsByTagName('a')[1].click();";
                browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
                DocComplete.WaitOne();
                browser.JsDialogHandler = new JsDialogHandler();
                script = @"function setV(na, val){document.getElementsByName(na)[0].value = val} setV('sF21650_5','1'); setV('sF21650_6','5'); setV('sF21650_7','1');setV('sF21650_8','1');setV('sF21650_9','1'); FrmXsPj.submit();";
                browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
                while (browser.Address != @"http://jszx-jxpt.cuit.edu.cn/Jxgl/Xs/netks/editSjRs.asp");
                browser.GetBrowser().CloseBrowser(true);
            }

        }
        class JsDialogHandler : CefSharp.IJsDialogHandler
        {
            public bool OnBeforeUnloadDialog(CefSharp.IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, string messageText, bool isReload, CefSharp.IJsDialogCallback callback)
            {
                return true;
            }
            public void OnDialogClosed(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
            {
            }

            public bool OnJSBeforeUnload(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string message, bool isReload, CefSharp.IJsDialogCallback callback)
            {
                return true;
            }

            public bool OnJSDialog(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string originUrl, CefSharp.CefJsDialogType dialogType, string messageText, string defaultPromptText, CefSharp.IJsDialogCallback callback, ref bool suppressMessage)
            {
                return true;
            }

            public void OnResetDialogState(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
            {

            }
        }
    }
}
