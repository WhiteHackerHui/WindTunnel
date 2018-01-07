using System.ComponentModel;

namespace WT_Platform
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
