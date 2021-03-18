using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BasicWebApi.Data.IdentityData
{
    public class IdentityUserInherit: IdentityUser
    {
        [Display(Name = "First Name")]
        [StringLength(15)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(15)]
        public string LastName { get; set; }

    }
}
