using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanFlowApp.ViewModel
{
    public class EditOperationViewModel : INotifyPropertyChanged
    {
        private readonly OperationAssignmentViewModel _original;

        public EditOperationViewModel(OperationAssignmentViewModel source)
        {
            _original = source;

            DetailTitle = source.DetailTitle;
            TypeOperationTitle = source.TypeOperationTitle;
            AssignedWorkerName = source.AssignedWorkerName;

            Issued = source.Quantity;
            Done = source.DoneQuantity;
        }

        public string DetailTitle { get; }
        public string TypeOperationTitle { get; }
        public string AssignedWorkerName { get; } 

        private int _issued;
        public int Issued
        {
            get => _issued;
            set
            {
                if (_issued != value)
                {
                    _issued = value;
                    OnPropertyChanged(nameof(Issued));
                    OnPropertyChanged(nameof(Remaining));
                    OnPropertyChanged(nameof(Rejected));
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }

        private int _done;
        public int Done
        {
            get => _done;
            set
            {
                if (_done != value)
                {
                    _done = value;
                    OnPropertyChanged(nameof(Done));
                    OnPropertyChanged(nameof(Remaining));
                    OnPropertyChanged(nameof(Rejected));
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }

        public int Remaining => Math.Max(0, Issued - Done);
        public int Rejected => Math.Max(0, Issued - Done);
        public int Progress => Issued > 0 ? (int)((double)Done / Issued * 100) : 0;

        public void ApplyChanges()
        {
            _original.Quantity = Issued;
            _original.DoneQuantity = Done;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
