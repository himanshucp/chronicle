using Chronicle.Entities;
using Chronicle.Services;
using Chronicle.Web.Areas.Companies;
using Chronicle.Web.Areas.Disciplines;
using Chronicle.Web.Areas.SubDiscipline;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.Employees
{

   

    [Area("Employees")]
    public class EmployeeController : BaseController
    {

        private readonly IEmployeeService? _employeeService;
        private readonly ILogger<EmployeeController>? _logger;
        private int _tenantId = 1;
        public EmployeeController(
        IEmployeeService employeeService,
        ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
            _tenantId = 1;
        }

        [HttpGet("/Employee")]
        public async Task<IActionResult> EmployeeList(string searchTerm = "",
            int page = 1,
            int pageSize = 10)
        {
            var result = await _employeeService.GetPagedEmployeesAsync(page, pageSize, searchTerm, 1);

            var viewModel = new PagedViewModel<Entities.Employee>
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                PageSize = pageSize,
                CurrentPage = page
            };


            return View(viewModel);
        }

        [HttpGet("/Employee/Create")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new EmployeeViewModel { EmployeeID = 0 };
            viewModel = await GetAsync(viewModel);
            return View(viewModel);
        }



        [HttpGet("/Employee/Edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = new EmployeeViewModel { EmployeeID = id };
            viewModel = await GetAsync(viewModel);

            return View("Create", viewModel);
        }


        [HttpPost("/Employee/Edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm] EmployeeViewModel model, int id)
        {

            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            try
            {
                model = await PostAsync(model);
                return Redirect("/Employee");
            }
            catch (Exception ex)
            {

                Failure = ex.Message;
            }

            return View("Create", model);
        }



        #region handlers

        private async Task<EmployeeViewModel> GetAsync(EmployeeViewModel model)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(model.EmployeeID, _tenantId);
            if (employee == null)
            {

                model = new EmployeeViewModel
                {
                    IsActive = true,
                    TenantID = _tenantId
                };
            }
            else
            {
                _mapper.Map(employee, model);

            }
            return model;
        }

        private async Task<EmployeeViewModel> PostAsync(EmployeeViewModel model)
        {


            if (model.EmployeeID == 0)
            {
                var employee = new Entities.Employee();
                _mapper.Map(model, employee);
                var result = await _employeeService.CreateEmployeeAsync(employee, _tenantId);
            }
            else
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(model.EmployeeID, _tenantId);
                _mapper.Map(model, employee);
                var result = await _employeeService.UpdateEmployeeAsync(employee, _tenantId);
            }

            return model;
        }
        #endregion
    }

    #region mapping
    public class MapperProfile : BaseProfile
    {
        public MapperProfile()
        {
            CreateMap<Entities.Employee, EmployeeViewModel>();
            //.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.OwnerCompanyID != null ? _cache.Companies[src.OwnerCompanyID].Name : null));

            CreateMap<EmployeeViewModel, Entities.Employee>();
        }
    }

    #endregion 


}
