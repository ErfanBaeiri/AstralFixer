using BugFixer.Domain.ViewModels.common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Account
{
    public class ForgotPasswordViewModel : GoogleRecaptchaViewModel
    {

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Email { get; set; }
    }
    public enum ForgotPasswordResult
    {
        Success,
        UserNotFound,
        UserIsBan,
    }
}
