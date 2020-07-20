namespace IsAnythingServer.Jobs.DailyStatistic
{
    public class Record
    {
        public string Subject { get; set; }
        public string Predicate { get; set; }
        public long TrueDailyCounter { get; set; }
        public long FalseDailyCounter { get; set; }
        public long TrueTotalCounter { get; set; }
        public long FalseTotalCounter { get; set; }
        public bool LastValue { get; set; }
    }
}
