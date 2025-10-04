using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Services
{
    internal class DepositAllicatorService
    {

        private float depositAmount = 0.0f;
        private bool depositIsTaxed = true;

        public DepositAllicatorService() 
        {

        }

        public void SetDepositAmount(float depositAmt, bool isTaxed = true)
        {
            depositAmount = depositAmt;
            depositIsTaxed = isTaxed;
        }
    }
}
