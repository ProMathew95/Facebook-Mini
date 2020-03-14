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
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;

                var CurrentUserPosts = _db.Posts.Include(p => p.Comment).Include(p => p.Like)
                    .Include(p => p.Publisher).Where(p => p.PublisherId == userId).ToList();

                if (CurrentUserPosts.Count > 0)
                {
                    //List<PostViewModel> postList = new List<PostViewModel>();

                    //foreach (var post in CurrentUserPosts)
                    //{
                    //    PostViewModel model = new PostViewModel()
                    //    {
                    //        post = post,
                    //        comments = _db.Comments.Include(c => c.User).Where(c => c.PostID == post.PostId),
                    //        likes = _db.Likes.Include(l => l.User).Where(l => l.PostID == post.PostId)
                    //    };

                    //    postList.Add(model);
                    //}
                    return View(CurrentUserPosts);
                }
            }

            return View();
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
    }
}
