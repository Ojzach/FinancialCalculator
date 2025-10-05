using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Services
{
    internal class DepositAllocationService
    {

        DepositStore depositStore;

        public DepositAllocationService(DepositStore _depositStore) 
        {
            depositStore = _depositStore;
        }


    }
}
