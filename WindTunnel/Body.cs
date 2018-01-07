using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using HaiFeng;

namespace WindTunnel
{
    
    public static class Body
    {
        //系统变量
        public static int nOrders = 1; //维护下单OrderRef
        public static CMdApi mktApi;
        public static CTradeApi trdApi;
        public static MainWindow mainWindow;
        public static List<FutureBroker> ListFutureBroker;
        public static List<Investor> ListInvestor;
        public static ConcurrentDictionary<string,List<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>>> Dic_InstrumentID_PositionKeys
            = new ConcurrentDictionary<string,List<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>>>();

        //可观察变量
        public static ObservableCollection<DepthMarketData> OcAllDepthMarketData;
        public static ObservableCollection<DepthMarketData> OcMainDepthMarketData;
        public static ObservableCollection<TradeField> OcAllTradeField = new ObservableCollection<TradeField>();
        public static ObservableCollection<DetailPositionField> OcAllDetailPositionField = new ObservableCollection<DetailPositionField>();
        public static ObservableCollection<Position> OcAllPosition = new ObservableCollection<Position>();
        public static ObservableCollection<OrderField> OcAllOrderField = new ObservableCollection<OrderField>();
        public static ObservableCollection<OrderField> OcNoTradeOrderField = new ObservableCollection<OrderField>();
        public static ObservableCollection<OrderField> OcCanceledOrderField = new ObservableCollection<OrderField>();
        public static ObservableCollection<OrderField> OcErrorOrderField = new ObservableCollection<OrderField>();
        public static ObservableCollection<ConditionOrderField> OcAllConditionOrderField = new ObservableCollection<ConditionOrderField>();
        public static ObservableCollection<TradingAccountField> OcAllTradingAccountField = new ObservableCollection<TradingAccountField>();

        //策略变量
        public static List<StrategyBase> ListStrategies = new List<StrategyBase>();

        //Body初始化参数
        public async static void Initialize()
        {
            //可观察变量初始化
            OcAllDepthMarketData = await GenerateOCAllMarketDepthData(Body.mktApi,Body.trdApi);

            //策略初始化
            //ListStrategies.Add(new CTAStrategy("动态9品种期货策略", mktApi, trdApi));  
        }



        //所有合约的市场行情数据
        public async static Task<ObservableCollection<DepthMarketData>> GenerateOCAllMarketDepthData(CMdApi MktApi, CTradeApi TrdApi)
        {
            var oc = new ObservableCollection<DepthMarketData>();
            while (MktApi.DicDepthMarketData.Count < TrdApi.DicInstrumentField.Count)
            {
                await Task.Delay(100);
            }
            lock (oc)
            { 
                foreach (var kvp in Body.mktApi.DicDepthMarketData)
                {
                    oc.Add(kvp.Value);
                }
                return oc;
            }
        }

        //主力合约的市场行情数据
        public static ObservableCollection<DepthMarketData> GenerateMainDepthData(ObservableCollection<DepthMarketData> OCAllMarketDepthData)
        {
            lock (OCAllMarketDepthData)
            {
                var oc = new ObservableCollection<DepthMarketData>();
                var grp = OCAllMarketDepthData.GroupBy(r => r.FutureCode);
                foreach (IGrouping<string, DepthMarketData> item in grp)
                {
                    DepthMarketData r = item.OrderByDescending(x => x.OpenInterest).FirstOrDefault();
                    oc.Add(r);
                }
                return oc;
            }
        }


        #region 2次回报

        //更新：Position、DetialPosition
        public static void Body_OnUpdate(object obj)
        {
            lock (obj)
            {
                if (obj.GetType() == typeof(ConcurrentDictionary<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>, Position>))
                {//更新持仓
                    Body.OcAllPosition.Clear();
                    var dic = obj as ConcurrentDictionary<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>, Position>;
                    foreach (var kvp in dic)
                    {
                        OcAllPosition.Add(kvp.Value);
                    }
                    foreach (var key in Body.trdApi.DicPosition.Keys)
                    {//更新Dic_InstrumentID_PositionKeys
                        var instrumentID = key.Item1;
                        var direction = key.Item2;
                        var hedge = key.Item3;
                        var value = Body.Dic_InstrumentID_PositionKeys.GetOrAdd(key.Item1,
                            k => new List<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>>());
                        value.Add(key);
                    }
                }
                else if (obj.GetType() == typeof(ConcurrentDictionary<Tuple<string, string>, DetailPositionField>))
                {//更新持仓细节
                    Body.OcAllDetailPositionField.Clear();
                    var dic = obj as ConcurrentDictionary<Tuple<string, string>, DetailPositionField>;
                    foreach (var kvp in dic)
                    {
                        OcAllDetailPositionField.Add(kvp.Value);
                    }
                }

            }

        }
        //行情回报
        public static void Body_OnTick(DepthMarketData dmd)
        {
            lock (Dic_InstrumentID_PositionKeys)
            {
                if (!Body.Dic_InstrumentID_PositionKeys.IsEmpty && Body.Dic_InstrumentID_PositionKeys.ContainsKey(dmd.InstrumentID))
                {
                    List<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>> value;
                    Body.Dic_InstrumentID_PositionKeys.TryGetValue(dmd.InstrumentID, out value);
                    foreach (var item in value)
                    {
                        var key = item;
                        var instrumentID = key.Item1;
                        Position pos;
                        if (!Body.trdApi.DicPosition.TryGetValue(key, out pos))
                        {
                            continue;
                        }
                        var lastPrice = Body.mktApi.DicDepthMarketData[key.Item1].LastPrice;
                        var mult = Body.trdApi.DicInstrumentField[dmd.InstrumentID].VolumeMultiple;
                        pos.LastPrice = lastPrice;
                        if (key.Item2 == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long)
                        {
                            pos.OpenProfit = (lastPrice - pos.AvgOpenPrice) * pos.TotalPosition * mult;
                            pos.PositionProfit = lastPrice * pos.TotalPosition * mult - pos.PositionCost;
                        }
                        else
                        {
                            pos.OpenProfit = -1 * (lastPrice - pos.AvgOpenPrice) * pos.TotalPosition * mult;
                            pos.PositionProfit = -1 * (lastPrice * pos.TotalPosition * mult - pos.PositionCost);
                        }
                        pos.Notify("");
                    }
                }
            };
            
        }
        //交易回报
        public static void Body_OnRtnTrade(TradeField tf)
        {
            lock (OcAllTradeField)
            {
                int index = OcAllTradeField.IndexOf(tf);
                if (index == -1)
                {
                    OcAllTradeField.Insert(0,tf);
                }
                else
                {
                    OcAllTradeField[index].CThostFtdcTradeFieldInstance = tf.CThostFtdcTradeFieldInstance;
                }
            }
            
        }
        //报单回报
        public static void Body_OnOrderField(OrderField of)
        {
            //所有报单
            lock (OcAllOrderField)
            {
                int index = OcAllOrderField.IndexOf(of);
                if (index == -1)
                {
                    OcAllOrderField.Insert(0,of);
                }
                else
                {
                    OcAllOrderField[index].OrderFieldInstance = of.OrderFieldInstance;
                }
            }
            //未成交报单
            lock (OcNoTradeOrderField)
            {
                int index = OcNoTradeOrderField.IndexOf(of);
                if (index == -1 && (of.OrderStatus== TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeNotQueueing || of.OrderStatus ==  TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeQueueing))
                {
                    OcNoTradeOrderField.Insert(0,of);
                }
                else if (index >= 0 && (of.OrderStatus != TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeNotQueueing && of.OrderStatus != TThostFtdcOrderStatusType.THOST_FTDC_OST_NoTradeQueueing))
                {
                    OcNoTradeOrderField.RemoveAt(index);
                }
            }
            //撤销的报单
            lock (OcCanceledOrderField)
            {
                int index = OcCanceledOrderField.IndexOf(of);
                if (index == -1 && (of.OrderStatus == TThostFtdcOrderStatusType.THOST_FTDC_OST_Canceled || of.OrderStatus == TThostFtdcOrderStatusType.THOST_FTDC_OST_Unknown))
                {
                    OcCanceledOrderField.Insert(0,of);
                }
                else if (index >= 0 && (of.OrderStatus != TThostFtdcOrderStatusType.THOST_FTDC_OST_Canceled && of.OrderStatus != TThostFtdcOrderStatusType.THOST_FTDC_OST_Unknown))
                {
                    OcCanceledOrderField.RemoveAt(index);
                }
            }

        }
        //错单回报
        public static void Body_OnErrorOrder(OrderField of)
        {
            lock (OcErrorOrderField)
            {
                OcErrorOrderField.Insert(0,of);
            }
        }
        //交易账户回报
        public static void Body_OnTradingAccount(TradingAccountField taf)
        {
            lock (OcAllTradingAccountField)
            {
                int index = OcAllTradingAccountField.IndexOf(taf);
                if (index == -1)
                {
                    OcAllTradingAccountField.Insert(0, taf);
                }
                else
                {
                    OcAllTradingAccountField[index].CThostFtdcTradingAccountFieldInstance = taf.CThostFtdcTradingAccountFieldInstance;
                }
            }

        }

        //合并持仓回报
        public static void Body_OnPosition(Position pos)
        {
            lock (OcAllPosition)
            {
                int index = OcAllPosition.IndexOf(pos);
                if (index == -1)
                {
                    OcAllPosition.Insert(0,pos);
                }
                else
                {
                    OcAllPosition[index] = pos;
                }
                foreach (var key in Body.trdApi.DicPosition.Keys)
                {//更新Dic_InstrumentID_PositionKeys
                    var instrumentID = key.Item1;
                    var direction = key.Item2;
                    var hedge = key.Item3;
                    var value = Body.Dic_InstrumentID_PositionKeys.GetOrAdd(key.Item1,
                        k => new List<Tuple<string, TThostFtdcPosiDirectionType, TThostFtdcHedgeFlagType>>());
                    value.Add(key);
                }
            }
        }
        //细节持仓回报
        public static void Body_OnDetailPosition(DetailPositionField dpf)
        {
            lock (OcAllDetailPositionField)
            {
                int index = OcAllDetailPositionField.IndexOf(dpf);
                if (index == -1)
                {
                    OcAllDetailPositionField.Insert(0, dpf);
                }
                else
                {
                    OcAllDetailPositionField[index].InvestorPositionDetailFieldInstance = dpf.InvestorPositionDetailFieldInstance;
                }
            }
        }

        #endregion

    }
}
