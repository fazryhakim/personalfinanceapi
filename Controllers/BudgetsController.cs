using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Data;
using PersonalFinanceAPI.Models;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BudgetsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        // GET: api/budgets?month=3&year=2026
        [HttpGet]
        public async Task<IActionResult> GetBudgets([FromQuery] int month, [FromQuery] int year)
        {
            int userId = GetUserId();

            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                .ToListAsync();

            var result = new List<object>();
            foreach (var b in budgets)
            {
                var totalSpent = await _context.Transactions
                    .Where(t => t.UserId == userId && (int)t.Type == 1 && t.CategoryId == b.CategoryId &&
                                t.Date.Month == month && t.Date.Year == year)
                    .SumAsync(t => (decimal?)t.Amount) ?? 0;

                result.Add(new
                {
                    id = b.Id,
                    categoryId = b.CategoryId,
                    categoryName = b.Category?.Name ?? "Tanpa Kategori",
                    amount = b.Amount,
                    spent = totalSpent,
                    percentage = b.Amount > 0 ? Math.Round((totalSpent / b.Amount) * 100, 1) : 0,
                    month = b.Month,
                    year = b.Year
                });
            }

            return Ok(result);
        }

        // POST: api/budgets
        [HttpPost]
        public async Task<IActionResult> UpsertBudget([FromBody] BudgetDto dto)
        {
            if (dto.Amount < 0)
                return BadRequest(new { message = "Nominal anggaran tidak boleh negatif." });

            int userId = GetUserId();

            // Pastikan kategori milik user yang sedang login
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);

            if (category == null)
            {
                return BadRequest(new { message = "Kategori tidak ditemukan atau bukan milik akun Anda." });
            }

            var existingBudget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == dto.CategoryId && b.Month == dto.Month && b.Year == dto.Year);

            if (existingBudget != null)
            {
                existingBudget.Amount = dto.Amount;
            }
            else
            {
                var newBudget = new Budget
                {
                    UserId = userId,
                    CategoryId = dto.CategoryId,
                    Amount = dto.Amount,
                    Month = dto.Month,
                    Year = dto.Year
                };
                _context.Budgets.Add(newBudget);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Anggaran berhasil disimpan." });
        }

        // DELETE: api/budgets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            int userId = GetUserId();
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
                return NotFound(new { message = "Anggaran tidak ditemukan." });

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Anggaran berhasil dihapus." });
        }
    }

    public class BudgetDto
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}