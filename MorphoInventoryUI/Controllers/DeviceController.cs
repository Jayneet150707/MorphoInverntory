using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MorphoInventoryUI.Models;
using MorphoInventoryUI.Services;

namespace MorphoInventoryUI.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(ApiService apiService, ILogger<DeviceController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // GET: Device
        public async Task<IActionResult> Index()
        {
            try
            {
                var devices = await _apiService.GetAllDevicesAsync();
                return View(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving devices");
                TempData["ErrorMessage"] = "Failed to retrieve devices. Please try again later.";
                return View(new List<Device>());
            }
        }

        // GET: Device/Available
        public async Task<IActionResult> Available()
        {
            try
            {
                var devices = await _apiService.GetAvailableDevicesAsync();
                return View("Index", devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available devices");
                TempData["ErrorMessage"] = "Failed to retrieve available devices. Please try again later.";
                return View("Index", new List<Device>());
            }
        }

        // GET: Device/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var device = await _apiService.GetDeviceBySerialNumberAsync(id);
                if (device == null)
                {
                    return NotFound();
                }

                return View(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device details for {SerialNumber}", id);
                TempData["ErrorMessage"] = "Failed to retrieve device details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Device/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var companies = await _apiService.GetCompaniesAsync();
                var deviceTypes = await _apiService.GetDeviceTypesAsync();

                ViewBag.Companies = new SelectList(companies, "Name", "Name");
                ViewBag.DeviceTypes = new SelectList(deviceTypes, "Name", "Name");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create device form");
                TempData["ErrorMessage"] = "Failed to load form. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Device/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddDeviceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var deviceId = await _apiService.AddDeviceAsync(model);
                    TempData["SuccessMessage"] = "Device added successfully.";
                    return RedirectToAction(nameof(Index));
                }

                var companies = await _apiService.GetCompaniesAsync();
                var deviceTypes = await _apiService.GetDeviceTypesAsync();

                ViewBag.Companies = new SelectList(companies, "Name", "Name");
                ViewBag.DeviceTypes = new SelectList(deviceTypes, "Name", "Name");

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating device");
                TempData["ErrorMessage"] = "Failed to create device. Please try again later.";

                var companies = await _apiService.GetCompaniesAsync();
                var deviceTypes = await _apiService.GetDeviceTypesAsync();

                ViewBag.Companies = new SelectList(companies, "Name", "Name");
                ViewBag.DeviceTypes = new SelectList(deviceTypes, "Name", "Name");

                return View(model);
            }
        }

        // GET: Device/BulkUpload
        public IActionResult BulkUpload()
        {
            return View();
        }

        // POST: Device/BulkUpload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpload(BulkDeviceUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ExcelFile != null && model.ExcelFile.Length > 0)
                    {
                        // In a real application, you would parse the Excel file
                        // For now, we'll just create a few sample devices
                        var devices = new List<AddDeviceViewModel>
                        {
                            new AddDeviceViewModel { Company = "Morpho", DeviceType = "Fingerprint Scanner", SerialNumber = "SN001", CreatedBy = model.CreatedBy },
                            new AddDeviceViewModel { Company = "Morpho", DeviceType = "Fingerprint Scanner", SerialNumber = "SN002", CreatedBy = model.CreatedBy },
                            new AddDeviceViewModel { Company = "Morpho", DeviceType = "Fingerprint Scanner", SerialNumber = "SN003", CreatedBy = model.CreatedBy }
                        };

                        var deviceIds = await _apiService.AddDevicesBulkAsync(devices);
                        TempData["SuccessMessage"] = $"{deviceIds.Count} devices added successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading devices in bulk");
                    TempData["ErrorMessage"] = "Failed to upload devices. Please try again later.";
                }
            }

            return View(model);
        }
    }
}

