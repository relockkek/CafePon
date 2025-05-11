using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.Models;
using CafeAutomation.ViewModels;

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

        public HomeMVVM()
        {
            LoadReport = new CommandMvvm(async () =>
            {
                await LoadDataAsync();
            }, () => true);

            // Автозагрузка при старте
            Task.Run(LoadDataAsync);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var start = DateTime.Now.Date;
                var end = DateTime.Now.Date.AddDays(1);

                // Выручка за день
                decimal totalRevenue = await OrdersDB.GetDb().GetTotalRevenueAsync(start, end);
                TodayRevenue = $"Выручка: {totalRevenue:C}";

                // Популярное блюдо
                var dish = await DishesDB.GetDb().GetMostPopularDishAsync();
                PopularDish = dish?.Name ?? "Нет данных";

                // Состояние столов
                var tables = await TablesDB.GetDb().SelectAllAsync();
                int activeTables = tables.Count(t => t.IsActive);
                TablesStatus = $"{activeTables} из {tables.Count} столов занято";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки главной страницы: " + ex.Message);
            }
        }
    }
}