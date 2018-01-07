using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT_Core;
using Numeric = System.Double;

namespace WT_Database
{
    /// <summary>
    /// 对MongoDB数据库操作的封装类
    /// </summary>
    public static class DBFunctions
    {
        /// <summary>
        /// 获得MongoDB中指定数据库的指定数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pClient"></param>
        /// <param name="pDatabaseName"></param>
        /// <param name="pTableName"></param>
        /// <returns></returns>
        public static IMongoCollection<T> GetOrCreateTable<T>(MongoClient pClient,string pDatabaseName, string pTableName)
        {
            IMongoDatabase db = pClient.GetDatabase(pDatabaseName); //获得指定的数据库实例
            return db.GetCollection<T>(pTableName);
        }

        /// <summary>
        /// 从CSV文件中获得Bar数据集合
        /// </summary>
        /// <param name="pFilePath"></param>
        /// <returns></returns>
        public static List<Bar> GetBarsFromCSV(string pFilePath)
        {
            Bar bar;
            string[] strs;
            List<Bar> listBars = new List<Bar>();
            FileStream fs = new FileStream(pFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string line = sr.ReadLine();
            while (line != null)
            {
                strs = line.Split(',');
                bar = new Bar {
                    UpdateDateTime = DateTime.Parse(strs[0]),
                    Open = Numeric.Parse(strs[1]),
                    High = Numeric.Parse(strs[2]),
                    Low = Numeric.Parse(strs[3]),
                    Close = Numeric.Parse(strs[4]),
                    Volume = Numeric.Parse(strs[5]),
                    OpenInt = Numeric.Parse(strs[6])
                };
                listBars.Add(bar);
                line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            return listBars;  
        }

        /// <summary>
        /// 获得当前工程下的文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentProjectFilePath()
        {
            var fileParts = Environment.CurrentDirectory.Split('\\');
            string projectFilePath = string.Empty;
            foreach (var part in fileParts)
            {
                if (part == "bin")
                {
                    break;
                }
                projectFilePath += part + @"\";
            }
            return projectFilePath;
        }

        /// <summary>
        /// 获得Tick数据集合
        /// </summary>
        /// <param name="pFilePath"></param>
        /// <param name="pDirectoryName"></param>
        /// <returns></returns>
        public static List<Tick> GetTicksFromCSV(string pFilePath, string pDirectoryName)
        {
            Tick tick;
            string[] strs;
            List<Tick> listTicks = new List<Tick>();
            FileStream fs = new FileStream(pFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string line = sr.ReadLine();
            while (line != null)
            {
                strs = line.Split(',');
                DateTime dt = DateTime.ParseExact(pDirectoryName + " " + int.Parse(strs[19]).ToString("D6"),"yyyyMMdd HHmmss",null);
                if (dt.Hour<3 && dt.Hour>=0)
                {
                    dt = dt.AddDays(1);
                }
                tick = new Tick
                {
                    InstrumentID = strs[0],
                    LastPrice = Numeric.Parse(strs[1]),
                    Volume = int.Parse(strs[2]),
                    BidPrice = Numeric.Parse(strs[6]),
                    BidVolume = int.Parse(strs[7]),
                    AskPrice = Numeric.Parse(strs[4]),
                    AskVolume = int.Parse(strs[5]),
                    UpdateTime = dt.ToString("yyyyMMdd HH:mm:ss"),
                    UpdateMillisec = 0,
                    OpenInt = Numeric.Parse(strs[11]),
                };
                //过滤无用数据
                if (tick.AskPrice * tick.BidPrice * tick.Volume * tick.AskVolume * tick.BidVolume >0)
                {
                    listTicks.Add(tick);     
                }
                line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            return listTicks;
        }

        //public static bool IsInTradingTime(string pBaseCode,DateTime pDateTime)
        //{
        //    DateTime dt_0900 = pDateTime.Date.AddHours(9).AddMinutes(30);
        //    DateTime dt_0930 = pDateTime.Date.AddHours(9);
        //    DateTime dt_1100 = pDateTime.Date.AddHours(11);
        //    DateTime dt_1130 = pDateTime.Date.AddHours(11).AddMinutes(30); 
        //    DateTime dt_1300 = pDateTime.Date.AddHours(13);
        //    DateTime dt_1500 = pDateTime.Date.AddHours(15);
        //    DateTime dt_2100 = pDateTime.Date.AddHours(21);
        //    DateTime dt_2330 = pDateTime.Date.AddHours(23).AddMinutes(30);

        //    string[] baseCodes1 = new string[] { "IC", "IF", "IH" };
        //    if (baseCodes1.Contains(pBaseCode.ToUpper()))
        //    {//IC、IF、IH

        //    }



        //}

    }
}
