using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        #region Ctor QuestionRepository
        private IQuestionRepository _questionRepository;
        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        #endregion

        #region Tags
        public async Task<List<Tag>> GetAllTags()
        {
            return await _questionRepository.GetTags();
        }
        #endregion
    }
}
