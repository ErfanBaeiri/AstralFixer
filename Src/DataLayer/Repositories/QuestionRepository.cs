using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        #region DI To BugFixerDBContext
        private readonly BugFixerDbContext _context;
        public QuestionRepository(BugFixerDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Tags
        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _context.Tags.Where(u => !u.IsDelete).AsQueryable().ToListAsync();
        }

        public async Task<bool> IsExistsTagByNameAsync(string tag)
        {
            return await _context.Tags.AnyAsync(u => u.Title == tag && !u.IsDelete);
        }

        public async Task<bool> CheckUserRequestedForTag(long userId, string tag)
        {
            return await _context.RequestTags.AnyAsync(u => u.UserId == userId && u.Title == tag && u.IsDelete == false);
        }

        public async Task AddRequestTagAsync(RequestTag tag)
        {
            await _context.RequestTags.AddAsync(tag);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> RequestCountForTagAsync(string tag)
        {
            return await _context.RequestTags.CountAsync(u => u.Title == tag && u.IsDelete == false);
        }

        public async Task AddTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public Task<Tag?> GetTagByName(string tag)
        {
            return _context.Tags.FirstOrDefaultAsync(u => u.Title == tag && !u.IsDelete);
        }
        #endregion

        #region Question
        public async Task AddQuestionAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
        }
        #endregion

        #region Selecte Question Tags
        public async Task AddSelectQuestionTagsAsync(SelectQuestionTag selectQuestionTag)
        {
            await _context.SelectQuestionTags.AddAsync(selectQuestionTag);
        }
        #endregion  
    }
}
