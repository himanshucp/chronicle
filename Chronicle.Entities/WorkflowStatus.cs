using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    /// <summary>
    /// Workflow status constants
    /// </summary>
    public static class WorkflowStatus
    {
        public const string Active = "Active";
        public const string Completed = "Completed";
        public const string Canceled = "Canceled";
        public const string Error = "Error";
    }
}
