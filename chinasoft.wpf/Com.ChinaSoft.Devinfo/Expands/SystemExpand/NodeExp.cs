using Com.ChinaSoft.Devinfo.Expands.AttcahProperty;
using Com.ChinaSoft.Devinfo.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    /// 专家经验库节点功能扩展
    /// </summary>
    public static class NodeExp
    {
        #region 数据上下文

        /// <summary>
        /// 获取节点数据上下文
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ExperienceBaseNode GetContext(this Button node)
        {
            var context = node?.DataContext as ExperienceBaseNode;
            return context == null ? new ExperienceBaseNode() : context;
        }

        /// <summary>
        /// 获取节点层次级别
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static float GetLevel(this Button node) => node.GetContext().Level;

        /// <summary>
        /// 获取节点折叠状态
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static CollapseStatusEnum GetCollapseStatus(this Button node) => node.GetContext().CollapseStatus;

        /// <summary>
        /// 设置节点折叠状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="status"></param>
        public static void SetCollapseStatus(this Button node, CollapseStatusEnum status) => node.GetContext().CollapseStatus = status;

        /// <summary>
        /// 设置节点样式
        /// </summary>
        /// <param name="node"></param>
        /// <param name="resources"></param>
        /// <param name="style"></param>
        public static void SetStyle(this Button node, ResourceDictionary resources, NodeStyleEnum style)
        {
            string key = string.Empty;
            switch (style)
            {
                case NodeStyleEnum.Root:
                    key = "button-root";
                    break;
                case NodeStyleEnum.ExpandRoot:
                    key = "button-expand-root";
                    break;
                case NodeStyleEnum.Leaf:
                    key = "button-leaf";
                    break;
                case NodeStyleEnum.ExpandExp:
                    key = "button-expand-exp";
                    break;
                case NodeStyleEnum.ExpandBom:
                    key = "button-expand-bom";
                    break;
                case NodeStyleEnum.Collapsed:
                    key = "button-collapsed";
                    break;
                default:
                    return;
            }

            var sty = resources[key] as Style;
            if (sty == null)
                return;

            node.Style = sty;
        }

        public static double GetLeft(this UIElement ele) => Canvas.GetLeft(ele);

        public static void SetLeft(this UIElement ele, double length) => Canvas.SetLeft(ele, length);

        public static double GetTop(this UIElement ele) => Canvas.GetTop(ele);

        public static void SetTop(this UIElement ele, double length) => Canvas.SetTop(ele, length);

        #endregion

        #region 结构树

        /// <summary>
        /// 获取节点前的水平连接短线
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Line GetConnectionLine(this Button node) => Node.GetConnectionLine(node);

        /// <summary>
        /// 设置节点前水平连接短线
        /// </summary>
        /// <param name="node"></param>
        /// <param name="line"></param>
        public static void SetConnectionLine(this Button node, Line line) => Node.SetConnectionLine(node, line);

        /// <summary>
        /// 获取节点正下方用于连接子Bom节点的竖线
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Line GetBomsVerticalLink(this Button node) => node.GetVisualTree()?.BomsVerticalLink;

        /// <summary>
        /// 设置节点正下方用于连接子Bom节点的竖线
        /// </summary>
        /// <param name="node"></param>
        /// <param name="line"></param>
        public static void SetBomsVerticalLink(this Button node, Line line)
        {
            var tree = node.GetVisualTree();
            if (tree == null)
                return;
            tree.BomsVerticalLink = line;
        }

        /// <summary>
        /// 获取父级Bom节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Button GetParentBom(this Button node) => Node.GetParentBom(node);

        /// <summary>
        /// 设置父级Bom节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        public static void SetParentBom(this Button node, Button parent) => Node.SetParentBom(node, parent);

        /// <summary>
        /// 获取父级Exp节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Button GetParentExp(this Button node) => Node.GetParentExp(node);

        /// <summary>
        /// 设置父级Exp节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        public static void SetParentExp(this Button node, Button parent) => Node.SetParentExp(node, parent);

        /// <summary>
        /// 获取节点的Exp和Bom结构树（节点和连接线）
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ExperienceBaseNodeVisualTree GetVisualTree(this Button node) => Node.GetTreeNodes(node);

        /// <summary>
        /// 设置节点的Exp和Bom结构树（节点和连接线）
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static void SetVisualTree(this Button node, ExperienceBaseNodeVisualTree tree) => Node.SetTreeNodes(node, tree);

        /// <summary>
        /// 清空节点缓存结构树的Bom结构子树
        /// </summary>
        /// <param name="node"></param>
        public static void ClearVisualBoms(this Button node)
        {
            var tree = node?.GetVisualTree();
            if (tree == null)
                return;

            tree.Boms.Clear();
            tree.BomsVerticalLink = null;
        }

        /// <summary>
        /// 清空节点缓存结构树的Exp结构子树
        /// </summary>
        /// <param name="node"></param>
        public static void ClearVisualExps(this Button node)
        {
            var tree = node?.GetVisualTree();
            if (tree == null)
                return;

            tree.Exps.Clear();
            tree.ExpLines = null;
        }

        /// <summary>
        /// 获取兄弟Exp节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Button> GetSiblingExps(this Button node)
        {
            List<Button> exps = new List<Button>();
            var parent = node.GetParentExp();
            if (parent == null)
                return exps;
            var tree = parent.GetVisualTree();
            if (tree == null)
                return exps;
            return tree.Exps;
        }

        /// <summary>
        /// 获取兄弟Bom节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Button> GetSiblingBoms(this Button node) => node.GetSiblingBomsIncludeSelf().Where(n => object.ReferenceEquals(n, node));

        /// <summary>
        /// 获取兄弟Bom节点（含自身）
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Button> GetSiblingBomsIncludeSelf(this Button node)
        {
            List<Button> boms = new List<Button>();
            var parent = node.GetParentBom();
            if (parent == null)
                return boms;
            var tree = parent.GetVisualTree();
            if (tree == null)
                return boms;
            return tree.Boms;
        }

        /// <summary>
        /// 获取位于显示在节点下方的兄弟Bom节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Button> GetBottomSiblingBoms(this Button node) => node.GetSiblingBomsIncludeSelf().Where(n => Canvas.GetTop(n) > Canvas.GetTop(node));

        /// <summary>
        /// 获取直接Boms节点集合
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Button> GetBoms(this Button node)
        {
            var tree = node.GetVisualTree();
            return tree == null ? new List<Button>() : tree.Boms;
        }

        /// <summary>
        /// 获取节点展开的子孙Bom节点数量
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int ExpandBomsCount(this Button node)
        {
            cnt = 0;
            Sum(node);
            return cnt;
        }

        static int cnt;
        static void Sum(Button node)
        {
            var boms = node.GetBoms();
            if (boms.Count() <= 0)
                return;

            cnt += boms.Count();

            var expBom = boms.FirstOrDefault(b => b.GetCollapseStatus() == CollapseStatusEnum.BomExpanded);
            if (expBom == null)
                return;

            Sum(expBom);
        }

        #endregion

        #region 淡入淡出
        /// <summary>
        /// 渐变动画
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="dp"></param>
        /// <param name="to"></param>
        public static void BeginAnimation(this UIElement ele, DependencyProperty dp, double? to)
        {
            DoubleAnimation animator = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(500)) };
            animator.To = to;
            ele?.BeginAnimation(dp, animator);
        }

        /// <summary>
        /// 淡入动画
        /// </summary>
        /// <param name="ele"></param>
        public static void FadeIn(this UIElement ele) => ele.BeginAnimation(UIElement.OpacityProperty, 1);

        /// <summary>
        /// 淡出动画
        /// </summary>
        /// <param name="ele"></param>
        public static void FadeOut(this UIElement ele) => ele.BeginAnimation(UIElement.OpacityProperty, 0);

        public static void FadeIn(this ExperienceBaseNodeVisualTree tree)
        {
            if (tree == null)
                return;

            //Bom结构树
            tree.Boms.ToList().ForEach(n =>
            {
                n.FadeIn();
                n.GetConnectionLine().FadeIn();
            });
            tree.BomsVerticalLink.FadeIn();
            tree.RootConnectionLine.FadeIn();

            //Exp结构树
            tree.Exps.ToList().ForEach(n => n.FadeIn());
            tree.ExpLines.FadeIn();
        }

        public static void FadeOut(this ExperienceBaseNodeVisualTree tree)
        {
            if (tree == null)
                return;

            //Bom结构树

            tree.Boms.ToList().ForEach(n =>
            {
                n.FadeOut();
                n.GetConnectionLine().FadeOut();
            });
            tree.BomsVerticalLink.FadeOut();
            tree.RootConnectionLine.FadeOut();

            //Exp结构树
            tree.Exps.ToList().ForEach(n => n.FadeOut());
            tree.ExpLines.FadeOut();
        }
        #endregion
    }
}
