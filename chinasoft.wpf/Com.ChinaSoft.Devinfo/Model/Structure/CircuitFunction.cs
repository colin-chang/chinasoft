using Com.ChinaSoft.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 电路图元器件功能
    /// </summary>
    public class CircuitFunction : IAlertViewItem
    {
        public string Id { get; set; }

        //private string id;
        //public string Id
        //{
        //    get { return CircuitCode; }
        //    set { id = value; }
        //}

        public string Name { get; set; }

        public string Code { get; set; }

        public string ResourceCode { get; set; }

        public string CircuitCode { get; set; }

        public int IsMain { get; set; }

        public string Text
        {
            get { return this.Name; }
            set { this.Name = value; }
        }

        public object Tag
        {
            get { return this.ResourceCode; }
            set { this.ResourceCode = value as string; }
        }
    }
}
