using System.Windows;
using Microsoft.Win32;
using CafeAutomation.Models;
using System.IO;
using System.Windows.Media.Imaging;

namespace CafeAutomation.Views
{
    public partial class AddDishDialog : Window
    {
        public Dishes ResultDish { get; private set; }

        public AddDishDialog(string category)
        {
            InitializeComponent();

            CategoryBox.ItemsSource = new[] { "Горячие блюда", "Напитки", "Закуски", "Десерты", "Салаты", "Завтраки" };
            CategoryBox.SelectedItem = category;
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения (*.png;*.jpg)|*.png;*.jpg";

            if (dialog.ShowDialog() == true)
            {
                ImagePathBox.Text = dialog.FileName;

                // Предпросмотр
                BitmapImage image = new BitmapImage(new Uri(dialog.FileName));
                PreviewImage.Source = image;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название блюда.");
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out var price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            if (CategoryBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.");
                return;
            }

            ResultDish = new Dishes
            {
                Name = NameBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim(),
                Price = price,
                Category = CategoryBox.SelectedItem.ToString(),
                ImagePath = ImagePathBox.Text,
                IsAvailable = true
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
