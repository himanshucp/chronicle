using Chronicle.Services;
using Chronicle.Web.Areas.Employees;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.Contract
{
    [Area("Contract")]
    public class ContractController : BaseController
    {

        private readonly IContractService? _contractService;
        private readonly IEmployeeService? _employeeService;
        private readonly ILogger<ContractController>? _logger;
        private int _tenantId = 1;

        public ContractController(
                 IContractService contractService,
                 IEmployeeService employeeService,
                 ILogger<ContractController> logger)
        {
            _contractService = contractService;
            _employeeService = employeeService;
            _logger = logger;
            _tenantId = 1;
        }

        #region Page


        [HttpGet("/Contract")]
        public async Task<IActionResult> ListContract(string searchTerm = "",
        int page = 1,
        int pageSize = 10)
        {
            var result = await _contractService.GetPagedContractsAsync(page, pageSize, _tenantId, searchTerm);

            var viewModel = new PagedViewModel<Entities.Contract>
            {
                Items = result.Data.Items,
                TotalCount = result.Data.TotalCount,
                PageSize = pageSize,
                CurrentPage = page
            };


            return View(viewModel);
        }

        [HttpGet("/Contract/Create")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ContractViewModel { ContractID = 0 };
            viewModel = await GetAsync(viewModel);
            return View(viewModel);
        }


        [HttpGet("/Contract/Edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = new ContractViewModel { ContractID = id };
            viewModel = await GetAsync(viewModel);

            return View("Create", viewModel);
        }

        [HttpPost("/Contract/Edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm] ContractViewModel model, int id)
        {

            if (!ModelState.IsValid)
            {
                // Reload the view with validation errors
                model = await GetAsync(model);
                return View("Create", model);
            }

            try
            {
                model = await PostAsync(model);

                // Add success message
                Success = model.ContractID == 0 ? "Contract created successfully!" : "Contract updated successfully!";

                return Redirect("/Contract");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving contract with ID: {ContractID}", model.ContractID);
                Failure = ex.Message;

                // Reload the view with the error
                model = await GetAsync(model);
            }

            return View("Create", model);
        }




        #endregion

        #region Handler

        private async Task<ContractViewModel> GetAsync(ContractViewModel model)
        {
            var contract = await _contractService.GetContractByIdAsync(model.ContractID, _tenantId);
            if (contract.Data == null)
            {
                model = new ContractViewModel
                {
                    IsActive = true,
                    TenantID = _tenantId,
                    ContractEmployees = new List<ContractEmployeeViewModel>()
                };
            }
            else
            {
                _mapper.Map(contract.Data, model);

                // Ensure ContractEmployees is initialized if null
                if (model.ContractEmployees == null)
                {
                    model.ContractEmployees = new List<ContractEmployeeViewModel>();
                }
            }
            return model;
        }

        private async Task<ContractViewModel> PostAsync(ContractViewModel model)
        {
            try
            {
                if (model.ContractID == 0)
                {
                    // Create new contract
                    var contract = new Entities.Contract();
                    
                    _mapper.Map(model, contract);

                    // Ensure TenantID is set
                    contract.TenantID = _tenantId;

                    // Map and set ContractEmployees
                    if (model.ContractEmployees != null && model.ContractEmployees.Any())
                    {
                        contract.ContractEmployees = new List<Entities.ContractEmployee>();
                        foreach (var empViewModel in model.ContractEmployees)
                        {
                            var contractEmployee = new Entities.ContractEmployee();
                            _mapper.Map(empViewModel, contractEmployee);
                            contractEmployee.TenantID = _tenantId;
                            contractEmployee.IsActive = empViewModel.IsActive;
                            contract.ContractEmployees.Add(contractEmployee);
                        }
                    }

                    var result = await _contractService.CreateContractAsync(contract, _tenantId);

                    if (result.Success)
                    {
                        model.ContractID = result.Data;
                    }
                    else
                    {
                        throw new Exception(result.Message ?? "Failed to create contract");
                    }
                }
                else
                {
                    // Update existing contract
                    // Update existing contract and employees in single transaction
                    var contractResult = await _contractService.GetContractByIdAsync(model.ContractID, _tenantId);
                    if (contractResult.Data == null)
                    {
                        throw new Exception("Contract not found");
                    }

                    var contract = contractResult.Data;
                    _mapper.Map(model, contract);

                    // Map ContractEmployees for update
                    if (model.ContractEmployees != null)
                    {
                        contract.ContractEmployees = new List<Entities.ContractEmployee>();
                        foreach (var empViewModel in model.ContractEmployees)
                        {
                            var contractEmployee = new Entities.ContractEmployee();
                            _mapper.Map(empViewModel, contractEmployee);
                            contractEmployee.ContractID = contract.ContractID;
                            contractEmployee.TenantID = _tenantId;
                            contractEmployee.IsActive = empViewModel.IsActive;
                            contract.ContractEmployees.Add(contractEmployee);
                        }
                    }

                    // Use the new UpdateWithEmployeesAsync method that handles both in single transaction
                    var updateResult = await _contractService.UpdateWithEmployeesAsync(contract, _tenantId);

                    if (!updateResult.Success)
                    {
                        throw new Exception(updateResult.Message ?? "Failed to update contract");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in PostAsync for ContractID: {ContractID}", model.ContractID);
                throw;
            }

            return model;


        }

        #endregion

        #region Ajax handler

        [HttpGet("Contract/GetEmployeeByCompany/{companyId}")]
        public async Task<JsonResult> GetEmployeeByCompany(int companyId)
        {

            try
            {
                var employees = await _employeeService.GetEmployeesByCompanyAsync(companyId, _tenantId);
                return Json(employees);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting employees for company: {CompanyId}", companyId);
                return Json(new List<object>());
            }
          
        }

        #endregion 

    }

    #region mapping
    public class MapperProfile : BaseProfile
    {
        public MapperProfile()
        {
            CreateMap<Entities.Contract, ContractViewModel>()
            .ForMember(dest => dest.ContractEmployees, opt => opt.MapFrom(src => src.ContractEmployees));

            CreateMap<ContractViewModel, Entities.Contract>()
            .ForMember(dest => dest.ContractEmployees, opt => opt.Ignore()); // Handle manually in controller

            CreateMap<Entities.ContractEmployee, ContractEmployeeViewModel>()
                 .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                     src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : ""))
                 .ForMember(dest => dest.Role, opt => opt.MapFrom(src =>
                     src.Role != null ? src.Role.Name : ""))
                 .ForMember(dest => dest.LineManagerName, opt => opt.MapFrom(src =>
                     src.LineManager != null ? $"{src.LineManager.FirstName} {src.LineManager.LastName}" : ""));

            CreateMap<ContractEmployeeViewModel, Entities.ContractEmployee>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.LineManager, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.ModuleAccess, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.HourlyRate, opt => opt.Ignore())
                .ForMember(dest => dest.EstimatedHours, opt => opt.Ignore())
                .ForMember(dest => dest.ActualHours, opt => opt.Ignore());
        }
    }

    #endregion 
}
