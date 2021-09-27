using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CostAccounting.Models;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace CostAccounting
{
    class UserQueryFacade
    {
        private const string COST_PARAMETER_NAME = "@cost";
        private const string CATEGORY_PARAMETER_NAME = "@category";
        private const string DATE_PARAMETER_NAME = "@date";

        private readonly string _insertExpenseSqlQuery = "INSERT INTO [expenses] ([cost],[category],[date]) VALUES" +
            $" ({COST_PARAMETER_NAME},{CATEGORY_PARAMETER_NAME},{DATE_PARAMETER_NAME});";

        private SqlCommand _insertExpenseSqlCommand;

        public UserQueryFacade() 
        {
            _insertExpenseSqlCommand = new SqlCommand(_insertExpenseSqlQuery);
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(COST_PARAMETER_NAME,System.Data.SqlDbType.Float));
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(CATEGORY_PARAMETER_NAME, System.Data.SqlDbType.NVarChar, 10));
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(DATE_PARAMETER_NAME, System.Data.SqlDbType.Date));
        }

        public async Task CreateExpenseRecordAsync(Expense expense) 
        {
            using(SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString)) 
            {
                await sqlConnection.OpenAsync();
                _insertExpenseSqlCommand.Connection = sqlConnection;

                int costParametrIndex = _insertExpenseSqlCommand.Parameters.IndexOf(COST_PARAMETER_NAME);
                int categoryParametrIndex = _insertExpenseSqlCommand.Parameters.IndexOf(CATEGORY_PARAMETER_NAME);
                int dateParametrIndex = _insertExpenseSqlCommand.Parameters.IndexOf(DATE_PARAMETER_NAME);

                _insertExpenseSqlCommand.Parameters[costParametrIndex].Value = expense.Cost;
                _insertExpenseSqlCommand.Parameters[categoryParametrIndex].Value = expense.Category;
                _insertExpenseSqlCommand.Parameters[dateParametrIndex].Value = expense.DateTime;

                await _insertExpenseSqlCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
