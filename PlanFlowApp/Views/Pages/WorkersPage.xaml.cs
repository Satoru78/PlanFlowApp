using PlanFlowApp.Model;
using PlanFlowApp.ViewModel;
using PlanFlowApp.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlanFlowApp.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для WorkersPage.xaml
    /// </summary>
    public partial class WorkersPage : Page
    {
        private ObservableCollection<WorkerViewModel> _workers = new ObservableCollection<WorkerViewModel>();
        public WorkersPage()
        {
            InitializeComponent();
            LoadWorkers();
        }
        private void LoadWorkers()
        {

            using (var ctx = new dbPlanFlowEntities())
            {
                // ...весь код, который был после using var...
                var list = ctx.Workers
                    .Select(w => new WorkerViewModel { Id = w.Id, FirstName = w.FirstName, LastName = w.LastName })
                    .ToList();

                _workers = new ObservableCollection<WorkerViewModel>(list);
                WorkersDataGrid.ItemsSource = _workers;
            }
        }

        private void WorkerSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = WorkerSearchBox.Text?.Trim().ToLower() ?? "";
            if (string.IsNullOrEmpty(q))
            {
                WorkersDataGrid.ItemsSource = _workers;
                return;
            }
            WorkersDataGrid.ItemsSource = _workers.Where(w =>
                (w.FirstName ?? "").ToLower().Contains(q) ||
                (w.LastName ?? "").ToLower().Contains(q) ||
                (w.FullName ?? "").ToLower().Contains(q)
            ).ToList();
        }

        private void AddWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = new WorkerViewModel();
            var win = new WorkerEditWindow(vm) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                using (var ctx = new dbPlanFlowEntities())
                {
                    // ...весь код, который был после using var...
                    var entity = new Workers { FirstName = vm.FirstName, LastName = vm.LastName };
                    ctx.Workers.Add(entity);
                    ctx.SaveChanges();
                    vm.Id = entity.Id;
                    _workers.Add(vm);
                }
            }
        }

        private void EditWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersDataGrid.SelectedItem is WorkerViewModel selected)
            {
                // Клонируем значение, чтобы отмена не меняла список
                var clone = new WorkerViewModel { Id = selected.Id, FirstName = selected.FirstName, LastName = selected.LastName };
                var win = new WorkerEditWindow(clone) { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true)
                {
                    using (var ctx = new dbPlanFlowEntities())
                    {
                        // ...весь код, который был после using var...
                        var ent = ctx.Workers.Find(clone.Id);
                        if (ent != null)
                        {
                            ent.FirstName = clone.FirstName;
                            ent.LastName = clone.LastName;
                            ctx.SaveChanges();

                            // Обновляем в списке
                            selected.FirstName = clone.FirstName;
                            selected.LastName = clone.LastName;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersDataGrid.SelectedItem is WorkerViewModel selected)
            {
                var res = MessageBox.Show($"Удалить сотрудника {selected.FullName}?", "Подтвердите", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes) return;

                using (var ctx = new dbPlanFlowEntities())
                {
                    // ...весь код, который был после using var...
                    var ent = ctx.Workers.Find(selected.Id);
                    if (ent == null) return;

                    // Обнуляем AssignedWorkerId у связанных операций
                    var ops = ctx.Operations.Where(o => o.AssignedWorkerId == ent.Id).ToList();
                    foreach (var op in ops) op.AssignedWorkerId = null;

                    ctx.Workers.Remove(ent);
                    ctx.SaveChanges();

                    _workers.Remove(selected);
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
