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
        public IActionResult Create(/*[Bind("PostId,isDeleted,Date,numberOfLikes,Text,PictureURL,PublisherId")]*/ Post post)
        {
            if (ModelState.IsValid)
            {
                _context.CreatePost(post);
                return RedirectToAction(nameof(Index));
            }
            //ViewData["PublisherId"] = new SelectList(_context.AppUsers, "Id", "Id", post.PublisherId);
            return View(post);
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
    }
}
