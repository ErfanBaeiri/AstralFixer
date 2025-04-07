using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Tags;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IQuestionService
    {
        #region Tags

        Task<List<Tag>> GetAllTags();

        #endregion
    }
}
