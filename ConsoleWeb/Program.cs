﻿using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text.RegularExpressions;

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
            public ChromiumWebBrowser browser;
            async void browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
            {
                source = await browser.GetSourceAsync();
                DocComplete.Set();
            }
            bool isf = false;
            void getNearest(string s, int index)
            { 
                int index1 = s.IndexOf(@">&nbsp;</td>");
                if (index1 == -1)
                {
                    Console.WriteLine(@"没有遗漏打卡的记录");
                    return;
                }
                s = s.Substring(index1);
                index1 = 0;
                int pos = s.IndexOf(@"成都信息工程大学2020年秋季学期");
                int index2 = s.IndexOf(@"疫情防控——师生健康状态采集");
                if (pos != -1)
                {
                    if (pos > index1 && pos < index2)
                    {
                        s = s.Substring(pos+1);
                        getNearest(s, 0);
                        return;
                    }
                }
                string res = s.Substring(index2 - 4, 4);
                int day = (res[2] - '0') * 10 + (res[3] - '0');
                int month = (res[0] - '0') * 10 + (res[1] - '0');
                if (System.DateTime.Now.Day == day && System.DateTime.Now.Month == month)
                {
                    isf = true;
                    s = s.Substring(index2+1);
                    getNearest(s, 0);
                    return;
                }
                else
                {
                    if(!isf)
                        Console.WriteLine(@"你今天已经录入过健康打卡，本次可能为你覆盖了之前的设置。此外");
                    DateTime dNow = DateTime.Now;
                    int yearadd = 0;
                    if (dNow.Month < month) yearadd = 1;
                    DateTime dpre = new DateTime(dNow.Year-yearadd, month, day, dNow.Hour, dNow.Minute, dNow.Second);
                    if((dNow - dpre).Days > 15)
                    {
                        Console.WriteLine(@"你最近15天没有遗漏打卡的记录");
                    }
                    else
                    {
                        Console.WriteLine(@"你最近一次遗漏打卡的时间是在" + month + "月" + day + "日");
                    }
                }
            }
            private static void theout(object sender, System.Timers.ElapsedEventArgs e)
            {
                Console.WriteLine("响应时间大于10秒，可能为账号信息错误");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            public void daka(string[] str)
            {
                isf = false;
                System.Timers.Timer t = new System.Timers.Timer(10000);
                //t.Elapsed += new System.Timers.ElapsedEventHandler(theout);
                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)； 
                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                browser = new ChromiumWebBrowser(@"http://login.cuit.edu.cn/Login/xLogin/Login.asp");
                browser.FrameLoadEnd += browser_FrameLoadEnd;
                DocComplete.WaitOne();
                string script = @"FmLgn.txtId.value = '" + str[0] + "'; FmLgn.txtMM.value = '" + str[1] + "'; FmLgn.txtVC.value = '1';FmLgn.submit();";
                browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
                DocComplete.WaitOne();
                DocComplete.WaitOne();
                DocComplete.WaitOne();
                browser.Load(@"http://jszx-jxpt.cuit.edu.cn/Jxgl/Xs/netks/sj.asp");
                DocComplete.WaitOne();
                getNearest(source, 0);
                script = @"document.getElementsByTagName('a')[0].click();";
                browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
                DocComplete.WaitOne();
                browser.JsDialogHandler = new JsDialogHandler();
                DataOprt dataOprt = new DataOprt();
                string [] sett  = dataOprt.Setting(str[2]);
                script = @"function setV(na, val){document.getElementsByName(na)[0].value = val} setV('sF21650_5','"+sett[0]+ "'); setV('sF21650_6','" + sett[1] + "'); setV('sF21650_7','" + sett[2] + "');setV('sF21650_8','" + sett[3] + "');setV('sF21650_9','" + sett[4] + "'); FrmXsPj.submit();";
                browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);
                while (browser.Address != @"http://jszx-jxpt.cuit.edu.cn/Jxgl/Xs/netks/editSjRs.asp") ;
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
