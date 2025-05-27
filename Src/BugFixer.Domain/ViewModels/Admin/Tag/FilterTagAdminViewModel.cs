using BugFixer.Domain.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Admin.Tag
{
    public class FilterTagAdminViewModel : Paging<Domain.Entities.Tags.Tag>
    {
        public FilterTagAdminViewModel()
        {
            Status = FilterTagAdminStatus.All;
        }

        public string? Title { get; set; }

        public FilterTagAdminStatus Status { get; set; }

    }

    public enum FilterTagAdminStatus
    {
        [Display(Name = "همه")] All,
        [Display(Name = "دارای توضیحات")] HasDescription,
        [Display(Name = "بدون توضیحات")] NoDescription
    }
}
