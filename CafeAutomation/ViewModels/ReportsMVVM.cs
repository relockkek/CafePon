using System;
using System.Collections.Generic;
using CafeAutomation.DB;
using CafeAutomation.Models;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace CafeAutomation.ViewModels
{
    internal class ReportsMVVM : BaseVM
    {
        private string revenueToday;
        private string popularDish;
        private string tablesLoad;

        public string RevenueToday
        {
            get => revenueToday;
            set
            {
                revenueToday = value;
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

        public string TablesLoad
        {
            get => tablesLoad;
            set
            {
                tablesLoad = value;
                Signal();
            }
        }

        public CommandMvvm LoadReport { get; }

        public ReportsMVVM()
        {
            LoadReport = new CommandMvvm(async (_) =>
            {
                try
                {
                    // Выручка за неделю
                    var start = DateTime.Now.Date.AddDays(-7);
                    var end = DateTime.Now.Date;
                    decimal totalRevenue = await OrdersDB.GetDb().GetTotalRevenueAsync(start, end); // Используем async
                    RevenueToday = $"Выручка: {totalRevenue:C}";

                    // Популярное блюдо
                    var dish = await DishesDB.GetDb().GetMostPopularDishAsync(); // Используем async
                    PopularDish = $"Популярное блюдо: {dish?.Name ?? "Нет данных"}";

                    // Загрузка столов
                    var tables = await TablesDB.GetDb().SelectAllAsync(); // Используем async
                    int activeTables = tables.Count(t => t.IsActive);
                    int totalTables = tables.Count;
                    TablesLoad = $"Столы: {activeTables}/{totalTables} занято";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки отчёта: " + ex.Message);
                }
            }, (_) => true);
        }
    }
}