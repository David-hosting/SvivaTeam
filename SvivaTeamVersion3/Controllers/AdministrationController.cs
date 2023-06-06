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

                string Code = "<!doctype html><html><head><meta name='viewport' content='width = device - width, initial - scale = 1.0'><meta http-equiv='Content - Type' content='text / html; charset = UTF - 8'><title>Simple Transactional Email</title><style>@media only screen and (max-width: 620px) {table.body h1 {font-size: 28px !important;margin-bottom: 10px !important;}table.body p,table.body ul,table.body ol,table.body td,table.body span,table.body a {font-size: 16px !important;}table.body .wrapper,table.body .article {padding: 10px !important;}table.body .content {padding: 0 !important;}table.body .container {padding: 0 !important;width: 100% !important;}table.body .main {border-left-width: 0 !important;border-radius: 0 !important;border-right-width: 0 !important;}table.body .btn table {width: 100% !important;}table.body .btn a {width: 100% !important;}table.body .img-responsive {height: auto !important;max-width: 100% !important;width: auto !important;}}@media all {.ExternalClass {width: 100%;}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div {line-height: 100%;}.apple-link a {color: inherit !important;font-family: inherit !important;font-size: inherit !important;font-weight: inherit !important;line-height: inherit !important;text-decoration: none !important;}#MessageViewBody a {color: inherit;text-decoration: none;font-size: inherit;font-family: inherit;font-weight: inherit;line-height: inherit;}.btn-primary table td:hover {background-color: #34495e !important;}.btn-primary a:hover {background-color: #34495e !important;border-color: #34495e !important;}}</style>" + $"</head><body style='background - color: #f6f6f6; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;'><span class='preheader' style='color: transparent; display: none; height: 0; max-height: 0; max-width: 0; opacity: 0; overflow: hidden; mso-hide: all; visibility: hidden; width: 0;'>This is preheader text. Some clients will show this text as a preview.</span><table role='presentation' border='0' cellpadding='0' cellspacing='0' class='body' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f6f6f6; width: 100%;' width='100%' bgcolor='#f6f6f6'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'>&nbsp;</td><td class='container' style='font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; max-width: 580px; padding: 10px; width: 580px; margin: 0 auto;' width='580' valign='top'><div class='content' style='box-sizing: border-box; display: block; margin: 0 auto; max-width: 580px; padding: 10px;'><!-- START CENTERED WHITE CONTAINER --><table role='presentation' class='main' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background: #ffffff; border-radius: 3px; width: 100%;' width='100%'><!-- START MAIN CONTENT AREA --><tr><td class='wrapper' style='font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box; padding: 20px;' valign='top'><table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' width='100%'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'><h1>Welcome to SvivaTeam!</h1><p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Hello user we are glad that you decided to join our great community!</p><p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Last thing before you can start reporting environmental issues we need you to confirm your email address by pressing the button below.</p><p>If you didn't create an account with <a href='https://www.firstbenefits.org/wp-content/uploads/2017/10/placeholder.png' style='color: #1a82e2;'>SvivaTeam</a>, please ignore this email.</p><table role='presentation' border='0' cellpadding='0' cellspacing='0' class='btn btn-primary' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; box-sizing: border-box; width: 100%;' width='100%'><tbody><tr><td align='left' style='font-family: sans-serif; font-size: 14px; vertical-align: top; padding-bottom: 15px;' valign='top'><table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tbody><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; border-radius: 5px; text-align: center; background-color: #3498db;' valign='top' align='center' bgcolor='#3498db'> <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' target='_blank' style='border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; display: inline-block; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-decoration: none; text-transform: capitalize; background-color: #3498db; border-color: #3498db; color: #ffffff;'>Call To Action</a></td></tr></tbody></table></td></tr></tbody></table><p>If the button doesn't work, please contact us at the email <a href='https://mail.google.com/mail/u/0/?tab=rm&ogbl#inbox?compose=CllgCJTLphnrdZcfjlfZrTGHLQHNDXPNPVjLLsrQVSVZcqQlwcDCfFKMGsjpKdvQVHcmCzJkvkL' style='color: #1a82e2;'>svivateam1@gmail.com</a></p><p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Thanks for joining us,<br> SvivaTeam</p></td></tr></table></td></tr><!-- END MAIN CONTENT AREA --></table><!-- END CENTERED WHITE CONTAINER --><!-- START FOOTER --><div class='footer' style='clear: both; margin-top: 10px; text-align: center; width: 100%;'><table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' width='100%'><tr><td class='content-block' style='font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px; text-align: center;' valign='top' align='center'><span class='apple-link' style='color: #999999; font-size: 12px; text-align: center;'>SvivaTeam - Digital Service Provider</span><!--<br> Don't like these emails? <a href='http://i.imgur.com/CScmqnj.gif' style='text-decoration: underline; color: #999999; font-size: 12px; text-align: center;'>Unsubscribe</a>.--></td></tr><tr><td class='content-block powered-by' style='font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px; text-align: center;' valign='top' align='center'>Powered by <a href='https://www.sendgrid.com/' style='color: #999999; font-size: 12px; text-align: center; text-decoration: none;'>SendGrid</a>.</td></tr></table></div><!-- END FOOTER --></div></td><td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'>&nbsp;</td></tr></table></body></html>";

                await _emailSender.SendEmailAsync(Input.NewEmail, "Confirm your email", Code);
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

