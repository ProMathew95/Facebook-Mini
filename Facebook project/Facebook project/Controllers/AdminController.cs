using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook_project.Models;
using Facebook_project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Facebook_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public AdminController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        //[Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            ViewBag.allroles = roleManager.Roles;
            ViewBag.roles = new SelectList(roleManager.Roles, "id");
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            ViewBag.roles = new SelectList(roleManager.Roles, "id");
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel user)
        {

            var x = user;


            return Redirect("Admin/ListUsers");
            return View();

        }







        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id= {id} cannot be found";
                //return View("NotFound");
            }
            var UserRoles = await userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
              
                id = user.Id,
                UserName = user.UserName,
                 Email = user.Email,
                Roles=UserRoles
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.id);
            
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id= {model.id} cannot be found";
                //return View("NotFound");
            }
            else
            {
               
               
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("listUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> BlockUser(string id)
        {
            var users = userManager.Users;
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id= {id} cannot be found";
                //return View("NotFound");
            }
            else
            {
                user.isBlocked = true;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("listUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(users);
        }

        public async Task<IActionResult> UnBlockUser(string id)
        {
            var users = userManager.Users;
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id= {id} cannot be found";
                //return View("NotFound");
            }
            else
            {
                user.isBlocked = false;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("listUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return RedirectToAction("ListUsers");
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole appRole = new AppRole { Name = model.RoleName };
                IdentityResult result = await roleManager.CreateAsync(appRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }
                foreach (var error in result.Errors)
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
        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await roleManager.FindByIdAsync(id);
            if (result == null)
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
                if (await userManager.IsInRoleAsync(user, result.Name))
                {
                    model.USers.Add(user.UserName);
                }
            }
            return View(model);
        }
        //[Authorize(Roles = "Admin")]
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
                var result2 = await roleManager.UpdateAsync(result);
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
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
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
                if (await userManager.IsInRoleAsync(user, role.Name))
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
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string id)
        {
            ViewBag.userId = id;
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} Cannot Be found";
                //return View("NotFound");
            }
            var model = new List<UserRolesViewModel>();
            foreach (var role in roleManager.Roles)
            {
                var instance = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    instance.IsSelected = true;
                }
                else
                {
                    instance.IsSelected = false;
                }
                model.Add(instance);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> models, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} Cannot Be found";
                //return View("NotFound");
            }
            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove User existing Roles");
                return View(models);
            }
            result = await userManager.AddToRolesAsync(user, models.Where(aa => aa.IsSelected == true).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected Roles to user");
                return View(models);
            }
            return RedirectToAction("EditUser", new { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleId)
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
        [HttpPost]
        [Route("/Admin/getuser/{Name}")]
        public IActionResult getUser(string Name)
        {
            var result = userManager.Users.Where(X => X.UserName == Name).FirstOrDefault();
            if (result == null)
            {
                return Json("eMPTY");
            }
            else
            {
                var UserRoles = userManager.GetRolesAsync(result);
                var model = new EditUserViewModel
                {
                    id = result.Id,
                   
                    UserName = result.UserName,
                    Email = result.Email,

                };
                return Json(model);
            }
        }
    }
    }