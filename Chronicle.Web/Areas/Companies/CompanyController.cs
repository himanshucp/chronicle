using chronicle.Model.Common;
using Chronicle.Entities;
using Chronicle.Services;
using Chronicle.Web.Areas.Projects;
using Chronicle.Web.Areas.Users;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Text;
namespace Chronicle.Web.Areas.Companies
{
    [Area("Companies")]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyRoleService _companyRoleService;

        public CompanyController(
            ICompanyService companyService,
            ICompanyRoleService companyRoleService)
        {
            _companyService = companyService;
            _companyRoleService = companyRoleService;
        }

        // GET: Company
        [HttpGet("/Company")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Company/GetPaged
        [HttpGet("/Company/GetPaged")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10, string searchTerm = "", int tenantId = 1)
        {
            try
            {
                var result = await _companyService.GetPagedCompaniesAsync(page, pageSize, searchTerm, tenantId);
                return Json(new { success = true, data = result });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetAll
        [HttpGet("/Company/GetAll")]
        public async Task<IActionResult> GetAll(int tenantId = 1)
        {
            try
            {
                var companies = await _companyService.GetAllCompaniesAsync(tenantId);
                return Json(new { success = true, data = companies });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetActive
        [HttpGet("/Company/GetActive")]
        public async Task<IActionResult> GetActive(int tenantId = 1)
        {
            try
            {
                var companies = await _companyService.GetActiveCompaniesAsync(tenantId);
                return Json(new { success = true, data = companies });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetById/5
        [HttpGet("/Company/GetById")]
        public async Task<IActionResult> GetById(int id, int tenantId = 1)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id, tenantId);
                if (company == null)
                {
                    return Json(new { success = false, message = "Company not found" });
                }
                return Json(new { success = true, data = company });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetByName
        [HttpGet("/Company/GetByName")]
        public async Task<IActionResult> GetByName(string name, int tenantId = 1)
        {
            try
            {
                var company = await _companyService.GetCompanyByNameAsync(name, tenantId);
                return Json(new { success = true, data = company });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetByEmail
        [HttpGet("/Company/GetByEmail")]
        public async Task<IActionResult> GetByEmail(string email, int tenantId = 1)
        {
            try
            {
                var company = await _companyService.GetCompanyByEmailAsync(email, tenantId);
                return Json(new { success = true, data = company });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetByAbbreviation
        [HttpGet("/Company/GetByAbbreviation")]
        public async Task<IActionResult> GetByAbbreviation(string abbreviation, int tenantId = 1)
        {
            try
            {
                var company = await _companyService.GetByAbbrivationAsync(abbreviation, tenantId);
                return Json(new { success = true, data = company });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetCompaniesByProject/5
        [HttpGet("/Company/GetCompaniesByProject")]
        public async Task<IActionResult> GetCompaniesByProject(int projectId, int tenantId = 1)
        {
            try
            {
                var companies = await _companyService.GetCompaniesByProjectAsync(projectId, tenantId);
                return Json(new { success = true, data = companies });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetCompaniesWithExpiringLicenses
        [HttpGet("/Company/GetCompaniesWithExpiringLicenses")]
        public async Task<IActionResult> GetCompaniesWithExpiringLicenses(
            int daysThreshold = 30,
            int page = 1,
            int pageSize = 10,
            int tenantId = 1)
        {
            try
            {
                var result = await _companyService.GetCompaniesWithExpiringLicensesAsync(daysThreshold, page, pageSize, tenantId);
                return Json(new { success = true, data = result });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/Search
        [HttpGet("/Company/Search")]
        public async Task<IActionResult> Search(string term, int tenantId = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                {
                    return Json(new { success = true, data = new List<Company>() });
                }

                var companies = await _companyService.GetAllCompaniesAsync(tenantId);
                var filtered = companies.Where(c =>
                    c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (c.Abbrivation != null && c.Abbrivation.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Location != null && c.Location.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (c.ContactPerson != null && c.ContactPerson.Contains(term, StringComparison.OrdinalIgnoreCase))
                ).Take(10).ToList();

                return Json(new { success = true, data = filtered });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Company/Create
        [HttpPost("/Company/Create")]
        public async Task<IActionResult> Create([FromBody] Company company, int tenantId = 1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed", errors = errors });
                }

                var id = await _companyService.CreateCompanyAsync(company, tenantId);
                return Json(new { success = true, message = "Company created successfully", data = new { CompanyID = id } });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // PUT: Company/Update
        [HttpPut("/Company/Update")]
        public async Task<IActionResult> Update([FromBody] Company company, int tenantId = 1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed", errors = errors });
                }

                var result = await _companyService.UpdateCompanyAsync(company, tenantId);
                if (result)
                {
                    return Json(new { success = true, message = "Company updated successfully" });
                }
                return Json(new { success = false, message = "Failed to update company" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // DELETE: Company/Delete/5
        [HttpDelete("/Company/Delete")]
        public async Task<IActionResult> Delete(int id, int tenantId = 1)
        {
            try
            {
                var result = await _companyService.DeleteCompanyAsync(id, tenantId);
                if (result)
                {
                    return Json(new { success = true, message = "Company deleted successfully" });
                }
                return Json(new { success = false, message = "Failed to delete company" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Company/Activate/5
        [HttpPost("/Company/Activate")]
        public async Task<IActionResult> Activate(int id, int tenantId = 1)
        {
            try
            {
                var result = await _companyService.ActivateCompanyAsync(id, tenantId);
                if (result)
                {
                    return Json(new { success = true, message = "Company activated successfully" });
                }
                return Json(new { success = false, message = "Failed to activate company" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Company/Deactivate/5
        [HttpPost("/Company/Deactivate")]
        public async Task<IActionResult> Deactivate(int id, int tenantId = 1)
        {
            try
            {
                var result = await _companyService.DeactivateCompanyAsync(id, tenantId);
                if (result)
                {
                    return Json(new { success = true, message = "Company deactivated successfully" });
                }
                return Json(new { success = false, message = "Failed to deactivate company" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Company/BulkUpdate
        [HttpPost("/Company/BulkUpdate")]
        public async Task<IActionResult> BulkUpdate([FromBody] BulkUpdateRequest request)
        {
            try
            {
                int successCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                foreach (var companyId in request.CompanyIds)
                {
                    try
                    {
                        bool result = false;
                        switch (request.Action.ToLower())
                        {
                            case "activate":
                                result = await _companyService.ActivateCompanyAsync(companyId, request.TenantId);
                                break;
                            case "deactivate":
                                result = await _companyService.DeactivateCompanyAsync(companyId, request.TenantId);
                                break;
                            case "delete":
                                result = await _companyService.DeleteCompanyAsync(companyId, request.TenantId);
                                break;
                            case "updatelocation":
                                var company = await _companyService.GetCompanyByIdAsync(companyId, request.TenantId);
                                if (company != null)
                                {
                                    company.Location = request.NewValue;
                                    result = await _companyService.UpdateCompanyAsync(company, request.TenantId);
                                }
                                break;
                        }

                        if (result)
                            successCount++;
                        else
                            errorCount++;
                    }
                    catch (System.Exception ex)
                    {
                        errorCount++;
                        errors.Add($"Company ID {companyId}: {ex.Message}");
                    }
                }

                return Json(new
                {
                    success = true,
                    message = $"Bulk operation completed. Success: {successCount}, Errors: {errorCount}",
                    successCount = successCount,
                    errorCount = errorCount,
                    errors = errors
                });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/GetStats
        [HttpGet("/Company/GetStats")]
        public async Task<IActionResult> GetStats(int tenantId = 1)
        {
            try
            {
                var allCompanies = await _companyService.GetAllCompaniesAsync(tenantId);
                var activeCompanies = await _companyService.GetActiveCompaniesAsync(tenantId);
                var expiringLicenses = await _companyService.GetCompaniesWithExpiringLicensesAsync(30, 1, int.MaxValue, tenantId);

                var stats = new
                {
                    TotalCompanies = allCompanies.Count(),
                    ActiveCompanies = activeCompanies.Count(),
                    InactiveCompanies = allCompanies.Count() - activeCompanies.Count(),
                    ExpiringLicenses = expiringLicenses.Items.Count()
                };

                return Json(new { success = true, data = stats });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Company/Export
        [HttpGet("/Company/Export")]
        public async Task<IActionResult> Export(int tenantId = 1)
        {
            try
            {
                var companies = await _companyService.GetAllCompaniesAsync(tenantId);

                var csv = new StringBuilder();
                csv.AppendLine("CompanyID,Name,Abbreviation,Email,ContactPerson,Phone,Fax,Website,Location,Address,TaxNumber,LicenseNumber,LicenseExpiryDate,InsuranceDetails,IsActive,CreatedDate,LastModifiedDate");

                foreach (var company in companies)
                {
                    csv.AppendLine($"{company.CompanyID}," +
                                 $"\"{company.Name?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.Abbrivation?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.Email?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.ContactPerson?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.Phone?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.Fax?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.WebSite?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.Location?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.Address?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.TaxNumber?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.LicenseNumber?.Replace("\"", "\"\"")}\"," +
                                 $"\"{company.LicenseExpiryDate?.ToString("yyyy-MM-dd")}\"," +
                                 $"\"{company.InsuranceDetails?.Replace("\"", "\"\"")}\"," +
                                 $"{company.IsActive}," +
                                 $"\"{company.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss")}\"," +
                                 $"\"{company.LastModifiedDate?.ToString("yyyy-MM-dd HH:mm:ss")}\"");
                }

                var fileName = $"companies_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var contentType = "text/csv";
                var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());

                return File(csvBytes, contentType, fileName);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to export companies: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Company/ValidateUnique
        [HttpPost("/Company/ValidateUnique")]
        public async Task<IActionResult> ValidateUnique([FromBody] ValidateUniqueRequest request)
        {
            try
            {
                bool isUnique = true;
                string message = "";

                switch (request.Field.ToLower())
                {
                    case "name":
                        var companyByName = await _companyService.GetCompanyByNameAsync(request.Value, request.TenantId);
                        isUnique = companyByName == null || (request.ExcludeId.HasValue && companyByName.CompanyID == request.ExcludeId.Value);
                        message = isUnique ? "Company name is available" : "Company name already exists";
                        break;
                    case "abbreviation":
                        var companyByAbbr = await _companyService.GetByAbbrivationAsync(request.Value, request.TenantId);
                        isUnique = companyByAbbr == null || (request.ExcludeId.HasValue && companyByAbbr.CompanyID == request.ExcludeId.Value);
                        message = isUnique ? "Abbreviation is available" : "Abbreviation already exists";
                        break;
                    case "email":
                        var companyByEmail = await _companyService.GetCompanyByEmailAsync(request.Value, request.TenantId);
                        isUnique = companyByEmail == null || (request.ExcludeId.HasValue && companyByEmail.CompanyID == request.ExcludeId.Value);
                        message = isUnique ? "Email is available" : "Email already exists";
                        break;
                }

                return Json(new { success = true, isUnique = isUnique, message = message });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }




#region mapping
public class MapperProfile : BaseProfile
    {
        public MapperProfile()
        {
            CreateMap<Company,CompanyModel>();
            CreateMap<CompanyModel, Company>();
        }
    }

    #endregion 
}

