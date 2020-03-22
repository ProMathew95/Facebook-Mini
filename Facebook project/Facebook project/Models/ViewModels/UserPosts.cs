using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models.ViewModels
{
    public class UserPosts
    {
        public AppUser myUser { get; set; }
        public List<Post> allpost { get; set; }
    }
}
