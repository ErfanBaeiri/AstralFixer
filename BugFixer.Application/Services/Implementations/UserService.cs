using BugFixer.Application.Generators;
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

namespace BugFixer.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        #region Ctor

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            // todo send email

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
    }
}
