using Pexip.Monitoring.Web.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Pexip.Monitoring.Web.Models;

namespace Pexip.Monitoring.Web
{
    public class DbService
    {
        readonly SQLiteContext _context;

        public DbService(SQLiteContext context)
        {
            _context = context;
        }

        #region statistics methods

        /// <summary>
        /// Returns a conferenceModel object consisting of the conferences per hour (started, stopped, and delta) for the current day
        /// </summary>
        /// <returns>ConferenceModel</returns>
        public ConferenceModel GetConferenceStatistics()
        {
            ICollection<ConferenceStatisticsModel> conferenceTotalsPerHour = new List<ConferenceStatisticsModel>();

            int hour = 0;
            var hourly = new DateTimeOffset(DateTimeOffset.UtcNow.ToLocalTime().Date);

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var conferenceStartCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd)).Count();
                var conferenceEndCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd)).Count();

                var conferenceDelta = (conferenceStartCount >= conferenceEndCount) ? conferenceStartCount - conferenceEndCount : conferenceEndCount - conferenceStartCount;

                conferenceTotalsPerHour.Add(new ConferenceStatisticsModel()
                {
                    Hour = new DateTime(1970, 1, 1, hour, 0, 0),
                    ConferenceStartCount = conferenceStartCount,
                    ConferenceEndCount = conferenceEndCount,
                    ConferenceDelta = conferenceDelta
                });

                hour++;
            }

            var totalConferenceStarts = conferenceTotalsPerHour.Sum(i => i.ConferenceStartCount);

            ConferenceModel conferenceModel = new ConferenceModel()
            {
                ConferenceStatistics = conferenceTotalsPerHour,
                TotalConferenceStartEvents = conferenceTotalsPerHour.Sum(i => i.ConferenceStartCount),
                TotalConferenceEndEvents = conferenceTotalsPerHour.Sum(i => i.ConferenceEndCount)
            };

            return conferenceModel;
        }

        /// <summary>
        /// Returns a ConferenceModel object consisting of the conferences per hour (started, stopped, and delta) for the current day with a date param to base the query on an alternate date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>ConferenceModel</returns>
        public ConferenceModel GetConferenceStatistics(DateTimeOffset date)
        {
            ICollection<ConferenceStatisticsModel> conferenceTotalsPerHour = new List<ConferenceStatisticsModel>();

            int hour = 0;
            var hourly = date;

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var conferenceStartCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd)).Count();
                var conferenceEndCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd)).Count();

                var conferenceDelta = (conferenceStartCount >= conferenceEndCount) ? conferenceStartCount - conferenceEndCount : conferenceEndCount - conferenceStartCount;

                conferenceTotalsPerHour.Add(new ConferenceStatisticsModel()
                {
                    Hour = new DateTime(1970, 1, 1, hour, 0, 0),
                    ConferenceStartCount = conferenceStartCount,
                    ConferenceEndCount = conferenceEndCount,
                    ConferenceDelta = conferenceDelta
                });

                hour++;
            }

            var totalConferenceStarts = conferenceTotalsPerHour.Sum(i => i.ConferenceStartCount);

            ConferenceModel conferenceModel = new ConferenceModel()
            {
                ConferenceStatistics = conferenceTotalsPerHour,
                TotalConferenceStartEvents = conferenceTotalsPerHour.Sum(i => i.ConferenceStartCount),
                TotalConferenceEndEvents = conferenceTotalsPerHour.Sum(i => i.ConferenceEndCount)
            };

            return conferenceModel;
        }

        /// <summary>
        /// Returns a ParticipantQualityTotals object consisting of the CallQuality totals per hour for the current day
        /// </summary>
        /// <returns>ParticipantQualityTotals</returns>
        public ParticipantQualityTotals GetParticipantQualityTotals()
        {
            ICollection<ParticipantQualityTotalsModel> qualityTotalsPerHour = new List<ParticipantQualityTotalsModel>();

            int hour = 0;
            var hourly = new DateTimeOffset(DateTimeOffset.UtcNow.ToLocalTime().Date);

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var unknown = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "0_unknown")
                    .Select(i => i.CallQuality).Count();

                var good = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "1_good")
                    .Select(i => i.CallQuality).Count();

                var ok = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "2_ok")
                    .Select(i => i.CallQuality).Count();

                var bad = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "3_bad")
                    .Select(i => i.CallQuality).Count();

                var terrible = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "4_terrible")
                    .Select(i => i.CallQuality).Count();

                qualityTotalsPerHour.Add(new ParticipantQualityTotalsModel()
                {
                    Hour = new DateTime(1970, 1, 1, hour, 0, 0),
                    Unknown = unknown,
                    Good = good,
                    Ok = ok,
                    Bad = bad,
                    Terrible = terrible
                });

                hour++;
            }

            ParticipantQualityTotals participantQualityTotals = new ParticipantQualityTotals
            {
                ParticipantQualities = qualityTotalsPerHour,
                TotalUnknown = qualityTotalsPerHour.Sum(i => i.Unknown),
                TotalGood = qualityTotalsPerHour.Sum(i => i.Good),
                TotalOk = qualityTotalsPerHour.Sum(i => i.Ok),
                TotalBad = qualityTotalsPerHour.Sum(i => i.Bad),
                TotalTerrible = qualityTotalsPerHour.Sum(i => i.Terrible)
            };

            return participantQualityTotals;
        }

        /// <summary>
        /// Returns a ParticipantQualityTotals object consisting of the CallQuality totals with a date param to base the query on an alternate date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>ParticipantQualityTotals</returns>
        public ParticipantQualityTotals GetParticipantQualityTotals(DateTimeOffset date)
        {
            ICollection<ParticipantQualityTotalsModel> qualityTotalsPerHour = new List<ParticipantQualityTotalsModel>();

            int hour = 0;
            var hourly = date;

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var unknown = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "0_unknown")
                    .Select(i => i.CallQuality).Count();

                var good = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "1_good")
                    .Select(i => i.CallQuality).Count();

                var ok = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "2_ok")
                    .Select(i => i.CallQuality).Count();

                var bad = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "3_bad")
                    .Select(i => i.CallQuality).Count();

                var terrible = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= timeWindowEnd && i.CallQuality == "4_terrible")
                    .Select(i => i.CallQuality).Count();

                qualityTotalsPerHour.Add(new ParticipantQualityTotalsModel()
                {
                    Hour = new DateTime(1970, 1, 1, hour, 0, 0),
                    Unknown = unknown,
                    Good = good,
                    Ok = ok,
                    Bad = bad,
                    Terrible = terrible
                });

                hour++;
            }

            ParticipantQualityTotals participantQualityTotals = new ParticipantQualityTotals
            {
                ParticipantQualities = qualityTotalsPerHour,
                TotalUnknown = qualityTotalsPerHour.Sum(i => i.Unknown),
                TotalGood = qualityTotalsPerHour.Sum(i => i.Good),
                TotalOk = qualityTotalsPerHour.Sum(i => i.Ok),
                TotalBad = qualityTotalsPerHour.Sum(i => i.Bad),
                TotalTerrible = qualityTotalsPerHour.Sum(i => i.Terrible)
            };

            return participantQualityTotals;
        }

        /// <summary>
        /// Returns a collection of ParticipantMediaStreamsWithHighLoss objects that contains attributes of participant media streams which suffered a level of packet loss
        /// </summary>
        /// <remarks>
        /// The packetLossThresholdPercentage is three by default, however, can be adjusted using the appsettings.json
        /// </remarks>
        /// <param name="remoteAliasFilterList"></param>
        /// <returns>ICollection<ParticipantMediaStreamsWithHighLoss></returns>
        public ICollection<ParticipantMediaStreamsWithHighLoss> GetLossyStreams(List<string> remoteAliasFilterList, int packetLossThresholdPercentage=3)
        {
            
            var today = new DateTimeOffset(DateTimeOffset.UtcNow.ToLocalTime().Date).ToUnixTimeSeconds();
            var tomorrow = new DateTimeOffset(DateTimeOffset.UtcNow.ToLocalTime().Date).AddDays(1).ToUnixTimeSeconds();

            var lossyParticipants = _context.MediaStreamHistory.Join(_context.ParticipantHistory,
                a => a.ParticipantId,
                b => b.ParticipantId,
                (a, b) => new {
                    a.ParticipantId,
                    b.DisplayName,
                    b.LocalAlias,
                    a.RxPacketLoss,
                    a.TxPacketLoss,
                    a.StreamType,
                    b.Protocol,
                    a.StartTime
                })
                .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= today && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= tomorrow && (i.RxPacketLoss >= packetLossThresholdPercentage || i.TxPacketLoss >= packetLossThresholdPercentage))
                .Select(i => new ParticipantMediaStreamsWithHighLoss
                {
                    ParticipantId = i.ParticipantId,
                    DisplayName = i.DisplayName,
                    LocalAlias = i.LocalAlias,
                    RxPacketLoss = i.RxPacketLoss,
                    TxPacketLoss = i.TxPacketLoss,
                    StreamType = i.StreamType,
                    Protocol = i.Protocol
                })
                .ToList();

            // Device Filter - If the device matches the filter based on the RemoteAlias it will not be added to the filtered list
            ICollection<ParticipantMediaStreamsWithHighLoss> filteredLossyParticipants = new List<ParticipantMediaStreamsWithHighLoss>();

            foreach (var rec in lossyParticipants)
            {
                if (!Filter(rec.DisplayName, remoteAliasFilterList))
                {
                    filteredLossyParticipants.Add(rec);
                }
            }

            return filteredLossyParticipants;
        }

        /// <summary>
        /// Returns a collection of ParticipantMediaStreamsWithHighLoss objects that contains attributes of participant media streams which suffered a level of packet loss with a date param to base the query on an alternate date
        /// </summary>
        /// <remarks>
        /// The packetLossThresholdPercentage is three by default, however, can be adjusted using the appsettings.json
        /// </remarks>
        /// <param name="remoteAliasFilterList"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ICollection<ParticipantMediaStreamsWithHighLoss> GetLossyStreams(List<string> remoteAliasFilterList, DateTimeOffset date, int packetLossThresholdPercentage = 3)
        {
            var today = date.ToUnixTimeSeconds();
            var tomorrow = date.AddDays(1).ToUnixTimeSeconds();

            var lossyParticipants = _context.MediaStreamHistory.Join(_context.ParticipantHistory,
                a => a.ParticipantId,
                b => b.ParticipantId,
                (a, b) => new {
                    a.ParticipantId,
                    b.DisplayName,
                    b.LocalAlias,
                    a.RxPacketLoss,
                    a.TxPacketLoss,
                    a.StreamType,
                    b.Protocol,
                    a.StartTime
                })
                .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= today && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= tomorrow && (i.RxPacketLoss >= packetLossThresholdPercentage || i.TxPacketLoss >= packetLossThresholdPercentage))
                .Select(i => new ParticipantMediaStreamsWithHighLoss
                {
                    ParticipantId = i.ParticipantId,
                    DisplayName = i.DisplayName,
                    LocalAlias = i.LocalAlias,
                    RxPacketLoss = i.RxPacketLoss,
                    TxPacketLoss = i.TxPacketLoss,
                    StreamType = i.StreamType,
                    Protocol = i.Protocol
                })
                .ToList();

            // Device Filter - If the device matches the filter based on the RemoteAlias it will not be added to the filtered list
            ICollection<ParticipantMediaStreamsWithHighLoss> filteredLossyParticipants = new List<ParticipantMediaStreamsWithHighLoss>();

            foreach (var rec in lossyParticipants)
            {
                if (!Filter(rec.DisplayName, remoteAliasFilterList))
                {
                    filteredLossyParticipants.Add(rec);
                }
            }

            return filteredLossyParticipants;
        }

        /// <summary>
        /// Used to filter the Aliases based on the filter list from the appsettings.json
        /// </summary>
        /// <param name="s"></param>
        /// <param name="remoteAliasFilterList"></param>
        /// <returns>bool</returns>
        public bool Filter(string s, List<string> remoteAliasFilterList)
        {
            foreach (var filterString in remoteAliasFilterList)
            {
                if (s == filterString)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
