using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class Discipline
    {
        public int DisciplineID { get; set; }
        public string DisciplineName { get; set; }
        public string Description { get; set; }
        public int TenantID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; } 
        public bool? IsActive { get; set; }
    }
}
