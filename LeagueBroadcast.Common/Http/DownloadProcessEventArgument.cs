using System;

namespace LeagueBroadcast.Update.Http
{
    //https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Http/DownloadProgressEventArguments.cs
    public class DownloadProgressEventArguments
    {
        public double Progress { get; }

        public double AverageDownloadSpeedInMBs { get; }

        public TimeSpan EstimatedRemainingTime { get; }

        public DownloadProgressEventArguments(double progress, double averageDownloadSpeedInMBs, TimeSpan estimatedRemainingTime)
        {
            Progress = progress;
            AverageDownloadSpeedInMBs = averageDownloadSpeedInMBs;
            EstimatedRemainingTime = estimatedRemainingTime;
        }
    }
}
