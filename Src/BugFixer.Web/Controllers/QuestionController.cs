using BugFixer.Application.Extensions;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Enums;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BugFixer.Web.Controllers
{
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

        #region Get Tags
        [HttpGet("get-tags")]
        public async Task<IActionResult> GetTagsForSuggest(string? name)
        {
            if (string.IsNullOrEmpty(name)) return Json(null);

            var tags = await _questionService.GetTagsAsync();

            var filteredTags = tags.Where(u => u.Title.Contains(name)).Select(u => u.Title).ToList();

            return Json(filteredTags);
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
    }
}
