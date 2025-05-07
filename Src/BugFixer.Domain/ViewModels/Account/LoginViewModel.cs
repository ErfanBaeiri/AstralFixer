using BugFixer.Domain.ViewModels.common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Account
{
    public class LoginViewModel : GoogleRecaptchaViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }

    public enum LoginResult
    {
        Success,
        UserIsBan,
        UserNotFound,
        UserNotActive,
    }
}
