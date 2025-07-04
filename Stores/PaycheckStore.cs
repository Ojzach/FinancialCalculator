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
        private float paycheckDeductions = 0;
        

        public float EstimatedyearlyIncome { get => estimatedYearlyIncome; set { estimatedYearlyIncome = value; OnPaycheckChanged(); } }
        public float PaycheckAmount { get => paycheckAmount; set {  paycheckAmount = value; OnPaycheckChanged(); } }
        public float PaycheckDeductions { get => paycheckDeductions; set { paycheckDeductions = value; OnPaycheckChanged(); } }
        public float TakeHomeAmount { get => paycheckAmount - paycheckDeductions; }


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
            paycheckAmount = 10000;
        }


        public float GetPaycheckAmount(bool preTaxAmount = false)
        {
            if (preTaxAmount) return PaycheckAmount;
            else return TakeHomeAmount;
        }


        private void OnPaycheckChanged()
        {
            PaycheckChanged?.Invoke();
        }

        public Action PaycheckChanged;
    }
}
