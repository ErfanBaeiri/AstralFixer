using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Enums;

namespace BugFixer.Domain.Entities.Questions
{
    public class AnswerUserScore : BaseEntity
    {
        #region Properties
        public long UserId { get; set; }
        public long AnswerId { get; set; }
        public AnswerScoreType Type { get; set; }
        #endregion


        #region Relation
        public User User { get; set; }
        public Answer Answer { get; set; }
        #endregion
    }
}
