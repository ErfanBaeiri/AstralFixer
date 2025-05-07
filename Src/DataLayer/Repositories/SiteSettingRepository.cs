using BugFixer.Domain.Entities.SiteSetting;
using BugFixer.Domain.Interfaces;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Repositories
{
    public class SiteSettingRepository : ISiteSettingRepository
    {
        #region Ctor
        private readonly BugFixerDbContext _context;
        public SiteSettingRepository(BugFixerDbContext context)
        {
            _context = context;
        }
        #endregion
        public async Task<EmailSetting?> GetEmailDefaultSettingAsync()
        {
            return await _context.EmailSettings.FirstOrDefaultAsync(x => x.IsDefault && !x.IsDelete);
        }
    }
}
