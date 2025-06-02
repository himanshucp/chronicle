using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.Disciplines
{
    public class DisciplineVewModel
    {
        public int DisciplineID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string? DisciplineName { get; set; }
        public string? Description { get; set; }
        public int TenantID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
