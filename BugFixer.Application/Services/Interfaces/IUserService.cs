using BugFixer.Domain.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IUserService
    {
        #region Regitser

        Task<RegisterResult> RegisterUser(RegisterViewModel register);

        #endregion

        #region Login

        Task<LoginResult> CheckUserForLogin(LoginViewModel login);

        Task<User> GetUserByEmail(string email);

        #endregion

        #region Email Activation

        Task<bool> ActivateUserEmail(string activationCode);

        #endregion
    }
}
