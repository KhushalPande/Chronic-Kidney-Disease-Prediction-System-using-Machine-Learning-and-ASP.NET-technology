using CKDPrediction.Data;
using CKDPrediction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CKDPrediction.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

            var query = isAdmin
                ? _context.PredictionHistories.AsQueryable()
                : _context.PredictionHistories.Where(p => p.UserId == user!.Id).AsQueryable();

            var all = await query.OrderByDescending(p => p.PredictedAt).ToListAsync();

            ViewBag.Total = all.Count;
            ViewBag.CKDCount = all.Count(p => p.HasCKD);
            ViewBag.NonCKDCount = all.Count(p => !p.HasCKD);
            ViewBag.HighRisk = all.Count(p => p.RiskLevel == "High");
            ViewBag.Recent = all.Take(5).ToList();

            // Chart data (last 7 days)
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.UtcNow.Date.AddDays(-i))
                .Reverse()
                .ToList();

            ViewBag.ChartLabels = last7Days.Select(d => d.ToString("MMM dd")).ToArray();
            ViewBag.ChartCKD = last7Days.Select(d => all.Count(p => p.PredictedAt.Date == d && p.HasCKD)).ToArray();
            ViewBag.ChartNonCKD = last7Days.Select(d => all.Count(p => p.PredictedAt.Date == d && !p.HasCKD)).ToArray();

            return View();
        }
    }
}