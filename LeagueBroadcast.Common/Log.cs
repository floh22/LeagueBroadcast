using System;
using System.IO;
using System.Text;

namespace LeagueBroadcast.Common
{
    public class Log
    {
        public static Log Instance;
        private StringBuilder Sb;
        private System.Timers.Timer LogTimer;

        private string LogDir;
        private FileInfo LogFile;

        public LogLevel Level { private set; get; }

        public Log(LogLevel level)
        {
            if (Instance != null)
                return;
            Instance = this;

            this.Level = level;

            Sb = new StringBuilder();
            LogDir = $"{Directory.GetCurrentDirectory()}\\Logs";

            Directory.CreateDirectory(LogDir);
            FileInfo newFileInfo = new FileInfo(Path.Combine(LogDir, $"Log-{DateTime.Now:yyyy-MM-dd-HH-mm}.log"));

            int i = 0;
            while(newFileInfo.Exists)
            {
                newFileInfo = new FileInfo(Path.Combine(LogDir, $"Log-{DateTime.Now:yyyy-MM-dd-HH-mm}-{i}.log"));
                i++;
            }

            LogFile = newFileInfo;
            var fs = LogFile.Create();
            fs.Close();

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
            AppDomain.CurrentDomain.UnhandledException += WriteToFile;

            Write($"Logging Init. Log Level set to {Level}");
        }

        public static void SetLogLevel(LogLevel level)
        {
            Instance.Level = level;
        }

        public static void Write(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Instance.Sb.Append($"[{DateTime.Now:HH-mm-ss}] {message}");
            Instance.Sb.AppendLine();
        }

        public static void WriteToFileAndPause()
        {
            Instance.WriteToFile(null, EventArgs.Empty);
            Instance.LogTimer.Stop();
        }

        public static void Resume()
        {
            Instance.LogTimer.Start();
        }

        public static void Info(object message)
        {
            if (Instance.Level >= LogLevel.Info)
            {
                Write(message);
            }

        }

        public static void Warn(object message)
        {
            if (Instance.Level >= LogLevel.Warn)
            {
                Write($"[WARNING] {message}");
            }
        }

        public static void Verbose(object message)
        {
            if (Instance.Level >= LogLevel.Verbose)
            {
                Write(message);
            }
        }

        private void WriteToFile(object sender, EventArgs e)
        {
            using var streamWriter = LogFile.AppendText();
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
            streamWriter.Close();
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
