using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using CafeAutomation.Models;

namespace CafeAutomation.Views
{
    public partial class AddDishDialog : Window
    {
        public Dishes ResultDish { get; private set; }
        private string selectedImagePath;

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
                string sourcePath = dialog.FileName;
                string fileName = Path.GetFileName(sourcePath);
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                // Создать папку, если её нет
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                string destPath = Path.Combine(imagesFolder, fileName);

                try
                {
                    File.Copy(sourcePath, destPath, true); // перезапись
                    selectedImagePath = Path.Combine("Images", fileName); // относительный путь
                    PreviewImage.Source = new BitmapImage(new Uri(destPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка копирования изображения: " + ex.Message);
                }
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
                ImageData = ImageFromBytes,
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