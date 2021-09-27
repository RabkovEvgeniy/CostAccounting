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
        public Expense(double cost, string category, DateTime date) 
        {
            Cost = cost;
            Category = category;
            DateTime = date;
        }
        public double Cost { get; set; }
        public string Category { get; set; }
        public string Date { get => DateTime.ToString("dd.MM.yy"); }
        public DateTime DateTime { set; get; }
    }
}
