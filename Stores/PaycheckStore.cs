using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Stores
{
    internal class PaycheckStore
    {

        private float paycheckAmount = 0;
        private float estimatedYearlyIncome = 0;
        private float federalTaxAmount = 0;
        private float stateTaxAmount = 0;
        

        public float EstimatedyearlyIncome { get => estimatedYearlyIncome; set { estimatedYearlyIncome = value; OnPaycheckChanged(); } }
        public float PaycheckAmount { get => paycheckAmount; set {  paycheckAmount = value; OnPaycheckChanged(); } }
        public float FederalTaxAmount { get => federalTaxAmount; set { federalTaxAmount = value; OnPaycheckChanged(); } }
        public float StateTaxAmount { get => stateTaxAmount; set { stateTaxAmount = value; OnPaycheckChanged(); } }
        public float TakeHomeAmount { get => paycheckAmount - federalTaxAmount - stateTaxAmount; }
        public int MonthsCoveredByPaycheck { get 
            { 
                if(estimatedYearlyIncome == 0) return 0;
                else
                {
                    return (int)MathF.Ceiling((paycheckAmount / estimatedYearlyIncome) * 12);
                }
            } }



        public PaycheckStore()
        {
            paycheckAmount = 9333.56f;
            federalTaxAmount = 2333.39f;
            stateTaxAmount = 746.68f;
        }


        private void OnPaycheckChanged()
        {
            PaycheckChanged?.Invoke();
        }

        public Action PaycheckChanged;
    }
}
