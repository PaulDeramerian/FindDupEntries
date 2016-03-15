using DuplicateAdvisorMatch.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch
{
    class Program
    {
        static void Main(string[] args)
        {

            //Console.Write("Please select txt file:");
            //var filePath = Console.ReadLine();
            var filePath = "c:\\temp\\advertisers.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Please select valid file");
                Console.ReadLine();
                return;
            }

            var outputFilePath = "c:\\temp\\";
            var manager = new AdvertisorManager(outputFilePath);
            manager.StartProcess(File.ReadAllLines(filePath), 0.75);

            //manager.PrintPossibleDuplicate( 0.75);

            Console.WriteLine("Press anykey.");
            Console.ReadLine();
        }
    }
}
