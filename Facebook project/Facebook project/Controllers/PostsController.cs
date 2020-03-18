using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Facebook_project.Data;
using Facebook_project.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Facebook_project.Repositories;
using Facebook_project.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Facebook_project.Controllers
{
    public class PostsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        PostRepository _context;
        public PostsController(PostRepository context)
        {
            _context = context;
        }

        // GET: Posts
        public IActionResult Index()
        {
            var repo = _context.GetPosts();
            return View(repo.ToList());
        }

        // GET: Posts/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .Include(p => p.Publisher)
        //        .FirstOrDefaultAsync(m => m.PostId == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        // GET: Posts/Create
        public IActionResult Create()
        {
           // ViewData["PublisherId"] = new SelectList(_context.AppUsers, "Id", "Id");
            return PartialView();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(/*[Bind("PostId,isDeleted,Date,numberOfLikes,Text,PictureURL,PublisherId")]*/ PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.CreatePost(model.Post);
                return RedirectToAction(nameof(Index),"Home");
            }
            //ViewData["PublisherId"] = new SelectList(_context.AppUsers, "Id", "Id", post.PublisherId);
            return View(model.Post);
        }

        // GET: Posts/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = _context.GetPostByID(id);
            if (post == null)
            {
                return NotFound();
            }
            //ViewData["PublisherId"] = new SelectList(_context.AppUsers, "Id", "Id", post.PublisherId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("PostId,isDeleted,Date,numberOfLikes,Text,PictureURL,PublisherId")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdatePost(id,post);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
           // ViewData["PublisherId"] = new SelectList(_context.AppUsers, "Id", "Id", post.PublisherId);
            return View(post);
        }

        // GET: Posts/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .Include(p => p.Publisher)
        //        .FirstOrDefaultAsync(m => m.PostId == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _context.DeletePosts(id);
            return RedirectToAction(nameof(Index));
        }
        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Like(int id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                _context.AddLike(userId, id);
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult Dislike(int id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                _context.Dislike(userId, id);
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult LikesModal(int id)
        {
            List<AppUser> Users = _context.GetPostLikers(id);
            return PartialView("_Likes",Users);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AddComment([FromBody]CommentViewModel commentVM)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                Comment comment = new Comment()
                {
                    PostID = commentVM.PostId,
                    Text = commentVM.Comment,
                    Time = DateTime.Now,
                    UserID = userId,
                    isRemoved = false
                };

                _context.AddComment(comment);
                return Json(comment);
            }

            return Json("error");
        }
    }
}
