using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Events
{
    public static class CommunityDragonEventHandler
    {
        public static EventHandler<CommunityDragonDataProviderLoadedEventArgs>? LoadComplete { get; set; }

        public static EventHandler<FileLoadProgressEventArgs>? FileDownloadComplete { get; set; }
    }


    public class CommunityDragonDataProviderLoadedEventArgs : EventArgs
    {
        public bool LoadSuccess { get; set; }

        public TimeSpan LoadDuration { get; set; }


        public CommunityDragonDataProviderLoadedEventArgs(bool loadSuccess, TimeSpan loadDuration)
        {
            LoadSuccess = loadSuccess;
            LoadDuration = loadDuration;
        }
    }

    public class FileLoadProgressEventArgs : EventArgs
    {
        public string FileName { get; set; } = "";

        public string Task { get; set; } = "";
        public int Completed { get; set; }
        public int Total { get; set; }


        public FileLoadProgressEventArgs(string fileName, string task, int completed, int total)
        {
            FileName = fileName;
            Task = task;
            Completed = completed;
            Total = total;
        }
    }
}
