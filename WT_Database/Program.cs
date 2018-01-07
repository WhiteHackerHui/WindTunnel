using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.IO;
using WT_Core;
using MongoDB.Bson;
using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using Numeric = System.Double;

namespace WT_Database
{
    class Program
    {
        static Stopwatch sw = new Stopwatch();
        static MongoClient client;
        static string projectFilePath;

        static void Main(string[] args)
        {
            string _dbHost = "localhost";
            int _dbPort = 27017;
            projectFilePath = DBFunctions.GetCurrentProjectFilePath();
            client = new MongoClient($"mongodb://{_dbHost}:{_dbPort}");

            //ImportTickCSVs();   //导入WindTunnel下的TickCSV文件

            //以Au为例，制作1分钟K线
            GenerateKLines();

        }

        /// <summary>
        /// 将所有从TB中下载的主力合约1分钟Bar数据导入到MongoDB中
        /// </summary>
        static void ImportBarCSVs()
        {
            DirectoryInfo csvFolder = new DirectoryInfo(projectFilePath + "TB_CSV");
            FileInfo[] csvFIs = csvFolder.GetFiles();
            foreach (var csvFI in csvFIs)
            {
                string s = csvFI.Name;
                var table = DBFunctions.GetOrCreateTable<Bar>(client, "CTPDB", "Bar_" + s.Substring(0, s.IndexOf('(')));
                var v = DBFunctions.GetBarsFromCSV(csvFI.FullName);
                FilterDefinitionBuilder<Bar> fdb = new FilterDefinitionBuilder<Bar>();
                table.DeleteMany(fdb.Empty);
                table.InsertMany(v);
                Console.WriteLine(table.CollectionNamespace.CollectionName);
            }


        }

        /// <summary>
        /// 导入所有TickCSV数据
        /// </summary>
        static void ImportTickCSVs()
        {
            DirectoryInfo ctpDBFolder = new DirectoryInfo(@"F:\CTP数据库");
            foreach (var csvFolder in ctpDBFolder.GetDirectories())
            {
                //DirectoryInfo csvFolder = new DirectoryInfo(@"F:\CTP数据库\20170103");
                string csvParentDirectoryName = csvFolder.Name;
                FileInfo[] csvFIs = csvFolder.GetFiles();
                foreach (var csvFI in csvFIs)
                {//遍历：其他、上午盘、下午盘、夜盘
                    string s = csvFI.Name;
                    var table = DBFunctions.GetOrCreateTable<Tick>(client, "CTPDB", "Tick");
                    var v = DBFunctions.GetTicksFromCSV(csvFI.FullName, csvParentDirectoryName);
                    FilterDefinitionBuilder<Tick> fdb = new FilterDefinitionBuilder<Tick>();
                    //table.DeleteMany(fdb.Empty);
                    if (v.Count>0)
                        table.InsertMany(v);
                    Console.WriteLine(csvParentDirectoryName + " " + s);
                }
            }
        }

        static void GenerateKLines()
        {
            string[] instrumentIDs = new string[] {
                //"IC1705","IF1705","IH1705","T1706","TF1706","CF709","FG709",                      
                //"JR711","MA709","OI709","RI709","RM709","RS707","SF802", "SM709",
                //"SR709","TA709","WH709","ZC709","a1709","b1709","bb1705","c1709",
                //"cs1709","fb1709","i1709","j1709","jd1709","jm1709","l1709","m1709",
                //"p1709","pp1709","v1709","y1709","ag1706","al1706","au1706","bu1709","cu1706","fu1709",
                //"hc1710","ni1709","pb1706","rb1710","ru1709","sn1709","wr1710","zn1706"
            };

            foreach (string instrumentID in instrumentIDs)
            {
                //var table3 = DBFunctions.GetOrCreateTable<Bar2>(client, "CTPDB", instrumentID + "_1min");
                //var barList = table3.Find(new FilterDefinitionBuilder<Bar2>().Empty, null).ToList();

                Console.WriteLine(instrumentID);
                BarSeries bars = new BarSeries();
                bars.Interval = 1;
                bars.IntervalType = IntervalType.Min;
                bars.InstrumentInfo = new InstrumentInfo { InstrumentID = instrumentID, PriceTick = 0, ProductID = "", VolumeMultiple = 0 };
                var table = DBFunctions.GetOrCreateTable<Tick2>(client, "CTPDB", "Tick");
                FilterDefinitionBuilder<Tick2> fdb = new FilterDefinitionBuilder<Tick2>();
                sw.Restart();
                var tickList = table.Find(Builders<Tick2>.Filter.Eq("InstrumentID", instrumentID)).ToList();
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds.ToString());
                tickList = tickList.OrderBy(t => DateTime.ParseExact(t.UpdateTime, "yyyyMMdd HH:mm:ss", null)).ToList() ;
                foreach (var tick in tickList)
                {
                    bars.OnRtnTick(tick);
                }
                var table2 = DBFunctions.GetOrCreateTable<Bar>(client, "CTPDB", instrumentID + "_1min");
                if (bars.Count > 0)
                {
                    table2.InsertMany(bars.ToArray());
                }
                
            }
        }
    }

    
    class Tick2:Tick
    {
        public ObjectId _id;
    }

    class Bar2
    {
        public ObjectId _id;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// 	开盘价
        /// </summary>
        [Description("开盘价"), Category("字段"), ReadOnly(true)]
        public Numeric Open { get; set; }

        /// <summary>
        /// 	最高价
        /// </summary>
        [Description("最高价"), Category("字段"), ReadOnly(true)]
        public Numeric High { get; set; }

        /// <summary>
        /// 	最低价
        /// </summary>
        [Description("最低价"), Category("字段"), ReadOnly(true)]
        public Numeric Low { get; set; }

        /// <summary>
        /// 	收盘价
        /// </summary>
        [Description("收盘价"), Category("字段"), ReadOnly(true)]
        public Numeric Close { get; set; }

        /// <summary>
        /// 	成交量
        /// </summary>
        [Description("成交量"), Category("字段"), ReadOnly(true)]
        public Numeric Volume { get; set; }

        /// <summary>
        /// 	持仓量
        /// </summary>
        [Description("持仓量"), Category("字段"), ReadOnly(true)]
        public Numeric OpenInt { get; set; }


    }

}
