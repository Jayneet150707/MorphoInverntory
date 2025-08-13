using MorphoInventoryApi.Models;
using MorphoInventoryApi.Repositories;

namespace MorphoInventoryApi.Services
{
    public class AssignmentService
    {
        private readonly AssignmentRepository _assignmentRepository;
        private readonly DeviceRepository _deviceRepository;

        public AssignmentService(AssignmentRepository assignmentRepository, DeviceRepository deviceRepository)
        {
            _assignmentRepository = assignmentRepository;
            _deviceRepository = deviceRepository;
        }

        public async Task<List<DeviceAssignment>> GetAllAssignmentsAsync()
        {
            return await _assignmentRepository.GetAllAssignmentsAsync();
        }

        public async Task<DeviceAssignment?> GetAssignmentByIdAsync(int assignmentId)
        {
            return await _assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        }

        public async Task<List<DeviceAssignment>> GetAssignmentsByAssigneeAsync(string assignedTo)
        {
            return await _assignmentRepository.GetAssignmentsByAssigneeAsync(assignedTo);
        }

        public async Task<List<DeviceAssignment>> GetAssignmentsByBranchAsync(string branch)
        {
            return await _assignmentRepository.GetAssignmentsByBranchAsync(branch);
        }

        public async Task<DeviceAssignment?> GetAssignmentBySerialNumberAsync(string serialNumber)
        {
            return await _assignmentRepository.GetAssignmentBySerialNumberAsync(serialNumber);
        }

        public async Task<int?> AssignDeviceToCoordinatorAsync(AssignDeviceToCoordinatorRequest request)
        {
            var device = await _deviceRepository.GetDeviceBySerialNumberAsync(request.SerialNumber);
            if (device == null || device.Status != "Available")
            {
                return null;
            }
            
            var assignment = new DeviceAssignment
            {
                DeviceId = device.Id,
                SerialNumber = device.SerialNumber,
                Company = device.Company,
                DeviceType = device.DeviceType,
                AssignedTo = request.AssignedTo,
                AssignmentDate = DateTime.Now,
                Status = "Active"
            };
            
            var assignmentId = await _assignmentRepository.CreateAssignmentAsync(assignment);
            
            if (assignmentId > 0)
            {
                await _deviceRepository.UpdateDeviceStatusAsync(device.Id, "Assigned", "Coordinator");
            }
            
            return assignmentId;
        }

        public async Task<int?> AssignDeviceToBranchAsync(AssignDeviceToBranchRequest request)
        {
            var device = await _deviceRepository.GetDeviceBySerialNumberAsync(request.SerialNumber);
            if (device == null || device.Status != "Assigned" || device.CurrentLocation != "Coordinator")
            {
                return null;
            }
            
            var assignment = new DeviceAssignment
            {
                DeviceId = device.Id,
                SerialNumber = device.SerialNumber,
                Company = device.Company,
                DeviceType = device.DeviceType,
                AssignedTo = "Branch",
                Creator = request.Creator,
                Branch = request.Branch,
                CsoId = request.CsoId,
                DeviceImage = request.DeviceImage,
                AssignmentDate = DateTime.Now,
                Status = "Active"
            };
            
            var assignmentId = await _assignmentRepository.CreateAssignmentAsync(assignment);
            
            if (assignmentId > 0)
            {
                await _deviceRepository.UpdateDeviceStatusAsync(device.Id, "Assigned", "Branch");
                
                // Update previous assignment to inactive
                var previousAssignment = await _assignmentRepository.GetAssignmentBySerialNumberAsync(request.SerialNumber);
                if (previousAssignment != null)
                {
                    await _assignmentRepository.UpdateAssignmentStatusAsync(previousAssignment.Id, "Inactive");
                }
            }
            
            return assignmentId;
        }
    }
}

