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

        public int ID { get; private set; }
        public string Name = "";
        public abstract string BudgetType { get; }
        public FinancialAccount AssociatedFinancialAccount;
        public List<int> ChildBudgets = new List<int>();

        public float CurrentBudgetBalance => localCurrentBudgetBalance;
        private float localCurrentBudgetBalance = 0;

        public Budget(int id, string name, FinancialAccount associatedFinancialAccount)
        {
            ID = id;
            Name = name;
            AssociatedFinancialAccount = associatedFinancialAccount;
        }

        public void AddChildBudget(int budgetID) => ChildBudgets.Add(budgetID);
        public void AddChildBudget(List<int> budgetIDs) => ChildBudgets.AddRange(budgetIDs);

        public abstract float GetMinMonthlyDepositAmt(float totalDeposit = 0);
        public abstract float GetMaxMonthlyDepositAmt(float totalDeposit = 0);
        public abstract float GetRecommendedMonthlyDepositAmt(float totalDeposit = 0);


        public abstract ViewModelBase ToViewModel();
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
