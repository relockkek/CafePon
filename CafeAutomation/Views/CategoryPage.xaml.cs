using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CafeAutomation.Models;

namespace CafeAutomation.Views
{
    public partial class CategoryPage : Page
    {
        public string SelectedCategory { get; set; }
        public ObservableCollection<Dishes> Dishes { get; set; }

        public CategoryPage(string categoryName)
        {
            InitializeComponent();
            DataContext = this;

            SelectedCategory = categoryName;
            LoadDishesAsync();
        }

        private async void LoadDishesAsync()
        {
            var allDishes = await DishesDB.GetDb().SelectAllAsync();
            Dishes = new ObservableCollection<Dishes>(
                allDishes.Where(d => d.Category == SelectedCategory && d.IsAvailable));
        }
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}