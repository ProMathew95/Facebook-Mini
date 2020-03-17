
using Facebook_project.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models
{
    public class AppRole: IdentityRole
    {
      
        public string Description { get; set; }
       
    }
}
