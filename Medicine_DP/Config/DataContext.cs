using System;
using System.Collections.Generic;
using System.Text;
using Medicine_DP.Models;
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Config
{
    public class DataContext : DbContext
    {
        public DbSet<appointments> appointments {  get; set; }
        public DbSet<employees> employees { get; set; }
        public DbSet<medical_records> medical_records { get; set; }
        public DbSet<medical_tests> medical_tests { get; set; }
        public DbSet<medications> medications { get; set; }
        public DbSet<patients> patients { get; set; }
        public DbSet<payments> payments { get; set; }
        public DbSet<rooms> rooms { get; set; }
        public DbSet<services> services { get; set; }
        public DbSet<schedules> schedules { get; set; }
        
        public DataContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Connect.Config.connection, Connect.Config.version);
        }
    }
}
