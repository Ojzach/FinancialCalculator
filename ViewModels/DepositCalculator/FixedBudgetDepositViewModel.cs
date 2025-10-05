using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    internal class FixedBudgetDepositViewModel : BudgetDepositViewModel
    {

        protected FixedBudget budget { get => _budget as FixedBudget; }

        public override float MaxAmt { 
            get 
            {
                if (IsUsrSet) return DepositAmt;
                else
                {
                    return _budget.GetMaxMonthlyDepositAmt(_deposit.GetDepositAmount(_budget.AssociatedFinancialAccount.isPreTaxAccount));
                }
            } 
        }

        public override float MinAmt
        {
            get
            {
                if (IsUsrSet) return DepositAmt;
                else
                {
                    return _budget.GetMinMonthlyDepositAmt(_deposit.GetDepositAmount(_budget.AssociatedFinancialAccount.isPreTaxAccount));
                }
            }
        }


        public FixedBudgetDepositViewModel(FixedBudget _budget, BudgetStore budgetStore, DepositStore _depositStore) : base(_budget, budgetStore, _depositStore)
        {

            if (!budget.IsSetByAmt) depositAmtPct.Percent = budget.BudgetMonthlyPct;



            if (budget.ChildBudgets.Count > 0)
            {
                UnallocattedBudget = new FlexiblebudgetDepositViewModel(new FillBudget(-1, "Unallocated " + budget.Name, budget.AssociatedFinancialAccount), budgetStore, _depositStore);
                SubItems.Add(UnallocattedBudget);
            }

        }
    }
}
