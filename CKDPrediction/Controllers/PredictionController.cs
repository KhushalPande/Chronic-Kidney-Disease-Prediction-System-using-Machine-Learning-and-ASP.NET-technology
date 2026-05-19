using CKDPrediction.Data;
using CKDPrediction.Models;
using CKDPrediction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CKDPrediction.Controllers
{
    [Authorize]
    public class PredictionController : Controller
    {
        private readonly ICKDPredictionService _predictionService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PredictionController(
            ICKDPredictionService predictionService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _predictionService = predictionService;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Predict()
        {
            return View(new CKDInput());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(CKDInput input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var result = _predictionService.Predict(input);
            var user = await _userManager.GetUserAsync(User);

            // Save to history
            var history = new PredictionHistory
            {
                UserId = user!.Id,
                Age = input.Age,
                BloodPressure = input.BloodPressure,
                SerumCreatinine = input.SerumCreatinine,
                Haemoglobin = input.Haemoglobin,
                BloodGlucoseRandom = input.BloodGlucoseRandom,
                BloodUrea = input.BloodUrea,
                Sodium = input.Sodium,
                Potassium = input.Potassium,
                HasCKD = result.HasCKD,
                Probability = result.Probability,
                RiskLevel = result.RiskLevel,
                PredictedAt = DateTime.UtcNow
            };
            _context.PredictionHistories.Add(history);
            await _context.SaveChangesAsync();

            return View("Result", result);
        }

        [HttpGet]
        public async Task<IActionResult> History(int page = 1, string? search = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

            var query = isAdmin
                ? _context.PredictionHistories.Include(p => p.User).AsQueryable()
                : _context.PredictionHistories.Where(p => p.UserId == user!.Id).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.RiskLevel.Contains(search));

            int pageSize = 10;
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(p => p.PredictedAt)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Search = search;
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var record = await _context.PredictionHistories.FindAsync(id);

            if (record != null && (record.UserId == user!.Id || await _userManager.IsInRoleAsync(user, "Admin")))
            {
                _context.PredictionHistories.Remove(record);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("History");
        }
    }
}