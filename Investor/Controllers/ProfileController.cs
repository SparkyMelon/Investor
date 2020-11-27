using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Investor.Data;
using Investor.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Investor.Controllers
{
    public class ProfileController : Controller
    {
        private readonly InvestorContext _context;

        public ProfileController(InvestorContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? accountId = HttpContext.Session.GetInt32("Id");
            
            if (accountId == null)
            {
                throw new Exception("This only ever happens during debugging while session strings are set manually");
            }

            List<CampaignInfo> campaignsInvestingIn = new List<CampaignInfo>();
            List<CampaignInfo> campaignsStarted = new List<CampaignInfo>();
            List<Company> companiesOwned = new List<Company>();

            //Get campaigns investing in
            DbData.Instance.GetSqlCon().Open();
            SqlCommand command = new SqlCommand(@"
                SELECT TOP(5) *
                FROM CAMPAIGN
                    LEFT JOIN PAYMENT
                    ON CAMPAIGN.ID = PAYMENT.CAMPAIGN_ID
                    LEFT JOIN COMPANY
	                ON CAMPAIGN.COMPANY_ID = COMPANY.ID
                WHERE PAYMENT.ACCOUNT_ID = @id
                ORDER BY CAMPAIGN.START_DATE DESC
            ", DbData.Instance.GetSqlCon());
            command.Parameters.Add(new SqlParameter("@id", accountId));
            using (SqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    CampaignInfo campaign = new CampaignInfo
                    {
                        Id = (int)dr["Id"],
                        CompanyId = (int)dr["COMPANY_ID"],
                        Description = dr["DESCRIPTION"].ToString(),
                        CashGoal = (decimal)dr["CASH_GOAL"],
                        StartDate = (DateTime)dr["START_DATE"],
                        EndDate = (DateTime)dr["END_DATE"],
                        GoalReached = (bool)dr["GOAL_REACHED"],
                        CompanyName = dr["NAME"].ToString(),
                        AccountId = (int)dr["ACCOUNT_ID"]
                    };
                    if (dr["SUCCESS"] == DBNull.Value)
                        campaign.success = null;
                    else
                        campaign.success = (bool?)dr["SUCCESS"];

                    campaignsInvestingIn.Add(campaign);
                }
            }

            //Get all money invested into each campaign
            for (int i = 0; i < campaignsInvestingIn.Count; i++)
            {
                command = new SqlCommand(@"
                    SELECT SUM(AMOUNT) AS a
                    FROM PAYMENT
                    WHERE ACCOUNT_ID = @id AND CAMPAIGN_ID = @campaignId
                    ", DbData.Instance.GetSqlCon());
                command.Parameters.Add(new SqlParameter("@id", campaignsInvestingIn[i].AccountId));
                command.Parameters.Add(new SqlParameter("@campaignId", campaignsInvestingIn[i].Id));
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        campaignsInvestingIn[i].CashCurrent = (decimal)dr["a"];
                    }
                }
            }

            ViewBag.campaignsInvestingIn = campaignsInvestingIn;

            //Get campaigns started
            command = new SqlCommand(@"
                SELECT TOP(5) *
                FROM CAMPAIGN
                    LEFT JOIN COMPANY
                    ON CAMPAIGN.COMPANY_ID = COMPANY.ID
	                LEFT JOIN ACCOUNT
	                ON COMPANY.ACCOUNT_ID = ACCOUNT.ID
                WHERE ACCOUNT.ID = @id
                ORDER BY CAMPAIGN.START_DATE DESC
            ", DbData.Instance.GetSqlCon());
            command.Parameters.Add(new SqlParameter("@id", accountId));
            using (SqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    CampaignInfo campaign = new CampaignInfo
                    {
                        Id = (int)dr["Id"],
                        CompanyId = (int)dr["COMPANY_ID"],
                        Description = dr["DESCRIPTION"].ToString(),
                        CashGoal = (decimal)dr["CASH_GOAL"],
                        StartDate = (DateTime)dr["START_DATE"],
                        EndDate = (DateTime)dr["END_DATE"],
                        GoalReached = (bool)dr["GOAL_REACHED"],
                        CompanyName = dr["NAME"].ToString(),
                        AccountId = (int)dr["ACCOUNT_ID"]
                    };
                    if (dr["SUCCESS"] == DBNull.Value) 
                        campaign.success = null;
                    else 
                        campaign.success = (bool?)dr["SUCCESS"];

                    campaignsStarted.Add(campaign);
                }
            }

            //Get all money invested into each campaign
            for (int i = 0; i < campaignsStarted.Count; i++)
            {
                command = new SqlCommand(@"
                    SELECT SUM(AMOUNT) AS a
                    FROM PAYMENT
                    WHERE ACCOUNT_ID = @id AND CAMPAIGN_ID = @campaignId
                    ", DbData.Instance.GetSqlCon());
                command.Parameters.Add(new SqlParameter("@id", campaignsStarted[i].AccountId));
                command.Parameters.Add(new SqlParameter("@campaignId", campaignsStarted[i].Id));
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        campaignsStarted[i].CashCurrent = (decimal)dr["a"];
                    }
                }
            }

            ViewBag.campaignsStarted = campaignsStarted;

            //Get companies owned
            command = new SqlCommand(@"
                SELECT TOP(5) *
                FROM COMPANY
                WHERE ACCOUNT_ID = @id
                ", DbData.Instance.GetSqlCon());
            command.Parameters.Add(new SqlParameter("@id", accountId));
            using (SqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    Company company = new Company
                    {
                        Id = (int)dr["ID"],
                        AccountId = (int)dr["ACCOUNT_ID"],
                        Name = dr["NAME"].ToString()
                    };

                    companiesOwned.Add(company);
                }
            }
            
            ViewBag.companiesOwned = companiesOwned;

            //Get total invested money
            decimal total = 0.00m;
            command = new SqlCommand(@"
                    SELECT SUM(AMOUNT) AS a
                    FROM PAYMENT
                    WHERE ACCOUNT_ID = @id
                    ", DbData.Instance.GetSqlCon());
            command.Parameters.Add(new SqlParameter("@id", accountId));
            using (SqlDataReader dr = command.ExecuteReader())
            {
                if (dr.Read())
                {
                    total = (decimal)dr["a"];
                }
            }

            ViewBag.totalInvested = total;

            DbData.Instance.GetSqlCon().Close();

            return View();
        }
    }
}
