using BugFixer.Domain.Entities.Tags;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IQuestionService
    {
        #region Tags
        Task<List<Tag>> GetAllTags();
        #endregion
    }
}
