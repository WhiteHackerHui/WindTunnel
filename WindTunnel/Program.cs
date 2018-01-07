using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindTunnel.Properties;

namespace WindTunnel
{
    static class Program
    {
        private static string _errLog = string.Empty;

        [STAThread]
        public static void Main()
        {
            //释放接口的C++相关文件
            Directory.CreateDirectory("ctp_dll");
            string[] files = { "ctp_dll\\thosttraderapi.dll", "ctp_dll\\ctp_trade.dll", "ctp_dll\\thostmduserapi.dll", "ctp_dll\\ctp_quote.dll" };
            object[] objs = { Resources.thosttraderapi, Resources.ctp_trade, Resources.thostmduserapi, Resources.ctp_quote };
            for (int i = 0; i < files.Length; ++i)
            {
                var bytes = (byte[])objs[i];
                if (!File.Exists(files[i]) || bytes.Length != new FileInfo(files[i]).Length)
                    File.WriteAllBytes(files[i], bytes);
            }

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

       }
}
