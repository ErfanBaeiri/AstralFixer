using BugFixer.Domain.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IUserService
    {
        #region Regitser

        Task<RegisterResult> RegisterUser(RegisterViewModel register);

        #endregion
    }
}
