using FinancialCalculator.ViewModels;
using NodaTime;
using NodaTime.Extensions;

namespace FinancialCalculator.Models
{
    public class SavingsBudget : FlexibleBudget
    {

        public override string BudgetType { get => "Savings Goal"; }


        private float savingsGoalAmount;
        private LocalDate goalDate = DateTime.Now.ToLocalDateTime().Date;
        private BudgetPriority savingsPriority;


        private int MonthsTillGoalDate
        {
            get
            {
                Period timeSpan = (goalDate - DateTime.Now.ToLocalDateTime().Date);
                return (timeSpan.Months + timeSpan.Years * 12) + 1;
            }
        }


        public SavingsBudget(int id, string name, FinancialAccount associatedFinancialAccount, float _savingsGoalAmt = 0f, LocalDate _goalDate = default(LocalDate), BudgetPriority _priority = BudgetPriority.Low) : base(id, name, associatedFinancialAccount)
        {
            savingsGoalAmount = _savingsGoalAmt;
            goalDate = _goalDate == default(LocalDate) ? DateTime.Now.ToLocalDateTime().Date : _goalDate;
            savingsPriority = _priority;
        }

        public override float GetMinMonthlyDepositAmt(float totalDeposit = 0) => 0;
        public override float GetMaxMonthlyDepositAmt(float totalDeposit = 0) => GetRecommendedMonthlyDepositAmt();
        public override float GetRecommendedMonthlyDepositAmt(float totalDeposit = 0) => savingsGoalAmount / (float)MonthsTillGoalDate;

        public override ViewModelBase ToViewModel() => new SavingsBudgetViewModel(this);
    }
}
