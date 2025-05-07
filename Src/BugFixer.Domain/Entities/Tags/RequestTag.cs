using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Tags
{
    public class RequestTag : BaseEntity
    {
        #region Propertise

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Title { get; set; }

        public long UserId { get; set; }

        #endregion

        #region Relation
        public User User { get; set; }
        #endregion
    }
}
