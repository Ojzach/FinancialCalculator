using FinancialCalculator.Data;
using FinancialCalculator.Models;

namespace FinancialCalculator.Stores
{
    internal class BudgetStore
    {
        public Dictionary<int, Budget> Budgets { get => budgets; }
        public Dictionary<int, Budget> HiddenBudgets { get => hiddenBudgets; }

        private Dictionary<int, Budget> budgets = new();
        private Dictionary<int, Budget> hiddenBudgets = new();

        private readonly FinancialDbContext _db;

        public BudgetStore(FinancialInstitutionsStore financialInstitutionsStore, FinancialDbContext db,
            List<Budget>? budgets = null, List<Budget>? hiddenBudgets = null)
        {
            _db = db;

            if (budgets != null)
                foreach (Budget budget in budgets) this.budgets.Add(budget.ID, budget);

            if (hiddenBudgets != null)
                foreach (Budget hiddenBudget in hiddenBudgets) this.hiddenBudgets.Add(hiddenBudget.ID, hiddenBudget);
        }

        public Budget GetBudget(int budgetID)
        {
            if (budgets.ContainsKey(budgetID)) return budgets[budgetID];
            else if (hiddenBudgets.ContainsKey(budgetID)) return hiddenBudgets[budgetID];

            throw new Exception("Budget ID does not exist in the Budget Store");
        }

        public bool IsBudgetPreTax(int budgetID) => GetBudget(budgetID).AssociatedFinancialAccount.isPreTaxAccount;

        /// <summary>
        /// Adds a new budget to the store and persists it to the database.
        /// </summary>
        public void AddBudget(Budget budget)
        {
            // Assign next available ID
            int newId = budgets.Keys.Any() ? budgets.Keys.Max() + 1 : 0;
            budget.ID = newId;
            budgets[newId] = budget;
            _db.Budgets.Add(budget);
            _db.SaveChanges();
        }

        /// <summary>
        /// Removes a budget from the store and deletes it from the database.
        /// </summary>
        public void DeleteBudget(int budgetID)
        {
            if (!budgets.ContainsKey(budgetID)) return;
            var budget = budgets[budgetID];
            budgets.Remove(budgetID);
            _db.Budgets.Remove(budget);
            _db.SaveChanges();
        }

        /// <summary>
        /// Persists the current state of a budget to the database.
        /// </summary>
        public void SaveBudget(int budgetID)
        {
            var budget = GetBudget(budgetID);
            _db.Budgets.Update(budget);
            _db.SaveChanges();
        }

        /// <summary>
        /// Updates the balance of a budget and saves it to the database.
        /// </summary>
        public void UpdateBudgetBalance(int budgetID, decimal depositAmount)
        {
            var budget = GetBudget(budgetID);
            budget.AddToBalance(depositAmount);
            _db.Budgets.Update(budget);
            _db.SaveChanges();
        }
    }
}
