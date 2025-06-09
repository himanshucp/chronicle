using Chronicle.Services;
using Chronicle.Services.Interface;
using Chronicle.Web.Code;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.WorkFlow
{
    [Area("WorkFlow")]
    public class WorkflowController : BaseController
    {
        private readonly IWorkflowService _workflowService;
        private readonly IWorkflowStepService _stepService;
        private readonly IWorkflowTransitionService _transitionService;
        private readonly IWorkflowInstanceService _instanceService;
        private readonly IWorkflowAssignmentService _assignmentService;
        private readonly ILogger<WorkflowController> _logger;
        private int _tenantId;


        public WorkflowController(
            IWorkflowService workflowService,
            IWorkflowStepService stepService,
            IWorkflowTransitionService transitionService,
            IWorkflowInstanceService instanceService,
            IWorkflowAssignmentService assignmentService,
            ILogger<WorkflowController> logger)
        {
            _workflowService = workflowService;
            _stepService = stepService;
            _transitionService = transitionService;
            _instanceService = instanceService;
            _assignmentService = assignmentService;
            _logger = logger;
            _tenantId = 1;
        }

        #region Page 

        [HttpGet("Workflow")]
        public async Task<IActionResult> WorkFlowListAsync(string searchTerm = "",
        int page = 1,
        int pageSize = 10)
        {

            var result = await _workflowService.GetPagedWorkflowsAsync(page, pageSize, _tenantId, searchTerm);

            var viewModel = new PagedViewModel<Entities.Workflow>
            {
                Items = result.Data.Items,
                TotalCount = result.Data.TotalCount,
                PageSize = pageSize,
                CurrentPage = page
            };


            return View(viewModel);
      
        }

        [HttpGet("Workflow/Create")]
        public async Task<IActionResult> CreateAsync()
        {
            return View();
        }


        #endregion


        #region Handler


        #endregion


    }


    #region mapping
    public class MapperProfile : BaseProfile
    {
        public MapperProfile()
        {
            CreateMap<Entities.Workflow, WorkflowViewModel>();


            CreateMap<WorkflowViewModel, Entities.Workflow>();

        }
    }

    #endregion 
}
