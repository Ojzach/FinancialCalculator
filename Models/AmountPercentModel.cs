using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class AmountPercentModel
    {

        public bool IsSetByAmount { get; private set; } = true;
        public float Amount { set => SetAmount(value); }
        public float Percent { set => SetPercent(value); }

        private float amount = 0;
        private float percent = 0; //Percents will be held in decimal form (Ex: 10% = 0.1, 100% = 1.0, 50% = 0.5)

        public AmountPercentModel(float initialAmount = 0, float initialPercent = 0)
        {
            if(initialAmount != 0) SetAmount(initialAmount);
            else if(initialPercent != 0) SetPercent(initialPercent);
        }

        private void SetAmount(float newAmount)
        {
            IsSetByAmount = true;
            amount = newAmount;
            percent = 0;
        }

        private void SetPercent(float newPercent) 
        {
            IsSetByAmount = false;
            percent = newPercent;
            amount = 0;
        }

        public float GetAmount(float referenceAmount)
        {
            if (IsSetByAmount) return amount;
            else return MathF.Round(referenceAmount * percent, 2);
        }

        public float GetPercent(float referenceAmount)
        {
            if (!IsSetByAmount) return percent;
            else if (referenceAmount == 0) return 0;
            else return amount / referenceAmount;
        }
    }
}
