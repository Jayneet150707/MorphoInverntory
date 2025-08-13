-- Create Database
CREATE DATABASE MorphoInventory;
GO

USE MorphoInventory;
GO

-- Create Devices Table
CREATE TABLE Devices (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SerialNumber NVARCHAR(100) NOT NULL UNIQUE,
    Company NVARCHAR(100) NOT NULL,
    DeviceType NVARCHAR(100) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    CreatedBy NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- Available, Assigned, Returned
    CurrentLocation NVARCHAR(100) NOT NULL -- Head Office, Coordinator, Branch
);

-- Create Device Order Requests Table
CREATE TABLE DeviceOrderRequests (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Creator NVARCHAR(100) NOT NULL,
    Branch NVARCHAR(100) NOT NULL,
    BranchAddress NVARCHAR(255) NOT NULL,
    PersonName NVARCHAR(100) NOT NULL,
    MobileNumber NVARCHAR(20) NOT NULL,
    DeviceCount INT NOT NULL,
    RequestDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- Pending, Approved, Rejected, Completed
    ApprovedBy NVARCHAR(100) NULL,
    ApprovalDate DATETIME NULL
);

-- Create Device Assignments Table
CREATE TABLE DeviceAssignments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DeviceId INT NOT NULL,
    SerialNumber NVARCHAR(100) NOT NULL,
    Company NVARCHAR(100) NOT NULL,
    DeviceType NVARCHAR(100) NOT NULL,
    AssignedTo NVARCHAR(100) NOT NULL, -- Coordinator or Branch
    Creator NVARCHAR(100) NULL,
    Branch NVARCHAR(100) NULL,
    CsoId NVARCHAR(100) NULL,
    AssignmentDate DATETIME NOT NULL,
    DeviceImage NVARCHAR(MAX) NULL, -- Path to uploaded image or Base64
    Status NVARCHAR(50) NOT NULL, -- Active, Inactive, Returned
    FOREIGN KEY (DeviceId) REFERENCES Devices(Id)
);

-- Create Device Return Requests Table
CREATE TABLE DeviceReturnRequests (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DeviceId INT NOT NULL,
    SerialNumber NVARCHAR(100) NOT NULL,
    CsoId NVARCHAR(100) NOT NULL,
    IssueType NVARCHAR(100) NOT NULL,
    DeviceImage NVARCHAR(MAX) NOT NULL, -- Path to uploaded image or Base64
    Remarks NVARCHAR(500) NOT NULL,
    RequestDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- Pending, Accepted
    AcceptanceRemarks NVARCHAR(500) NULL,
    AcceptanceDate DATETIME NULL,
    FOREIGN KEY (DeviceId) REFERENCES Devices(Id)
);

-- Create IssueTypes Table for dropdown values
CREATE TABLE IssueTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Insert default issue types
INSERT INTO IssueTypes (Name) VALUES 
('Damaged'),
('Not Working'),
('Battery Issue'),
('Screen Problem'),
('Software Issue'),
('Other');

-- Create Coordinators Table
CREATE TABLE Coordinators (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Create Branches Table
CREATE TABLE Branches (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    CsoId NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Create DeviceTypes Table for dropdown values
CREATE TABLE DeviceTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Insert default device types
INSERT INTO DeviceTypes (Name) VALUES 
('Fingerprint Scanner'),
('Biometric Device'),
('Card Reader');

-- Create Companies (Brands) Table for dropdown values
CREATE TABLE Companies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Insert default companies
INSERT INTO Companies (Name) VALUES 
('Morpho'),
('SecuGen'),
('ZKTeco');

-- Create Users Table for authentication
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Role NVARCHAR(50) NOT NULL, -- Admin, VP, Coordinator, EVP, ITSupport
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL
);

-- Create Audit Log Table for tracking changes
CREATE TABLE AuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    Username NVARCHAR(100) NULL,
    Action NVARCHAR(100) NOT NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId INT NULL,
    Details NVARCHAR(MAX) NULL,
    Timestamp DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Create Indexes for better performance
CREATE INDEX IX_Devices_Status ON Devices(Status);
CREATE INDEX IX_Devices_CurrentLocation ON Devices(CurrentLocation);
CREATE INDEX IX_DeviceOrderRequests_Status ON DeviceOrderRequests(Status);
CREATE INDEX IX_DeviceAssignments_DeviceId ON DeviceAssignments(DeviceId);
CREATE INDEX IX_DeviceAssignments_SerialNumber ON DeviceAssignments(SerialNumber);
CREATE INDEX IX_DeviceAssignments_Status ON DeviceAssignments(Status);
CREATE INDEX IX_DeviceReturnRequests_DeviceId ON DeviceReturnRequests(DeviceId);
CREATE INDEX IX_DeviceReturnRequests_SerialNumber ON DeviceReturnRequests(SerialNumber);
CREATE INDEX IX_DeviceReturnRequests_Status ON DeviceReturnRequests(Status);

GO

