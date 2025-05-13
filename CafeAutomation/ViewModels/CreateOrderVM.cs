using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CafeAutomation.Models;
using System.Windows;

namespace CafeAutomation.ViewModels
{
    public class CreateOrderVM : BaseVM
    {
        public ObservableCollection<Dishes> AvailableDishes { get; set; }
        public ObservableCollection<DishForOrder> SelectedDishes { get; set; } = new();

        public DishForOrder SelectedDishForOrder { get; set; }
        public Dishes SelectedAvailableDish { get; set; }

        public decimal OrderTotal => SelectedDishes.Sum(x => x.Total);

        public CommandMvvm AddToOrder { get; }
        public CommandMvvm RemoveFromOrder { get; }
        public CommandMvvm ConfirmOrder { get; }

        public CreateOrderVM()
        {
            AvailableDishes = new ObservableCollection<Dishes>(DishesDB.GetDb().SelectAll());

            AddToOrder = new CommandMvvm((_) =>
            {
                if (SelectedAvailableDish == null) return;

                var existing = SelectedDishes.FirstOrDefault(x => x.Dish.ID == SelectedAvailableDish.ID);
                if (existing != null)
                    existing.Quantity++;
                else
                    SelectedDishes.Add(new DishForOrder { Dish = SelectedAvailableDish, Quantity = 1 });

                Signal(nameof(OrderTotal));
            });

            RemoveFromOrder = new CommandMvvm((_) =>
            {
                if (SelectedDishForOrder != null)
                {
                    SelectedDishes.Remove(SelectedDishForOrder);
                    Signal(nameof(OrderTotal));
                }
            });

            ConfirmOrder = new CommandMvvm((_) =>
            {
                var order = new Orders
                {
                    EmployeeID = 1,
                    TableNumber = 1,
                    OrderDate = DateTime.Now,
                    StatusID = 1,
                    TotalAmount = OrderTotal
                };

                if (OrdersDB.GetDb().Insert(order))
                {
                    foreach (var dish in SelectedDishes)
                    {
                        OrderItemsDB.GetDb().Insert(new OrderItems
                        {
                            OrderID = order.ID,
                            DishID = dish.Dish.ID,
                            Amount = dish.Quantity,
                            PriceAtOrderTime = dish.Dish.Price
                        });
                    }

                    MessageBox.Show("Заказ оформлен!");
                }
            });
        }
    }
}

