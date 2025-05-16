using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
        public async Task<IQueryable<Tag>> GetAllTagsAsQueryableAsync()
        {
            return _context.Tags.Where(u => !u.IsDelete).AsQueryable();
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
        public async Task UpdateTagAsync(Tag tag)
        {
            _context.Update(tag);
        }
        public async Task<List<string>> GetTagListByQuestionIdAsync(long questionId)
        {
            return await _context.SelectQuestionTags
                 .Include(s => s.Tag)
                 .Where(u => u.QuestionId == questionId)
                 .Select(s => s.Tag.Title)
                 .ToListAsync();
        }
        public async Task RemoveTagAsync(Tag tag)
        {
            _context.Remove(tag);
        }
        public async Task RemoveSelectQuestionTagAsync(SelectQuestionTag selectQuestionTag)
        {
            _context.Remove(selectQuestionTag);
        }
        #endregion

        #region Question
        public async Task AddQuestionAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
        }
        public async Task<IQueryable<Question>> GetAllQuestions()
        {
            return _context.Questions.Where(u => !u.IsDelete).AsQueryable();
        }
        public async Task updateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
        }
        public async Task<bool> IsExistQuestionScoreByUserIdAsync(long userId, long questionId)
        {
            return await _context.QuestionUserScores.AnyAsync(s => s.UserId == userId && s.QuestionId == questionId);
        }
        public async Task AddScoreToQuestionByUser(QuestionUserScore questionUserScore)
        {
            await _context.QuestionUserScores.AddAsync(questionUserScore);
        }
        public async Task AddQuestionToBookMarkAsync(UserQuestionBookMark bookMark)
        {
            await _context.BookMarks.AddAsync(bookMark);
        }

        public async Task RemoveQuestionToBookMarkAsync(UserQuestionBookMark bookMark)
        {
            _context.Remove(bookMark);
        }

        public async Task<bool> IsExistsQuestionInUserBookMarks(long userId, long questionId)
        {
            return await _context.BookMarks.AnyAsync(s => s.UserId == userId && s.QuestionId == questionId);
        }

        public async Task<UserQuestionBookMark?> GetQuestionBookMarkByQuestionAndUserId(long userId, long questionId)
        {
            return await _context.BookMarks.FirstOrDefaultAsync(s => s.UserId == userId && s.QuestionId == questionId);
        }
        public void SaveChange()
        {
            _context.SaveChanges();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        #endregion

        #region Selecte Question Tags
        public async Task AddSelectQuestionTagsAsync(SelectQuestionTag selectQuestionTag)
        {
            await _context.SelectQuestionTags.AddAsync(selectQuestionTag);
        }

        public async Task<Question?> GetQuestionByIdAsync(long questionId)
        {
            return await _context.Questions
                .Include(s => s.Answers)
                .Include(s => s.User)
                .Include(s => s.SelectQuestionTags)
                .FirstOrDefaultAsync(s => s.IsDelete == false && s.Id == questionId);
        }

        #endregion

        #region Answer
        public async Task AddAnswerByUserAsync(Answer answer)
        {
            await _context.Answers.AddAsync(answer);
        }

        public async Task<List<Answer>> GetAllQuestionAnswerAsync(long questionId)
        {
            return await _context.Answers
                .Include(s => s.User)
                .Where(u => u.QuestionId == questionId && u.IsDelete == false)
                .OrderByDescending(s => s.CreateDate)
                .ToListAsync();
        }
        public async Task<Answer?> GetAnswerByIdAsync(long answerId)
        {
            return await _context.Answers.Include(s => s.Question).FirstOrDefaultAsync(s => s.Id == answerId && s.IsDelete == false);
        }
        public async Task UpdateAnswerAsync(Answer answer)
        {
            _context.Update(answer);
        }
        public async Task<bool> IsExistsUserScoreForAnswer(long userId, long answerId)
        {
            return await _context.AnswerUserScores.AnyAsync(s => s.UserId == userId && s.AnswerId == answerId);
        }
        public async Task AddAnswerUserScoreAsync(AnswerUserScore answerUserScore)
        {
            await _context.AnswerUserScores.AddAsync(answerUserScore);
        }
        #endregion

        #region View
        public async Task<bool> IsExistViewforQuestAsync(string userIP, long questionId)
        {
            return await _context.QuestionViews.AnyAsync(s => s.UserIP.Equals(userIP) && s.QuestionId == questionId);
        }

        public async Task AddViewForQuestionAsync(QuestionView questionView)
        {
            await _context.QuestionViews.AddAsync(questionView);
        }


        #endregion
    }
}
