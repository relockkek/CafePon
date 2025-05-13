using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;
using CafeAutomation.Views;

namespace CafeAutomation.ViewModels
{
    internal class OrdersMVVM : BaseVM
    {
        private Orders selectedOrder;
        private ObservableCollection<Orders> orders = new();
        public OrderItemsMVVM OrderItemsVM { get; } = new OrderItemsMVVM();


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

            AddOrder = new CommandMvvm((_) =>
            {
                var dialog = new CreateOrderDialog(); // имя окна
                dialog.ShowDialog();                  // открыть окно

                LoadDataAsync(); // обновить список заказов после закрытия окна
            });


            UpdateOrder = new CommandMvvm(async (_) =>
            {
                if (SelectedOrder != null && await OrdersDB.GetDb().UpdateAsync(SelectedOrder))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedOrder != null);

            RemoveOrder = new CommandMvvm(async (_) =>
            {
                if (SelectedOrder != null && await OrdersDB.GetDb().DeleteAsync(SelectedOrder))
                {
                    MessageBox.Show("Удалён заказ");
                    await LoadDataAsync();
                }
            }, (_) => SelectedOrder != null);
        }

        private async Task LoadDataAsync()
        {
            var data = await OrdersDB.GetDb().SelectAllAsync();
            Orders = new ObservableCollection<Orders>(data);
        }
    }
}