using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 专家库节点包含的可视化树
    /// </summary>
    public class ExperienceBaseNodeVisualTree
    {
        public IList<Button> Exps { get; set; }

        public Path ExpLines { get; set; }

        /// <summary>
        /// Boms结构，每个Bom节点包括一个按钮和附加在按钮前面的水平连接短线(ConnectionLine)
        /// </summary>
        public IList<Button> Boms { get; set; }

        /// <summary>
        /// Boms节点前的垂直连接线
        /// </summary>
        public Line BomsVerticalLink { get; set; }

        /// <summary>
        /// 根节点后的水平连接线
        /// </summary>
        public Line RootConnectionLine { get; set; }

        public ExperienceBaseNodeVisualTree()
        {
            this.Exps = new List<Button>();
            this.Boms = new List<Button>();
        }
    }
}
