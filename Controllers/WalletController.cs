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
    public class WalletsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WalletsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        // GET: api/wallets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetWallets()
        {
            int userId = GetUserId();
            return await _context.Wallets
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        // POST: api/wallets
        [HttpPost]
        public async Task<ActionResult<Wallet>> PostWallet(Wallet wallet)
        {
            wallet.UserId = GetUserId();
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWallets), new { id = wallet.Id }, wallet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            int userId = GetUserId();
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
                
            if (wallet == null) return NotFound();

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest(new { message = "Nominal transfer harus lebih besar dari 0." });

            if (dto.SourceWalletId == dto.TargetWalletId)
                return BadRequest(new { message = "Dompet asal dan tujuan tidak boleh sama." });

            int userId = GetUserId();
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sourceWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.Id == dto.SourceWalletId && w.UserId == userId);
                var targetWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.Id == dto.TargetWalletId && w.UserId == userId);

                if (sourceWallet == null || targetWallet == null)
                    return NotFound(new { message = "Dompet asal atau tujuan tidak ditemukan." });

                if (sourceWallet.Balance < dto.Amount)
                    return BadRequest(new { message = "Saldo dompet asal tidak mencukupi." });

                // Proses mutasi saldo
                sourceWallet.Balance -= dto.Amount;
                targetWallet.Balance += dto.Amount;

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok(new { message = "Transfer berhasil dilakukan." });
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return StatusCode(500, new { message = "Terjadi kesalahan saat memproses transfer.", error = ex.Message });
            }
        }
    }
}