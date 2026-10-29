using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models
{
    public enum TransactionType
    {
        Income,   // Pemasukan
        Expense   // Pengeluaran
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;

        // Foreign Key ke Dompet
        public int WalletId { get; set; }
        public Wallet? Wallet { get; set; }

        // Ubah menjadi int? agar opsional dan tidak error 500
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}