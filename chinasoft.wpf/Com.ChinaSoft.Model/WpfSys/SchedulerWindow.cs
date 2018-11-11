using Com.ChinaSoft.Utility.Communication;
using Com.ChinaSoft.Utility.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Com.ChinaSoft.Model.WpfSys
{
    /// <summary>
    /// 调度程序MainWindow基类
    /// </summary>
    public abstract class SchedulerWindow : Window
    {
        HwndSource hwndSource;
        protected string msg;//子程序发送的消息内容
        IntPtr childAppHwnd;//子程序主窗口句柄

        public SchedulerWindow()
        {
            this.Loaded += (sender, e) => (hwndSource = PresentationSource.FromVisual(this) as HwndSource).AddHook(new HwndSourceHook(this.WndProc));

            Application.Current.Exit += (sender, e) => this.CloseChild();
        }

        /// <summary>
        /// 接收消息处理
        /// </summary>
        /// <param name="hwnd">消息接收者句柄</param>
        /// <param name="msg">消息内容</param>
        /// <param name="wParam">消息发送者句柄</param>
        /// <param name="lParam">消息参数</param>
        /// <param name="handled">消息是否被处理</param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32API.WM_COPYDATA)
            {
                CopyDataStruct cds = (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                this.msg = cds.lpData;
                childAppHwnd = wParam;
            }
            return hwnd;
        }

        /// <summary>
        /// 关闭子程序
        /// </summary>
        void CloseChild()
        {
            if (childAppHwnd != IntPtr.Zero)
                Win32API.PostMessage(childAppHwnd, Win32API.WM_CLOSE, 0, 0);
        }

        /// <summary>
        /// 启动子系统
        /// </summary>
        /// <param name="fileName">子系统exe文件绝对地址</param>
        /// <param name="args">子系统初始化参数</param>
        public void StartChildApp(string fileName, params string[] args)
        {
            this.CloseChild();

            List<string> paras = new List<string> { hwndSource.Handle.ToString() };
            if (args != null && args.Length > 0)
                paras.AddRange(args);

            Process.Start(fileName, new StartupArgs(null, paras.ToArray()).ToString());
        }
    }
}
