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

        public decimal MaxMonthlyAmt { get => maxMonthlyAmt; set => maxMonthlyAmt = value; }
        public decimal MaxMonthlyPct { get => maxMonthlyPct; set => maxMonthlyPct = Math.Min(1, value); }
        public decimal MinMonthlyAmt { get => minMonthlyAmt; set => minMonthlyAmt = value; }
        public decimal MinMonthlyPct { get => minMonthlyPct; set => minMonthlyPct = Math.Min(1, value); }

        private decimal maxMonthlyAmt = decimal.MaxValue;
        private decimal maxMonthlyPct = 1;
        private decimal minMonthlyAmt = 0;
        private decimal minMonthlyPct = 0;

        public FlexibleBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, List<int>? childBudgets = null) : base(id, name, priority, associatedFinancialAccount, childBudgets)
        {
        }

        public override decimal MinDepositAmount(decimal referenceDeposit, int numMonths = 1)
        {
            return Math.Max(minMonthlyPct * referenceDeposit, minMonthlyAmt) * numMonths;
        }

        public override decimal MaxDepositAmount(decimal referenceDeposit, int numMonths = 1) => Math.Min(maxMonthlyPct * referenceDeposit, maxMonthlyAmt) * numMonths;
        public override decimal RecommendedDepositAmount(decimal referenceDeposit, int numMonths = 1) => MinDepositAmount(referenceDeposit, numMonths);

        public override ViewModelBase ToViewModel() => new FlexibleBudgetViewModel(this);
    }
}
