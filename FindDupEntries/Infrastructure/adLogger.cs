using System;

namespace DuplicateAdvisorMatch.Infrastructure
{
    public class dcLogger<T> where T : class
    {

        public void Log(object message)
        {
            Console.WriteLine(message);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine(ex.InnerException.Message);
        }
    }
}
