# Morpho Inventory Management API

A .NET Core 8 API for managing Morpho device inventory, procurement, assignment, and returns.

## Features

- Device inventory management
- Order request workflow
- Device assignment to coordinators and branches
- Device return processing
- ADO.NET implementation (no Entity Framework)

## Tech Stack

- .NET Core 8
- ADO.NET for data access
- SQL Server database
- Swagger for API documentation

## Project Structure

- **Controllers**: API endpoints for each domain entity
- **Models**: Data models and request/response DTOs
- **Services**: Business logic implementation
- **Repositories**: Data access layer using ADO.NET
- **Data**: Database context and connection management

## API Endpoints

### Device Management

- `GET /api/device`: Get all devices
- `GET /api/device/available`: Get available devices
- `GET /api/device/{serialNumber}`: Get device by serial number
- `POST /api/device`: Add a new device
- `POST /api/device/bulk`: Add multiple devices

### Order Management

- `GET /api/order`: Get all orders
- `GET /api/order/{orderId}`: Get order by ID
- `GET /api/order/status/{status}`: Get orders by status
- `POST /api/order`: Create a new order
- `POST /api/order/approve`: Approve or reject an order
- `PUT /api/order/{orderId}/status/{status}`: Update order status

### Assignment Management

- `GET /api/assignment`: Get all assignments
- `GET /api/assignment/{assignmentId}`: Get assignment by ID
- `GET /api/assignment/assignee/{assignedTo}`: Get assignments by assignee
- `GET /api/assignment/branch/{branch}`: Get assignments by branch
- `GET /api/assignment/device/{serialNumber}`: Get assignment by device serial number
- `POST /api/assignment/coordinator`: Assign device to coordinator
- `POST /api/assignment/branch`: Assign device to branch

### Return Management

- `GET /api/return`: Get all return requests
- `GET /api/return/{requestId}`: Get return request by ID
- `GET /api/return/status/{status}`: Get return requests by status
- `POST /api/return`: Create a new return request
- `POST /api/return/accept`: Accept a return request

## Database Schema

The database schema is defined in `MorphoInventoryApi/Database/schema.sql` and includes the following tables:

- Devices
- DeviceOrderRequests
- DeviceAssignments
- DeviceReturnRequests
- IssueTypes
- Coordinators
- Branches
- DeviceTypes
- Companies
- Users
- AuditLogs

## Setup and Configuration

1. Create the database using the SQL script in `MorphoInventoryApi/Database/schema.sql`
2. Update the connection string in `appsettings.json` with your SQL Server details
3. Build and run the application

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=MorphoInventory;User Id=your-user;Password=your-password;TrustServerCertificate=True;"
  }
}
```

## Workflow

### Device Procurement Process

1. VP initiates device order with branch details
2. Coordinator reviews and forwards for approval
3. EVP approves or rejects the request
4. IT Support processes approved requests and places orders
5. Devices are added to inventory
6. Devices are assigned to coordinators
7. Coordinators assign devices to branches

### Device Return Process

1. Branch initiates return request with issue details
2. Head Office reviews and accepts the return
3. Device is marked as available in inventory

## License

This project is licensed under the MIT License.

