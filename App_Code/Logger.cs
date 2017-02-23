using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Logging
{
    public class Logger
    {
        // If the invoice generation is changed to use threads for each invoice,
        // then this lock will be necessary to allow logging
        static object Lock = new object[0];

        public static void Log(string text)
        {
            lock (Lock)
            {
                // Get the log filepath
                //string logFile = ConfigurationManager.AppSettings["LogFile"];
                string logFile = "C:\\musicgroup\\apilog.txt";

                // Create file if it doesn't exist
                if (!File.Exists(logFile))
                {
                    using (StreamWriter stream = File.CreateText(logFile))
                    {
                        stream.WriteLine("Log created on " + DateTime.Now.ToString());
                    }

                }

                // Append to file making the file longer over time if it is not deleted.
                using (StreamWriter stream = File.AppendText(logFile))
                {
                    if (text.Equals("\n"))
                    {
                        // Add a new line without the prepended datetime
                        stream.WriteLine("");
                    }
                    else
                    {
                        // Log normally
                        stream.WriteLine(String.Format("{0}: {1}\n", DateTime.Now.ToString(/*"MMMM dd, yyyy"*/), text));
                    }
                }
            }
        }

        public static void LogError(string sSource, string sLog, string sEvent)
        {
            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
            }
            EventLog.WriteEntry(sSource, sEvent);
            EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Warning, 234);
        }

        static void Main() { }
    }
}
