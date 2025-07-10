using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Identity; // For PasswordHasher if seeding users
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingManager.API.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            // This method is more for ASP.NET Core Identity.
            // We are using a custom User/Role system, so direct DbContext seeding is preferred for now.
            // Kept for potential future integration with Identity.
            await Task.CompletedTask;
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Seed Permissions
                if (!await context.Permissions.AnyAsync())
                {
                    var permissions = new List<Permission>
                    {
                        // System Permissions
                        new Permission { Name = "Permissions.System.ManagePlatformSettings", Module = "System", Description = "Manage overall platform settings" },
                        new Permission { Name = "Permissions.System.ViewAuditLogs", Module = "System", Description = "View system-wide audit logs" },

                        // User Management Permissions
                        new Permission { Name = "Permissions.User.ViewAnyProfile", Module = "UserManagement", Description = "View any user's profile" },
                        new Permission { Name = "Permissions.User.EditAnyProfile", Module = "UserManagement", Description = "Edit any user's profile" },
                        new Permission { Name = "Permissions.User.DeleteAnyUser", Module = "UserManagement", Description = "Delete any user" },
                        new Permission { Name = "Permissions.User.AssignRoles", Module = "UserManagement", Description = "Assign roles to users" },
                        new Permission { Name = "Permissions.User.VerifyManagers", Module = "UserManagement", Description = "Verify or reject manager role assignments" },

                        // Role Management Permissions
                        new Permission { Name = "Permissions.Role.Create", Module = "RoleManagement", Description = "Create new roles" },
                        new Permission { Name = "Permissions.Role.View", Module = "RoleManagement", Description = "View roles" },
                        new Permission { Name = "Permissions.Role.Edit", Module = "RoleManagement", Description = "Edit roles" },
                        new Permission { Name = "Permissions.Role.Delete", Module = "RoleManagement", Description = "Delete roles" },
                        new Permission { Name = "Permissions.Role.AssignPermissions", Module = "RoleManagement", Description = "Assign permissions to roles" },

                        // Complex Management Permissions
                        new Permission { Name = "Permissions.Complex.Create", Module = "ComplexManagement", Description = "Create new complexes" },
                        new Permission { Name = "Permissions.Complex.View", Module = "ComplexManagement", Description = "View complex details" },
                        new Permission { Name = "Permissions.Complex.Update", Module = "ComplexManagement", Description = "Update complex details" },
                        new Permission { Name = "Permissions.Complex.Delete", Module = "ComplexManagement", Description = "Delete complexes" },
                        new Permission { Name = "Permissions.Complex.ManageBlocks", Module = "ComplexManagement", Description = "Manage blocks within own complex(es)" },
                        new Permission { Name = "Permissions.Complex.AssignComplexManager", Module = "ComplexManagement", Description = "Assign other managers to own complex(es)" },

                        // Block Management Permissions
                        new Permission { Name = "Permissions.Block.Create", Module = "BlockManagement", Description = "Create new blocks (standalone or within managed complex)" },
                        new Permission { Name = "Permissions.Block.View", Module = "BlockManagement", Description = "View block details" },
                        new Permission { Name = "Permissions.Block.Update", Module = "BlockManagement", Description = "Update block details" },
                        new Permission { Name = "Permissions.Block.Delete", Module = "BlockManagement", Description = "Delete blocks" },
                        new Permission { Name = "Permissions.Block.ManageUnits", Module = "BlockManagement", Description = "Manage units within own block(s)" },
                        new Permission { Name = "Permissions.Block.AssignBlockManager", Module = "BlockManagement", Description = "Assign other managers to own block(s)" },

                        // Unit Management Permissions
                        new Permission { Name = "Permissions.Unit.Create", Module = "UnitManagement", Description = "Create units within managed block(s)" },
                        new Permission { Name = "Permissions.Unit.View", Module = "UnitManagement", Description = "View unit details" },
                        new Permission { Name = "Permissions.Unit.Update", Module = "UnitManagement", Description = "Update unit details" },
                        new Permission { Name = "Permissions.Unit.Delete", Module = "UnitManagement", Description = "Delete units" },
                        new Permission { Name = "Permissions.Unit.AssignResident", Module = "UnitManagement", Description = "Assign residents to units" },

                        // Billing Permissions
                        new Permission { Name = "Permissions.Billing.GenerateInvoices", Module = "Billing", Description = "Generate invoices for units/blocks" },
                        new Permission { Name = "Permissions.Billing.ViewAnyInvoice", Module = "Billing", Description = "View any invoice in managed entities" },
                        new Permission { Name = "Permissions.Billing.ManagePayments", Module = "Billing", Description = "Manage payment records" },
                        new Permission { Name = "Permissions.Billing.ConfigureSettings", Module = "Billing", Description = "Configure billing settings for block/complex" },
                        new Permission { Name = "Permissions.Billing.ViewOwnInvoices", Module = "Billing", Description = "View own invoices (for residents)" },
                        new Permission { Name = "Permissions.Billing.PayOwnInvoices", Module = "Billing", Description = "Pay own invoices (for residents)" },

                        // Ticketing Permissions
                        new Permission { Name = "Permissions.Ticket.Create", Module = "Ticketing", Description = "Create new tickets (for residents/managers)" },
                        new Permission { Name = "Permissions.Ticket.ViewOwn", Module = "Ticketing", Description = "View own tickets" },
                        new Permission { Name = "Permissions.Ticket.ViewAssigned", Module = "Ticketing", Description = "View tickets assigned to self" },
                        new Permission { Name = "Permissions.Ticket.ViewAllInScope", Module = "Ticketing", Description = "View all tickets in managed block/complex" },
                        new Permission { Name = "Permissions.Ticket.Update", Module = "Ticketing", Description = "Update ticket status, assignment, priority" },
                        new Permission { Name = "Permissions.Ticket.Delete", Module = "Ticketing", Description = "Delete tickets" },

                        // Announcement Permissions
                        new Permission { Name = "Permissions.Announcement.Create", Module = "Communication", Description = "Create announcements" },
                        new Permission { Name = "Permissions.Announcement.View", Module = "Communication", Description = "View announcements" },
                        new Permission { Name = "Permissions.Announcement.Delete", Module = "Communication", Description = "Delete announcements" },

                        // Poll Permissions
                        new Permission { Name = "Permissions.Poll.Create", Module = "Communication", Description = "Create polls" },
                        new Permission { Name = "Permissions.Poll.View", Module = "Communication", Description = "View polls and results" },
                        new Permission { Name = "Permissions.Poll.Vote", Module = "Communication", Description = "Vote in polls" },
                        new Permission { Name = "Permissions.Poll.Delete", Module = "Communication", Description = "Delete polls" },
                    };
                    await context.Permissions.AddRangeAsync(permissions);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} permissions.", permissions.Count);
                }

                // Seed Roles
                if (!await context.Roles.AnyAsync())
                {
                    var roles = new List<Role>
                    {
                        new Role { Name = "Super Administrator", NormalizedName = "SUPERADMINISTRATOR", Description = "Full system access", AppliesToHierarchyLevel = HierarchyLevel.System, IsSystemRole = true },
                        new Role { Name = "Complex Manager", NormalizedName = "COMPLEXMANAGER", Description = "Manages a specific complex and its blocks", AppliesToHierarchyLevel = HierarchyLevel.Complex, IsSystemRole = false },
                        new Role { Name = "Block Manager", NormalizedName = "BLOCKMANAGER", Description = "Manages a specific block and its units", AppliesToHierarchyLevel = HierarchyLevel.Block, IsSystemRole = false },
                        new Role { Name = "Resident - Owner", NormalizedName = "RESIDENTOWNER", Description = "Owner of a unit", AppliesToHierarchyLevel = HierarchyLevel.Block, IsSystemRole = false },
                        new Role { Name = "Resident - Tenant", NormalizedName = "RESIDENTTENANT", Description = "Tenant of a unit", AppliesToHierarchyLevel = HierarchyLevel.Block, IsSystemRole = false },
                        new Role { Name = "Complex Board Member", NormalizedName = "COMPLEXBOARDMEMBER", Description = "Board member for a complex", AppliesToHierarchyLevel = HierarchyLevel.Complex, IsSystemRole = false },
                        new Role { Name = "Block Board Member", NormalizedName = "BLOCKBOARDMEMBER", Description = "Board member for a block", AppliesToHierarchyLevel = HierarchyLevel.Block, IsSystemRole = false },
                    };
                    await context.Roles.AddRangeAsync(roles);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} roles.", roles.Count);

                    // Seed RolePermissions (Example for SuperAdmin - has all permissions)
                    var superAdminRole = await context.Roles.FirstAsync(r => r.NormalizedName == "SUPERADMINISTRATOR");
                    var allPermissions = await context.Permissions.ToListAsync();
                    foreach (var perm in allPermissions)
                    {
                        context.RolePermissions.Add(new RolePermission { RoleId = superAdminRole.Id, PermissionId = perm.Id });
                    }

                    // Example: Assign some permissions to ComplexManager
                    var complexManagerRole = await context.Roles.FirstAsync(r => r.NormalizedName == "COMPLEXMANAGER");
                    var complexManagerPermissions = new List<string> {
                        "Permissions.Complex.View", "Permissions.Complex.Update", "Permissions.Complex.ManageBlocks",
                        "Permissions.Block.View", "Permissions.Block.ManageUnits",
                        "Permissions.Announcement.Create", "Permissions.Announcement.View",
                        "Permissions.Poll.Create", "Permissions.Poll.View",
                        "Permissions.Billing.GenerateInvoices", "Permissions.Billing.ViewAnyInvoice", "Permissions.Billing.ConfigureSettings",
                        "Permissions.Ticket.ViewAllInScope", "Permissions.Ticket.Update"
                    };
                    foreach (var permName in complexManagerPermissions)
                    {
                        var perm = await context.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
                        if (perm != null)
                        {
                            context.RolePermissions.Add(new RolePermission { RoleId = complexManagerRole.Id, PermissionId = perm.Id });
                        }
                    }
                    // ... Seed other role permissions as needed ...

                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded initial RolePermissions.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw; // Re-throw the exception to indicate seeding failure
            }
        }
    }
}
