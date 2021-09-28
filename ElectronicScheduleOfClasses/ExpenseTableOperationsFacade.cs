using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;

using Microsoft.Data.SqlClient;

using CostAccounting.Models;

namespace CostAccounting
{
    class ExpenseTableOperationsFacade
    {
        public ExpenseTableOperationsFacade()
        {
            _getAllExpenseSqlCommand = new SqlCommand(_getAllExpenseSqlQuery);

            _deleteExpenseSqlCommand = new SqlCommand(_deleteExpenseSqlQuery);
            _deleteExpenseSqlCommand.Parameters.Add(new SqlParameter(ID_PARAMETER_NAME, System.Data.SqlDbType.Int));

            _insertExpenseSqlCommand = new SqlCommand(_insertExpenseSqlQuery);
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(COST_PARAMETER_NAME, System.Data.SqlDbType.Float));
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(CATEGORY_PARAMETER_NAME, System.Data.SqlDbType.NVarChar, 10));
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(DATE_PARAMETER_NAME, System.Data.SqlDbType.Date));

            _updateExpenseSqlCommand = new SqlCommand(_updateExpenseSqlQuery);
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(COST_PARAMETER_NAME, System.Data.SqlDbType.Float));
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(CATEGORY_PARAMETER_NAME, System.Data.SqlDbType.NVarChar, 10));
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(DATE_PARAMETER_NAME, System.Data.SqlDbType.Date));
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(ID_PARAMETER_NAME, System.Data.SqlDbType.Int));

            _getExpenseSumOfDateSqlCommand = new SqlCommand(_getExpenseSumOfDateSqlQuery);
            _getExpenseSumOfDateSqlCommand.Parameters.Add(new SqlParameter(DATE_PARAMETER_NAME, System.Data.SqlDbType.Date));
        }

        public async Task CreateExpenseRecordAsync(Expense expense)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
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

        public async Task<List<Expense>> GetListOfExpenseRecordsAsync()
        {
            List<Expense> queryResult = new List<Expense>();

            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
            {
                await sqlConnection.OpenAsync();

                _getAllExpenseSqlCommand.Connection = sqlConnection;
                SqlDataReader reader = await _getAllExpenseSqlCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    double cost = reader.GetDouble(1);
                    string category = reader.GetString(2);
                    DateTime date = reader.GetDateTime(3);
                    queryResult.Add(new Expense(cost, category, date, id));
                }
            }

            return queryResult;
        }

        public async Task DeleteExpenseAsync(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
            {
                await sqlConnection.OpenAsync();
                _deleteExpenseSqlCommand.Connection = sqlConnection;

                int idParametrIndex = _deleteExpenseSqlCommand.Parameters.IndexOf(ID_PARAMETER_NAME);
                _deleteExpenseSqlCommand.Parameters[idParametrIndex].Value = id;

                await _deleteExpenseSqlCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateExpenseAsync(int id, Expense expense)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
            {
                await sqlConnection.OpenAsync();
                _updateExpenseSqlCommand.Connection = sqlConnection;

                int costParametrIndex = _updateExpenseSqlCommand.Parameters.IndexOf(COST_PARAMETER_NAME);
                int categoryParametrIndex = _updateExpenseSqlCommand.Parameters.IndexOf(CATEGORY_PARAMETER_NAME);
                int dateParametrIndex = _updateExpenseSqlCommand.Parameters.IndexOf(DATE_PARAMETER_NAME);
                int idParametrIndex = _updateExpenseSqlCommand.Parameters.IndexOf(ID_PARAMETER_NAME);

                _updateExpenseSqlCommand.Parameters[costParametrIndex].Value = expense.Cost;
                _updateExpenseSqlCommand.Parameters[categoryParametrIndex].Value = expense.Category;
                _updateExpenseSqlCommand.Parameters[dateParametrIndex].Value = expense.DateTime;
                _updateExpenseSqlCommand.Parameters[idParametrIndex].Value = id;

                await _updateExpenseSqlCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<double> GetSumExpenseOfDateAsync(DateTime date) 
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
            {
                await sqlConnection.OpenAsync();
                _getExpenseSumOfDateSqlCommand.Connection = sqlConnection;
                
                int dateParametrIndex = _getExpenseSumOfDateSqlCommand.Parameters.IndexOf(DATE_PARAMETER_NAME);

                _getExpenseSumOfDateSqlCommand.Parameters[dateParametrIndex].Value = date;

                object result = await _getExpenseSumOfDateSqlCommand.ExecuteScalarAsync();

                if (result is DBNull) 
                {
                    return 0;
                }
                
                return (double)result;
            }
        }

        private SqlCommand _insertExpenseSqlCommand;
        private SqlCommand _getAllExpenseSqlCommand;
        private SqlCommand _deleteExpenseSqlCommand;
        private SqlCommand _updateExpenseSqlCommand;
        private SqlCommand _getExpenseSumOfDateSqlCommand;

        #region SQL querys
        private readonly string _insertExpenseSqlQuery = $"INSERT INTO {TABLE_NAME} ({COST_COLUMN_NAME},{CATEGORY_COLUMN_NAME},{DATE_COLUMN_NAME}) VALUES" +
            $" ({COST_PARAMETER_NAME},{CATEGORY_PARAMETER_NAME},{DATE_PARAMETER_NAME});";

        private readonly string _getAllExpenseSqlQuery = $"SELECT {TABLE_NAME}.{ID_COLUMN_NAME}, {TABLE_NAME}.{COST_COLUMN_NAME}, {TABLE_NAME}.{CATEGORY_COLUMN_NAME}," +
            $" {TABLE_NAME}.{DATE_COLUMN_NAME} FROM {TABLE_NAME}";

        private readonly string _deleteExpenseSqlQuery = $"DELETE FROM {TABLE_NAME} WHERE {TABLE_NAME}.{ID_COLUMN_NAME} = {ID_PARAMETER_NAME}";

        private readonly string _updateExpenseSqlQuery = $"UPDATE {TABLE_NAME} \n" +
            $"SET {COST_COLUMN_NAME} = {COST_PARAMETER_NAME}, {CATEGORY_COLUMN_NAME} = {CATEGORY_PARAMETER_NAME}, {DATE_COLUMN_NAME} = {DATE_PARAMETER_NAME}\n" +
            $"WHERE {ID_COLUMN_NAME} = {ID_PARAMETER_NAME}";

        private readonly string _getExpenseSumOfDateSqlQuery = $"SELECT SUM({COST_COLUMN_NAME}) FROM {TABLE_NAME} WHERE {DATE_COLUMN_NAME} = {DATE_PARAMETER_NAME};";
        #endregion

        #region Consts
        private const string COST_PARAMETER_NAME = "@cost";
        private const string CATEGORY_PARAMETER_NAME = "@category";
        private const string DATE_PARAMETER_NAME = "@date";
        private const string ID_PARAMETER_NAME = "@id";

        private const string TABLE_NAME = "[expenses]";
        private const string ID_COLUMN_NAME = "[id]";
        private const string COST_COLUMN_NAME = "[cost]";
        private const string CATEGORY_COLUMN_NAME = "[category]";
        private const string DATE_COLUMN_NAME = "[date]";
        #endregion
    }
}
