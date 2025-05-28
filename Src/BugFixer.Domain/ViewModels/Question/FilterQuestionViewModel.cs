using BugFixer.Domain.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Question
{
    public class FilterQuestionViewModel : Paging<QuestionListViewModel>
    {
        public FilterQuestionViewModel()
        {
            Sort = FilterQuestionEnum.NewToOld;
            CheckedStatus = FilterQuestionCheckedStatus.All;
        }

        public string? Title { get; set; }

        public string? TagTitle { get; set; }

        public FilterQuestionEnum Sort { get; set; }

        public FilterQuestionCheckedStatus CheckedStatus { get; set; }

    }

    public enum FilterQuestionEnum
    {
        [Display(Name = "تاریخ ثبت نزولی")] NewToOld,
        [Display(Name = "تاریخ ثبت صعودی")] OldToNew,
        [Display(Name = "امتیاز نزولی")] ScoreHighToLow,
        [Display(Name = "امتیاز صعودی")] ScoreLowToHigh,
    }
    public enum FilterQuestionCheckedStatus
    {
        [Display(Name = "همه")] All,
        [Display(Name = "بررسی شده")] IsChecked,
        [Display(Name = "بررسی نشده")] NotChecked,
    }
}
