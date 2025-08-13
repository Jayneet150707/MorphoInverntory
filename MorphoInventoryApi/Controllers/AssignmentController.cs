using Microsoft.AspNetCore.Mvc;
using MorphoInventoryApi.Models;
using MorphoInventoryApi.Services;

namespace MorphoInventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly AssignmentService _assignmentService;

        public AssignmentController(AssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceAssignment>>> GetAllAssignments()
        {
            var assignments = await _assignmentService.GetAllAssignmentsAsync();
            return Ok(assignments);
        }

        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<DeviceAssignment>> GetAssignmentById(int assignmentId)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(assignmentId);
            
            if (assignment == null)
            {
                return NotFound($"Assignment with ID {assignmentId} not found");
            }
            
            return Ok(assignment);
        }

        [HttpGet("assignee/{assignedTo}")]
        public async Task<ActionResult<List<DeviceAssignment>>> GetAssignmentsByAssignee(string assignedTo)
        {
            var assignments = await _assignmentService.GetAssignmentsByAssigneeAsync(assignedTo);
            return Ok(assignments);
        }

        [HttpGet("branch/{branch}")]
        public async Task<ActionResult<List<DeviceAssignment>>> GetAssignmentsByBranch(string branch)
        {
            var assignments = await _assignmentService.GetAssignmentsByBranchAsync(branch);
            return Ok(assignments);
        }

        [HttpGet("device/{serialNumber}")]
        public async Task<ActionResult<DeviceAssignment>> GetAssignmentBySerialNumber(string serialNumber)
        {
            var assignment = await _assignmentService.GetAssignmentBySerialNumberAsync(serialNumber);
            
            if (assignment == null)
            {
                return NotFound($"No active assignment found for device with serial number {serialNumber}");
            }
            
            return Ok(assignment);
        }

        [HttpPost("coordinator")]
        public async Task<ActionResult<int>> AssignDeviceToCoordinator([FromBody] AssignDeviceToCoordinatorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var assignmentId = await _assignmentService.AssignDeviceToCoordinatorAsync(request);
            
            if (assignmentId == null)
            {
                return BadRequest($"Device with serial number {request.SerialNumber} is not available for assignment");
            }
            
            return CreatedAtAction(nameof(GetAssignmentById), new { assignmentId }, assignmentId);
        }

        [HttpPost("branch")]
        public async Task<ActionResult<int>> AssignDeviceToBranch([FromBody] AssignDeviceToBranchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var assignmentId = await _assignmentService.AssignDeviceToBranchAsync(request);
            
            if (assignmentId == null)
            {
                return BadRequest($"Device with serial number {request.SerialNumber} is not available for branch assignment");
            }
            
            return CreatedAtAction(nameof(GetAssignmentById), new { assignmentId }, assignmentId);
        }
    }
}

