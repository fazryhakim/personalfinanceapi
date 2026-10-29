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
    public class RecurringTransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecurringTransactionsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        // GET: api/RecurringTransactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurringTransaction>>> GetRecurringTransactions()
        {
            int userId = GetUserId();
            return await _context.RecurringTransactions
                .Where(r => r.UserId == userId)
                .Include(r => r.Wallet)
                .Include(r => r.Category)
                .ToListAsync();
        }

        // POST: api/RecurringTransactions
        [HttpPost]
        public async Task<ActionResult<RecurringTransaction>> PostRecurringTransaction(RecurringTransaction recurring)
        {
            int userId = GetUserId();
            recurring.UserId = userId;

            // Pastikan dompet milik user yang sedang login
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == recurring.WalletId && w.UserId == userId);

            if (wallet == null)
            {
                return BadRequest(new { message = "Dompet tidak ditemukan atau bukan milik akun Anda." });
            }

            _context.RecurringTransactions.Add(recurring);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRecurringTransactions), new { id = recurring.Id }, recurring);
        }

        // DELETE: api/RecurringTransactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecurringTransaction(int id)
        {
            int userId = GetUserId();
            var recurring = await _context.RecurringTransactions
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (recurring == null) return NotFound();

            _context.RecurringTransactions.Remove(recurring);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/RecurringTransactions/process-due
        [HttpPost("process-due")]
        public async Task<IActionResult> ProcessDueTransactions()
        {
            int userId = GetUserId();
            var today = DateTime.Today;
            var currentDay = today.Day;

            var recurrings = await _context.RecurringTransactions
                .Include(r => r.Wallet)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            int processedCount = 0;

            foreach (var rec in recurrings)
            {
                bool isDueDay = currentDay >= rec.DayOfMonth;
                bool notProcessedThisMonth = rec.LastProcessedDate == null ||
                    rec.LastProcessedDate.Value.Month != today.Month ||
                    rec.LastProcessedDate.Value.Year != today.Year;

                if (isDueDay && notProcessedThisMonth)
                {
                    // 1. Buat transaksi baru secara otomatis dengan UserId yang sesuai
                    var transaction = new Transaction
                    {
                        UserId = userId,
                        Description = rec.Title,
                        Amount = rec.Amount,
                        Type = (TransactionType)rec.Type, // <-- Tambahkan cast explicit di sini
                        Date = new DateTime(today.Year, today.Month, rec.DayOfMonth),
                        WalletId = rec.WalletId,
                        CategoryId = rec.CategoryId
                    };

                    // 2. Perbarui saldo dompet
                    var wallet = await _context.Wallets
                        .FirstOrDefaultAsync(w => w.Id == rec.WalletId && w.UserId == userId);

                    if (wallet != null)
                    {
                        if (rec.Type == 0) // Pemasukan
                            wallet.Balance += rec.Amount;
                        else // Pengeluaran
                            wallet.Balance -= rec.Amount;
                    }

                    // 3. Update tanggal terakhir diproses
                    rec.LastProcessedDate = today;
                    processedCount++;
                }
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Berhasil memproses {processedCount} transaksi berulang." });
        }
    }
}