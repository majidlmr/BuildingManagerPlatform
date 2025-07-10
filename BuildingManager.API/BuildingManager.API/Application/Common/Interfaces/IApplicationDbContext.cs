// File: Application/Common/Interfaces/IApplicationDbContext.cs
using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Net.Sockets;

namespace BuildingManager.API.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Block> Blocks { get; } // Renamed from Buildings
    DbSet<Unit> Units { get; }
    DbSet<UnitAssignment> UnitAssignments { get; } // Renamed from ResidentAssignments
    DbSet<SettlementAccount> SettlementAccounts { get; }
    DbSet<Asset> Assets { get; }
    DbSet<AssetAssignment> AssetAssignments { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketUpdate> TicketUpdates { get; }
    DbSet<TicketAttachment> TicketAttachments { get; } // Added
    DbSet<Invoice> Invoices { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Withdrawal> Withdrawals { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<ConversationParticipant> ConversationParticipants { get; } // Renamed from Participants
    DbSet<Message> Messages { get; }
    DbSet<BillingCycle> BillingCycles { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<ExpenseType> ExpenseTypes { get; } // Added
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
    DbSet<UserRoleAssignment> UserRoleAssignments { get; } // Renamed from UserRoles
    DbSet<RolePermission> RolePermissions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}