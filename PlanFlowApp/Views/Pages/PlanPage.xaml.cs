using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Win32;
using PlanFlowApp.Model;
using PlanFlowApp.ViewModel;
using PlanFlowApp.Views.Windows;
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
    /// Логика взаимодействия для PlanPage.xaml
    /// </summary>
    public partial class PlanPage : Page
    {
        private dbPlanFlowEntities _context;

        public PlanPage()
        {
            InitializeComponent();
            _context = new dbPlanFlowEntities();
            LoadOperations();
        }

        private void LoadOperations()
        {
            _context = new dbPlanFlowEntities();

            var operations = _context.Operations
                .Include("Details")
                .Include("TypeOperation")
                .Include("OperationStatuses")
                .Include("Workers")
                .ToList();

            var operationVMs = operations.Select(op => new OperationAssignmentViewModel
            {
                OperationId = op.Id,
                DetailCode = op.Details.Code,
                DetailTitle = op.Details.Title,
                TypeOperationTitle = op.TypeOperation.Title,
                EstimatedTime = op.EstimatedTime ?? 0,
                Quantity = op.Quantity ?? 0,
                DoneQuantity = op.DoneQuantity ?? 0,
                AssignedWorkerName = op.Workers != null ? op.Workers.FirstName + " " + op.Workers.LastName : "",
                StatusTitle = op.OperationStatuses.Name
            }).ToList();

            OperationsDataGrid.DataContext = operationVMs;
        }
        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var importedRows = ReadExcel(dialog.FileName);
                    ImportOperations(importedRows);
                    LoadOperations();
                    MessageBox.Show("Импорт успешно выполнен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при импорте: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private List<OperationImportModel> ReadExcel(string filePath)
        {
            var rows = new List<OperationImportModel>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Лист 1
                var firstRowUsed = worksheet.FirstRowUsed().RowNumber();
                var lastRowUsed = worksheet.LastRowUsed().RowNumber();

                // Предполагаем, что первая строка - заголовки, данные начинаются со второй
                for (int rowNum = firstRowUsed + 1; rowNum <= lastRowUsed; rowNum++)
                {
                    var row = worksheet.Row(rowNum);

                    var detailCode = row.Cell(1).GetString().Trim();
                    var detailTitle = row.Cell(2).GetString().Trim();
                    var typeOperationTitle = row.Cell(3).GetString().Trim();

                    // Время и Количество - int, парсим с проверкой
                    if (!int.TryParse(row.Cell(4).GetString().Trim(), out int estimatedTime))
                        estimatedTime = 0;

                    if (!int.TryParse(row.Cell(5).GetString().Trim(), out int quantity))
                        quantity = 0;

                    if (string.IsNullOrEmpty(detailCode) || string.IsNullOrEmpty(typeOperationTitle))
                        continue; // пропускаем пустые или некорректные

                    rows.Add(new OperationImportModel
                    {
                        DetailCode = detailCode,
                        DetailTitle = detailTitle,
                        TypeOperationTitle = typeOperationTitle,
                        EstimatedTime = estimatedTime,
                        Quantity = quantity
                    });
                }
            }

            return rows;
        }

        private void ImportOperations(List<OperationImportModel> importedRows)
        {
            var detailsDict = _context.Details.ToDictionary(d => d.Code, d => d);
            var typeOperationsDict = _context.TypeOperation.ToDictionary(t => t.Title, t => t.Id);

            int defaultStatusId = 1; // "Не начато"

            foreach (var row in importedRows)
            {
                // Ищем или создаём Detail
                if (!detailsDict.TryGetValue(row.DetailCode, out var detail))
                {
                    detail = new Details { Code = row.DetailCode, Title = row.DetailTitle ?? "" };
                    _context.Details.Add(detail);
                    _context.SaveChanges();
                    detailsDict[detail.Code] = detail;
                }

                // Ищем или создаём TypeOperation
                if (!typeOperationsDict.TryGetValue(row.TypeOperationTitle, out int typeOperationId))
                {
                    var typeOp = new TypeOperation { Title = row.TypeOperationTitle };
                    _context.TypeOperation.Add(typeOp);
                    _context.SaveChanges();
                    typeOperationId = typeOp.Id;
                    typeOperationsDict[typeOp.Title] = typeOperationId;
                }

                // Ищем существующую операцию по DetailId + TypeOperationId
                var operation = _context.Operations.FirstOrDefault(op =>
                    op.DetailId == detail.Id && op.TypeOperationId == typeOperationId);

                if (operation != null)
                {
                    // Обновляем поля, кроме AssignedWorkerId и DoneQuantity
                    operation.EstimatedTime = row.EstimatedTime;
                    operation.Quantity = row.Quantity;
                    operation.StatusId = defaultStatusId;
                }
                else
                {
                    // Добавляем новую операцию с дефолтным статусом
                    operation = new Operations
                    {
                        DetailId = detail.Id,
                        TypeOperationId = typeOperationId,
                        EstimatedTime = row.EstimatedTime,
                        Quantity = row.Quantity,
                        DoneQuantity = 0,
                        StatusId = defaultStatusId,
                        AssignedWorkerId = null
                    };
                    _context.Operations.Add(operation);
                }
            }

            _context.SaveChanges();
        }

        private void OperationsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OperationsDataGrid.SelectedItem is OperationAssignmentViewModel selectedOperation)
            {
                var editWindow = new EditOperationWindow(selectedOperation);
                editWindow.Owner = Window.GetWindow(this); // Правильно установим окно-владельца

                if (editWindow.ShowDialog() == true)
                {
                    // После закрытия и сохранения данных обновим таблицу
                    LoadOperations();
                }
            }
        }
    }

}

