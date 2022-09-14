using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasCom.Lib
{
    [Table("Admins")]
    public class Admin
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(maximumLength: 128, MinimumLength = 2)]
        public string LastName { get; set; }

        [StringLength(maximumLength: 128, MinimumLength = 2)]
        public string FirstName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(maximumLength: 128, MinimumLength = 2)]
        public string UserName { get; set; }

        [Required]
        public string PassworHash { get; set; }

        public DateTime DateJoined { get; set; }
        public DateTime LastLogin { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsStaff { get; set; } = false;
        public bool IsSuperAdmin { get; set; } = false;
    }
}
