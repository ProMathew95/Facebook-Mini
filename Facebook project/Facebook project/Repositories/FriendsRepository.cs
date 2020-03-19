using Facebook_project.Data;
using Facebook_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Repositories
{
    public class FriendsRepository
    {
        ApplicationDbContext _context;
        public FriendsRepository(ApplicationDbContext db)
        {
            _context = db;
        }

        public List<AppUser> GetFriendRequests(string UserId)
        {
            return _context.Friends.Include(f => f.SenderUser).
                Where(f => f.receiverUserID == UserId
                && f.Status == Status.RequestPending).Select(f => f.SenderUser).ToList();
        }

        public void changeRequestStatus(string senderId, string userId, Status status)
        {
            var result = _context.Friends.Single(f => f.senderUserID == senderId && f.receiverUserID == userId);
            if (result != null)
                result.Status = status;
            _context.SaveChanges();
        }

        public List<AppUser> Search(string searchKey)
        {
            return _context.AppUsers.Where(u => u.FullName.Contains(searchKey)).ToList();
        }
    }
}
