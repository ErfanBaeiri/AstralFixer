using BugFixer.Application.Extensions;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Common;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BugFixer.Application.Services.Implementation
{
    public class QuestionService : IQuestionService
    {
        #region Dependeny Injection
        private readonly IQuestionRepository _questionRepository;
        private readonly ScoreManagementViewModel _scoreManagement;
        private readonly IUserService _userService;
        public QuestionService(IQuestionRepository questionRepository, IOptions<ScoreManagementViewModel> scoreManagement, IUserService userService)
        {
            _questionRepository = questionRepository;
            _scoreManagement = scoreManagement.Value;
            _userService = userService;
        }

        #endregion

        #region Tags

        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _questionRepository.GetTagsAsync();
        }

        public async Task<CreateQuestionResult> CheckTagsAsync(List<string> tags, long userId)
        {
            if (tags != null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    var isExistsTag = await _questionRepository.IsExistsTagByNameAsync(tag.SanitizeText().ToLower().Trim());

                    if (isExistsTag) continue;

                    var isUserRequestedForTag = await _questionRepository.CheckUserRequestedForTag(userId, tag.SanitizeText().ToLower().Trim());

                    if (isUserRequestedForTag)
                    {
                        return new CreateQuestionResult
                        {
                            Status = CreateQuestionResultEnum.NotValidTag,
                            Message = $"تگ {tag} برای اعتبار سنجی نیاز به {_scoreManagement.MinRequestCountForVerifyTag} درخواست دارد"
                        };
                    }

                    var tagRequest = new RequestTag
                    {
                        Title = tag.SanitizeText().Trim().ToLower(),
                        UserId = userId
                    };

                    await _questionRepository.AddRequestTagAsync(tagRequest);
                    await _questionRepository.SaveChangesAsync();

                    var requestedCount = await _questionRepository.RequestCountForTagAsync(tag.SanitizeText().ToLower().Trim());

                    if (requestedCount < _scoreManagement.MinRequestCountForVerifyTag)
                    {
                        return new CreateQuestionResult
                        {
                            Status = CreateQuestionResultEnum.NotValidTag,
                            Message = $"تگ {tag} برای اعتبار سنجی نیاز به {_scoreManagement.MinRequestCountForVerifyTag} درخواست دارد"
                        };
                    }

                    var newTag = new Tag
                    {
                        Title = tag.SanitizeText().Trim().ToLower(),
                    };

                    await _questionRepository.AddTagAsync(newTag);
                    await _questionRepository.SaveChangesAsync();

                }


                return new CreateQuestionResult
                {
                    Status = CreateQuestionResultEnum.Success,
                    Message = "تگ های ورودی معتبر می باشد."
                };

            }

            return new CreateQuestionResult
            {
                Status = CreateQuestionResultEnum.NotValidTag,
                Message = "تگ های ورودی نمی تواند خالی باشد"
            };
        }

        public async Task<bool> CreateQuestionAsync(CreateQuestionViewModel createQuestion)
        {
            var question = new Domain.Entities.Questions.Question
            {
                Content = createQuestion.Description.SanitizeText(),
                Title = createQuestion.Title.SanitizeText(),
                UserId = createQuestion.UserId
            };

            await _questionRepository.AddQuestionAsync(question);
            await _questionRepository.SaveChangesAsync();

            if (createQuestion.SelectedTags != null && createQuestion.SelectedTags.Any())
            {
                foreach (var questionSelectedTag in createQuestion.SelectedTags)
                {
                    var tag = await _questionRepository.GetTagByName(questionSelectedTag.SanitizeText().Trim().ToLower());

                    if (tag == null) continue;

                    tag.UseCount += 1;

                    await _questionRepository.UpdateTagAsync(tag);

                    var selectedTag = new SelectQuestionTag
                    {
                        QuestionId = question.Id,
                        TagId = tag.Id
                    };

                    await _questionRepository.AddSelectQuestionTagsAsync(selectedTag);
                }
                await _questionRepository.SaveChangesAsync();
            }

            await _userService.UpdateUserScoreAndMedalAsync(createQuestion.UserId, _scoreManagement.AddNewQuestionScore);

            return true;
        }
        public async Task<FilterTagViewModel> FilterTagAsync(FilterTagViewModel filterTag)
        {

            var query = await _questionRepository.GetAllTagsAsQueryableAsync();

            if (!string.IsNullOrEmpty(filterTag.Title))
            {
                query = query.Where(s => s.Title.Contains(filterTag.Title));
            }

            switch (filterTag.Sort)
            {
                case FilterTagEnum.NewToOld:
                    query = query.OrderByDescending(s => s.CreateDate);
                    break;
                case FilterTagEnum.OldToNew:
                    query = query.OrderBy(s => s.CreateDate);
                    break;
                case FilterTagEnum.UseCountHighToLow:
                    query = query.OrderByDescending(s => s.UseCount);
                    break;
                case FilterTagEnum.UseCountLowToHigh:
                    query = query.OrderBy(s => s.UseCount);
                    break;
            }

            await filterTag.SetPaging(query);

            return filterTag;
        }
        public async Task<List<string>> GetTagListByQuestionIdAsync(long questionId)
        {
            return await _questionRepository.GetTagListByQuestionIdAsync(questionId);
        }
        #endregion

        #region Question
        public async Task<FilterQuestionViewModel> FilterQuestionAsync(FilterQuestionViewModel filterQuestion)
        {
            var query = await _questionRepository.GetAllQuestions();

            #region Filter By Tag
            if (!string.IsNullOrEmpty(filterQuestion.TagTitle))
            {
                query = query.Include(s => s.SelectQuestionTags).ThenInclude(s => s.Tag)
                    .Where(s => s.SelectQuestionTags.Any(a => a.Tag.Title.Equals(filterQuestion.TagTitle)));
            }
            #endregion

            switch (filterQuestion.Sort)
            {
                case FilterQuestionEnum.NewToOld:
                    query = query.OrderByDescending(u => u.CreateDate);
                    break;
                case FilterQuestionEnum.OldToNew:
                    query = query.OrderBy(u => u.CreateDate);
                    break;
                case FilterQuestionEnum.ScoreHighToLow:
                    query = query.OrderByDescending(u => u.Score);
                    break;
                case FilterQuestionEnum.ScoreLowToHigh:
                    query = query.OrderBy(u => u.Score);
                    break;

            }

            var result = query
                .Include(s => s.Answers)
                .Include(s => s.SelectQuestionTags).ThenInclude(a => a.Tag)
                .Include(s => s.User)
                .Select(s => new QuestionListViewModel
                {
                    AnswerCount = s.Answers.Count(s => !s.IsDelete),
                    HasAnyAnswer = s.Answers.Any(s => !s.IsDelete),
                    HasAnyTrueAnswer = s.Answers.Any(s => !s.IsDelete && s.IsTrue),
                    QuestionId = s.Id,
                    Score = s.Score,
                    Title = s.Title,
                    ViewCount = s.ViewCount,
                    UserQuestionName = s.User.GetUserDisplayName(),
                    Tags = s.SelectQuestionTags.Where(a => !a.Tag.IsDelete).Select(a => a.Tag.Title).ToList(),
                    AnswerUserDispalyName = s.Answers.Any(a => !a.IsDelete) ? s.Answers.OrderByDescending(a => a.CreateDate).First().User.GetUserDisplayName() : null,
                    CreateDate = s.CreateDate.AsTimeAgo(),
                    CreateDateAnswer = s.Answers.Any(a => !a.IsDelete) ? s.Answers.OrderByDescending(a => a.CreateDate).First().CreateDate.AsTimeAgo() : null

                }).AsQueryable();

            await filterQuestion.SetPaging(result);
            return filterQuestion;
        }

        public async Task<Question?> GetQuestionById(long questionId)
        {
            return await _questionRepository.GetQuestionByIdAsync(questionId);
        }

        public async Task AddViewForQuestionAsync(string userIP, Question question)
        {
            if (await _questionRepository.IsExistViewforQuestAsync(userIP, question.Id)) return;

            var view = new QuestionView
            {
                UserIP = userIP,
                QuestionId = question.Id
            };
            await _questionRepository.AddViewForQuestionAsync(view);
            await _questionRepository.SaveChangesAsync();
            question.ViewCount += 1;
            await _questionRepository.updateQuestionAsync(question);
            await _questionRepository.SaveChangesAsync();
        }
        #endregion

        #region Answer
        public async Task<bool> AnswerQuestion(AnswerQuestionViewModel answerQuestion)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(answerQuestion.QuestionId);

            if (question == null) return false;

            var answer = new Answer
            {
                Content = answerQuestion.Answer.SanitizeText(),
                QuestionId = answerQuestion.QuestionId,
                UserId = answerQuestion.UserId,

            };

            await _questionRepository.AddAnswerByUserAsync(answer);
            await _questionRepository.SaveChangesAsync();

           await _userService.UpdateUserScoreAndMedalAsync(answerQuestion.UserId, _scoreManagement.AddNewAnswerScore);

            return true;
        }



        public async Task<List<Answer>> GetAllQuestionAnswerAsync(long questionId)
        {
            return await _questionRepository.GetAllQuestionAnswerAsync(questionId);
        }


        #endregion

    }
}