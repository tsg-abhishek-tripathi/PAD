-- PAD 2.0 Stored Procedures
-- All database interactions must go through stored procedures as per requirements

USE PAD2_DB;
GO

-- =============================================================================
-- EMPLOYEE MANAGEMENT PROCEDURES
-- =============================================================================

-- Get employees with filtering and pagination
CREATE OR ALTER PROCEDURE [GetEmployees]
    @UserId INT,
    @SearchTerm NVARCHAR(200) = NULL,
    @OfficeId INT = NULL,
    @PracticeId INT = NULL,
    @Region NVARCHAR(50) = NULL,
    @IsActive BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Apply user permissions and filters
    SELECT 
        e.EmployeeId,
        e.EmployeeCode,
        e.FirstName,
        e.LastName,
        e.Email,
        e.Level,
        e.Title,
        o.OfficeName,
        o.Region,
        o.Country,
        e.IsActive,
        e.HireDate,
        e.TerminationDate,
        COUNT(*) OVER() as TotalRecords
    FROM [Employees] e
    INNER JOIN [Offices] o ON e.HomeOfficeId = o.OfficeId
    WHERE 
        (@SearchTerm IS NULL OR 
         e.FirstName LIKE '%' + @SearchTerm + '%' OR 
         e.LastName LIKE '%' + @SearchTerm + '%' OR 
         e.EmployeeCode LIKE '%' + @SearchTerm + '%' OR
         e.Email LIKE '%' + @SearchTerm + '%')
        AND (@OfficeId IS NULL OR e.HomeOfficeId = @OfficeId)
        AND (@Region IS NULL OR o.Region = @Region)
        AND e.IsActive = @IsActive
    ORDER BY e.LastName, e.FirstName
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- Get single employee details
CREATE OR ALTER PROCEDURE [GetEmployeeById]
    @EmployeeId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmployeeId,
        e.EmployeeCode,
        e.FirstName,
        e.LastName,
        e.Email,
        e.HomeOfficeId,
        o.OfficeName,
        o.Region,
        o.Country,
        e.Level,
        e.Title,
        e.IsActive,
        e.HireDate,
        e.TerminationDate,
        e.LastSyncDate
    FROM [Employees] e
    INNER JOIN [Offices] o ON e.HomeOfficeId = o.OfficeId
    WHERE e.EmployeeId = @EmployeeId;
END;
GO

-- Update employee (from Workday sync)
CREATE OR ALTER PROCEDURE [UpsertEmployee]
    @EmployeeCode NVARCHAR(20),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(200),
    @HomeOfficeCode NVARCHAR(10),
    @Level NVARCHAR(50),
    @Title NVARCHAR(100),
    @IsActive BIT,
    @HireDate DATE = NULL,
    @TerminationDate DATE = NULL,
    @ModifiedBy NVARCHAR(100) = 'System'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @HomeOfficeId INT;
    DECLARE @EmployeeId INT;
    
    -- Get Office ID
    SELECT @HomeOfficeId = OfficeId 
    FROM [Offices] 
    WHERE OfficeCode = @HomeOfficeCode;
    
    IF @HomeOfficeId IS NULL
    BEGIN
        RAISERROR('Invalid office code: %s', 16, 1, @HomeOfficeCode);
        RETURN;
    END
    
    -- Check if employee exists
    SELECT @EmployeeId = EmployeeId 
    FROM [Employees] 
    WHERE EmployeeCode = @EmployeeCode;
    
    IF @EmployeeId IS NULL
    BEGIN
        -- Insert new employee
        INSERT INTO [Employees] (
            EmployeeCode, FirstName, LastName, Email, HomeOfficeId,
            Level, Title, IsActive, HireDate, TerminationDate, LastSyncDate
        )
        VALUES (
            @EmployeeCode, @FirstName, @LastName, @Email, @HomeOfficeId,
            @Level, @Title, @IsActive, @HireDate, @TerminationDate, GETUTCDATE()
        );
        
        SET @EmployeeId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        -- Update existing employee
        UPDATE [Employees]
        SET 
            FirstName = @FirstName,
            LastName = @LastName,
            Email = @Email,
            HomeOfficeId = @HomeOfficeId,
            Level = @Level,
            Title = @Title,
            IsActive = @IsActive,
            HireDate = @HireDate,
            TerminationDate = @TerminationDate,
            LastSyncDate = GETUTCDATE(),
            ModifiedDate = GETUTCDATE()
        WHERE EmployeeId = @EmployeeId;
    END
    
    SELECT @EmployeeId as EmployeeId;
END;
GO

-- =============================================================================
-- AFFILIATION MANAGEMENT PROCEDURES
-- =============================================================================

-- Get employee affiliations
CREATE OR ALTER PROCEDURE [GetEmployeeAffiliations]
    @EmployeeId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ea.AffiliationId,
        ea.EmployeeId,
        rt.RoleName,
        rt.RoleCode,
        rt.RoleCategory,
        t.Name as PracticeName,
        t.Code as PracticeCode,
        t.FacetType,
        t.Level as PracticeLevel,
        o.OfficeName,
        o.Region,
        ea.LocationScope,
        ea.EffectiveDate,
        ea.ExpirationDate,
        ea.IsActive,
        ea.Source,
        ea.CreatedBy,
        ea.CreatedDate,
        ea.ModifiedBy,
        ea.ModifiedDate
    FROM [EmployeeAffiliations] ea
    INNER JOIN [RoleTypes] rt ON ea.RoleTypeId = rt.RoleTypeId
    INNER JOIN [Taxonomy] t ON ea.PracticeId = t.TaxonomyId
    LEFT JOIN [Offices] o ON ea.LocationId = o.OfficeId
    WHERE ea.EmployeeId = @EmployeeId
        AND ea.IsActive = 1
    ORDER BY rt.RoleCategory, t.Name;
END;
GO

-- Add or update employee affiliation
CREATE OR ALTER PROCEDURE [UpsertEmployeeAffiliation]
    @EmployeeId INT,
    @RoleTypeCode NVARCHAR(20),
    @PracticeCode NVARCHAR(20),
    @LocationCode NVARCHAR(10) = NULL,
    @LocationScope NVARCHAR(50) = NULL,
    @EffectiveDate DATE,
    @ExpirationDate DATE = NULL,
    @Source NVARCHAR(50) = 'Manual',
    @ChangeReason NVARCHAR(500),
    @ModifiedBy NVARCHAR(100),
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRANSACTION;
    
    DECLARE @RoleTypeId INT, @PracticeId INT, @LocationId INT;
    DECLARE @AffiliationId INT;
    DECLARE @OldValues NVARCHAR(MAX);
    
    -- Validate role type
    SELECT @RoleTypeId = RoleTypeId 
    FROM [RoleTypes] 
    WHERE RoleCode = @RoleTypeCode AND IsActive = 1;
    
    IF @RoleTypeId IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('Invalid role type code: %s', 16, 1, @RoleTypeCode);
        RETURN;
    END
    
    -- Validate practice
    SELECT @PracticeId = TaxonomyId 
    FROM [Taxonomy] 
    WHERE Code = @PracticeCode AND IsActive = 1;
    
    IF @PracticeId IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('Invalid practice code: %s', 16, 1, @PracticeCode);
        RETURN;
    END
    
    -- Get location ID if specified
    IF @LocationCode IS NOT NULL
    BEGIN
        SELECT @LocationId = OfficeId 
        FROM [Offices] 
        WHERE OfficeCode = @LocationCode;
        
        IF @LocationId IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Invalid location code: %s', 16, 1, @LocationCode);
            RETURN;
        END
    END
    
    -- Check for existing affiliation
    SELECT @AffiliationId = AffiliationId,
           @OldValues = CONCAT('RoleType:', rt.RoleName, '; Practice:', t.Name, '; Location:', ISNULL(o.OfficeName, 'N/A'))
    FROM [EmployeeAffiliations] ea
    LEFT JOIN [RoleTypes] rt ON ea.RoleTypeId = rt.RoleTypeId
    LEFT JOIN [Taxonomy] t ON ea.PracticeId = t.TaxonomyId
    LEFT JOIN [Offices] o ON ea.LocationId = o.OfficeId
    WHERE ea.EmployeeId = @EmployeeId 
        AND ea.RoleTypeId = @RoleTypeId 
        AND ea.PracticeId = @PracticeId 
        AND ISNULL(ea.LocationId, 0) = ISNULL(@LocationId, 0);
    
    IF @AffiliationId IS NULL
    BEGIN
        -- Insert new affiliation
        INSERT INTO [EmployeeAffiliations] (
            EmployeeId, RoleTypeId, PracticeId, LocationId, LocationScope,
            EffectiveDate, ExpirationDate, Source, CreatedBy, ModifiedBy
        )
        VALUES (
            @EmployeeId, @RoleTypeId, @PracticeId, @LocationId, @LocationScope,
            @EffectiveDate, @ExpirationDate, @Source, @ModifiedBy, @ModifiedBy
        );
        
        SET @AffiliationId = SCOPE_IDENTITY();
        
        -- Log audit
        EXEC [LogAudit] 
            @TableName = 'EmployeeAffiliations',
            @RecordId = @AffiliationId,
            @FieldName = 'ALL',
            @OldValue = NULL,
            --@NewValue = CONCAT('Added affiliation: ', @RoleTypeCode, ' for ', @PracticeCode),
            @ChangeType = 'INSERT',
            @ChangeReason = @ChangeReason,
            @ChangedBy = @ModifiedBy;
    END
    ELSE
    BEGIN
        -- Update existing affiliation
        UPDATE [EmployeeAffiliations]
        SET 
            LocationId = @LocationId,
            LocationScope = @LocationScope,
            EffectiveDate = @EffectiveDate,
            ExpirationDate = @ExpirationDate,
            Source = @Source,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETUTCDATE()
        WHERE AffiliationId = @AffiliationId;
        
        -- Log audit
        EXEC [LogAudit] 
            @TableName = 'EmployeeAffiliations',
            @RecordId = @AffiliationId,
            @FieldName = 'ALL',
            @OldValue = @OldValues,
            --@NewValue = CONCAT('Updated affiliation: ', @RoleTypeCode, ' for ', @PracticeCode),
            @ChangeType = 'UPDATE',
            @ChangeReason = @ChangeReason,
            @ChangedBy = @ModifiedBy;
    END
    
    COMMIT TRANSACTION;
    
    SELECT @AffiliationId as AffiliationId;
END;
GO

-- Remove employee affiliation
CREATE OR ALTER PROCEDURE [RemoveEmployeeAffiliation]
    @AffiliationId INT,
    @ChangeReason NVARCHAR(500),
    @ModifiedBy NVARCHAR(100),
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRANSACTION;
    
    DECLARE @OldValues NVARCHAR(MAX);
    
    -- Get current values for audit
    SELECT @OldValues = CONCAT('RoleType:', rt.RoleName, '; Practice:', t.Name, '; Location:', ISNULL(o.OfficeName, 'N/A'))
    FROM [EmployeeAffiliations] ea
    LEFT JOIN [RoleTypes] rt ON ea.RoleTypeId = rt.RoleTypeId
    LEFT JOIN [Taxonomy] t ON ea.PracticeId = t.TaxonomyId
    LEFT JOIN [Offices] o ON ea.LocationId = o.OfficeId
    WHERE ea.AffiliationId = @AffiliationId;
    
    -- Soft delete (set IsActive = 0)
    UPDATE [EmployeeAffiliations]
    SET 
        IsActive = 0,
        ModifiedBy = @ModifiedBy,
        ModifiedDate = GETUTCDATE()
    WHERE AffiliationId = @AffiliationId;
    
    -- Log audit
    EXEC [LogAudit] 
        @TableName = 'EmployeeAffiliations',
        @RecordId = @AffiliationId,
        @FieldName = 'IsActive',
        @OldValue = @OldValues,
        @NewValue = 'REMOVED',
        @ChangeType = 'DELETE',
        @ChangeReason = @ChangeReason,
        @ChangedBy = @ModifiedBy;
    
    COMMIT TRANSACTION;
END;
GO

-- =============================================================================
-- MASTER DATA PROCEDURES
-- =============================================================================

-- Get taxonomy data
CREATE OR ALTER PROCEDURE [GetTaxonomy]
    @FacetType NVARCHAR(20) = NULL,
    @Level INT = NULL,
    @ParentId INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TaxonomyId,
        FacetType,
        Level,
        ParentId,
        Code,
        Name,
        Description,
        IsActive
    FROM [Taxonomy]
    WHERE 
        (@FacetType IS NULL OR FacetType = @FacetType)
        AND (@Level IS NULL OR Level = @Level)
        AND (@ParentId IS NULL OR ParentId = @ParentId)
        AND IsActive = @IsActive
    ORDER BY FacetType, Level, Name;
END;
GO

-- Get offices
CREATE OR ALTER PROCEDURE [GetOffices]
    @Region NVARCHAR(50) = NULL,
    @Country NVARCHAR(50) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        OfficeId,
        OfficeCode,
        OfficeName,
        Country,
        Region,
        Cluster,
        IsActive
    FROM [Offices]
    WHERE 
        (@Region IS NULL OR Region = @Region)
        AND (@Country IS NULL OR Country = @Country)
        AND IsActive = @IsActive
    ORDER BY Region, Country, OfficeName;
END;
GO

-- Get role types
CREATE OR ALTER PROCEDURE [GetRoleTypes]
    @RoleCategory NVARCHAR(50) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        RoleTypeId,
        RoleCategory,
        RoleName,
        RoleCode,
        Description,
        RequiresPracticeSelection,
        RequiresLocationSelection,
        AllowsMultiplePerPractice,
        IsActive
    FROM [RoleTypes]
    WHERE 
        (@RoleCategory IS NULL OR RoleCategory = @RoleCategory)
        AND IsActive = @IsActive
    ORDER BY RoleCategory, RoleName;
END;
GO

-- =============================================================================
-- USER MANAGEMENT PROCEDURES
-- =============================================================================

-- Get or create user
CREATE OR ALTER PROCEDURE [UpsertUser]
    @UserPrincipalName NVARCHAR(200),
    @DisplayName NVARCHAR(200),
    @Email NVARCHAR(200),
    @EmployeeCode NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT;
    DECLARE @EmployeeId INT;
    
    -- Get employee ID if provided
    IF @EmployeeCode IS NOT NULL
    BEGIN
        SELECT @EmployeeId = EmployeeId 
        FROM [Employees] 
        WHERE EmployeeCode = @EmployeeCode;
    END
    
    -- Check if user exists
    SELECT @UserId = UserId 
    FROM [Users] 
    WHERE UserPrincipalName = @UserPrincipalName;
    
    IF @UserId IS NULL
    BEGIN
        -- Insert new user
        INSERT INTO [Users] (
            EmployeeId, UserPrincipalName, DisplayName, Email, LastLoginDate
        )
        VALUES (
            @EmployeeId, @UserPrincipalName, @DisplayName, @Email, GETUTCDATE()
        );
        
        SET @UserId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        -- Update existing user
        UPDATE [Users]
        SET 
            EmployeeId = ISNULL(@EmployeeId, EmployeeId),
            DisplayName = @DisplayName,
            Email = @Email,
            LastLoginDate = GETUTCDATE(),
            ModifiedDate = GETUTCDATE()
        WHERE UserId = @UserId;
    END
    
    -- Return user details with permissions
    SELECT 
        u.UserId,
        u.EmployeeId,
        u.UserPrincipalName,
        u.DisplayName,
        u.Email,
        e.EmployeeCode,
        e.FirstName + ' ' + e.LastName as FullName,
        o.Region,
        o.OfficeName
    FROM [Users] u
    LEFT JOIN [Employees] e ON u.EmployeeId = e.EmployeeId
    LEFT JOIN [Offices] o ON e.HomeOfficeId = o.OfficeId
    WHERE u.UserId = @UserId;
END;
GO

-- =============================================================================
-- AUDIT AND LOGGING PROCEDURES
-- =============================================================================

-- Log audit trail
CREATE OR ALTER PROCEDURE [LogAudit]
    @TableName NVARCHAR(100),
    @RecordId INT,
    @FieldName NVARCHAR(100),
    @OldValue NVARCHAR(MAX),
    @NewValue NVARCHAR(MAX),
    @ChangeType NVARCHAR(20),
    @ChangeReason NVARCHAR(500),
    @ChangedBy NVARCHAR(100),
    @UserAgent NVARCHAR(500) = NULL,
    @IPAddress NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [AuditLog] (
        TableName, RecordId, FieldName, OldValue, NewValue,
        ChangeType, ChangeReason, ChangedBy, UserAgent, IPAddress
    )
    VALUES (
        @TableName, @RecordId, @FieldName, @OldValue, @NewValue,
        @ChangeType, @ChangeReason, @ChangedBy, @UserAgent, @IPAddress
    );
END;
GO

-- Get audit history
CREATE OR ALTER PROCEDURE [GetAuditHistory]
    @TableName NVARCHAR(100) = NULL,
    @RecordId INT = NULL,
    @ChangedBy NVARCHAR(100) = NULL,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        AuditId,
        TableName,
        RecordId,
        FieldName,
        OldValue,
        NewValue,
        ChangeType,
        ChangeReason,
        ChangedBy,
        ChangedDate,
        UserAgent,
        IPAddress,
        COUNT(*) OVER() as TotalRecords
    FROM [AuditLog]
    WHERE 
        (@TableName IS NULL OR TableName = @TableName)
        AND (@RecordId IS NULL OR RecordId = @RecordId)
        AND (@ChangedBy IS NULL OR ChangedBy LIKE '%' + @ChangedBy + '%')
        AND (@StartDate IS NULL OR ChangedDate >= @StartDate)
        AND (@EndDate IS NULL OR ChangedDate <= @EndDate)
    ORDER BY ChangedDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- =============================================================================
-- VALIDATION PROCEDURES
-- =============================================================================

-- Validate business rules
CREATE OR ALTER PROCEDURE [ValidateAffiliationRules]
    @EmployeeId INT,
    @RoleTypeCode NVARCHAR(20),
    @PracticeCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ValidationErrors TABLE (ErrorMessage NVARCHAR(500));
    DECLARE @RoleTypeId INT, @PracticeId INT;
    DECLARE @FacetType NVARCHAR(20), @PracticeLevel INT;
    
    -- Get role and practice details
    SELECT @RoleTypeId = RoleTypeId FROM [RoleTypes] WHERE RoleCode = @RoleTypeCode;
    SELECT @PracticeId = TaxonomyId, @FacetType = FacetType, @PracticeLevel = Level 
    FROM [Taxonomy] WHERE Code = @PracticeCode;
    
    -- Rule: L1, L2, Connected can only be at Level 1 practices
    IF @RoleTypeCode IN ('L1', 'L2', 'CONNECTED') AND @PracticeLevel != 1
    BEGIN
        INSERT INTO @ValidationErrors VALUES ('L1, L2, and Connected affiliations can only be assigned to Level 1 practices');
    END
    
    -- Rule: Cannot have duplicate L1/L2/Connected for same practice
    IF @RoleTypeCode IN ('L1', 'L2', 'CONNECTED')
    BEGIN
        IF EXISTS (
            SELECT 1 FROM [EmployeeAffiliations] ea
            INNER JOIN [RoleTypes] rt ON ea.RoleTypeId = rt.RoleTypeId
            INNER JOIN [Taxonomy] t ON ea.PracticeId = t.TaxonomyId
            INNER JOIN [Taxonomy] parent ON t.ParentId = parent.TaxonomyId OR (t.Level = 1 AND t.TaxonomyId = parent.TaxonomyId)
            WHERE ea.EmployeeId = @EmployeeId 
                AND rt.RoleCode IN ('L1', 'L2', 'CONNECTED')
                AND parent.TaxonomyId = (
                    SELECT CASE WHEN Level = 1 THEN TaxonomyId ELSE ParentId END 
                    FROM [Taxonomy] WHERE TaxonomyId = @PracticeId
                )
                AND ea.IsActive = 1
        )
        BEGIN
            INSERT INTO @ValidationErrors VALUES ('Employee already has an L1, L2, or Connected affiliation for this practice area');
        END
    END
    
    -- Return validation results
    SELECT ErrorMessage FROM @ValidationErrors;
END;
GO

PRINT 'PAD 2.0 Stored Procedures created successfully'; 