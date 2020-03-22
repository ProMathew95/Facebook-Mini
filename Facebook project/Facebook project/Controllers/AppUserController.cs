using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook_project.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Facebook_project.Controllers
{
    public class AppUserController : Controller
    {
        AppUserRepository _context;
        public AppUserController(AppUserRepository db)
        {
            _context = db;
        }
        public IActionResult UserPage(string id)
        {
           var user= _context.GetUserByid(id);
            return View(user);
        }
    }
}