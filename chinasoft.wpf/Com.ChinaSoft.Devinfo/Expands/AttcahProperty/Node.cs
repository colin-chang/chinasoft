using System.Windows;
using Com.ChinaSoft.Devinfo.Model;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Com.ChinaSoft.Devinfo.Expands.AttcahProperty
{
    class Node : DependencyObject
    {
        public static ExperienceBaseNodeVisualTree GetTreeNodes(DependencyObject obj) => (ExperienceBaseNodeVisualTree)obj.GetValue(TreeNodesProperty);

        public static void SetTreeNodes(DependencyObject obj, ExperienceBaseNodeVisualTree value) => obj.SetValue(TreeNodesProperty, value);

        // Using a DependencyProperty as the backing store for TreeNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeNodesProperty =
            DependencyProperty.RegisterAttached("TreeNodes", typeof(ExperienceBaseNodeVisualTree), typeof(ExperienceBaseNodeVisualTree), new PropertyMetadata());



        public static Line GetConnectionLine(DependencyObject obj) => (Line)obj.GetValue(ConnectionLineProperty);

        public static void SetConnectionLine(DependencyObject obj, Line value) => obj.SetValue(ConnectionLineProperty, value);

        // Using a DependencyProperty as the backing store for ConnectionLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionLineProperty =
            DependencyProperty.RegisterAttached("ConnectionLine", typeof(Line), typeof(Line), new PropertyMetadata(null));


        //父级Bom
        public static Button GetParentBom(DependencyObject obj) => (Button)obj.GetValue(ParentBomProperty);

        public static void SetParentBom(DependencyObject obj, Button value)=>obj.SetValue(ParentBomProperty, value);
        

        // Using a DependencyProperty as the backing store for ParentBom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentBomProperty =
            DependencyProperty.RegisterAttached("ParentBom", typeof(Button), typeof(Button), new PropertyMetadata(null));



        public static Button GetParentExp(DependencyObject obj)=>(Button)obj.GetValue(ParentExpProperty);

        public static void SetParentExp(DependencyObject obj, Button value)=>obj.SetValue(ParentExpProperty, value);

        // Using a DependencyProperty as the backing store for ParentExp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentExpProperty =
            DependencyProperty.RegisterAttached("ParentExp", typeof(Button), typeof(Button), new PropertyMetadata(null));

    }
}
