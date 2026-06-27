using FinancialCalculator.ViewModels;
using NodaTime;
using NodaTime.Extensions;
using System.Diagnostics;

namespace FinancialCalculator.Models
{
    public class SavingsBudget : FlexibleBudget
    {

        public override string BudgetType { get => "Savings Goal"; }


        private decimal savingsGoalAmount;
        private LocalDate goalDate = DateTime.Now.ToLocalDateTime().Date;

        public decimal SavingsGoalAmount
        {
            get => savingsGoalAmount;
            set => savingsGoalAmount = value;
        }

        public LocalDate GoalDate
        {
            get => goalDate;
            set => goalDate = value;
        }

        private int MonthsTillGoalDate
        {
            get
            {
                Period timeSpan = (goalDate - DateTime.Now.ToLocalDateTime().Date);
                return (timeSpan.Months + timeSpan.Years * 12) + 1;
            }
        }


        public SavingsBudget() { }

        public SavingsBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, decimal _savingsGoalAmt = 0m, LocalDate _goalDate = default, List<int>? childBudgets = null) : base(id, name, priority, associatedFinancialAccount, childBudgets: childBudgets)
        {
            savingsGoalAmount = _savingsGoalAmt;
            goalDate = _goalDate == default ? DateTime.Now.ToLocalDateTime().Date : _goalDate;
        }

        public override decimal MinDepositAmount(decimal referenceDeposit, int numMonths = 1) => 0;
        public override decimal MaxDepositAmount(decimal referenceDeposit, int numMonths = 1) => savingsGoalAmount - CurrentBudgetBalance;
        public override decimal RecommendedDepositAmount(decimal referenceDeposit, int numMonths = 1)
        {
            int MonthsLeft = MonthsTillGoalDate;
            if (MonthsLeft > 1) return (savingsGoalAmount / MonthsLeft) * numMonths;
            else return savingsGoalAmount - CurrentBudgetBalance;

        }

        public override ViewModelBase ToViewModel() => new SavingsBudgetViewModel(this);
    }
}
