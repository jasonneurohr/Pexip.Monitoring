using Pexip.Monitoring.Agent.Models;

namespace Pexip.Monitoring.Agent
{
    public interface ISQLiteServices
    {
        int AddParticipantHistory(ParticipantHistoryModel p);
        int AddConferenceHistory(ConferenceHistoryModel c);
        int AddMediaStreamHistory(MediaStreamHistoryModel m);
    }
}
