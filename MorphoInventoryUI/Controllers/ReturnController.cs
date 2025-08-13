using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MorphoInventoryUI.Models;
using MorphoInventoryUI.Services;

namespace MorphoInventoryUI.Controllers
{
    public class ReturnController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<ReturnController> _logger;

        public ReturnController(ApiService apiService, ILogger<ReturnController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // GET: Return
        public async Task<IActionResult> Index()
        {
            try
            {
                var returnRequests = await _apiService.GetAllReturnRequestsAsync();
                return View(returnRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving return requests");
                TempData["ErrorMessage"] = "Failed to retrieve return requests. Please try again later.";
                return View(new List<DeviceReturnRequest>());
            }
        }

        // GET: Return/Pending
        public async Task<IActionResult> Pending()
        {
            try
            {
                var returnRequests = await _apiService.GetReturnRequestsByStatusAsync("Pending");
                ViewBag.StatusFilter = "Pending";
                return View("Index", returnRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending return requests");
                TempData["ErrorMessage"] = "Failed to retrieve pending return requests. Please try again later.";
                return View("Index", new List<DeviceReturnRequest>());
            }
        }

        // GET: Return/Accepted
        public async Task<IActionResult> Accepted()
        {
            try
            {
                var returnRequests = await _apiService.GetReturnRequestsByStatusAsync("Accepted");
                ViewBag.StatusFilter = "Accepted";
                return View("Index", returnRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving accepted return requests");
                TempData["ErrorMessage"] = "Failed to retrieve accepted return requests. Please try again later.";
                return View("Index", new List<DeviceReturnRequest>());
            }
        }

        // GET: Return/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var returnRequest = await _apiService.GetReturnRequestByIdAsync(id);
                if (returnRequest == null)
                {
                    return NotFound();
                }

                return View(returnRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving return request details for {RequestId}", id);
                TempData["ErrorMessage"] = "Failed to retrieve return request details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Return/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var issueTypes = await _apiService.GetIssueTypesAsync();
                ViewBag.IssueTypes = new SelectList(issueTypes, "Name", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create return request form");
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Return/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeviceReturnRequestViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var base64Image = await _apiService.ConvertImageToBase64Async(model.DeviceImageFile);
                    var requestId = await _apiService.CreateReturnRequestAsync(model, base64Image);
                    TempData["SuccessMessage"] = "Return request created successfully.";
                    return RedirectToAction(nameof(Details), new { id = requestId });
                }

                var issueTypes = await _apiService.GetIssueTypesAsync();
                ViewBag.IssueTypes = new SelectList(issueTypes, "Name", "Name");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating return request");
                TempData["ErrorMessage"] = "Failed to create return request. Please try again later.";

                var issueTypes = await _apiService.GetIssueTypesAsync();
                ViewBag.IssueTypes = new SelectList(issueTypes, "Name", "Name");
                return View(model);
            }
        }

        // GET: Return/Accept/5
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                var returnRequest = await _apiService.GetReturnRequestByIdAsync(id);
                if (returnRequest == null)
                {
                    return NotFound();
                }

                if (returnRequest.Status != "Pending")
                {
                    TempData["ErrorMessage"] = "Only pending return requests can be accepted.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var model = new AcceptReturnViewModel
                {
                    ReturnRequestId = id
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading accept return request form for {RequestId}", id);
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Return/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(AcceptReturnViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.AcceptReturnRequestAsync(model);
                    TempData["SuccessMessage"] = "Return request accepted successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.ReturnRequestId });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting return request {RequestId}", model.ReturnRequestId);
                TempData["ErrorMessage"] = "Failed to accept return request. Please try again later.";
                return View(model);
            }
        }
    }
}

