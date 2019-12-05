using Pexip.Monitoring.Web.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Pexip.Monitoring.Web.Models;
using Microsoft.Extensions.Logging;

namespace Pexip.Monitoring.Web
{
    public class DbService
    {
        readonly SQLiteContext _context;
        private readonly ILogger<DbService> _logger;

        public DbService(SQLiteContext context, ILogger<DbService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region statistics methods

        /// <summary>
        /// Returns a conferenceModel object consisting of the conferences per hour for the current day
        /// </summary>
        /// <returns>ConferenceModel</returns>
        public ConferenceModel GetConferenceStatistics()
        {
            _logger.LogInformation("Getting ConferenceStatistics from the DB");
            ICollection<ConferenceStatisticsModel> conferenceTotalsPerHour = new List<ConferenceStatisticsModel>();

            int hour = 0;
            var hourly = new DateTimeOffset(DateTimeOffset.UtcNow.ToLocalTime().Date);

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var conferenceStartCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd)).Count();
                var conferenceEndCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd)).Count();

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
        /// Returns a ConferenceModel object consisting of the conferences per hour for a specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>ConferenceModel</returns>
        public ConferenceModel GetConferenceStatistics(DateTimeOffset date)
        {
            _logger.LogInformation("Getting ConferenceStatistics from the DB with date");
            ICollection<ConferenceStatisticsModel> conferenceTotalsPerHour = new List<ConferenceStatisticsModel>();

            int hour = 0;
            var hourly = date;

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var conferenceStartCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd)).Count();
                var conferenceEndCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd)).Count();

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
        /// Returns a ConferencesPerDayModel object of the confernces per day for the specified duration
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>ConferencesPerDayModel</returns>
        public ConferencesPerDayModel GetConferenceStatisticsPerDay(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            _logger.LogInformation("Getting GetConferenceStatisticsPerDay from the DB");
            ICollection<ConferenceStatisticsPerDayModel> conferenceTotalsPerDay = new List<ConferenceStatisticsPerDayModel>();

            int numOfDays = (endDate - startDate).Days + 1;

            int dayCount = 0;

            while (dayCount <= numOfDays)
            {
                var timeWindowStart = startDate.AddDays(dayCount).ToUnixTimeSeconds();
                var timeWindowEnd = startDate.AddDays(dayCount + 1).ToUnixTimeSeconds();

                var conferenceStartCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd)).Count();
                var conferenceEndCount = _context.ConferenceHistory.Where(i => (DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.EndTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd)).Count();

                var conferenceDelta = (conferenceStartCount >= conferenceEndCount) ? conferenceStartCount - conferenceEndCount : conferenceEndCount - conferenceStartCount;

                conferenceTotalsPerDay.Add(new ConferenceStatisticsPerDayModel()
                {
                    Day = DateTimeOffset.FromUnixTimeSeconds(timeWindowStart).Date,
                    ConferenceStartCount = conferenceStartCount,
                    ConferenceEndCount = conferenceEndCount,
                    ConferenceDelta = conferenceDelta
                });

                dayCount++;
            }

            var totalConferenceStarts = conferenceTotalsPerDay.Sum(i => i.ConferenceStartCount);

            ConferencesPerDayModel conferenceModel = new ConferencesPerDayModel()
            {
                ConferenceStatistics = conferenceTotalsPerDay,
                TotalConferenceStartEvents = conferenceTotalsPerDay.Sum(i => i.ConferenceStartCount),
                TotalConferenceEndEvents = conferenceTotalsPerDay.Sum(i => i.ConferenceEndCount)
            };

            return conferenceModel;
        }

        /// <summary>
        /// Returns a ParticipantQualityTotals object per hour for the current day
        /// </summary>
        /// <returns>ParticipantQualityTotals</returns>
        public ParticipantQualityTotals GetParticipantQualityTotals()
        {
            _logger.LogInformation("Getting GetParticipantQualityTotals from the DB");
            ICollection<ParticipantQualityTotalsModel> qualityTotalsPerHour = new List<ParticipantQualityTotalsModel>();

            int hour = 0;
            var hourly = new DateTimeOffset(DateTimeOffset.UtcNow.ToLocalTime().Date);

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var unknown = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "0_unknown")
                    .Select(i => i.CallQuality).Count();

                var good = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "1_good")
                    .Select(i => i.CallQuality).Count();

                var ok = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "2_ok")
                    .Select(i => i.CallQuality).Count();

                var bad = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "3_bad")
                    .Select(i => i.CallQuality).Count();

                var terrible = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "4_terrible")
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
        /// Returns a ParticipantQualityTotals object per hour for a specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>ParticipantQualityTotals</returns>
        public ParticipantQualityTotals GetParticipantQualityTotals(DateTimeOffset date)
        {
            _logger.LogInformation("Getting GetParticipantQualityTotals from the DB with date");
            ICollection<ParticipantQualityTotalsModel> qualityTotalsPerHour = new List<ParticipantQualityTotalsModel>();

            int hour = 0;
            var hourly = date;

            while (hour < 24)
            {
                var timeWindowStart = hourly.AddHours(hour).ToUnixTimeSeconds();
                var timeWindowEnd = hourly.AddHours(hour + 1).ToUnixTimeSeconds();

                var unknown = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "0_unknown")
                    .Select(i => i.CallQuality).Count();

                var good = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "1_good")
                    .Select(i => i.CallQuality).Count();

                var ok = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "2_ok")
                    .Select(i => i.CallQuality).Count();

                var bad = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "3_bad")
                    .Select(i => i.CallQuality).Count();

                var terrible = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "4_terrible")
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
        /// Returns a ParticipantQualityTotals object per day for the specified duration
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>ParticipantQualityTotals</returns>
        public ParticipantQualityTotalsPerDay GetParticipantQualityTotalsPerDay(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            _logger.LogInformation("Getting GetParticipantQualityTotalsPerDay from the DB");
            ICollection<ParticipantQualityTotalsPerDayModel> qualityTotalsPerDay = new List<ParticipantQualityTotalsPerDayModel>();

            int numOfDays = (endDate - startDate).Days + 1;

            int dayCount = 0;

            while (dayCount <= numOfDays)
            {
                var timeWindowStart = startDate.AddDays(dayCount).ToLocalTime().ToUnixTimeSeconds();
                var timeWindowEnd = startDate.AddDays(dayCount + 1).ToLocalTime().ToUnixTimeSeconds();

                var unknown = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "0_unknown")
                    .Select(i => i.CallQuality).Count();

                var good = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "1_good")
                    .Select(i => i.CallQuality).Count();

                var ok = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "2_ok")
                    .Select(i => i.CallQuality).Count();

                var bad = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "3_bad")
                    .Select(i => i.CallQuality).Count();

                var terrible = _context.ParticipantHistory
                    .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= timeWindowStart && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() < timeWindowEnd && i.CallQuality == "4_terrible")
                    .Select(i => i.CallQuality).Count();

                qualityTotalsPerDay.Add(new ParticipantQualityTotalsPerDayModel()
                {
                    Day = DateTimeOffset.FromUnixTimeSeconds(timeWindowStart).Date,
                    Unknown = unknown,
                    Good = good,
                    Ok = ok,
                    Bad = bad,
                    Terrible = terrible
                });

                dayCount++;
            }

            ParticipantQualityTotalsPerDay participantQualityTotals = new ParticipantQualityTotalsPerDay
            {
                ParticipantQualities = qualityTotalsPerDay,
                TotalUnknown = qualityTotalsPerDay.Sum(i => i.Unknown),
                TotalGood = qualityTotalsPerDay.Sum(i => i.Good),
                TotalOk = qualityTotalsPerDay.Sum(i => i.Ok),
                TotalBad = qualityTotalsPerDay.Sum(i => i.Bad),
                TotalTerrible = qualityTotalsPerDay.Sum(i => i.Terrible)
            };

            return participantQualityTotals;
        }

        /// <summary>
        /// Returns a collection of ParticipantMediaStreamsWithHighLoss objects
        /// </summary>
        /// <remarks>
        /// The packetLossThresholdPercentage is three by default, however, can be adjusted using the appsettings.json
        /// </remarks>
        /// <param name="remoteAliasFilterList"></param>
        /// <param name="packetLossThresholdPercentage"></param>
        /// <returns>ICollection<ParticipantMediaStreamsWithHighLoss></returns>
        public ICollection<ParticipantMediaStreamsWithHighLoss> GetLossyStreams(List<string> remoteAliasFilterList, int packetLossThresholdPercentage=3)
        {
            _logger.LogInformation("Getting GetLossyStreams from the DB with remoteAliasFilterList and packetLossThresholdPercentage");
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
        /// Returns a collection of ParticipantMediaStreamsWithHighLoss objects
        /// </summary>
        /// <remarks>
        /// The packetLossThresholdPercentage is three by default, however, can be adjusted using the appsettings.json
        /// Facilitates filtering using a protocolFilter parameter, e.g. "SIP", "WebRTC", "MSSIP".
        /// </remarks>
        /// <param name="remoteAliasFilterList"></param>
        /// <param name="protocolFilter">SIP, MSSIP, or WebRTC</param>
        /// <returns>ICollection<ParticipantMediaStreamsWithHighLoss></returns>
        public ICollection<ParticipantMediaStreamsWithHighLoss> GetLossyStreams(List<string> remoteAliasFilterList, string protocolFilter, int packetLossThresholdPercentage = 3)
        {
            _logger.LogInformation("Getting GetLossyStreams from the DB with remoteAliasFilterList, protocolFilter, and packetLossThresholdPercentage");
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
                .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= today && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= tomorrow && (i.RxPacketLoss >= packetLossThresholdPercentage || i.TxPacketLoss >= packetLossThresholdPercentage) && i.Protocol == protocolFilter)
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
        /// Returns a collection of ParticipantMediaStreamsWithHighLoss objects
        /// </summary>
        /// <remarks>
        /// The packetLossThresholdPercentage is three by default, however, can be adjusted using the appsettings.json
        /// </remarks>
        /// <param name="remoteAliasFilterList"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ICollection<ParticipantMediaStreamsWithHighLoss> GetLossyStreams(List<string> remoteAliasFilterList, DateTimeOffset date, int packetLossThresholdPercentage = 3)
        {
            _logger.LogInformation("Getting GetLossyStreams from the DB with remoteAliasFilterList, date, and packetLossThresholdPercentage");
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
        /// Returns a collection of ParticipantMediaStreamsWithHighLoss
        /// </summary>
        /// <remarks>
        /// The packetLossThresholdPercentage is three by default, however, can be adjusted using the appsettings.json
        /// Facilitates filtering using a protocolFilter parameter, e.g. "SIP", "WebRTC", "MSSIP".
        /// </remarks>
        /// <param name="remoteAliasFilterList"></param>
        /// <param name="protocolFilter">SIP, MSSIP, or WebRTC</param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ICollection<ParticipantMediaStreamsWithHighLoss> GetLossyStreams(List<string> remoteAliasFilterList, DateTimeOffset date, string protocolFilter, int packetLossThresholdPercentage = 3)
        {
            _logger.LogInformation("Getting GetLossyStreams from the DB with remoteAliasFilterList, date, protocolFilter, and packetLossThresholdPercentage");
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
                .Where(i => DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() >= today && DateTimeOffset.FromUnixTimeSeconds(i.StartTime).ToLocalTime().ToUnixTimeSeconds() <= tomorrow && (i.RxPacketLoss >= packetLossThresholdPercentage || i.TxPacketLoss >= packetLossThresholdPercentage) && i.Protocol == protocolFilter)
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
        /// <param name="s">The alias to check</param>
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
