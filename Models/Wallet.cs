using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- Tambahkan ini

namespace PersonalFinanceAPI.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // Contoh: BCA, Cash, GoPay

        [Column(TypeName = "decimal(18,2)")] // <-- Tambahkan ini untuk presisi keuangan
        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}