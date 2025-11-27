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
        public List<int> DepositChildrenIDs { get; private set; } = new();

        public AmountPercentModel DepositAmtPct { get => depositAmtPct; }
        public AmountPercentModel UnallocatedAmtPct { get => unallocatedAmtPct; }

        private AmountPercentModel unallocatedAmtPct;
        private AmountPercentModel depositAmtPct;

        public bool DepositIsUserSet { get; set; } = false;
        public bool UnallocatedIsUserSet { get; set; } = false;
        public bool IsDepositAmountInvalid { get; set; } = false;
        public string DepositInvalidMsg { get; set; } = "";

        public BudgetDeposit(int depositBudgetID, AmountPercentModel initialAmtPct, bool depositIsDeduction = false)
        {
            DepositBudgetID = depositBudgetID;
            depositAmtPct = initialAmtPct;
            unallocatedAmtPct = new();
            DepositIsDeduction = depositIsDeduction;
        }

        public void SetParent(int parenDepositID) => DepositParentID = parenDepositID;
        public void SetChildren(List<int> childrenDepositIDs) => DepositChildrenIDs = childrenDepositIDs;

        public void DepositInvalid(string message)
        {
            IsDepositAmountInvalid = true;
            DepositInvalidMsg = message;
        }

    }
}
