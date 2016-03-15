using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch.DAL.Model
{
    public interface IAdvertiserNameWord
    {
        Guid Id { get; set; }

        Guid NameSourceId { get; set; }
        string Word { get; set; }
        int SourceWordCount { get; set; }
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("AdvertiserNameWords")]

    public class AdvertiserNameWord : IAdvertiserNameWord
    {
        [Key]
        public Guid Id { get; set; }

        public Guid NameSourceId { get; set; }
        public string Word { get; set; }
        public int SourceWordCount { get; set; }


    }
}
