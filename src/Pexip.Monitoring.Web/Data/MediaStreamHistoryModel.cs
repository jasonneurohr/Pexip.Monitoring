using System;

namespace Pexip.Monitoring.Web.Data
{
    public class MediaStreamHistoryModel
    {
        public int EndTime { get; set; }
        public int Id { get; set; }
        public string Node { get; set; }
        public string ParticipantUri { get; set; }
        public int RxBitrate { get; set; }
        public string RxCodec { get; set; }
        public double RxPacketLoss { get; set; }
        public int RxPacketsLost { get; set; }
        public int RxPacketsReceived { get; set; }
        public string RxResolution { get; set; }
        public int StartTime { get; set; }
        public string StreamId { get; set; }
        public string StreamType { get; set; }
        public int TxBitrate { get; set; }
        public string TxCodec { get; set; }
        public double TxPacketLoss { get; set; }
        public int TxPacketsLost { get; set; }
        public int TxPacketsSent { get; set; }
        public string TxResolution { get; set; }

        public string ParticipantId { get; set; }
        public ParticipantHistoryModel Participant { get; set; }
    }
}
