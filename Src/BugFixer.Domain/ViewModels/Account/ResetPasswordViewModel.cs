using BugFixer.Domain.ViewModels.common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Account
{
    public class ResetPasswordViewModel : GoogleRecaptchaViewModel
    {
        [Required]
        public string EmailActivationCode { get; set; }

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

    public enum ResetPasswordResult
    {
        Success,
        UserNotFound,
        UserIsBan
    }
}
