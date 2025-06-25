# Practice Area Affiliation Database (PAD) 2.0

## Overview
PAD 2.0 is a modern, secure, and maintainable platform that replaces the legacy PAD system. It manages employee industry and capability affiliations with role-based permissions, full audit trails, and integration capabilities.

## Technology Stack
- **Frontend**: React 18 with Redux Toolkit for state management
- **Backend**: .NET 8 Web API
- **Database**: Microsoft SQL Server
- **Authentication**: MSAL (Microsoft Authentication Library)
- **Database Access**: SQL Server stored procedures only

## Project Structure
```
PAD/
├── frontend/          # React application
├── backend/           # .NET 8 Web API
├── database/          # SQL Server scripts and stored procedures
├── docs/              # Documentation
└── README.md
```

## Quick Start

### Prerequisites
- Node.js 18+
- .NET 8 SDK
- SQL Server 2019+
- Visual Studio 2022 or VS Code

### Backend Setup
1. Navigate to backend directory: `cd backend`
2. Restore packages: `dotnet restore`
3. Update connection string in `appsettings.json`
4. Run database migrations: `dotnet ef database update`
5. Start API: `dotnet run`

### Frontend Setup
1. Navigate to frontend directory: `cd frontend`
2. Install dependencies: `npm install`
3. Update MSAL configuration in `src/config/msalConfig.js`
4. Start development server: `npm start`

### Database Setup
1. Execute scripts in `database/` directory in order:
   - `01_CreateDatabase.sql`
   - `02_CreateTables.sql`
   - `03_CreateStoredProcedures.sql`
   - `04_SeedData.sql`

## Key Features
- **Secure Authentication**: MSAL integration with role-based access control
- **Granular Permissions**: Different user roles with specific data access and edit rights
- **Audit Trail**: Complete change history for all data modifications
- **Real-time Validation**: Business rules enforcement with immediate feedback
- **Integration Ready**: APIs for upstream/downstream system integration
- **Bulk Operations**: Efficient handling of multiple record updates

## User Roles
1. **System Administrator**: Full access to all data and system settings
2. **Office Staffing Teams**: Manage affiliations for employees in their office
3. **PPK Global S&O Leads**: Manage practice-related roles globally
4. **Regional S&O Leads**: Manage practice-related roles in their region
5. **PPK Program Team**: Manage system access roles
6. **Staffing System Support**: Manage Boss Reporting roles

## Development Guidelines
- All database interactions must use stored procedures
- Follow React/Redux best practices for state management
- Implement comprehensive error handling and logging
- Write unit tests for all API endpoints
- Follow security best practices for authentication and authorization

## API Documentation
API documentation is available at `/swagger` when running the backend application.

## Support
For questions or issues, contact the PAD 2.0 development team. 