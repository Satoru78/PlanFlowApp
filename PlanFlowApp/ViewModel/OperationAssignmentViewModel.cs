using PlanFlowApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public int Quantity { get; set; }

        public int EstimatedTime { get; set; }

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
            set
            {
                if (_statusTitle != value)
                {
                    _statusTitle = value;
                    OnPropertyChanged(nameof(StatusTitle));
                }
            }
        }
        public int? AssignedWorkerId { get; set; }
        public string AssignedWorkerName { get; set; }

        // === INotifyPropertyChanged реализация ===
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
