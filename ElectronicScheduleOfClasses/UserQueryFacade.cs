﻿using System;
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
        private const string ID_PARAMETER_NAME = "@id";

        private readonly string _insertExpenseSqlQuery = "INSERT INTO [expenses] ([cost],[category],[date]) VALUES" +
            $" ({COST_PARAMETER_NAME},{CATEGORY_PARAMETER_NAME},{DATE_PARAMETER_NAME});";
        private readonly string _getAllExpenseSqlQuery = "SELECT [expenses].[id], [expenses].[cost], [expenses].[category], [expenses].[date] " +
            "FROM [expenses]";
        private readonly string _deleteExpenseSqlQuery = $"DELETE FROM [expenses] WHERE [expenses].[id] = {ID_PARAMETER_NAME}";
        private readonly string _updateExpenseSqlQuery = $"UPDATE [expenses] \n" +
            $"SET [cost] = {COST_PARAMETER_NAME}, [category] = {CATEGORY_PARAMETER_NAME}, [date] = {DATE_PARAMETER_NAME}\n" +
            $"WHERE [id] = {ID_PARAMETER_NAME}";

        private SqlCommand _insertExpenseSqlCommand;
        private SqlCommand _getAllExpenseSqlCommand;
        private SqlCommand _deleteExpenseSqlCommand;
        private SqlCommand _updateExpenseSqlCommand;
        public UserQueryFacade() 
        {
            _getAllExpenseSqlCommand = new SqlCommand(_getAllExpenseSqlQuery);

            _insertExpenseSqlCommand = new SqlCommand(_insertExpenseSqlQuery);
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(COST_PARAMETER_NAME,System.Data.SqlDbType.Float));
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(CATEGORY_PARAMETER_NAME, System.Data.SqlDbType.NVarChar, 10));
            _insertExpenseSqlCommand.Parameters.Add(new SqlParameter(DATE_PARAMETER_NAME, System.Data.SqlDbType.Date));

            _deleteExpenseSqlCommand = new SqlCommand(_deleteExpenseSqlQuery);
            _deleteExpenseSqlCommand.Parameters.Add(new SqlParameter(ID_PARAMETER_NAME, System.Data.SqlDbType.Int));

            _updateExpenseSqlCommand = new SqlCommand(_updateExpenseSqlQuery);
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(COST_PARAMETER_NAME, System.Data.SqlDbType.Float));
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(CATEGORY_PARAMETER_NAME, System.Data.SqlDbType.NVarChar, 10));
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(DATE_PARAMETER_NAME, System.Data.SqlDbType.Date));
            _updateExpenseSqlCommand.Parameters.Add(new SqlParameter(ID_PARAMETER_NAME, System.Data.SqlDbType.Int));
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
                    queryResult.Add(new Expense(id, cost, category, date));
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
    }
}
