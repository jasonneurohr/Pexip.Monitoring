using System;
using System.ComponentModel.DataAnnotations;

namespace Pexip.Monitoring.Agent.Models
{
    public class ParticipantsModel
    {
        [Key]
        public int Id { get; set; }

        public int Timestamp { get; set; }
        public int Bandwidth { get; set; }
        public string CallDirection { get; set; }
        public string CallQuality { get; set; }
        public string CallUuid { get; set; }
        public string Conference { get; set; }
        public DateTime ConnectTime { get; set; }
        public string ConversationId { get; set; }
        public string DestinationAlias { get; set; }
        public string DisplayName { get; set; }
        public string Encryption { get; set; }
        public bool HasMedia { get; set; }
        public string PexipId { get; set; }
        public bool IsMuted { get; set; }
        public bool IsOnHold { get; set; }
        public bool IsPresentationSupported { get; set; }
        public bool IsPresenting { get; set; }
        public bool IsStreaming { get; set; }
        public int LicenseCount { get; set; }
        public string LicenseType { get; set; }
        public string MediaNode { get; set; }
        public string ParentId { get; set; }
        public string ParticipantAlias { get; set; }
        public string Protocol { get; set; }
        public string ProxyNode { get; set; }
        public string RemoteAddress { get; set; }
        public int RemotePort { get; set; }
        public string ResourceUri { get; set; }
        public string Role { get; set; }
        public string ServiceTag { get; set; }
        public string ServiceType { get; set; }
        public string SignallingNode { get; set; }
        public string SourceAlias { get; set; }
        public string SystemLocation { get; set; }
        public string Vendor { get; set; }
    }
}
