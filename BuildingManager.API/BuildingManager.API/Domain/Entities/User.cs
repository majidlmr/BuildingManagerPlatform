using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities
{
    /// <summary>
    /// Represents a user in the system.
    /// Holds identity and communication information for the user.
    /// </summary>
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(75)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(75)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(10)] // Assuming National ID is 10 digits
        public string NationalId { get; set; } // National ID, should be unique

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } // Primary identifier for login and communication

        public bool PhoneNumberConfirmed { get; set; } = false;

        [MaxLength(255)]
        public string? Email { get; set; } // Optional, but should be unique if provided

        public bool EmailConfirmed { get; set; } = false;

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(500)]
        public string? ProfilePictureUrl { get; set; }
        public DateTime? LastLoginAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true; // To allow deactivating users

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }


        // --- Navigation Properties for Relationships ---
        public ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
        public ICollection<UnitAssignment> UnitAssignments { get; set; } = new List<UnitAssignment>(); // Units this user is assigned to (as owner/tenant)
        public ICollection<SettlementAccount> SettlementAccounts { get; set; } = new List<SettlementAccount>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();


        // --- Constructors ---
        public User() { }

        public User(string firstName, string lastName, string nationalId, string phoneNumber, string passwordHash)
        {
            FirstName = firstName;
            LastName = lastName;
            NationalId = nationalId;
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash;
        }
    }
}