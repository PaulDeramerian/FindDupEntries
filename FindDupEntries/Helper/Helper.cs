using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace DuplicateAdvisorMatch.Helper
{
    public static class Helper
    {
        private static string[] unwantedCharecter = new string[] { ".", ",", "\'", "&" };
        private static string[] unwantedWords = new string[] { "llc",  "company", "the", "a", "to", "of", "ltd", "for", "corporation", "Group", "and", "inc", "com" };

        public static string RemoveUnwantedCharecters(this string entry)
        {
            entry = entry.ToLower();
            foreach (var word in unwantedCharecter)
            {
                entry = entry.Replace(word, "");
            }

            foreach (var word in unwantedWords)
            {
                entry = entry.Replace(word + " ", "").Replace(" " + word, "");
            }
            return entry.Trim().ToLower();
        }
    }
}

