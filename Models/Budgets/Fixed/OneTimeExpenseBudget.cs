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
        public override string BudgetType { get => "Recurring Expense"; }

        public float ExpenseAmt = 0f;

        public OneTimeExpenseBudget(int id, string name, FinancialAccount associatedFinancialAccount, float _expenseAmt = 0) : base(id, name, associatedFinancialAccount)
        {
            ExpenseAmt = _expenseAmt;
        }

        public override float MinDepositAmount(float referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float MaxDepositAmount(float referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float RecommendedDepositAmount(float referenceDeposit, int numMonths = 1) => ExpenseAmt;

        public override ViewModelBase ToViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
