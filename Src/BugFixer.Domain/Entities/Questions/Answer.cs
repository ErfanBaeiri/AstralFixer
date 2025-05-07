using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Questions
{
    public class Answer : BaseEntity
    {
        #region Propertise

        [Display(Name = "پاسخ")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        public string Content { get; set; }

        public long QuestionId { get; set; }

        public long UserId { get; set; }

        [Display(Name = "امتیاز")]
        public int Score { get; set; } = 0;

        public bool IsTrue { get; set; }
        #endregion

        #region Relation
        public Question Question { get; set; }
        public User User { get; set; }
        #endregion
    }
}
