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
            get => MathF.Max(0, DepositAmount - DepositDeductions.Sum(deduction => deduction.Value.DepositAmtPct.GetAmount(depositAmount)));
        }


        private float estimatedAnnualIncome = 0;
        public float EstimatedAnnualIncome 
        { 
            get => estimatedAnnualIncome; 
            set => estimatedAnnualIncome = MathF.Max(0, value); 
        }

        public int MonthsCoveredByDeposit 
        {
            get => estimatedAnnualIncome == 0 ? 0 : (int)MathF.Ceiling((depositAmount / estimatedAnnualIncome) * 12);
        }


        public BudgetStore budgetStore;
        private DepositAllocationService depositService;
        public IReadOnlyDictionary<int, BudgetDeposit> BudgetDeposits => deposits.Where(deposit => !deposit.Value.DepositIsDeduction).ToDictionary();
        public IReadOnlyDictionary<int, BudgetDeposit> DepositDeductions => deposits.Where(deposit => deposit.Value.DepositIsDeduction).ToDictionary();


        public event Action<List<int>>? DepositsChanged;
        
        private Dictionary<int, BudgetDeposit> deposits = new();


        public DepositStore(BudgetStore _budgetStore)
        {
            budgetStore = _budgetStore;
            depositService = new DepositAllocationService(this, budgetStore);


            foreach(Budget deduction in budgetStore.HiddenBudgets.Values)
            {
                AmountPercentModel amtPct = new AmountPercentModel(() => DepositAmount, ((FixedBudget)deduction).SetAmount, ((FixedBudget)deduction).SetPercent);

                deposits.Add(deduction.ID, new BudgetDeposit(deduction.ID, amtPct, true));
            }

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
        public float GetBudgetReferenceAmount(int depositID) => budgetStore.IsBudgetPreTax(GetDepositBudget(depositID).ID) ? DepositAmount : TakeHomeAmount;

        public void UpdatedBudgetSettings(int budgetID) => PublishDepositChanged(depositService.AllocateWholeDeposit());

        public void UpdateDepositValue(int depositID, float amount = -1, float percent = -1)
        {
            if (amount >= 0) deposits[depositID].DepositAmtPct.Amount = amount;
            else if (percent >= 0) deposits[depositID].DepositAmtPct.Percent = percent;

            PublishDepositChanged(depositService.AllocateWholeDeposit());
        }

        public Budget GetDepositBudget(int depositID) => budgetStore.GetBudget(GetDeposit(depositID).DepositBudgetID);
        public Dictionary<int, Budget> GetDepositBudgets(List<int> deposits) => deposits.ToDictionary(depositID => depositID, depositID => GetDepositBudget(depositID));


        public BudgetDeposit GetDeposit(int depositID)
        {
            if (BudgetDeposits.ContainsKey(depositID)) return BudgetDeposits[depositID];
            else if (DepositDeductions.ContainsKey(depositID)) return DepositDeductions[depositID];

            throw new Exception("Budget ID does not exist in the Budget Store");
        }
    }
}
