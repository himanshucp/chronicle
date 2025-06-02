using Chronicle.Services;
using Chronicle.Web.Areas.Companies;
using Chronicle.Web.Areas.Projects;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.Disciplines
{
    [Area("Disciplines")]
    public class DisciplineController : BaseController
    {

        private readonly IDisciplineService _disciplineService;
        private readonly ILogger<DisciplineController> _logger;
        private int _tenantId = 1;

        public DisciplineController(
           IDisciplineService disciplineService,
           ILogger<DisciplineController> logger)
        {
            _disciplineService = disciplineService;
            _logger = logger;
            _tenantId = 1;
        }

        [HttpGet("Discipline")]
        public async Task<IActionResult> ListDiscipline(string searchTerm = "",
             int page = 1,
             int pageSize = 10)
        {
            var pagedResult = await _disciplineService.GetPagedDisciplinesAsync(page, pageSize, searchTerm, _tenantId);


            var viewModel = new PagedViewModel<Entities.Discipline>
            {
                Items = pagedResult.Items,
                TotalCount = pagedResult.TotalCount,
                PageSize = pageSize,
                CurrentPage = page
            };

            return View(viewModel);
        }

        [HttpGet("/Discipline/Create")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new DisciplineVewModel { DisciplineID = 0 };
            viewModel = await GetAsync(viewModel);
            return View(viewModel);
        }

        [HttpGet("/Discipline/Edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = new DisciplineVewModel { DisciplineID = id };
            viewModel = await GetAsync(viewModel);
          
            return View("Create", viewModel);
        }


        [HttpPost("/Discipline/Edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm] DisciplineVewModel model, int id)
        {
          
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            try
            {
                model = await PostAsync(model);
                return Redirect("/Discipline");
            }
            catch (Exception ex)
            {

                Failure = ex.Message;
            }

            return View("Create", model);
        }


        #region handlers

        private async Task<DisciplineVewModel> GetAsync(DisciplineVewModel model)
        {
            var discipline = await _disciplineService.GetDisciplineByIdAsync(model.DisciplineID, _tenantId);
            if (discipline == null)
            {

                model = new DisciplineVewModel
                {
                    IsActive = true,
                    TenantID = _tenantId
                };
            }
            else
            {
                _mapper.Map(discipline, model);

            }
            return model;
        }

        private async Task<DisciplineVewModel> PostAsync(DisciplineVewModel model)
        {


            if (model.DisciplineID == 0)
            {
                var discipline = new Entities.Discipline();
                _mapper.Map(model, discipline);
                var result = await _disciplineService.CreateDisciplineAsync(discipline, _tenantId);
                _cache.UpdateDisciplineInCache(discipline);
            }
            else
            {
                var discipline = await _disciplineService.GetDisciplineByIdAsync(model.DisciplineID, _tenantId);
                _mapper.Map(model, discipline);
                var result = await _disciplineService.UpdateDisciplineAsync(discipline, _tenantId);
                _cache.UpdateDisciplineInCache(discipline);
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
            CreateMap<Entities.Discipline, DisciplineVewModel>();
            //.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.OwnerCompanyID != null ? _cache.Companies[src.OwnerCompanyID].Name : null));

            CreateMap<DisciplineVewModel, Entities.Discipline>();
        }
    }

    #endregion 
}
