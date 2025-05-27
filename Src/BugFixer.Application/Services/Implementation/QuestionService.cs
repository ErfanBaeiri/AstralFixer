using BugFixer.Application.Extensions;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Enums;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Admin.Tag;
using BugFixer.Domain.ViewModels.Common;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Domain.ViewModels.UserPanel.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

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
        public async Task<CreateScoreForQuestionResult> AddQuestionScore(long userId, long questionId, QuestionScoreType type)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (question == null) return CreateScoreForQuestionResult.Error;

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return CreateScoreForQuestionResult.UserDontLogged;

            if (await _questionRepository.IsExistQuestionScoreByUserIdAsync(userId, questionId))
                return CreateScoreForQuestionResult.UserCreateScoreBefore;

            if (type == QuestionScoreType.Minus)
                if (user.Score < _scoreManagement.MinScoreForDownScoreAnswer)
                    return CreateScoreForQuestionResult.NotEnoughScoreForDown;

            if (type == QuestionScoreType.Plus)
                if (user.Score < _scoreManagement.MinScoreForUpScoreAnswer)
                    return CreateScoreForQuestionResult.NotEnoughScoreForUp;

            var score = new QuestionUserScore
            {
                QuestionId = questionId,
                UserId = userId,
                ScoreType = type
            };

            await _questionRepository.AddScoreToQuestionByUser(score);

            if (type == QuestionScoreType.Plus)
                question.Score++;
            if (type == QuestionScoreType.Minus)
                question.Score--;

            await _questionRepository.updateQuestionAsync(question);

            await _questionRepository.SaveChangesAsync();

            return CreateScoreForQuestionResult.Success;

        }
        public async Task<bool> AddQuestionToBookMark(long userId, long questionId)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (question == null) return false;

            var bookMark = await _questionRepository.GetQuestionBookMarkByQuestionAndUserId(userId, questionId);

            if (bookMark != null)
            {
                await _questionRepository.RemoveQuestionToBookMarkAsync(bookMark);
            }
            else
            {
                bookMark = new UserQuestionBookMark
                {
                    QuestionId = questionId,
                    UserId = userId
                };

                await _questionRepository.AddQuestionToBookMarkAsync(bookMark);
            }

            await _questionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsExistQuestionScoreByUserIdAsync(long userId, long questionId)
        {
            return await _questionRepository.IsExistsQuestionInUserBookMarks(userId, questionId);
        }
        public async Task<EditQuestionViewModel?> FillEditQuestionViewModel(long userId, long questionId)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (question == null) return null;

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return null;

            if (user.IsAdmin == false && question.UserId != user.Id) return null;

            var tags = await GetTagListByQuestionIdAsync(questionId);

            var result = new EditQuestionViewModel
            {
                Description = question.Content,
                Title = question.Title,
                Id = question.Id,
                SelectedTagsJson = JsonConvert.SerializeObject(tags)
            };

            return result;

        }
        public async Task<bool> EditQuestionAsync(EditQuestionViewModel edit)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(edit.Id);

            if (question == null) return false;

            var user = await _userService.GetUserByIdAsync(edit.UserId);

            if (user == null) return false;

            if (question.UserId != edit.UserId && !user.IsAdmin) return false;

            FileExtension.ManageEditorImages(question.Content, edit.Description, PathTools.CkEditorImageFullPath);

            #region Delete Current Tags
            question.Title = edit.Title;
            question.Content = edit.Description;

            var currentTags = question.SelectQuestionTags.ToList();

            foreach (var tag in currentTags)
            {
                await _questionRepository.RemoveSelectQuestionTagAsync(tag);
            }
            #endregion

            #region Add New Tags
            if (edit.SelectedTags != null && edit.SelectedTags.Any())
            {
                foreach (var questionSelectedTag in edit.SelectedTags)
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
            #endregion

            return true;

        }

        public async Task<IQueryable<Question>> GetAllQuestion()
        {
            return await _questionRepository.GetAllQuestions();
        }

        public async Task<FilterQuestionBookMarksViewModel> FilterQuestionBookMarks(FilterQuestionBookMarksViewModel filter)
        {
            var query = await _questionRepository.GetAllBookMarks();

            query = query.Where(s => s.UserId == filter.UserId);

            await filter.SetPaging(query.Select(s => s.Question).AsQueryable());

            return filter;
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

        public async Task<bool> HasUserAccessToSelectTrueAnswer(long userId, long questionId)
        {
            var answer = await _questionRepository.GetAnswerByIdAsync(questionId);
            if (answer == null) return false;

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return false;
            if (user.IsAdmin) return true;
            if (answer.Question.UserId != userId) return false;

            return true;
        }

        public async Task SelectTrueAnswer(long answerId)
        {
            var answer = await _questionRepository.GetAnswerByIdAsync(answerId);

            if (answer == null) return;

            answer.IsTrue = !answer.IsTrue;

            await _questionRepository.UpdateAnswerAsync(answer);
            await _questionRepository.SaveChangesAsync();
        }

        public async Task<CreateScoreForAnswerResult> CreateScoreForAnswer(long userId, long answerId, AnswerScoreType type)
        {
            var answer = await _questionRepository.GetAnswerByIdAsync(answerId);
            if (answer == null) return CreateScoreForAnswerResult.Error;

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return CreateScoreForAnswerResult.UserDontLogged;

            if (type == AnswerScoreType.Minus && user.Score < _scoreManagement.MinScoreForDownScoreAnswer)
                return CreateScoreForAnswerResult.NotEnoughScoreForDown;

            if (type == AnswerScoreType.Plus && user.Score < _scoreManagement.MinScoreForUpScoreAnswer)
                return CreateScoreForAnswerResult.NotEnoughScoreForUp;

            if (await _questionRepository.IsExistsUserScoreForAnswer(userId, answerId))
                return CreateScoreForAnswerResult.UserCreateScoreBefore;

            var score = new AnswerUserScore
            {
                AnswerId = answerId,
                UserId = userId,
                Type = type
            };

            await _questionRepository.AddAnswerUserScoreAsync(score);

            if (type == AnswerScoreType.Plus)
                answer.Score++;
            else
                answer.Score--;

            await _questionRepository.UpdateAnswerAsync(answer);

            await _questionRepository.SaveChangesAsync();

            return CreateScoreForAnswerResult.Success;
        }

        public async Task<EditAnswerViewModel> FillEditAnswerViewModel(long answerId, long userId)
        {
            var answer = await _questionRepository.GetAnswerByIdAsync(answerId);
            if (answer == null) return null;

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return null;

            if (user.IsAdmin == false && answer.UserId != user.Id)
                return null;


            return new EditAnswerViewModel
            {
                Answer = answer.Content,
                AnswerId = answer.Id,
                QuestionId = answer.QuestionId
            };

        }

        public async Task<bool> EditAnswer(EditAnswerViewModel editAnswer)
        {
            var answer = await _questionRepository.GetAnswerByIdAsync(editAnswer.AnswerId);
            if (answer == null) return false;

            if (answer.QuestionId != editAnswer.QuestionId) return false;

            var user = await _userService.GetUserByIdAsync(editAnswer.UserId);
            if (user == null) return false;

            if (user.IsAdmin == false && answer.UserId != user.Id)
                return false;

            answer.Content = editAnswer.Answer;

            await _questionRepository.UpdateAnswerAsync(answer);
            await _questionRepository.SaveChangesAsync();

            return true;
        }
        #endregion

        #region Admin
        public async Task<List<TagViewModelJson>> GetTagViewModelJson()
        {
            var tags = await _questionRepository.GetAllTagsAsQueryableAsync();

            return tags.OrderByDescending(s => s.UseCount).Take(10).Select(s => new TagViewModelJson
            {
                Title = s.Title,
                UseCount = s.UseCount
            }).ToList();
        }
        public async Task<FilterTagAdminViewModel> FilterTagAdmin(FilterTagAdminViewModel filter)
        {
            var query = await _questionRepository.GetAllTagsAsQueryableAsync();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => s.Title.Contains(filter.Title));
            }

            switch (filter.Status)
            {
                case FilterTagAdminStatus.All:
                    query = query.OrderByDescending(s => s.CreateDate);
                    break;
                case FilterTagAdminStatus.HasDescription:
                    query = query.Where(s => !string.IsNullOrEmpty(s.Description));
                    break;
                case FilterTagAdminStatus.NoDescription:
                    query = query.Where(s => string.IsNullOrEmpty(s.Description));
                    break;
            }

            await filter.SetPaging(query);

            return filter;
        }
        public async Task CreateTagAdmin(CreateTagAdminViewModel createTagAdminViewModel)
        {
            var tag = new Tag
            {
                Description = createTagAdminViewModel.Description,
                Title = createTagAdminViewModel.Title,
            };

            await _questionRepository.AddTagAsync(tag);
            await _questionRepository.SaveChangesAsync();
        }

        public async Task<EditTagAdminViewModel?> FillEditTagAdminViewModel(long id)
        {
            var tag = await _questionRepository.GetTagById(id);

            if (tag == null || tag.IsDelete)
                return null;

            var result = new EditTagAdminViewModel
            {
                Description = tag.Description,
                Id = tag.Id,
                Title = tag.Title
            };

            return result;
        }

        public async Task<bool> EditTagAdmin(EditTagAdminViewModel edit)
        {
            var tag = await _questionRepository.GetTagById(edit.Id);

            if (tag == null || tag.IsDelete)
                return false;

            tag.Title = edit.Title;
            tag.Description = edit.Description;

            await _questionRepository.UpdateTagAsync(tag);
            await _questionRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTagAdmin(long id)
        {
            var tag = await _questionRepository.GetTagById(id);

            if (tag == null || tag.IsDelete == true)
                return false;

            tag.IsDelete = true;

            await _questionRepository.UpdateTagAsync(tag);
            await _questionRepository.SaveChangesAsync();

            return true;
        }

        #endregion

    }
}