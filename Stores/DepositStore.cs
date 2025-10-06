using FinancialCalculator.Models;
using FinancialCalculator.Services;
using System.Diagnostics;
using System.Net;

namespace FinancialCalculator.Stores
{
    internal class DepositStore
    {

        public float DepositAmount { get => depositAmount; set {  depositAmount = MathF.Max(0, value); } }
        public float TakeHomeAmount { get
            {
                return MathF.Max(0, DepositAmount - depositDeductions.Sum(deduction => deduction.DepositAmtPct.GetAmount(DepositAmount)));
            }
        }


        public float EstimatedyearlyIncome { get => estimatedYearlyIncome; set { estimatedYearlyIncome = MathF.Max(0, value); } }
        public int MonthsCoveredByDeposit { get 
            { 
                if(estimatedYearlyIncome == 0) return 0;
                else return (int)MathF.Ceiling((depositAmount / estimatedYearlyIncome) * 12);
            } }


        private float depositAmount = 0;
        private Dictionary<int, BudgetDeposit> deposits = new Dictionary<int, BudgetDeposit>();
        private List<BudgetDeposit> depositDeductions = new List<BudgetDeposit>();

        
        private float estimatedYearlyIncome = 0;

        public IReadOnlyDictionary<int, BudgetDeposit> BudgetDeposits { get => deposits; }


        public BudgetStore budgetStore;

        public DepositStore(BudgetStore _budgetStore)
        {
            budgetStore = _budgetStore;

            depositDeductions.Add(new BudgetDeposit(-2, new AmountPercentModel(initialPercent: 0.145f)));
            depositDeductions.Add(new BudgetDeposit(-3, new AmountPercentModel(initialPercent: 0.014f)));
            depositDeductions.Add(new BudgetDeposit(-4, new AmountPercentModel(initialPercent: 0.062f)));
            depositDeductions.Add(new BudgetDeposit(-5, new AmountPercentModel(initialPercent: 0.0f)));

            foreach (Budget budget in budgetStore.Budgets.Values) deposits.Add(budget.ID, new BudgetDeposit(budget.ID));

            foreach (int budget in budgetStore.Budgets.Keys)
            {
                foreach(int childBudget in budgetStore.Budgets[budget].ChildBudgets)
                {
                    deposits[childBudget].SetParent(budget);
                }
            }

        }

        public float GetDepositAmount(bool preTaxAmount = false)
        {
            if (preTaxAmount) return DepositAmount;
            else return TakeHomeAmount;
        }

        public float GetBudgetDepositAmount(int depositID)
        {
            return deposits[depositID].DepositAmtPct.GetAmount(GetBudgetReferenceAmount(depositID));
        }
        public float GetBudgetReferenceAmount(int depositID) => budgetStore.IsBudgetPreTax(depositID) ? DepositAmount : TakeHomeAmount;

        public float SetBudgetDepositAmt(int depositID, float amount) => deposits[depositID].DepositAmtPct.Amount = amount;
        public float SetBudgetDepositPct(int depositID, float percent) => deposits[depositID].DepositAmtPct.Percent = percent;
        public float SetDepositDeductionAmt(int deductionID, float amount) => depositDeductions[deductionID].DepositAmtPct.Amount = amount;

        public Action<List<int>> DepositsChanged;
    }
}
