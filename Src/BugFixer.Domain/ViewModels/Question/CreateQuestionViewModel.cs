using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Question
{
    public class CreateQuestionViewModel
    {
        [Display(Name = "عنوان")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }
        public List<string> SelectedTags { get; set; }
        public long UserId { get; set; }
    }
}
