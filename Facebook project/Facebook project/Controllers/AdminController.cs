using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook_project.Models;
using Facebook_project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Facebook_project.Controllers
{
   
    public class AdminController : Controller
    {
        private readonly RoleManager<AppRole> roleManager;

        public AdminController(RoleManager<AppRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole appRole = new AppRole { Name=model.RoleName };
                IdentityResult result = await roleManager.CreateAsync(appRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}