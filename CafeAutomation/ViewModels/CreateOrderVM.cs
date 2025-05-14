using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using CafeAutomation.DB;

namespace CafeAutomation.ViewModels
{
    public class CreateOrderVM : BaseVM
    {
        public ObservableCollection<Dishes> AvailableDishes { get; set; } = new();
        public ObservableCollection<DishForOrder> SelectedDishes { get; set; } = new();

        private List<Dishes> allDishes;

        public DishForOrder SelectedDishForOrder { get; set; }
        public Dishes SelectedAvailableDish { get; set; }

        public ObservableCollection<string> Categories { get; set; } = new();
        private string selectedCategory;
        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                FilterDishes();
                Signal();
            }
        }

        public ObservableCollection<int> TableNumbers { get; set; } =
            new ObservableCollection<int>(Enumerable.Range(1, 20));
        public int SelectedTable { get; set; } = 1;

        public string OrderNotes { get; set; } = "";

        public decimal OrderTotal => SelectedDishes.Sum(x => x.Total);

        public CommandMvvm AddToOrder { get; }
        public CommandMvvm RemoveFromOrder { get; }
        public CommandMvvm ConfirmOrder { get; }

        public CommandMvvm IncreaseQuantity { get; }
        public CommandMvvm DecreaseQuantity { get; }

        public CreateOrderVM()
        {
            LoadDishes();

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
                    TableNumber = SelectedTable,
                    OrderDate = DateTime.Now,
                    StatusID = 1,
                    TotalAmount = OrderTotal
                    // При желании: OrderNotes = OrderNotes
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

                    MessageBox.Show($"Заказ оформлен!\nСтол: {SelectedTable}\nДетали: {OrderNotes}");
                }
            });

            IncreaseQuantity = new CommandMvvm((obj) =>
            {
                if (obj is DishForOrder item)
                {
                    item.Quantity++;
                    Signal(nameof(OrderTotal));
                }
            });

            DecreaseQuantity = new CommandMvvm((obj) =>
            {
            if (obj is DishForOrder item)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
                else
                    SelectedDishes.Remove(item);
                    Signal(nameof(OrderTotal));
                }
            });
        }

        private async void LoadDishes()
        {
            allDishes = await DishesDB.GetDb().SelectAllAsync();
            Categories = new ObservableCollection<string>(allDishes.Select(d => d.Category).Distinct());
            SelectedCategory = Categories.FirstOrDefault();
            Signal(nameof(Categories));
        }

        private void FilterDishes()
        {
            if (string.IsNullOrEmpty(SelectedCategory)) return;
            var filtered = allDishes.Where(d => d.Category == SelectedCategory).ToList();
            AvailableDishes = new ObservableCollection<Dishes>(filtered);
            Signal(nameof(AvailableDishes));
        }
    }
}