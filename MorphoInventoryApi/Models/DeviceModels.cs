using System.ComponentModel.DataAnnotations;

namespace MorphoInventoryApi.Models
{
    // Device Inventory Model
    public class Device
    {
        public int Id { get; set; }
        public string? SerialNumber { get; set; }
        public string? Company { get; set; }
        public string? DeviceType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Status { get; set; } // Available, Assigned, Returned, etc.
        public string? CurrentLocation { get; set; } // Branch, Coordinator, Head Office
    }

    // Device Order Request Model
    public class DeviceOrderRequest
    {
        public int Id { get; set; }
        public string? Creator { get; set; }
        public string? Branch { get; set; }
        public string? BranchAddress { get; set; }
        public string? PersonName { get; set; }
        public string? MobileNumber { get; set; }
        public int DeviceCount { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Status { get; set; } // Pending, Approved, Rejected, Completed
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }

    // Device Assignment Model
    public class DeviceAssignment
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string? SerialNumber { get; set; }
        public string? Company { get; set; }
        public string? DeviceType { get; set; }
        public string? AssignedTo { get; set; } // Coordinator or Branch
        public string? Creator { get; set; }
        public string? Branch { get; set; }
        public string? CsoId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string? DeviceImage { get; set; } // Path to uploaded image
        public string? Status { get; set; } // Active, Returned
    }

    // Device Return Request Model
    public class DeviceReturnRequest
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string? SerialNumber { get; set; }
        public string? CsoId { get; set; }
        public string? IssueType { get; set; }
        public string? DeviceImage { get; set; }
        public string? Remarks { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Status { get; set; } // Pending, Accepted
        public string? AcceptanceRemarks { get; set; }
        public DateTime? AcceptanceDate { get; set; }
    }

    // Request Models for API
    public class AddDeviceRequest
    {
        [Required]
        public string? Company { get; set; }
        
        [Required]
        public string? DeviceType { get; set; }
        
        [Required]
        public string? SerialNumber { get; set; }
        
        [Required]
        public string? CreatedBy { get; set; }
    }

    public class BulkDeviceUploadRequest
    {
        [Required]
        public List<AddDeviceRequest> Devices { get; set; } = new List<AddDeviceRequest>();
    }

    public class CreateOrderRequest
    {
        [Required]
        public string? Creator { get; set; }
        
        [Required]
        public string? Branch { get; set; }
        
        [Required]
        public string? BranchAddress { get; set; }
        
        [Required]
        public string? PersonName { get; set; }
        
        [Required]
        public string? MobileNumber { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Device count must be at least 1")]
        public int DeviceCount { get; set; }
    }

    public class ApproveOrderRequest
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public string? ApprovedBy { get; set; }
        
        [Required]
        public bool IsApproved { get; set; }
    }

    public class AssignDeviceToCoordinatorRequest
    {
        [Required]
        public string? SerialNumber { get; set; }
        
        [Required]
        public string? AssignedTo { get; set; } // Coordinator name or email
    }

    public class AssignDeviceToBranchRequest
    {
        [Required]
        public string? SerialNumber { get; set; }
        
        [Required]
        public string? Creator { get; set; }
        
        [Required]
        public string? Branch { get; set; }
        
        [Required]
        public string? CsoId { get; set; }
        
        [Required]
        public string? DeviceImage { get; set; } // Base64 encoded image or file path
    }

    public class DeviceReturnRequestModel
    {
        [Required]
        public string? SerialNumber { get; set; }
        
        [Required]
        public string? CsoId { get; set; }
        
        [Required]
        public string? IssueType { get; set; }
        
        [Required]
        public string? DeviceImage { get; set; }
        
        [Required]
        [MinLength(10, ErrorMessage = "Remarks must be at least 10 characters")]
        public string? Remarks { get; set; }
    }

    public class AcceptReturnRequest
    {
        [Required]
        public int ReturnRequestId { get; set; }
        
        [Required]
        [MinLength(10, ErrorMessage = "Remarks must be at least 10 characters")]
        public string? Remarks { get; set; }
    }
}

