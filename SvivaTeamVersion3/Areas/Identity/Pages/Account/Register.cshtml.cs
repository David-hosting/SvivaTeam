using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SvivaTeamVersion3.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using System;
using SvivaTeamVersion3.Data;

namespace SvivaTeamVersion3.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
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
            [Display(Name = "Terms of Service")]
            [IsTrue(true, ErrorMessage = "You must agree to the Terms of Service")]
            public bool AcceptedTOS { get; set; }

            //Experimental
            [Required]
            [Display(Name = "DOB")]
            [DataType(DataType.Date, ErrorMessage = "Age must be above 13.")]
            [MinimumAge(13)]
            public DateTime DOB { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    AcceptedTOS = Input.AcceptedTOS,
                    DOB = Input.DOB
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    //if(_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    //{
                    //    await Response.WriteAsync($"<script>alert('Confirm email: {user.Email}');</script>"); //works great

                    //    return Page();
                    //}

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    
                    string Code = "<!doctype html><html><head><meta name='viewport' content='width = device - width, initial - scale = 1.0'><meta http-equiv='Content - Type' content='text / html; charset = UTF - 8'><title>Simple Transactional Email</title><style>@media only screen and (max-width: 620px) {table.body h1 {font-size: 28px !important;margin-bottom: 10px !important;}table.body p,table.body ul,table.body ol,table.body td,table.body span,table.body a {font-size: 16px !important;}table.body .wrapper,table.body .article {padding: 10px !important;}table.body .content {padding: 0 !important;}table.body .container {padding: 0 !important;width: 100% !important;}table.body .main {border-left-width: 0 !important;border-radius: 0 !important;border-right-width: 0 !important;}table.body .btn table {width: 100% !important;}table.body .btn a {width: 100% !important;}table.body .img-responsive {height: auto !important;max-width: 100% !important;width: auto !important;}}@media all {.ExternalClass {width: 100%;}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div {line-height: 100%;}.apple-link a {color: inherit !important;font-family: inherit !important;font-size: inherit !important;font-weight: inherit !important;line-height: inherit !important;text-decoration: none !important;}#MessageViewBody a {color: inherit;text-decoration: none;font-size: inherit;font-family: inherit;font-weight: inherit;line-height: inherit;}.btn-primary table td:hover {background-color: #34495e !important;}.btn-primary a:hover {background-color: #34495e !important;border-color: #34495e !important;}}</style>" + $"</head><body style='background - color: #f6f6f6; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;'><span class='preheader' style='color: transparent; display: none; height: 0; max-height: 0; max-width: 0; opacity: 0; overflow: hidden; mso-hide: all; visibility: hidden; width: 0;'>This is preheader text. Some clients will show this text as a preview.</span><table role='presentation' border='0' cellpadding='0' cellspacing='0' class='body' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f6f6f6; width: 100%;' width='100%' bgcolor='#f6f6f6'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'>&nbsp;</td><td class='container' style='font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; max-width: 580px; padding: 10px; width: 580px; margin: 0 auto;' width='580' valign='top'><div class='content' style='box-sizing: border-box; display: block; margin: 0 auto; max-width: 580px; padding: 10px;'><!-- START CENTERED WHITE CONTAINER --><table role='presentation' class='main' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background: #ffffff; border-radius: 3px; width: 100%;' width='100%'><!-- START MAIN CONTENT AREA --><tr><td class='wrapper' style='font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box; padding: 20px;' valign='top'><table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' width='100%'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'><h1>Welcome to SvivaTeam!</h1><p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Hello user we are glad that you decided to join our great community!</p><p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Last thing before you can start reporting environmental issues we need you to confirm your email address by pressing the button below.</p><p>If you didn't create an account with <a href='https://www.firstbenefits.org/wp-content/uploads/2017/10/placeholder.png' style='color: #1a82e2;'>SvivaTeam</a>, please ignore this email.</p><table role='presentation' border='0' cellpadding='0' cellspacing='0' class='btn btn-primary' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; box-sizing: border-box; width: 100%;' width='100%'><tbody><tr><td align='left' style='font-family: sans-serif; font-size: 14px; vertical-align: top; padding-bottom: 15px;' valign='top'><table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tbody><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; border-radius: 5px; text-align: center; background-color: #3498db;' valign='top' align='center' bgcolor='#3498db'> <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' target='_blank' style='border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; display: inline-block; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-decoration: none; text-transform: capitalize; background-color: #3498db; border-color: #3498db; color: #ffffff;'>Call To Action</a></td></tr></tbody></table></td></tr></tbody></table><p>If the button doesn't work, please contact us at the email <a href='https://mail.google.com/mail/u/0/?tab=rm&ogbl#inbox?compose=CllgCJTLphnrdZcfjlfZrTGHLQHNDXPNPVjLLsrQVSVZcqQlwcDCfFKMGsjpKdvQVHcmCzJkvkL' style='color: #1a82e2;'>svivateam1@gmail.com</a></p><p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Thanks for joining us,<br> SvivaTeam</p></td></tr></table></td></tr><!-- END MAIN CONTENT AREA --></table><!-- END CENTERED WHITE CONTAINER --><!-- START FOOTER --><div class='footer' style='clear: both; margin-top: 10px; text-align: center; width: 100%;'><table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' width='100%'><tr><td class='content-block' style='font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px; text-align: center;' valign='top' align='center'><span class='apple-link' style='color: #999999; font-size: 12px; text-align: center;'>SvivaTeam - Digital Service Provider</span><!--<br> Don't like these emails? <a href='http://i.imgur.com/CScmqnj.gif' style='text-decoration: underline; color: #999999; font-size: 12px; text-align: center;'>Unsubscribe</a>.--></td></tr><tr><td class='content-block powered-by' style='font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px; text-align: center;' valign='top' align='center'>Powered by <a href='https://www.sendgrid.com/' style='color: #999999; font-size: 12px; text-align: center; text-decoration: none;'>SendGrid</a>.</td></tr></table></div><!-- END FOOTER --></div></td><td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'>&nbsp;</td></tr></table></body></html>";

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", Code); //Change the message 
                    //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
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
        public bool IsNotNull(string confirmation)
        {
            return true ? confirmation != null : false;
        }
    }
}