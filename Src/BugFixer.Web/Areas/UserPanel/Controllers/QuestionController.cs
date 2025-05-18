using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Domain.ViewModels.UserPanel.Question;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    public class QuestionController : UserPanelBaseController
    {
        #region DI
        private readonly IQuestionService _questionService;
        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        #endregion

        #region BookMarks
        [HttpGet]
        public async Task<IActionResult> QuestionBookMark(FilterQuestionBookMarksViewModel filter)
        {

            filter.UserId = User.GetUserId();

            var result = await _questionService.FilterQuestionBookMarks(filter);

            return View(result);
        }
        #endregion

    }
}
