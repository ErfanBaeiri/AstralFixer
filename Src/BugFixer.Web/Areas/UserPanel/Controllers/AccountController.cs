using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.UserPanel.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    public class AccountController : UserPanelBaseController
    {
        #region Ctor
        private readonly IUserService _userService;
        private readonly IStateService _stateService;
        public AccountController(IUserService userService, IStateService stateService)
        {
            _userService = userService;
            _stateService = stateService;
        }
        #endregion

        #region Edit User Info
        [HttpGet]
        public async Task<IActionResult> EditInfo()
        {
            ViewData["States"] = await _stateService.GetAllStates();

            var result = await _userService.FillEditUserViewModel(HttpContext.User.GetUserId());

            if (result.CountryId.HasValue)
            {
                ViewData["Cities"] = await _stateService.GetAllStates(result.CountryId.Value);
            }

            return View(result);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfo(EditUserViewModel editViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.EditUserInfoAsync(editViewModel, HttpContext.User.GetUserId());

                switch (result)
                {
                    case EditUserInfoResult.Success:
                        TempData[SuccessMessage] = "عملیات با موفقیت انجام شد";
                        return RedirectToAction("EditInfo", "Account", new { area = "UserPanel" });
                    case EditUserInfoResult.NotValidDate:
                        TempData[ErrorMessage] = "تاریخ وارد شده معتبر نمی باشد ";
                        break;
                }
            }

            ViewData["States"] = await _stateService.GetAllStates();

            if (editViewModel.CountryId.HasValue)
            {
                ViewData["Cities"] = await _stateService.GetAllStates(editViewModel.CountryId.Value);
            }

            return View(editViewModel);
        }
        #endregion

        #region Load Cities
        public async Task<IActionResult> LoadCities(long countryId)
        {
            return new JsonResult(await _stateService.GetAllStates(countryId));
        }
        #endregion

        #region Change User Password
        [HttpGet]
        public async Task<IActionResult> ChangeUserPassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordViewModel changeUserPassword)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangeUserPasswordAsync(changeUserPassword, HttpContext.User.GetUserId());

                switch (result)
                {
                    case ChangeUserPasswordResult.Success:
                        TempData[SuccessMessage] = "عملیات  با موفقیت انجام شد";
                        await HttpContext.SignOutAsync();
                        return RedirectToAction("login", "Account", new { area = "" });
                    case ChangeUserPasswordResult.CurrntPasswordNotValid:
                        ModelState.AddModelError("CurrentPassword", "کلمه عبور وارد شده اشتباه است");
                        break;
                }

            }

            return View(changeUserPassword);
        }
        #endregion
    }
}
