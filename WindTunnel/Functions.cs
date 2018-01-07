using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;


namespace WindTunnel
{
    public static class Functions
    {
        
        //输出ObesrvableCollection<Trade>至CSV文件中
        public static void ToCSV<T>(this Collection<T> CollectionSeries, string Path, bool HeadNeeded = true)//成交单
        {
            CreatePathIfNeeded(Path);
            FileStream fs = new FileStream(Path, FileMode.Create, FileAccess.Write);    //写入
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            StringBuilder data = new StringBuilder();
            Console.WriteLine("{0} 数据正在写入中。。。", Path);
            int rows = CollectionSeries.Count;   //行名
            string[] propertiesToPrint = null;

            //如果没有记录就不打印
            if (rows == 0)
            {
                Console.WriteLine("无记录！");
                return;
            }

            //选出要打印的属性
            propertiesToPrint = PropertiesToPrint<T>();

            //打印到CSV文件
            if (HeadNeeded)
            {//打印属性标题
                data.AppendLine(string.Join(",",propertiesToPrint));
            }

            for (int i = 0; i < rows; i++)
            {//打印属性值
                List<string> tmpValue = new List<string>();
                foreach (string propName in propertiesToPrint)
                {
                    tmpValue.Add(CollectionSeries[i].GetType().GetProperty(propName).GetValue(CollectionSeries[i]).ToString());
                }
                data.AppendLine(string.Join(",", tmpValue.ToArray()));
            }

            sw.WriteLine(data.ToString());
            sw.Close();
            fs.Close();
            Console.WriteLine("{0} 数据已经成功写入！", Path);
        }

        //从CSV读取所有数据，无标题
        public static Collection<string[]> FromCSV(string Path,char Seperator=',')
        {
            Collection<string[]> result = new Collection<string[]>();
            FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string line = sr.ReadLine();
            while (line != null)
            {
                result.Add(line.Split(Seperator));
                line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            return result;
        }

        public static Collection<T> FromCSV<T>(string Path, char Seperator = ',')
        {
            Collection<T> result = new Collection<T>();
            FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string line = sr.ReadLine();
            while (line != null)
            {
                foreach (string item in line.Split(Seperator))
                {
                    result.Add((T)Convert.ChangeType(item, typeof(T)));
                }
                line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            return result;
        }

        public static T[,] FromCSV2<T>(string Path, char Seperator = ',')
        {
            List<T[]> temp = new List<T[]>();
            T[,] result;
            FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string[] rowStrValue;
            T[] rowTValue;
            int nCols = 0;

            string line = sr.ReadLine();
            while (line != null)
            {
                rowStrValue = line.Split(Seperator);
                nCols = rowStrValue.Length;
                rowTValue = new T[rowStrValue.Length];
                for (int j = 0; j < rowTValue.Length; j++)
                {
                    rowTValue[j] = (T)Convert.ChangeType(rowStrValue[j], typeof(T));
                }
                temp.Add(rowTValue);
                line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            result = new T[temp.Count, nCols];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = temp[i][j];
                }
            }
            return result;
        }

        public static void ToCSV<T>(this T[,] TwoDimArray, string Path, char Seperator = ',')
        {
            CreatePathIfNeeded(Path);
            FileStream fs = new FileStream(Path, FileMode.Create, FileAccess.Write);    //写入
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            StringBuilder data = new StringBuilder();
            Console.WriteLine("{0} 数据正在写入中。。。", Path);
            int nRows = TwoDimArray.GetLength(0);   //行数
            int nCols = TwoDimArray.GetLength(1);   

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    data.Append(TwoDimArray[i, j].ToString() + ",");
                }
                data.AppendLine();
            }

            sw.WriteLine(data.ToString());
            sw.Close();
            fs.Close();
            Console.WriteLine("{0} 数据已经成功写入！", Path);
        }

        public static void ToCSV<T>(this T[] OneDimArray, string Path, char Seperator = ',')
        {
            CreatePathIfNeeded(Path);
            FileStream fs = new FileStream(Path, FileMode.Create, FileAccess.Write);    //写入
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            StringBuilder data = new StringBuilder();
            Console.WriteLine("{0} 数据正在写入中。。。", Path);
            int nCols = OneDimArray.GetLength(0);

            for (int j = 0; j < nCols; j++)
            {
                data.Append(OneDimArray[j].ToString() + ",");
            }
            data.AppendLine();

            sw.WriteLine(data.ToString());
            sw.Close();
            fs.Close();
            Console.WriteLine("{0} 数据已经成功写入！", Path);
        }

        //获得二维数据的最后一行数据
        public static T[] LastRow<T>(this T[,] TwoDimArray)
        {
            int nRows = TwoDimArray.GetLength(0);
            int nCols = TwoDimArray.GetLength(1);
            T[] lastRow = new T[nCols];
            for (int j = 0; j < nCols; j++)
            {
                lastRow[j] = TwoDimArray[nRows - 1, j];
            }
            return lastRow;
        }

        //根据泛型T，选出要打印的属性
        public static string[] PropertiesToPrint<T>()
        {
            string[] propertiesToPrint = new string[] { };
            //根据T选取指定的属性
            if (typeof(T) == typeof(TradingAccountField))
            {
                propertiesToPrint = new string[] { "TradeID", "InstrumentID", "Side", "Price", "Qty", "Time", "TradeDate", "OpenClose" };
            }
            else if (typeof(T) == typeof(OrderField))
            {
                propertiesToPrint = new string[] { "OrderID", "InstrumentID", "Side", "Price", "Qty", "Status", "OpenClose", "LeavesQty" };
            }
            else if (typeof(T) == typeof(Position))
            {
                propertiesToPrint = new string[] { "InstrumentID", "Side", "Positions", "TodayPosition", "HistoryPosition", "AccountID" };
            }
            //else if (typeof(T) == typeof(CTACalResult))
            //{
            //    propertiesToPrint = new string[] { "AccountID", "InstrumentID", "ExpectedContracts", "FiltedSignal", "W", "RiskFactor", "UpdateTime", "Price","PreClose","EWMAStd","Signal" };
            //}
            else if (typeof(T) == typeof(DepthMarketData))
            {
                propertiesToPrint = new string[] {"InstrumentID","LastPrice","Volume","Turnover","AskPrice1","AskVol1",
                    "BidPrice1","BidVol1","AveragePrice","HighestPrice","LowestPrice","OpenInterest","PreOpenInterest","PriceChange","UpperLimitPrice",
                    "LowerLimitPrice","PreSettlementPrice","PreClosePrice", "OpenPrice","UpdateTime" };
            }
            //else if (typeof(T) == typeof(TdxInstrumentInfo))
            //{
            //    propertiesToPrint = new string[] { "Market", "Code", "Name", "Info"};
            //}
            return propertiesToPrint;
        }

        //根据T来返回Type[]
        public static Type[] TypesToPrint<T>()
        {
            List<Type> typesToPrint = new List<Type>();
            string[] columnNames = PropertiesToPrint<T>();
            for (int col = 0; col < columnNames.Length; col++)
            {
                typesToPrint.Add(typeof(T).GetProperty(columnNames[col]).PropertyType);
            }
            return typesToPrint.ToArray();
        }

        //Collection转换为DataTable
        public static DataTable ToDataTable<T>(this IEnumerable<T> CollectionSeries)
        {
            lock (CollectionSeries)
            { 
                if (CollectionSeries.Count()<=0)
                {
                    return null;
                }

                string[] columnNames = PropertiesToPrint<T>();
                DataTable resultDT = new DataTable();
                foreach (string colName in columnNames)
                {//添加列名和列的类型
                    resultDT.Columns.Add(colName, typeof(T).GetProperty(colName).PropertyType);
                }
                foreach (T itemData in CollectionSeries)
                {
                    DataRow dr = resultDT.NewRow();
                    for (int col = 0; col < columnNames.Length; col++)
                    {
                        dr[col] = typeof(T).GetProperty(columnNames[col]).GetValue(itemData);
                    }
                    resultDT.Rows.Add(dr);
                }
                return resultDT;
            }
        }

        //从CSV中读取T类型的DataTable，带有Header
        public static DataTable FromCSV<T>(out DataTable resultDT, string Path, char Seperator = ',')
        {
            resultDT = new DataTable();
            FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            //第一行为Header 
            string[] columnNames = PropertiesToPrint<T>();
            foreach (string colName in columnNames)
            {//添加列名和列的类型
                resultDT.Columns.Add(colName, typeof(T).GetProperty(colName).PropertyType);
            }
            string line = sr.ReadLine();
            while (line != null ) 
            {
                if (line.Contains("E+308"))
                {//剔除掉double最大值超范围的情况
                    line = sr.ReadLine();
                    continue;
                }
                DataRow dr = resultDT.NewRow();
                string[] strRowData = line.Split(Seperator);
                for (int col = 0; col < strRowData.Length; col++)
                {
                    dr[col] = Convert.ChangeType(strRowData[col], typeof(T).GetProperty(columnNames[col]).PropertyType);
                }
                resultDT.Rows.Add(dr);
                line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            return resultDT;
        }

        //如果路径不存在，则创建
        public static void CreatePathIfNeeded(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            string folderPath = directoryInfo.Parent.FullName;
            if (!Directory.Exists(folderPath))
            {//如果路径不存在，则创建
                new DirectoryInfo(folderPath).Create();
            }
        }

        //检查给定DateTime是否在指定的时间范围内,但是必须是同一天内的！（前闭后开）
        public static bool IsInTimeRange(this DateTime time, int startTimeHour, int startTimeMinute,int endTimeHour, int endTimeMinute)
        {
            DateTime startTime = new DateTime(time.Year, time.Month, time.Day, startTimeHour, startTimeMinute, 0);
            DateTime endTime = new DateTime(time.Year, time.Month, time.Day, endTimeHour, endTimeMinute, 0);
            if (time.Subtract(startTime).Milliseconds >= 0 && time.Subtract(endTime).Milliseconds < 0 )
            {
                return true;
            }
            return false;
        }

        ////开始写入行情数据
        //public static void writeBegin(List<DepthMarketData> StoreDataList,int beginRow, int endRow, string filePath)
        //{
        //    CreatePathIfNeeded(filePath);
        //    DepthMarketData dmd;
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = beginRow; i <= endRow; i++)
        //    {
        //        dmd = StoreDataList[i];
        //        if (dmd!=null)
        //        {
        //            sb.AppendLine(
        //            string.Join(",", new string[] { dmd.InstrumentID, dmd.LastPrice.ToString(),
        //            dmd.Volume.ToString(), dmd.Turnover.ToString(),
        //            dmd.AskPrice1.ToString(), dmd.AskVol1.ToString(),
        //            dmd.BidPrice1.ToString(), dmd.BidVol1.ToString(),
        //            dmd.AveragePrice.ToString(),dmd.HighestPrice.ToString(),
        //            dmd.LowestPrice.ToString(),dmd.OpenInterest.ToString(),
        //            dmd.PreOpenInterest.ToString(),dmd.PriceChange.ToString(),
        //            dmd.UpperLimitPrice.ToString(),dmd.LowerLimitPrice.ToString(),
        //            dmd.PreSettlementPrice.ToString(),dmd.PreClosePrice.ToString(),
        //            dmd.OpenPrice.ToString(),dmd.UpdateTime.ToString() }));
        //        }
        //    }
        //    byte[] bytes = Encoding.Default.GetBytes(sb.ToString());
        //    FileStream writer = new FileStream(filePath, FileMode.Append, FileAccess.Write);
        //    Console.WriteLine("异步写入开始..");
        //    writer.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(writeDone), writer);
        //}
        ////写入完毕后调用
        //public static void writeDone(IAsyncResult asr)
        //{
        //    using (Stream str = (Stream)asr.AsyncState)
        //    {
        //        str.EndWrite(asr);
        //        Console.WriteLine("异步写入结束");
        //    }
        //}


    }
}
