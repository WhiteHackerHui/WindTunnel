using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindTunnel
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        //内部变量
        private CancellationTokenSource _cts;
        private CTradeApi _trdApi;
        private CMdApi _mktApi;

        public LoginWindow()
        {
            InitializeComponent();

            this._cts = new CancellationTokenSource();
            Body.ListFutureBroker = InitServer();
            this.cbxServer.ItemsSource = Body.ListFutureBroker.Select(fb => fb.Name);
            this.cbxBrokerID.ItemsSource = Body.ListFutureBroker.Select(fb => fb.Broker);
            this.cbxQuoteIP.ItemsSource = Body.ListFutureBroker.Select(fb => fb.QuoteIP);
            this.cbxQuotePort.ItemsSource = Body.ListFutureBroker.Select(fb => fb.QuotePort);
            this.cbxTradeIP.ItemsSource = Body.ListFutureBroker.Select(fb => fb.TradeIP);
            this.cbxTradePort.ItemsSource = Body.ListFutureBroker.Select(fb => fb.TradePort);
            Body.ListInvestor = InitInvestor();
            this.cbxInvestorID.ItemsSource = Body.ListInvestor.Select(i => i.InvestorID);
            this.cbxPassword.ItemsSource = Body.ListInvestor.Select(i => i.Password);

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

        //登录
        async private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //禁用
                this.btnLogin.IsEnabled = false;
                this.btnCancel.IsEnabled = true;
                //API初始化
                this._trdApi = new CTradeApi(this.cbxInvestorID.Text, this.cbxPassword.Text, this.cbxBrokerID.Text, "tcp://" + cbxTradeIP.Text + ":" + cbxTradePort.Text);
                this._mktApi = new CMdApi(this.cbxInvestorID.Text, this.cbxPassword.Text, this.cbxBrokerID.Text, "tcp://" + cbxQuoteIP.Text + ":" + cbxQuotePort.Text);
                Body.trdApi = this._trdApi;
                Body.mktApi = this._mktApi;

                #region 绑定后台Body
                this._mktApi.OnTick += Body.Body_OnTick;
                this._trdApi.OnTrade += Body.Body_OnRtnTrade;
                //this._trdApi.OnDetailPosition += Body.Body_OnDetailPosition;
                //this._trdApi.OnPosition += Body.Body_OnPosition;
                this._trdApi.OnOrder += Body.Body_OnOrderField;
                this._trdApi.OnErrorOrder += Body.Body_OnErrorOrder;
                this._trdApi.OnUpdate += Body.Body_OnUpdate;
                this._trdApi.OnTradingAccount += Body.Body_OnTradingAccount;
                #endregion

                //连接    
                tbStatus.Text = "正在连接行情端...";
                await this._mktApi.AsyncConnect(_cts.Token);
                tbStatus.Text = "行情端已连接！";
                progressBar.Value += 10;
                tbStatus.Text = "正在连接交易端...";
                await _trdApi.AsyncConnect(_cts.Token);
                tbStatus.Text = "交易端已连接！";
                progressBar.Value += 10;
                //登陆
                tbStatus.Text = "正在登陆行情端...";
                await _mktApi.AsyncLogin(_cts.Token);
                tbStatus.Text = "行情端已登陆！";
                progressBar.Value += 10;
                tbStatus.Text = "正在登陆交易端...";
                await _trdApi.AsyncLogin(_cts.Token);
                tbStatus.Text = "交易端已登陆！";
                progressBar.Value += 10;
                //如果是今天第一次登陆，则要确认结算单
                if (IsFirstTimeLogin() || (bool)this.chxIsConfirmSettlementInfo.IsChecked)
                {
                    tbStatus.Text = "正在查询结算信息...";
                    _trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.QrySettlementInfo, string.Empty));
                    await _trdApi.ExecStackQuery();
                    MessageBox.Show(_trdApi.SettlementInfoContent);
                    tbStatus.Text = "正在确认结算信息...";
                    _trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.SettlementInfoConfirm, string.Empty));
                    await _trdApi.ExecStackQuery();
                    tbStatus.Text = "已确认结算单！";
                    progressBar.Value += 10;
                }
                //查询合约
                tbStatus.Text = "正在查询合约...";
                _trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.QryInstrument, string.Empty));
                await _trdApi.ExecStackQuery();
                tbStatus.Text = "合约查询完毕！";
                progressBar.Value += 10;
                //查询持仓细节
                tbStatus.Text = "正在查询持仓细节...";
                _trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.QryInvestorPositionDetail, string.Empty));
                await _trdApi.ExecStackQuery();
                tbStatus.Text = "持仓细节查询完毕！";
                progressBar.Value += 10;
                //查询持仓
                tbStatus.Text = "正在查询持仓...";
                _trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.QryInvestorPosition, string.Empty));
                await _trdApi.ExecStackQuery();
                tbStatus.Text = "持仓查询完毕！";
                progressBar.Value += 10;
                //查询交易账户信息
                tbStatus.Text = "正在交易账户...";
                _trdApi.StackQuery.Push(new Tuple<EnumReqCmdType, object>(EnumReqCmdType.QryTradingAccount, string.Empty));
                await _trdApi.ExecStackQuery();
                tbStatus.Text = "交易账户查询完毕！";
                progressBar.Value += 10;
                //订阅行情数据
                tbStatus.Text = "正在订阅行情数据...";
                _mktApi.ReqSubscribeMarketData(_trdApi.DicInstrumentField.Keys.ToArray());
                progressBar.Value += 10;

                //调用主交易窗口
                Hide();
                MainWindow mainWindow = new MainWindow();
                Body.mainWindow = mainWindow;
                mainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                //this._mktApi.ReqUserLogout();
                //this._trdApi.ReqUserLogout();
                MessageBox.Show(ex.Message);
                this.btnCancel.IsEnabled = false;
                this.btnLogin.IsEnabled = true;
                this.progressBar.Value = 0;
            }
            finally
            {
                //关闭登录窗口
                Close();
            }    
           
        }



        //取消登录
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this._cts.Cancel();
        }

        //脱机登录
        private void offlineLogin(object sender, RoutedEventArgs e)
        {

        }

        //发布版的自动登录
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #if DEBUG
            #else
            this.btnLogin_Click(sender,e);
            #endif
        }

        //初始化服务前置
        private List<FutureBroker> InitServer(string ServerPath = "./server.json")
        {
            var list = new List<FutureBroker>();
            if (File.Exists(ServerPath))
                list = JsonConvert.DeserializeObject<List<FutureBroker>>(File.ReadAllText(ServerPath, Encoding.Default));
            else
            {
                list.Add(new FutureBroker
                {
                    Name = "模拟 电信1",
                    Broker = "9999",
                    TradeIP = "180.168.146.187",
                    TradePort = 10000,
                    QuoteIP = "180.168.146.187",
                    QuotePort = 10010,

                });
                list.Add(new FutureBroker
                {
                    Name = "模拟 电信2",
                    Broker = "9999",
                    TradeIP = "180.168.146.187",
                    TradePort = 10001,
                    QuoteIP = "180.168.146.187",
                    QuotePort = 10011,
                });
                list.Add(new FutureBroker
                {
                    Name = "模拟 移动",
                    Broker = "9999",
                    TradeIP = "218.202.237.33",
                    TradePort = 10002,
                    QuoteIP = "218.202.237.33",
                    QuotePort = 10012,
                });
                list.Add(new FutureBroker
                {
                    Name = "CTP Mini",
                    Broker = "9999",
                    TradeIP = "180.168.146.187",
                    TradePort = 10003,
                    QuoteIP = "180.168.146.187",
                    QuotePort = 10013,
                });
                list.Add(new FutureBroker
                {
                    Name = "CTP 7*24",
                    Broker = "9999",
                    TradeIP = "180.168.146.187",
                    TradePort = 10030,
                    QuoteIP = "180.168.146.187",
                    QuotePort = 10031,
                });
                File.WriteAllText(ServerPath, JsonConvert.SerializeObject(list),Encoding.Default);
            }
            return list;
        }

        //初始化账户密码
        private List<Investor> InitInvestor(string InvestorPath= "./investor.json")
        {
            var list = new List<Investor>();
            if (File.Exists(InvestorPath))
                list = JsonConvert.DeserializeObject<List<Investor>>(File.ReadAllText(InvestorPath, Encoding.Default));
            else
            {
                File.WriteAllText(InvestorPath, JsonConvert.SerializeObject(list),Encoding.Default);
            }
            return list;
        }

        //是否是今日第一次登陆
        private bool IsFirstTimeLogin(string LastLoginDateTimePath = "./lastloginindatetime.json")
        {
            DateTime lastLoginDate = DateTime.MinValue;
            if (File.Exists(LastLoginDateTimePath))
                lastLoginDate = JsonConvert.DeserializeObject<DateTime>(File.ReadAllText(LastLoginDateTimePath, Encoding.Default));
            bool isFirstTimeLogin = true;

            DateTime dt1,dt2;
            if (DateTime.Now.Hour < 17 )
            {
                dt1= DateTime.Now.AddDays(-1).Date.AddHours(21);
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

        //自动绑定设置
        private void cbxServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FutureBroker fbroker = Body.ListFutureBroker.ElementAtOrDefault(cbxServer.SelectedIndex);
            if (fbroker != null)
            {
                this.cbxQuoteIP.Text = fbroker.QuoteIP;
                this.cbxQuotePort.Text = fbroker.QuotePort.ToString();
                this.cbxTradeIP.Text = fbroker.TradeIP;
                this.cbxTradePort.Text = fbroker.TradePort.ToString();
                this.cbxBrokerID.Text = fbroker.Broker.ToString();
            }
            
        }

    }
}
