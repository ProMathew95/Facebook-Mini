using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Models
{
    public enum Status
    {
        RequestPending,
        Blocked,//for future work
        RequestConfirmed,
        FriendRemoved,
        RequestCanceled,
        RequestRejected
    }
    public class Friend
    {
        [Key, Column(Order = 0)]
        public string senderUserID { get; set; }

        [Key, Column(Order = 1)]
        public string receiverUserID { get; set; }
        [Required]
        public Status Status { get; set; }
        [JsonIgnore]
        [ForeignKey("senderUserID")]
        public virtual AppUser SenderUser { get; set; }
        [JsonIgnore]
        [ForeignKey("receiverUserID")]
        public virtual AppUser RecieverUser { get; set; }
    }
}
