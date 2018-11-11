namespace System.Windows.Controls
{
    public static class UserControlExp
    {
        public static Panel GetPanel(this Control c)=>(Application.Current.Properties["Panel"]) as Panel;

        public static void Navigate2(this Control c,UserControl uc)
        {
            var panel = (Application.Current.Properties["Panel"]) as Panel;
            if (panel == null)
                return;

            panel.Children.Clear();
            panel.Children.Add(uc);
        }
    }
}
