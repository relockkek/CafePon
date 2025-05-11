using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    internal class DishesMVVM : BaseVM
    {
        private Dishes selectedDish;
        private ObservableCollection<Dishes> dishes = new();

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

        public CommandMvvm AddDish { get; }
        public CommandMvvm UpdateDish { get; }
        public CommandMvvm RemoveDish { get; }

        public DishesMVVM()
        {
            LoadDataAsync();

            AddDish = new CommandMvvm(() =>
            {
                var newDish = new Dishes
                {
                    Name = "Новое блюдо",
                    Price = 0,
                    Category = "Не задано"
                };

                if (DishesDB.GetDb().Insert(newDish))
                {
                    LoadDataAsync();
                    SelectedDish = newDish;
                }
            }, () => true);

            UpdateDish = new CommandMvvm(async () =>
            {
                if (SelectedDish != null && await DishesDB.GetDb().UpdateAsync(SelectedDish))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, () => SelectedDish != null);

            RemoveDish = new CommandMvvm(async () =>
            {
                if (SelectedDish != null && await DishesDB.GetDb().DeleteAsync(SelectedDish))
                {
                    MessageBox.Show("Удалено");
                    await LoadDataAsync();
                }
            }, () => SelectedDish != null);
        }

        private async Task LoadDataAsync()
        {
            var data = await DishesDB.GetDb().SelectAllAsync();
            Dishes = new ObservableCollection<Dishes>(data);
        }
    }
}