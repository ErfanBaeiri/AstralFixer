using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.ViewModels.Question;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IQuestionService
    {
        #region Tags
        Task<List<Tag>> GetTagsAsync();
        Task<CreateQuestionResult> CheckTagsAsync(List<string> tags, long userId);
        Task<bool> CreateQuestionAsync(CreateQuestionViewModel createQuestion);
        Task<FilterTagViewModel> FilterTagAsync(FilterTagViewModel filterTag);
        #endregion

        #region Question
        Task<FilterQuestionViewModel> FilterQuestionAsync(FilterQuestionViewModel filterQuestion);
        Task<Question?> GetQuestionById(long questionId);
        #endregion
    }
}
