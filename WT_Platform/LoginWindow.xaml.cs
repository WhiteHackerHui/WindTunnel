using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WT_CTP;
using WT_Core;
using System.Threading;

namespace WT_Platform
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        //内部变量
        private readonly CMdApi _quoteApi;
        private readonly CTradeApi _tradeApi;
        private readonly MainWindow _mw;
        private readonly List<Broker> _listBroker;
        private readonly List<Investor> _listInvestor;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();


        /// <summary>
        /// 构造函数，传入PlatForm中的行情和交易Api作为参数
        /// </summary>
        public LoginWindow(MainWindow mw, CMdApi quoteApi, CTradeApi tradeApi)
        {
            InitializeComponent();

            //内部变量赋值
            this._mw = mw;
            this._quoteApi = quoteApi;
            this._tradeApi = tradeApi;
            this._listBroker = this.GetBrokerList();
            this._listInvestor = this.GetInvestorList();

            //行情交易参数初始化
            this.cbxServer.ItemsSource      = _listBroker.Select(fb => fb.Name);
            this.cbxBrokerID.ItemsSource    = _listBroker.Select(fb => fb.BrokerID);
            this.cbxQuoteIP.ItemsSource     = _listBroker.Select(fb => fb.QuoteIP);
            this.cbxQuotePort.ItemsSource   = _listBroker.Select(fb => fb.QuotePort);
            this.cbxTradeIP.ItemsSource     = _listBroker.Select(fb => fb.TradeIP);
            this.cbxTradePort.ItemsSource   = _listBroker.Select(fb => fb.TradePort);

            //账户参数初始化
            this.cbxInvestorID.ItemsSource  = _listInvestor.Select(i => i.InvestorID);
            this.cbxPassword.ItemsSource    = _listInvestor.Select(i => i.Password);

            #if DEBUG
            this.cbxBrokerID.SelectedIndex = 0;
            this.cbxPassword.SelectedIndex = 0;
            this.cbxServer.SelectedIndex = 0;
            this.cbxInvestorID.SelectedIndex = 0;
            this.cbxPassword.SelectedIndex = 0;
            #else
            this.cbxBrokerID.SelectedIndex = 1;
            this.cbxPassword.SelectedIndex = 1;
            this.cbxServer.SelectedIndex = 5;
            this.cbxInvestorID.SelectedIndex = 1;
            this.cbxPassword.SelectedIndex = 1;
            #endif

        }

        /// <summary>
        /// 得到行情、交易服务器设置信息
        /// </summary>
        /// <param name="ServerPath"></param>
        /// <returns></returns>
        private List<Broker> GetBrokerList(string ServerPath = "./server.json")
        {
            var list = new List<Broker>();
            if (File.Exists(ServerPath))
                list = JsonConvert.DeserializeObject<List<Broker>>(File.ReadAllText(ServerPath, Encoding.Default));
            else
            {
                list.Add(new Broker { Name = "模拟 电信1", BrokerID = "9999", TradeIP = "180.168.146.187", TradePort = 10000, QuoteIP = "180.168.146.187", QuotePort = 10010 });
                list.Add(new Broker { Name = "模拟 电信2", BrokerID = "9999", TradeIP = "180.168.146.187", TradePort = 10001, QuoteIP = "180.168.146.187", QuotePort = 10011 });
                list.Add(new Broker { Name = "模拟 移动", BrokerID = "9999", TradeIP = "218.202.237.33", TradePort = 10002, QuoteIP = "218.202.237.33", QuotePort = 10012 });
                list.Add(new Broker { Name = "CTP Mini", BrokerID = "9999", TradeIP = "180.168.146.187", TradePort = 10003, QuoteIP = "180.168.146.187", QuotePort = 10013 });
                list.Add(new Broker { Name = "CTP 7*24", BrokerID = "9999", TradeIP = "180.168.146.187", TradePort = 10030, QuoteIP = "180.168.146.187", QuotePort = 10031 });
                File.WriteAllText(ServerPath, JsonConvert.SerializeObject(list), Encoding.Default);
            }
            return list;
        }

        /// <summary>
        /// 得到用户账户设置信息
        /// </summary>
        private List<Investor> GetInvestorList(string InvestorPath = "./investor.json")
        {
            var list = new List<Investor>();
            if (File.Exists(InvestorPath))
                list = JsonConvert.DeserializeObject<List<Investor>>(File.ReadAllText(InvestorPath, Encoding.Default));
            else
            {
                File.WriteAllText(InvestorPath, JsonConvert.SerializeObject(list), Encoding.Default);
            }
            return list;
        }

        /// <summary>
        /// 是否是今日第一次登陆
        /// </summary>
        private bool IsFirstTimeLogin(string LastLoginDateTimePath = "./lastloginindatetime.json")
        {
            DateTime lastLoginDate = DateTime.MinValue;
            if (File.Exists(LastLoginDateTimePath))
                lastLoginDate = JsonConvert.DeserializeObject<DateTime>(File.ReadAllText(LastLoginDateTimePath, Encoding.Default));
            bool isFirstTimeLogin = true;

            DateTime dt1, dt2;
            if (DateTime.Now.Hour < 17)
            {
                dt1 = DateTime.Now.AddDays(-1).Date.AddHours(21);
                dt2 = DateTime.Now.Date.AddHours(15);
            }
            else
            {
                dt1 = DateTime.Now.Date.AddHours(20);
                dt2 = DateTime.Now.Date.AddDays(1).AddHours(15);
            }
            if (lastLoginDate > dt1)
            {
                isFirstTimeLogin = false;
            }
            File.WriteAllText(LastLoginDateTimePath, JsonConvert.SerializeObject(DateTime.Now), Encoding.Default);
            return isFirstTimeLogin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #if DEBUG
            #else
                this.btnLogin_Click(sender,e);
            #endif
        }

        /// <summary>
        /// 登录确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //禁用
                this.btnLogin.IsEnabled = false;
                this.btnCancel.IsEnabled = true;
                //交易API初始化
                this._tradeApi.Investor = this.cbxInvestorID.Text;
                this._tradeApi.Password = this.cbxPassword.Text;
                this._tradeApi.Broker = this.cbxBrokerID.Text;
                this._tradeApi.Address = "tcp://" + cbxTradeIP.Text + ":" + cbxTradePort.Text;
                //行情API初始化
                this._quoteApi.Investor = this.cbxInvestorID.Text;
                this._quoteApi.Password = this.cbxPassword.Text;
                this._quoteApi.Broker = this.cbxBrokerID.Text;
                this._quoteApi.Address = "tcp://" + cbxQuoteIP.Text + ":" + cbxQuotePort.Text;

                //同步行情连接    
                tbStatus.Text = "正在连接行情端...";
                //this._quoteApi.Connect();
                await Task.Run(() => this._quoteApi.Connect(), this._cts.Token);
                tbStatus.Text = "行情端已连接！";
                progressBar.Value += 10;

                //同步交易连接
                tbStatus.Text = "正在连接交易端...";
                await Task.Run(() => this._tradeApi.Connect(), this._cts.Token);
                tbStatus.Text = "交易端已连接！";
                progressBar.Value += 10;

                //同步登陆
                tbStatus.Text = "正在登陆行情端...";
                await Task.Run(() => this._quoteApi.Login(), this._cts.Token);
                tbStatus.Text = "行情端已登陆！";
                progressBar.Value += 10;
                tbStatus.Text = "正在登陆交易端...";
                await Task.Run(() => this._tradeApi.Login(), this._cts.Token);
                tbStatus.Text = "交易端已登陆！";
                progressBar.Value += 10;

                //如果是今天第一次登陆，则要确认结算单
                if (IsFirstTimeLogin() || (bool)this.chxIsConfirmSettlementInfo.IsChecked)
                {
                    tbStatus.Text = "正在查询结算信息...";
                    await Task.Run(() => this._tradeApi.SyncReqQrySettlementInfo(""), this._cts.Token);
                    MessageBox.Show(this._tradeApi.SettlementInfoContent);
                    tbStatus.Text = "正在确认结算信息...";
                    await Task.Run(() => this._tradeApi.SyncReqSettlementInfoConfirm(), this._cts.Token);
                    tbStatus.Text = "已确认结算单！";
                    progressBar.Value += 10;
                }

                //查询合约
                tbStatus.Text = "正在查询合约...";
                await Task.Run(() => this._tradeApi.SyncReqQryInstrumentField(), this._cts.Token);
                tbStatus.Text = "合约查询完毕！";
                progressBar.Value += 10;

                //查询持仓细节
                tbStatus.Text = "正在查询持仓细节...";
                await Task.Run(() => this._tradeApi.SyncReqQryDetailPosition(), this._cts.Token);
                tbStatus.Text = "持仓细节查询完毕！";
                progressBar.Value += 10;

                //查询持仓
                tbStatus.Text = "正在查询持仓...";
                await Task.Run(() => this._tradeApi.SyncReqQryInvestorPosition(), this._cts.Token);
                tbStatus.Text = "持仓查询完毕！";
                progressBar.Value += 10;

                //查询交易账户信息
                tbStatus.Text = "正在查询交易账户...";
                await Task.Run(() => this._tradeApi.ReqQry_TradeAccount(), this._cts.Token);
                tbStatus.Text = "交易账户查询完毕！";
                progressBar.Value += 10;

                //订阅行情数据
                tbStatus.Text = "正在订阅行情数据...";
                this._quoteApi.ReqSubscribeMarketData(this._tradeApi.DictInstrumentField.Keys.ToArray());
                progressBar.Value += 10;
            }
            catch (Exception ex)
            {
                this._quoteApi.Logout();
                this._tradeApi.Logout();
                MessageBox.Show(ex.Message);
            }
            //关闭登录窗口
            Close();

        }

        /// <summary>
        /// 取消登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this._cts.Cancel();
            this.btnCancel.IsEnabled = false;
            this.btnLogin.IsEnabled = true;
            this.progressBar.Value = 0;
        }

        private void offlineLogin(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 根据选中的Broker，自动绑定设置
        /// </summary>
        private void cbxServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Broker fbroker = this._listBroker.ElementAtOrDefault(cbxServer.SelectedIndex);
            if (fbroker != null)
            {
                this.cbxQuoteIP.Text = fbroker.QuoteIP;
                this.cbxQuotePort.Text = fbroker.QuotePort.ToString();
                this.cbxTradeIP.Text = fbroker.TradeIP;
                this.cbxTradePort.Text = fbroker.TradePort.ToString();
                this.cbxBrokerID.Text = fbroker.BrokerID.ToString();
            }

        }
    }
}
