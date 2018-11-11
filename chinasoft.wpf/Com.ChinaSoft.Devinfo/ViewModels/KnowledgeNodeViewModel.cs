using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public class KnowledgeNodeViewModel : BaseViewModel
    {
        public KnowledgeNodeViewModel(string text)
        {
            this.Text = text;
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { SetProperty<string>(ref text, value); }
        }

        public DelegateCommand CloseCommand { get { return new DelegateCommand(() => this.CurrentWindow.DialogResult = false); } }
    }
}
