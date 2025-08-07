using PlanFlowApp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для EditOperationWindow.xaml
    /// </summary>
    public partial class EditOperationWindow : Window
    {
        private EditOperationViewModel _viewModel;
        private bool _isSaved = false;

        public EditOperationWindow(OperationAssignmentViewModel operation)
        {
            InitializeComponent();

            _viewModel = new EditOperationViewModel(operation);
            DataContext = _viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ApplyChanges(); // применить к модели
            _isSaved = true;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_isSaved)
            {
                var result = MessageBox.Show(
                    "Вы действительно хотите закрыть окно, не сохранив изменения?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }
    }
}
