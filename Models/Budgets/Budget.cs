using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public abstract class Budget
    {

        public int ID { get; private set; }
        public string Name = "";
        public BudgetPriority Priority;
        public abstract string BudgetType { get; }
        public FinancialAccount AssociatedFinancialAccount;
        public List<int> ChildBudgets;

        public decimal CurrentBudgetBalance => localCurrentBudgetBalance;
        private decimal localCurrentBudgetBalance = 0;

        public Budget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, List<int>? childBudgets)
        {
            ID = id;
            Name = name;
            Priority = priority;
            AssociatedFinancialAccount = associatedFinancialAccount;
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
