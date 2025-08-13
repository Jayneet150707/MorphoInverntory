using System.ComponentModel.DataAnnotations;

namespace MorphoInventoryUI.Models
{
    // Device Inventory Model
    public class Device
    {
        public int Id { get; set; }
        
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }
        
        [Display(Name = "Company/Brand")]
        public string? Company { get; set; }
        
        [Display(Name = "Device Type")]
        public string? DeviceType { get; set; }
        
        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
        
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        
        [Display(Name = "Status")]
        public string? Status { get; set; } // Available, Assigned, Returned, etc.
        
        [Display(Name = "Current Location")]
        public string? CurrentLocation { get; set; } // Branch, Coordinator, Head Office
    }

    // Device Order Request Model
    public class DeviceOrderRequest
    {
        public int Id { get; set; }
        
        [Display(Name = "Creator")]
        public string? Creator { get; set; }
        
        [Display(Name = "Branch")]
        public string? Branch { get; set; }
        
        [Display(Name = "Branch Address")]
        public string? BranchAddress { get; set; }
        
        [Display(Name = "Person Name")]
        public string? PersonName { get; set; }
        
        [Display(Name = "Mobile Number")]
        public string? MobileNumber { get; set; }
        
        [Display(Name = "Device Count")]
        public int DeviceCount { get; set; }
        
        [Display(Name = "Request Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        
        [Display(Name = "Status")]
        public string? Status { get; set; } // Pending, Approved, Rejected, Completed
        
        [Display(Name = "Approved By")]
        public string? ApprovedBy { get; set; }
        
        [Display(Name = "Approval Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovalDate { get; set; }
    }

    // Device Assignment Model
    public class DeviceAssignment
    {
        public int Id { get; set; }
        
        [Display(Name = "Device ID")]
        public int DeviceId { get; set; }
        
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }
        
        [Display(Name = "Company/Brand")]
        public string? Company { get; set; }
        
        [Display(Name = "Device Type")]
        public string? DeviceType { get; set; }
        
        [Display(Name = "Assigned To")]
        public string? AssignedTo { get; set; } // Coordinator or Branch
        
        [Display(Name = "Creator")]
        public string? Creator { get; set; }
        
        [Display(Name = "Branch")]
        public string? Branch { get; set; }
        
        [Display(Name = "CSO ID")]
        public string? CsoId { get; set; }
        
        [Display(Name = "Assignment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AssignmentDate { get; set; }
        
        [Display(Name = "Device Image")]
        public string? DeviceImage { get; set; } // Path to uploaded image
        
        [Display(Name = "Status")]
        public string? Status { get; set; } // Active, Returned
    }

    // Device Return Request Model
    public class DeviceReturnRequest
    {
        public int Id { get; set; }
        
        [Display(Name = "Device ID")]
        public int DeviceId { get; set; }
        
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }
        
        [Display(Name = "CSO ID")]
        public string? CsoId { get; set; }
        
        [Display(Name = "Issue Type")]
        public string? IssueType { get; set; }
        
        [Display(Name = "Device Image")]
        public string? DeviceImage { get; set; }
        
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
        
        [Display(Name = "Request Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        
        [Display(Name = "Status")]
        public string? Status { get; set; } // Pending, Accepted
        
        [Display(Name = "Acceptance Remarks")]
        public string? AcceptanceRemarks { get; set; }
        
        [Display(Name = "Acceptance Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? AcceptanceDate { get; set; }
    }

    // Request Models for UI Forms
    public class AddDeviceViewModel
    {
        [Required]
        [Display(Name = "Company/Brand")]
        public string? Company { get; set; }
        
        [Required]
        [Display(Name = "Device Type")]
        public string? DeviceType { get; set; }
        
        [Required]
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }
        
        [Required]
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
    }

    public class BulkDeviceUploadViewModel
    {
        [Required]
        [Display(Name = "Excel File")]
        public IFormFile? ExcelFile { get; set; }
        
        [Required]
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
    }

    public class CreateOrderViewModel
    {
        [Required]
        [Display(Name = "Creator")]
        public string? Creator { get; set; }
        
        [Required]
        [Display(Name = "Branch")]
        public string? Branch { get; set; }
        
        [Required]
        [Display(Name = "Branch Address")]
        public string? BranchAddress { get; set; }
        
        [Required]
        [Display(Name = "Person Name")]
        public string? PersonName { get; set; }
        
        [Required]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string? MobileNumber { get; set; }
        
        [Required]
        [Display(Name = "Device Count")]
        [Range(1, int.MaxValue, ErrorMessage = "Device count must be at least 1")]
        public int DeviceCount { get; set; }
    }

    public class ApproveOrderViewModel
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        [Display(Name = "Approved By")]
        public string? ApprovedBy { get; set; }
        
        [Required]
        [Display(Name = "Approve")]
        public bool IsApproved { get; set; }
        
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }

    public class AssignDeviceToCoordinatorViewModel
    {
        [Required]
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }
        
        [Required]
        [Display(Name = "Coordinator")]
        public string? AssignedTo { get; set; } // Coordinator name or email
    }

    public class AssignDeviceToBranchViewModel
    {
        [Required]
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }
        
        [Required]
        [Display(Name = "Creator")]
        public string? Creator { get; set; }
        
        [Required]
        [Display(Name = "Branch")]
        public string? Branch { get; set; }
        
        [Required]
        [Display(Name = "CSO ID")]
        public string? CsoId { get; set; }
        
        [Required]
        [Display(Name = "Device Image")]
        public IFormFile? DeviceImageFile { get; set; }
    }

    public class DeviceReturnRequestViewModel
    {
        [Required]
        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }
        
        [Required]
        [Display(Name = "CSO ID")]
        public string? CsoId { get; set; }
        
        [Required]
        [Display(Name = "Issue Type")]
        public string? IssueType { get; set; }
        
        [Required]
        [Display(Name = "Device Image")]
        public IFormFile? DeviceImageFile { get; set; }
        
        [Required]
        [MinLength(10, ErrorMessage = "Remarks must be at least 10 characters")]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }

    public class AcceptReturnViewModel
    {
        [Required]
        public int ReturnRequestId { get; set; }
        
        [Required]
        [MinLength(10, ErrorMessage = "Remarks must be at least 10 characters")]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }

    // Dropdown data models
    public class IssueType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class Coordinator
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class Branch
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? CsoId { get; set; }
        public bool IsActive { get; set; }
    }

    public class DeviceType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
    }
}

