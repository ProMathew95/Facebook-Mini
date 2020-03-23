using System;
using System.Collections.Generic;
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
    }
}