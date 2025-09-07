using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class FixedBudgetDepositViewModel : BudgetDepositViewModel
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


        public FixedBudgetDepositViewModel(FixedBudget _budget, DepositStore _depositStore) : base(_budget, _depositStore)
        {

            if (!budget.IsSetByAmt) depositAmtPct.SetPct(budget.BudgetMonthlyPct, TotalDepositAmt);



            if (budget.ChildBudgets.Count > 0)
            {
                UnallocattedBudget = new FlexiblebudgetDepositViewModel(new FillBudget("Unallocated " + budget.Name, budget.AssociatedFinancialAccount), _depositStore);
                SubItems.Add(UnallocattedBudget);
            }

        }
    }
}
