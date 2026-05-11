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
        public decimal Amount {
            get
            {
                if (IsSetByAmount) return amount;
                else return Math.Round(ResolveReferenceAmount() * percent, 2);
            }
            set {
                IsSetByAmount = true;
                amount = value;
                percent = 0;
            }
        }
        public decimal Percent {
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

        private decimal amount = 0;
        private decimal percent = 0; //Percents will be held in decimal form (Ex: 10% = 0.1, 100% = 1.0, 50% = 0.5)

        private Func<decimal> referenceAmountProvider;

        public AmountPercentModel(Func<decimal> referenceAmountProvider = null, decimal initialAmount = 0, decimal initialPercent = 0)
        {
            this.referenceAmountProvider = referenceAmountProvider;
            if (initialAmount != 0) Amount = initialAmount;
            else if(initialPercent != 0) Percent = initialPercent;
        }

        public decimal GetAmount(decimal referenceAmount)
        {
            if (IsSetByAmount) return amount;
            else return Math.Round(referenceAmount * percent, 2);
        }

        public decimal GetPercent(decimal referenceAmount)
        {
            if (!IsSetByAmount) return percent;
            else if (referenceAmount == 0) return 0;
            else return amount / referenceAmount;
        }


        private decimal ResolveReferenceAmount()
        {
            if (referenceAmountProvider is null)
            {
                throw new InvalidOperationException("Reference amount provider has not been set.");
            }

            return referenceAmountProvider();
        }
    }
}
