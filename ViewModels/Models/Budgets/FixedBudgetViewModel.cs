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
            get => _fixedBudget.SetAmountPercent.IsSetByAmount; 
            set 
            {
                if (value == true && IsSetByAmt != true) _fixedBudget.SetAmountPercent.Amount = 0;
                else if (value == false && IsSetByAmt != false) _fixedBudget.SetAmountPercent.Percent = 0;

                OnPropertyChanged(nameof(IsSetByAmt));
            } 
        }

        public float SetMthlyDepositAmt { get => _fixedBudget.SetAmountPercent.GetAmount(0); set { _fixedBudget.SetAmountPercent.Amount = value; } }
        public float SetMthlyDepositPct { get => _fixedBudget.SetAmountPercent.GetPercent(0); set { _fixedBudget.SetAmountPercent.Percent = value; } }

        public FixedBudgetViewModel(FixedBudget budget) : base(budget)
        {
        }
    }
}
