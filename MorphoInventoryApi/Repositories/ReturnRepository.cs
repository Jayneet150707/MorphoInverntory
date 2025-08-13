using System.Data;
using MorphoInventoryApi.Data;
using MorphoInventoryApi.Models;

namespace MorphoInventoryApi.Repositories
{
    public class ReturnRepository
    {
        private readonly DatabaseContext _dbContext;

        public ReturnRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DeviceReturnRequest>> GetAllReturnRequestsAsync()
        {
            var query = "SELECT * FROM DeviceReturnRequests ORDER BY RequestDate DESC";
            var dataTable = await _dbContext.ExecuteQueryAsync(query);
            
            return ConvertDataTableToReturnRequestList(dataTable);
        }

        public async Task<DeviceReturnRequest?> GetReturnRequestByIdAsync(int requestId)
        {
            var query = "SELECT * FROM DeviceReturnRequests WHERE Id = @RequestId";
            var parameters = new Dictionary<string, object>
            {
                { "@RequestId", requestId }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            var requests = ConvertDataTableToReturnRequestList(dataTable);
            return requests.FirstOrDefault();
        }

        public async Task<List<DeviceReturnRequest>> GetReturnRequestsByStatusAsync(string status)
        {
            var query = "SELECT * FROM DeviceReturnRequests WHERE Status = @Status ORDER BY RequestDate DESC";
            var parameters = new Dictionary<string, object>
            {
                { "@Status", status }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            return ConvertDataTableToReturnRequestList(dataTable);
        }

        public async Task<DeviceReturnRequest?> GetReturnRequestBySerialNumberAsync(string serialNumber)
        {
            var query = "SELECT * FROM DeviceReturnRequests WHERE SerialNumber = @SerialNumber AND Status = 'Pending'";
            var parameters = new Dictionary<string, object>
            {
                { "@SerialNumber", serialNumber }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            var requests = ConvertDataTableToReturnRequestList(dataTable);
            return requests.FirstOrDefault();
        }

        public async Task<int> CreateReturnRequestAsync(DeviceReturnRequest request)
        {
            var query = @"
                INSERT INTO DeviceReturnRequests (DeviceId, SerialNumber, CsoId, IssueType, DeviceImage, Remarks, RequestDate, Status)
                VALUES (@DeviceId, @SerialNumber, @CsoId, @IssueType, @DeviceImage, @Remarks, @RequestDate, @Status);
                SELECT SCOPE_IDENTITY();";
            
            var parameters = new Dictionary<string, object>
            {
                { "@DeviceId", request.DeviceId },
                { "@SerialNumber", request.SerialNumber ?? string.Empty },
                { "@CsoId", request.CsoId ?? string.Empty },
                { "@IssueType", request.IssueType ?? string.Empty },
                { "@DeviceImage", request.DeviceImage ?? string.Empty },
                { "@Remarks", request.Remarks ?? string.Empty },
                { "@RequestDate", request.RequestDate },
                { "@Status", request.Status ?? "Pending" }
            };
            
            var result = await _dbContext.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result);
        }

        public async Task<bool> AcceptReturnRequestAsync(int requestId, string remarks)
        {
            var query = @"
                UPDATE DeviceReturnRequests 
                SET Status = 'Accepted', AcceptanceRemarks = @AcceptanceRemarks, AcceptanceDate = @AcceptanceDate
                WHERE Id = @RequestId";
            
            var parameters = new Dictionary<string, object>
            {
                { "@RequestId", requestId },
                { "@AcceptanceRemarks", remarks },
                { "@AcceptanceDate", DateTime.Now }
            };
            
            var rowsAffected = await _dbContext.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        private List<DeviceReturnRequest> ConvertDataTableToReturnRequestList(DataTable dataTable)
        {
            var requests = new List<DeviceReturnRequest>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                var request = new DeviceReturnRequest
                {
                    Id = Convert.ToInt32(row["Id"]),
                    DeviceId = Convert.ToInt32(row["DeviceId"]),
                    SerialNumber = row["SerialNumber"].ToString(),
                    CsoId = row["CsoId"].ToString(),
                    IssueType = row["IssueType"].ToString(),
                    DeviceImage = row["DeviceImage"].ToString(),
                    Remarks = row["Remarks"].ToString(),
                    RequestDate = Convert.ToDateTime(row["RequestDate"]),
                    Status = row["Status"].ToString()
                };
                
                if (row["AcceptanceRemarks"] != DBNull.Value)
                {
                    request.AcceptanceRemarks = row["AcceptanceRemarks"].ToString();
                }
                
                if (row["AcceptanceDate"] != DBNull.Value)
                {
                    request.AcceptanceDate = Convert.ToDateTime(row["AcceptanceDate"]);
                }
                
                requests.Add(request);
            }
            
            return requests;
        }
    }
}

