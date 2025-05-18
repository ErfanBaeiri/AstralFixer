using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Controllers;

public class HomeController : BaseController
{

    #region Dependency Injection
    private readonly IQuestionService _questionService;
    public HomeController(IQuestionService questionService)
    {
        _questionService = questionService;
    }
    #endregion

    public async Task<IActionResult> Index()
    {
        var options = new FilterQuestionViewModel
        {
            TakeEntityToShow = 10,
            Sort = FilterQuestionEnum.NewToOld
        };

        ViewData["Questions"] = await _questionService.FilterQuestionAsync(options);

        return View();
    }

    #region CkEditor Upload Image
    public async Task<IActionResult> UploadEditorImage(IFormFile upload)
    {
        var fileName = Guid.NewGuid() + Path.GetFileName(upload.FileName);

        upload.UploadFile(fileName, PathTools.CkEditorImageFullPath);

        return Json(new { url = $"{PathTools.CkEditorImageFullPath}/{fileName}" });
    }
    #endregion

}
