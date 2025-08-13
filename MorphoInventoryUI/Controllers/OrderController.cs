using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MorphoInventoryUI.Models;
using MorphoInventoryUI.Services;

namespace MorphoInventoryUI.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApiService apiService, ILogger<OrderController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _apiService.GetAllOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                TempData["ErrorMessage"] = "Failed to retrieve orders. Please try again later.";
                return View(new List<DeviceOrderRequest>());
            }
        }

        // GET: Order/Pending
        public async Task<IActionResult> Pending()
        {
            try
            {
                var orders = await _apiService.GetOrdersByStatusAsync("Pending");
                ViewBag.StatusFilter = "Pending";
                return View("Index", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending orders");
                TempData["ErrorMessage"] = "Failed to retrieve pending orders. Please try again later.";
                return View("Index", new List<DeviceOrderRequest>());
            }
        }

        // GET: Order/Approved
        public async Task<IActionResult> Approved()
        {
            try
            {
                var orders = await _apiService.GetOrdersByStatusAsync("Approved");
                ViewBag.StatusFilter = "Approved";
                return View("Index", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approved orders");
                TempData["ErrorMessage"] = "Failed to retrieve approved orders. Please try again later.";
                return View("Index", new List<DeviceOrderRequest>());
            }
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _apiService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details for {OrderId}", id);
                TempData["ErrorMessage"] = "Failed to retrieve order details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var branches = await _apiService.GetBranchesAsync();
                ViewBag.Branches = new SelectList(branches, "Name", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create order form");
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var orderId = await _apiService.CreateOrderAsync(model);
                    TempData["SuccessMessage"] = "Order created successfully.";
                    return RedirectToAction(nameof(Details), new { id = orderId });
                }

                var branches = await _apiService.GetBranchesAsync();
                ViewBag.Branches = new SelectList(branches, "Name", "Name");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                TempData["ErrorMessage"] = "Failed to create order. Please try again later.";

                var branches = await _apiService.GetBranchesAsync();
                ViewBag.Branches = new SelectList(branches, "Name", "Name");
                return View(model);
            }
        }

        // GET: Order/Approve/5
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var order = await _apiService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                if (order.Status != "Pending")
                {
                    TempData["ErrorMessage"] = "Only pending orders can be approved.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var model = new ApproveOrderViewModel
                {
                    OrderId = id,
                    IsApproved = true
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approve order form for {OrderId}", id);
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Order/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(ApproveOrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.ApproveOrderAsync(model);
                    
                    var status = model.IsApproved ? "approved" : "rejected";
                    TempData["SuccessMessage"] = $"Order {status} successfully.";
                    
                    return RedirectToAction(nameof(Details), new { id = model.OrderId });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving order {OrderId}", model.OrderId);
                TempData["ErrorMessage"] = "Failed to approve order. Please try again later.";
                return View(model);
            }
        }

        // GET: Order/UpdateStatus/5
        public async Task<IActionResult> UpdateStatus(int id)
        {
            try
            {
                var order = await _apiService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                ViewBag.OrderId = id;
                ViewBag.CurrentStatus = order.Status;
                
                var statuses = new List<string> { "Pending", "Approved", "Rejected", "Completed" };
                ViewBag.Statuses = new SelectList(statuses.Where(s => s != order.Status));
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading update status form for {OrderId}", id);
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    ModelState.AddModelError("", "Status is required.");
                    
                    var order = await _apiService.GetOrderByIdAsync(orderId);
                    ViewBag.OrderId = orderId;
                    ViewBag.CurrentStatus = order?.Status;
                    
                    var statuses = new List<string> { "Pending", "Approved", "Rejected", "Completed" };
                    ViewBag.Statuses = new SelectList(statuses.Where(s => s != order?.Status));
                    
                    return View();
                }

                await _apiService.UpdateOrderStatusAsync(orderId, status);
                TempData["SuccessMessage"] = "Order status updated successfully.";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order {OrderId}", orderId);
                TempData["ErrorMessage"] = "Failed to update order status. Please try again later.";
                
                var order = await _apiService.GetOrderByIdAsync(orderId);
                ViewBag.OrderId = orderId;
                ViewBag.CurrentStatus = order?.Status;
                
                var statuses = new List<string> { "Pending", "Approved", "Rejected", "Completed" };
                ViewBag.Statuses = new SelectList(statuses.Where(s => s != order?.Status));
                
                return View();
            }
        }
    }
}

