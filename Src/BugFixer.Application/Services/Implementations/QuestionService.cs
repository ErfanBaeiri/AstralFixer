using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Common;
using Microsoft.Extensions.Options;

namespace BugFixer.Application.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        #region Ctor

        private IQuestionRepository _questionRepository;
        private ScoreManagementViewModel _scoreManagement;

        public QuestionService(IQuestionRepository questionRepository, IOptions<ScoreManagementViewModel> scoreManagement)
        {
            _questionRepository = questionRepository;
            _scoreManagement = scoreManagement.Value;
        }

        #endregion

        #region Tags

        public async Task<List<Tag>> GetAllTags()
        {
            return await _questionRepository.GetAllTags();
        }

        #endregion
    }
}
