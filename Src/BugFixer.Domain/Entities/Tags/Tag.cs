using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Entities.Questions;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Tags
{
    public class Tag : BaseEntity
    {
        #region Properties

        [Display(Name = "عنوان")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        #endregion
        #region Relations

        public ICollection<SelectQuestionTag> SelectQuestionTags { get; set; }

        #endregion

    }
}
