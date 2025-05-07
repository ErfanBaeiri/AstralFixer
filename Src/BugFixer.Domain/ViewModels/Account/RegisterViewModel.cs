using BugFixer.Domain.ViewModels.common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Account
{
    public class RegisterViewModel : GoogleRecaptchaViewModel
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

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Compare("Password", ErrorMessage = "کلمه عبور و تکرار آن یکسان نمی باشد")]
        public string RePassword { get; set; }

    }
    public enum RegisterResult
    {
        EmailExists,
        Success,
    }
}
