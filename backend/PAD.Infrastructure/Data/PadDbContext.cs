using Microsoft.EntityFrameworkCore;
using PAD.Core.Entities;

namespace PAD.Infrastructure.Data;

public class PadDbContext : DbContext
{
    public PadDbContext(DbContextOptions<PadDbContext> options) : base(options)
    {
    }

    // Master Data
    public DbSet<Office> Offices { get; set; }
    public DbSet<Taxonomy> Taxonomies { get; set; }
    public DbSet<RoleType> RoleTypes { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    // Employee Data
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeAffiliation> EmployeeAffiliations { get; set; }
    public DbSet<EmployeeRole> EmployeeRoles { get; set; }

    // User Management
    public DbSet<User> Users { get; set; }
    public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }

    // Audit and Logging
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    // Integration
    public DbSet<WorkdaySyncStatus> WorkdaySyncStatuses { get; set; }
    public DbSet<IntegrationLog> IntegrationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema
        modelBuilder.HasDefaultSchema("pad");

        // Configure Office entity
        modelBuilder.Entity<Office>(entity =>
        {
            entity.HasKey(e => e.OfficeId);
            entity.Property(e => e.OfficeCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.OfficeName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Region).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Cluster).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.OfficeCode).IsUnique();
        });

        // Configure Taxonomy entity
        modelBuilder.Entity<Taxonomy>(entity =>
        {
            entity.HasKey(e => e.TaxonomyId);
            entity.Property(e => e.FacetType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.Children)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.ToTable(t => t.HasCheckConstraint("CK_Taxonomy_FacetType", "[FacetType] IN ('Industry', 'Capability', 'Keyword')"));
            entity.ToTable(t => t.HasCheckConstraint("CK_Taxonomy_Level", "[Level] BETWEEN 1 AND 3"));
        });

        // Configure RoleType entity
        modelBuilder.Entity<RoleType>(entity =>
        {
            entity.HasKey(e => e.RoleTypeId);
            entity.Property(e => e.RoleCategory).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RoleCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.RoleCode).IsUnique();
        });

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);
            entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
            
            entity.HasIndex(e => e.EmployeeCode).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(e => e.HomeOffice)
                  .WithMany(o => o.Employees)
                  .HasForeignKey(e => e.HomeOfficeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure EmployeeAffiliation entity
        modelBuilder.Entity<EmployeeAffiliation>(entity =>
        {
            entity.HasKey(e => e.AffiliationId);
            entity.Property(e => e.Source).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LocationScope).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).IsRequired().HasMaxLength(100);

            entity.ToTable(t => t.HasCheckConstraint("CK_EmployeeAffiliation_LocationScope", "[LocationScope] IN ('Global', 'Regional', 'Local')"));
            entity.ToTable(t => t.HasCheckConstraint("CK_EmployeeAffiliation_Source", "[Source] IN ('Manual', 'Workday', 'Import')"));

            entity.HasOne(e => e.Employee)
                  .WithMany(e => e.Affiliations)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.RoleType)
                  .WithMany()
                  .HasForeignKey(e => e.RoleTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Practice)
                  .WithMany()
                  .HasForeignKey(e => e.PracticeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Location)
                  .WithMany()
                  .HasForeignKey(e => e.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure EmployeeRole entity
        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmployeeRoleId);
            entity.Property(e => e.LocationScope).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.Roles)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.RoleType)
                  .WithMany(rt => rt.Roles)
                  .HasForeignKey(e => e.RoleTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.PrimaryPractice)
                  .WithMany(t => t.PrimaryRoles)
                  .HasForeignKey(e => e.PrimaryPracticeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.SecondaryPractice)
                  .WithMany(t => t.SecondaryRoles)
                  .HasForeignKey(e => e.SecondaryPracticeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Location)
                  .WithMany(o => o.Roles)
                  .HasForeignKey(e => e.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserPrincipalName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.UserPrincipalName).IsUnique();
            
            entity.HasOne(e => e.Employee)
                  .WithOne()
                  .HasForeignKey<User>(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure UserRole entity
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Configure UserRoleAssignment entity
        modelBuilder.Entity<UserRoleAssignment>(entity =>
        {
            entity.HasKey(e => e.UserRoleAssignmentId);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserRoleAssignments)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.UserRole)
                  .WithMany(r => r.UserRoleAssignments)
                  .HasForeignKey(e => e.UserRoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(20);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).IsRequired().HasMaxLength(50);
        });

        // Configure ActivityLog entity
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.ActivityId);
            entity.Property(e => e.ActivityType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Details).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(500);
            entity.Property(e => e.IPAddress).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UserAgent).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure WorkdaySyncStatus entity
        modelBuilder.Entity<WorkdaySyncStatus>(entity =>
        {
            entity.HasKey(e => e.SyncId);
            entity.Property(e => e.SyncType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ErrorMessage).HasMaxLength(500);
            
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkdaySyncStatus_Status", "[Status] IN ('Pending', 'InProgress', 'Completed', 'Failed')"));
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkdaySyncStatus_SyncType", "[SyncType] IN ('Full', 'Incremental')"));
        });

        // Configure IntegrationLog entity
        modelBuilder.Entity<IntegrationLog>(entity =>
        {
            entity.HasKey(e => e.IntegrationLogId);
            entity.Property(e => e.SystemName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IntegrationType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RequestType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Operation).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.RequestPayload).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.ResponsePayload).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.ErrorMessage).HasMaxLength(500);
        });
    }
} 