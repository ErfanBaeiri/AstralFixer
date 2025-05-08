using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using System.Threading.Tasks;

namespace BugFixer.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        #region Tags
        Task<List<Tag>> GetTagsAsync();
        Task<Tag?> GetTagByName(string tag);
        Task<bool> IsExistsTagByNameAsync(string tag);
        Task<int> RequestCountForTagAsync(string tag);
        Task AddTagAsync(Tag tag);
        Task<bool> CheckUserRequestedForTag(long userId, string tag);
        Task AddRequestTagAsync(RequestTag tag);
        Task SaveChangesAsync();
        #endregion

        #region Question
        Task AddQuestionAsync(Question question);
         Task<IQueryable<Question>> GetAllQuestions();
        #endregion

        #region Selected Tag
        Task AddSelectQuestionTagsAsync(SelectQuestionTag selectQuestionTag);
        #endregion

    }
}
