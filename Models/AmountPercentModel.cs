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
        public float Amount {
            get
            {
                if (IsSetByAmount) return amount;
                else return MathF.Round(ResolveReferenceAmount() * percent, 2);
            }
            set {
                IsSetByAmount = true;
                amount = value;
                percent = 0;
            }
        }
        public float Percent {
            get
            {
                if (!IsSetByAmount) return percent;
                else if (ResolveReferenceAmount() == 0) return 0;
                else return amount / ResolveReferenceAmount();
            }
            set
            {
                IsSetByAmount = false;
                percent = value;
                amount = 0;
            } 
        }

        private float amount = 0;
        private float percent = 0; //Percents will be held in decimal form (Ex: 10% = 0.1, 100% = 1.0, 50% = 0.5)

        private Func<float> referenceAmountProvider;

        public AmountPercentModel(Func<float> referenceAmountProvider = null, float initialAmount = 0, float initialPercent = 0)
        {
            this.referenceAmountProvider = referenceAmountProvider;
            if (initialAmount != 0) Amount = initialAmount;
            else if(initialPercent != 0) Percent = initialPercent;
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


        private float ResolveReferenceAmount()
        {
            if (referenceAmountProvider is null)
            {
                throw new InvalidOperationException("Reference amount provider has not been set.");
            }

            return referenceAmountProvider();
        }
    }
}
