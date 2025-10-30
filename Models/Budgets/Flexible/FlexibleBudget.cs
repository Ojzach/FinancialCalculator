using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class FlexibleBudget : Budget
    {

        public override string BudgetType { get => "Flexible"; }

        public float MaxMonthlyAmt { get => maxMonthlyAmt; set => maxMonthlyAmt = value; }
        public float MaxMonthlyPct { get => maxMonthlyPct; set => maxMonthlyPct = MathF.Min(1, value); }
        public float MinMonthlyAmt { get => minMonthlyAmt; set => minMonthlyAmt = value; }
        public float MinMonthlyPct { get => minMonthlyPct; set => minMonthlyPct = MathF.Min(1, value); }

        private float maxMonthlyAmt = float.MaxValue;
        private float maxMonthlyPct = 1;
        private float minMonthlyAmt = 0;
        private float minMonthlyPct = 0;

        public FlexibleBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, List<int>? childBudgets = null) : base(id, name, priority, associatedFinancialAccount, childBudgets)
        {
        }

        public override float MinDepositAmount(float referenceDeposit, int numMonths = 1)
        {
            return MathF.Max(minMonthlyPct * referenceDeposit, minMonthlyAmt) * numMonths;
        }

        public override float MaxDepositAmount(float referenceDeposit, int numMonths = 1) => MathF.Min(maxMonthlyPct * referenceDeposit, maxMonthlyAmt) * numMonths;
        public override float RecommendedDepositAmount(float referenceDeposit, int numMonths = 1) => MinDepositAmount(referenceDeposit, numMonths);

        public override ViewModelBase ToViewModel() => new FlexibleBudgetViewModel(this);
    }
}
