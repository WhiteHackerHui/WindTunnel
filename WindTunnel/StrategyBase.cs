using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindTunnel
{
    //策略父类
    public abstract class StrategyBase
    {
        //变量
        protected CMdApi _mktApi;
        protected CTradeApi _trdApi;
        protected string _strategyName = string.Empty;
        //策略中目标合约
        protected ConcurrentDictionary<string,InstrumentField> 
            _dicStrategyInstruments = new ConcurrentDictionary<string, InstrumentField>();
        //"合约ID_买（卖）" ~ 手数
        protected ConcurrentDictionary<Tuple<string,TThostFtdcDirectionType, TThostFtdcOffsetFlagType>, int>
            _dicStrategyExpectedResult = new ConcurrentDictionary<Tuple<string, TThostFtdcDirectionType, TThostFtdcOffsetFlagType>, int>();
        //应下单
        protected ObservableCollection<OrderField> _ocStrategyOrders;
        //计算结果
        protected ObservableCollection<object> _ocStrategyCalculationResults;
        //行情字典
        protected ConcurrentDictionary<string, DepthMarketData> _dicStrategyDmds = new ConcurrentDictionary<string, DepthMarketData>();

        //属性
        public CMdApi MktApi { get { return this._mktApi; } set { this._mktApi = value; } }
        public CTradeApi TrdApi { get { return this._trdApi; } set { this._trdApi = value; } }
        public string Name { get { return this._strategyName; } set { this._strategyName = value; } }
        public ConcurrentDictionary<string, InstrumentField> DicStrategyInstruments { get { return this._dicStrategyInstruments; } }
        public ConcurrentDictionary<Tuple<string, TThostFtdcDirectionType, TThostFtdcOffsetFlagType>, int> DicStrategyExpectedResult{ get { return this._dicStrategyExpectedResult; } }
        public ConcurrentDictionary<string, DepthMarketData> DicStrategyDmds { get { return this._dicStrategyDmds; } }
        public ObservableCollection<OrderField> OcStrategyOrders { get { return this._ocStrategyOrders; } }
        public ObservableCollection<object> OcStrategyCalculationResults { get { return this._ocStrategyCalculationResults; } }

        //回调函数：更新行情
        abstract public void OnTick(DepthMarketData dmd);

        //构造器
        public StrategyBase(string StrategyName, CMdApi mktApi, CTradeApi trdApi)
        {
            this._strategyName = StrategyName;
            this._mktApi = mktApi;
            this._trdApi = trdApi;
            //this._mktApi.OnTick += this.OnTick;
        }

        //计算信号
        abstract public void Calculate();

        #region 报单构建函数

        //构造平仓单：对手价限价单（OrderOffsetFlag 平今平昨）
        public OrderField ClosePositionOrder(string InstrumentID, int VolumeToClose,
            TThostFtdcPosiDirectionType OrderDirection, TThostFtdcOffsetFlagType OrderOffsetFlag, 
            TThostFtdcOrderPriceTypeType OrderPriceType)
        {
            DepthMarketData dmd;
            this._dicStrategyDmds.TryGetValue(InstrumentID, out dmd);
            OrderField of = new OrderField();
            of.InstrumentID = InstrumentID;
            of.OrderPriceType = OrderPriceType;
            of.CombOffsetFlag = new String((char)OrderOffsetFlag, 1); 
            if (OrderDirection == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long)
            {
                of.LimitPrice = dmd.BidPrice1;
                of.Direction = TThostFtdcDirectionType.THOST_FTDC_D_Sell;
            }
            else
            {
                of.LimitPrice = dmd.AskPrice1;
                of.Direction = TThostFtdcDirectionType.THOST_FTDC_D_Buy;
            }
            of.VolumeTotalOriginal = VolumeToClose;//确保是正数
            return of;
        }

        //构造开仓单：限价单
        public OrderField OpenPositionOrder(string InstrumentID, double OpenPrice, int VolumeToOpen,
            TThostFtdcDirectionType OrderDirection, TThostFtdcOrderPriceTypeType OrderPriceType)
        {
            OrderField orderField = new OrderField();
            orderField.InstrumentID = InstrumentID;
            orderField.OrderPriceType = OrderPriceType;
            orderField.CombOffsetFlag = new String((char)TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open, 1); ;
            orderField.Direction = OrderDirection;
            orderField.LimitPrice = OpenPrice;
            orderField.VolumeTotalOriginal = VolumeToOpen;
            return orderField;
        }

        //根据持仓和预期手数生成Orders【核心】
        public OrderField[] CreateOrders(Position[] currentPositions, Position[] expectedPositions)
        {
            List<OrderField> resultOrders = new List<OrderField>();

            //1、当前组合中持有的，但是期望中不持有：全部平掉！
            Position[] curPositionsToClose = currentPositions.Except(expectedPositions, new Position()).ToArray();
            for (int i = 0; i < curPositionsToClose.Length; i++)
            {
                Position pos = curPositionsToClose[i];
                //平昨
                if (pos.YdPosition > 0)
                {
                    resultOrders.Add(ClosePositionOrder(pos.InstrumentID, pos.YdPosition, 
                        pos.PosiDirection, TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close,
                        TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                }
                //平今
                if (pos.TdPosition > 0)
                {
                    resultOrders.Add(ClosePositionOrder(pos.InstrumentID, pos.TdPosition, 
                        pos.PosiDirection, TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday,
                        TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                }
            }
            //2、当前组合中不持有，但是期望中要持有：全部开仓！
            Position[] expPositionToOpen = expectedPositions.Except(currentPositions, new Position()).ToArray();
            for (int i = 0; i < expPositionToOpen.Length; i++)
            {
                Position pos = expPositionToOpen[i];
                TThostFtdcDirectionType orderDirection = default(TThostFtdcDirectionType);
                if (pos.PosiDirection == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long)
                    orderDirection = TThostFtdcDirectionType.THOST_FTDC_D_Buy;
                else if (pos.PosiDirection == TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short)
                    orderDirection = TThostFtdcDirectionType.THOST_FTDC_D_Sell;
                resultOrders.Add(OpenPositionOrder(pos.InstrumentID, pos.AvgOpenPrice,
                    pos.TdPosition, orderDirection, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
            }
            //3、当前组合与期望组合重合部分（instrumentID和positionSide相同）
            Position[] ol_curPosition = currentPositions.Intersect(expectedPositions, new Position()).OrderBy(p => p.InstrumentID).ThenBy(p => p.PosiDirection).ToArray();
            Position[] ol_expPosition = expectedPositions.Intersect(currentPositions, new Position()).OrderBy(p => p.InstrumentID).ThenBy(p => p.PosiDirection).ToArray();
            if (ol_curPosition.Length != ol_expPosition.Length)
            {
                throw new Exception("长度不匹配！");
            }

            for (int i = 0; i < ol_curPosition.Length; i++)
            {
                Position curPos = ol_curPosition[i];
                Position expPos = ol_expPosition[i];
                string instID = curPos.InstrumentID;
                int x = curPos.TotalPosition;
                int y = expPos.TotalPosition;
                if (x < 0)
                {//当前组合中该合约为“空头”
                    #region 
                    if (x > y)
                    {//继续开空x-y手
                        resultOrders.Add(OpenPositionOrder(instID, expPos.AvgOpenPrice, x - y, TThostFtdcDirectionType.THOST_FTDC_D_Sell, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                    }
                    else if (x < y)
                    {
                        if (y - x <= -x)//只要平掉y-x手空头
                        {
                            if (curPos.YdPosition > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, (int)Math.Min(curPos.YdPosition, y - x),
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short, TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                            if (curPos.TdPosition > 0 && (y - x) - (int)Math.Min(curPos.YdPosition, y - x) > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, (y - x) - (int)Math.Min(curPos.YdPosition, y - x),
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short, TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                        }
                        else//(y-x>-x)先平掉-x手空头（即所有空头），再开y手多头
                        {
                            //先平掉所有空头
                            if (curPos.YdPosition > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, curPos.YdPosition,
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short, TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                            if (curPos.TdPosition > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, curPos.TdPosition,
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Short, TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                            //再开y手多头
                            resultOrders.Add(OpenPositionOrder(instID, expPos.AvgOpenPrice, y, TThostFtdcDirectionType.THOST_FTDC_D_Buy, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                        }
                    }
                    #endregion
                }
                else if (x > 0)
                {//当前组合中该合约为“多头”
                    #region 
                    if (x < y)
                    {//继续开多y-x手
                        resultOrders.Add(OpenPositionOrder(instID, expPos.AvgOpenPrice, y - x, TThostFtdcDirectionType.THOST_FTDC_D_Buy, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                    }
                    else if (x > y)
                    {
                        if (x - y <= x)//只要平掉x-y手空头
                        {
                            if (curPos.YdPosition > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, (int)Math.Min(curPos.YdPosition, x - y),
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long, TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                            if (curPos.TdPosition > 0 && (x - y) - (int)Math.Min(curPos.YdPosition, x - y) > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, (x - y) - (int)Math.Min(curPos.YdPosition, x - y),
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long, TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                        }
                        else//(x - y > x )先平掉x手多头（即所有多头），再开-y手空头
                        {
                            //先平掉所有多头
                            if (curPos.YdPosition > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, curPos.YdPosition,
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long, TThostFtdcOffsetFlagType.THOST_FTDC_OF_Close, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                            if (curPos.TdPosition > 0)
                            {
                                resultOrders.Add(ClosePositionOrder(instID, curPos.TdPosition,
                                    TThostFtdcPosiDirectionType.THOST_FTDC_PD_Long, TThostFtdcOffsetFlagType.THOST_FTDC_OF_CloseToday, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                            }
                            //再开-y手空头
                            resultOrders.Add(OpenPositionOrder(instID, expPos.AvgOpenPrice, -y, TThostFtdcDirectionType.THOST_FTDC_D_Sell, TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_LimitPrice));
                        }
                    }
                    #endregion
                }
            }
            return resultOrders.ToArray();
        }

        #endregion 报单构建函数

        //下单函数
        public void SendOrders()
        {
            foreach (var order in this._ocStrategyOrders)
            {
                this._trdApi.SendOrder(order);
            }
            this._ocStrategyOrders.Clear();
        }

        public override string ToString()
        {
            return this._strategyName;
        }

    }

}
