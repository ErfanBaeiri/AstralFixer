using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Enums
{
    public enum QuestionScoreType
    {
        [Display(Name = "مثبت")] plus,
        [Display(Name = "منفی")] Minus
    }
}
