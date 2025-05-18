using BugFixer.Application.Extensions;
using BugFixer.Application.Generators;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Enums;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Account;
using BugFixer.Domain.ViewModels.Common;
using BugFixer.Domain.ViewModels.UserPanel.Account;
using Microsoft.Extensions.Options;

namespace BugFixer.Application.Services.Implementation
{
    public class UserService : IUserService
    {
        #region Dependency Injection
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ScoreManagementViewModel _scoreManagement;
        public UserService(IUserRepository userRepository, IEmailService emailService, IOptions<ScoreManagementViewModel> scoreManagement)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _scoreManagement = scoreManagement.Value;
        }


        #endregion

        #region Register
        public async Task<RegisterResult> RegisterUserAsync(RegisterViewModel register)
        {
            // Check if the email already exists
            if (await _userRepository.IsEmailExistByEmailAsync(register.Email.SanitizeText().ToLower().Trim()))
            {
                return RegisterResult.EmailExists;
            }

            // Hash the user password
            var hashedPassword = PasswordHelper.HashPassword(register.Password.SanitizeText());

            // Create a new user entity
            User user = new User
            {
                Email = register.Email.SanitizeText().ToLower().Trim(),
                Password = hashedPassword.SanitizeText(),
                Avatar = PathTools.DefaultUserAvatar,
                EmailActivationCode = CodeGenerator.CreateActivationCode(),
            };

            // Save the user to the database
            await _userRepository.CreateUserAsync(user);

            // Save changes to the database
            await _userRepository.SaveChangesAsync();

            // Send activation email to the user
            #region Send Activation Email
            //var body = $@"
            //    <div> برای فعالسازی حساب کاربری بر روی لینک زیر کلیک نمایید . </div>
            //    <a href='{PathTools.SiteAddress}/ActivationUserEmail/{user.EmailActivationCode}'>فعالسازی حساب کاربری</a>";

            //await _emailService.SendEmailAsync(user.Email, "BugFixer", body);
            #endregion
            return RegisterResult.Success;
        }
        #endregion

        #region Login
        public async Task<LoginResult> LoginUserAsync(LoginViewModel login)
        {
            var user = await _userRepository.GetUserByEmailAsync(login.Email.SanitizeText().ToLower().Trim());

            if (user == null) return LoginResult.UserNotFound;
            if (user.IsBan) return LoginResult.UserIsBan;
            if (!user.IsEmailConfirmed) return LoginResult.UserNotActive;
            if (user.IsDelete) return LoginResult.UserNotFound;
            // Copmare Hashed Input UserPassword with Hashed UserPassword in DB
            if (PasswordHelper.HashPassword(login.Password.SanitizeText()) != user.Password) return LoginResult.UserNotFound;

            return LoginResult.Success;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email.SanitizeText().ToLower().Trim());
        }

        #endregion

        #region Email Activation
        public async Task<bool> ActivateUserEmail(string ActivationCode)
        {
            var user = await _userRepository.GetUserByActivationCode(ActivationCode.SanitizeText());

            if (user == null) return false;
            if (user.IsBan || user.IsDelete) return false;

            user.IsEmailConfirmed = true;
            user.EmailActivationCode = CodeGenerator.CreateActivationCode();

            await _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }


        #endregion

        #region Forgot Passowrd
        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordViewModel forgotPassword)
        {
            var user = await _userRepository.GetUserByEmailAsync(forgotPassword.Email.SanitizeText().ToLower().Trim());

            if (user == null || user.IsDelete) return ForgotPasswordResult.UserNotFound;
            if (user.IsBan) return ForgotPasswordResult.UserIsBan;

            // Send Activation Code by EmailAddres
            #region Send Activation Email
            var body = $@"
                <div> برای بازیابی حساب کاربری بر روی لینک زیر کلیک نمایید . </div>
                <a href='{PathTools.SiteAddress}/Reset-Password/{user.EmailActivationCode}'>بازیابی حساب کاربری</a>";

            await _emailService.SendEmailAsync(user.Email, "BugFixer", body);
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

            var userNewPassword = PasswordHelper.HashPassword(reset.Password.SanitizeText());
            user.Password = userNewPassword;
            user.EmailActivationCode = CodeGenerator.CreateActivationCode();
            user.IsEmailConfirmed = true;
            await _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();

            return ResetPasswordResult.Success;

        }

        public Task<User?> GetUserByEmailActivationCodeAsync(string emailActivationCode)
        {
            return _userRepository.GetUserByActivationCode(emailActivationCode.SanitizeText());
        }


        #endregion

        #region User Panel
        public async Task<User?> GetUserByIdAsync(long id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }
        public async Task ChangeUserAvatarAsync(long userId, string avatarFileName)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            #region Delete User Avatar
            if (user.Avatar != PathTools.DefaultUserAvatar)
                user.Avatar.DeleteFile(PathTools.UserAvatarFullPath);
            #endregion


            user.Avatar = avatarFileName.SanitizeText();

            await _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<EditUserViewModel> FillEditUserViewModel(long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            var result = new EditUserViewModel
            {
                Birthdate = user.Birthdate != null ? user.Birthdate.Value.ToPersianDate() : null,
                CityId = user.CityId,
                CountryId = user.CountryId,
                Description = user.Description,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                GetNewsletter = user.GetNewsletter,
                PhoneNumber = user.PhoneNumber
            };

            return result;

        }

        public async Task<EditUserInfoResult> EditUserInfoAsync(EditUserViewModel editUserViewModel, long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);


            if (!string.IsNullOrEmpty(editUserViewModel.Birthdate))
            {
                try
                {
                    var date = editUserViewModel.Birthdate.ToMiladi();
                    user.Birthdate = date;
                }
                catch (Exception exception)
                {

                    return EditUserInfoResult.NotValidDate;
                }
            }

            user.FirstName = editUserViewModel.FirstName.SanitizeText();
            user.LastName = editUserViewModel.LastName.SanitizeText();
            user.PhoneNumber = editUserViewModel.PhoneNumber.SanitizeText();
            user.Description = editUserViewModel.Description.SanitizeText();
            user.CityId = editUserViewModel.CityId;
            user.CountryId = editUserViewModel.CountryId;
            user.GetNewsletter = editUserViewModel.GetNewsletter;

            await _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();

            return EditUserInfoResult.Success;
        }

        public async Task<ChangeUserPasswordResult> ChangeUserPasswordAsync(ChangeUserPasswordViewModel changeUserPassword, long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            var userCurrentPassword = PasswordHelper.HashPassword(changeUserPassword.CurrentPassword.SanitizeText());

            if (user.Password != userCurrentPassword)
            {
                return ChangeUserPasswordResult.CurrntPasswordNotValid;
            }

            user.Password = PasswordHelper.HashPassword(changeUserPassword.NewPassword.SanitizeText());

            await _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();

            return ChangeUserPasswordResult.Success;
        }

        #endregion


        #region User Question / User Score and Medal
        public async Task UpdateUserScoreAndMedalAsync(long userId, int score)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            user.Score += score;

            await _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();


            if (user.Score >= _scoreManagement.MinScoreForBronzeMedal && user.Score <= _scoreManagement.MinScoreForSilverMedal)
            {
                if (user.Medal != null && user.Medal == UserMedal.Bronze) return;

                user.Medal = UserMedal.Bronze;

                await _userRepository.UpdateUser(user);
                await _userRepository.SaveChangesAsync();
            }

            else if (user.Score >= _scoreManagement.MinScoreForSilverMedal && user.Score <= _scoreManagement.MinScoreForGoldMedal)
            {
                if (user.Medal != null && user.Medal == UserMedal.Silver) return;

                user.Medal = UserMedal.Silver;

                await _userRepository.UpdateUser(user);
                await _userRepository.SaveChangesAsync();
            }

            else if (user.Score >= _scoreManagement.MinScoreForGoldMedal)
            {
                if (user.Medal != null && user.Medal == UserMedal.Gold) return;

                user.Medal = UserMedal.Gold;

                await _userRepository.UpdateUser(user);
                await _userRepository.SaveChangesAsync();
            }

        }
        #endregion


    }
}
