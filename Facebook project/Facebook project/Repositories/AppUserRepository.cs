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
			var appUser = _context.AppUsers.Include(c => c.Posts).ThenInclude(c=>(c as Post).Comment).Include(c => c.Posts).ThenInclude(c=>(c as Post).Like ).FirstOrDefault(c => c.Id == userId);
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
		public void CreatePost(Post post)
		{
			post.isDeleted = false;
			post.numberOfLikes = 0;
			post.Date = DateTime.Now;
			_context.Posts.Add(post);
			_context.SaveChanges();
		}
		public Post GetPostByUserAndDate(string userId, DateTime date)
		{
			return _context.Posts.Include(p => p.Publisher).Include(p => p.Like).Include(p => p.Comment).Where(p => p.PublisherId == userId && p.Date == date).FirstOrDefault();
		}
	}
}
