using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Investor.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int CampaignId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
    }
}
