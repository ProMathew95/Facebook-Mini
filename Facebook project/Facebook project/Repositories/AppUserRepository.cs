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
	}
}
