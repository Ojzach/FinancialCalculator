using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    internal class BudgetCalculatorDeposit
    {


        public int DepositBudgetID { get; private set; }
        public AmountPercentModel DepositAmtPct { get => depositAmtPct; }

        private AmountPercentModel depositAmtPct;

        public BudgetCalculatorDeposit(DepositStore depositStore, int depositBudgetID)
        {
            DepositBudgetID = depositBudgetID;
            depositAmtPct = new AmountPercentModel();
        }
    }
}
