using System;

namespace Pexip.Monitoring.Web.Models
{
    public class ConferenceStatisticsPerDayModel
    {
        public DateTime Day { get; set; }
        public int ConferenceStartCount { get; set; }
        public int ConferenceEndCount { get; set; }
        public int ConferenceDelta { get; set; }
    }
}
