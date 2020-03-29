using Facebook_project.Data;
using Facebook_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Repositories
{
	public class AppUserRepository
	{
		ApplicationDbContext _context;
		public AppUserRepository(ApplicationDbContext db)
		{
			_context = db;
		}
		public AppUser GetUserByid(string userId)
		{
			var appUser = _context.AppUsers.Include(c => c.Posts).ThenInclude(c=>(c as Post).Comment).Include(c => c.Posts).ThenInclude(c=>(c as Post).Like ).Include(c => c.Friends).FirstOrDefault(c => c.Id == userId);
			return appUser;
		}
		public bool isBlocked(string userID)
		{
			var user = _context.AppUsers.FirstOrDefault(u => u.Id == userID);
			if(user!=null)
			{
				if (user.isBlocked == null)
					return false;
				return (bool)user.isBlocked;
			}
			return true;
		}
		public AppUser UpdateUserInfo(AppUser user)
		{
			if (user != null)
			{
				var appUSer = _context.AppUsers.FirstOrDefault(u => u.Id == user.Id);
				if (appUSer != null)
				{
					appUSer.FullName = user.FullName;
					appUSer.Bio = user.Bio;
					appUSer.BirthDate = user.BirthDate;
					appUSer.Gender = user.Gender;
				}
				_context.SaveChanges();
				return appUSer;
			}
			return null;
		}
		public AppUser UpdateImageUser(string id, string PictureUrl, bool removeImg)
		{
			var AppUser = _context.AppUsers.FirstOrDefault(u => u.Id == id);

			if (AppUser != null)
			{

				if (removeImg)
					AppUser.PhotoURL = "default.jpg";


				else if (PictureUrl != "")
					AppUser.PhotoURL = PictureUrl;



			}

			_context.SaveChanges();
			return AppUser;
		}


	      

		public void ChangeRelation(string senderId, string receiverId, string status)
        {
            var prevActivity = _context.Friends.FirstOrDefault(f => f.senderUserID == senderId || f.senderUserID == receiverId);
            if (prevActivity != null)
                prevActivity.Status = (Status)Enum.Parse(typeof(Status), status);
            else
                _context.Friends.Add(new Friend() { senderUserID = senderId, receiverUserID = receiverId, Status = (Status)Enum.Parse(typeof(Status), status) });

            _context.SaveChanges();
        }
    }
}
