﻿using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.Views;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public abstract class LazyLoadingBaseViewModel : LazyLoadingViewModel
    {
        public string EquipmentName
        {
            get
            {
                string en = Session["EquipmentName"] as string;
                if (string.IsNullOrWhiteSpace(en))
                {
                    string url = Path.Combine(this.ApiBaseUrl, "equipment", "info", "detail", this.ModuleId).ToUrl();
                    try
                    {
                        var task = this.HttpService.GetAsync(url, null, true);
                        task.Wait();
                        if (string.IsNullOrWhiteSpace(task.Result))
                        {
                            MessageBox.Show("抱歉，请求网络数据异常！\r\n错误消息：数据为空", "数据错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            return en;
                        }
                        var result = JsonConvert.DeserializeObject<DeviceOverview>(task.Result);
                        en = result.EquipmentName;
                        Session["EquipmentName"] = en;
                        return en;
                    }
                    catch (Exception ex)
                    {
                        ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                        return en;
                    }
                }
                return en;
            }
            set
            {
                Session["EquipmentName"] = value;
            }
        }

        public override UserControl MainUC => new MainUC();
    }
}
