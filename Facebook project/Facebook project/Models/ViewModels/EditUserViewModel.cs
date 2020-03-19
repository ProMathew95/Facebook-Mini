using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Roles = new List<string>();        
        }
        public string id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required][EmailAddress]
        public string Email { get; set; }
       

        public IList<string> Roles { get;  set; }
    }
}
