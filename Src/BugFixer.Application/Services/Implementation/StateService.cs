using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.common;

namespace BugFixer.Application.Services.Implementation
{
    public class StateService : IStateService
    {
        #region Ctor
        private readonly IStateRepository _stateRepository;
        public StateService(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        #endregion
        public async Task<List<SelectListViewModel>?> GetAllStates(long? stateId = null)
        {
            var states = await _stateRepository.GetAllStates(stateId);

            return states?.Select(x => new SelectListViewModel
            {
                Id = x.Id,
                Title = x.Title,

            }).ToList();

        }

    }
}
