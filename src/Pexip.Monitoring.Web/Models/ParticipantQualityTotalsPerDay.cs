using System.Collections.Generic;

namespace Pexip.Monitoring.Web.Models
{
    public class ParticipantQualityTotalsPerDay
    {
        public ICollection<ParticipantQualityTotalsPerDayModel> ParticipantQualities { get; set; }
        public int TotalUnknown { get; set; }
        public int TotalGood { get; set; }
        public int TotalOk { get; set; }
        public int TotalBad { get; set; }
        public int TotalTerrible { get; set; }
    }
}
