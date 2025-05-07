using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.IdentityModel.Tokens;
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
    }
}
