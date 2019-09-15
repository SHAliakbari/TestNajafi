using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Najafi_Test.Models
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> Options) : base(Options)
        {

        }

        private DbConnection _dbConnection;
        public DB(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_dbConnection != null)
                optionsBuilder.UseSqlServer(_dbConnection);

            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        #endregion

        public virtual DbSet<RegisterForm> RegisterForms { get; set; }
    }
}
