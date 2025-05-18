using BugFixer.Application.Extensions;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Enums;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BugFixer.Web.Controllers
{
    [Route("Question")]
    public class QuestionController : BaseController
    {
        #region Ctor
        private readonly IQuestionService _questionService;
        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        #endregion

        #region Create Question

        [Authorize]
        [HttpGet("Create-Question")]
        public async Task<IActionResult> CreateQuestion()
        {
            return View();
        }

        [Authorize]
        [HttpPost("Create-Question"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(CreateQuestionViewModel createQuestion)
        {
            var tagResult = await _questionService.CheckTagsAsync(createQuestion.SelectedTags, HttpContext.User.GetUserId());

            if (tagResult.Status == CreateQuestionResultEnum.NotValidTag)
            {
                createQuestion.SelectedTagsJson = JsonConvert.SerializeObject(createQuestion.SelectedTags);
                createQuestion.SelectedTags = null;

                TempData[WarningMessage] = tagResult.Message;
                return View(createQuestion);
            }

            if (!ModelState.IsValid)
            {
                createQuestion.SelectedTagsJson = JsonConvert.SerializeObject(createQuestion.SelectedTags);
                createQuestion.SelectedTags = null;
                TempData[WarningMessage] = "اطلاعات ورودی شما معتبر نمی باشد";
                return View(createQuestion);
            }

            createQuestion.UserId = HttpContext.User.GetUserId();

            var result = await _questionService.CreateQuestionAsync(createQuestion);

            if (result)
            {
                TempData[SuccessMessage] = "سوال شما با موفقیت ثبت شد";
                return RedirectToAction("Index", "Home");
            }

            createQuestion.SelectedTagsJson = JsonConvert.SerializeObject(createQuestion.SelectedTags);
            createQuestion.SelectedTags = null;

            return View(createQuestion);
        }

        #endregion

        #region Edit Question
        [HttpGet("edit-question/{id}")]
        [Authorize]
        public async Task<IActionResult> EditQuestion(long id)
        {
            var result = await _questionService.FillEditQuestionViewModel(HttpContext.User.GetUserId(), id);

            if (result == null) return NotFound();

            return View(result);
        }

        [HttpPost("edit-question/{id}"), ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditQuestion(EditQuestionViewModel edit)
        {
            var tagResult = await _questionService.CheckTagsAsync(edit.SelectedTags, HttpContext.User.GetUserId());

            if (tagResult.Status == CreateQuestionResultEnum.NotValidTag)
            {
                edit.SelectedTagsJson = JsonConvert.SerializeObject(edit.SelectedTags);
                edit.SelectedTags = null;

                TempData[WarningMessage] = tagResult.Message;
                return View(edit);
            }

            if (!ModelState.IsValid)
            {
                edit.SelectedTagsJson = JsonConvert.SerializeObject(edit.SelectedTags);
                edit.SelectedTags = null;
                TempData[WarningMessage] = "اطلاعات ورودی شما معتبر نمی باشد";
                return View(edit);
            }

            edit.UserId = HttpContext.User.GetUserId();

            var result = await _questionService.EditQuestionAsync(edit);

            if (result)
            {
                TempData[SuccessMessage] = "سوال شما با موفقیت ثبت شد";
                return RedirectToAction("Index", "Home");
            }

            edit.SelectedTagsJson = JsonConvert.SerializeObject(edit.SelectedTags);
            edit.SelectedTags = null;

            return View(edit);
        }
        #endregion

        #region Get Tags Ajax
        [HttpGet("get-tags")]
        public async Task<IActionResult> GetTagsForSuggest(string? name)
        {
            if (string.IsNullOrEmpty(name)) return Json(null);

            var tags = await _questionService.GetTagsAsync();

            var filteredTags = tags.Where(u => u.Title.Contains(name)).Select(u => u.Title).ToList();

            return Json(filteredTags);
        }
        #endregion

        #region Get Question Ajax
        [HttpGet("get-questions")]
        public async Task<IActionResult> GetQuestionsForSuggest(string name)
        {
            if (string.IsNullOrEmpty(name)) return Json(null);

            var questions = await _questionService.GetAllQuestion();

            var filterQuestions = await questions.Where(s => s.Title.Contains(name)).Select(s => s.Title).ToListAsync();

            return Json(filterQuestions);
        }
        #endregion

        #region Question List
        [HttpGet("Questions")]
        public async Task<IActionResult> QuestionList(FilterQuestionViewModel filter)
        {
            var result = await _questionService.FilterQuestionAsync(filter);

            return View(result);
        }
        #endregion

        #region Filter Question By Tag
        [HttpGet("Tags/{tagName}")]
        public async Task<IActionResult> QuestionListByTag(FilterQuestionViewModel filter, string tagName)
        {
            tagName = tagName.Trim().ToLower().SanitizeText();

            filter.TagTitle = tagName;

            var result = await _questionService.FilterQuestionAsync(filter);

            ViewBag.TagTitle = tagName;

            return View(result);
        }
        #endregion

        #region Filtr Tags
        [HttpGet("tags")]
        public async Task<IActionResult> FilterTags(FilterTagViewModel filter)
        {
            filter.TakeEntityToShow = 12;
            var result = await _questionService.FilterTagAsync(filter);

            return View(result);
        }
        #endregion

        #region Question Detail
        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> QuestionDetail(long questionId)
        {
            var question = await _questionService.GetQuestionById(questionId);

            if (question == null) return NotFound();

            ViewBag.IsBookMark = false;

            if (User.Identity.IsAuthenticated && await _questionService.IsExistQuestionScoreByUserIdAsync(HttpContext.User.GetUserId(), questionId))
            {
                ViewBag.IsBookMark = true;
            }

            var userIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (userIP != null) await _questionService.AddViewForQuestionAsync(userIP, question);

            ViewData["TagsList"] = await _questionService.GetTagListByQuestionIdAsync(questionId);

            return View(question);
        }

        [HttpGet("q/{questionId}")]
        public async Task<IActionResult> QuestionDetailByShortLink(long questionId)
        {
            var question = await _questionService.GetQuestionById(questionId);

            if (question == null) return NotFound();

            return RedirectToAction("QuestionDetail", "Question", new { questionId = questionId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AnswerQuestion(AnswerQuestionViewModel answerQuestion)
        {
            if (string.IsNullOrEmpty(answerQuestion.Answer))
            {
                return new JsonResult(new { status = "EmptyAnswer" });

            }

            answerQuestion.UserId = HttpContext.User.GetUserId();

            var result = await _questionService.AnswerQuestion(answerQuestion);

            if (result) return new JsonResult(new { status = "Success" });

            return new JsonResult(new { status = "Error" });

        }

        [HttpGet("EditAnswer/{answerId}")]
        [Authorize]
        public async Task<IActionResult> EditAnswer(long answerId)
        {

            var result = await _questionService.FillEditAnswerViewModel(answerId, HttpContext.User.GetUserId());

            if (result == null) return NotFound();

            return View(result);
        }
        [HttpPost("EditAnswer/{answerId}"), ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditAnswer(EditAnswerViewModel edit)
        {
            if (!ModelState.IsValid)
                return View(edit);
            edit.UserId = HttpContext.User.GetUserId();

            var result = await _questionService.EditAnswer(edit);

            if (result)
            {
                TempData[SuccessMessage] = "عملیات با موفقیت انجام شد";
                return RedirectToAction("QuestionDetail", "Question", new { questionId = edit.QuestionId });
            }

            TempData[ErrorMessage] = "خطایی رخ داده است";

            return View(edit);
        }
        #endregion

        #region Select True Answer
        [HttpPost("SelectTrueAnswer")]
        public async Task<IActionResult> SelectTrueAnswer(long answerId)
        {
            if (!User.Identity.IsAuthenticated) return new JsonResult(new { status = "NotAuthenticated" });

            if (!await _questionService.HasUserAccessToSelectTrueAnswer(HttpContext.User.GetUserId(), answerId))
            {
                return new JsonResult(new { status = "NotAccess" });
            }

            await _questionService.SelectTrueAnswer(answerId);
            return new JsonResult(new { status = "Success" });
        }
        #endregion

        #region Score Answer
        [HttpPost("ScoreUpForAnswer")]
        public async Task<IActionResult> ScoreUpForAnswer(long answerId)
        {
            var result = await _questionService.CreateScoreForAnswer(HttpContext.User.GetUserId(), answerId, AnswerScoreType.Plus);

            switch (result)
            {
                case CreateScoreForAnswerResult.Error:
                    return new JsonResult(new { status = "Error" });
                case CreateScoreForAnswerResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForAnswerResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForAnswerResult.Success:
                    return new JsonResult(new { status = "Success" });
                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForAnswerResult.UserDontLogged:
                    return new JsonResult(new { status = "UserDontLogged" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        [HttpPost("ScoreDownForAnswer")]
        public async Task<IActionResult> ScoreDownForAnswer(long answerId)
        {
            var result = await _questionService.CreateScoreForAnswer(HttpContext.User.GetUserId(), answerId, AnswerScoreType.Minus);

            switch (result)
            {
                case CreateScoreForAnswerResult.Error:
                    return new JsonResult(new { status = "Error" });
                case CreateScoreForAnswerResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForAnswerResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForAnswerResult.Success:
                    return new JsonResult(new { status = "Success" });
                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForAnswerResult.UserDontLogged:
                    return new JsonResult(new { status = "UserDontLogged" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region Score Question
        [HttpPost("ScoreQuestionPlus")]
        public async Task<IActionResult> ScoreQuestionPlus(long questionId)
        {
            var result = await _questionService.AddQuestionScore(HttpContext.User.GetUserId(), questionId, QuestionScoreType.Plus);

            switch (result)
            {
                case CreateScoreForQuestionResult.Error:
                    return new JsonResult(new { status = "Error" });
                case CreateScoreForQuestionResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForQuestionResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForQuestionResult.Success:
                    return new JsonResult(new { status = "Success" });
                case CreateScoreForQuestionResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForQuestionResult.UserDontLogged:
                    return new JsonResult(new { status = "UserDontLogged" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpPost("ScoreQuestionMinus")]
        public async Task<IActionResult> ScoreQuestionMinus(long questionId)
        {
            var result = await _questionService.AddQuestionScore(HttpContext.User.GetUserId(), questionId, QuestionScoreType.Plus);

            switch (result)
            {
                case CreateScoreForQuestionResult.Error:
                    return new JsonResult(new { status = "Error" });
                case CreateScoreForQuestionResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForQuestionResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForQuestionResult.Success:
                    return new JsonResult(new { status = "Success" });
                case CreateScoreForQuestionResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForQuestionResult.UserDontLogged:
                    return new JsonResult(new { status = "UserDontLogged" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region Add Question To BookMark
        [HttpGet("AddQueestionToBookMark")]
        public async Task<IActionResult> AddQueestionToBookMark(long questionId)
        {
            if (!User.Identity.IsAuthenticated) return new JsonResult(new { status = "NotAuthorized" });

            var result = await _questionService.AddQuestionToBookMark(HttpContext.User.GetUserId(), questionId);

            if (result == false) return new JsonResult(new { status = "Error" });

            return new JsonResult(new { status = "Success" });
        }
        #endregion
    }
}
