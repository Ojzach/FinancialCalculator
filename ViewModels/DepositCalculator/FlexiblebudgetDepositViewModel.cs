using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class FlexiblebudgetDepositViewModel : BudgetDepositViewModel
    {

        protected FlexibleBudget budget { get => _budget as FlexibleBudget; }

        public override float MaxAmt { 
            get
            {

                if(IsUsrSet) return DepositAmt;
                else if (SubItems.Count > 0) return MathF.Min(_budget.GetMaxMonthlyDepositAmt(_deposit.GetDepositAmount(_budget.AssociatedFinancialAccount.isPreTaxAccount)), SubItems.Sum(budget => budget.MaxAmt));
                else return _budget.GetMaxMonthlyDepositAmt(_deposit.GetDepositAmount(_budget.AssociatedFinancialAccount.isPreTaxAccount));
            } 
        }

        public FlexiblebudgetDepositViewModel(FlexibleBudget _budget, DepositStore _depositStore) : base(_budget, _depositStore)
        {

            DepositAmt = BudgetRecommendedAmtPerMonth;

            if (budget.ChildBudgets.Count > 0)
            {
                
                UnallocattedBudget = new FlexiblebudgetDepositViewModel(new FlexibleBudget("Unallocated " + budget.Name, budget.AssociatedFinancialAccount), _depositStore);
                SubItems.Add(UnallocattedBudget);


                //RebalanceSubItems();
            }
        }
    }
}
