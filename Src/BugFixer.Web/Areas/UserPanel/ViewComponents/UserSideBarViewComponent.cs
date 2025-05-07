using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.ViewComponents
{
    public class UserSideBarViewComponent : ViewComponent
    {
        // This code defines a ViewComponent for the User Panel area of a web application.
        #region Ctor
        private readonly IUserService _userService;
        public UserSideBarViewComponent(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        #endregion
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieves the user ID from the current HTTP context and fetches the user details asynchronously.
            var user = await _userService.GetUserByIdAsync(HttpContext.User.GetUserId());

            // This method is called to render the UserSideBar view component.
            // It returns the "UserSideBar" view, which is expected to be located in the Views/Shared/Components folder.
            return View("UserSideBar", user);
        }


        //public IViewComponentResult Invoke()
        //{
        //    // This method is called to render the UserSideBar view component.
        //    // It returns the "UserSideBar" view, which is expected to be located in the Views/Shared/Components folder.
        //    return View("UserSideBar");
        //}


        //public async Task<IViewComponentResult> Invoke()
        //{
        //    return View("UserSideBar");
        //}
    }
}
