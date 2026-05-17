using System.Data.Entity;
using SmartSpend.Models;

namespace SmartSpend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("SmartSpendDB")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Prevent cascade delete conflicts
            modelBuilder.Entity<Transaction>()
                .HasRequired(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                .HasRequired(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Budget>()
                .HasRequired(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Budget>()
                .HasRequired(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Category>()
                .HasRequired(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Subscription>()
                .HasRequired(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<SavingsGoal>()
                .HasRequired(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notification>()
                .HasRequired(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}