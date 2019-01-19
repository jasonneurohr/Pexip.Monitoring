using System.Collections.Generic;

namespace Pexip.Monitoring.Web.Models
{
    public class ConferenceModel
    {
        public ICollection<ConferenceStatisticsModel> ConferenceStatistics { get; set; }
        public int TotalConferenceStartEvents { get; set; }
        public int TotalConferenceEndEvents { get; set; }
    }
}
