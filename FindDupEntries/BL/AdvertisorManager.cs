using DuplicateAdvisorMatch.DAL;
using DuplicateAdvisorMatch.DAL.Model;
using DuplicateAdvisorMatch.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace DuplicateAdvisorMatch.BL
{
    public class AdvertisorManager
    {
        protected IUnitOfWork _uow = null;
        private dcLogger<AdvertisorManager> logger = new dcLogger<AdvertisorManager>();
        private Dictionary<Guid, List<AdvertiserNameWord>> _wordEntries;
        private readonly AdvertisorRepository _AdRepo;
        private readonly string _outputFilePath;

        public AdvertisorManager(string outputFilePath)
        {
            _uow = new UnitOfWork();
            _wordEntries = new Dictionary<Guid, List<AdvertiserNameWord>>();
            _AdRepo = new AdvertisorRepository(_uow);
            _outputFilePath = outputFilePath;

        }

        public void StartProcess(string[] names, double percentageMatch)
        {
            //save entries in the DB
            SaveAdvertisersName(names);
            StartAnalysingName(_AdRepo.ReadWords());
            PrintPossibleDuplicate(percentageMatch);
        }

        private bool SaveAdvertisersName(string[] names)
        {

            try
            {
                logger.Log("Check entries with database");
                var _repoEntry = new GenericRepository<AdvertiserName>(_uow);
                var _repoWordSplit = new GenericRepository<AdvertiserNameWord>(_uow);

                var entries = _repoEntry.GetAll();
                var newEntries = (from n in names
                                  join e in entries on n.Trim() equals e.OriginName into ej
                                  from e in ej.DefaultIfEmpty()
                                  where !string.IsNullOrEmpty(n) && e == null
                                  select new AdvertiserName()
                                  {
                                      Id = Guid.NewGuid(),
                                      OriginName = n.Trim(),
                                      CompareWithName = Helper.Helper.RemoveUnwantedCharecters(n.Trim()),
                                      WordCount = 0
                                  }).ToList();

                var pageCount = 1000;

                Stopwatch timer = new Stopwatch();
                if (newEntries.Count() != 0)
                {
                    timer.Start();
                    logger.Log(string.Format("{0} new advertisers found, inserting to DB ..", newEntries.Count()));
                }

                List<AdvertiserNameWord> wordList = new List<AdvertiserNameWord>();
                int i = 0;
                while (true)
                {
                    var entryPage = newEntries.Skip(i * pageCount).Take(pageCount);
                    _AdRepo.SaveEntries(_repoEntry, entryPage);

                    wordList.Clear();
                    for (var q = 0; q < entryPage.Count(); ++q)
                    {
                        var entry = entryPage.ElementAt(q);
                        var words = entry.CompareWithName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        wordList.AddRange((from w in words.Distinct()
                                           where !string.IsNullOrEmpty(w)
                                           select new AdvertiserNameWord()
                                           {
                                               Id = Guid.NewGuid(),
                                               NameSourceId = entry.Id,
                                               Word = w
                                           }));
                    }

                    _AdRepo.SaveEntries(_repoWordSplit, wordList);

                    //update word count in main table
                    _AdRepo.UpdateAdvertisorWordCount();

                    if (entryPage.Count() < 1000)
                    {
                        timer.Stop();
                        logger.Log(string.Format("End inserting to DB, time {0} sec", timer.ElapsedMilliseconds / 1000));
                        break;
                    }
                    ++i;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        private void StartAnalysingName(List<string> words)
        {
            //concurrentCalls should be read from config
            int concurrentCalls = 5;

            Task[] tasks = new Task[concurrentCalls];
            int asyncCallCount = 0;
            _AdRepo.RemoveAllMapping();

            Stopwatch timer = new Stopwatch();
            timer.Start();
            logger.Log("Start checking for words");

            for (var i = 0; i < words.Count; ++i)
            {
                logger.Log(string.Format("checking for word:{0} [{1}-{2}]", words[i], i, words.Count));
                tasks[asyncCallCount] = CheckDupAsync(words[i]);
                if (asyncCallCount == (concurrentCalls - 1))
                {
                    Task.WaitAll(tasks);
                    asyncCallCount = 0;
                }

                ++asyncCallCount;
            }

            timer.Stop();
            logger.Log(string.Format("complete with time:{0} sec", timer.ElapsedMilliseconds / 1000));

        }

        private Task CheckDupAsync(string word)
        {

            var entries = _AdRepo.GetEntriesByWords(word);
            var groupMmapper = new List<AdvertisorWordMapper>();
            var repo = new GenericRepository<AdvertiserNameWord>(_uow);

            foreach (var entry in entries)
            {
                if (!_wordEntries.ContainsKey(entry.Id))
                {
                    _wordEntries.Add(entry.Id, _AdRepo.ReadEntryWordFromDB(repo, entry.Id));
                }
                foreach (var groupWord in _wordEntries[entry.Id])
                {
                    //ignore check group word since we know it contains in the list
                    if (groupWord.Word == word)
                        continue;

                    foreach (var wordEntry in entries)
                    {
                        if (wordEntry.Id != groupWord.NameSourceId && wordEntry.CompareWithName.Contains(groupWord.Word))
                            groupMmapper.Add(new AdvertisorWordMapper() { EntryId = entry.Id, MatchEntryId = wordEntry.Id, MatchWord = groupWord.Word });
                    }
                }
            }

            var dupEntries = (from m in groupMmapper
                              group m by new { m.EntryId, m.MatchEntryId } into grp
                              where grp.Count() > 1
                              select new AdvertisorWordMapper
                              {
                                  Id = Guid.NewGuid(),
                                  EntryId = grp.Key.EntryId,
                                  MatchEntryId = grp.Key.MatchEntryId,
                                  WordMatchCount = grp.Count() + 1
                              }).ToList();

            var _repo = new GenericRepository<AdvertisorWordMapper>(_uow);
            _AdRepo.SaveEntries(_repo, dupEntries);

            return Task.FromResult<bool>(true);
        }

        public void PrintPossibleDuplicate(double percentageMatch)
        {
            var names = _AdRepo.GetAdvertisorMatchs(percentageMatch);
            if (names == null || names.Count == 0 || string.IsNullOrEmpty(_outputFilePath))
                return;

            if (!Directory.Exists(_outputFilePath))
                Directory.CreateDirectory(_outputFilePath);

            string filePath = Path.Combine(_outputFilePath, "AdvertisorDuplicateNames.csv");
            if (File.Exists(filePath))
                File.Delete(filePath);

            logger.Log("writing to the file: " + filePath);

            StringBuilder content = new StringBuilder();
            content.Append("Advertisor;MatchAdvertisor;WordMatchCount;PercentMatch" + Environment.NewLine);

            for (var i = 0; i < names.Count; ++i)
            {
                content.AppendFormat("{0},{1},{2},{3},{4}", names[i].OriginName.Replace(",", ""),
                    names[i].MatchAdvertisor.Replace(",", ""), names[i].WordMatchCount, names[i].PercentMatch.ToString("P"), Environment.NewLine);
            }

            File.AppendAllText(filePath, content.ToString());

        }
    }
}
