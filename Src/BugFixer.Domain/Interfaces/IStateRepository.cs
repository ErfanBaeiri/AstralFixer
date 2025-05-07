using BugFixer.Domain.Entities.Location;

namespace BugFixer.Domain.Interfaces
{
    public interface IStateRepository
    {
        Task<List<State>?> GetAllStates(long? stateId = null);
    }
}
