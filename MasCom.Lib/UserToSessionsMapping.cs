using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasCom.Lib
{
    [Table("UserToSessionsMappings")]
    public class UserToSessionsMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string P2PSid { get; set; }
        public string GroupSid { get; set; }
        public string FileDeliverySid { get; set; }
        public string FileHeadersDeliverySid { get; set; }
        public string VideoCallSid { get; set; }
        public string VideoConfSid { get; set; }
    }
}
