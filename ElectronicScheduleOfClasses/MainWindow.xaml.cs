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
            new Action(async () =>
            {
                try
                {
                    expensesDataGrid.ItemsSource = await _queryFacade.GetListOfExpenseRecordsAsync();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }).Invoke();
        }

        private List<Expense> _expenses;

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

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (expensesDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the lines", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try{
                foreach (var item in expensesDataGrid.SelectedItems)
                {
                    await _queryFacade.DeleteExpenseAsync(((Expense)item).Id);
                }

                expensesDataGrid.ItemsSource = await _queryFacade.GetListOfExpenseRecordsAsync();
                expensesDataGrid.Items.Refresh();
            }catch(Exception exception) 
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
