using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SvivaTeamVersion3.Models
{
    public class UserRoleModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsSelected { get; set; }
    }
}
