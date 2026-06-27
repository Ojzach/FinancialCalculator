using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public abstract class Budget
    {

        public int ID { get; set; }
        public string Name { get; set; } = "";
        public BudgetPriority Priority { get; set; }
        public abstract string BudgetType { get; }

        // EF Core FK — set before saving
        public int AssociatedFinancialAccountId { get; set; }
        public FinancialAccount AssociatedFinancialAccount { get; set; } = null!;

        public List<int> ChildBudgets { get; set; } = new();

        // Whether this budget is a hidden deduction (taxes, etc.)
        public bool IsHidden { get; set; } = false;

        public decimal CurrentBudgetBalance { get; set; } = 0;

        public void AddToBalance(decimal amount) => CurrentBudgetBalance += amount;

        protected Budget() { }

        public Budget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, List<int>? childBudgets)
        {
            ID = id;
            Name = name;
            Priority = priority;
            AssociatedFinancialAccount = associatedFinancialAccount;
            AssociatedFinancialAccountId = associatedFinancialAccount.ID;
            ChildBudgets = childBudgets == null ? new List<int>() : childBudgets;
        }

        public abstract decimal MinDepositAmount(decimal referenceDeposit, int numMonths = 1);
        public abstract decimal MaxDepositAmount(decimal referenceDeposit, int numMonths = 1);
        public abstract decimal RecommendedDepositAmount(decimal referenceDeposit, int numMonths = 1);


        public abstract ViewModelBase ToViewModel();
    }

    public enum BudgetPriority
    {
        None = 1,
        VeryLow = 2,
        Low = 3,
        Medium = 6,
        High = 9,
        VeryHigh = 18
    }
}
