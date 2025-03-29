using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Tags
{
    public class RequestTag : BaseEntity
    {
        #region Properties

        [Display(Name = "عنوان")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        public long UserId { get; set; }

        #endregion

        #region Relations
        public User User { get; set; }
        #endregion

    }
}
