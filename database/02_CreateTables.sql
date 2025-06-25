-- PAD 2.0 Table Creation Script
-- This script creates all necessary tables for the PAD 2.0 system

USE PAD2_DB;
GO

-- =============================================================================
-- MASTER DATA TABLES
-- =============================================================================

-- Office/Location Master Data
CREATE TABLE [Offices] (
    [OfficeId] INT IDENTITY(1,1) PRIMARY KEY,
    [OfficeCode] NVARCHAR(10) NOT NULL UNIQUE,
    [OfficeName] NVARCHAR(100) NOT NULL,
    [Country] NVARCHAR(50) NOT NULL,
    [Region] NVARCHAR(50) NOT NULL,  -- AMERICAS, EMEA, APAC
    [Cluster] NVARCHAR(50) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Taxonomy Master Data (Industry, Capability, Keywords)
CREATE TABLE [Taxonomy] (
    [TaxonomyId] INT IDENTITY(1,1) PRIMARY KEY,
    [FacetType] NVARCHAR(20) NOT NULL, -- Industry, Capability, Keyword
    [Level] INT NOT NULL, -- 1, 2, 3 (hierarchy level)
    [ParentId] INT NULL FOREIGN KEY REFERENCES [Taxonomy]([TaxonomyId]),
    [Code] NVARCHAR(20) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [CK_Taxonomy_FacetType] CHECK ([FacetType] IN ('Industry', 'Capability', 'Keyword')),
    CONSTRAINT [CK_Taxonomy_Level] CHECK ([Level] BETWEEN 1 AND 3)
);

-- Role Types Master Data
CREATE TABLE [RoleTypes] (
    [RoleTypeId] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleCategory] NVARCHAR(50) NOT NULL, -- Affiliation, Leadership, Expertise, Access
    [RoleName] NVARCHAR(100) NOT NULL,
    [RoleCode] NVARCHAR(20) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [RequiresPracticeSelection] BIT NOT NULL DEFAULT 0,
    [RequiresLocationSelection] BIT NOT NULL DEFAULT 0,
    [AllowsMultiplePerPractice] BIT NOT NULL DEFAULT 1,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- User Roles/Permissions Master Data
CREATE TABLE [UserRoles] (
    [UserRoleId] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleName] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(200) NULL,
    [IsSystemRole] BIT NOT NULL DEFAULT 0,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =============================================================================
-- EMPLOYEE AND AFFILIATION TABLES
-- =============================================================================

-- Employee Master Data (synced from Workday)
CREATE TABLE [Employees] (
    [EmployeeId] INT IDENTITY(1,1) PRIMARY KEY,
    [EmployeeCode] NVARCHAR(20) NOT NULL UNIQUE, -- Workday Employee ID
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(200) NOT NULL UNIQUE,
    [HomeOfficeId] INT NOT NULL FOREIGN KEY REFERENCES [Offices]([OfficeId]),
    [Level] NVARCHAR(50) NOT NULL, -- SM, AP, Partner, Senior Partner, etc.
    [Title] NVARCHAR(100) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [HireDate] DATE NULL,
    [TerminationDate] DATE NULL,
    [LastSyncDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Employee Affiliations (L1, L2, Connected, etc.)
CREATE TABLE [EmployeeAffiliations] (
    [AffiliationId] INT IDENTITY(1,1) PRIMARY KEY,
    [EmployeeId] INT NOT NULL FOREIGN KEY REFERENCES [Employees]([EmployeeId]),
    [RoleTypeId] INT NOT NULL FOREIGN KEY REFERENCES [RoleTypes]([RoleTypeId]),
    [PracticeId] INT NOT NULL FOREIGN KEY REFERENCES [Taxonomy]([TaxonomyId]),
    [LocationId] INT NULL FOREIGN KEY REFERENCES [Offices]([OfficeId]),
    [LocationScope] NVARCHAR(50) NULL, -- Global, Regional, Local
    [EffectiveDate] DATE NOT NULL DEFAULT GETDATE(),
    [ExpirationDate] DATE NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [Source] NVARCHAR(50) NOT NULL DEFAULT 'Manual', -- Manual, EP/HLA, Import
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [UK_EmployeeAffiliations_Unique] UNIQUE ([EmployeeId], [RoleTypeId], [PracticeId], [LocationId])
);

-- Employee Special Roles (Practice Leader, Sector Leader, etc.)
CREATE TABLE [EmployeeRoles] (
    [EmployeeRoleId] INT IDENTITY(1,1) PRIMARY KEY,
    [EmployeeId] INT NOT NULL FOREIGN KEY REFERENCES [Employees]([EmployeeId]),
    [RoleTypeId] INT NOT NULL FOREIGN KEY REFERENCES [RoleTypes]([RoleTypeId]),
    [PrimaryPracticeId] INT NULL FOREIGN KEY REFERENCES [Taxonomy]([TaxonomyId]),
    [SecondaryPracticeId] INT NULL FOREIGN KEY REFERENCES [Taxonomy]([TaxonomyId]), -- For Interlock Champions
    [LocationId] INT NULL FOREIGN KEY REFERENCES [Offices]([OfficeId]),
    [LocationScope] NVARCHAR(50) NULL, -- Global, Regional, Local
    [EffectiveDate] DATE NOT NULL DEFAULT GETDATE(),
    [ExpirationDate] DATE NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =============================================================================
-- USER MANAGEMENT TABLES
-- =============================================================================

-- Application Users
CREATE TABLE [Users] (
    [UserId] INT IDENTITY(1,1) PRIMARY KEY,
    [EmployeeId] INT NULL FOREIGN KEY REFERENCES [Employees]([EmployeeId]),
    [UserPrincipalName] NVARCHAR(200) NOT NULL UNIQUE, -- Azure AD UPN
    [DisplayName] NVARCHAR(200) NOT NULL,
    [Email] NVARCHAR(200) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastLoginDate] DATETIME2 NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- User Role Assignments
CREATE TABLE [UserRoleAssignments] (
    [UserRoleAssignmentId] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([UserId]),
    [UserRoleId] INT NOT NULL FOREIGN KEY REFERENCES [UserRoles]([UserRoleId]),
    [PracticeId] INT NULL FOREIGN KEY REFERENCES [Taxonomy]([TaxonomyId]), -- For practice-specific roles
    [LocationId] INT NULL FOREIGN KEY REFERENCES [Offices]([OfficeId]), -- For location-specific roles
    [IsActive] BIT NOT NULL DEFAULT 1,
    [AssignedBy] NVARCHAR(100) NOT NULL,
    [AssignedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [RevokedBy] NVARCHAR(100) NULL,
    [RevokedDate] DATETIME2 NULL,
    CONSTRAINT [UK_UserRoleAssignments_Unique] UNIQUE ([UserId], [UserRoleId], [PracticeId], [LocationId])
);

-- =============================================================================
-- AUDIT AND LOGGING TABLES
-- =============================================================================

-- Audit Log for all changes
CREATE TABLE [AuditLog] (
    [AuditId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [TableName] NVARCHAR(100) NOT NULL,
    [RecordId] INT NOT NULL,
    [FieldName] NVARCHAR(100) NOT NULL,
    [OldValue] NVARCHAR(MAX) NULL,
    [NewValue] NVARCHAR(MAX) NULL,
    [ChangeType] NVARCHAR(20) NOT NULL, -- INSERT, UPDATE, DELETE
    [ChangeReason] NVARCHAR(500) NULL,
    [ChangedBy] NVARCHAR(100) NOT NULL,
    [ChangedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UserAgent] NVARCHAR(500) NULL,
    [IPAddress] NVARCHAR(50) NULL
);

-- System Activity Log
CREATE TABLE [ActivityLog] (
    [ActivityId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NULL FOREIGN KEY REFERENCES [Users]([UserId]),
    [ActivityType] NVARCHAR(50) NOT NULL, -- Login, Logout, Search, Export, etc.
    [EntityType] NVARCHAR(50) NULL, -- Employee, Affiliation, Role
    [EntityId] INT NULL,
    [Description] NVARCHAR(500) NULL,
    [IPAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =============================================================================
-- INTEGRATION TABLES
-- =============================================================================

-- Workday Sync Status
CREATE TABLE [WorkdaySyncStatus] (
    [SyncId] INT IDENTITY(1,1) PRIMARY KEY,
    [SyncType] NVARCHAR(50) NOT NULL, -- Full, Delta
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [Status] NVARCHAR(20) NOT NULL, -- Running, Completed, Failed
    [RecordsProcessed] INT NULL,
    [RecordsUpdated] INT NULL,
    [RecordsErrors] INT NULL,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- External System Integration Log
CREATE TABLE [IntegrationLog] (
    [IntegrationId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [SystemName] NVARCHAR(100) NOT NULL, -- Cortex, R&A, Revenue, etc.
    [RequestType] NVARCHAR(50) NOT NULL, -- GET, POST, PUT, DELETE
    [Endpoint] NVARCHAR(500) NOT NULL,
    [RequestData] NVARCHAR(MAX) NULL,
    [ResponseData] NVARCHAR(MAX) NULL,
    [StatusCode] INT NULL,
    [IsSuccess] BIT NOT NULL,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [ProcessingTime] INT NULL, -- milliseconds
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =============================================================================
-- INDEXES FOR PERFORMANCE
-- =============================================================================

-- Employee indexes
CREATE INDEX IX_Employees_EmployeeCode ON [Employees]([EmployeeCode]);
CREATE INDEX IX_Employees_Email ON [Employees]([Email]);
CREATE INDEX IX_Employees_HomeOfficeId ON [Employees]([HomeOfficeId]);
CREATE INDEX IX_Employees_IsActive ON [Employees]([IsActive]);

-- Affiliation indexes
CREATE INDEX IX_EmployeeAffiliations_EmployeeId ON [EmployeeAffiliations]([EmployeeId]);
CREATE INDEX IX_EmployeeAffiliations_RoleTypeId ON [EmployeeAffiliations]([RoleTypeId]);
CREATE INDEX IX_EmployeeAffiliations_PracticeId ON [EmployeeAffiliations]([PracticeId]);
CREATE INDEX IX_EmployeeAffiliations_IsActive ON [EmployeeAffiliations]([IsActive]);

-- Role indexes
CREATE INDEX IX_EmployeeRoles_EmployeeId ON [EmployeeRoles]([EmployeeId]);
CREATE INDEX IX_EmployeeRoles_RoleTypeId ON [EmployeeRoles]([RoleTypeId]);
CREATE INDEX IX_EmployeeRoles_IsActive ON [EmployeeRoles]([IsActive]);

-- Taxonomy indexes
CREATE INDEX IX_Taxonomy_FacetType ON [Taxonomy]([FacetType]);
CREATE INDEX IX_Taxonomy_Level ON [Taxonomy]([Level]);
CREATE INDEX IX_Taxonomy_ParentId ON [Taxonomy]([ParentId]);
CREATE INDEX IX_Taxonomy_IsActive ON [Taxonomy]([IsActive]);

-- Audit log indexes
CREATE INDEX IX_AuditLog_TableName ON [AuditLog]([TableName]);
CREATE INDEX IX_AuditLog_RecordId ON [AuditLog]([RecordId]);
CREATE INDEX IX_AuditLog_ChangedBy ON [AuditLog]([ChangedBy]);
CREATE INDEX IX_AuditLog_ChangedDate ON [AuditLog]([ChangedDate]);

-- User indexes
CREATE INDEX IX_Users_UserPrincipalName ON [Users]([UserPrincipalName]);
CREATE INDEX IX_Users_EmployeeId ON [Users]([EmployeeId]);
CREATE INDEX IX_Users_IsActive ON [Users]([IsActive]);

PRINT 'PAD 2.0 Tables created successfully'; 