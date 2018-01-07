using System.IO;
using WT_CTP.Properties;

namespace WT_CTP
{
    public static class CTPFactory
    {
        /// <summary>
        /// 释放接口的C++相关文件
        /// </summary>
        static private void Initialize()
        {
            Directory.CreateDirectory("ctp_dll");
            string[] files = { "ctp_dll\\thosttraderapi.dll", "ctp_dll\\ctp_trade.dll", "ctp_dll\\thostmduserapi.dll", "ctp_dll\\ctp_quote.dll" };
            object[] objs = { Resources.thosttraderapi, Resources.ctp_trade, Resources.thostmduserapi, Resources.ctp_quote };
            for (int i = 0; i < files.Length; ++i)
            {
                var bytes = (byte[])objs[i];
                if (!File.Exists(files[i]) || bytes.Length != new FileInfo(files[i]).Length)
                    File.WriteAllBytes(files[i], bytes);
            }
        }

        /// <summary>
        /// 生成CMdApi实例
        /// </summary>
        /// <param name="_investor"></param>
        /// <param name="_pwd"></param>
        /// <param name="_broker"></param>
        /// <param name="_addr"></param>
        /// <returns></returns>
        static public CMdApi CreateCMdApiInstance(string _investor = "", string _pwd = "", string _broker = "9999", string _addr = "tcp://180.168.146.187:10031")
        {
            Initialize();
            return new CMdApi(_investor, _pwd, _broker, _addr);
        }

        /// <summary>
        /// 生成CTradeApi实例
        /// </summary>
        /// <param name="_investor"></param>
        /// <param name="_pwd"></param>
        /// <param name="_broker"></param>
        /// <param name="_addr"></param>
        /// <returns></returns>
        static public CTradeApi CreateCTradeApiInstance(string _investor = "", string _pwd = "", string _broker = "9999", string _addr = "tcp://180.168.146.187:10030")
        {
            Initialize();
            return new CTradeApi(_investor, _pwd, _broker, _addr);
        }
    }
}
