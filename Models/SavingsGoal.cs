using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- Tambahkan namespace ini

namespace PersonalFinanceAPI.Models
{
    public class SavingsGoal
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")] // <-- Atur presisi SQL
        public decimal TargetAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")] // <-- Atur presisi SQL
        public decimal CurrentAmount { get; set; } = 0;

        public DateTime? TargetDate { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}