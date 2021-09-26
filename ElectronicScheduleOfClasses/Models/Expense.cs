using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CostAccounting.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public double Cost { get; set; }
        public string Category { get; set; }
        public string Date { get => DateTime.ToString("dd.MM.yy"); }
        public DateTime DateTime { set; get; }
        public Expense() => Id = createdExpenseCount++;
        public Expense(int id, double cost, string category, DateTime date) 
        {
            Id = id;
            Cost = cost;
            Category = category;
            DateTime = date;
        }
        static Expense()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
            {
                connection.Open();
                SqlCommand getMaxIdCommand = new SqlCommand(getMaxIdSQLQuery, connection);
                var MaxID = getMaxIdCommand.ExecuteScalar() ?? 0;
            }
        }

        private static int createdExpenseCount;
        
        private const string getMaxIdSQLQuery = "SELECT MAX([id]) FROM expenses";
    }
}
