using System.Windows;
using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;

namespace Com.ChinaSoft.Devinfo.Helper
{
    [ComVisible(true)]
    public class ObjectForScripting
    {
        Action<string> changeCode;
        Func<string, Task> loadCircuitFuns;
        public ObjectForScripting(Action<string> changeCode, Func<string, Task> loadCircuitFuns)
        {
            this.changeCode = changeCode;
            this.loadCircuitFuns = loadCircuitFuns;
        }

        /// <summary>
        /// 选中电路图元件
        /// </summary>
        /// <param name="code"></param>
        public void SelectCircuit(string code)
        {
            if (changeCode != null)
                changeCode(code);
        }

        /// <summary>
        /// 双击电路图元件
        /// </summary>
        /// <param name="code"></param>
        public async void SelectFunction(string code)
        {
            if (loadCircuitFuns != null)
                await loadCircuitFuns(code);
        }
    }
}
