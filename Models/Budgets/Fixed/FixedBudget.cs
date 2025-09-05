using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public class FixedBudget : Budget
    {
        public override string BudgetType { get => "Fixed"; }

        public bool IsSetByAmt { get; set; }
        public float BudgetMonthlyAmt { private get; set; }
        public float BudgetMonthlyPct { get; set; }


        public FixedBudget(string name, FinancialAccount associatedFinancialAccount, bool isSetByAmt = true, float setAmt = 0f, float setPct = 0.0f) : base(name, associatedFinancialAccount)
        {
            IsSetByAmt = isSetByAmt;
            BudgetMonthlyAmt = setAmt;
            BudgetMonthlyPct = setPct;
        }

        public override float GetMinMonthlyDepositAmt(float totalDeposit = 0) => GetRecommendedMonthlyDepositAmt(totalDeposit);
        public override float GetMaxMonthlyDepositAmt(float totalDeposit = 0) => GetRecommendedMonthlyDepositAmt(totalDeposit);
        public override float GetRecommendedMonthlyDepositAmt(float totalDeposit = 0) 
        {
            if (IsSetByAmt) return BudgetMonthlyAmt;
            else return BudgetMonthlyPct * totalDeposit;
        }

        public override ViewModelBase ToViewModel() => new FixedBudgetViewModel(this);
    }
}
