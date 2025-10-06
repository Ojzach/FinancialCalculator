using FinancialCalculator.Models;
using FinancialCalculator.Services;
using System.Diagnostics;
using System.Net;

namespace FinancialCalculator.Stores
{
    internal class DepositStore
    {

        private float depositAmount = 0;
        private float estimatedYearlyIncome = 0;

        private float federalTaxAmt;
        private float medicareAmt;
        private float socialSecurityAmt;
        private float stateTaxAmt;


        private List<BudgetDeposit> depositDeductions = new List<BudgetDeposit>();

        public float FederalTaxAmt { get => federalTaxAmt; set { federalTaxAmt = MathF.Max(0, value); OnDepositChanged(); } }
        public float MedicareAmt { get => medicareAmt; set { medicareAmt = MathF.Max(0, value); OnDepositChanged(); } }
        public float SocialSecurityAmt { get => socialSecurityAmt; set { socialSecurityAmt = MathF.Max(0, value); OnDepositChanged(); } }
        public float StateTaxAmt { get => stateTaxAmt; set { stateTaxAmt = MathF.Max(0, value); OnDepositChanged(); } }

        public void UpdateDeductions(float _federalTaxAmt = -1, float _medicareAmt = -1, float _socialSecurityAmt = -1, float _stateTaxAmt = -1)
        {
            if (_federalTaxAmt >= 0) federalTaxAmt = _federalTaxAmt;
            if (_medicareAmt >= 0) medicareAmt = _medicareAmt;
            if (_socialSecurityAmt >= 0) socialSecurityAmt = _socialSecurityAmt;
            if (_stateTaxAmt >= 0) stateTaxAmt = _stateTaxAmt;

        }



        public float EstimatedyearlyIncome { get => estimatedYearlyIncome; set { estimatedYearlyIncome = MathF.Max(0, value); OnDepositChanged(); } }
        public float DepositAmount { get => depositAmount; set {  depositAmount = MathF.Max(0, value); OnDepositChanged(); } }
        public float TakeHomeAmount { get {

                return MathF.Max(0, depositAmount - depositDeductions.Sum(deduction => deduction.DepositAmtPct.GetAmount(DepositAmount)));
            }
        }


        public int MonthsCoveredByDeposit { get 
            { 
                if(estimatedYearlyIncome == 0) return 0;
                else
                {
                    return (int)MathF.Ceiling((depositAmount / estimatedYearlyIncome) * 12);
                }
            } }


        Dictionary<int, BudgetDeposit> deposits = new Dictionary<int, BudgetDeposit>();

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


        private void OnDepositChanged()
        {
            DepositChanged?.Invoke();
        }

        public Action DepositChanged;
    }
}
