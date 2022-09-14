using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasCom.Lib
{
    [Table("Users")]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(maximumLength: 128, MinimumLength = 2)]
        public string UserName { get; set; }

        [Required, StringLength(maximumLength: 128, MinimumLength = 2)]
        public string Name { get; set; }

        [Required, StringLength(maximumLength: 128, MinimumLength = 2)]
        public string LastName { get; set; }

        public string PasswordHash { get; set; }

        [NotMapped]
        public virtual UserToSessionsMapping SessionMapping { get; set; }
    }
}
