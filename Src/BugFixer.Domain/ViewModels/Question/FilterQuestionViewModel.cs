using BugFixer.Domain.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Question
{
    public class FilterQuestionViewModel : Paging<QuestionListViewModel>
    {

        public string? Title { get; set; }

        public FilterQuestionEnum Sort { get; set; }

    }

    public enum FilterQuestionEnum
    {
        [Display(Name = "تاریخ ثبت نزولی")] NewToOld,
        [Display(Name = "تاریخ ثبت صعودی")] OldToNew,
        [Display(Name = "امتیاز نزولی")] ScoreHighToLow,
        [Display(Name = "امتیاز صعودی")] ScoreLowToHigh,
    }
}
