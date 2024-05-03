using RedditDataRepository.tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthStatus.Controllers
{
    public class HomeController : Controller
    {
        private static HealthCheckRepository _healthCheckRepository = new HealthCheckRepository();

        public ActionResult Index()
        {
            DateTime now = DateTime.UtcNow;
            DateTime oneHourAgo = now.AddHours(-1);
            DateTime twentyFourHoursAgo = now.AddHours(-24);

            int oneHourOkCount = _healthCheckRepository.GetOkCheckCount(oneHourAgo, now);
            int oneHourTotalCount = _healthCheckRepository.GetCheckCount(oneHourAgo, now);
            int twentyFourHoursOkCount = _healthCheckRepository.GetOkCheckCount(twentyFourHoursAgo, now);
            int twentyFourHoursTotalCount = _healthCheckRepository.GetCheckCount(twentyFourHoursAgo, now);

            ViewBag.OneHourOkCount = oneHourOkCount;
            ViewBag.OneHourTotalCount = oneHourTotalCount;
            ViewBag.TwentyFourHoursOkCount = twentyFourHoursOkCount;
            ViewBag.TwentyFourHoursTotalCount = twentyFourHoursTotalCount;
            ViewBag.HealthCheckRepository = _healthCheckRepository;

            return View();
        }

    }
}