using Chronicle.Services;
using Chronicle.Web.Areas.WorkFlow.Views;
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
        }

        #region Page 

        [HttpGet("Workflow")]
        public IActionResult WorkFlowList()
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
