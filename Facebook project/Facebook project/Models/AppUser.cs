using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models
{
    public enum Gender
    {
        Female, Male
    }
    public class AppUser:IdentityUser
    {
        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }
        [Display(Name = "BirthDate")]
        
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string Bio { get; set; }
        public bool? isBlocked { get; set; }
        public string PhotoURL { get; set; }

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }
        [JsonIgnore]
        public virtual ICollection<Like> Likes { get; set; }
        [JsonIgnore]
        public virtual ICollection<Friend> Friends { get; set; }
    }
}
