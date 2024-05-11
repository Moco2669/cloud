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

            double oneHourRatio = oneHourTotalCount == 0 ? 0 : (double)oneHourOkCount / oneHourTotalCount;

            ViewBag.OneHourRatio = oneHourRatio * 100;

            List<double> twentyFourHoursRatios = new List<double>();

            for (int i = 0; i < 24; i++)
            {
                DateTime hourStartTime = DateTime.UtcNow.AddHours(-24 + i);
                DateTime hourEndTime = hourStartTime.AddHours(1);

                int hourOkCount = _healthCheckRepository.GetOkCheckCount(hourStartTime, hourEndTime);
                int hourTotalCount = _healthCheckRepository.GetCheckCount(hourStartTime, hourEndTime);

                double hourRatio = hourTotalCount == 0 ? 0 : (double)hourOkCount / hourTotalCount;

                twentyFourHoursRatios.Add(hourRatio);
            }

            ViewBag.TwentyFourHoursRatios = twentyFourHoursRatios;

            return View();
        }

    }
}