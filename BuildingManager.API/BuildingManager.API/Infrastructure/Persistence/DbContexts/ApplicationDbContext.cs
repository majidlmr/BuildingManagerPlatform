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
    public DbSet<Building> Buildings { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<ResidentAssignment> ResidentAssignments { get; set; }
    public DbSet<SettlementAccount> SettlementAccounts { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetAssignment> AssetAssignments { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketUpdate> TicketUpdates { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<BillingCycle> BillingCycles { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Revenue> Revenues { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<BuildingRule> BuildingRules { get; set; }
    public DbSet<RuleAcknowledgment> RuleAcknowledgments { get; set; }

     public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ManagerAssignment> ManagerAssignments { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // اعمال تمام پیکربندی‌ها از اسمبلی فعلی
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}