namespace BugFixer.Domain.ViewModels.Question
{
    public class QuestionListViewModel
    {
        public long QuestionId { get; set; }

        public string Title { get; set; }

        public string UserQuestionName { get; set; }

        public string CreateDate { get; set; }

        public bool HasAnyAnswer { get; set; }
        public bool IsChecked { get; set; }

        public bool HasAnyTrueAnswer { get; set; }

        public int AnswerCount { get; set; }

        public int ViewCount { get; set; }

        public int Score { get; set; }

        public string? AnswerUserDispalyName { get; set; }

        public string? CreateDateAnswer { get; set; }

        public List<string> Tags { get; set; }

    }
}
