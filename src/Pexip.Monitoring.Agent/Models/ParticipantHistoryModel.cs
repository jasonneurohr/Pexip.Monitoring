using System.Collections.Generic;

namespace Pexip.Monitoring.Agent.Models
{
    public class ParticipantHistoryModel
    {
        public string ParticipantId { get; set; }
        public string AvId { get; set; }
        public int Bandwidth { get; set; }
        public string CallDirection { get; set; }
        public string CallQuality { get; set; }
        public string CallUuid { get; set; }
        public string Conference { get; set; }
        public string ConferenceName { get; set; }
        public string ConversationId { get; set; }
        public string DisconnectReason { get; set; }
        public string DisplayName { get; set; }
        public int Duration { get; set; }
        public string Encryption { get; set; }
        public int EndTime { get; set; }
        public bool HasMedia { get; set; }
        public bool IsStreaming { get; set; }
        public int LicenseCount { get; set; }
        public string LicenseType { get; set; }
        public string LocalAlias { get; set; }
        public string MediaNode { get; set; }
        public List<MediaStreamHistoryModel> MediaStreams { get; set; }
        public string ParentId { get; set; }
        public string PresentationId { get; set; }
        public string Protocol { get; set; }
        public string ProxyNode { get; set; }
        public string RemoteAddress { get; set; }
        public string RemoteAlias { get; set; }
        public int RemotePort { get; set; }
        public string ResourceUri { get; set; }
        public string Role { get; set; }
        public string ServiceTag { get; set; }
        public string ServiceType { get; set; }
        public string SignallingNode { get; set; }
        public int StartTime { get; set; }
        public string SystemLocation { get; set; }
        public string Vendor { get; set; }
    }
}
