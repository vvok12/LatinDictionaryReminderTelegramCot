using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace DictionaryTgBot
{
    class ReminderContext : DbContext
    {
        public DbSet<Remind> Reminds { get; set; }

        public ReminderContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("server=localhost;user=root;database=reminds;");
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Reminds;Trusted_Connection=True;");
        }
    }
}
