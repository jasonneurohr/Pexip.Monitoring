using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pexip.Monitoring.Web.Models;

namespace Pexip.Monitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbService _service;
        private readonly IOptions<AppConfiguration> _config;

        public HomeController(DbService service, IOptions<AppConfiguration> config)
        {
            _service = service;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new StatisticsViewModel
            {
                ParticipantQualityTotals = _service.GetParticipantQualityTotals(),
                //LossyStreams = _service.GetLossyStreams(_config.Value.FilterList),
                LossySIPStreams = _service.GetLossyStreams(
                    _config.Value.FilterList,
                    "SIP", _config.Value.PacketLossThresholdPercentage),
                LossyMSSIPStreams = _service.GetLossyStreams(
                    _config.Value.FilterList,
                    "MSSIP", _config.Value.PacketLossThresholdPercentage),
                LossyWebRTCStreams = _service.GetLossyStreams(
                    _config.Value.FilterList,
                    "WebRTC", _config.Value.PacketLossThresholdPercentage),
                ConferenceModel = _service.GetConferenceStatistics()
            });
        }

        [HttpPost("/")]
        public IActionResult IndexWithDate()
        {
            return View("Index", new StatisticsViewModel
            {
                ParticipantQualityTotals = _service.GetParticipantQualityTotals(
                    DateTimeOffset.Parse(Request.Form["reportdate"])),
                //LossyStreams = _service.GetLossyStreams(
                //    _config.Value.FilterList,
                //    DateTimeOffset.Parse(Request.Form["reportdate"]),
                //    _config.Value.PacketLossThresholdPercentage),
                ConferenceModel = _service.GetConferenceStatistics(
                    DateTimeOffset.Parse(Request.Form["reportdate"])),
                LossySIPStreams = _service.GetLossyStreams(
                    _config.Value.FilterList,
                    DateTimeOffset.Parse(Request.Form["reportdate"]),
                    "SIP", _config.Value.PacketLossThresholdPercentage),
                LossyMSSIPStreams = _service.GetLossyStreams(
                    _config.Value.FilterList,
                    DateTimeOffset.Parse(Request.Form["reportdate"]),
                    "MSSIP", _config.Value.PacketLossThresholdPercentage),
                LossyWebRTCStreams = _service.GetLossyStreams(
                    _config.Value.FilterList,
                    DateTimeOffset.Parse(Request.Form["reportdate"]),
                    "WebRTC", _config.Value.PacketLossThresholdPercentage)
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
