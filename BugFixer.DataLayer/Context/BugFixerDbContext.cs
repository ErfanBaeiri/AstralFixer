using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Context
{
    public class BugFixerDbContext : DbContext
    {
        #region Ctor

        public BugFixerDbContext(DbContextOptions<BugFixerDbContext> options) : base(options)
        {
            
        }

        #endregion

        #region DbSet

        public DbSet<User> Users { get; set; }

        #endregion
    }
}
