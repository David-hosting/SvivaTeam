
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SvivaTeamVersion3.Models
{
    public class CreateRoleModel
    {
        [Required(ErrorMessage = "This field is required")]
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
    }
}
