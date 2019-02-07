using System;

namespace Pexip.Monitoring.Web.Models
{
    public class ParticipantQualityTotalsPerDayModel
    {
        public DateTime Day { get; set; }
        public int Unknown { get; set; }
        public int Good { get; set; }
        public int Ok { get; set; }
        public int Bad { get; set; }
        public int Terrible { get; set; }
    }
}
