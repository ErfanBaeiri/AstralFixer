using BugFixer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Controllers
{
    public class QuestionController : BaseController
    {
        #region Ctor

        private IQuestionService _questionService;
        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        #endregion

        #region Create Question

        [Authorize]
        [HttpGet("create-question")]
        public async Task<IActionResult> CreateQuestion()
        {
            return View();
        }

        #endregion

        #region GetTags
        [HttpGet("get-tags")]
        public async Task<IActionResult> GetTagsForSuggest(string name)
        {
            var tags = await _questionService.GetAllTags();
            var filteredTags = tags.Where(s => s.Title.Contains(name)).Select(s => s.Title).ToList();
            return Json(filteredTags);
        }
        #endregion
    }
}
