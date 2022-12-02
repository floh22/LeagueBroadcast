namespace LeagueBroadcast.Common.Config
{
    interface IWatchableFile
    {
        bool AttachFileWatcher();
        bool DetachFileWatcher();
    }
}
