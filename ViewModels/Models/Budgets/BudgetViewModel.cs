using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public float CurrentBalance { get => _budget.CurrentBudgetBalance; }


        public BudgetPriority SelectedBudgetPriority { get => _budget.Priority; set => _budget.Priority = value; }
        
        private ObservableCollection<BudgetPriority> budgetPriorities = new(Enum.GetValues(typeof(BudgetPriority)).Cast<BudgetPriority>().ToList());
        public ObservableCollection<BudgetPriority> BudgetPriorities => budgetPriorities;

        public BudgetViewModel(Budget budget)
        {
            _budget = budget;
        }
    }
}
