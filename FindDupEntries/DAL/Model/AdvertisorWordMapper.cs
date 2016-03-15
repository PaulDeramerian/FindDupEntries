using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch.DAL.Model
{
    [System.ComponentModel.DataAnnotations.Schema.Table("EntryWordMappers")]
    public class AdvertisorWordMapper
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EntryId { get; set; }
        public Guid MatchEntryId { get; set; }

        [NotMapped]
        public string MatchWord { get; set; }
        public int WordMatchCount { get; set; }

    }
}
