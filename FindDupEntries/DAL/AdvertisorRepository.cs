using DuplicateAdvisorMatch.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch.DAL
{
    public class AdvertisorRepository
    {
        protected IUnitOfWork _uow = null;
        public AdvertisorRepository(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void SaveEntries<T>(IRepository<T> repo, IEnumerable<T> entries) where T : class
        {
            repo.AddRange(entries);
        }

        public List<T> ReadEntryWordFromDB<T>(IRepository<T> repo, Guid id) where T : class, IAdvertiserNameWord
        {
            return repo.FindBy(x => x.NameSourceId == id).ToList();
        }

        public void UpdateAdvertisorWordCount()
        {
            string query = @"update [dbo].[AdvertiserNames]
set WordCount = w.Wcount
from[dbo].[AdvertiserNames] ad
   join (
       select NameSourceId, count(NameSourceId)Wcount
       from[dbo].[AdvertiserNameWords]
       group by NameSourceId
	) w
    on w.NameSourceId = ad.Id
where ad.WordCount = 0
";
            var generic = new GenericRepository(_uow);
            generic.ExecuteCommand(query);
        }

        public List<string> ReadWords()
        {
            var _repoEntry = new GenericRepository(_uow);
            var query = @"SELECT word FROM [dbo].[AdvertiserNameWords] group by word
                  having count(1) > 1
                  order by count(1) desc";
            return _repoEntry.ExecuteCommand<string>(query).ToList();
        }

        public List<AdvertiserName> GetEntriesByWords(string word)
        {
            var _repoEntry = new GenericRepository(_uow);
            var query = string.Format(@"SELECT n.Id, n.CompareWithName, w.SourceWordCount WordCount, n.OriginName
    FROM  [AdvertiserNameWords] w
	    join AdvertiserNames n
	    on w.NameSourceId = n.Id
    where Word ='{0}' 
    order by CompareWithName", word);

            return _repoEntry.ExecuteCommand<AdvertiserName>(query).ToList();
        }

        public void RemoveAllMapping()
        {
            var _repo = new GenericRepository(_uow);
            _repo.ExecuteCommand("truncate table EntryWordMappers;");
        }

        public List<AdvertisorMatcher> GetAdvertisorMatchs(double percentageMatch)
        {
            var query = string.Format(@"select OriginName, MatchAdvertisor, [WordMatchCount], PercentMatch
                    from(
                        SELECT ad.Id, ad.OriginName, ad.WordCount,
                            mad.Id MatchId, mad.OriginName MatchAdvertisor, mad.WordCount MatchmadWordCount,  [WordMatchCount],
		                    case when ad.WordCount >= mad.WordCount then
                                [WordMatchCount] / cast(ad.WordCount as decimal(18, 7))
		                    else 0 end PercentMatch
                          FROM[dbo].[EntryWordMappers]  m with(nolock)
                           join[dbo].[AdvertiserNames]  ad with(nolock)
                           on ad.id = m.EntryId
                           join[dbo].[AdvertiserNames] mad with(nolock)
                           on mad.id = m.MatchEntryId
                    ) m
                    where PercentMatch > {0}
                    order by PercentMatch desc", percentageMatch);

            var repo = new GenericRepository(_uow);
            return repo.ExecuteCommand<AdvertisorMatcher>(query).ToList();

        }
    }
}
