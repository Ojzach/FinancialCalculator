using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    internal class BudgetDeposit
    {


        public int DepositBudgetID { get; private set; } = -1; // -1 Should never occur. Throw Error
        public int DepositParentID { get; private set; } = -1; //-1 means no parent

        public AmountPercentModel DepositAmtPct { get => depositAmtPct; }

        private AmountPercentModel depositAmtPct;

        public bool DepositIsUserSet { get; set; } = false;

        public BudgetDeposit(int depositBudgetID, AmountPercentModel initialAmtPct = null)
        {
            DepositBudgetID = depositBudgetID;
            depositAmtPct = initialAmtPct is null ? new AmountPercentModel() : initialAmtPct;
        }

        public void SetParent(int parentBudgetID) => DepositParentID = parentBudgetID;

    }
}
