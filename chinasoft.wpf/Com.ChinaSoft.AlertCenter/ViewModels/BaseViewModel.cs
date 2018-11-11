using Com.ChinaSoft.Model.ViewModels;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Com.ChinaSoft.AlertCenter.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        [JsonIgnore]
        public DelegateCommand CloseWindowCommand => new DelegateCommand(() => { this.CurrentWindow.Close(); });

    }
}
