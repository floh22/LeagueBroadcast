namespace LeagueBroadcast.OperatingSystem
{
    //Taken from https://github.com/Johannes-Schneider/GoldDiff/blob/c5ae4f82a1494c1af582d647ff4c814ad68e7279/GoldDiff/OperatingSystem/ProcessEventEventArguments.cs

    public class ProcessEventArguments
    {
        public int ProcessId { get; }

        public ProcessEventArguments(int processId)
        {
            ProcessId = processId;
        }
    }
}
