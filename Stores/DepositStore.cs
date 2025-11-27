using FinancialCalculator.Models;
using FinancialCalculator.Services;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace FinancialCalculator.Stores
{
    internal class DepositStore
    {

        private decimal depositAmount = 0;
        public decimal DepositAmount 
        { 
            get => depositAmount;
            set
            {
                depositAmount = Math.Max(0, value);
                PublishDepositChanged(depositService.AllocateWholeDeposit());
            }
        }

        public decimal TakeHomeAmount 
        { 
            get => Math.Max(0, DepositAmount - DepositDeductions.Sum(deduction => deduction.Value.DepositAmtPct.GetAmount(depositAmount)));
        }


        private decimal estimatedAnnualIncome = 0;
        public decimal EstimatedAnnualIncome 
        { 
            get => estimatedAnnualIncome; 
            set => estimatedAnnualIncome = Math.Max(0, value); 
        }

        public int MonthsCoveredByDeposit 
        {
            get => estimatedAnnualIncome == 0 ? 0 : (int)Math.Ceiling((depositAmount / estimatedAnnualIncome) * 12);
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
                deposits[budget].SetChildren(budgetStore.Budgets[budget].ChildBudgets);

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


        public decimal GetBudgetDepositAmount(int depositID)
        {
            return deposits[depositID].DepositAmtPct.Amount;
        }
        public decimal GetBudgetReferenceAmount(int depositID) => budgetStore.IsBudgetPreTax(GetDepositBudget(depositID).ID) ? DepositAmount : TakeHomeAmount;

        public void UpdatedBudgetSettings(int budgetID) => PublishDepositChanged(depositService.AllocateWholeDeposit());

        public void UsrUpdateDepositValue(int depositID, decimal amount = -1, decimal percent = -1)
        {


            BudgetDeposit selectedDeposit = deposits[depositID];
            AmountPercentModel selectedDepositAmtPct = selectedDeposit.DepositAmtPct;
            decimal availableAmount = GetMaxUsrSetAmount(depositID);

            Debug.Print(depositID + " Amt: " + amount + " Pct: " + percent + " " + " Max Amount: " + availableAmount);

            if (amount >= 0)
                selectedDepositAmtPct.Amount = Math.Min(amount, availableAmount);
            else if (percent >= 0)
                selectedDepositAmtPct.Percent = Math.Min(percent, availableAmount / GetBudgetReferenceAmount(depositID));

            if (selectedDeposit.DepositParentID == -1)
                PublishDepositChanged(depositService.AllocateWholeDeposit());
            else
            {
                List<int> changed = depositService.Allocate(selectedDeposit.DepositParentID, deposits[selectedDeposit.DepositParentID].DepositAmtPct.GetAmount(GetBudgetReferenceAmount(selectedDeposit.DepositParentID)), deposits[selectedDeposit.DepositParentID].DepositChildrenIDs.ToList());
                PublishDepositChanged(changed);

            }
        }

        private decimal GetMaxUsrSetAmount(int depositID)
        {
            decimal availableAmount = 0;
            BudgetDeposit selectedDeposit = deposits[depositID];

            if (selectedDeposit.DepositIsDeduction)
                availableAmount = TakeHomeAmount + selectedDeposit.DepositAmtPct.GetAmount(DepositAmount);
            else
            {
                decimal usedAmount;

                if (selectedDeposit.DepositParentID == -1)
                {
                    usedAmount = BudgetDeposits
                        .Where(deposit => deposit.Value.DepositParentID == -1 && deposit.Key != depositID)
                        .Sum(budgetID => deposits[budgetID.Key].DepositIsUserSet ? deposits[budgetID.Key].DepositAmtPct.GetAmount(TakeHomeAmount) : 0);

                    availableAmount = TakeHomeAmount - usedAmount;
                }
                else
                {
                    usedAmount = budgetStore.Budgets[selectedDeposit.DepositParentID].ChildBudgets
                        .Where(budgetID => budgetID != depositID)
                        .Sum(budgetID => deposits[budgetID].DepositIsUserSet ? deposits[budgetID].DepositAmtPct.GetAmount(TakeHomeAmount) : 0);

                    availableAmount = Math.Max(0, deposits[selectedDeposit.DepositParentID].DepositAmtPct.GetAmount(TakeHomeAmount) - usedAmount);
                }

            }

            return availableAmount;
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
