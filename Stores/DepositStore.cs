using FinancialCalculator.Models;
using FinancialCalculator.Services;
using System.Diagnostics;
using System.Net;

namespace FinancialCalculator.Stores
{
    internal class DepositStore
    {

        private float depositAmount = 0;
        public float DepositAmount 
        { 
            get => depositAmount;
            set
            {
                depositAmount = MathF.Max(0, value);
                PublishDepositChanged(depositService.AllocateWholeDeposit());
            }
        }

        public float TakeHomeAmount 
        { 
            get => MathF.Max(0, DepositAmount - depositDeductions.Sum(deduction => deduction.DepositAmtPct.Amount));
        }


        private float estimatedYearlyIncome = 0;
        public float EstimatedyearlyIncome 
        { 
            get => estimatedYearlyIncome; 
            set => estimatedYearlyIncome = MathF.Max(0, value); 
        }

        public int MonthsCoveredByDeposit 
        {
            get => estimatedYearlyIncome == 0 ? 0 : (int)MathF.Ceiling((depositAmount / estimatedYearlyIncome) * 12);
        }


        public BudgetStore budgetStore;
        private DepositAllocationService depositService;
        public IReadOnlyDictionary<int, BudgetDeposit> BudgetDeposits => deposits;


        public event Action<List<int>>? DepositsChanged;
        
        private Dictionary<int, BudgetDeposit> deposits = new();
        private List<BudgetDeposit> depositDeductions = new();


        public DepositStore(BudgetStore _budgetStore)
        {
            budgetStore = _budgetStore;
            depositService = new DepositAllocationService(this, budgetStore);

            depositDeductions.Add(new BudgetDeposit(-2, new AmountPercentModel(() => DepositAmount, initialPercent: 0.145f)));
            depositDeductions.Add(new BudgetDeposit(-3, new AmountPercentModel(() => DepositAmount, initialPercent: 0.014f)));
            depositDeductions.Add(new BudgetDeposit(-4, new AmountPercentModel(() => DepositAmount,initialPercent: 0.062f)));
            depositDeductions.Add(new BudgetDeposit(-5, new AmountPercentModel(() => DepositAmount, initialPercent: 0.0f)));

            foreach (Budget budget in budgetStore.Budgets.Values)
            {
                if(budgetStore.IsBudgetPreTax(budget.ID)) deposits.Add(budget.ID, new BudgetDeposit(budget.ID, new AmountPercentModel(() => DepositAmount)));
                else deposits.Add(budget.ID, new BudgetDeposit(budget.ID, new AmountPercentModel(() => TakeHomeAmount)));

            }


            foreach (int budget in budgetStore.Budgets.Keys)
            {
                foreach(int childBudget in budgetStore.Budgets[budget].ChildBudgets)
                {
                    deposits[childBudget].SetParent(budget);
                }
            }

        }

        private void PublishDepositChanged(List<int> depositsChanged)
        {
            DepositsChanged?.Invoke(depositsChanged);
        }


        public float GetBudgetDepositAmount(int depositID)
        {
            return deposits[depositID].DepositAmtPct.Amount;
        }
        public float GetBudgetReferenceAmount(int depositID) => budgetStore.IsBudgetPreTax(depositID) ? DepositAmount : TakeHomeAmount;

        public float SetBudgetDepositAmt(int depositID, float amount) => deposits[depositID].DepositAmtPct.Amount = amount;

    }
}
