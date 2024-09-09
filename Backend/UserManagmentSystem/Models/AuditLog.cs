using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserManagmentSystem.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; }
        [StringLength(50)]
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
