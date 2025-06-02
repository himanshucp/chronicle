using Chronicle.Caching;
using Chronicle.Entities;
using Chronicle.Services;
using Chronicle.Web.Areas.Companies;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;

namespace Chronicle.Web.Areas.Projects
{
    [Area("Projects")]
    public class ProjectController : BaseController 
    {


        private readonly IProjectService _projectService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<ProjectController> _logger;
        private readonly int _tenantId;

        public ProjectController(
           IProjectService projectService,
           ICompanyService companyService,
           ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _companyService = companyService;
            _logger = logger;
            _tenantId = 1;
        }


        [HttpGet("/Project")]
        public async Task<IActionResult> ProjectList(string searchTerm = "",
             int page = 1,
             int pageSize = 10)
        {
            var pagedResult = await _projectService.GetPagedProjectsAsync(page, pageSize,searchTerm, _tenantId);

            
            var viewModel = new PagedViewModel<Entities.Project>
            {
                Items = pagedResult.Items,
                TotalCount = pagedResult.TotalCount,
                PageSize = pageSize,
                CurrentPage = page
            };

            return View(viewModel);



        }

        [HttpGet("/Project/Create")]
        public async Task<IActionResult> Create()
        {
           

            var viewModel = new ProjectViewModel { ProjectID = 0 };
            viewModel = await GetAsync(viewModel);
            return View(viewModel);
        }


        [HttpPost("/Project/Create")]
        public async Task<IActionResult> Create([FromForm] ProjectViewModel model)
        {
            ModelState.Remove("CompanySelectList");
            ModelState.Remove("CompanyName");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {

                model = await PostAsync(model);
                return Redirect("/Project");

            } catch (Exception ex)
            {
             
                Failure = ex.Message;
            }
          

            return View(model);
        }


        [HttpGet("/Project/Edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = new ProjectViewModel { ProjectID = id };
            viewModel = await GetAsync(viewModel);
         
            return View("Create", viewModel);
        }

        [HttpPost("/Project/Edit/{id?}")]
        public async Task<IActionResult> Edit([FromForm] ProjectViewModel model,int id)
        {
            ModelState.Remove("CompanySelectList");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model = await PostAsync(model);
                return Redirect("/Project");
            }
            catch (Exception ex)
            {
              
                Failure = ex.Message;
            }

            return View("Create", model);
        }


        #region handlers

        private async Task<ProjectViewModel> GetAsync(ProjectViewModel model)
        {
          

            var project = await _projectService.GetProjectByIdAsync(model.ProjectID, _tenantId);
            if (project == null) {

                model = new ProjectViewModel
                {
                    IsActive = true,
                    TenantID = _tenantId
                };
            }
            else
            {
                _mapper.Map(project ,model);
               
            }

       

            return model;
        }


        private async Task<ProjectViewModel> PostAsync(ProjectViewModel model)
        {
            

            if (model.ProjectID == 0)
            {
                var project = new Entities.Project();
                _mapper.Map(model, project);
                var result = await _projectService.CreateProjectAsync(project, _tenantId);
            } else
            {
                var project = await _projectService.GetProjectByIdAsync(model.ProjectID, _tenantId);
                _mapper.Map(model, project);
                var result = await _projectService.UpdateProjectAsync(project, _tenantId);
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
            CreateMap<Entities.Project, ProjectViewModel>();
                  //.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.OwnerCompanyID != null ? _cache.Companies[src.OwnerCompanyID].Name : null));
         
            CreateMap<ProjectViewModel, Entities.Project>();
        }
    }

    #endregion 
}
