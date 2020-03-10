using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models
{
    public class Comment
    {
        [Required]
        public int PostID { get; set; }
        [Required]
        public string UserID { get; set; }

        public bool isRemoved { get; set; }

        //[JsonIgnore]
        [ForeignKey("UserID")]
        public virtual AppUser User { get; set; }
        //[JsonIgnore]
        [ForeignKey("PostID")]
        public virtual Post Post { get; set; }

        public string Text { get; set; }

        public string PictureURL { get; set; }

    }
}
