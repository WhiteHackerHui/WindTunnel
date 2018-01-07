using System;

namespace WT_Platform
{
    static class Program
    {
        [STAThread]
        static public void Main()
        {
            //调用WPF窗口
            App app = new App();
            app.InitializeComponent();
            app.Run();  
        }
    }
}
