using System;
using System.IO;

namespace GoogleHosts
{
    public class Log
    {
        private const string LogFilePath = "C:\\GoogleHostServiceLog.txt";

        public void Write(string format, params object[] arg)
        {
            using (StreamWriter sw = new StreamWriter(LogFilePath, true))
            {
                sw.WriteLine(format + "-Time:" + DateTime.Now, arg);
            }
        }
    }
}