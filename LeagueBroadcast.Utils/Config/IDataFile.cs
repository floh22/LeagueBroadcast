using System;

namespace LeagueBroadcast.Utils.Config
{
    public interface IDataFile
    {
        event EventHandler FileChanged;
        event EventHandler FileRemoved;
        string Name { get; }

        string FilePath { get; }
        StringVersion FileVersion { get; set; }
        StringVersion CurrentVersion { get; }
        void RevertToDefault(string fileName);
        void CheckForUpdate();
    }
}
