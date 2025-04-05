using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.DataLayer.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        #region Ctor
        private BugFixerDbContext _context;
        public QuestionRepository(BugFixerDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Tags
        public async Task<List<Tag>> GetTags()
        {
            return await _context.Tags.Where(x => !x.IsDelete).ToListAsync();
        }
        #endregion



    }
}
