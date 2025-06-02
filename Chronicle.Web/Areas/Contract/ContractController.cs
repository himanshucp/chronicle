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
                return View("Create", model);
            }

            try
            {
                model = await PostAsync(model);
                return Redirect("/Contract");
            }
            catch (Exception ex)
            {

                Failure = ex.Message;
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
                    TenantID = _tenantId
                };
            }
            else
            {
                _mapper.Map(contract.Data, model);

            }
            return model;
        }

        private async Task<ContractViewModel> PostAsync(ContractViewModel model)
        {


            if (model.ContractID == 0)
            {
                var contract = new Entities.Contract();
                _mapper.Map(model, contract);
                var result = await _contractService.CreateContractAsync(contract, _tenantId);
            }
            else
            {
                var contract = await _contractService.GetContractByIdAsync(model.ContractID, _tenantId);
                _mapper.Map(model, contract.Data);
                var result = await _contractService.UpdateAsync(contract.Data, _tenantId);
            }

            return model;
        }

        #endregion

        #region Ajax handler

        [HttpGet("Contract/GetEmployeeByCompany/{companyId}")]
        public async Task<JsonResult> GetEmployeeByCompany(int companyId)
        {
            var employees = await _employeeService.GetEmployeesByCompanyAsync(companyId,_tenantId);  
            return Json(employees);
        }

        #endregion 

    }

    #region mapping
    public class MapperProfile : BaseProfile
    {
        public MapperProfile()
        {
            CreateMap<Entities.Contract, ContractViewModel>();
            //.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.OwnerCompanyID != null ? _cache.Companies[src.OwnerCompanyID].Name : null));

            CreateMap<ContractViewModel, Entities.Contract>();
        }
    }

    #endregion 
}
