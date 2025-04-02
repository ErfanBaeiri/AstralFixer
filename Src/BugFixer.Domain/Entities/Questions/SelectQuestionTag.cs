using BugFixer.Domain.Entities.Tags;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Questions
{
    public class SelectQuestionTag
    {
        #region Propertise
        [Key]
        public long Id { get; set; }

        public long QuestionId { get; set; }
        public long TagId { get; set; }
        #endregion

        #region Relations
        public Question Question { get; set; }
        public Tag Tag { get; set; }
        #endregion
    }
}
