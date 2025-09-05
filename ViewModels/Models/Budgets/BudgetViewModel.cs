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

        public BudgetViewModel(Budget budget)
        {
            _budget = budget;
        }
    }
}
