using FinancialCalculator.Models;
using FinancialCalculator.Models.Budgets.Fixed;
using Microsoft.EntityFrameworkCore;

namespace FinancialCalculator.Data
{
    /// <summary>
    /// Handles first-run seeding and loading of domain models from the database.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void EnsureCreated(FinancialDbContext db)
        {
            db.Database.EnsureCreated();

            if (!db.FinancialInstitutions.Any())
                SeedDefaultData(db);
        }

        /// <summary>
        /// Loads all institutions and accounts from the DB and wires them together.
        /// Returns (institutions list, accounts dictionary by ID).
        /// </summary>
        public static (List<FinancialInstitution> institutions, Dictionary<int, FinancialAccount> accounts)
            LoadAccountsAndInstitutions(FinancialDbContext db)
        {
            var dbAccounts = db.FinancialAccounts.ToList();
            var dbInstitutions = db.FinancialInstitutions.ToList();

            var accountsById = dbAccounts.ToDictionary(a => a.ID);

            foreach (var inst in dbInstitutions)
            {
                var instAccounts = dbAccounts
                    .Where(a => a.financialInstitutionID == inst.Id)
                    .ToList();
                inst.financialAccounts.Clear();
                foreach (var a in instAccounts)
                    inst.financialAccounts.Add(a);
            }

            return (dbInstitutions, accountsById);
        }

        /// <summary>
        /// Loads all budgets from the DB and wires AssociatedFinancialAccount references.
        /// Returns (active budgets, hidden/deduction budgets).
        /// </summary>
        public static (List<Budget> budgets, List<Budget> hiddenBudgets)
            LoadBudgets(FinancialDbContext db, Dictionary<int, FinancialAccount> accountsById)
        {
            var allBudgets = db.Budgets.ToList();

            foreach (var budget in allBudgets)
            {
                if (accountsById.TryGetValue(budget.AssociatedFinancialAccountId, out var account))
                    budget.AssociatedFinancialAccount = account;
            }

            var active = allBudgets.Where(b => !b.IsHidden).ToList();
            var hidden = allBudgets.Where(b => b.IsHidden).ToList();
            return (active, hidden);
        }

        private static void SeedDefaultData(FinancialDbContext db)
        {
            // Seed institutions
            var usaa = new FinancialInstitution("USAA") { Id = 1 };
            var discover = new FinancialInstitution("Discover") { Id = 2 };
            var fidelity = new FinancialInstitution("Fidelity") { Id = 3 };
            db.FinancialInstitutions.AddRange(usaa, discover, fidelity);

            // Seed accounts
            var accounts = new List<FinancialAccount>
            {
                new FinancialAccount(0, "Income",         usaa.Id,     BankAccountType.Checking,    2000),
                new FinancialAccount(1, "Spending",       usaa.Id,     BankAccountType.Checking,    1000),
                new FinancialAccount(2, "Credit Card",    usaa.Id,     BankAccountType.Credit,     -1000),
                new FinancialAccount(3, "Emergency Fund", discover.Id, BankAccountType.Savings,    10000),
                new FinancialAccount(4, "State Tax",      discover.Id, BankAccountType.Savings,     5000, _isPreTaxAccount: true),
                new FinancialAccount(5, "Roth IRA",       fidelity.Id, BankAccountType.Investment, 10000),
                new FinancialAccount(6, "401K",           fidelity.Id, BankAccountType.Investment, 10000, _isPreTaxAccount: true),
            };
            db.FinancialAccounts.AddRange(accounts);

            var accountsById = accounts.ToDictionary(a => a.ID);

            // Seed active budgets
            var budgets = new List<Budget>
            {
                new FixedBudget(0, "Base Budget",   BudgetPriority.VeryHigh, accountsById[0], setPct: 1,    childBudgets: [1, 2, 3, 4]),
                new FixedBudget(1, "Investments",   BudgetPriority.VeryHigh, accountsById[0], setPct: 0.6m),
                new FlexibleBudget(2, "Fixed Costs", BudgetPriority.VeryHigh, accountsById[1]),
                new FixedBudget(3, "Savings",       BudgetPriority.Medium,   accountsById[0], setPct: 0.2m),
                new FlexibleBudget(4, "Free Spending", BudgetPriority.Low,   accountsById[1]),
            };

            // Seed hidden/deduction budgets
            var hiddenBudgets = new List<Budget>
            {
                new FixedBudget(100, "Federal Income Tax", BudgetPriority.VeryHigh, accountsById[4], setPct: 0.145m)  { IsHidden = true },
                new FixedBudget(101, "Medicare",           BudgetPriority.VeryHigh, accountsById[4], setPct: 0.0145m) { IsHidden = true },
                new FixedBudget(102, "Social Security",    BudgetPriority.VeryHigh, accountsById[4], setPct: 0.062m)  { IsHidden = true },
                new FixedBudget(103, "State Income Tax",   BudgetPriority.VeryHigh, accountsById[4], setPct: 0m)      { IsHidden = true },
            };

            db.Budgets.AddRange(budgets);
            db.Budgets.AddRange(hiddenBudgets);
            db.SaveChanges();
        }
    }
}
