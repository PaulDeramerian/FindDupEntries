using DuplicateAdvisorMatch.DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch.DAL
{
    public class adContext : DbContext
    {
        public adContext()
        {
        }
        public adContext(string connectionstring)
            : base(connectionstring)
        {
            Database.SetInitializer<adContext>(null);
            
        }

        public virtual DbSet<AdvertiserName> AdvertiserNames { get; set; }
        public virtual DbSet<AdvertiserNameWord> AdvertiserNameWords { get; set; }
        public virtual DbSet<AdvertisorWordMapper> EntryWordMappers { get; set; }


    }
}
