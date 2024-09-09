using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace UserManagmentSystem.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
