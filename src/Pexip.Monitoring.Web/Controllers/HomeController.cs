using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pexip.Monitoring.Web.Models;
using Microsoft.Extensions.Logging;

namespace Pexip.Monitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbService _service;
        private readonly IOptions<AppConfiguration> _config;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DbService service, IOptions<AppConfiguration> config, ILogger<HomeController> logger)
        {
            _service = service;
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("GET request received to index page");
            return View("~/Views/Home/IndexGET.cshtml");
        }

        [HttpGet("/rangedreport")]
        public IActionResult RangedReport()
        {
            _logger.LogInformation("GET request received to ranged report page");
            return View("~/Views/Home/RangedReportGET.cshtml");
        }

        [HttpPost("/rangedreport")]
        public IActionResult RangedReportWithDates()
        {
            _logger.LogInformation("POST request received to ranged report page");
            _logger.LogInformation($"requested range start {Request.Form["reportDateStart"]}");
            _logger.LogInformation($"requested range end {Request.Form["reportDateEnd"]}");
            return View("~/Views/Home/RangedReportPOST.cshtml", new StatisticsPerDayViewModel
            {
                ParticipantQualityTotals = _service.GetParticipantQualityTotalsPerDay(
                    DateTimeOffset.Parse(Request.Form["reportDateStart"]),
                    DateTimeOffset.Parse(Request.Form["reportDateEnd"])),
                ConferencesPerDayModel = _service.GetConferenceStatisticsPerDay(
                    DateTimeOffset.Parse(Request.Form["reportDateStart"]),
                    DateTimeOffset.Parse(Request.Form["reportDateEnd"]))
            });
        }

        [HttpPost("/")]
        public IActionResult IndexWithDate()
        {
            _logger.LogInformation("POST request received to index page");
            _logger.LogInformation($"requested date {Request.Form["reportdate"]}");
            return View("~/Views/Home/IndexPOST.cshtml", new StatisticsViewModel
            {
                ParticipantQualityTotals = _service.GetParticipantQualityTotals(
                    DateTimeOffset.Parse(Request.Form["reportdate"])),
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
