using Pexip.Monitoring.Agent.Models;

namespace Pexip.Monitoring.Agent
{
    public class SQLiteServices : ISQLiteServices
    {
        private readonly SQLiteContext _context = new SQLiteContext();

        public int AddParticipantHistory(ParticipantHistoryModel p)
        {
            if (_context.ParticipantHistory.Find(p.ParticipantId) == null)
            {
                _context.ParticipantHistory.Add(p);
                return _context.SaveChanges();
            }

            return 0;
        }

        public int AddConferenceHistory(ConferenceHistoryModel c)
        {
            if (_context.ConferenceHistory.Find(c.ConferenceId) == null)
            {
                _context.ConferenceHistory.Add(c);
                return _context.SaveChanges();
            }

            return 0;
        }

        public int AddMediaStreamHistory(MediaStreamHistoryModel m)
        {
            // MediaStreamHistory uses a composite primary key
            // Note, the key order must match what is defined in the context otherwise a null reference exception is raised
            if (_context.MediaStreamHistory.Find(m.ParticipantId, m.Id) == null)
            {
                _context.MediaStreamHistory.Add(m);
                return _context.SaveChanges();
            }

            return 0;
        }
    }
}
