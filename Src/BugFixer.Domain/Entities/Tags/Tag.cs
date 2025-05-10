using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Entities.Questions;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Tags
{
    public class Tag : BaseEntity
    {
        #region Propertise

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Title { get; set; }
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
        public int UseCount { get; set; } = 0;
        #endregion

        #region Relation
        public ICollection<SelectQuestionTag> SelectQuestionTags { get; set; }

        #endregion
    }
}
