using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Models;

namespace PersonalFinanceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<SavingsGoal> SavingsGoals { get; set; }

        public DbSet<RecurringTransaction> RecurringTransactions { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mencegah multiple cascade paths pada SQL Server
            modelBuilder.Entity<Budget>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RecurringTransaction>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}