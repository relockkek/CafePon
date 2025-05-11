using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    internal class OrdersMVVM : BaseVM
    {
        private Orders selectedOrder;
        private ObservableCollection<Orders> orders = new();

        public ObservableCollection<Orders> Orders
        {
            get => orders;
            set
            {
                orders = value;
                Signal();
            }
        }

        public Orders SelectedOrder
        {
            get => selectedOrder;
            set
            {
                selectedOrder = value;
                Signal();
            }
        }

        public CommandMvvm AddOrder { get; }
        public CommandMvvm UpdateOrder { get; }
        public CommandMvvm RemoveOrder { get; }

        public OrdersMVVM()
        {
            LoadDataAsync(); // Асинхронная загрузка данных

            AddOrder = new CommandMvvm(() =>
            {
                var order = new Orders
                {
                    EmployeeID = 1,
                    TableNumber = 1,
                    OrderDate = DateTime.Now,
                    TotalAmount = 0,
                    StatusID = 1
                };

                if (OrdersDB.GetDb().Insert(order))
                {
                    LoadDataAsync();
                    SelectedOrder = order;
                }
            }, () => true);

            UpdateOrder = new CommandMvvm(async () =>
            {
                if (SelectedOrder != null && await OrdersDB.GetDb().UpdateAsync(SelectedOrder))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, () => SelectedOrder != null);

            RemoveOrder = new CommandMvvm(async () =>
            {
                if (SelectedOrder != null && await OrdersDB.GetDb().DeleteAsync(SelectedOrder))
                {
                    MessageBox.Show("Удалён заказ");
                    await LoadDataAsync();
                }
            }, () => SelectedOrder != null);
        }

        private async Task LoadDataAsync()
        {
            var data = await OrdersDB.GetDb().SelectAllAsync();
            Orders = new ObservableCollection<Orders>(data);
        }
    }
}