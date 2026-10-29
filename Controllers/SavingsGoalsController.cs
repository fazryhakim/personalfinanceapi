using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Data;
using PersonalFinanceAPI.Models;
using System.Security.Claims;

namespace PersonalFinanceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsGoalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SavingsGoalsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavingsGoal>>> GetSavingsGoals()
        {
            int userId = GetUserId();
            return await _context.SavingsGoals
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SavingsGoal>> PostSavingsGoal(SavingsGoal goal)
        {
            goal.UserId = GetUserId();
            _context.SavingsGoals.Add(goal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSavingsGoals), new { id = goal.Id }, goal);
        }

        // Endpoint untuk menambah/menabung saldo ke target
        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> DepositToGoal(int id, [FromBody] DepositDto dto)
        {
            int userId = GetUserId();
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal == null) return NotFound();

            goal.CurrentAmount += dto.Amount;
            await _context.SaveChangesAsync();

            return Ok(goal);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSavingsGoal(int id)
        {
            int userId = GetUserId();
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal == null) return NotFound();

            _context.SavingsGoals.Remove(goal);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class DepositDto
    {
        public decimal Amount { get; set; }
    }
}