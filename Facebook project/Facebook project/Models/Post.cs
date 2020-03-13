using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Facebook_project.Models
{
    public class Post
    {
        [Key]
        [Required]
        public int PostId { get; set; }

        public bool? isDeleted { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int? numberOfLikes { get; set; }


        //Content
        public string Text { get; set; }

        public string PictureURL { get; set; }
        //


        [Required]
        public string PublisherId { get; set; }


        //[JsonIgnore]
        [ForeignKey("PublisherId")]
        public virtual AppUser Publisher { get; set; }

        //[JsonIgnore]
        public virtual ICollection<Comment> Comment { get; set; }

        //[JsonIgnore]
        public virtual ICollection<Like> Like { get; set; }



        //[Required]
        //[JsonIgnore]
        //public virtual Content content { get; set; }
    }
}
