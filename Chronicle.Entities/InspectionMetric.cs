using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class InspectionMetric
    {
        public int MetricID { get; set; }
        public int InspectionRequestID { get; set; }
        public string MetricType { get; set; }
        public string MetricName { get; set; }
        public decimal MetricValue { get; set; }
        public string MetricUnit { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? Variance { get; set; }
        public decimal? BenchmarkValue { get; set; }
        public DateTime CalculationDate { get; set; }
        public int? CalculatedBy { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
