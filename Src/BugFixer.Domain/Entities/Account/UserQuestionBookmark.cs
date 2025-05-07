using BugFixer.Domain.Entities.Questions;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Account
{
    public class UserQuestionBookMark 
    {
        #region Propertise
        [Key]
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public long UserId { get; set; }
        #endregion

        #region Relation
        public Question Question { get; set; }
        public User User { get; set; }
        #endregion
    }
}
