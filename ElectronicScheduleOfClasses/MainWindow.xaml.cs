using System;
using System.Windows;
using System.IO;

using CostAccounting.Models;

namespace CostAccounting
{
    public partial class MainWindow : Window
    {
        private ExpenseTableOperationsFacade _dbOperations;
        public MainWindow()
        {
            InitializeComponent();

            _dbOperations = new ExpenseTableOperationsFacade();
            new Action(async () =>
            {
                try
                {
                    expensesDataGrid.ItemsSource = await _dbOperations.GetListOfExpenseRecordsAsync();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }).Invoke();
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double cost = Convert.ToDouble(costTextBox.Text);
                string category = categoryTextBox.Text;
                DateTime date = datePicker.SelectedDate ?? DateTime.Now;

                await _dbOperations.CreateExpenseRecordAsync(new Expense(cost, category, date));

                expensesDataGrid.ItemsSource = await _dbOperations.GetListOfExpenseRecordsAsync();
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

            try
            {
                foreach (var item in expensesDataGrid.SelectedItems)
                {
                    await _dbOperations.DeleteExpenseAsync(((Expense)item).Id);
                }
                expensesDataGrid.ItemsSource = await _dbOperations.GetListOfExpenseRecordsAsync();
                expensesDataGrid.Items.Refresh();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (expensesDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select one lines", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); ;
                return;
            }

            try
            {
                double cost = Convert.ToDouble(costTextBox.Text);
                string category = categoryTextBox.Text;
                DateTime date = datePicker.SelectedDate ?? DateTime.Now;
                int id = ((Expense)expensesDataGrid.SelectedItem).Id;

                await _dbOperations.UpdateExpenseAsync(id, new Expense(cost, category, date));

                expensesDataGrid.ItemsSource = await _dbOperations.GetListOfExpenseRecordsAsync();
                expensesDataGrid.Items.Refresh();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GenerateScilabScriptMenuItem_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter fileStream = new StreamWriter($"{AppDomain.CurrentDomain.BaseDirectory}HistogramOfExpensesForTheLastMonth.sce", append:false);
            
            await fileStream.WriteLineAsync("clf();");
            await fileStream.WriteLineAsync($"xtitle(\"Расходы по дням за последний месяц\"," +
                $"\"c {DateTime.Now.AddMonths(-1).ToString("yyyy.MM.dd")} по {DateTime.Now.ToString("yyyy.MM.dd")}\"," +
                $"\"Сумма расходов\");");

            await fileStream.WriteAsync("y=[");

            int dayCount = 0;
            for (DateTime i = DateTime.Now.AddMonths(-1); i <= DateTime.Now; i = i.AddDays(1))
            {
                await fileStream.WriteAsync(" " + await _dbOperations.GetSumExpenseOfDateAsync(i));
                dayCount++;
            }
            await fileStream.WriteAsync("];\n");
            
            await fileStream.WriteAsync("x=[");
            for (int i = 1; i <= dayCount; i++)
            {
                await fileStream.WriteAsync(" " + i);
            }
            await fileStream.WriteAsync("];\n");

            await fileStream.WriteLineAsync("bar(x,y,1);");

            fileStream.Close();
        }
    }
}
