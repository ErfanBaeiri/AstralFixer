using BugFixer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BugFixer.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        #region Ctor
        public readonly IQuestionService _questionService;
        public HomeController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        #endregion

        public async Task<IActionResult> LoadFilterTagsPartial()
        {
            return PartialView("_FilterTagsPartial");
        }

        public async Task<IActionResult> Dashboard()
        {

            ViewData["ChartDataJson"] = JsonConvert.SerializeObject(await _questionService.GetTagViewModelJson());

            return View();
        }
    }
}
