using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models.ViewModels
{
    public class ResponseViewModel
    {
        public int PostId  { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public string PicURL { get; set; }
        public string UserPicURL { get; set; }
    }
}
