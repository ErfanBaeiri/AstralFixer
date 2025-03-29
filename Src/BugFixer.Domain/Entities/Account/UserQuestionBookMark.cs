using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Entities.Questions;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Account
{
    public class UserQuestionBookMark 
    {
        #region Properties

        [Key]
        public long Id { get; set; }

        public long QuestionId { get; set; }

        public long UserId { get; set; }

        #endregion

        #region Relations

        public Question Question { get; set; }

        public User User { get; set; }

        #endregion
    }
}
