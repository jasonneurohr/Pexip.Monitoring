using System.Collections.Generic;

namespace Pexip.Monitoring.Web.Models
{
    public class ConferencesPerDayModel
    {
        public ICollection<ConferenceStatisticsPerDayModel> ConferenceStatistics { get; set; }
        public int TotalConferenceStartEvents { get; set; }
        public int TotalConferenceEndEvents { get; set; }
    }
}
