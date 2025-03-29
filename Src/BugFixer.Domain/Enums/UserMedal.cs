using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Enums
{
    public enum UserMedal
    {
        [Display(Name = "برنز")] Bronze,

        [Display(Name = "نقره")] Silver,

        [Display(Name = "طلا")] Gold
    }
}
