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

        
        // GET: Posts/Create
        public IActionResult Create()
        {
           // ViewData["PublisherId"] = new SelectList(_context.AppUsers, "Id", "Id");
            return PartialView();
        }

        
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

                   

                    return PartialView("~/Views/Posts/_Post.cshtml",respPost);

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
                    //_context.UpdatePost(id,post,_);
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


        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _context.DeletePost(id);
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
        public IActionResult UserInfoModal(string id)
        {
            AppUser UserInfo = _context.GetUserByid(id);
            return View("_UserInfo", UserInfo);
        }
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

                    var respComment = _context.GetComment(comment.UserID, comment.PostID, comment.Time.ToString());
                    return PartialView("~/Views/Posts/_Comment.cshtml", respComment);
                }
            }

            return Json("error");
        }

        public IActionResult DeletePost(int Id)
        {
            if (_context.DeletePost(Id))
                return Json("success");
            return Json("error"); 
        }

        //[HttpPost]
        public IActionResult DeleteComment(string objId)
        {
            var arr = objId.Split("*");
            var postId = int.Parse(arr[1]);
            var publisherId = arr[2];
            var date = arr[3];

            if (_context.DeleteComment(postId, publisherId, date))
                return Json("success");
            return Json("error");
        }

        public IActionResult ConfirmDeleteModal(string objId)
        {
            return PartialView("_ComfirmDelete", objId);
        }
        public IActionResult EditPostModal(int Id)
        {
            var post = _context.GetPostByID(Id);

            if(post != null)
                return PartialView("_EditPost", post);
            return Json("error");
        }
        public IActionResult EditCommentModal(string objId)
        {
            var arr = objId.Split("*");
            var postId = int.Parse(arr[1]);
            var publisherId = arr[2];
            var date = arr[3];

            var comment = _context.GetComment(publisherId, postId, date);

            if (comment != null)
                return PartialView("_EditComment", comment);
            return Json("error");
        }
        
        [HttpPost]
        [Authorize]
        public IActionResult EditPost(IFormFile file)
        {
            

            if (HttpContext.Request.Form.Keys.Any())
            {
                if (HttpContext.Request.Form.Keys.Contains("postId"))
                {

                    Microsoft.Extensions.Primitives.StringValues postText = "";
                    Microsoft.Extensions.Primitives.StringValues PID = "";
                    Microsoft.Extensions.Primitives.StringValues removeImg = "";
                    HttpContext.Request.Form.TryGetValue("postText", out postText);
                    HttpContext.Request.Form.TryGetValue("postId", out PID);
                    HttpContext.Request.Form.TryGetValue("removeImg", out removeImg);




                    string PostText = postText.ToString();
                    int PostID =int.Parse(PID);
                    string picName = "";
                    bool removeImage = bool.Parse(removeImg);

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

                    var respPost = _context.UpdatePost(PostID, PostText, picName, removeImage);


                    ResponseViewModel response = new ResponseViewModel()
                    {
                        PostId = respPost.PostId,
                        UserId = respPost.PublisherId,
                        //UserName = respPost.Publisher.FullName,
                        Time = respPost.Date.ToString(),
                        Text = respPost.Text,
                        PicURL = respPost.PictureURL,
                        //UserPicURL = respPost.Publisher.PhotoURL
                    };

                    return Json(response);

                }
            }

            return Json("error");
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditComment(IFormFile file)
        {
            if (HttpContext.Request.Form.Keys.Any())
            {
                if (HttpContext.Request.Form.Keys.Contains("postId"))
                {

                    Microsoft.Extensions.Primitives.StringValues commentText = "";
                    Microsoft.Extensions.Primitives.StringValues commentTime = "";
                    Microsoft.Extensions.Primitives.StringValues userId = "";
                    Microsoft.Extensions.Primitives.StringValues PID = "";
                    Microsoft.Extensions.Primitives.StringValues removeImg = "";
                    HttpContext.Request.Form.TryGetValue("commentText", out commentText);
                    HttpContext.Request.Form.TryGetValue("commentTime", out commentTime);
                    HttpContext.Request.Form.TryGetValue("userId", out userId);
                    HttpContext.Request.Form.TryGetValue("postId", out PID);
                    HttpContext.Request.Form.TryGetValue("removeImg", out removeImg);




                    string CommentText = commentText.ToString();
                    string CommentTime = commentTime.ToString();
                    string UserId = userId.ToString();
                    int PostID = int.Parse(PID);
                    bool removeImage = bool.Parse(removeImg);
                    string picName = "";

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

                    var respComment = _context.UpdateComment(PostID, userId, CommentTime, commentText, picName, removeImage);


                    ResponseViewModel response = new ResponseViewModel()
                    {
                        PostId = respComment.PostID,
                        UserId = respComment.UserID,
                        //UserName = respPost.Publisher.FullName,
                        Time = respComment.Time.ToString().Replace(" ",""),
                        Text = respComment.Text,
                        PicURL = respComment.PictureURL,
                        //UserPicURL = respPost.Publisher.PhotoURL
                    };

                    return Json(response);

                }
            }

            return Json("error");
        }
    }
}
