using System;

namespace CostAccounting.Models
{
    public class Expense
    {
        public Expense(double cost, string category, DateTime date, int id = NaN)
        {
            Id = id;
            Cost = cost;
            Category = category;
            DateTime = date;
        }

        public int Id { get; set; }
        public double Cost { get; set; }
        public string Category { get; set; }
        public DateTime DateTime { set; get; }

        public string Date { get => DateTime.Date.ToString("yyyy.MM.dd"); }

        private const int NaN = -1;
    }
}
