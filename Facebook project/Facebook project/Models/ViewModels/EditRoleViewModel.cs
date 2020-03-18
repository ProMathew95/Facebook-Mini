using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            USers = new List<string>();
        }
        public string Id { get; set; }
        [Required(ErrorMessage ="Role Name Required")]
        public string RoleName { get; set; }
        public string Description { get; set; }
        public List<string> USers { get; set; }
    }
}
