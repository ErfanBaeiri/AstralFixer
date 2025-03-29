using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Questions
{
    public class Answer : BaseEntity
    {
        #region Properties

        [Display(Name = "پاسخ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Content { get; set; }

        public long UserId { get; set; }

        [Display(Name = "امتیاز")]
        public int Score { get; set; } = 0;

        public bool IsTrue { get; set; }

        public long QuestionId { get; set; }

        #endregion

        #region Relations

        public User User { get; set; }

        public Question Question { get; set; }

        #endregion
    }
}
