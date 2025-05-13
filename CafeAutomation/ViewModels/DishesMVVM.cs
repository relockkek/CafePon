using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.Models;
using System.Windows.Input;

namespace CafeAutomation.ViewModels
{
    internal class DishesMVVM : BaseVM
    {
        private Dishes selectedDish;
        private ObservableCollection<Dishes> dishes = new();
        private readonly string? selectedCategory;

        public ObservableCollection<Dishes> Dishes
        {
            get => dishes;
            set
            {
                dishes = value;
                Signal();
            }
        }

        public Dishes SelectedDish
        {
            get => selectedDish;
            set
            {
                selectedDish = value;
                Signal();
            }
        }

        public ObservableCollection<string> Categories { get; set; } = new()
        {
            "Горячие блюда",
            "Напитки",
            "Закуски",
            "Десерты",
            "Салаты",
            "Завтраки"
        };

        public CommandMvvm AddDish { get; }
        public CommandMvvm UpdateDish { get; }
        public CommandMvvm RemoveDish { get; }

        public CommandMvvm NavigateToCategoryCommand { get; }

        // Конструктор для MenuPage
        public DishesMVVM()
        {
            LoadDataAsync();
            NavigateToCategoryCommand = new CommandMvvm(
                (categoryObj) =>
                {
                    if (categoryObj is string category)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var mainWindow = Application.Current.MainWindow as MainWindow;
                            mainWindow?.MainFrame.Navigate(new Views.DishesPage(category));
                        });
                    }
                },
                _ => true // вот это исправлено
            );


        }

        // Конструктор для конкретной категории
        public DishesMVVM(string category)
        {
            selectedCategory = category;
            LoadDataAsync(category);

            AddDish = new CommandMvvm((_) =>
            {
                var newDish = new Dishes
                {
                    Name = "Новое блюдо",
                    Price = 0,
                    Category = category
                };

                if (DishesDB.GetDb().Insert(newDish))
                {
                    LoadDataAsync(category);
                    SelectedDish = newDish;
                }
            }, (_) => true);

            UpdateDish = new CommandMvvm(async (_) =>
            {
                if (SelectedDish != null && await DishesDB.GetDb().UpdateAsync(SelectedDish))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync(category);
                }
            }, (_) => SelectedDish != null);

            RemoveDish = new CommandMvvm(async (_) =>
            {
                if (SelectedDish != null && await DishesDB.GetDb().DeleteAsync(SelectedDish))
                {
                    MessageBox.Show("Удалено");
                    await LoadDataAsync(category);
                }
            }, (_) => SelectedDish != null);
        }


        private async Task LoadDataAsync()
        {
            var data = await DishesDB.GetDb().SelectAllAsync();
            Dishes = new ObservableCollection<Dishes>(data);
        }

        private async Task LoadDataAsync(string category)
        {
            var data = await DishesDB.GetDb().SelectAllAsync();
            Dishes = new ObservableCollection<Dishes>(
                data.Where(d => d.Category == category));
        }

        public async Task RefreshData(string category)
        {
            await LoadDataAsync(category);
        }

    }
}
