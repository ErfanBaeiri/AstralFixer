using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Enums;

namespace BugFixer.Domain.Entities.Questions
{
    public class QuestionUserScore : BaseEntity
    {
        #region Propertise
        public long UserId { get; set; }

        public long QuestionId { get; set; }

        public QuestionScoreType ScoreType { get; set; }
        #endregion

        #region Relation
        public User User { get; set; }
        public Question Question { get; set; }
        #endregion
    }
}
