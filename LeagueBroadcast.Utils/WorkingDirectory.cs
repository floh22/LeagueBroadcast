using LeagueBroadcast.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Utils
{
    public static class WorkingDirectory
    {
        private static string? _directory;

        private static int _getDirectoryTimeout = 5000;


        public static async Task<string> GetDirectoryAsync()
        {
            int waitDuration = 0;
            while(_directory is null)
            {
                await Task.Delay(100);
                waitDuration += 100;

                if(waitDuration > _getDirectoryTimeout)
                {
                    _directory = Directory.GetCurrentDirectory();
                }
            }

            return _directory;
        }

        public static string GetDirectory()
        {
            int waitDuration = 0;
            while (_directory is null)
            {
                Task.Delay(100).Wait();
                waitDuration += 100;

                if (waitDuration > _getDirectoryTimeout)
                {
                    _directory = Directory.GetCurrentDirectory();
                }
            }

            return _directory;
        }


        public static void SetDirectory(string directory)
        {
            if (_directory is not null)
            {
                $"Cannot change config directory after it has already been set. Please restart the LeagueBroadcast Server".Warn();
                return;
            }

            _directory = directory;
        }
    }
}
