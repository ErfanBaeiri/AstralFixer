using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.UserPanel.Account
{
    public class ChangeUserPasswordViewModel
    {
        [Display(Name = "کلمه عبور فعلی")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string CurrentPassword { get; set; }

        [Display(Name = "کلمه عبور جدید")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string NewPassword { get; set; }

        [Display(Name = "تکرار کلمه عبور جدید")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string ConfirmNewPassword { get; set; }
    }
    public enum ChangeUserPasswordResult
    {
        Success,
        CurrntPasswordNotValid
    }
}
