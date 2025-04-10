﻿using BugFixer.Application.Generators;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using BugFixer.Domain.ViewModels.UserPanel.Account;

namespace BugFixer.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        #region Ctor

        private readonly IUserRepository _userRepository;
        private IEmailService _emailService;

        public UserService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        #endregion

        #region Register

        public async Task<RegisterResult> RegisterUser(RegisterViewModel register)
        {
            // Check Email Exists
            if (await _userRepository.IsExistsUserByEmail(register.Email.SanitizeText().Trim().ToLower()))
            {
                return RegisterResult.EmailExists;
            }

            // hash password
            var password = PasswordHelper.EncodePasswordMd5(register.Password.SanitizeText());

            // create user
            var user = new User
            {
                Avatar = PathTools.DefaultUserAvatar,
                Email = register.Email.SanitizeText().Trim().ToLower(),
                Password = password,
                EmailActivationCode = CodeGenerator.CreateActivationCode()
            };

            // Add To database
            await _userRepository.CreateUser(user);
            await _userRepository.Save();

            #region Send Activation Email

            var body = $@"
                <div> برای فعالسازی حساب کاربری خود روی لینک زیر کلیک کنید . </div>
                <a href='{PathTools.SiteAddress}/Activate-Email/{user.EmailActivationCode}'>فعالسازی حساب کاربری</a>
                ";

            await _emailService.SendEmail(user.Email, "فعالسازی حساب کاربری", body);

            #endregion

            return RegisterResult.Success;
        }

        #endregion

        #region Login

        public async Task<LoginResult> CheckUserForLogin(LoginViewModel login)
        {
            var user = await _userRepository.GetUserByEmail(login.Email.Trim().ToLower().SanitizeText());

            if (user == null) return LoginResult.UserNotFound;

            var hashedPassword = PasswordHelper.EncodePasswordMd5(login.Password.SanitizeText());

            if (hashedPassword != user.Password)
            {
                return LoginResult.UserNotFound;
            }

            if (user.IsDelete) return LoginResult.UserNotFound;

            if (user.IsBan) return LoginResult.UserIsBan;

            if (!user.IsEmailConfirmed) return LoginResult.EmailNotActivated;

            return LoginResult.Success;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        #endregion

        #region Activation Email

        public async Task<bool> ActivateUserEmail(string activationCode)
        {
            var user = await _userRepository.GetUserByActivationCode(activationCode);

            if (user == null) return false;

            if (user.IsBan || user.IsDelete) return false;

            user.IsEmailConfirmed = true;
            user.EmailActivationCode = CodeGenerator.CreateActivationCode();

            await _userRepository.UpdateUser(user);
            await _userRepository.Save();

            return true;
        }

        #endregion

        #region Forgot Password

        public async Task<ForgotPasswordResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            var email = forgotPassword.Email.SanitizeText().Trim().ToLower();

            var user = await _userRepository.GetUserByEmail(email);

            if (user == null || user.IsDelete) return ForgotPasswordResult.UserNotFound;

            if (user.IsBan) return ForgotPasswordResult.UserBan;

            #region Send Activation Email

            var body = $@"
                <div> برای فراموشی کلمه عبور روی لینک زیر کلیک کنید . </div>
                <a href='{PathTools.SiteAddress}/Reset-Password/{user.EmailActivationCode}'>فراموشی کلمه عبور</a>
                ";

            await _emailService.SendEmail(user.Email, "فراموشی کلمه عبور", body);

            #endregion

            return ForgotPasswordResult.Success;
        }

        #endregion

        #region Reset Password

        public async Task<ResetPasswordResult> ResetPassword(ResetPasswordViewModel reset)
        {
            var user = await _userRepository.GetUserByActivationCode(reset.EmailActivationCode.SanitizeText());

            if (user == null || user.IsDelete) return ResetPasswordResult.UserNotFound;

            if (user.IsBan) return ResetPasswordResult.UserIsBan;

            var password = PasswordHelper.EncodePasswordMd5(reset.Password.SanitizeText());

            user.Password = password;
            user.IsEmailConfirmed = true;
            user.EmailActivationCode = CodeGenerator.CreateActivationCode();

            await _userRepository.UpdateUser(user);
            await _userRepository.Save();

            return ResetPasswordResult.Success;
        }

        public async Task<User> GetUserByActivationCode(string activationCode)
        {
            return await _userRepository.GetUserByActivationCode(activationCode.SanitizeText());
        }

        #endregion

        #region User Panel

        public async Task<User?> GetUserById(long userId)
        {
            return await _userRepository.GetUserById(userId);
        }

        public async Task ChangeUserAvatar(long userId, string fileName)
        {
            var user = await GetUserById(userId);

            user.Avatar = fileName;

            await _userRepository.UpdateUser(user);
            await _userRepository.Save();
        }

        public async Task<EditUserViewModel> FillEditUserViewModel(long userId)
        {
            var user = await GetUserById(userId);

            var result = new EditUserViewModel
            {
                BirthDate = user.BirthDate != null ? user.BirthDate.Value.ToShamsi() : string.Empty,
                CityId = user.CityId,
                CountryId = user.CountryId,
                Description = user.Description,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                GetNewsLetter = user.GetNewsLetter,
                PhoneNumber = user.PhoneNumber
            };

            return result;
        }

        public async Task<EditUserInfoResult> EditUserInfo(EditUserViewModel editUserViewModel, long userId)
        {
            var user = await GetUserById(userId);

            if (!string.IsNullOrEmpty(editUserViewModel.BirthDate))
            {
                try
                {
                    var date = editUserViewModel.BirthDate.SanitizeText().ToMiladi();

                    user.BirthDate = date;
                }
                catch (Exception exception)
                {
                    return EditUserInfoResult.NotValidDate;
                }
            }

            user.FirstName = editUserViewModel.FirstName.SanitizeText();
            user.LastName = editUserViewModel.LastName.SanitizeText();
            user.Description = editUserViewModel.Description.SanitizeText();
            user.PhoneNumber = editUserViewModel.PhoneNumber.SanitizeText();
            user.GetNewsLetter = editUserViewModel.GetNewsLetter;
            user.CountryId = editUserViewModel.CountryId;
            user.CityId = editUserViewModel.CityId;

            await _userRepository.UpdateUser(user);
            await _userRepository.Save();

            return EditUserInfoResult.Success;
        }

        public async Task<ChangeUserPasswordResult> ChangeUserPassword(long userId, ChangeUserPasswordViewModel changeUserPassword)
        {
            var user = await GetUserById(userId);

            var password = PasswordHelper.EncodePasswordMd5(changeUserPassword.OldPassword.SanitizeText());

            if (password != user.Password)
            {
                return ChangeUserPasswordResult.OldPasswordIsNotValid;
            }

            user.Password = PasswordHelper.EncodePasswordMd5(changeUserPassword.Password.SanitizeText());

            await _userRepository.UpdateUser(user);
            await _userRepository.Save();

            return ChangeUserPasswordResult.Success;
        }

        #endregion
    }
}
