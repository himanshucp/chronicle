using Chronicle.Entities;
using Chronicle.Services;
using Chronicle.Web.Areas.Projects;
using Chronicle.Web.Areas.Users;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
namespace Chronicle.Web.Areas.Companies
{
    [Area("Companies")]
    public class CompanyController : BaseController 
    {
        private readonly ICompanyService  _companyService;
        //private readonly ILogger<CompanyController> _logger;

        public CompanyController(
           ICompanyService companyService)
        {
            _companyService = companyService;
            //_logger = logger;
        }

        [HttpGet("/Company")]
        public async Task<IActionResult> CompanyList(string searchTerm = "",
            int page = 1,
            int pageSize = 10)
        {
            var result = await _companyService.GetPagedCompaniesAsync(page, pageSize,searchTerm,1);

         

            var model = new CompanyListViewModel
            {
                Companies = result.Items,
                TotalCount = result.TotalCount,
                CurrentPage = page,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            return View(model);


       
        }


        [HttpGet("/Company/Create")]
        public async Task<IActionResult> Create()
        {
            CompanyModel model = new CompanyModel();
            return View(model);
        }

        [HttpPost("/Company/Create")]
        public async Task<IActionResult> Create([FromForm] CompanyModel model)
        {
          
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model = await PostAsync(model);
                return Redirect("/Company");
            }
            catch (Exception ex)
            {

                Failure = ex.Message;
            }


            return View(model);
        }

        [HttpGet("/Company/Edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            CompanyModel model = new CompanyModel();
            var company = await _companyService.GetCompanyByIdAsync(id,1);
            _mapper.Map(company, model);
            return View("Create", model);
        }

        [HttpPost("/Company/Edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm] CompanyModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model = await PostAsync(model);
                return Redirect("/Company");
            }
            catch (Exception ex)
            {

                Failure = ex.Message;
            }
            return View("Create", model);
        }

     
        [HttpPost("delete"), AjaxOnly]
        public async Task<IActionResult> Delete([FromForm] Delete model)
        {
            int id = model.Id; 
            //await PostAsync(model);
            return Json(true);
        }



        #region handler

        private async Task<CompanyModel> PostAsync(CompanyModel model)
        {


            if (model.CompanyID == 0)
            {
                var company = new Entities.Company();
                _mapper.Map(model, company);
                var result = await _companyService.CreateCompanyAsync(company, 1);
            }
            else
            {
                var company = await _companyService.GetCompanyByIdAsync(model.CompanyID, 1);
                _mapper.Map(model, company);
                var result = await _companyService.UpdateCompanyAsync(company, 1);
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
            CreateMap<Company,CompanyModel>();
            CreateMap<CompanyModel, Company>();
        }
    }

    #endregion 
}
