namespace Pexip.Monitoring.Web.Models
{
    public class ParticipantMediaStreamsWithHighLoss
    {
        public string ParticipantId { get; set; }
        public double RxPacketLoss { get; set; }
        public double TxPacketLoss { get; set; }
        public string StreamType { get; set; }
        public string LocalAlias { get; set; }
        public string DisplayName { get; set; }
        public string Protocol { get; set; }
    }
}
