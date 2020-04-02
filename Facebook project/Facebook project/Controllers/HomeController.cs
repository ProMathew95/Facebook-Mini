using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Facebook_project.Models;
using Facebook_project.Data;
using Facebook_project.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Facebook_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
       
        public IActionResult Index()
        {
            AppUser currentUser = null;
            var x = (ClaimsIdentity)this.User.Identity;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                currentUser = _db.AppUsers.Find(userId);

                if(currentUser.isBlocked != null && currentUser.isBlocked == true)
                {
                    //LOGOUT HERE
                }

                List<string> friendsIds = _db.Friends.Where(f => f.receiverUserID == userId && f.Status == Status.RequestConfirmed).Select(f => f.senderUserID).ToList();
                friendsIds.AddRange(_db.Friends.Where(f => f.senderUserID == userId && f.Status == Status.RequestConfirmed).Select(f => f.receiverUserID).ToList());

                var CurrentPosts = _db.Posts.Include(p => p.Comment).ThenInclude(c => (c as Comment).User)
                    .Include(p => p.Like).ThenInclude(l => (l as Like).User)
                    .Include(p => p.Publisher)
                    .Where(p => p.PublisherId == userId || 
                    (friendsIds.Contains(p.PublisherId) 
                    && (p.Publisher.isBlocked == null || p.Publisher.isBlocked == false))).ToList();

                foreach (var post in CurrentPosts)
                {
                    var commentsToRemove = post.Comment.Where(c => (c.User.isBlocked != null && c.User.isBlocked == true)).ToList();
                    if(commentsToRemove != null && commentsToRemove.Count > 0)
                    {
                        foreach(var comment in commentsToRemove)
                        {
                            post.Comment.Remove(comment);
                        }
                    }

                    var likesToRemove = post.Like.Where(c => (c.User.isBlocked != null && c.User.isBlocked == true)).ToList();
                    if (likesToRemove != null && likesToRemove.Count > 0)
                    {
                        foreach (var like in likesToRemove)
                        {
                            post.Like.Remove(like);
                        }
                    }
                }

                List<int> likedPosts = _db.Likes.Where(l => l.UserID == userId && l.isLiked).Select(l => l.PostID).ToList();

              //  if (CurrentPosts != null)
                if (CurrentPosts.Count> 0 )

                 {
                    UserPosts uspo1 = new UserPosts()
                    {
                        myUser = currentUser,
                        allpost = new List<Post>()

                    };
                    //return View(currentUser);
                    return View(CurrentPosts);

                }
                //else
                //{
                  return View(new List<Post>());
                //}
            }

            UserPosts uspo = new UserPosts()
            {
                myUser = currentUser,
                allpost = new List<Post>()

            };

           // return View(new List<Post>());
            return View(uspo);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public IActionResult HomePartial()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                var user = _db.AppUsers.FirstOrDefault(u => u.Id == userId);

                if(user != null)
                {
                    return PartialView("~/Views/Shared/_HomePartial.cshtml", user);
                }
                return NotFound();
            }
            return NotFound();
        }
    }
}
