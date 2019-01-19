using System;

namespace Pexip.Monitoring.Web.Models
{
    public class ParticipantQualityTotalsModel
    {
        //0 = Unknown, 1 = Good, 2 = OK, 3 = Bad, 4 = Terrible e.g. 4_terrible
        public DateTime Hour { get; set; }
        public int Unknown { get; set; }
        public int Good { get; set; }
        public int Ok { get; set; }
        public int Bad { get; set; }
        public int Terrible { get; set; }

    }
}
