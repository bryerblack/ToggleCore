using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToggleCoreLibrary.Models;

namespace ToggleCoreLibrary.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<FeatureToggleModel> featureToggleModels { get; set; }
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeatureToggleModel>();
        }
    }
}
