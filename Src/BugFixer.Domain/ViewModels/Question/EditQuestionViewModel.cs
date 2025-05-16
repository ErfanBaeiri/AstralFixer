using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Question
{
    public class EditQuestionViewModel
    {
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        public string Description { get; set; }

        public List<string>? SelectedTags { get; set; }

        public string? SelectedTagsJson { get; set; }

        public long UserId { get; set; }
    }
}
