using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Facebook_project.Models;
using Facebook_project.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Facebook_project.Controllers
{
    public class UsersController : Controller
    {

        AppUserRepository _context;
        public UsersController(AppUserRepository db)
        {
            _context = db;
        }
        public IActionResult UserPage(string id)
        {
            var user = _context.GetUserByid(id);
            return View(user);
        }
        public IActionResult EditUserDataModal()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                var user = _context.GetUserByid(userId);
                return PartialView("EditUserDataModal", user);
            }
            return Json("error");
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditUserData(IFormFile file)
        {
            if (HttpContext.Request.Form.Keys.Any())
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    Microsoft.Extensions.Primitives.StringValues fullname = "";
                    Microsoft.Extensions.Primitives.StringValues bio = "";
                    Microsoft.Extensions.Primitives.StringValues birthdate = "";
                    Microsoft.Extensions.Primitives.StringValues gender = "";

                    HttpContext.Request.Form.TryGetValue("fullname", out fullname);
                    HttpContext.Request.Form.TryGetValue("bio", out bio);
                    HttpContext.Request.Form.TryGetValue("birthdate", out birthdate);
                    HttpContext.Request.Form.TryGetValue("gender", out gender);

                    var userId = claim.Value;
                    var status = _context.isBlocked(userId);
                    if(!status)
                    {
                        AppUser user = new AppUser
                        {
                            Id = userId,
                            FullName = fullname.ToString(),
                            Bio = bio.ToString(),
                            BirthDate = DateTime.Parse(birthdate.ToString()),
                            Gender = (Gender)Enum.Parse(typeof(Gender), gender.ToString())
                        };
                        _context.UpdateUserInfo(user);
                        return Json(user);
                    }
                  }
                }

            return Json("error");
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



                    return PartialView("~/Views/Posts/_Post.cshtml", respPost);

                }
            }

            return Json("error");
        }
    }
}