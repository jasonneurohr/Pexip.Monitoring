using System.Collections.Generic;

namespace Pexip.Monitoring.Web.Models
{
    public class StatisticsViewModel
    {
        public ParticipantQualityTotals ParticipantQualityTotals { get; set; }
        public ICollection<ParticipantMediaStreamsWithHighLoss> LossyStreams { get; set; }
        public ConferenceModel ConferenceModel { get; set; }
        public ICollection<ParticipantMediaStreamsWithHighLoss> LossySIPStreams { get; set; }
        public ICollection<ParticipantMediaStreamsWithHighLoss> LossyMSSIPStreams { get; set; }
        public ICollection<ParticipantMediaStreamsWithHighLoss> LossyWebRTCStreams { get; set; }
    }
}
