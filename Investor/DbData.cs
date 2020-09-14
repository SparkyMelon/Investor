using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Investor
{
    public sealed class DbData
    {
        private static DbData instance = null;
        private static readonly object padlock = new object();

        private SqlConnection sqlCon;

        public DbData()
        {
            //Trusted_Conn‌ection=True;Multiple‌​ActiveResultSets=tru‌​e;
            sqlCon = new SqlConnection("Server=SPARKY-PC\\SQLEXPRESS;Database=PokeData;Integrated Security=True");
        }

        public static DbData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DbData();
                    }

                    return instance;
                }
            }
        }
    }
}
