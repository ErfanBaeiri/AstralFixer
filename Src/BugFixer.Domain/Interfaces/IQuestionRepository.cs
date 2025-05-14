using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.ViewModels.Question;
using System.Threading.Tasks;

namespace BugFixer.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        #region Tags
        Task<List<Tag>> GetTagsAsync();
        Task<IQueryable<Tag>> GetAllTagsAsQueryableAsync();
        Task<List<string>> GetTagListByQuestionIdAsync(long questionId);
        Task<Tag?> GetTagByName(string tag);
        Task<bool> IsExistsTagByNameAsync(string tag);
        Task<int> RequestCountForTagAsync(string tag);
        Task AddTagAsync(Tag tag);
        Task<bool> CheckUserRequestedForTag(long userId, string tag);
        Task AddRequestTagAsync(RequestTag tag);
        Task UpdateTagAsync(Tag tag);
        Task SaveChangesAsync();
        #endregion

        #region Question
        Task AddQuestionAsync(Question question);
        Task updateQuestionAsync(Question question);
        Task<IQueryable<Question>> GetAllQuestions();
        Task<Question?> GetQuestionByIdAsync(long questionId);
        #endregion

        #region View
        Task<bool> IsExistViewforQuestAsync(string userIP, long questionId);
        Task AddViewForQuestionAsync(QuestionView questionView);
        #endregion

        #region Selected Tag
        Task AddSelectQuestionTagsAsync(SelectQuestionTag selectQuestionTag);
        #endregion

        #region Answer
        Task AddAnswerByUserAsync(Answer answer);
        Task UpdateAnswerAsync(Answer answer);

        Task<List<Answer>> GetAllQuestionAnswerAsync(long questionId);
        Task<Answer?> GetAnswerByIdAsync(long answerId);
        #endregion

    }
}
