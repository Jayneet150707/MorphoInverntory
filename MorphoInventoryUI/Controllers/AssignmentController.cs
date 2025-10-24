using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MorphoInventoryUI.Models;
using MorphoInventoryUI.Services;

namespace MorphoInventoryUI.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AssignmentController> _logger;

        public AssignmentController(ApiService apiService, ILogger<AssignmentController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // GET: Assignment
        public async Task<IActionResult> Index()
        {
            try
            {
                var assignments = await _apiService.GetAllAssignmentsAsync();
                return View(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments");
                TempData["ErrorMessage"] = "Failed to retrieve assignments. Please try again later.";
                return View(new List<DeviceAssignment>());
            }
        }

        // GET: Assignment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var assignment = await _apiService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    return NotFound();
                }

                return View(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignment details for {AssignmentId}", id);
                TempData["ErrorMessage"] = "Failed to retrieve assignment details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Assignment/ByAssignee/name
        public async Task<IActionResult> ByAssignee(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var assignments = await _apiService.GetAssignmentsByAssigneeAsync(id);
                ViewBag.AssigneeFilter = id;
                return View("Index", assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for assignee {Assignee}", id);
                TempData["ErrorMessage"] = "Failed to retrieve assignments. Please try again later.";
                return View("Index", new List<DeviceAssignment>());
            }
        }

        // GET: Assignment/ByBranch/name
        public async Task<IActionResult> ByBranch(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var assignments = await _apiService.GetAssignmentsByBranchAsync(id);
                ViewBag.BranchFilter = id;
                return View("Index", assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for branch {Branch}", id);
                TempData["ErrorMessage"] = "Failed to retrieve assignments. Please try again later.";
                return View("Index", new List<DeviceAssignment>());
            }
        }

        // GET: Assignment/ByDevice/serialNumber
        public async Task<IActionResult> ByDevice(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var assignment = await _apiService.GetAssignmentBySerialNumberAsync(id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = $"No active assignment found for device with serial number {id}.";
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Details), new { id = assignment.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignment for device {SerialNumber}", id);
                TempData["ErrorMessage"] = "Failed to retrieve assignment. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Assignment/AssignToCoordinator
        public async Task<IActionResult> AssignToCoordinator()
        {
            try
            {
                var coordinators = await _apiService.GetCoordinatorsAsync();
                ViewBag.Coordinators = new SelectList(coordinators, "Name", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assign to coordinator form");
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Assignment/AssignToCoordinator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToCoordinator(AssignDeviceToCoordinatorViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var assignmentId = await _apiService.AssignDeviceToCoordinatorAsync(model);
                    TempData["SuccessMessage"] = "Device assigned to coordinator successfully.";
                    return RedirectToAction(nameof(Details), new { id = assignmentId });
                }

                var coordinators = await _apiService.GetCoordinatorsAsync();
                ViewBag.Coordinators = new SelectList(coordinators, "Name", "Name");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning device to coordinator");
                TempData["ErrorMessage"] = "Failed to assign device. Please try again later.";

                var coordinators = await _apiService.GetCoordinatorsAsync();
                ViewBag.Coordinators = new SelectList(coordinators, "Name", "Name");
                return View(model);
            }
        }

        // GET: Assignment/AssignToBranch
        public async Task<IActionResult> AssignToBranch()
        {
            try
            {
                var branches = await _apiService.GetBranchesAsync();
                ViewBag.Branches = new SelectList(branches, "Name", "Name");
                ViewBag.CsoIds = new SelectList(branches, "CsoId", "CsoId");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assign to branch form");
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Assignment/AssignToBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToBranch(AssignDeviceToBranchViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var base64Image = await _apiService.ConvertImageToBase64Async(model.DeviceImageFile);
                    var assignmentId = await _apiService.AssignDeviceToBranchAsync(model, base64Image);
                    TempData["SuccessMessage"] = "Device assigned to branch successfully.";
                    return RedirectToAction(nameof(Details), new { id = assignmentId });
                }

                var branches = await _apiService.GetBranchesAsync();
                ViewBag.Branches = new SelectList(branches, "Name", "Name");
                ViewBag.CsoIds = new SelectList(branches, "CsoId", "CsoId");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning device to branch");
                TempData["ErrorMessage"] = "Failed to assign device. Please try again later.";

                var branches = await _apiService.GetBranchesAsync();
                ViewBag.Branches = new SelectList(branches, "Name", "Name");
                ViewBag.CsoIds = new SelectList(branches, "CsoId", "CsoId");
                return View(model);
            }
        }
    }
}

