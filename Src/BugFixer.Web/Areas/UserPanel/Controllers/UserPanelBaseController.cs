using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class UserPanelBaseController : Controller
    {
        public static string SuccessMessage = "SuccessMessage";
        public static string WarningMessage = "WarningMessage";
        public static string InfoMessage = "InfoMessage";
        public static string ErrorMessage = "ErrorMessage";
    }
}
