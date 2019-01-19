using Pexip.Lib;
using Pexip.Monitoring.Agent.Models;
using System;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Pexip.Monitoring.Agent
{
    public class PexipOps
    {
        private readonly IManagerNode _managerNode;
        private readonly ISQLiteServices _sqliteServices;


        public PexipOps(IManagerNode m, ISQLiteServices s)
        {
            _managerNode = m;
            _sqliteServices = s;
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public int GetConferenceHistory(int historyHours)
        {
            int confs = 0;
            int conflicts = 0;
            int successfulAdds = 0;

            Log.Information("*************************************************");
            Log.Information("Retrieving Conference History");
            Log.Information($"Filtering from {DateTimeOffset.UtcNow.AddHours(historyHours)} to {DateTimeOffset.UtcNow} in UTC");
            Log.Information("*************************************************");
            var conferenceHistory = _managerNode.ConferenceHistory.Get(DateTimeOffset.UtcNow.AddHours(historyHours), DateTimeOffset.UtcNow).Result;

            foreach (var c in conferenceHistory)
            {
                Log.Debug("Parsing conference {0}", c.Id);
                ConferenceHistoryModel conference = new ConferenceHistoryModel();
                conference.ConferenceId = c.Id;
                conference.Duration = c.Duration;
                conference.EndTime = (c.EndTime == null) ? 0 : (int)DateTimeOffset.Parse(c.EndTime, null, DateTimeStyles.AssumeUniversal).ToUnixTimeSeconds();
                conference.InstantMessageCount = c.InstantMessageCount;
                conference.Name = c.Name;
                conference.ParticipantCount = c.ParticipantCount;
                conference.ResourceUri = c.ResourceUri;
                conference.ServiceType = c.ServiceType;
                conference.StartTime = (c.StartTime == null) ? 0 : (int)DateTimeOffset.Parse(c.StartTime, null, DateTimeStyles.AssumeUniversal).ToUnixTimeSeconds();
                conference.Tag = c.Tag;
                
                try
                {
                    confs++;
                    Log.Debug("Writing conference data to DB");
                    successfulAdds += _sqliteServices.AddConferenceHistory(conference);
                }
                catch (DbUpdateException e)
                {
                    Log.Information(e.InnerException.Message);
                    if(e.InnerException.Message.Contains("UNIQUE constraint failed"))
                    {
                        conflicts++;
                    }
                }
                catch (Exception e)
                {
                    Log.Information($"Exception when writing conference data to DB {e.InnerException.Message}");
                }
            }
            Log.Information($"+++++++++++++++++++++++++++++++++++ Total Confs: {confs}");
            Log.Information($"+++++++++++++++++++++++++++++++++++ Confs added to DB: {successfulAdds}");
            Log.Information($"+++++++++++++++++++++++++++++++++++ Conflicts: {conflicts}");
            return successfulAdds;
        }

        public int GetParticipantHistory(int historyHours)
        {
            int successfulAdds = 0;
            int endTime;

            // Get all the participants
            Log.Information("*************************************************");
            Log.Information("Retrieving Participant History");
            Log.Information($"Filtering from {DateTimeOffset.UtcNow.AddHours(historyHours)} to {DateTimeOffset.UtcNow} in UTC");
            Log.Information("*************************************************");
            var participantList = _managerNode.ParticipantHistory.Get(DateTimeOffset.UtcNow.AddHours(historyHours), DateTimeOffset.UtcNow).Result;

            // Add the timestamp to the participants and add to the DB
            foreach (var p in participantList)
            {
                Log.Debug("Parsing participant {0}", p.Id);

                if (p.EndTime != null)
                {
                    endTime = (int)DateTimeOffset.Parse(p.EndTime).ToUnixTimeSeconds();
                }

                ParticipantHistoryModel participant = new ParticipantHistoryModel();
                participant.AvId = p.AvId;
                participant.Bandwidth = p.Bandwidth;
                participant.CallDirection = p.CallDirection;
                participant.CallQuality = p.CallQuality;
                participant.CallUuid = p.CallUuid;
                participant.Conference = p.Conference ?? "";
                participant.ConferenceName = p.ConferenceName;
                participant.ConversationId = p.ConversationId;
                participant.DisconnectReason = p.DisconnectReason;
                participant.DisplayName = p.DisplayName;
                participant.Duration = p.Duration;
                participant.Encryption = p.Encryption;
                participant.EndTime = (p.EndTime == null) ? -1 : (int)DateTimeOffset.Parse(p.EndTime, null, DateTimeStyles.AssumeUniversal).ToUnixTimeSeconds();
                participant.HasMedia = p.HasMedia;
                participant.ParticipantId = p.Id;
                participant.IsStreaming = p.IsStreaming;
                participant.LicenseCount = p.LicenseCount;
                participant.LicenseType = p.LicenseType;
                participant.LocalAlias = p.LocalAlias;
                participant.MediaNode = p.MediaNode;
                participant.ParentId = p.ParentId;
                participant.PresentationId = p.PresentationId;
                participant.Protocol = p.Protocol;
                participant.ProxyNode = p.ProxyNode;
                participant.RemoteAddress = p.RemoteAddress;
                participant.RemoteAlias = p.RemoteAlias;
                participant.RemotePort = p.RemotePort;
                participant.ResourceUri = p.ResourceUri;
                participant.Role = p.Role;
                participant.ServiceTag = p.ServiceTag;
                participant.ServiceType = p.ServiceType;
                participant.SignallingNode = p.SignallingNode;
                participant.StartTime = (p.StartTime == null) ? -1 : (int)DateTimeOffset.Parse(p.StartTime, null, DateTimeStyles.AssumeUniversal).ToUnixTimeSeconds();
                participant.SystemLocation = p.SystemLocation;
                participant.Vendor = p.Vendor;

                try
                {
                    Log.Debug("Writing participant data to DB");
                    successfulAdds += _sqliteServices.AddParticipantHistory(participant);

                    if (p.MediaStreams.Count > 0)
                    {
                        foreach (var stream in p.MediaStreams)
                        {
                            try
                            {
                                Log.Debug("Parsing and writing participant media stream {0}", stream.StreamId);
                                _sqliteServices.AddMediaStreamHistory(new MediaStreamHistoryModel
                                {
                                    ParticipantId = p.Id,
                                    EndTime = (stream.EndTime == null) ? -1 : (int)DateTimeOffset.Parse(stream.EndTime, null, DateTimeStyles.AssumeUniversal).ToUnixTimeSeconds(),
                                    Id = stream.Id,
                                    Node = stream.Node,
                                    ParticipantUri = stream.Participant,
                                    RxBitrate = stream.RxBitrate,
                                    RxCodec = stream.RxCodec,
                                    RxPacketLoss = stream.RxPacketLoss,
                                    RxPacketsLost = stream.RxPacketsLost,
                                    RxPacketsReceived = stream.RxPacketsReceived,
                                    RxResolution = stream.RxResolution,
                                    StartTime = (stream.StartTime == null) ? -1 :(int)DateTimeOffset.Parse(stream.StartTime, null, DateTimeStyles.AssumeUniversal).ToUnixTimeSeconds(),
                                    StreamId = stream.StreamId,
                                    StreamType = stream.StreamType,
                                    TxBitrate = stream.TxBitrate,
                                    TxCodec = stream.TxCodec,
                                    TxPacketLoss = stream.TxPacketLoss,
                                    TxPacketsLost = stream.TxPacketsLost,
                                    TxPacketsSent = stream.TxPacketsSent,
                                    TxResolution = stream.TxResolution
                                });
                            }
                            catch (Exception e)
                            {
                                Log.Information($"Exception when writing participant media stream data to DB{e.InnerException.Message}");
                            }
                        }
                    }

                } catch (Exception e)
                {
                    Log.Information($"Exception when writing participant data to DB: {e.InnerException.Message}");
                }
            }

            return successfulAdds;
        }
    }
}
