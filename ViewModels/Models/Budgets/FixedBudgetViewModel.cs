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

        public bool IsSetByAmt { 
            get => _fixedBudget.IsSetByAmount; 
            set 
            {
                _fixedBudget.IsSetByAmount = value;
                OnPropertyChanged(nameof(IsSetByAmt));
            } 
        }

        public float SetDepositAmt { get => _fixedBudget.SetAmount; set { _fixedBudget.SetAmount = value; } }
        public float SetDepositPct { get => _fixedBudget.SetPercent; set { _fixedBudget.SetPercent = value; } }

        public FixedBudgetViewModel(FixedBudget budget) : base(budget)
        {
        }
    }
}
