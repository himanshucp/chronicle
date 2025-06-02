using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class SubDiscipline
    {
        public int SubDisciplineID { get; set; }
        public string SubDisciplineName { get; set; }
        public string Description { get; set; }
        public int TenantID { get; set; }
        public int DisciplineID { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
        public bool? IsActive { get; set; } = true;

        // Optional: Navigation property for related Discipline
        // public virtual Discipline Discipline { get; set; }
    }
}
