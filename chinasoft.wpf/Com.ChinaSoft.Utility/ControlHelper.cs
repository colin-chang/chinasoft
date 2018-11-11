using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Com.ChinaSoft.Utility
{
    public static class ControlHelper
    {
        /// <summary>
        /// 查找父控件中指定类型的子控件集合
        /// </summary>
        /// <typeparam name="T"><see cref="T"/>子控件的类型</typeparam>
        /// <param name="parent">父控件</param>
        /// <param name="childType">指定类型</param>
        /// <returns>子控件集合</returns>
        public static List<T> FindChildObjects<T>(DependencyObject parent, Type childType) where T : FrameworkElement
        {
            DependencyObject child = null;

            List<T> childList = new List<T>();

            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int idx = 0; idx < childCount; idx++)
            {
                child = VisualTreeHelper.GetChild(parent, idx);
                var item = child as T;
                if (item != null && item.GetType() == childType)
                    childList.Add(item);

                childList.AddRange(FindChildObjects<T>(child, childType));
            }

            return childList;
        }
    }
}