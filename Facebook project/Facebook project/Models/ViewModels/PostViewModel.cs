using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models.ViewModels
{
    public class PostViewModel
    {
        public List<Post> IncommingPosts { get; set; }
        public Post post { get; set; }
        //public IEnumerable<Comment> comments { get; set; }
        //public IEnumerable<Like> likes { get; set; }
        //public AppUser publisher { get; set; }
    }
}
