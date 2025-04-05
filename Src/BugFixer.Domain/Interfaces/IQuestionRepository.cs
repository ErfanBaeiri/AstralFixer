using BugFixer.Domain.Entities.Tags;

namespace BugFixer.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        #region Tags
        Task<List<Tag>> GetTags();
        #endregion
    }
}
