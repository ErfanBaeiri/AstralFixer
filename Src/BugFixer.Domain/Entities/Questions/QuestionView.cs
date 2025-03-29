using BugFixer.Domain.Entities.Common;

namespace BugFixer.Domain.Entities.Questions
{
    public class QuestionView : BaseEntity
    {
        #region Priperties

        public string UserIP { get; set; }

        public long QuestionId { get; set; }

        #endregion

        #region Relations

        public Question Question { get; set; }

        #endregion
    }
}
