using System.Data;
using MorphoInventoryApi.Data;
using MorphoInventoryApi.Models;

namespace MorphoInventoryApi.Repositories
{
    public class DeviceRepository
    {
        private readonly DatabaseContext _dbContext;

        public DeviceRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            var query = "SELECT * FROM Devices";
            var dataTable = await _dbContext.ExecuteQueryAsync(query);
            
            return ConvertDataTableToDeviceList(dataTable);
        }

        public async Task<Device?> GetDeviceBySerialNumberAsync(string serialNumber)
        {
            var query = "SELECT * FROM Devices WHERE SerialNumber = @SerialNumber";
            var parameters = new Dictionary<string, object>
            {
                { "@SerialNumber", serialNumber }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            var devices = ConvertDataTableToDeviceList(dataTable);
            return devices.FirstOrDefault();
        }

        public async Task<int> AddDeviceAsync(Device device)
        {
            var query = @"
                INSERT INTO Devices (SerialNumber, Company, DeviceType, CreatedDate, CreatedBy, Status, CurrentLocation)
                VALUES (@SerialNumber, @Company, @DeviceType, @CreatedDate, @CreatedBy, @Status, @CurrentLocation);
                SELECT SCOPE_IDENTITY();";
            
            var parameters = new Dictionary<string, object>
            {
                { "@SerialNumber", device.SerialNumber ?? string.Empty },
                { "@Company", device.Company ?? string.Empty },
                { "@DeviceType", device.DeviceType ?? string.Empty },
                { "@CreatedDate", device.CreatedDate },
                { "@CreatedBy", device.CreatedBy ?? string.Empty },
                { "@Status", device.Status ?? "Available" },
                { "@CurrentLocation", device.CurrentLocation ?? "Head Office" }
            };
            
            var result = await _dbContext.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateDeviceStatusAsync(int deviceId, string status, string location)
        {
            var query = @"
                UPDATE Devices 
                SET Status = @Status, CurrentLocation = @CurrentLocation
                WHERE Id = @DeviceId";
            
            var parameters = new Dictionary<string, object>
            {
                { "@DeviceId", deviceId },
                { "@Status", status },
                { "@CurrentLocation", location }
            };
            
            var rowsAffected = await _dbContext.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<List<Device>> GetAvailableDevicesAsync()
        {
            var query = "SELECT * FROM Devices WHERE Status = 'Available'";
            var dataTable = await _dbContext.ExecuteQueryAsync(query);
            
            return ConvertDataTableToDeviceList(dataTable);
        }

        private List<Device> ConvertDataTableToDeviceList(DataTable dataTable)
        {
            var devices = new List<Device>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                devices.Add(new Device
                {
                    Id = Convert.ToInt32(row["Id"]),
                    SerialNumber = row["SerialNumber"].ToString(),
                    Company = row["Company"].ToString(),
                    DeviceType = row["DeviceType"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    CreatedBy = row["CreatedBy"].ToString(),
                    Status = row["Status"].ToString(),
                    CurrentLocation = row["CurrentLocation"].ToString()
                });
            }
            
            return devices;
        }
    }
}

