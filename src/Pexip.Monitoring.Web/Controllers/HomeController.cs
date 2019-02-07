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
            return View("~/Views/Home/IndexGET.cshtml");
        }

        [HttpGet("/rangedreport")]
        public IActionResult RangedReport()
        {
            return View("~/Views/Home/RangedReportGET.cshtml");
        }

        [HttpPost("/rangedreport")]
        public IActionResult RangedReportWithDates()
        {
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
