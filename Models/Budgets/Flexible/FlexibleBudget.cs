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

        public FlexibleBudget(int id, string name, FinancialAccount associatedFinancialAccount) : base(id, name, associatedFinancialAccount)
        {
        }

        public override float MinDepositAmount(float referenceDeposit, int numMonths = 1) => MathF.Max(minMonthlyPct * referenceDeposit, minMonthlyAmt) * numMonths;
        public override float MaxDepositAmount(float referenceDeposit, int numMonths = 1) => MathF.Min(maxMonthlyPct * referenceDeposit, maxMonthlyAmt) * numMonths;
        public override float RecommendedDepositAmount(float referenceDeposit, int numMonths = 1) => MinDepositAmount(referenceDeposit, numMonths);

        public override ViewModelBase ToViewModel() => new FlexibleBudgetViewModel(this);
    }
}
