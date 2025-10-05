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
        public float Amount { get => amount; set => SetAmount(value); }
        public float Percent { get => percent; set => SetPercent(value); }

        private float amount = 0;
        private float percent = 0; //Percents will be held in decimal form (Ex: 10% = 0.1, 100% = 1.0, 50% = 0.5)

        private Func<float> referanceAmount;

        public AmountPercentModel(Func<float> _refereanceAmount)
        {
            referanceAmount = _refereanceAmount;
        }

        private void SetAmount(float newAmount)
        {
            IsSetByAmount = true;
            amount = MathF.Round(newAmount, 2);

            if(referanceAmount != null && referanceAmount() != 0) percent = amount / referanceAmount();
            else percent = 0;
        }

        private void SetPercent(float newPercent) 
        {
            IsSetByAmount = false;
            percent = newPercent;

            if (referanceAmount != null) amount = MathF.Round(percent * referanceAmount(), 2);
            else amount = 0;
        }
    }
}
