using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using MorphoInventoryUI.Models;

namespace MorphoInventoryUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001/api";
        }

        #region Device Methods
        public async Task<List<Device>> GetAllDevicesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/device");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Device>>(content) ?? new List<Device>();
        }

        public async Task<List<Device>> GetAvailableDevicesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/device/available");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Device>>(content) ?? new List<Device>();
        }

        public async Task<Device?> GetDeviceBySerialNumberAsync(string serialNumber)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/device/{serialNumber}");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Device>(content);
        }

        public async Task<int> AddDeviceAsync(AddDeviceViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/device", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task<List<int>> AddDevicesBulkAsync(List<AddDeviceViewModel> devices)
        {
            var model = new { Devices = devices };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/device/bulk", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<int>>(responseContent) ?? new List<int>();
        }
        #endregion

        #region Order Methods
        public async Task<List<DeviceOrderRequest>> GetAllOrdersAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/order");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceOrderRequest>>(content) ?? new List<DeviceOrderRequest>();
        }

        public async Task<DeviceOrderRequest?> GetOrderByIdAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/order/{orderId}");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeviceOrderRequest>(content);
        }

        public async Task<List<DeviceOrderRequest>> GetOrdersByStatusAsync(string status)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/order/status/{status}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceOrderRequest>>(content) ?? new List<DeviceOrderRequest>();
        }

        public async Task<int> CreateOrderAsync(CreateOrderViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/order", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task ApproveOrderAsync(ApproveOrderViewModel model)
        {
            var approveModel = new
            {
                OrderId = model.OrderId,
                ApprovedBy = model.ApprovedBy,
                IsApproved = model.IsApproved
            };
            
            var json = JsonConvert.SerializeObject(approveModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/order/approve", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var response = await _httpClient.PutAsync($"{_baseUrl}/order/{orderId}/status/{status}", null);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Assignment Methods
        public async Task<List<DeviceAssignment>> GetAllAssignmentsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/assignment");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceAssignment>>(content) ?? new List<DeviceAssignment>();
        }

        public async Task<DeviceAssignment?> GetAssignmentByIdAsync(int assignmentId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/assignment/{assignmentId}");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeviceAssignment>(content);
        }

        public async Task<List<DeviceAssignment>> GetAssignmentsByAssigneeAsync(string assignedTo)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/assignment/assignee/{assignedTo}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceAssignment>>(content) ?? new List<DeviceAssignment>();
        }

        public async Task<List<DeviceAssignment>> GetAssignmentsByBranchAsync(string branch)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/assignment/branch/{branch}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceAssignment>>(content) ?? new List<DeviceAssignment>();
        }

        public async Task<DeviceAssignment?> GetAssignmentBySerialNumberAsync(string serialNumber)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/assignment/device/{serialNumber}");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeviceAssignment>(content);
        }

        public async Task<int> AssignDeviceToCoordinatorAsync(AssignDeviceToCoordinatorViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/assignment/coordinator", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task<int> AssignDeviceToBranchAsync(AssignDeviceToBranchViewModel model, string base64Image)
        {
            var assignModel = new
            {
                SerialNumber = model.SerialNumber,
                Creator = model.Creator,
                Branch = model.Branch,
                CsoId = model.CsoId,
                DeviceImage = base64Image
            };
            
            var json = JsonConvert.SerializeObject(assignModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/assignment/branch", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(responseContent);
        }
        #endregion

        #region Return Methods
        public async Task<List<DeviceReturnRequest>> GetAllReturnRequestsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/return");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceReturnRequest>>(content) ?? new List<DeviceReturnRequest>();
        }

        public async Task<DeviceReturnRequest?> GetReturnRequestByIdAsync(int requestId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/return/{requestId}");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeviceReturnRequest>(content);
        }

        public async Task<List<DeviceReturnRequest>> GetReturnRequestsByStatusAsync(string status)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/return/status/{status}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceReturnRequest>>(content) ?? new List<DeviceReturnRequest>();
        }

        public async Task<int> CreateReturnRequestAsync(DeviceReturnRequestViewModel model, string base64Image)
        {
            var returnModel = new
            {
                SerialNumber = model.SerialNumber,
                CsoId = model.CsoId,
                IssueType = model.IssueType,
                DeviceImage = base64Image,
                Remarks = model.Remarks
            };
            
            var json = JsonConvert.SerializeObject(returnModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/return", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task AcceptReturnRequestAsync(AcceptReturnViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/return/accept", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Reference Data Methods
        public async Task<List<IssueType>> GetIssueTypesAsync()
        {
            // In a real application, this would call an API endpoint
            // For now, we'll return mock data
            return new List<IssueType>
            {
                new IssueType { Id = 1, Name = "Damaged", IsActive = true },
                new IssueType { Id = 2, Name = "Not Working", IsActive = true },
                new IssueType { Id = 3, Name = "Battery Issue", IsActive = true },
                new IssueType { Id = 4, Name = "Screen Problem", IsActive = true },
                new IssueType { Id = 5, Name = "Software Issue", IsActive = true },
                new IssueType { Id = 6, Name = "Other", IsActive = true }
            };
        }

        public async Task<List<Coordinator>> GetCoordinatorsAsync()
        {
            // In a real application, this would call an API endpoint
            // For now, we'll return mock data
            return new List<Coordinator>
            {
                new Coordinator { Id = 1, Name = "John Doe", Email = "john.doe@example.com", IsActive = true },
                new Coordinator { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", IsActive = true },
                new Coordinator { Id = 3, Name = "Bob Johnson", Email = "bob.johnson@example.com", IsActive = true }
            };
        }

        public async Task<List<Branch>> GetBranchesAsync()
        {
            // In a real application, this would call an API endpoint
            // For now, we'll return mock data
            return new List<Branch>
            {
                new Branch { Id = 1, Name = "Branch A", Address = "123 Main St", CsoId = "CSO001", IsActive = true },
                new Branch { Id = 2, Name = "Branch B", Address = "456 Oak Ave", CsoId = "CSO002", IsActive = true },
                new Branch { Id = 3, Name = "Branch C", Address = "789 Pine Rd", CsoId = "CSO003", IsActive = true }
            };
        }

        public async Task<List<DeviceType>> GetDeviceTypesAsync()
        {
            // In a real application, this would call an API endpoint
            // For now, we'll return mock data
            return new List<DeviceType>
            {
                new DeviceType { Id = 1, Name = "Fingerprint Scanner", IsActive = true },
                new DeviceType { Id = 2, Name = "Biometric Device", IsActive = true },
                new DeviceType { Id = 3, Name = "Card Reader", IsActive = true }
            };
        }

        public async Task<List<Company>> GetCompaniesAsync()
        {
            // In a real application, this would call an API endpoint
            // For now, we'll return mock data
            return new List<Company>
            {
                new Company { Id = 1, Name = "Morpho", IsActive = true },
                new Company { Id = 2, Name = "SecuGen", IsActive = true },
                new Company { Id = 3, Name = "ZKTeco", IsActive = true }
            };
        }
        #endregion

        #region Helper Methods
        public async Task<string> ConvertImageToBase64Async(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }
        #endregion
    }
}

