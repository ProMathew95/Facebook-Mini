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
        private readonly UserManager<AppUser> userManager;

        public AdminController(RoleManager<AppRole> roleManager,UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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
                    return RedirectToAction("ListRoles", "Admin");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var result=await roleManager.FindByIdAsync(id);
            if(result == null)
            {
                ViewBag.ErrorMessage = $"role with Id={id} cannot be found";
               // return View("NotFound");
            }
            EditRoleViewModel model = new EditRoleViewModel
            {
                Id = result.Id,
                RoleName = result.Name,
                Description = result.Description

            };
            foreach (var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, result.Name))
                {
                    model.USers.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var result = await roleManager.FindByIdAsync(model.Id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"role with Id={model.Id} cannot be found";
                // return View("NotFound");
            }
            else
            {
                result.Name = model.RoleName;
                result.Description = model.Description;
                var result2=await roleManager.UpdateAsync(result);
                if (result2.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
            foreach (var error in result2.Errors)
            {
                    ModelState.AddModelError("", error.Description);
            }
            }
            return View(model);
        }
        [HttpGet]
       public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role =await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} Cannot Be found";
                //return View("notFound");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users)
            {
                var userRoleviewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleviewModel.IsSelected = true;
                }
                else
                {
                    userRoleviewModel.IsSelected = false;
                }
                model.Add(userRoleviewModel);
            }
            return View(model);
        } 
        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model,string roleId)
        {

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} Cannot Be found";
                // return View(NotFound);
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);

                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }
            return RedirectToAction("EditRole", new { Id = roleId });

        }
    }
}