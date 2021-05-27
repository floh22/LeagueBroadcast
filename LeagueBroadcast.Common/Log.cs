using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static List<string> ErrorMessageExtras = new List<string>()
        {
            "Looks like Viego and Morde bugs got the better of us.",
            "Everything went according to plan. Seriously, this is the best case scenario.",
            "Still more stable than the League Client",
            "Gotta fix this somehow... Up Up Down Down Left Right Left Right B A Enter... Nah, still broken"
        };
        private string Version;

        public LogLevel Level { private set; get; }

        public Log(LogLevel level, string version)
        {
            if (Instance != null)
                return;
            Instance = this;

            this.Level = level;
            this.Version = version;

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
            AppDomain.CurrentDomain.UnhandledException += HandleCrash;

            Write($"Logging Init");
        }

        public static void SetLogLevel(LogLevel level)
        {
            Instance.Level = level;
            Write($"Log Level set to {level}");
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

        private void HandleCrash(object sender, UnhandledExceptionEventArgs args)
        {
            if(args.IsTerminating)
            {
                var CrashDirName = "CrashLogs";
                var CrashDir = $"{LogDir}\\{CrashDirName}";
                Directory.CreateDirectory(CrashDir);

                Exception e = (Exception) args.ExceptionObject;

                Random r = new Random();
                File.WriteAllText(Path.Combine(CrashDir, $"Crash-{DateTime.Now:yyyy-MM-dd-HH-mm}.log"), 
                    $"--- LeagueBroadcast Crash Report --- \n" +
                    $"{ErrorMessageExtras[r.Next(ErrorMessageExtras.Count)]}\n\n" +
                    $"Time: {DateTime.Now:yyyy.MM.dd-HH:mm}\n" +
                    $"Description: {e.Message}\n\n" +
                    $"Stacktrace: \n{e.StackTrace}\n\n" +
                    $"-- System Details -- \n" +
                    $"League Broadcast Version: {Version}\n" +
                    $"OS: Win 10 {Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString()}");
                Log.Warn($"App Crash detected. Writing Crash report to {CrashDirName}\\Crash-{DateTime.Now:yyyy-MM-dd-HH-mm}.log");
                WriteToFile(null, EventArgs.Empty);
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
