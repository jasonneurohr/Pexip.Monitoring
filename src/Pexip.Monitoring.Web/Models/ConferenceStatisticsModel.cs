using System;

namespace Pexip.Monitoring.Web.Models
{
    public class ConferenceStatisticsModel
    {
        public DateTime Hour { get; set; }
        public int ConferenceStartCount { get; set; }
        public int ConferenceEndCount { get; set; }
        public int ConferenceDelta { get; set; }
    }
}
