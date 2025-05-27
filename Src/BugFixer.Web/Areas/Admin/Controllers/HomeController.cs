using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Admin.Tag;
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

        #region Dashbord
        public async Task<IActionResult> Dashboard()
        {

            ViewData["ChartDataJson"] = JsonConvert.SerializeObject(await _questionService.GetTagViewModelJson());

            return View();
        }
        #endregion

        #region Filter Tags
        public async Task<IActionResult> LoadFilterTagsPartial(FilterTagAdminViewModel filter)
        {

            var result = await _questionService.FilterTagAdmin(filter);

            return PartialView("_FilterTagsPartial", result);
        }
        #endregion

        #region Create Tags
        public IActionResult LoadCreateTagPartial()
        {

            return PartialView("_CreateTagPartial");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag(CreateTagAdminViewModel create)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { status = "error", message = "مقادیر ورودی معتبر نمی باشد" });


            await _questionService.CreateTagAdmin(create);
            return new JsonResult(new { staus = "success", message = "عملیات با موفقیت انجام شد" });
        }
        #endregion

        #region Edit Tags

        public async Task<IActionResult> LoadEditTagPartial(long id)
        {
            var result = await _questionService.FillEditTagAdminViewModel(id);

            if (result == null)
                return PartialView("_NotFoundDataPartial");

            return PartialView("_EditTagPartial", result);
        }

        [HttpPost]
        public async Task<IActionResult> EditTag(EditTagAdminViewModel edit)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { status = "error", message = "مقادیر ورودی معتبر نمی باشد" });

            var result = await _questionService.EditTagAdmin(edit);

            if (!result)
                return new JsonResult(new { status = "error", message = "مقادیر ورودی معتبر نمی باشد" });

            return new JsonResult(new { staus = "success", message = "عملیات با موفقیت انجام شد" });
        }
        #endregion

        #region Delete Tag
        [HttpPost]
        public async Task<IActionResult> DeleteTag(long id)
        {
            var result = await _questionService.DeleteTagAdmin(id);

            if (!result)
                return new JsonResult(new { status = "error", message = "مقادیر ورودی معتبر نمی باشد" });

            return new JsonResult(new { staus = "success", message = "عملیات با موفقیت انجام شد" });
        }
        #endregion

    }
}
