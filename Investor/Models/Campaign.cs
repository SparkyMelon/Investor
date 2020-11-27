using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Investor.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public decimal CashGoal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool GoalReached { get; set; }
        public bool? success { get; set; }
    }
}
