using System.Windows;
using CafeAutomation.ViewModels;
using CafeAutomation.Views;

namespace CafeAutomation
{
        public partial class MainWindow : Window
        {
            public MainWindow()
            {
                InitializeComponent();
                DataContext = new HomeMVVM(); 
            }
        private void NavigateToHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Home());
        }

        private void NavigateToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MenuPage());
        }

        private void NavigateToOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void NavigateToReservations_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReservationsPage());
        }

        private void NavigateToEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}