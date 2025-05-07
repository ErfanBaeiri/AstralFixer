using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Common;
using BugFixer.Domain.ViewModels.Question;
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
            var question = new Question
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


        #endregion
    }
}