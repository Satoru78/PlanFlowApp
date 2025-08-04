using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanFlowApp.Model
{
    public class OperationImportModel
    {
        public string DetailCode { get; set; }
        public string DetailTitle { get; set; }
        public string TypeOperationTitle { get; set; }  // ← теперь строка
        public int EstimatedTime { get; set; }
        public int Quantity { get; set; }
    }
}
