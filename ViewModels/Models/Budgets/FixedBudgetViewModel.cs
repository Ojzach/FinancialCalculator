using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class FixedBudgetViewModel : BudgetViewModel
    {

        public override string BudgetType => "Fixed Budget";
        private FixedBudget _fixedBudget => _budget as FixedBudget;

        public bool IsSetByAmt { get => _fixedBudget.IsSetByAmt; set { _fixedBudget.IsSetByAmt = value; OnPropertyChanged("IsSetByAmt"); } }

        public float SetMthlyDepositAmt { get => _fixedBudget.BudgetMonthlyAmt; set { _fixedBudget.BudgetMonthlyAmt = value; } }
        public float SetMthlyDepositPct { get => _fixedBudget.BudgetMonthlyPct; set { _fixedBudget.BudgetMonthlyPct = value; } }

        public FixedBudgetViewModel(FixedBudget budget) : base(budget)
        {
        }
    }
}
