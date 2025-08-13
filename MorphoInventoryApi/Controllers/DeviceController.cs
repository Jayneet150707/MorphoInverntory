using Microsoft.AspNetCore.Mvc;
using MorphoInventoryApi.Models;
using MorphoInventoryApi.Services;

namespace MorphoInventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _deviceService;

        public DeviceController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Device>>> GetAllDevices()
        {
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("available")]
        public async Task<ActionResult<List<Device>>> GetAvailableDevices()
        {
            var devices = await _deviceService.GetAvailableDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("{serialNumber}")]
        public async Task<ActionResult<Device>> GetDeviceBySerialNumber(string serialNumber)
        {
            var device = await _deviceService.GetDeviceBySerialNumberAsync(serialNumber);
            
            if (device == null)
            {
                return NotFound($"Device with serial number {serialNumber} not found");
            }
            
            return Ok(device);
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddDevice([FromBody] AddDeviceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var existingDevice = await _deviceService.GetDeviceBySerialNumberAsync(request.SerialNumber);
            if (existingDevice != null)
            {
                return Conflict($"Device with serial number {request.SerialNumber} already exists");
            }
            
            var deviceId = await _deviceService.AddDeviceAsync(request);
            return CreatedAtAction(nameof(GetDeviceBySerialNumber), new { serialNumber = request.SerialNumber }, deviceId);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<List<int>>> AddDevicesBulk([FromBody] BulkDeviceUploadRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var deviceIds = await _deviceService.AddDevicesBulkAsync(request);
            return Ok(deviceIds);
        }
    }
}

