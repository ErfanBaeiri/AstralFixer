using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Account;
using BugFixer.Web.ActionFilters;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.Win32;
using System.Security.Claims;

namespace BugFixer.Web.Controllers
{
    public class AccountController : BaseController
    {
        #region DI To IUserService,GoogleRecaptcha
        private readonly IUserService _userService;
        private readonly ICaptchaValidator _captchaValidator;
        public AccountController(IUserService userService, ICaptchaValidator captchaValidator)
        {
            _userService = userService;
            _captchaValidator = captchaValidator;
        }
        #endregion

        #region Login
        [HttpGet("Login")]
        [RedirectToHomeIfUserLoggedActionFilter]
        public IActionResult Login(string? ReturnUrl)
        {
            var result = new LoginViewModel();
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                result.ReturnUrl = ReturnUrl;
            }
            return View(result);
        }

        [HttpPost("Login"), ValidateAntiForgeryToken]
        [RedirectToHomeIfUserLoggedActionFilter]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(login.Captcha))
            {
                TempData[ErrorMessage] = "اعتبار سنجی Captcha با خطا مواجه شد لطفا مجدد تلاش نمایید";
                return View(login);
            }
            if (!ModelState.IsValid) return View(login);

            var result = await _userService.LoginUserAsync(login);

            switch (result)
            {
                case LoginResult.UserIsBan:
                    TempData[ErrorMessage] = "حساب کاربری مسدود میباشد";
                    return RedirectToAction("Login", "Account");
                case LoginResult.UserNotFound:
                    TempData[ErrorMessage] = "کاربر مورد نظر یافت نشد.";
                    return RedirectToAction("Login", "Account");
                case LoginResult.UserNotActive:
                    TempData[ErrorMessage] = "ایمیل حساب کاربری تایید نشده است";
                    return RedirectToAction("Login", "Account");

                case LoginResult.Success:

                    // Set Cookie and Login User
                    var user = await _userService.GetUserByEmail(login.Email);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties { IsPersistent = login.RememberMe };
                    await HttpContext.SignInAsync(principal, properties);

                    if (!string.IsNullOrEmpty(login.ReturnUrl))
                    {
                        return Redirect(login.ReturnUrl);
                    }
                    return Redirect("/");
            }

            return View();
        }
        #endregion

        #region Register
        [HttpGet("Register")]
        [RedirectToHomeIfUserLoggedActionFilter]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register"), ValidateAntiForgeryToken]
        [RedirectToHomeIfUserLoggedActionFilter]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            //if (!await _captchaValidator.IsCaptchaPassedAsync(register.Captcha))
            //{
            //    TempData[ErrorMessage] = "اعتبار سنجی Captcha با خطا مواجه شد لطفا مجدد تلاش نمایید";
            //    return View(register);
            //}

            if (!ModelState.IsValid) return View(register);

            var result = await _userService.RegisterUserAsync(register);

            switch (result)
            {
                case RegisterResult.EmailExists:
                    TempData[ErrorMessage] = "ایمیل وارد شده از قبل موجود است";
                    break;
                case RegisterResult.Success:
                    TempData[SuccessMessage] = "ثبت نام با موفقیت انجام شد";
                    return RedirectToAction("Login", "Account");
            }

            return View(register);
        }
        #endregion

        #region Logout
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #endregion

        #region Email Activation
        [HttpGet("ActivationUserEmail/{ActivationCode}")]
        [RedirectToHomeIfUserLoggedActionFilter]
        public Task<IActionResult> ActivationUserEmail(string ActivationCode)
        {
            var result = _userService.ActivateUserEmail(ActivationCode).Result;
            if (string.IsNullOrEmpty(ActivationCode) || !result)
            {
                TempData[ErrorMessage] = "کد فعال سازی نامعتبر است";
                return Task.FromResult<IActionResult>(Redirect("/"));
            }
            else
            {
                TempData[SuccessMessage] = "ایمیل شما با موفقیت تایید شد";
                return Task.FromResult<IActionResult>(RedirectToAction("Login", "Account"));
            }
        }
        #endregion

        #region Forgot Password
        [HttpGet("Forgot-Password")]
        public Task<IActionResult> ForgotPassword()
        {
            return Task.FromResult<IActionResult>(View());
        }

        [HttpPost("Forgot-Password"), ValidateAntiForgeryToken]
        [RedirectToHomeIfUserLoggedActionFilter]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(forgot.Captcha))
            {
                TempData[ErrorMessage] = "اعتبار سنجی Captcha با خطا مواجه شد لطفا مجدد تلاش نمایید";
                return View(forgot);
            }

            if (!ModelState.IsValid) return View(forgot);

            var result = await _userService.ForgotPasswordAsync(forgot);

            switch (result)
            {
                case ForgotPasswordResult.Success:
                    TempData[SuccessMessage] = "لینک بازیابی کلمه عبور به ایمیل شما ارسال شد";
                    return RedirectToAction("Login", "Account");
                case ForgotPasswordResult.UserNotFound:
                    TempData[ErrorMessage] = "کاربر با این ایمیل یافت نشد";
                    break;
                case ForgotPasswordResult.UserIsBan:
                    TempData[ErrorMessage] = "حساب کاربری مسدود میباشد";
                    break;
            }

            return View(forgot);
        }
        #endregion

        #region Reset Password
        [HttpGet("Reset-Password/{emailActivationCode}")]
        [RedirectToHomeIfUserLoggedActionFilter]
        public async Task<IActionResult> ResetPassword(string emailActivationCode)
        {
            var user = await _userService.GetUserByEmailActivationCodeAsync(emailActivationCode);

            if (user == null || user.IsBan || user.IsDelete) return NotFound();

            return View(new ResetPasswordViewModel
            {
                EmailActivationCode = emailActivationCode,
            });
        }
        [HttpPost("Reset-Password/{emailActivationCode}"), ValidateAntiForgeryToken]
        [RedirectToHomeIfUserLoggedActionFilter]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel reset)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(reset.Captcha))
            {
                TempData[ErrorMessage] = "اعتبار سنجی Captcha با خطا مواجه شد لطفا مجدد تلاش نمایید";
                return View(reset);
            }

            if (!ModelState.IsValid) return View(reset);

            var result = await _userService.ResetPassword(reset);

            switch (result)
            {
                case ResetPasswordResult.Success:
                    TempData[SuccessMessage] = "کلمه عبور با موفقیت تغییر یافت";
                    return RedirectToAction("Login", "Account");
                case ResetPasswordResult.UserNotFound:
                    TempData[ErrorMessage] = "کاربر مورد نظر یافت نشد";
                    break;
                case ResetPasswordResult.UserIsBan:
                    TempData[ErrorMessage] = "حساب کاربری وارد شده مسدود میباشد";
                    break;
            }

            return View(reset);

        }
        #endregion
    }
}
