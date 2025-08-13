-- MorphoInventorySchema.sql
-- SQL Schema for Morpho Inventory Management System

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MorphoInventory')
BEGIN
    CREATE DATABASE MorphoInventory;
END
GO

USE MorphoInventory;
GO

-- Reference Tables
-- Companies (Device Manufacturers)
CREATE TABLE Companies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Device Types
CREATE TABLE DeviceTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Branches
CREATE TABLE Branches (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CsoId NVARCHAR(50) NOT NULL UNIQUE,
    Address NVARCHAR(255),
    ContactPerson NVARCHAR(100),
    ContactNumber NVARCHAR(20),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Coordinators
CREATE TABLE Coordinators (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    ContactNumber NVARCHAR(20),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Issue Types for Device Returns
CREATE TABLE IssueTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Role NVARCHAR(50) NOT NULL, -- VP, Coordinator, EVP, ITSupport, HeadOffice
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    LastLoginDate DATETIME
);
GO

-- Core Tables
-- Devices
CREATE TABLE Devices (
    SerialNumber NVARCHAR(100) PRIMARY KEY,
    CompanyId INT NOT NULL REFERENCES Companies(Id),
    DeviceTypeId INT NOT NULL REFERENCES DeviceTypes(Id),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Available', -- Available, Assigned, Returned, Defective
    CurrentLocation NVARCHAR(50), -- Inventory, Coordinator, Branch, HeadOffice
    CreatedBy NVARCHAR(100) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedDate DATETIME,
    LastUpdatedBy NVARCHAR(100)
);
GO

-- Device Order Requests
CREATE TABLE DeviceOrderRequests (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Creator NVARCHAR(100) NOT NULL,
    Branch NVARCHAR(100) NOT NULL,
    BranchAddress NVARCHAR(255),
    PersonName NVARCHAR(100),
    MobileNumber NVARCHAR(20),
    DeviceCount INT NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected, InProcess, Completed
    RequestDate DATETIME NOT NULL DEFAULT GETDATE(),
    ApprovedBy NVARCHAR(100),
    ApprovalDate DATETIME,
    Remarks NVARCHAR(255),
    VendorId INT,
    OrderDate DATETIME,
    CompletionDate DATETIME
);
GO

-- Device Assignments
CREATE TABLE DeviceAssignments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SerialNumber NVARCHAR(100) NOT NULL REFERENCES Devices(SerialNumber),
    AssignedTo NVARCHAR(100) NOT NULL, -- Coordinator Name
    Branch NVARCHAR(100), -- Branch Name (if assigned to branch)
    CsoId NVARCHAR(50), -- CSO ID (if assigned to branch)
    Creator NVARCHAR(100), -- Person who assigned to branch
    DeviceImage NVARCHAR(MAX), -- Base64 encoded image (if assigned to branch)
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Active, Returned
    AssignmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    ReturnDate DATETIME,
    CreatedBy NVARCHAR(100) NOT NULL,
    LastUpdatedDate DATETIME,
    LastUpdatedBy NVARCHAR(100)
);
GO

-- Device Return Requests
CREATE TABLE DeviceReturnRequests (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SerialNumber NVARCHAR(100) NOT NULL REFERENCES Devices(SerialNumber),
    CsoId NVARCHAR(50) NOT NULL,
    IssueType NVARCHAR(100) NOT NULL,
    DeviceImage NVARCHAR(MAX) NOT NULL, -- Base64 encoded image
    Remarks NVARCHAR(255) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Accepted
    RequestDate DATETIME NOT NULL DEFAULT GETDATE(),
    AcceptanceDate DATETIME,
    AcceptanceRemarks NVARCHAR(255),
    AcceptedBy NVARCHAR(100)
);
GO

-- Audit Logs
CREATE TABLE AuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EntityType NVARCHAR(50) NOT NULL, -- Device, Order, Assignment, Return
    EntityId NVARCHAR(100) NOT NULL, -- Primary key of the entity
    Action NVARCHAR(50) NOT NULL, -- Create, Update, Delete, StatusChange
    OldValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    UserId INT REFERENCES Users(Id),
    Username NVARCHAR(50) NOT NULL,
    ActionDate DATETIME NOT NULL DEFAULT GETDATE(),
    IPAddress NVARCHAR(50)
);
GO

-- Insert Initial Reference Data
-- Companies
INSERT INTO Companies (Name) VALUES 
('Morpho'),
('SecuGen'),
('ZKTeco'),
('Mantra'),
('Startek');
GO

-- Device Types
INSERT INTO DeviceTypes (Name, Description) VALUES 
('Fingerprint Scanner', 'Biometric fingerprint scanning device'),
('Card Reader', 'RFID card reading device'),
('Biometric Device', 'Multi-modal biometric device'),
('Iris Scanner', 'Iris recognition device'),
('Face Recognition Device', 'Facial recognition device');
GO

-- Issue Types
INSERT INTO IssueTypes (Name, Description) VALUES 
('Defective', 'Device is not functioning properly'),
('Damaged', 'Device has physical damage'),
('Replacement', 'Device needs to be replaced'),
('Upgrade', 'Device needs to be upgraded'),
('Not Required', 'Device is no longer required');
GO

-- Create Stored Procedures
-- Add Device
CREATE PROCEDURE sp_AddDevice
    @SerialNumber NVARCHAR(100),
    @Company NVARCHAR(100),
    @DeviceType NVARCHAR(100),
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    DECLARE @CompanyId INT, @DeviceTypeId INT;
    
    -- Get Company ID
    SELECT @CompanyId = Id FROM Companies WHERE Name = @Company;
    IF @CompanyId IS NULL
    BEGIN
        RAISERROR('Invalid Company', 16, 1);
        RETURN;
    END
    
    -- Get Device Type ID
    SELECT @DeviceTypeId = Id FROM DeviceTypes WHERE Name = @DeviceType;
    IF @DeviceTypeId IS NULL
    BEGIN
        RAISERROR('Invalid Device Type', 16, 1);
        RETURN;
    END
    
    -- Check if device already exists
    IF EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber)
    BEGIN
        RAISERROR('Device with this Serial Number already exists', 16, 1);
        RETURN;
    END
    
    -- Insert Device
    INSERT INTO Devices (SerialNumber, CompanyId, DeviceTypeId, Status, CurrentLocation, CreatedBy)
    VALUES (@SerialNumber, @CompanyId, @DeviceTypeId, 'Available', 'Inventory', @CreatedBy);
    
    -- Return the Serial Number
    SELECT @SerialNumber AS SerialNumber;
END
GO

-- Create Order
CREATE PROCEDURE sp_CreateOrder
    @Creator NVARCHAR(100),
    @Branch NVARCHAR(100),
    @BranchAddress NVARCHAR(255),
    @PersonName NVARCHAR(100),
    @MobileNumber NVARCHAR(20),
    @DeviceCount INT
AS
BEGIN
    -- Insert Order
    INSERT INTO DeviceOrderRequests (Creator, Branch, BranchAddress, PersonName, MobileNumber, DeviceCount)
    VALUES (@Creator, @Branch, @BranchAddress, @PersonName, @MobileNumber, @DeviceCount);
    
    -- Return the Order ID
    SELECT SCOPE_IDENTITY() AS OrderId;
END
GO

-- Approve Order
CREATE PROCEDURE sp_ApproveOrder
    @OrderId INT,
    @ApprovedBy NVARCHAR(100),
    @IsApproved BIT,
    @Remarks NVARCHAR(255)
AS
BEGIN
    -- Check if order exists
    IF NOT EXISTS (SELECT 1 FROM DeviceOrderRequests WHERE Id = @OrderId)
    BEGIN
        RAISERROR('Order not found', 16, 1);
        RETURN;
    END
    
    -- Check if order is already approved or rejected
    IF EXISTS (SELECT 1 FROM DeviceOrderRequests WHERE Id = @OrderId AND Status != 'Pending')
    BEGIN
        RAISERROR('Order is already processed', 16, 1);
        RETURN;
    END
    
    -- Update Order
    UPDATE DeviceOrderRequests
    SET Status = CASE WHEN @IsApproved = 1 THEN 'Approved' ELSE 'Rejected' END,
        ApprovedBy = @ApprovedBy,
        ApprovalDate = GETDATE(),
        Remarks = @Remarks
    WHERE Id = @OrderId;
    
    -- Return the Order ID
    SELECT @OrderId AS OrderId;
END
GO

-- Assign Device to Coordinator
CREATE PROCEDURE sp_AssignDeviceToCoordinator
    @SerialNumber NVARCHAR(100),
    @AssignedTo NVARCHAR(100),
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    -- Check if device exists
    IF NOT EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber)
    BEGIN
        RAISERROR('Device not found', 16, 1);
        RETURN;
    END
    
    -- Check if device is available
    IF NOT EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber AND Status = 'Available')
    BEGIN
        RAISERROR('Device is not available for assignment', 16, 1);
        RETURN;
    END
    
    -- Begin Transaction
    BEGIN TRANSACTION;
    
    -- Update Device Status
    UPDATE Devices
    SET Status = 'Assigned',
        CurrentLocation = 'Coordinator',
        LastUpdatedDate = GETDATE(),
        LastUpdatedBy = @CreatedBy
    WHERE SerialNumber = @SerialNumber;
    
    -- Insert Assignment
    INSERT INTO DeviceAssignments (SerialNumber, AssignedTo, Status, CreatedBy)
    VALUES (@SerialNumber, @AssignedTo, 'Active', @CreatedBy);
    
    -- Get Assignment ID
    DECLARE @AssignmentId INT = SCOPE_IDENTITY();
    
    -- Commit Transaction
    COMMIT TRANSACTION;
    
    -- Return the Assignment ID
    SELECT @AssignmentId AS AssignmentId;
END
GO

-- Assign Device to Branch
CREATE PROCEDURE sp_AssignDeviceToBranch
    @SerialNumber NVARCHAR(100),
    @Branch NVARCHAR(100),
    @CsoId NVARCHAR(50),
    @Creator NVARCHAR(100),
    @DeviceImage NVARCHAR(MAX)
AS
BEGIN
    -- Check if device exists
    IF NOT EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber)
    BEGIN
        RAISERROR('Device not found', 16, 1);
        RETURN;
    END
    
    -- Check if device is assigned to coordinator
    IF NOT EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber AND Status = 'Assigned' AND CurrentLocation = 'Coordinator')
    BEGIN
        RAISERROR('Device is not assigned to a coordinator', 16, 1);
        RETURN;
    END
    
    -- Get current assignment
    DECLARE @AssignmentId INT;
    SELECT TOP 1 @AssignmentId = Id
    FROM DeviceAssignments
    WHERE SerialNumber = @SerialNumber AND Status = 'Active'
    ORDER BY AssignmentDate DESC;
    
    IF @AssignmentId IS NULL
    BEGIN
        RAISERROR('No active assignment found for this device', 16, 1);
        RETURN;
    END
    
    -- Begin Transaction
    BEGIN TRANSACTION;
    
    -- Update Device Status
    UPDATE Devices
    SET CurrentLocation = 'Branch',
        LastUpdatedDate = GETDATE(),
        LastUpdatedBy = @Creator
    WHERE SerialNumber = @SerialNumber;
    
    -- Update Assignment
    UPDATE DeviceAssignments
    SET Branch = @Branch,
        CsoId = @CsoId,
        Creator = @Creator,
        DeviceImage = @DeviceImage,
        LastUpdatedDate = GETDATE(),
        LastUpdatedBy = @Creator
    WHERE Id = @AssignmentId;
    
    -- Commit Transaction
    COMMIT TRANSACTION;
    
    -- Return the Assignment ID
    SELECT @AssignmentId AS AssignmentId;
END
GO

-- Create Return Request
CREATE PROCEDURE sp_CreateReturnRequest
    @SerialNumber NVARCHAR(100),
    @CsoId NVARCHAR(50),
    @IssueType NVARCHAR(100),
    @DeviceImage NVARCHAR(MAX),
    @Remarks NVARCHAR(255)
AS
BEGIN
    -- Check if device exists
    IF NOT EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber)
    BEGIN
        RAISERROR('Device not found', 16, 1);
        RETURN;
    END
    
    -- Check if device is assigned to branch
    IF NOT EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber AND Status = 'Assigned' AND CurrentLocation = 'Branch')
    BEGIN
        RAISERROR('Device is not assigned to a branch', 16, 1);
        RETURN;
    END
    
    -- Check if there's already a pending return request
    IF EXISTS (SELECT 1 FROM DeviceReturnRequests WHERE SerialNumber = @SerialNumber AND Status = 'Pending')
    BEGIN
        RAISERROR('There is already a pending return request for this device', 16, 1);
        RETURN;
    END
    
    -- Insert Return Request
    INSERT INTO DeviceReturnRequests (SerialNumber, CsoId, IssueType, DeviceImage, Remarks)
    VALUES (@SerialNumber, @CsoId, @IssueType, @DeviceImage, @Remarks);
    
    -- Return the Request ID
    SELECT SCOPE_IDENTITY() AS RequestId;
END
GO

-- Accept Return Request
CREATE PROCEDURE sp_AcceptReturnRequest
    @RequestId INT,
    @Remarks NVARCHAR(255),
    @AcceptedBy NVARCHAR(100)
AS
BEGIN
    -- Check if request exists
    IF NOT EXISTS (SELECT 1 FROM DeviceReturnRequests WHERE Id = @RequestId)
    BEGIN
        RAISERROR('Return request not found', 16, 1);
        RETURN;
    END
    
    -- Check if request is already accepted
    IF EXISTS (SELECT 1 FROM DeviceReturnRequests WHERE Id = @RequestId AND Status = 'Accepted')
    BEGIN
        RAISERROR('Return request is already accepted', 16, 1);
        RETURN;
    END
    
    -- Get Serial Number
    DECLARE @SerialNumber NVARCHAR(100);
    SELECT @SerialNumber = SerialNumber FROM DeviceReturnRequests WHERE Id = @RequestId;
    
    -- Begin Transaction
    BEGIN TRANSACTION;
    
    -- Update Return Request
    UPDATE DeviceReturnRequests
    SET Status = 'Accepted',
        AcceptanceDate = GETDATE(),
        AcceptanceRemarks = @Remarks,
        AcceptedBy = @AcceptedBy
    WHERE Id = @RequestId;
    
    -- Update Device Status
    UPDATE Devices
    SET Status = 'Available',
        CurrentLocation = 'HeadOffice',
        LastUpdatedDate = GETDATE(),
        LastUpdatedBy = @AcceptedBy
    WHERE SerialNumber = @SerialNumber;
    
    -- Update Assignment
    UPDATE DeviceAssignments
    SET Status = 'Returned',
        ReturnDate = GETDATE(),
        LastUpdatedDate = GETDATE(),
        LastUpdatedBy = @AcceptedBy
    WHERE SerialNumber = @SerialNumber AND Status = 'Active';
    
    -- Commit Transaction
    COMMIT TRANSACTION;
    
    -- Return the Request ID
    SELECT @RequestId AS RequestId;
END
GO

