using System.Data;
using MorphoInventoryApi.Data;
using MorphoInventoryApi.Models;

namespace MorphoInventoryApi.Repositories
{
    public class AssignmentRepository
    {
        private readonly DatabaseContext _dbContext;

        public AssignmentRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DeviceAssignment>> GetAllAssignmentsAsync()
        {
            var query = "SELECT * FROM DeviceAssignments ORDER BY AssignmentDate DESC";
            var dataTable = await _dbContext.ExecuteQueryAsync(query);
            
            return ConvertDataTableToAssignmentList(dataTable);
        }

        public async Task<DeviceAssignment?> GetAssignmentByIdAsync(int assignmentId)
        {
            var query = "SELECT * FROM DeviceAssignments WHERE Id = @AssignmentId";
            var parameters = new Dictionary<string, object>
            {
                { "@AssignmentId", assignmentId }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            var assignments = ConvertDataTableToAssignmentList(dataTable);
            return assignments.FirstOrDefault();
        }

        public async Task<List<DeviceAssignment>> GetAssignmentsByAssigneeAsync(string assignedTo)
        {
            var query = "SELECT * FROM DeviceAssignments WHERE AssignedTo = @AssignedTo AND Status = 'Active' ORDER BY AssignmentDate DESC";
            var parameters = new Dictionary<string, object>
            {
                { "@AssignedTo", assignedTo }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            return ConvertDataTableToAssignmentList(dataTable);
        }

        public async Task<List<DeviceAssignment>> GetAssignmentsByBranchAsync(string branch)
        {
            var query = "SELECT * FROM DeviceAssignments WHERE Branch = @Branch AND Status = 'Active' ORDER BY AssignmentDate DESC";
            var parameters = new Dictionary<string, object>
            {
                { "@Branch", branch }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            return ConvertDataTableToAssignmentList(dataTable);
        }

        public async Task<DeviceAssignment?> GetAssignmentBySerialNumberAsync(string serialNumber)
        {
            var query = "SELECT * FROM DeviceAssignments WHERE SerialNumber = @SerialNumber AND Status = 'Active'";
            var parameters = new Dictionary<string, object>
            {
                { "@SerialNumber", serialNumber }
            };
            
            var dataTable = await _dbContext.ExecuteQueryAsync(query, parameters);
            
            var assignments = ConvertDataTableToAssignmentList(dataTable);
            return assignments.FirstOrDefault();
        }

        public async Task<int> CreateAssignmentAsync(DeviceAssignment assignment)
        {
            var query = @"
                INSERT INTO DeviceAssignments (DeviceId, SerialNumber, Company, DeviceType, AssignedTo, Creator, Branch, CsoId, AssignmentDate, DeviceImage, Status)
                VALUES (@DeviceId, @SerialNumber, @Company, @DeviceType, @AssignedTo, @Creator, @Branch, @CsoId, @AssignmentDate, @DeviceImage, @Status);
                SELECT SCOPE_IDENTITY();";
            
            var parameters = new Dictionary<string, object>
            {
                { "@DeviceId", assignment.DeviceId },
                { "@SerialNumber", assignment.SerialNumber ?? string.Empty },
                { "@Company", assignment.Company ?? string.Empty },
                { "@DeviceType", assignment.DeviceType ?? string.Empty },
                { "@AssignedTo", assignment.AssignedTo ?? string.Empty },
                { "@Creator", assignment.Creator ?? string.Empty },
                { "@Branch", assignment.Branch ?? string.Empty },
                { "@CsoId", assignment.CsoId ?? string.Empty },
                { "@AssignmentDate", assignment.AssignmentDate },
                { "@DeviceImage", assignment.DeviceImage ?? string.Empty },
                { "@Status", assignment.Status ?? "Active" }
            };
            
            var result = await _dbContext.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAssignmentStatusAsync(int assignmentId, string status)
        {
            var query = @"
                UPDATE DeviceAssignments 
                SET Status = @Status
                WHERE Id = @AssignmentId";
            
            var parameters = new Dictionary<string, object>
            {
                { "@AssignmentId", assignmentId },
                { "@Status", status }
            };
            
            var rowsAffected = await _dbContext.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        private List<DeviceAssignment> ConvertDataTableToAssignmentList(DataTable dataTable)
        {
            var assignments = new List<DeviceAssignment>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                assignments.Add(new DeviceAssignment
                {
                    Id = Convert.ToInt32(row["Id"]),
                    DeviceId = Convert.ToInt32(row["DeviceId"]),
                    SerialNumber = row["SerialNumber"].ToString(),
                    Company = row["Company"].ToString(),
                    DeviceType = row["DeviceType"].ToString(),
                    AssignedTo = row["AssignedTo"].ToString(),
                    Creator = row["Creator"].ToString(),
                    Branch = row["Branch"].ToString(),
                    CsoId = row["CsoId"].ToString(),
                    AssignmentDate = Convert.ToDateTime(row["AssignmentDate"]),
                    DeviceImage = row["DeviceImage"].ToString(),
                    Status = row["Status"].ToString()
                });
            }
            
            return assignments;
        }
    }
}

