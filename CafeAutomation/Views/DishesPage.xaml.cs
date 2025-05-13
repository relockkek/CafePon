using CafeAutomation.DB;
using CafeAutomation.Models;
using CafeAutomation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CafeAutomation.Views
{
    public partial class DishesPage : Page
    {
        private readonly string category;

        public DishesPage(string category)
        {
            InitializeComponent();
            this.category = category;
            DataContext = new DishesMVVM(category);
            CategoryTitle.Text = category;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }

        private async void AddDish_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddDishDialog(category); // category — поле класса
            if (dialog.ShowDialog() == true)
            {
                var newDish = dialog.ResultDish;

                if (DishesDB.GetDb().Insert(newDish))
                {
                    await ((DishesMVVM)DataContext).RefreshData(category); // обновим список
                    ((DishesMVVM)DataContext).SelectedDish = newDish;      // выделим новое блюдо
                }
            }
        }


        private async void EditDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Dishes dish)
            {
                var editDialog = new EditDishDialog(dish);
                if (editDialog.ShowDialog() == true)
                {
                    await DishesDB.GetDb().UpdateAsync(editDialog.Dish);
                    await ((DishesMVVM)DataContext).RefreshData(category);
                    MessageBox.Show("Блюдо обновлено");
                }
            }
        }

        private async void DeleteDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Dishes dish)
            {
                var result = MessageBox.Show($"Удалить блюдо \"{dish.Name}\"?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await DishesDB.GetDb().DeleteAsync(dish);
                    await ((DishesMVVM)DataContext).RefreshData(category);
                    MessageBox.Show("Блюдо удалено");
                }
            }
        }
    }
}
