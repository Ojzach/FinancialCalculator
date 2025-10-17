using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Services
{
    internal class DepositAllocationService
    {

        DepositStore depositStore;
        BudgetStore budgetStore;

        public DepositAllocationService(DepositStore _depositStore, BudgetStore _budgetStore)
        {
            depositStore = _depositStore;
            budgetStore = _budgetStore;
        }

        public List<int> AllocateWholeDeposit()
        {
            Allocate(-1, depositStore.TakeHomeAmount, depositStore.BudgetDeposits.Keys.Where(id => depositStore.BudgetDeposits[id].DepositParentID == -1).ToList());
            return new List<int>() { 0 };
        }

        public List<int> Allocate(int parentDeposit, float allocationAmount, List<int> depositsToAllocate)
        {
            if(parentDeposit != -1) depositStore.GetDeposit(parentDeposit).IsDepositAmountInvalid = false; //Resets Error Outline

            ReadOnlyDictionary<int, Budget> depositBudgets = new (depositStore.GetDepositBudgets(depositsToAllocate));
            Dictionary<int, float> changedDeposits = new Dictionary<int, float>();


            StringBuilder debugOutput = new StringBuilder();
            if(parentDeposit != -1) debugOutput.AppendLine("----------  " + depositStore.GetDepositBudget(parentDeposit).Name + "  ----------");


            //Find the sum of the budgets that are set by the user. These are not editable so they will be subtracted out of rebalance
            int[] userSetDeposits = depositsToAllocate.Where(id => depositStore.GetDeposit(id).DepositIsUserSet).ToArray();
            foreach (int depositID in userSetDeposits)
            {
                allocationAmount -= depositStore.GetBudgetDepositAmount(depositID);
                depositsToAllocate.Remove(depositID);
            }

            if (allocationAmount < 0) throw new AllocationException("User-set amounts are greater than amount available.");


            //Allocate Minimum Amount To All
            if (depositsToAllocate.Sum(id => depositBudgets[id].MinDepositAmount(RefAmt(id))) <= allocationAmount)
            {

                foreach (int depositID in depositsToAllocate)
                {
                    float budgetMinDeposit = depositBudgets[depositID].MinDepositAmount(RefAmt(depositID));

                    changedDeposits[depositID] = budgetMinDeposit;
                    allocationAmount -= budgetMinDeposit;
                }

                depositsToAllocate.RemoveAll(id => depositBudgets[id].MinDepositAmount(RefAmt(id)) == depositBudgets[id].MaxDepositAmount(RefAmt(id)));
            }
            else
            {
                depositStore.BudgetDeposits[parentDeposit].DepositInvalid("Child budget's minimum amounts are more then the amount available");
            }

            debugOutput.AppendLine(depositsToAllocate.Count() + " " +  allocationAmount);

            foreach (KeyValuePair<int, float> depositAmt in DistributeAmtAmongDeposits(
                depositsToAllocate.Where(id => depositBudgets[id].Priority == BudgetPriority.VeryHigh || depositBudgets[id].Priority == BudgetPriority.High).ToList(),
                allocationAmount,
                (id) => (float)depositBudgets[id].Priority,
                (id) => changedDeposits.ContainsKey(id) ? changedDeposits[id] : 0,
                (id) => depositBudgets[id].MaxDepositAmount(RefAmt(id))
                ))
            {
                if (changedDeposits.ContainsKey(depositAmt.Key)) changedDeposits[depositAmt.Key] += depositAmt.Value;
                else changedDeposits[depositAmt.Key] = depositAmt.Value;
                
                allocationAmount -= depositAmt.Value;
                depositsToAllocate.Remove(depositAmt.Key);
            }

            foreach (KeyValuePair<int, float> depositAmt in DistributeAmtAmongDeposits(
                depositsToAllocate,
                allocationAmount,
                (id) => (float)depositBudgets[id].Priority,
                (id) => changedDeposits.ContainsKey(id) ? changedDeposits[id] : 0,
                (id) => depositBudgets[id].MaxDepositAmount(RefAmt(id))
                ))
            {
                if (changedDeposits.ContainsKey(depositAmt.Key)) changedDeposits[depositAmt.Key] += depositAmt.Value;
                else changedDeposits[depositAmt.Key] = depositAmt.Value;

                allocationAmount -= depositAmt.Value;
                depositsToAllocate.Remove(depositAmt.Key);
            }


            Debug.Print(debugOutput.ToString());

            //This will not update children that are set by percentages if the parent amount didnt change
            changedDeposits = changedDeposits.Where(deposit => deposit.Value != depositStore.GetBudgetDepositAmount(deposit.Key)).ToDictionary();

            foreach (KeyValuePair<int, float> deposit in changedDeposits)
            {
                depositStore.BudgetDeposits[deposit.Key].DepositAmtPct.Amount = deposit.Value;

                if (budgetStore.Budgets[deposit.Key].ChildBudgets.Count > 0)
                {
                    Allocate(deposit.Key, depositStore.BudgetDeposits[deposit.Key].DepositAmtPct.Amount, budgetStore.Budgets[deposit.Key].ChildBudgets.ToList());
                }
            }


            return changedDeposits.Keys.ToList();

        }

        private float RefAmt(int depositID) => depositStore.GetBudgetReferenceAmount(depositID); //Gets either Takehome amount or Deposit Amount depending on if budget is pre-tax
        
        
        private Dictionary<int, float> DistributeAmtAmongDeposits(List<int> deposits, float amt, Func<int, float> ratioWeight, Func<int, float> preAllocatedAmt, Func<int, float> maxAllocationAmt)
        {
            Dictionary<int, float> distributedAmts = deposits.ToDictionary(b => b, b => 0f);

            bool finishedAllocation = false;
            float totalUsed = 0;

            while (finishedAllocation == false)
            {
                finishedAllocation = true;
                float ratioTotal = deposits.Sum(id => ratioWeight(id));

                foreach (int deposit in deposits)
                {
                    float calculatedAmt = (amt - totalUsed) * (ratioWeight(deposit) / ratioTotal);

                    if (calculatedAmt + preAllocatedAmt(deposit) >= maxAllocationAmt(deposit))
                    {
                        float maxDepositAmount = maxAllocationAmt(deposit) - preAllocatedAmt(deposit);
                        distributedAmts[deposit] = maxDepositAmount;
                        totalUsed += maxDepositAmount;
                        deposits.Remove(deposit);
                        finishedAllocation = false;
                        break;
                    }
                    else distributedAmts[deposit] = calculatedAmt;
                }
            }

            return distributedAmts;
        }

    }


    internal class AllocationException : Exception
    {
        public AllocationException() { }
        public AllocationException(string message) : base(message) { }
        public AllocationException(string message, Exception inner) : base(message, inner) { }
    }
}
