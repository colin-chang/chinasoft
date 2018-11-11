using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using Com.ChinaSoft.Utility.Win32;

namespace Com.ChinaSoft.Utility.Communication
{
    public class MessageHelper
    {
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage
        (
            IntPtr hWnd,                   //目标窗体句柄
            int Msg,                       //WM_COPYDATA
            IntPtr wParam,                 //自定义数值
            ref  CopyDataStruct lParam     //结构体
        );

        /// <summary>
        /// 跨进程发送消息
        /// </summary>
        /// <param name="targetProcessId">接收方进程ID</param>
        /// <param name="strMsg">消息内容</param>
        public static void SendMessage(int targetProcessId, IntPtr fromHandle, string msg)
        {
            var process = Process.GetProcessById(targetProcessId);
            if (process == null)
                return;

            SendMessage(process.MainWindowHandle, fromHandle, msg);
        }

        /// <summary>
        /// 向指定窗口句柄发送消息
        /// </summary>
        /// <param name="targetHandle">接收方窗口句柄</param>
        /// <param name="msg">消息内容</param>
        public static void SendMessage(IntPtr targetHandle, IntPtr fromHandle, string msg)
        {
            if (targetHandle == null || targetHandle == IntPtr.Zero)
                return;

            msg = msg ?? string.Empty;

            CopyDataStruct cds = new CopyDataStruct();
            cds.dwData = IntPtr.Zero;
            cds.lpData = msg;
            cds.cbData = Encoding.Default.GetBytes(msg).Length + 1;
            SendMessage(targetHandle, Win32API.WM_COPYDATA, fromHandle, ref cds);
        }
    }
}
