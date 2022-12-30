using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using SvivaTeamVersion3.Data;

namespace SvivaTeamVersion3.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        //Experimental
        [Required]
        [IsTrue(true, ErrorMessage = "You must agree to the Terms of Service")]
        //[Display(Name = "I have read and accept the Terms of Service")]
        //[Column(TypeName = "bool")]
        public bool AcceptedTOS { get; set; }

        [PersonalData]
        [Required]
        [MinimumAge(13)]
        public DateTime DOB { get; set; }
    }
}
