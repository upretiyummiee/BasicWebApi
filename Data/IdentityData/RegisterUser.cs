using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BasicWebApi.Data.IdentityData
{
    public class RegisterUser
    {

        [StringLength(15)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(15)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(35)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
