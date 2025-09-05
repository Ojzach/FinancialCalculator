using System.Diagnostics;

namespace FinancialCalculator.Stores
{
    public class DepositStore
    {

        private float depositAmount = 0;
        private float estimatedYearlyIncome = 0;

        private float federalTaxAmt;
        private float medicareAmt;
        private float socialSecurityAmt;
        private float stateTaxAmt;

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
                return MathF.Max(0, depositAmount - FederalTaxAmt - MedicareAmt - SocialSecurityAmt - StateTaxAmt);
            }
        }//=> MathF.Max(0, paycheckAmount - federalTaxAmt - medicareAmt - socialSecurityAmt - stateTaxAmt); }


        public int MonthsCoveredByDeposit { get 
            { 
                if(estimatedYearlyIncome == 0) return 0;
                else
                {
                    return (int)MathF.Ceiling((depositAmount / estimatedYearlyIncome) * 12);
                }
            } }



        public DepositStore()
        {
            depositAmount = 10000;
        }


        public float GetDepositAmount(bool preTaxAmount = false)
        {
            if (preTaxAmount) return DepositAmount;
            else return TakeHomeAmount;
        }


        private void OnDepositChanged()
        {
            DepositChanged?.Invoke();
        }

        public Action DepositChanged;
    }
}
