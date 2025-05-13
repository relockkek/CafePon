using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CafeAutomation.DB;
using CafeAutomation.Models;
using CafeAutomation.ViewModels;
using CafeAutomation.Views;

namespace CafeAutomation.ViewModels
{
    internal class HomeMVVM : BaseVM
    {
        private string todayRevenue;
        private string popularDish;
        private string tablesStatus;

        public string TodayRevenue
        {
            get => todayRevenue;
            set
            {
                todayRevenue = value;
                Signal();
            }
        }

        public string PopularDish
        {
            get => popularDish;
            set
            {
                popularDish = value;
                Signal();
            }
        }

        public string TablesStatus
        {
            get => tablesStatus;
            set
            {
                tablesStatus = value;
                Signal();
            }
        }

        public CommandMvvm LoadReport { get; }

        // Новая команда навигации к категории
        public ICommand NavigateToCategoryCommand { get; }

        public HomeMVVM()
        {
            LoadReport = new CommandMvvm(async (_) =>
            {
                await LoadDataAsync();
            }, (_) => true);

            // Инициализация команды перехода
            NavigateToCategoryCommand = new CommandMvvm(_ => ExecuteNavigateToCategory(), _ => true);


            // Автозагрузка данных
            Task.Run(LoadDataAsync);
        }

        private void ExecuteNavigateToCategory()
        {
            var param = "Горячие блюда"; // Пример параметра (можно передать из XAML)
            OnNavigateToCategory(param);
        }

        private void OnNavigateToCategory(object param)
        {
            if (param is string categoryName)
            {
                var categoryPage = new CategoryPage(categoryName);
                var mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = categoryPage;
                }
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var start = DateTime.Now.Date;
                var end = DateTime.Now.Date.AddDays(1);

                decimal totalRevenue = await OrdersDB.GetDb().GetTotalRevenueAsync(start, end);
                TodayRevenue = $"Выручка за день: {totalRevenue:C}";

                var dish = await DishesDB.GetDb().GetMostPopularDishAsync();
                PopularDish = $"Популярное блюдо: {dish?.Name ?? "Нет данных"}";

                var tables = await TablesDB.GetDb().SelectAllAsync();
                int activeTables = tables.Count(t => t.IsActive);
                TablesStatus = $"{activeTables} из {tables.Count} столов занято";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }
    }
}