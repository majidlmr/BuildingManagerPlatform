using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BuildingManager.API.Infrastructure.Persistence.DbContexts;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // --- DbSets ---
    public DbSet<User> Users { get; set; }
    public DbSet<Complex> Complexes { get; set; } // Added Complex
    public DbSet<Block> Blocks { get; set; } // Renamed from Buildings to Blocks
    public DbSet<Unit> Units { get; set; }
    // public DbSet<ResidentAssignment> ResidentAssignments { get; set; } // Will be renamed/replaced by UnitAssignment later
    public DbSet<UnitAssignment> UnitAssignments { get; set; } // Added UnitAssignment
    public DbSet<SettlementAccount> SettlementAccounts { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetAssignment> AssetAssignments { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketUpdate> TicketUpdates { get; set; }
    public DbSet<TicketAttachment> TicketAttachments { get; set; } // Added
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    // public DbSet<Participant> Participants { get; set; } // Will be renamed/replaced by ConversationParticipant later
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; } // Added ConversationParticipant
    public DbSet<Message> Messages { get; set; }
    public DbSet<BillingCycle> BillingCycles { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseType> ExpenseTypes { get; set; } // Added
    public DbSet<Revenue> Revenues { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<BuildingRule> BuildingRules { get; set; } // Might need renaming if rules are per Complex/Block
    public DbSet<RuleAcknowledgment> RuleAcknowledgments { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ManagerAssignment> ManagerAssignments { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    // public DbSet<UserRole> UserRoles { get; set; } // Will be renamed/replaced by UserRoleAssignment later
    public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; } // Added UserRoleAssignment
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // اعمال تمام پیکربندی‌ها از اسمبلی فعلی
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}