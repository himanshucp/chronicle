using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.Contract
{
    public class ContractEmployeeViewModel
    {
        public int ContractEmployeeID { get; set; }

        public int ContractID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        public string? EmployeeName { get; set; }

        public int? RoleID { get; set; }

        public string? Role { get; set; }

        public int? LineManagerID { get; set; }

        public string? LineManagerName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateActivated { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateDeactivated { get; set; }

        public bool IsActive { get; set; } = true;

        public int TenantID { get; set; }

        public bool IsNewRecord { get; set; } = true;
    }
}
