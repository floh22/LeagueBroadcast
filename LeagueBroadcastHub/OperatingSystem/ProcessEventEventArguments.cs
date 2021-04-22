using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.OperatingSystem
{
    //Taken from https://github.com/Johannes-Schneider/GoldDiff/blob/c5ae4f82a1494c1af582d647ff4c814ad68e7279/GoldDiff/OperatingSystem/ProcessEventEventArguments.cs
    public class ProcessEventEventArguments
    {
        public int ProcessId { get; }

        public ProcessEventEventArguments(int processId)
        {
            ProcessId = processId;
        }
    }
}
