using System;
using System.Diagnostics;
using System.IO;
using CefSharp.OffScreen;
using System.Threading;
using System.Runtime.CompilerServices;
using CefSharp;
using System.Reflection;

namespace WebVisit
{
    public class WebVisit
    {
        public static bool isChecked = true;
        public static string errstring;
        private static AutoResetEvent DocComplete = new AutoResetEvent(false);
        public WebVisit()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            //InitializeCefSharp();
        }
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
            DocComplete.Set();
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
            script = @"document.getElementsByTagName('a')[1].click();";
            browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
            DocComplete.WaitOne();
            browser.JsDialogHandler = new JsDialogHandler();
            script = @"function setV(na, val){document.getElementsByName(na)[0].value = val} setV('sF21650_5','1'); setV('sF21650_6','5'); setV('sF21650_7','1');setV('sF21650_8','1');setV('sF21650_9','1'); FrmXsPj.submit();";
            browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
            script = @"window.alert=function(){};";
            browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();

            // Set BrowserSubProcessPath based on app bitness at runtime
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");

            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
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
