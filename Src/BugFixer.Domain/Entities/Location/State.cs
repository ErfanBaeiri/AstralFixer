using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugFixer.Domain.Entities.Location
{
    public class State : BaseEntity
    {
        #region Propertise

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} نمی تواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Title { get; set; }
        public long? ParentId { get; set; }
        #endregion

        #region Relations
        public State? Parent { get; set; }

        [InverseProperty("Country")]
        public ICollection<User>? UserCountries { get; set; }
        [InverseProperty("City")]
        public ICollection<User>? UserCities { get; set; }
        #endregion
    }
}
