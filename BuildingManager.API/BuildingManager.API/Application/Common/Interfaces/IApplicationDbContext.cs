// File: Application/Common/Interfaces/IApplicationDbContext.cs
using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Net.Sockets;

namespace BuildingManager.API.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Building> Buildings { get; }
    DbSet<Unit> Units { get; }
    DbSet<ResidentAssignment> ResidentAssignments { get; }
    DbSet<SettlementAccount> SettlementAccounts { get; }
    DbSet<Asset> Assets { get; }
    DbSet<AssetAssignment> AssetAssignments { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketUpdate> TicketUpdates { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Withdrawal> Withdrawals { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Participant> Participants { get; }
    DbSet<Message> Messages { get; }
    DbSet<BillingCycle> BillingCycles { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<Revenue> Revenues { get; }
    DbSet<Announcement> Announcements { get; }

    DbSet<Poll> Polls { get; }
    DbSet<PollOption> PollOptions { get; }
    DbSet<Vote> Votes { get; }
    DbSet<BuildingRule> BuildingRules { get; }
    DbSet<RuleAcknowledgment> RuleAcknowledgments { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<ManagerAssignment> ManagerAssignments { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}