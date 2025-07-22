using FinancialCalculator.Models;
using System.Collections.ObjectModel;
using NodaTime;

namespace FinancialCalculator.ViewModels
{
    public class DepositCalculatorBudgetViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> subItems = new ObservableCollection<ViewModelBase>();
        public ObservableCollection<ViewModelBase> SubItems { get => subItems; set { subItems = value; OnPropertyChanged(nameof(SubItems)); } }


        private Budget budget;

        public string BudgetName { get => budget.Name; }


        public DepositCalculatorBudgetViewModel(Budget _budget)
        {
            budget = _budget;

            foreach (Budget childBudget in budget.ChildBudgets) SubItems.Add(new DepositCalculatorBudgetViewModel(childBudget));
            SubItems.Add(new TransactionViewModel(new Transaction("General " + budget.Name, true, 0, LocalDateTime.FromDateTime(DateTime.UtcNow), new FinancialAccount("", BankAccountType.Checking), budget)));
        }
    }
}
