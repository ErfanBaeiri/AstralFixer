using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.UserPanel.Account
{
    public class EditUserViewModel
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? LastName { get; set; }

        [Display(Name = "شماره تماس")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "تاریخ تولد")]
        [RegularExpression("""^\d{4}$|^\d{4}/((0?\d)|(1[012]))/(((0?|[12])\d)|3[01])$""", ErrorMessage = "{0} باید به فرمت yyyy/MM/dd باشد")]
        public string? Birthdate { get; set; }
        [Display(Name = "کشور")]
        public long? CountryId { get; set; }
        [Display(Name = "شهر")]
        public long? CityId { get; set; }

        public bool GetNewsletter { get; set; }
    }
    public enum EditUserInfoResult
    {
        Success,
        NotValidDate,
    }
}
