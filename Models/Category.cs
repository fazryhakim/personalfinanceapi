using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // Contoh: Makanan, Transport

        [Required]
        public TransactionType Type { get; set; } // Income atau Expense

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}