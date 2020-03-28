using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Facebook_project.Models;
using Facebook_project.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Facebook_project.Controllers
{
    public class FriendsController : Controller
    {
        FriendsRepository _context;
        public FriendsController(FriendsRepository db)
        {
            _context = db;
        }
        public IActionResult FriendRequestsModal()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;

                var users = _context.GetFriendRequests(userId);
                return PartialView("_FriendRequests", users);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RejectRequest(string id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                _context.changeRequestStatus(id, userId, Status.RequestRejected);
            }
            return Json("success");
        }

        [HttpPost]
        public IActionResult AcceptRequest(string id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                _context.changeRequestStatus(id, userId, Status.RequestConfirmed);
            }
            return Json("success");
        }

        public IActionResult Search(string Id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                //var userId = claim.Value;
                var result = _context.Search(Id);
                return Json(result);
            }
            return Json("error");
        }
        public IActionResult FriendsModal()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;

                var users = _context.GetFriends(userId);
                return PartialView("_FriendsModal", users);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}