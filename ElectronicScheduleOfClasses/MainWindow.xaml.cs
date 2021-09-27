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
            new Action (async () =>
            {
                expensesDataGrid.ItemsSource = await _queryFacade.GetListOfExpenseRecordsAsync();
            }).Invoke();
        }

        private List<Expense> _expenses;

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
                expensesDataGrid.ItemsSource = await _queryFacade.GetListOfExpenseRecordsAsync();
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
