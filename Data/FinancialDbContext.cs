using FinancialCalculator.Models;
using FinancialCalculator.Models.Budgets.Fixed;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FinancialCalculator.Data
{
    public class FinancialDbContext : DbContext
    {
        private readonly string _dbPath;

        public DbSet<FinancialInstitution> FinancialInstitutions { get; set; } = null!;
        public DbSet<FinancialAccount> FinancialAccounts { get; set; } = null!;
        public DbSet<Budget> Budgets { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        public FinancialDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // FinancialInstitution PK
            modelBuilder.Entity<FinancialInstitution>()
                .HasKey(fi => fi.Id);

            modelBuilder.Entity<FinancialInstitution>()
                .Ignore(fi => fi.financialAccounts);

            // FinancialAccount — manually assigned IDs, not auto-generated
            modelBuilder.Entity<FinancialAccount>()
                .HasKey(a => a.ID);
            modelBuilder.Entity<FinancialAccount>()
                .Property(a => a.ID)
                .ValueGeneratedNever();
            modelBuilder.Entity<FinancialAccount>()
                .Property(a => a.accountName).HasColumnName("AccountName");
            modelBuilder.Entity<FinancialAccount>()
                .Property(a => a.financialInstitutionID).HasColumnName("FinancialInstitutionId");
            modelBuilder.Entity<FinancialAccount>()
                .Property(a => a.accountType).HasColumnName("AccountType");
            modelBuilder.Entity<FinancialAccount>()
                .Property(a => a.currentBalance).HasColumnName("CurrentBalance");
            modelBuilder.Entity<FinancialAccount>()
                .Property(a => a.isPreTaxAccount).HasColumnName("IsPreTaxAccount");

            // Budget — manually assigned IDs, TPH discriminator
            modelBuilder.Entity<Budget>()
                .HasKey(b => b.ID);
            modelBuilder.Entity<Budget>()
                .Property(b => b.ID)
                .ValueGeneratedNever();

            modelBuilder.Entity<Budget>()
                .HasDiscriminator<string>("BudgetDiscriminator")
                .HasValue<FixedBudget>("Fixed")
                .HasValue<RecurringExpenseBudget>("RecurringExpense")
                .HasValue<OneTimeExpenseBudget>("OneTime")
                .HasValue<FlexibleBudget>("Flexible")
                .HasValue<SavingsBudget>("Savings");

            // Store ChildBudgets as JSON string
            var jsonOptions = new JsonSerializerOptions();
            modelBuilder.Entity<Budget>()
                .Property(b => b.ChildBudgets)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<int>>(v, jsonOptions) ?? new List<int>()
                );

            // Ignore navigation property — we wire it manually after loading
            modelBuilder.Entity<Budget>()
                .Ignore(b => b.AssociatedFinancialAccount);

            // SavingsBudget: store NodaTime LocalDate as string "yyyy-MM-dd"
            modelBuilder.Entity<SavingsBudget>()
                .Property(b => b.GoalDate)
                .HasConversion(
                    v => v.ToString(),
                    v => NodaTime.LocalDate.FromDateTime(System.DateTime.Parse(v))
                );

            // Transaction
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.Id);
        }
    }
}
