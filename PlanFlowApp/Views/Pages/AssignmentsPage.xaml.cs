using PlanFlowApp.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlanFlowApp.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AssignmentsPage.xaml
    /// </summary>
    public partial class AssignmentsPage : Page
    {
        private dbPlanFlowEntities _context = new dbPlanFlowEntities();
        public List<Workers> Workers { get; set; }
        public AssignmentsPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            Workers = _context.Workers.ToList(); // FullName уже доступен

            var operations = _context.Operations
                .Select(op => new OperationAssignmentViewModel
                {
                    OperationId = op.Id,
                    DetailCode = op.Details.Code,
                    DetailTitle = op.Details.Title,
                    TypeOperationTitle = op.TypeOperation.Title,
                    AssignedWorkerId = op.AssignedWorkerId,
                    AssignedWorkerName = op.Workers != null
                        ? (op.Workers.FirstName + " " + op.Workers.LastName)
                        : ""
                }).ToList();

            AssignmentGrid.DataContext = operations;
        }

        private void SaveAssignmentsBtn_Click(object sender, RoutedEventArgs e)
        {
            var items = AssignmentGrid.ItemsSource as List<OperationAssignmentViewModel>;

            foreach (var item in items)
            {
                var op = _context.Operations.FirstOrDefault(o => o.Id == item.OperationId);
                if (op != null)
                {
                    op.AssignedWorkerId = item.AssignedWorkerId;
                }
            }

            _context.SaveChanges();
            MessageBox.Show("Назначения сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
