using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT_CTP
{
    /// <summary>
    /// 价格条件单
    /// </summary>
    public sealed class PriceConditionOrderField : OrderField
    {
        #region ==========内部变量==========

        private bool _isRunning = false;
        private ConditionOrderType _conditionOrderType = ConditionOrderType.PriceCondition;
        private string _expressionSymbol;
        private double _targetPrice;
        private bool _isOrderSent = false;
        private string _conditionText;
        private ConditionOrderExecutionStatus _conditionOrderExecutionStatus = ConditionOrderExecutionStatus.NotRunning;
        private static readonly object _lock = new object();

        private ConditionOrderExecutionTrigger _conditionOrderExecutionTrigger;
        private delegate bool DeleIsPriceConditionSatisfied(DepthMarketData dmd);
        private DeleIsPriceConditionSatisfied _isPriceConditionSatisfied;

        #endregion ==========内部变量==========

        #region ==========属性==========
        
        [DisplayName("执行状态")]
        public ConditionOrderExecutionStatus ExecutionStatus { get { return this._conditionOrderExecutionStatus; } }
        [DisplayName("条件单说明")]
        public string ConditionText { get { return this._conditionText; } }
        [DisplayName("条件单类型")]
        public ConditionOrderType ConditionOrderType { get { return this._conditionOrderType; } }
        [DisplayName("表达式")]
        public string ExpressionSymbol { get { return _expressionSymbol; } set { this._expressionSymbol = value; } }
        [DisplayName("条件价格")]
        public double TargetPrice { get { return this._targetPrice; } set { this._targetPrice = value; } }
        [DisplayName("是否运行中")]
        public bool IsRunning
        {
            get { return this._isRunning; }
            set
            {
                lock (_lock)
                {
                    this._isRunning = value;
                    if (this._isRunning)
                    {
                        this._conditionOrderExecutionStatus = ConditionOrderExecutionStatus.Running;

                        this._conditionText = string.Format("市价 {0} {1}", this._expressionSymbol, this._targetPrice);
                        if (this._expressionSymbol == ">")
                        {
                            this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice > this._targetPrice; };
                        }
                        else if (this._expressionSymbol == ">=")
                        {
                            this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice >= this._targetPrice; };
                        }
                        else if (this._expressionSymbol == "=")
                        {
                            this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice == this._targetPrice; };
                        }
                        else if (this._expressionSymbol == "<=")
                        {
                            this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice <= this._targetPrice; };
                        }
                        else if (this._expressionSymbol == "<")
                        {
                            this._isPriceConditionSatisfied = (dmd) => { return dmd.LastPrice < this._targetPrice; };
                        }
                    }
                    else
                    {
                        this._conditionOrderExecutionStatus = ConditionOrderExecutionStatus.NotRunning;
                        this._isOrderSent = false;
                    }
                    this.Notify("");
                }
            }
        }

        #endregion ==========属性==========

        #region ==========事件==========

        public delegate void ConditionOrderExecutionTrigger(PriceConditionOrderField cof);
        public event ConditionOrderExecutionTrigger ExecutionTrigger { add { this._conditionOrderExecutionTrigger += value; } remove { this._conditionOrderExecutionTrigger -= value; } }
        
        #endregion ==========事件==========
        
        /// <summary>
        /// 启动，绑定OnRtnDMD
        /// </summary> 
        public void Run(DepthMarketData dmd)
        {
            lock (_lock)
            {
                if (!this._isOrderSent && dmd.InstrumentID == this.InstrumentID 
                    && this._isPriceConditionSatisfied != null && this._isRunning
                    && this._isPriceConditionSatisfied(dmd) 
                    && this._conditionOrderExecutionStatus == ConditionOrderExecutionStatus.Running)
                {
                    this._isRunning = false;
                    this._conditionOrderExecutionTrigger?.Invoke(this);
                    this._isOrderSent = true;
                    this._conditionOrderExecutionStatus = ConditionOrderExecutionStatus.Complete;
                    Notify("");
                }
            }
        }

    }

}
