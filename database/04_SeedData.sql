-- PAD 2.0 Seed Data Script
-- This script inserts initial seed data for development and testing

USE PAD2_DB;
GO

-- =============================================================================
-- SEED OFFICE DATA
-- =============================================================================

SET IDENTITY_INSERT [Offices] ON;

INSERT INTO [Offices] (OfficeId, OfficeCode, OfficeName, Country, Region, Cluster, IsActive)
VALUES
    (1, 'NYC', 'New York', 'United States', 'AMERICAS', 'North America', 1),
    (2, 'BOS', 'Boston', 'United States', 'AMERICAS', 'North America', 1),
    (3, 'CHI', 'Chicago', 'United States', 'AMERICAS', 'North America', 1),
    (4, 'SF', 'San Francisco', 'United States', 'AMERICAS', 'North America', 1),
    (5, 'LON', 'London', 'United Kingdom', 'EMEA', 'Western Europe', 1),
    (6, 'PAR', 'Paris', 'France', 'EMEA', 'Western Europe', 1),
    (7, 'FRA', 'Frankfurt', 'Germany', 'EMEA', 'Western Europe', 1),
    (8, 'MIL', 'Milan', 'Italy', 'EMEA', 'Western Europe', 1),
    (9, 'TOK', 'Tokyo', 'Japan', 'APAC', 'Asia Pacific', 1),
    (10, 'SIN', 'Singapore', 'Singapore', 'APAC', 'Asia Pacific', 1),
    (11, 'SYD', 'Sydney', 'Australia', 'APAC', 'Asia Pacific', 1),
    (12, 'HKG', 'Hong Kong', 'Hong Kong', 'APAC', 'Asia Pacific', 1);

SET IDENTITY_INSERT [Offices] OFF;

-- =============================================================================
-- SEED TAXONOMY DATA (Industry and Capability)
-- =============================================================================

SET IDENTITY_INSERT [Taxonomy] ON;

-- Level 1 Industries
INSERT INTO [Taxonomy] (TaxonomyId, FacetType, Level, ParentId, Code, Name, Description, IsActive)
VALUES
    (1, 'Industry', 1, NULL, 'FS', 'Financial Services', 'Banking, Insurance, Capital Markets', 1),
    (2, 'Industry', 1, NULL, 'HC', 'Healthcare', 'Healthcare and Life Sciences', 1),
    (3, 'Industry', 1, NULL, 'TMT', 'Technology, Media & Telecommunications', 'Technology, Media and Telecom sectors', 1),
    (4, 'Industry', 1, NULL, 'PE', 'Private Equity', 'Private Equity and Alternative Investments', 1),
    (5, 'Industry', 1, NULL, 'CPG', 'Consumer Products & Retail', 'Consumer Products and Retail', 1),
    (6, 'Industry', 1, NULL, 'INO', 'Industrial Goods & Services', 'Industrial and Manufacturing', 1),
    (7, 'Industry', 1, NULL, 'EN', 'Energy & Natural Resources', 'Energy, Oil, Gas, Mining', 1);

-- Level 2 Industries (Sample)
INSERT INTO [Taxonomy] (TaxonomyId, FacetType, Level, ParentId, Code, Name, Description, IsActive)
VALUES
    (101, 'Industry', 2, 1, 'BANK', 'Banking', 'Commercial and Investment Banking', 1),
    (102, 'Industry', 2, 1, 'INS', 'Insurance', 'Life, Property & Casualty Insurance', 1),
    (103, 'Industry', 2, 2, 'PHARMA', 'Pharmaceuticals', 'Pharmaceutical and Biotech', 1),
    (104, 'Industry', 2, 2, 'MEDDEV', 'Medical Devices', 'Medical Technology and Devices', 1),
    (105, 'Industry', 2, 3, 'TECH', 'Technology', 'Software and Hardware Technology', 1),
    (106, 'Industry', 2, 3, 'MEDIA', 'Media', 'Traditional and Digital Media', 1);

-- Level 1 Capabilities
INSERT INTO [Taxonomy] (TaxonomyId, FacetType, Level, ParentId, Code, Name, Description, IsActive)
VALUES
    (201, 'Capability', 1, NULL, 'PI', 'Performance Improvement', 'Operational excellence and performance improvement', 1),
    (202, 'Capability', 1, NULL, 'STRAT', 'Strategy', 'Corporate and Business Strategy', 1),
    (203, 'Capability', 1, NULL, 'DDD', 'Digital, Data & Analytics', 'Digital transformation and analytics', 1),
    (204, 'Capability', 1, NULL, 'M&A', 'Mergers & Acquisitions', 'M&A strategy and execution', 1),
    (205, 'Capability', 1, NULL, 'ORG', 'Organization', 'Organizational design and change', 1),
    (206, 'Capability', 1, NULL, 'MKTG', 'Marketing & Sales', 'Marketing, sales and customer experience', 1);

-- Level 2 Capabilities (Sample)
INSERT INTO [Taxonomy] (TaxonomyId, FacetType, Level, ParentId, Code, Name, Description, IsActive)
VALUES
    (301, 'Capability', 2, 201, 'OPEX', 'Operational Excellence', 'Process optimization and operational efficiency', 1),
    (302, 'Capability', 2, 201, 'SUPPLY', 'Supply Chain', 'Supply chain management and optimization', 1),
    (303, 'Capability', 2, 203, 'ANALYTICS', 'Advanced Analytics', 'Data science and advanced analytics', 1),
    (304, 'Capability', 2, 203, 'DIGITAL', 'Digital Strategy', 'Digital transformation strategy', 1);

-- Keywords
INSERT INTO [Taxonomy] (TaxonomyId, FacetType, Level, ParentId, Code, Name, Description, IsActive)
VALUES
    (401, 'Keyword', 1, NULL, 'AI', 'Artificial Intelligence', 'AI and Machine Learning', 1),
    (402, 'Keyword', 1, NULL, 'ESG', 'ESG & Sustainability', 'Environmental, Social and Governance', 1),
    (403, 'Keyword', 1, NULL, 'CYBER', 'Cybersecurity', 'Information security and cyber risk', 1),
    (404, 'Keyword', 1, NULL, 'CLOUD', 'Cloud Technology', 'Cloud computing and migration', 1);

SET IDENTITY_INSERT [Taxonomy] OFF;

-- =============================================================================
-- SEED ROLE TYPES
-- =============================================================================

SET IDENTITY_INSERT [RoleTypes] ON;

-- Affiliation Roles
INSERT INTO [RoleTypes] (RoleTypeId, RoleCategory, RoleName, RoleCode, Description, RequiresPracticeSelection, RequiresLocationSelection, AllowsMultiplePerPractice, IsActive)
VALUES
    (1, 'Affiliation', 'L1', 'L1', 'Level 1 affiliation - primary expertise', 1, 0, 0, 1),
    (2, 'Affiliation', 'L2', 'L2', 'Level 2 affiliation - secondary expertise', 1, 0, 0, 1),
    (3, 'Affiliation', 'Connected', 'CONNECTED', 'Connected affiliation - emerging expertise', 1, 0, 0, 1),
    (4, 'Affiliation', 'Dedicated Affiliate', 'DEDICATED', 'Dedicated affiliate for specific practice', 1, 0, 0, 1);

-- Leadership Roles
INSERT INTO [RoleTypes] (RoleTypeId, RoleCategory, RoleName, RoleCode, Description, RequiresPracticeSelection, RequiresLocationSelection, AllowsMultiplePerPractice, IsActive)
VALUES
    (5, 'Leadership', 'Practice Leader', 'PRACTICE_LEADER', 'Practice area leadership role', 1, 1, 0, 1),
    (6, 'Leadership', 'Sector Leader', 'SECTOR_LEADER', 'Industry sector leadership', 1, 1, 0, 1),
    (7, 'Leadership', 'Solution Leader', 'SOLUTION_LEADER', 'Capability solution leadership', 1, 1, 0, 1),
    (8, 'Leadership', 'Interlock Champion', 'INTERLOCK', 'Cross-practice interlock champion', 1, 1, 1, 1),
    (9, 'Leadership', 'Local Practice Leader', 'LPL', 'Local practice leader', 1, 1, 0, 1);

-- Expertise Roles
INSERT INTO [RoleTypes] (RoleTypeId, RoleCategory, RoleName, RoleCode, Description, RequiresPracticeSelection, RequiresLocationSelection, AllowsMultiplePerPractice, IsActive)
VALUES
    (10, 'Expertise', 'Practice Certified Expert', 'CERT_EXPERT', 'Certified practice expert', 1, 0, 1, 1),
    (11, 'Expertise', 'Sector Experience', 'SECTOR_EXP', 'Industry sector experience', 1, 0, 1, 1),
    (12, 'Expertise', 'Solution Experience', 'SOLUTION_EXP', 'Capability solution experience', 1, 0, 1, 1);

-- Access Roles
INSERT INTO [RoleTypes] (RoleTypeId, RoleCategory, RoleName, RoleCode, Description, RequiresPracticeSelection, RequiresLocationSelection, AllowsMultiplePerPractice, IsActive)
VALUES
    (13, 'Access', 'Senior Practice Manager/VP', 'SPM_VP', 'Senior Practice Manager or VP access', 1, 1, 1, 1),
    (14, 'Access', 'Practice Consultant/Manager', 'PC_MGR', 'Practice Consultant or Manager access', 1, 1, 1, 1),
    (15, 'Access', 'Practice Analyst', 'PA', 'Practice Analyst access', 1, 1, 1, 1),
    (16, 'Access', 'Senior Knowledge Specialist/Manager', 'SKS_MGR', 'Senior Knowledge Specialist or Manager', 1, 1, 1, 1),
    (17, 'Access', 'Knowledge Associate/Specialist', 'KA_SPEC', 'Knowledge Associate or Specialist', 1, 1, 1, 1),
    (18, 'Access', 'Strategy & Operations', 'STRAT_OPS', 'Strategy and Operations team access', 0, 1, 1, 1),
    (19, 'Access', 'Operations Team', 'OPS_TEAM', 'Operations team access', 0, 1, 1, 1),
    (20, 'Access', 'Boss Reporting', 'BOSS_REPORT', 'Boss reporting system access', 0, 0, 1, 1),
    (21, 'Access', 'Reporting', 'REPORTING', 'General reporting access', 0, 0, 1, 1);

SET IDENTITY_INSERT [RoleTypes] OFF;

-- =============================================================================
-- SEED USER ROLES
-- =============================================================================

SET IDENTITY_INSERT [UserRoles] ON;

INSERT INTO [UserRoles] (UserRoleId, RoleName, Description, IsSystemRole)
VALUES
    (1, 'SystemAdmin', 'System Administrator - Full access to all data and settings', 1),
    (2, 'OfficeStaffing', 'Office Staffing Teams - Manage employees in their office', 0),
    (3, 'PPKGlobalLead', 'PPK Global S&O Leads - Global practice management', 0),
    (4, 'PPKRegionalLead', 'PPK Regional S&O Leads - Regional practice management', 0),
    (5, 'PPKProgramTeam', 'PPK Program Team - System access role management', 0),
    (6, 'StaffingSystemSupport', 'Staffing System Support - Boss reporting management', 0);

SET IDENTITY_INSERT [UserRoles] OFF;

-- =============================================================================
-- SEED SAMPLE EMPLOYEES (FOR DEVELOPMENT)
-- =============================================================================

SET IDENTITY_INSERT [Employees] ON;

INSERT INTO [Employees] (EmployeeId, EmployeeCode, FirstName, LastName, Email, HomeOfficeId, Level, Title, IsActive, HireDate)
VALUES
    (1, 'E001', 'John', 'Smith', 'john.smith@bain.com', 1, 'Partner', 'Partner', 1, '2015-01-15'),
    (2, 'E002', 'Sarah', 'Johnson', 'sarah.johnson@bain.com', 2, 'Principal', 'Principal', 1, '2018-03-20'),
    (3, 'E003', 'Michael', 'Chen', 'michael.chen@bain.com', 5, 'Manager', 'Manager', 1, '2020-06-10'),
    (4, 'E004', 'Emily', 'Davis', 'emily.davis@bain.com', 9, 'Senior Manager', 'Senior Manager', 1, '2019-09-05'),
    (5, 'E005', 'David', 'Wilson', 'david.wilson@bain.com', 1, 'Associate Principal', 'Associate Principal', 1, '2017-11-12');

SET IDENTITY_INSERT [Employees] OFF;

-- =============================================================================
-- SEED SAMPLE AFFILIATIONS (FOR DEVELOPMENT)
-- =============================================================================

SET IDENTITY_INSERT [EmployeeAffiliations] ON;

INSERT INTO [EmployeeAffiliations] (AffiliationId, EmployeeId, RoleTypeId, PracticeId, LocationId, LocationScope, EffectiveDate, Source, CreatedBy, ModifiedBy)
VALUES
    (1, 1, 1, 1, NULL, 'Global', '2015-01-15', 'Manual', 'System', 'System'), -- John Smith - L1 Financial Services
    (2, 1, 2, 201, NULL, 'Global', '2016-01-01', 'Manual', 'System', 'System'), -- John Smith - L2 Performance Improvement
    (3, 2, 1, 2, NULL, 'Regional', '2018-03-20', 'Manual', 'System', 'System'), -- Sarah Johnson - L1 Healthcare
    (4, 3, 2, 3, NULL, 'Regional', '2020-06-10', 'Manual', 'System', 'System'), -- Michael Chen - L2 TMT
    (5, 4, 1, 203, NULL, 'Regional', '2019-09-05', 'Manual', 'System', 'System'), -- Emily Davis - L1 Digital
    (6, 5, 3, 4, NULL, 'Local', '2017-11-12', 'Manual', 'System', 'System'); -- David Wilson - Connected PE

SET IDENTITY_INSERT [EmployeeAffiliations] OFF;

-- =============================================================================
-- SEED SAMPLE USERS (FOR DEVELOPMENT)
-- =============================================================================

SET IDENTITY_INSERT [Users] ON;

INSERT INTO [Users] (UserId, EmployeeId, UserPrincipalName, DisplayName, Email, IsActive)
VALUES
    (1, 1, 'john.smith@bain.com', 'John Smith', 'john.smith@bain.com', 1),
    (2, 2, 'sarah.johnson@bain.com', 'Sarah Johnson', 'sarah.johnson@bain.com', 1),
    (3, NULL, 'admin@bain.com', 'System Administrator', 'admin@bain.com', 1);

SET IDENTITY_INSERT [Users] OFF;

-- =============================================================================
-- SEED USER ROLE ASSIGNMENTS (FOR DEVELOPMENT)
-- =============================================================================

SET IDENTITY_INSERT [UserRoleAssignments] ON;

INSERT INTO [UserRoleAssignments] (UserRoleAssignmentId, UserId, UserRoleId, PracticeId, LocationId, IsActive, AssignedBy, AssignedDate)
VALUES
    (1, 3, 1, NULL, NULL, 1, 'System', GETUTCDATE()), -- Admin - System Admin
    (2, 1, 3, 1, NULL, 1, 'System', GETUTCDATE()), -- John Smith - PPK Global Lead for FS
    (3, 2, 2, NULL, 2, 1, 'System', GETUTCDATE()); -- Sarah Johnson - Office Staffing for Boston

SET IDENTITY_INSERT [UserRoleAssignments] OFF;

PRINT 'PAD 2.0 Seed Data inserted successfully';
PRINT 'Sample employees, affiliations, and users created for development';
PRINT 'Remember to update Azure AD configuration and user mappings for production'; 