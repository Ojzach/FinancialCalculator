using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public abstract class Budget
    {

        public string Name = "";
        public abstract string BudgetType { get; }
        public FinancialAccount AssociatedFinancialAccount;
        public List<Budget> ChildBudgets = new List<Budget>();

        public float CurrentBudgetBalance { get => localCurrentBudgetBalance + ChildBudgets.Sum(budget => budget.CurrentBudgetBalance); }
        private float localCurrentBudgetBalance = 0;


        public List<Transaction> BudgetTransactions = new List<Transaction>();


        public Budget(string name, FinancialAccount associatedFinancialAccount)
        {
            Name = name;
            AssociatedFinancialAccount = associatedFinancialAccount;
        }

        public abstract float GetMinMonthlyDepositAmt(float totalDeposit = 0);
        public abstract float GetMaxMonthlyDepositAmt(float totalDeposit = 0);
        public abstract float GetRecommendedMonthlyDepositAmt(float totalDeposit = 0);


        public abstract ViewModelBase ToViewModel();
    }


    public enum BudgetType
    {
        FixedAmount,
        VariableAmount,
        SavingsGoal
    }

    public enum BudgetPriority
    {
        None,
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    }
}
