using System.Collections.Generic;

namespace Pexip.Monitoring.Web.Models
{
    public class AppConfiguration
    {
        public List<string> FilterList { get; set; }

        public int PacketLossThresholdPercentage { get; set; }
    }
}
