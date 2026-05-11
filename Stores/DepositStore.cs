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
                deposits[BaseDepositID].DepositAmtPct.Amount = TakeHomeAmount;
                depositService.Allocate(BaseDepositID, TakeHomeAmount);
                PublishDepositChanged([]);
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

        public int BaseDepositID { get; private set; } = -1;

        public event Action<List<int>>? DepositsChanged;
        
        private Dictionary<int, BudgetDeposit> deposits = new();


        public DepositStore(BudgetStore _budgetStore)
        {
            budgetStore = _budgetStore;
            depositService = new DepositAllocationService(this);          

            foreach (var deduction in budgetStore.HiddenBudgets)
            {
                AmountPercentModel amtPct = new AmountPercentModel(() => DepositAmount, ((FixedBudget)deduction.Value).SetAmount, ((FixedBudget)deduction.Value).SetPercent);

                deposits.Add(deduction.Value.ID, new BudgetDeposit(deduction.Value.ID, amtPct, true));
                deposits[deduction.Key].SetParent(-2);
            }

            foreach (var budget in budgetStore.Budgets)
            {
                if(budgetStore.IsBudgetPreTax(budget.Value.ID)) deposits.Add(budget.Value.ID, new BudgetDeposit(budget.Value.ID, new AmountPercentModel(() => DepositAmount)));
                else deposits.Add(budget.Value.ID, new BudgetDeposit(budget.Value.ID, new AmountPercentModel(() => TakeHomeAmount)));

            }

            foreach (var budget in budgetStore.Budgets)
            {
                deposits[budget.Key].SetChildren(budgetStore.Budgets[budget.Key].ChildBudgets);

                foreach (int childBudget in budgetStore.Budgets[budget.Key].ChildBudgets)
                {
                    deposits[childBudget].SetParent(budget.Key);
                }

            }

            BaseDepositID = 0;
            deposits[BaseDepositID].DepositAmtPct.Percent = 1;

        }

        private void PublishDepositChanged(List<int> depositsChanged)
        {
            DepositsChanged?.Invoke(depositsChanged);
        }


        public decimal GetBudgetDepositAmount(int depositID)
        {
            return deposits[depositID].DepositAmtPct.Amount;
        }

        public bool IsBudgetDepositInvalid(int depositID) => BudgetDeposits[depositID].IsDepositAmountInvalid;
        public string InvalidDepositMessage(int depositID) => BudgetDeposits[depositID].DepositInvalidMsg;

        public decimal GetBudgetReferenceAmount(int depositID) => budgetStore.IsBudgetPreTax(GetDepositBudget(depositID).ID) ? DepositAmount : TakeHomeAmount;

        public void UpdatedBudgetSettings(int budgetID)
        {
            depositService.Allocate(0, TakeHomeAmount);
            PublishDepositChanged([0]);
        }

        public void UsrUpdateDepositValue(int depositID, decimal amount = -1, decimal percent = -1)
        {
            BudgetDeposit selectedDeposit = deposits[depositID];
            AmountPercentModel selectedDepositAmtPct = selectedDeposit.DepositAmtPct;
            decimal availableAmount = GetMaxUsrSetAmount(depositID);

            if (amount >= 0)
                selectedDepositAmtPct.Amount = Math.Min(amount, availableAmount);
            else if (percent >= 0)
                selectedDepositAmtPct.Percent = Math.Min(percent, availableAmount / GetBudgetReferenceAmount(depositID));


            List<int> changed = new();
            if (selectedDeposit.DepositParentID == -2)
            {
                deposits[BaseDepositID].DepositAmtPct.Amount = TakeHomeAmount;
                changed = depositService.Allocate(BaseDepositID, TakeHomeAmount);
                changed.Add(BaseDepositID);
            }
            else
            {
                changed  = depositService.Allocate(selectedDeposit.DepositParentID, deposits[selectedDeposit.DepositParentID].DepositAmtPct.GetAmount(GetBudgetReferenceAmount(selectedDeposit.DepositParentID)));
                changed.AddRange(depositService.Allocate(depositID, selectedDepositAmtPct.GetAmount(GetBudgetReferenceAmount(depositID))));
            }

            PublishDepositChanged(changed);
        }

        private decimal GetMaxUsrSetAmount(int depositID)
        {
            var availableAmount = 0m;
            BudgetDeposit selectedDeposit = deposits[depositID];

            if (selectedDeposit.DepositIsDeduction)
                availableAmount = TakeHomeAmount + selectedDeposit.DepositAmtPct.GetAmount(DepositAmount);
            else
            {
                var usedAmount = budgetStore.Budgets[selectedDeposit.DepositParentID].ChildBudgets
                        .Where(budgetID => budgetID != depositID)
                        .Sum(budgetID => deposits[budgetID].DepositIsUserSet ? deposits[budgetID].DepositAmtPct.GetAmount(TakeHomeAmount) : 0);

                    availableAmount = Math.Max(0, deposits[selectedDeposit.DepositParentID].DepositAmtPct.GetAmount(TakeHomeAmount) - usedAmount);


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
