using MorphoInventoryApi.Models;
using MorphoInventoryApi.Repositories;

namespace MorphoInventoryApi.Services
{
    public class DeviceService
    {
        private readonly DeviceRepository _deviceRepository;

        public DeviceService(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            return await _deviceRepository.GetAllDevicesAsync();
        }

        public async Task<Device?> GetDeviceBySerialNumberAsync(string serialNumber)
        {
            return await _deviceRepository.GetDeviceBySerialNumberAsync(serialNumber);
        }

        public async Task<List<Device>> GetAvailableDevicesAsync()
        {
            return await _deviceRepository.GetAvailableDevicesAsync();
        }

        public async Task<int> AddDeviceAsync(AddDeviceRequest request)
        {
            var device = new Device
            {
                SerialNumber = request.SerialNumber,
                Company = request.Company,
                DeviceType = request.DeviceType,
                CreatedDate = DateTime.Now,
                CreatedBy = request.CreatedBy,
                Status = "Available",
                CurrentLocation = "Head Office"
            };
            
            return await _deviceRepository.AddDeviceAsync(device);
        }

        public async Task<List<int>> AddDevicesBulkAsync(BulkDeviceUploadRequest request)
        {
            var deviceIds = new List<int>();
            
            foreach (var deviceRequest in request.Devices)
            {
                var deviceId = await AddDeviceAsync(deviceRequest);
                deviceIds.Add(deviceId);
            }
            
            return deviceIds;
        }

        public async Task<bool> UpdateDeviceStatusAsync(int deviceId, string status, string location)
        {
            return await _deviceRepository.UpdateDeviceStatusAsync(deviceId, status, location);
        }
    }
}

