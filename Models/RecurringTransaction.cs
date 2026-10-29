using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models
{
    public class RecurringTransaction
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int Type { get; set; } // 0 = Pemasukan, 1 = Pengeluaran

        public int WalletId { get; set; }
        public Wallet? Wallet { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public int DayOfMonth { get; set; } // Tanggal eksekusi rutin setiap bulan (1 - 31)

        public DateTime? LastProcessedDate { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}