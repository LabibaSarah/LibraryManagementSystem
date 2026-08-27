using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using LibraryManagementSystem.Data;

namespace LibraryManagementSystem.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;


    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }


    [BindProperty]
    public InputModel Input { get; set; } = default!;


    public string? ReturnUrl { get; set; }


    public IList<AuthenticationScheme>? ExternalLogins { get; set; }


    public class InputModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;


        // University ID is NOT globally required here.
        // It will be required only for Student in OnPostAsync.

        [StringLength(30)]
        [Display(Name = "University ID")]
        public string? UniversityId { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;


        [Required]
        [StringLength(
            100,
            ErrorMessage =
                "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;


        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(
            "Password",
            ErrorMessage =
                "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }


        [Required]
        [Display(Name = "Account Type")]
        public string Role { get; set; } = string.Empty;
    }


    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();
    }


    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");


        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();


        // ==========================================
        // VALIDATE ACCOUNT TYPE
        // ==========================================

        if (Input.Role != "Student" &&
            Input.Role != "Faculty")
        {
            ModelState.AddModelError(
                "Input.Role",
                "Please select a valid account type.");
        }


        // ==========================================
        // UNIVERSITY ID VALIDATION
        // ==========================================

        // Student MUST provide University ID

        if (Input.Role == "Student" &&
            string.IsNullOrWhiteSpace(Input.UniversityId))
        {
            ModelState.AddModelError(
                "Input.UniversityId",
                "University ID is required for students.");
        }


        // Faculty can leave University ID empty.
        // Convert empty value to null.

        if (Input.Role == "Faculty" &&
            string.IsNullOrWhiteSpace(Input.UniversityId))
        {
            Input.UniversityId = null;
        }


        // ==========================================
        // CREATE USER
        // ==========================================

        if (ModelState.IsValid)
        {
            var user = CreateUser();


            user.FullName =
                Input.FullName.Trim();


            // Student gets University ID.
            // Faculty can have null/empty University ID.

            user.UniversityId =
                string.IsNullOrWhiteSpace(Input.UniversityId)
                    ? string.Empty
                    : Input.UniversityId.Trim();


            // ==========================================
            // EMAIL
            // ==========================================

            await _userStore.SetUserNameAsync(
                user,
                Input.Email,
                CancellationToken.None);


            await _emailStore.SetEmailAsync(
                user,
                Input.Email,
                CancellationToken.None);


            // ==========================================
            // CREATE IDENTITY USER
            // ==========================================

            var result =
                await _userManager.CreateAsync(
                    user,
                    Input.Password);


            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "User created a new account with password.");


                // ==========================================
                // ASSIGN ROLE
                // ==========================================

                var roleResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        Input.Role);


                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }


                    await _userManager.DeleteAsync(user);

                    return Page();
                }


                // ==========================================
                // EMAIL CONFIRMATION
                // ==========================================

                var userId =
                    await _userManager.GetUserIdAsync(user);


                var code =
                    await _userManager
                        .GenerateEmailConfirmationTokenAsync(user);


                code =
                    WebEncoders.Base64UrlEncode(
                        Encoding.UTF8.GetBytes(code));


                var callbackUrl =
                    Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            area = "Identity",
                            userId = userId,
                            code = code,
                            returnUrl = returnUrl
                        },
                        protocol: Request.Scheme)!;


                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Confirm your email",
                    $"Please confirm your account by " +
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" +
                    $"clicking here</a>.");


                // ==========================================
                // SIGN IN / CONFIRMATION
                // ==========================================

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage(
                        "RegisterConfirmation",
                        new
                        {
                            email = Input.Email,
                            returnUrl = returnUrl
                        });
                }
                else
                {
                    await _signInManager.SignInAsync(
                        user,
                        isPersistent: false);


                    return LocalRedirect(returnUrl);
                }
            }


            // ==========================================
            // IDENTITY ERRORS
            // ==========================================

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }
        }


        return Page();
    }


    // ==========================================
    // CREATE USER
    // ==========================================

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException(
                $"Can't create an instance of " +
                $"'{nameof(ApplicationUser)}'. " +
                $"Ensure that " +
                $"'{nameof(ApplicationUser)}' is not an " +
                $"abstract class and has a parameterless " +
                $"constructor.");
        }
    }


    // ==========================================
    // EMAIL STORE
    // ==========================================

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException(
                "The default UI requires a user store " +
                "with email support.");
        }


        return
            (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
