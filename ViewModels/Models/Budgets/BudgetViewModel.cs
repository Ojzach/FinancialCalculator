using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public abstract class BudgetViewModel : ViewModelBase
    {
        protected Budget _budget;


        public string BudgetName { get => _budget.Name; private set => _budget.Name = value; }
        public abstract string BudgetType { get; }
        public string AssociatedFinancialAccountName { get => _budget.AssociatedFinancialAccount.accountName; }

        public float CurrentBalance { get => _budget.CurrentBudgetBalance; }

        public string DebugInfo { get => "Recommended MD: $" + _budget.GetRecommendedMonthlyDepositAmt() + "  Min MD: $" + _budget.GetMinMonthlyDepositAmt() + "  Max MD: $" + _budget.GetMaxMonthlyDepositAmt();  }

        public BudgetViewModel(Budget budget)
        {
            _budget = budget;
        }
    }
}
