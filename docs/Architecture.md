# PAD 2.0 System Architecture

## Overview

The Practice Area Affiliation Database (PAD) 2.0 is a modern, secure, and maintainable platform built to replace the legacy PAD system. It manages employee industry and capability affiliations with role-based permissions, full audit trails, and integration capabilities.

## System Architecture

### High-Level Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│                 │    │                 │    │                 │
│   React Frontend│────│   .NET 8 API   │────│  SQL Server DB  │
│   (Port 3000)   │    │   (Port 5000)   │    │                 │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                        │                        │
         │                        │                        │
    ┌──────────┐             ┌──────────┐            ┌──────────┐
    │   MSAL   │             │ Azure AD │            │ Stored   │
    │  Auth    │             │   Auth   │            │   Procs  │
    └──────────┘             └──────────┘            └──────────┘
```

### Technology Stack

#### Frontend
- **React 18**: Modern React with Hooks and functional components
- **TypeScript**: Type-safe development
- **Redux Toolkit**: State management with modern Redux patterns
- **Ant Design**: Enterprise-class UI components
- **MSAL React**: Microsoft Authentication Library for Azure AD
- **Axios**: HTTP client for API communication

#### Backend
- **.NET 8**: Latest .NET framework for high performance
- **ASP.NET Core Web API**: RESTful API development
- **Entity Framework Core**: Object-relational mapping
- **Dapper**: Micro-ORM for stored procedure execution
- **AutoMapper**: Object-object mapping
- **Serilog**: Structured logging
- **JWT Bearer Authentication**: Token-based authentication

#### Database
- **SQL Server**: Enterprise database platform
- **Stored Procedures**: All data access through procedures
- **Comprehensive Indexing**: Optimized for performance
- **Audit Logging**: Full change tracking

## Data Architecture

### Core Entities

#### Employee Management
- **Employees**: Core employee data synced from Workday
- **EmployeeAffiliations**: L1, L2, Connected, and other affiliations
- **EmployeeRoles**: Special roles like Practice Leader, Sector Leader

#### Master Data
- **Offices**: Location and regional information
- **Taxonomy**: Hierarchical Industry, Capability, and Keyword data
- **RoleTypes**: Definitions of available roles and their requirements

#### Security & Audit
- **Users**: Application user accounts linked to Azure AD
- **UserRoles**: Role-based access control definitions
- **AuditLog**: Complete change tracking for compliance
- **ActivityLog**: User activity monitoring

### Business Rules Implementation

#### Affiliation Rules
1. **L1, L2, Connected**: Only at Level 1 taxonomy terms
2. **No Duplicates**: Cannot have multiple L1/L2/Connected for same practice
3. **Hierarchy Enforcement**: Business rules validated in stored procedures
4. **Location Scope**: Dynamic or manual based on role type

#### Permission Rules
1. **Role-Based Access**: Different permissions per user role
2. **Data Filtering**: Row-level security based on office/practice/region
3. **Field-Level Control**: Editable fields determined by user permissions
4. **Audit Enforcement**: All changes tracked with user context

## Security Architecture

### Authentication Flow
1. User accesses application
2. MSAL redirects to Azure AD login
3. Azure AD validates credentials
4. Returns JWT token with user claims
5. Backend validates token and extracts user info
6. User permissions determined and cached

### Authorization Levels
- **System Administrator**: Full system access
- **Office Staffing**: Access to employees in their office
- **PPK Global/Regional Leads**: Practice-specific permissions
- **PPK Program Team**: System access role management
- **Staffing System Support**: Boss reporting access

### Data Security
- **Encryption at Rest**: Database-level encryption
- **Encryption in Transit**: HTTPS/TLS for all communications
- **Row-Level Security**: Data filtering at database level
- **Audit Logging**: Complete change tracking
- **Input Validation**: Comprehensive validation at API level

## Integration Architecture

### Workday Integration
- **Daily Sync**: Automated employee data synchronization
- **Delta Processing**: Only process changed records
- **Error Handling**: Comprehensive error logging and alerting
- **Fallback Options**: Manual data correction capabilities

### Downstream Systems
- **RESTful APIs**: Standard HTTP APIs for data access
- **Real-time Access**: Live data queries
- **Bulk Export**: CSV/Excel export capabilities
- **Webhooks**: Event-driven notifications (future)

## Performance Considerations

### Database Performance
- **Stored Procedures**: Optimized query execution
- **Indexing Strategy**: Comprehensive index coverage
- **Query Optimization**: Regular performance monitoring
- **Connection Pooling**: Efficient connection management

### API Performance
- **Async Operations**: Non-blocking I/O operations
- **Caching Strategy**: Redis caching for frequently accessed data
- **Pagination**: Efficient large dataset handling
- **Rate Limiting**: API abuse prevention

### Frontend Performance
- **Code Splitting**: Dynamic import for route-based splitting
- **Lazy Loading**: Components loaded on demand
- **State Management**: Efficient Redux patterns
- **Bundle Optimization**: Webpack optimization

## Deployment Architecture

### Environment Strategy
- **Development**: Local development with localdb
- **QA**: Shared environment for testing
- **Staging**: Production-like environment for UAT
- **Production**: High-availability production setup

### CI/CD Pipeline
1. **Source Control**: Git-based version control
2. **Build Process**: Automated builds on commit
3. **Testing**: Unit tests, integration tests, E2E tests
4. **Deployment**: Automated deployment to environments
5. **Monitoring**: Application performance monitoring

## Monitoring & Observability

### Logging Strategy
- **Structured Logging**: JSON-formatted logs with Serilog
- **Centralized Logging**: Log aggregation platform
- **Error Tracking**: Comprehensive error monitoring
- **Performance Metrics**: API response times, database performance

### Health Monitoring
- **Health Checks**: Built-in health check endpoints
- **Database Health**: Connection and performance monitoring
- **External Dependencies**: Workday API health checks
- **Alerting**: Proactive issue notification

## Scalability Considerations

### Horizontal Scaling
- **Stateless API**: No server-side session state
- **Load Balancing**: Multiple API instances
- **Database Scaling**: Read replicas for reporting
- **CDN Integration**: Static asset delivery

### Vertical Scaling
- **Resource Optimization**: CPU and memory tuning
- **Database Optimization**: Query and index optimization
- **Caching Layers**: Multiple levels of caching
- **Connection Pooling**: Efficient resource utilization

## Future Enhancements

### Planned Features
- **Mobile Support**: Responsive design improvements
- **Advanced Analytics**: Power BI integration
- **Workflow Engine**: Approval workflows for changes
- **Real-time Notifications**: WebSocket-based notifications

### Technical Improvements
- **Microservices**: Service decomposition for better scalability
- **Event Sourcing**: Event-driven architecture patterns
- **GraphQL API**: Flexible data querying
- **Kubernetes**: Container orchestration 