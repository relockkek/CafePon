using System.Windows;
using CafeAutomation.Models;

namespace CafeAutomation.Views
{
    public partial class EditDishDialog : Window
    {
        public Dishes Dish { get; }

        public EditDishDialog(Dishes dish)
        {
            InitializeComponent();

            // Создаем копию блюда, чтобы не редактировать оригинал до сохранения
            Dish = new Dishes
            {
                ID = dish.ID,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Category = dish.Category,
                IsAvailable = dish.IsAvailable
            };

            DataContext = this; // Главное — DataContext должен быть один
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
