using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Enums;
using BugFixer.Domain.ViewModels.Admin.Tag;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Domain.ViewModels.UserPanel.Question;

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
        Task<List<string>> GetTagListByQuestionIdAsync(long questionId);
        Task AddViewForQuestionAsync(string userIP, Question question);
        Task<CreateScoreForQuestionResult> AddQuestionScore(long userId, long questionId, QuestionScoreType type);
        Task<bool> AddQuestionToBookMark(long userId, long questionId);
        Task<bool> IsExistQuestionScoreByUserIdAsync(long userId, long questionId);
        Task<EditQuestionViewModel?> FillEditQuestionViewModel(long userId, long questionId);
        Task<bool> EditQuestionAsync(EditQuestionViewModel edit);
        Task<IQueryable<Question>> GetAllQuestion();
        Task<FilterQuestionBookMarksViewModel> FilterQuestionBookMarks(FilterQuestionBookMarksViewModel filter);
        #endregion

        #region Answer
        Task<bool> AnswerQuestion(AnswerQuestionViewModel answerQuestion);
        Task<List<Answer>> GetAllQuestionAnswerAsync(long questionId);
        Task<bool> HasUserAccessToSelectTrueAnswer(long userId, long questionId);
        Task SelectTrueAnswer(long answerId);
        Task<CreateScoreForAnswerResult> CreateScoreForAnswer(long userId, long answerId, AnswerScoreType type);
        Task<EditAnswerViewModel> FillEditAnswerViewModel(long answerId, long userId);
        Task<bool> EditAnswer(EditAnswerViewModel editAnswer);
        #endregion

        #region Admin
        Task<List<TagViewModelJson>> GetTagViewModelJson();
        Task<FilterTagAdminViewModel> FilterTagAdmin(FilterTagAdminViewModel filter);
        Task CreateTagAdmin(CreateTagAdminViewModel createTagAdminViewModel);
        Task<EditTagAdminViewModel?> FillEditTagAdminViewModel(long id);
        Task<bool> EditTagAdmin(EditTagAdminViewModel edit);
        Task<bool> DeleteTagAdmin(long id);
        #endregion
    }
}
