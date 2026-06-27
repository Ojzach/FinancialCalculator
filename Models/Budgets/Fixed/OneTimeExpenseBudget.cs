using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models.Budgets.Fixed
{
    public class OneTimeExpenseBudget : FixedBudget
    {
        public override string BudgetType { get => "One-Time Expense"; }

        public decimal ExpenseAmt { get; set; } = 0m;

        public OneTimeExpenseBudget() { }

        public OneTimeExpenseBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, decimal _expenseAmt = 0, List<int>? childBudgets = null) : base(id, name, priority, associatedFinancialAccount, childBudgets: childBudgets)
        {
            ExpenseAmt = _expenseAmt;
        }

        public override decimal MinDepositAmount(decimal referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override decimal MaxDepositAmount(decimal referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override decimal RecommendedDepositAmount(decimal referenceDeposit, int numMonths = 1) => ExpenseAmt;

        public override ViewModelBase ToViewModel() => new OneTimeExpenseViewModel(this);
    }
}
