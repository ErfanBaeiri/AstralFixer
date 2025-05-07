using BugFixer.Domain.Entities.SiteSetting;

namespace BugFixer.Domain.Interfaces
{
    public interface ISiteSettingRepository
    {
        Task<EmailSetting?> GetEmailDefaultSettingAsync();
    }
}
