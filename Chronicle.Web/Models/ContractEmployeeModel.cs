using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Models
{
    public class ContractEmployeeModel
    {
        public int ContractEmployeeID { get; set; }

     
        public int ContractID { get; set; }

        [Required(ErrorMessage = "Employeeis required")]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public int RoleID { get; set; }

        [Required(ErrorMessage = "Line Manager is required")]
        public int? LineManagerID { get; set; }
        public decimal? HourlyRate { get; set; }
        public int? EstimatedHours { get; set; }
        public int? ActualHours { get; set; }
        public DateTime DateActivated { get; set; }
        public DateTime? DateDeactivated { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
