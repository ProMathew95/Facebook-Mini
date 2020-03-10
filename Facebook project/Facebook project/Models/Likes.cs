using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models
{
    public class Like
    {
        [Required]
        [Key, Column(Order = 0)]
        public string UserID { get; set; }

        [Required]
        [Key, Column(Order = 1)]
        public int PostID { get; set; }

        public bool isLiked { get; set; }

        //[JsonIgnore]
        [ForeignKey("UserID")]
        public virtual AppUser User { get; set; }

        //[JsonIgnore]
        [ForeignKey("PostID")]
        public virtual Post Post { get; set; }
    }
}
