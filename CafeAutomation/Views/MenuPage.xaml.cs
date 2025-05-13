using System.Windows;
using System.Windows.Controls;
using CafeAutomation.ViewModels;
namespace CafeAutomation.Views
{

    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
            DataContext = new DishesMVVM();
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            // Гибкая проверка — работает даже если sender НЕ Button
            if (sender is FrameworkElement fe && fe.DataContext is string category)
            {
                NavigationService.Navigate(new DishesPage(category));
            }
            else
            {
                MessageBox.Show("Ошибка: категория не определена.");
            }
        }
    }
}

