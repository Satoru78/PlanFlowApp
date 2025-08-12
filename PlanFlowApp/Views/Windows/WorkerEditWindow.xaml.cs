using PlanFlowApp.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PlanFlowApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkerEditWindow.xaml
    /// </summary>
    public partial class WorkerEditWindow : Window
    {

        private WorkerViewModel _vm;
        public WorkerEditWindow(WorkerViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = _vm;
        }

        private void SaveWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_vm.FirstName) || string.IsNullOrWhiteSpace(_vm.LastName))
            {
                MessageBox.Show("Имя и фамилия обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}

