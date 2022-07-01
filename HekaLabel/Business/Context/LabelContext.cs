using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaLabel.Business.Context
{
    public class LabelContext : DbContext
    {
        public LabelContext()
            : base("name=LabelContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<PrintHistory> PrintHistory { get; set; }
        public virtual DbSet<RevisionChangeLog> RevisionChangeLog { get; set; }
    }
}
