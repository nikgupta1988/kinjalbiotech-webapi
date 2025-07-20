# KinjalBiotech Medicine Management API

A comprehensive ASP.NET Core microservice for managing medicine inventory with departments using Entity Framework Core.

## Features

- **Complete CRUD operations** for Departments and Medicines
- **Entity Framework Core** with flexible database provider support
- **RESTful API** with comprehensive endpoints
- **Swagger UI** for API documentation and testing
- **Foreign key relationships** with cascade delete
- **Data validation** and comprehensive error handling
- **Async/await patterns** for scalability
- **CORS support** for cross-origin requests
- **Structured logging** throughout the application

## Database Tables

### Departments
- `DeptID` (Primary Key, Auto-increment)
- `DeptName` (Required, Max 100 chars)
- `UpdatedBy` (Optional, Max 50 chars)
- `UpdateDate` (DateTime, nullable)

### Medicines
- `MedicineID` (Primary Key, Auto-increment)
- `DeptID` (Foreign Key to Departments)
- `MedicineName` (Required, Max 200 chars)
- `MedicineDesc` (Optional, Max 500 chars)
- `MedicineQuantity` (Integer, validated positive)
- `ImageUrl` (Optional, Max 500 chars)
- `UpdatedDate` (DateTime, nullable)
- `UpdatedBy` (Optional, Max 50 chars)

## API Endpoints

### Departments Management
- `GET /api/Departments` - Get all departments
- `POST /api/Departments` - Create new department
- `GET /api/Departments/{id}` - Get department by ID
- `PUT /api/Departments/{id}` - Update department
- `DELETE /api/Departments/{id}` - Delete department

### Medicines Management
- `GET /api/Medicines` - Get all medicines (with department info)
- `POST /api/Medicines` - Create new medicine
- `GET /api/Medicines/{id}` - Get medicine by ID
- `PUT /api/Medicines/{id}` - Update medicine
- `DELETE /api/Medicines/{id}` - Delete medicine
- `GET /api/Medicines/department/{deptId}` - Get medicines by department

### System Endpoints
- `GET /health` - Health check endpoint

## Database Configuration

The application supports both SQL Server and SQLite databases. Configure in `appsettings.json`:

### For SQL Server (Production)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-VBJT67G\\SQLEXPRESS;Database=KijalBioTechDB;Trusted_connection=True;TrustServerCertificate=True",
    "SqliteConnection": "Data Source=KinjalBiotechDb.db"
  },
  "DatabaseProvider": "SqlServer"
}
```

### For SQLite (Development/Testing)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-VBJT67G\\SQLEXPRESS;Database=KijalBioTechDB;Trusted_connection=True;TrustServerCertificate=True",
    "SqliteConnection": "Data Source=KinjalBiotechDb.db"
  },
  "DatabaseProvider": "SQLite"
}
```

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (for production) or SQLite (for development)
- Entity Framework Core Tools

### Installation
1. Clone the repository
2. Navigate to the project directory
3. Install EF Core tools (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

### Database Setup

#### For SQL Server
1. Ensure SQL Server is running
2. Update connection string in `appsettings.json`
3. Set `DatabaseProvider` to `"SqlServer"`
4. Run migrations:
   ```bash
   dotnet ef database update
   ```

#### For SQLite (Development)
1. Set `DatabaseProvider` to `"SQLite"` in `appsettings.json`
2. Run migrations:
   ```bash
   dotnet ef database update
   ```

### Running the Application
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5067`
- Swagger UI: `http://localhost:5067` (root URL)

## Migration Commands

### Create New Migration
```bash
dotnet ef migrations add MigrationName
```

### Update Database
```bash
dotnet ef database update
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

## Testing

Use the built-in Swagger UI at the root URL to test all endpoints interactively.

### Sample API Calls

#### Create Department
```bash
POST /api/Departments
{
  "deptName": "Cardiology",
  "updatedBy": "Admin"
}
```

#### Create Medicine
```bash
POST /api/Medicines
{
  "deptID": 1,
  "medicineName": "Aspirin",
  "medicineDesc": "Pain reliever and anti-inflammatory",
  "medicineQuantity": 100,
  "imageUrl": "https://example.com/aspirin.jpg",
  "updatedBy": "Pharmacist"
}
```

## Technical Stack
- **ASP.NET Core 8.0** Web API
- **Entity Framework Core 9.0.7**
- **SQL Server** (Production) / **SQLite** (Development)
- **Swagger/OpenAPI** for documentation
- **Built-in dependency injection**
- **Structured logging**

## Project Structure
```
KinjalBiotech.Webapi/
├── Controllers/
│   ├── DepartmentsController.cs
│   └── MedicinesController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── Department.cs
│   └── Medicine.cs
├── Migrations/
├── Program.cs
├── appsettings.json
└── README.md
