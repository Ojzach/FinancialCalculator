using FinancialCalculator.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Stores
{
    internal class BudgetStore
    {
        public Dictionary<int, Budget> Budgets { get => budgets; }
        public Dictionary<int, Budget> HiddenBudgets { get => hiddenBudgets; }


        Dictionary<int, Budget> budgets = new Dictionary<int, Budget>();
        Dictionary<int, Budget> hiddenBudgets = new Dictionary<int, Budget>();

        public BudgetStore(FinancialInstitutionsStore financialInstitutionsStore, List<Budget> budgets = null, List<Budget> hiddenBudgets = null)
        {
            if (budgets != null)
                foreach (Budget budget in budgets) this.budgets.Add(budget.ID, budget);

            if(hiddenBudgets != null)
                foreach (Budget hiddenBudget in hiddenBudgets) this.hiddenBudgets.Add(hiddenBudget.ID, hiddenBudget);
        }

        public Budget GetBudget(int budgetID)
        {
            if(budgets.ContainsKey(budgetID)) return budgets[budgetID];
            else if(hiddenBudgets.ContainsKey(budgetID)) return hiddenBudgets[budgetID];

            throw new Exception("Budget ID does not exist in the Budget Store");
        }

        public bool IsBudgetPreTax(int budgetID) => GetBudget(budgetID).AssociatedFinancialAccount.isPreTaxAccount;
    }
}
