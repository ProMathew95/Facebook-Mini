﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Facebook_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Facebook_project.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private RoleManager<AppRole> _roleManager;

        //private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger, RoleManager<AppRole> roleManager)//,
            //IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            //_emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
          
            [EmailAddress]
            [Display(Name = "Email")]
            [BindProperty]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "BirthDate")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }

            [Display(Name = "Gender")]
            public Gender? Gender { get; set; }
        }



        public class InputModelWithRole:InputModel
        {

            public AppRole Role { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = Input.Email, Email = Input.Email , PhotoURL="default.jpg",FullName = $"{Input.FirstName} {Input.LastName}",
                    BirthDate = Input.BirthDate, Gender = Input.Gender};

                 

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);



                        //assigne role member for all new users 
                        bool x = await _roleManager.RoleExistsAsync("Member");
                        if (!x)
                        {
                            // first we create Member rool    
                            var role = new AppRole();
                            role.Name = "Member";
                            await _roleManager.CreateAsync(role);

                            
                       
                        }
                        await _userManager.AddToRoleAsync(user, "Member");

                      





                            return RedirectToAction("Index", "Home", Input);

                       // return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
       
        //public async Task<IActionResult> IsEmailInUse(string email)
        //{
        //    var result = await _userManager.FindByEmailAsync(email);
        //    if(result != null)
        //    {
        //        return new JsonResult($"Email {email} is already in use");
        //    }
        //        return new JsonResult(true);
           
        //}
    }
}
