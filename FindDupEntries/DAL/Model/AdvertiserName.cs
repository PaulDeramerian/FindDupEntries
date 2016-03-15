using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch.DAL.Model
{

    public interface IBaseAdvertiserName
    {
        Guid Id { get; set; }
        string OriginName { get; set; }
        string CompareWithName { get; set; }

    }

    public interface IAdvertiserName : IBaseAdvertiserName
    {
        int WordCount { get; set; }
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("AdvertiserNames")]
    public class AdvertiserName : IAdvertiserName
    {
        [Key]
        public Guid Id { get; set; }
        public string OriginName { get; set; }

        public string CompareWithName { get; set; }

        public int WordCount { get; set; }
    }
}
