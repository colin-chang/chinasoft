using System.Linq;
using System.Windows;

namespace Com.ChinaSoft.UserControls.ConfirmBox
{
    /// <summary>
    /// 消息框处理类
    /// </summary>
    public class ConfirmBox
    {
        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="content">对话框内容</param>
        /// <param name="title">标题</param>
        /// <param name="msgBoxBtn">在消息框中要显示的按钮</param>
        /// <param name="msgBoxImage">消息框指示图标</param>
        /// <returns>消息窗口的DialogResult</returns>
        public static bool Show(string content, string title, MessageBoxButton msgBoxBtn, MessageBoxImage msgBoxImage)
        {
            var win = new ConfirmWindow(content, title, msgBoxBtn, msgBoxImage)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive)
            };

            bool? retVal = win.ShowDialog();
            return retVal.HasValue && retVal.Value;
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="content">对话框内容</param>
        /// <returns>消息窗口的DialogResult</returns>
        public static bool Show(string content)
        {
            return Show(content, "数据错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="content">消息框内容</param>
        /// <param name="title">消息框标题</param>
        /// <returns>消息窗口的DialogResult</returns>
        public static bool Show(string content, string title)
        {
            return Show(content, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="content">对话框内容</param>
        /// <param name="title">标题</param>
        /// <param name="msgBoxBtn">在消息框中要显示的按钮</param>
        /// <returns>消息窗口的DialogResult</returns>
        public static bool Show(string content, string title, MessageBoxButton msgBoxBtn)
        {
            return Show(content, title, msgBoxBtn, MessageBoxImage.Information);
        }
    }
}