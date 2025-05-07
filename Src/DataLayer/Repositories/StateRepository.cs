using BugFixer.Domain.Entities.Location;
using BugFixer.Domain.Interfaces;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Repositories
{
    public class StateRepository : IStateRepository
    {
        #region DI To DBCONTEXT
        private readonly BugFixerDbContext _context;
        public StateRepository(BugFixerDbContext context)
        {
            _context = context;
        }
        #endregion
        public async Task<List<State>?> GetAllStates(long? stateId = null)
        {
            var states = _context.States.Where(x => x.IsDelete == false).AsQueryable();

            if (stateId != null)
            {
                states = states.Where(x => x.ParentId == stateId);
            }
            else
            {
                states = states.Where(x => x.ParentId == null);
            }

            return await states.ToListAsync();
        }
    }
}
