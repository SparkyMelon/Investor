using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Investor.Models;

namespace Investor.Data
{
    public class InvestorContext : DbContext
    {
        public InvestorContext (DbContextOptions<InvestorContext> options) : base(options)
        {

        }

        public DbSet<Account> Account { get; set; }
    }
}
