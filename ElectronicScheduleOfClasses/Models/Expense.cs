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
        public Expense(int id, double cost, string category, DateTime date) 
        {
            Id = id;
            Cost = cost;
            Category = category;
            DateTime = date;
        }
        public Expense( double cost, string category, DateTime date) 
        {
            Id = -1;
            Cost = cost;
            Category = category;
            DateTime = date;
        }

        public int Id { get; set; }
        public double Cost { get; set; }
        public string Category { get; set; }
        public string Date { get => DateTime.Date.ToString("yyyy.MM.dd"); }
        public DateTime DateTime { set; get; }
    }
}
