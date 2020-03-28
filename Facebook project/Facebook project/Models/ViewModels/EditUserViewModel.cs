using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models.ViewModels
{




    public class EditUserViewModel
    {

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string id { get; set; }



        //public EditUserViewModel()
        //{
        //    Roles = new List<string>();
        //}



        //[Required]
        //public string UserName { get; set; }
        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }


        //public IList<string> Roles { get; set; }
    }
}
