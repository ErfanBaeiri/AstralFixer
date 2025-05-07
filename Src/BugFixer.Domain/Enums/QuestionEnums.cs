using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Enums
{
    public enum QuestionScoreType
    {
        [Display(Name = "مثبت")] Plus,
        [Display(Name = "مثبت")] Minus
    }
}
