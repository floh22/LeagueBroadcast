using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Server
{
    class CommandUtils
    {
        public static Process RunNPM (string commandToRun, string workingDirectory = null)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            }

            var processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                WorkingDirectory = workingDirectory,
                ErrorDialog = false,
                CreateNoWindow = false,
                UseShellExecute = false
            };

            var process = Process.Start(processStartInfo);

            if (process == null)
            {
                throw new Exception("Process should not be null.");
            }

            process.StandardInput.WriteLine($"npm run {commandToRun} -loglevel silent");
            return process;
        }
    }
}
