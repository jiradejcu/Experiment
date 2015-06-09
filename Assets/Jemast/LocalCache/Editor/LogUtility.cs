using System;
using System.IO;

namespace Jemast.LocalCache
{
    public static class LogUtility
    {
        private static readonly object LogLock = new object();

        public static void LogImmediate(string message)
        {
            if (Preferences.EnableLogFile == false)
                return;

            lock (LogLock)
            {
                if (!Directory.Exists(Preferences.CachePath))
                    Directory.CreateDirectory(Preferences.CachePath);

                using (StreamWriter w = File.AppendText(Preferences.CachePath + "log.txt"))
                {
                    w.WriteLine(string.Format("[{0}] {1}", DateTime.Now, message), w);
                }
            }
        }

        public static void LogImmediate(string message, params object[] args)
        {
            if (Preferences.EnableLogFile == false)
                return;

            lock (LogLock)
            {
                if (!Directory.Exists(Preferences.CachePath))
                    Directory.CreateDirectory(Preferences.CachePath);

                using (StreamWriter w = File.AppendText(Preferences.CachePath + "log.txt"))
                {
                    w.WriteLine(string.Format("[{0}] {1}", DateTime.Now, string.Format(message, args)), w);
                }
            }
        }
    }
}