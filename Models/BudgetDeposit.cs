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
        public bool DepositIsDeduction = false;
        public int DepositParentID { get; private set; } = -1; //-1 means no parent

        public AmountPercentModel DepositAmtPct { get => depositAmtPct; }

        private AmountPercentModel depositAmtPct;

        public bool DepositIsUserSet { get; set; } = false;
        public bool IsDepositAmountInvalid { get; set; } = false;
        public string DepositInvalidMsg { get; set; } = "";

        public BudgetDeposit(int depositBudgetID, AmountPercentModel initialAmtPct, bool depositIsDeduction = false)
        {
            DepositBudgetID = depositBudgetID;
            depositAmtPct = initialAmtPct;
            DepositIsDeduction = depositIsDeduction;
        }

        public void SetParent(int parentBudgetID) => DepositParentID = parentBudgetID;

        public void DepositInvalid(string message)
        {
            IsDepositAmountInvalid = true;
            DepositInvalidMsg = message;
        }

    }
}
