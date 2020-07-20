namespace IsAnythingServer.Jobs
{
    public class CronJobSettings : JobSettings
    {
        public string Cron { get; set; }
        public bool IsExecutedOnStart { get; set; }
        public bool IsActive { get; set; }
    }
}
