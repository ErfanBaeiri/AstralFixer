using BugFixer.Application.Extensions;
using BugFixer.Application.Statics;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Controllers;

public class HomeController : BaseController
{

    public IActionResult Index()
    {
        return View();
    }

    #region CkEditor Upload Image
    public async Task<IActionResult> UploadEditorImage(IFormFile upload)
    {
        var fileName = Guid.NewGuid() + Path.GetFileName(upload.FileName);

        upload.UploadFile(fileName, PathTools.CkEditorImageFullPath);   

        return Json(new { url = $"{PathTools.UserAvatarFullPath}{fileName}" });
    }
    #endregion

}
