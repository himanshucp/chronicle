using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.SubDiscipline
{
    public class SubDisciplineVewModel
    {
        public int SubDisciplineID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string SubDisciplineName { get; set; }
        public string Description { get; set; }
        public int TenantID { get; set; }

        [Required(ErrorMessage = "Discipline is required")]
        public int DisciplineID { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
