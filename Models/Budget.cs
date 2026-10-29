using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models
{
    public class Budget
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Batas maksimal anggaran

        [Required]
        public int Month { get; set; } // 1 - 12

        [Required]
        public int Year { get; set; } // Contoh: 2026

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}