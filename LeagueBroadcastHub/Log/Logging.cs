using System;
using System.IO;
using System.Text;

namespace LeagueBroadcastHub.Log
{
    public class Logging
    {
        public static Logging Instance;
        private StringBuilder Sb;
        private System.Timers.Timer LogTimer;

        private string LogDir;
        private FileInfo LogFile;

        public LogLevel Level { private set; get; }

        public Logging(LogLevel level)
        {
            if (Instance != null)
                return;
            Instance = this;

            this.Level = level;

            Sb = new StringBuilder();
            LogDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            Directory.CreateDirectory(LogDir);
            LogFile = new FileInfo(Path.Combine(LogDir, $"Log-{DateTime.Now:yyyy-MM-dd-HH-mm}.log"));

            if (!LogFile.Exists)
                LogFile.Create();

            LogTimer = new System.Timers.Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            //Update log every couple seconds to save performance
            LogTimer.Elapsed += WriteToFile;

            //Update log on exit/crash
            AppDomain.CurrentDomain.ProcessExit += WriteToFile;
        }

        public static void SetLogLevel(LogLevel level)
        {
            Instance.Level = level;
        }

        public static void Write(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Instance.Sb.Append($"[{DateTime.Now:HH-mm-ss}] {message}");
            Instance.Sb.AppendLine();
        }

        public static void Info(string message)
        {
            if (Instance.Level >= LogLevel.Info)
            {
                Write(message);
            }

        }

        public static void Warn(string message)
        {
            if (Instance.Level >= LogLevel.Warn)
            {
                Write($"[WARNING] {message}");
            }
        }

        public static void Verbose(string message)
        {
            if (Instance.Level >= LogLevel.Verbose)
            {
                Write(message);
            }
        }

        private void WriteToFile(object sender, EventArgs e)
        {
            using (var streamWriter = LogFile.AppendText())
            {
                try
                {
                    streamWriter.Write(Sb.ToString());
                    Sb.Clear();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Could not write to log");
                    System.Diagnostics.Debug.WriteLine(ex);
                }

            }
        }

        public enum LogLevel
        {
            None,
            Warn,
            Info,
            Verbose
        }
    }
}
