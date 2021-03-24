using Adventure.Logic.Models;
using Adventure.Logic.Services;
using Adventure.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Adventure.Web.Controllers
{
    public class HomeController : Controller
    {
        private static List<BestSalesPeopleModel> _bestSalesStub = Enumerable.Range(0, 999).Select(i => new BestSalesPeopleModel()).ToList();
        private readonly ILogger<HomeController> _logger;
        private readonly StatisticsService _statisticsService;

        public HomeController(ILogger<HomeController> logger, StatisticsService statisticsService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            HomeIndexViewModel vm = new HomeIndexViewModel();
            vm.BestSalesPeople = _bestSalesStub;// await _statisticsService.GetBestSalesPeople(10);
            return View(vm);
        }

        public async Task<IActionResult> Cache(int top)
        {
            await _statisticsService.CacheProductModelProductDescription(top);
            return new EmptyResult();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
