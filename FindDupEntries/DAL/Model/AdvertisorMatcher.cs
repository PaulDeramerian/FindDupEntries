using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch.DAL.Model
{
    public class AdvertisorMatcher
    {
        public string OriginName { get; set; }
        public string MatchAdvertisor { get; set; }
        public int WordMatchCount { get; set; }
        public decimal PercentMatch { get; set; }
    }
}
