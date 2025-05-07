using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.ViewModels.Account;
using BugFixer.Domain.ViewModels.UserPanel.Account;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResult> RegisterUserAsync(RegisterViewModel register);

        Task<LoginResult> LoginUserAsync(LoginViewModel login);

        Task<User?> GetUserByEmail(string email);

        #region Email Activation
        Task<bool> ActivateUserEmail(string ActivationCode);
        #endregion

        #region Forgot Password
        Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordViewModel forgotPassword);
        #endregion

        #region Reset Password
        Task<ResetPasswordResult> ResetPassword(ResetPasswordViewModel reset);

        Task<User?> GetUserByEmailActivationCodeAsync(string emailActivationCode);
        #endregion

        #region User Panel
        Task<User?> GetUserByIdAsync(long id);

        Task ChangeUserAvatarAsync(long userId, string avatarFileName);

        Task<EditUserViewModel> FillEditUserViewModel(long userId);

        Task<EditUserInfoResult> EditUserInfoAsync(EditUserViewModel editUserViewModel, long userId);

        Task<ChangeUserPasswordResult> ChangeUserPasswordAsync(ChangeUserPasswordViewModel changeUserPassword, long userId);
        #endregion

        #region User Question / User Score and Medal
        Task UpdateUserScoreAndMedalAsync(long userId, int score);
        #endregion
    }
}
