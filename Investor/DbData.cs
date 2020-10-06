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

        private static SqlConnection sqlCon;

        public DbData()
        {
            //Trusted_Conn‌ection=True;Multiple‌​ActiveResultSets=tru‌​e;
            sqlCon = new SqlConnection("Server=SPARKY-PC\\SQLEXPRESS;Database=Investor;Integrated Security=True");
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

        public SqlConnection GetSqlCon()
        {
            return sqlCon;
        }

        public void SQLQuery(string sql, List<SqlParameter> sqlParams = null)
        {
            sqlCon.Open();

            SqlCommand command;
            command = new SqlCommand(sql, sqlCon);

            if (sqlParams != null)
            {
                foreach (SqlParameter sqlParam in sqlParams)
                {
                    command.Parameters.Add(sqlParam);
                }
            }

            try
            {
                command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            sqlCon.Close();
        }

        public bool DoesTableExist(string tableName)
        {
            bool res;

            sqlCon.Open();
            SqlCommand command = new SqlCommand("SELECT 1 FROM Investor.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName", sqlCon);
            command.Parameters.Add(new SqlParameter("@tableName", tableName));
            using (SqlDataReader dr = command.ExecuteReader())
            {
                if (dr.Read()) res = true;
                else res = false;
            }
            sqlCon.Close();
            return res;
        }

        public void CheckDatabase()
        {
            if (!DoesTableExist("ACCOUNT")) //TODO: Get rid of COMPANY column
            {
                string sql = @"
                    CREATE TABLE ACCOUNT (
                        ID int IDENTITY(1,1) PRIMARY KEY,
                        USERNAME varchar(25) NOT NULL,
                        EMAIL varchar(50) NOT NULL,
                        PASSWORD varchar(500) NOT NULL,
                        SALT binary(32) NOT NULL,
                        COMPANY bit NOT NULL
                    )
                ";
                SQLQuery(sql);
            }
        }
    }
}
