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


        public int BudgetID => _budget.ID;
        public string BudgetName { get => _budget.Name; set { _budget.Name = value; OnPropertyChanged(nameof(BudgetName)); } }
        public string AssociatedFinancialAccountName => _budget.AssociatedFinancialAccount?.accountName ?? "N/A";

        public abstract string BudgetType { get; }

        public decimal CurrentBalance { get => _budget.CurrentBudgetBalance; }


        public BudgetPriority SelectedBudgetPriority { get => _budget.Priority; set => _budget.Priority = value; }
        
        private ObservableCollection<BudgetPriority> budgetPriorities = new(Enum.GetValues(typeof(BudgetPriority)).Cast<BudgetPriority>().ToList());
        public ObservableCollection<BudgetPriority> BudgetPriorities => budgetPriorities;

        public BudgetViewModel(Budget budget)
        {
            _budget = budget;
        }
    }
}
