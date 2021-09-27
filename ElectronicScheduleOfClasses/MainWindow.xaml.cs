using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using CostAccounting.Models;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace CostAccounting
{
    public partial class MainWindow : Window
    {
        private UserQueryFacade _queryFacade;
        public MainWindow()
        {
            InitializeComponent();
            _queryFacade = new UserQueryFacade();
            _expenses = new List<Expense>();
            GetAllExpensesFromBD();
        }

        private List<Expense> _expenses;

        private async void GetAllExpensesFromBD()
        {
            using (SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
            {
                await _sqlConnection.OpenAsync();

                SqlCommand getAllExpensesSQLCommand = new SqlCommand(GET_ALL_EXPENSES_SQL_QUERY, _sqlConnection);
                SqlDataReader reader = await getAllExpensesSQLCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    double cost = reader.GetDouble(1);
                    string category = reader.GetString(2);
                    DateTime date = reader.GetDateTime(3);
                    _expenses.Add(new Expense(cost, category, date));
                }
            }

            expensesDataGrid.ItemsSource = _expenses;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalMSSQLDATABASE"].ConnectionString))
            {
                 _sqlConnection.OpenAsync();

                SqlCommand deleteAllValuesOfTableExpensesSQLCommand = new SqlCommand(DELETE_ALL_VALUES_OF_TABLE_EXPENSES_SQL_QUERY, _sqlConnection);
                 deleteAllValuesOfTableExpensesSQLCommand.ExecuteNonQueryAsync();

                StringBuilder insertAllExspensesIntoExpensesTableSQLQuery = new StringBuilder();
                foreach (var item in _expenses)
                {
                    insertAllExspensesIntoExpensesTableSQLQuery.AppendLine(InsertExpenseSQLQuery(item));
                }

                SqlCommand insertAllExpensesIntoExpensesTableCommand
                    = new SqlCommand(insertAllExspensesIntoExpensesTableSQLQuery.ToString(), _sqlConnection);
                insertAllExpensesIntoExpensesTableCommand.ExecuteNonQueryAsync();
            }
        }

        private const string GET_ALL_EXPENSES_SQL_QUERY = "SELECT * FROM [expenses]";

        private const string DELETE_ALL_VALUES_OF_TABLE_EXPENSES_SQL_QUERY = "DELETE [expenses]";

        private string InsertExpenseSQLQuery(Expense expense) => $"INSERT INTO [expenses] ([cost],[category],[date]) VALUES" +
            $" ({expense.Cost},'{expense.Category}','{expense.DateTime.ToString("yyyy-MM-dd")}');";

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double cost = Convert.ToDouble(costTextBox.Text);
                string category = categoryTextBox.Text;
                DateTime date = datePicker.SelectedDate ?? DateTime.Now;

                Expense createdExpense = new Expense(cost, category, date);

                await _queryFacade.CreateExpenseRecordAsync(createdExpense);

                _expenses.Add(createdExpense);
                expensesDataGrid.Items.Refresh();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (expensesDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберете строки!");
                return;
            }

            foreach (var item in expensesDataGrid.SelectedItems)
            {
                _expenses.Remove((Expense)item);
            }

            expensesDataGrid.Items.Refresh();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (expensesDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Выберете 1 строку!");
                return;
            }
            Expense selectedItem = (Expense)expensesDataGrid.SelectedItem;
            selectedItem.Category = categoryTextBox.Text;
            selectedItem.Cost = Convert.ToDouble(costTextBox.Text);
            selectedItem.DateTime = datePicker.SelectedDate ?? DateTime.Now;
            expensesDataGrid.Items.Refresh();
        }
    }
}
