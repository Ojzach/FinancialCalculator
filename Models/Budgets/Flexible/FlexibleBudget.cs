using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class FlexibleBudget : Budget
    {

        public override string BudgetType { get => "Flexible"; }

        private float maxMonthlyAmt = float.MaxValue;
        private float maxMonthlyPct = 1;
        private float minMonthlyAmt = 0;
        private float minMonthlyPct = 0;

        public FlexibleBudget(string name, FinancialAccount associatedFinancialAccount) : base(name, associatedFinancialAccount)
        {
        }

        public override float GetMinMonthlyDepositAmt(float totalDeposit = 0) => MathF.Max(minMonthlyPct * totalDeposit, minMonthlyAmt);
        public override float GetMaxMonthlyDepositAmt(float totalDeposit = 0) => MathF.Min(maxMonthlyPct * totalDeposit, maxMonthlyAmt);
        public override float GetRecommendedMonthlyDepositAmt(float totalDeposit = 0) => GetMinMonthlyDepositAmt();

        public override ViewModelBase ToViewModel() => new FlexibleBudgetViewModel(this);
    }
}
