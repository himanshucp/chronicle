using Chronicle.Services;
using Chronicle.Web.Areas.Disciplines;
using Chronicle.Web.Areas.Projects;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.SubDiscipline
{
    [Area("SubDiscipline")]
    public class SubDisciplineController : BaseController 
    {

        private readonly ISubDisciplineService _subDisciplineService;
        private readonly ILogger<SubDisciplineController> _logger;
        private int _tenantId = 1;

        public SubDisciplineController(
           ISubDisciplineService subCisciplineService,
           ILogger<SubDisciplineController> logger)
        {
            _subDisciplineService = subCisciplineService;
            _logger = logger;
            _tenantId = 1;
        }
        [HttpGet("SubDiscipline")]
        public async Task<IActionResult> ListSubDiscipline(string searchTerm = "",
               int page = 1,
               int pageSize = 10)
        {
            var pagedResult = await _subDisciplineService.GetPagedSubDisciplinesAsync(page, pageSize, searchTerm, _tenantId);


            var viewModel = new PagedViewModel<Entities.SubDiscipline>
            {
                Items = pagedResult.Items,
                TotalCount = pagedResult.TotalCount,
                PageSize = pageSize,
                CurrentPage = page
            };

            return View(viewModel);
        }

        [HttpGet("/SubDiscipline/Create")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new SubDisciplineVewModel { SubDisciplineID = 0 };
            viewModel = await GetAsync(viewModel);
            return View(viewModel);
        }


        [HttpGet("/SubDiscipline/Edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = new SubDisciplineVewModel { SubDisciplineID = id };
            viewModel = await GetAsync(viewModel);

            return View("Create", viewModel);
        }

        [HttpPost("/SubDiscipline/Edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm] SubDisciplineVewModel model, int id)
        {

            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            try
            {
                model = await PostAsync(model);
                return Redirect("/SubDiscipline");
            }
            catch (Exception ex)
            {

                Failure = ex.Message;
            }

            return View("Create", model);
        }


        #region handlers

        private async Task<SubDisciplineVewModel> GetAsync(SubDisciplineVewModel model)
        {
            var subDiscipline = await _subDisciplineService.GetSubDisciplineByIdAsync(model.SubDisciplineID, _tenantId);
            if (subDiscipline == null)
            {

                model = new SubDisciplineVewModel
                {
                    IsActive = true,
                    TenantID = _tenantId
                };
            }
            else
            {
                _mapper.Map(subDiscipline, model);

            }
            return model;
        }

        private async Task<SubDisciplineVewModel> PostAsync(SubDisciplineVewModel model)
        {


            if (model.SubDisciplineID == 0)
            {
                var subDiscipline = new Entities.SubDiscipline();
                _mapper.Map(model, subDiscipline);
                var result = await _subDisciplineService.CreateSubDisciplineAsync(subDiscipline, _tenantId);
            }
            else
            {
                var subDiscipline = await _subDisciplineService.GetSubDisciplineByIdAsync(model.SubDisciplineID, _tenantId);
                _mapper.Map(model, subDiscipline);
                var result = await _subDisciplineService.UpdateSubDisciplineAsync(subDiscipline, _tenantId);
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
            CreateMap<Entities.SubDiscipline, SubDisciplineVewModel>();
            //.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.OwnerCompanyID != null ? _cache.Companies[src.OwnerCompanyID].Name : null));

            CreateMap<SubDisciplineVewModel, Entities.SubDiscipline>();
        }
    }

    #endregion 
}
