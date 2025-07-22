using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class Budget
    {

        public string Name = "";
        public BudgetType Type;

        public Budget ParentBudget;
        public List<Budget> ChildBudgets = new List<Budget>();

        public float CurrentBudgetBalance { get => localCurrentBudgetBalance + ChildBudgets.Sum(budget => budget.CurrentBudgetBalance); }
        private float localCurrentBudgetBalance = 0;

        public List<Transaction> BudgetTransactions = new List<Transaction>();


        public Budget(string name)
        {
            Name = name;
        }

    }


    public enum BudgetType
    {
        FixedAmount,
        VariableAmount,
        SavingsGoal
    }
}
