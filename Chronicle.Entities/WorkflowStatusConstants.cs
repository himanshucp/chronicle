﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    /// <summary>
    /// Workflow status constants
    /// </summary>
    public static class WorkflowStatusConstants
    {
        public const string Active = "Active";
        public const string Completed = "Completed";
        public const string Cancelled = "Canceled";
        public const string Suspended = "Suspended";
        public const string Error = "Error";
     
    }

}
