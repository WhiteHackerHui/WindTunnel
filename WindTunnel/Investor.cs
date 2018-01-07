using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindTunnel
{
    public class Investor
    {
        [DisplayName("用户名")]
        public string InvestorID { get; set; }
        [DisplayName("密码")]
        public string Password { get; set; }
        public override string ToString()
        {
            return InvestorID;
        }
    }
}
