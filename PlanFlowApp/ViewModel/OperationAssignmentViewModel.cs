using PlanFlowApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace PlanFlowApp.ViewModel
{
    public class OperationAssignmentViewModel : INotifyPropertyChanged
    {
        public int OperationId { get; set; }
        public string DetailCode { get; set; }
        public string DetailTitle { get; set; }
        public string TypeOperationTitle { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); UpdateStatus(); }
        }
        private int _estimatedTime;
        public int EstimatedTime
        {
            get => _estimatedTime;
            set { _estimatedTime = value; OnPropertyChanged(); }
        }

        private int _doneQuantity;
        public int DoneQuantity
        {
            get => _doneQuantity;
            set
            {
                if (_doneQuantity != value)
                {
                    _doneQuantity = value;
                    OnPropertyChanged(nameof(DoneQuantity));
                    UpdateStatus();
                }
            }
        }
        private string _statusTitle;
        public string StatusTitle
        {
            get => _statusTitle;
            set { _statusTitle = value; OnPropertyChanged(); }
        }
        public int? AssignedWorkerId { get; set; }
        private string _assignedWorkerName;
        public string AssignedWorkerName
        {
            get => _assignedWorkerName;
            set { _assignedWorkerName = value; OnPropertyChanged(); }
        }

        // === INotifyPropertyChanged реализация ===
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private void UpdateStatus()
        {
            if (DoneQuantity <= 0)
            {
                StatusTitle = "Не начато";
            }
            else if (DoneQuantity >= Quantity)
            {
                StatusTitle = "Выполнено";
            }
            else
            {
                StatusTitle = "В процессе";
            }
        }
    }

}
