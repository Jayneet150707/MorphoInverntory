using MorphoInventoryApi.Models;
using MorphoInventoryApi.Repositories;

namespace MorphoInventoryApi.Services
{
    public class ReturnService
    {
        private readonly ReturnRepository _returnRepository;
        private readonly DeviceRepository _deviceRepository;
        private readonly AssignmentRepository _assignmentRepository;

        public ReturnService(
            ReturnRepository returnRepository,
            DeviceRepository deviceRepository,
            AssignmentRepository assignmentRepository)
        {
            _returnRepository = returnRepository;
            _deviceRepository = deviceRepository;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<List<DeviceReturnRequest>> GetAllReturnRequestsAsync()
        {
            return await _returnRepository.GetAllReturnRequestsAsync();
        }

        public async Task<DeviceReturnRequest?> GetReturnRequestByIdAsync(int requestId)
        {
            return await _returnRepository.GetReturnRequestByIdAsync(requestId);
        }

        public async Task<List<DeviceReturnRequest>> GetReturnRequestsByStatusAsync(string status)
        {
            return await _returnRepository.GetReturnRequestsByStatusAsync(status);
        }

        public async Task<int?> CreateReturnRequestAsync(DeviceReturnRequestModel request)
        {
            var device = await _deviceRepository.GetDeviceBySerialNumberAsync(request.SerialNumber);
            if (device == null || device.Status != "Assigned" || device.CurrentLocation != "Branch")
            {
                return null;
            }
            
            var assignment = await _assignmentRepository.GetAssignmentBySerialNumberAsync(request.SerialNumber);
            if (assignment == null || assignment.CsoId != request.CsoId)
            {
                return null;
            }
            
            var returnRequest = new DeviceReturnRequest
            {
                DeviceId = device.Id,
                SerialNumber = device.SerialNumber,
                CsoId = request.CsoId,
                IssueType = request.IssueType,
                DeviceImage = request.DeviceImage,
                Remarks = request.Remarks,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };
            
            return await _returnRepository.CreateReturnRequestAsync(returnRequest);
        }

        public async Task<bool> AcceptReturnRequestAsync(AcceptReturnRequest request)
        {
            var returnRequest = await _returnRepository.GetReturnRequestByIdAsync(request.ReturnRequestId);
            if (returnRequest == null || returnRequest.Status != "Pending")
            {
                return false;
            }
            
            var success = await _returnRepository.AcceptReturnRequestAsync(request.ReturnRequestId, request.Remarks);
            
            if (success)
            {
                // Update device status
                await _deviceRepository.UpdateDeviceStatusAsync(returnRequest.DeviceId, "Available", "Head Office");
                
                // Update assignment status
                var assignment = await _assignmentRepository.GetAssignmentBySerialNumberAsync(returnRequest.SerialNumber);
                if (assignment != null)
                {
                    await _assignmentRepository.UpdateAssignmentStatusAsync(assignment.Id, "Returned");
                }
            }
            
            return success;
        }
    }
}

