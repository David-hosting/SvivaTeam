using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SvivaTeamVersion3.Areas.Identity.Data;
using SvivaTeamVersion3.Areas.Identity.Pages.Account.Manage;
using SvivaTeamVersion3.Models;
using SvivaTeamVersion3.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace SvivaTeamVersion3.Controllers
{
    public class AdministrationController : Controller
    {

        private readonly RoleManager<IdentityRole> roleManager;

        private UserManager<ApplicationUser> userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IEmailSender _emailSender;
        
        private readonly ILogger logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        IEmailSender emailSender,
                                        ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _emailSender = emailSender;
            this.logger = logger;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Required()]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return RedirectToAction("ListUsers", "Administration");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(EditUserModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var email = await userManager.GetEmailAsync(user);
            if (Input.NewEmail != email && Input.NewEmail != null)
            {
                var userId = await userManager.GetUserIdAsync(user);
                var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToAction("ListUsers", "Administration");
            }

            StatusMessage = "Your email is unchanged.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserModel model) 
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound($"User with Id: {model.Id} cannot be found.");

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)//If this returns true pay attemtion to the comments below. 
            {
                if (user.Email != Input.NewEmail)
                    await ChangeEmail(model); //If acctivated the function has its own return statement so this'll be the last line of the code.
                return RedirectToAction("ListUsers"); //If the line above does not get excuted this'll be the last line of the code.
            } /* 
                | Note: FIX --> When Email is changed username will stay the same. 
               */

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesModel>();

            foreach(var role in roleManager.Roles)
            {
                var userRoleModel = new UserRolesModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name)) userRoleModel.IsSelected = true;
                else userRoleModel.IsSelected = false;

                model.Add(userRoleModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user's existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                                                       model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new {Id = userId});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"User with Id: {id} cannot be found.");
            }
            else
            {
                var result = await userManager.DeleteAsync(await user);

                if(result.Succeeded)
                    return RedirectToAction("ListUsers");

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View("ListUser");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = roleManager.FindByIdAsync(id);
            
            if (role == null)
            {
                return NotFound($"Role with Id: {id} cannot be found.");
            }
            else
            {
                try
                {

                    var result = await roleManager.DeleteAsync(await role);

                    if (result.Succeeded)
                        return RedirectToAction("ListRoles");

                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View("ListRoles");
                }
                catch (DbUpdateException e)
                {
                    logger.LogError($"Error while deleting role {e}");

                    var roleName = await roleManager.GetRoleNameAsync(await role);

                    ViewBag.ErrorTitle = $"{roleName} role is in use";
                    ViewBag.ErrorMessage = $"{roleName} role cannot be deleted as there are users " +
                        $"in this role. If you want to delete this role, please remove the users from " +
                        $"the role and then try to delete.";
                    return View("Error");
                }
            }

        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID: {id} can not be found";
                return View("NotFound");
            }

            var model = new EditRoleModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRoleModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID: {model.Id} can not be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {role.Id} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleModel = new UserRoleModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                    userRoleModel.IsSelected = true;
                else
                    userRoleModel.IsSelected = false;

                model.Add(userRoleModel);

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {role.Id} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !await userManager.IsInRoleAsync(user, role.Name))
                    result = await userManager.AddToRoleAsync(user, role.Name);
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                else
                    continue;

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRoles", new { Id = roleId });
                }
            }


            return RedirectToAction("EditRoles", new { Id = roleId });
        }
    }
}

