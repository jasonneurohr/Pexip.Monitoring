namespace Pexip.Monitoring.Web.Data
{
    public class ConferenceHistoryModel
    {
        public int Duration { get; set; }
        public int EndTime { get; set; }
        public string ConferenceId { get; set; }
        public int InstantMessageCount { get; set; }
        public string Name { get; set; }
        public int ParticipantCount { get; set; }
        public string ResourceUri { get; set; }
        public string ServiceType { get; set; }
        public int StartTime { get; set; }
        public string Tag { get; set; }
    }
}
