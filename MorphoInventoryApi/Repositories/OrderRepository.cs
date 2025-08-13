using System.Data;
using MorphoInventoryApi.Data;
using MorphoInventoryApi.Models;

namespace MorphoInventoryApi.Repositories
{
    public class OrderRepository
    {
        private readonly DatabaseContext _dbContext;

        public OrderRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DeviceOrderRequest>> GetAllOrdersAsync()
        {
            var query = "SELECT * FROM DeviceOrderRequests ORDER BY RequestDate DESC";
            var dataTable = await _dbContext.ExecuteQueryAsync(query);
            
            return ConvertDataTableToOrderList(dataTable);
        }

        public async Task<DeviceOrderRequest?> GetOrderByIdAsync(int orderId)
        {
            var query = "SELECT * FROM DeviceOrderRequests WHERE Id = @OrderId";
            var parameters = new Dictionary<string, object>
            {
                { "@OrderId", orderId }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            var orders = ConvertDataTableToOrderList(dataTable);
            return orders.FirstOrDefault();
        }

        public async Task<List<DeviceOrderRequest>> GetOrdersByStatusAsync(string status)
        {
            var query = "SELECT * FROM DeviceOrderRequests WHERE Status = @Status ORDER BY RequestDate DESC";
            var parameters = new Dictionary<string, object>
            {
                { "@Status", status }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            return ConvertDataTableToOrderList(dataTable);
        }

        public async Task<int> CreateOrderAsync(DeviceOrderRequest order)
        {
            var query = @"
                INSERT INTO DeviceOrderRequests (Creator, Branch, BranchAddress, PersonName, MobileNumber, DeviceCount, RequestDate, Status)
                VALUES (@Creator, @Branch, @BranchAddress, @PersonName, @MobileNumber, @DeviceCount, @RequestDate, @Status);
                SELECT SCOPE_IDENTITY();";
            
            var parameters = new Dictionary<string, object>
            {
                { "@Creator", order.Creator ?? string.Empty },
                { "@Branch", order.Branch ?? string.Empty },
                { "@BranchAddress", order.BranchAddress ?? string.Empty },
                { "@PersonName", order.PersonName ?? string.Empty },
                { "@MobileNumber", order.MobileNumber ?? string.Empty },
                { "@DeviceCount", order.DeviceCount },
                { "@RequestDate", order.RequestDate },
                { "@Status", order.Status ?? "Pending" }
            };
            
            var result = await _dbContext.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? approvedBy = null)
        {
            var query = @"
                UPDATE DeviceOrderRequests 
                SET Status = @Status";
            
            var parameters = new Dictionary<string, object>
            {
                { "@OrderId", orderId },
                { "@Status", status }
            };
            
            if (!string.IsNullOrEmpty(approvedBy))
            {
                query += @", ApprovedBy = @ApprovedBy, ApprovalDate = @ApprovalDate";
                parameters.Add("@ApprovedBy", approvedBy);
                parameters.Add("@ApprovalDate", DateTime.Now);
            }
            
            query += " WHERE Id = @OrderId";
            
            var rowsAffected = await _dbContext.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        private List<DeviceOrderRequest> ConvertDataTableToOrderList(DataTable dataTable)
        {
            var orders = new List<DeviceOrderRequest>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                var order = new DeviceOrderRequest
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Creator = row["Creator"].ToString(),
                    Branch = row["Branch"].ToString(),
                    BranchAddress = row["BranchAddress"].ToString(),
                    PersonName = row["PersonName"].ToString(),
                    MobileNumber = row["MobileNumber"].ToString(),
                    DeviceCount = Convert.ToInt32(row["DeviceCount"]),
                    RequestDate = Convert.ToDateTime(row["RequestDate"]),
                    Status = row["Status"].ToString()
                };
                
                if (row["ApprovedBy"] != DBNull.Value)
                {
                    order.ApprovedBy = row["ApprovedBy"].ToString();
                }
                
                if (row["ApprovalDate"] != DBNull.Value)
                {
                    order.ApprovalDate = Convert.ToDateTime(row["ApprovalDate"]);
                }
                
                orders.Add(order);
            }
            
            return orders;
        }
    }
}

