using System.IO;
using System.Reflection;

namespace FileScaner
{
    public static class Constants
    {
        /// <summary>
        /// Directory where the log files are stored
        /// </summary>
        public static string LogDirectory
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs"); }
        }

        /// <summary>
        /// location of the file that contains a list of path names that were last loaded
        /// </summary>
        public static string SentFileLog
        {
            get { return Path.Combine(LogDirectory, "Sent.txt"); }
        }

        /// <summary>
        /// location of the file that contains a list of path names that were not loaded
        /// </summary>
        public static string SkipFileLog
        {
            get { return Path.Combine(LogDirectory, "DidNotSend.txt"); }
        }
    }
}
