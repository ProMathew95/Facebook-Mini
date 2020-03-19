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
using System.IO;

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
        //[HttpPost]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(/*[Bind("PostId,isDeleted,Date,numberOfLikes,Text,PictureURL,PublisherId")]*/ IFormFile file, PostViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (file != null)
        //        {
        //            string pic = Path.GetFileName(file.FileName);
        //            byte[] array;


        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                file.CopyTo(ms);
        //                array = ms.GetBuffer();
        //                var picName = $"{Guid.NewGuid()}.jpg";
        //                var str = Path.Combine(Environment.CurrentDirectory, "wwwroot//PostsPics//",$"{picName}");
        //                System.IO.File.WriteAllBytes(str, array);
        //                model.Post.PictureURL = picName;
        //            }
        //        }

        //        _context.CreatePost(model.Post);
        //        return RedirectToAction(nameof(Index),"Home");
        //    }

        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public IActionResult AddPost(IFormFile file)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                var picName = "";

                /////checking image
                if (HttpContext.Request.Form.Files.Any())
                {
                    var img = HttpContext.Request.Form.Files[0];
                    string pic = Path.GetFileName(img.FileName);
                    byte[] array;


                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.CopyTo(ms);
                        array = ms.GetBuffer();
                        picName = $"{Guid.NewGuid()}.jpg";
                        var str = Path.Combine(Environment.CurrentDirectory, "wwwroot//PostsPics", picName);
                        System.IO.File.WriteAllBytes(str, array);
                    }
                }
                /////////////////////

                if (HttpContext.Request.Form.Keys.Any())
                {
                    Microsoft.Extensions.Primitives.StringValues post = "";
                    Microsoft.Extensions.Primitives.StringValues PID = "";
                    HttpContext.Request.Form.TryGetValue("postText", out post);

                    string PostText = post.ToString();

                    Post newpPost = new Post()
                    {
                        Date = DateTime.Now,
                        isDeleted = false,
                        Text = PostText,
                        PublisherId = userId
                    };

                    if (picName != "")
                        newpPost.PictureURL = picName;


                    _context.CreatePost(newpPost);

                    var respPost = _context.GetPostByUserAndDate(newpPost.PublisherId, newpPost.Date);

                    ResponseViewModel response = new ResponseViewModel()
                    {
                        PostId = respPost.PostId,
                        UserId = respPost.PublisherId,
                        UserName = respPost.Publisher.FullName,
                        Time = respPost.Date,
                        Text = respPost.Text,
                        PicURL = picName,
                        UserPicURL = respPost.Publisher.PhotoURL
                    };
                    return Json(response);

                }
            }

            return Json("error");
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddComment([FromBody]CommentViewModel commentVM)
        //{
        //    var claimsIdentity = (ClaimsIdentity)this.User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    if (claim != null)
        //    {
        //        var userId = claim.Value;
        //        Comment comment = new Comment()
        //        {
        //            PostID = commentVM.PostId,
        //            Text = commentVM.Comment,
        //            Time = DateTime.Now,
        //            UserID = userId,
        //            isRemoved = false
        //        };
        //        _context.AddComment(comment);
        //        var respComment = _context.GetComment(comment.UserID, comment.PostID, comment.Time);

        //        CommentResponseViewModel response = new CommentResponseViewModel()
        //        {
        //            PostId = respComment.PostID,
        //            UserId = respComment.UserID,
        //            UserName = respComment.User.FullName,
        //            Time = respComment.Time,
        //            Text = respComment.Text,
        //            UserPicURL = respComment.User.PhotoURL
        //        };
        //        return Json(response);
        //    }

        //    return Json("error");
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AddComment(IFormFile file)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                var picName = "";

                /////checking image
                if (HttpContext.Request.Form.Files.Any())
                {
                    var img = HttpContext.Request.Form.Files[0];
                    string pic = Path.GetFileName(img.FileName);
                    byte[] array;


                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.CopyTo(ms);
                        array = ms.GetBuffer();
                        picName = $"{Guid.NewGuid()}.jpg";
                        var str = Path.Combine(Environment.CurrentDirectory, "wwwroot//CommentsPics", picName);
                        System.IO.File.WriteAllBytes(str, array);
                    }
                }
                /////////////////////

                if (HttpContext.Request.Form.Keys.Any())
                {
                    Microsoft.Extensions.Primitives.StringValues cmnt = "";
                    Microsoft.Extensions.Primitives.StringValues PID = "";
                    HttpContext.Request.Form.TryGetValue("comment", out cmnt);
                    HttpContext.Request.Form.TryGetValue("PostId", out PID);

                    string PostId = PID.ToString();
                    string Comment = cmnt.ToString();

                    Comment comment = new Comment()
                    {
                        PostID = int.Parse(PostId),
                        Text = Comment,
                        Time = DateTime.Now,
                        UserID = userId,
                        isRemoved = false
                        
                    };
                    if (picName != "")
                        comment.PictureURL = picName;


                    _context.AddComment(comment);

                    var respComment = _context.GetComment(comment.UserID, comment.PostID, comment.Time);

                    ResponseViewModel response = new ResponseViewModel()
                    {
                        PostId = respComment.PostID,
                        UserId = respComment.UserID,
                        UserName = respComment.User.FullName,
                        Time = respComment.Time,
                        Text = respComment.Text,
                        PicURL = picName,
                        UserPicURL = respComment.User.PhotoURL
                    };
                    return Json(response);
                }
            }

            return Json("error");
        }
    }
}
