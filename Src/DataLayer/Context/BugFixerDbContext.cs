using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Location;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.SiteSetting;
using BugFixer.Domain.Entities.Tags;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    public class BugFixerDbContext : DbContext
    {
        public BugFixerDbContext(DbContextOptions<BugFixerDbContext> options) : base(options)
        {

        }

        #region DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<EmailSetting> EmailSettings { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<UserQuestionBookMark> BookMarks { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionView> QuestionViews { get; set; }
        public DbSet<SelectQuestionTag> SelectQuestionTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RequestTag> RequestTags { get; set; }
        public DbSet<QuestionUserScore> QuestionUserScores { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relation in modelBuilder.Model.GetEntityTypes().SelectMany(s => s.GetForeignKeys()))
            {
                relation.DeleteBehavior = DeleteBehavior.Restrict;
            }


            //var date = DateTime.MinValue;
            //modelBuilder.Entity<EmailSetting>().HasData(new EmailSetting
            //{
            //    CreateDate = date,
            //    DisplayName = "BugFixer",
            //    EnableSSL = true,
            //    From = "****",
            //    Id = 1,
            //    IsDefault = true,
            //    Password = "****",
            //    Port = 587,
            //    SMTP = "smtp.gmail.com",
            //    IsDelete = false,
            //});
        }

    }
}
