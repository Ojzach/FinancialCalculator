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

        public List<int> Allocate(int parentDeposit, decimal allocationAmount, List<int> depositsToAllocate)
        {
            if(parentDeposit != -1) 
                depositStore.GetDeposit(parentDeposit).IsDepositAmountInvalid = false; //Resets Error Outline

            ReadOnlyDictionary<int, Budget> depositBudgets = new (depositStore.GetDepositBudgets(depositsToAllocate));
            Dictionary<int, decimal> changedDeposits = new();


            //Find the sum of the budgets that are set by the user. These are not editable so they will be subtracted out of rebalance
            var userSetDeposits = depositsToAllocate.Where(id => depositStore.GetDeposit(id).DepositIsUserSet).ToList();
            foreach (int depositID in userSetDeposits)
            {
                allocationAmount -= depositStore.GetBudgetDepositAmount(depositID);
                Debug.Print(depositID + " " + depositStore.GetBudgetDepositAmount(depositID) + " " + allocationAmount);
                depositsToAllocate.Remove(depositID);
            }


            if (allocationAmount < 0) 
                throw new AllocationException("User-set amounts are greater than amount available.");


            void Distribute(List<int> deposits)
            {
                Dictionary<int, decimal> distributions = DistributeAmtAmongDeposits(
                    deposits,
                    allocationAmount,
                    id => (decimal)depositBudgets[id].Priority,
                    id => changedDeposits.ContainsKey(id) ? changedDeposits[id] : 0,
                    id => depositBudgets[id].MaxDepositAmount(RefAmt(id))
                );

                allocationAmount -= distributions.Values.Sum();
                foreach (var distrobution in distributions)
                {
                    if (changedDeposits.ContainsKey(distrobution.Key))
                        changedDeposits[distrobution.Key] += distrobution.Value;
                    else
                        changedDeposits[distrobution.Key] = distrobution.Value;

                    depositsToAllocate.Remove(distrobution.Key);
                }
            }

            //Allocate Minimum Amount To All
            decimal sumMinAmounts = depositsToAllocate.Sum(id => depositBudgets[id].MinDepositAmount(RefAmt(id)));
            if (sumMinAmounts <= allocationAmount)
            {

                foreach (int depositID in depositsToAllocate)
                {
                    decimal budgetMinDeposit = depositBudgets[depositID].MinDepositAmount(RefAmt(depositID));
                    changedDeposits[depositID] = budgetMinDeposit;
                    allocationAmount -= budgetMinDeposit;
                }

                depositsToAllocate.RemoveAll(id => depositBudgets[id].MinDepositAmount(RefAmt(id)) == depositBudgets[id].MaxDepositAmount(RefAmt(id)));

                var highPriority = depositsToAllocate.Where(id =>
                depositBudgets[id].Priority != BudgetPriority.None).ToList();
                Distribute(highPriority);

            }
            else
            {
                if (parentDeposit != -1)
                    depositStore.BudgetDeposits[parentDeposit].DepositInvalid("Child budget's minimum amounts are more then the amount available");
                else new AllocationException("Top level budgets minimum amounts are more then the amount available");

                var highPriority = depositsToAllocate.Where(id =>
                    depositBudgets[id].Priority == BudgetPriority.VeryHigh
                    || depositBudgets[id].Priority == BudgetPriority.High).ToList();
                Distribute(highPriority);

                var mediumPriority = depositsToAllocate.Where(id =>
                    depositBudgets[id].Priority == BudgetPriority.Medium
                    || depositBudgets[id].Priority == BudgetPriority.Low
                    || depositBudgets[id].Priority == BudgetPriority.VeryLow).ToList();
                Distribute(mediumPriority);

                
            }

            Distribute(depositsToAllocate); //Distribute All Leftover Deposits



            //This will not update children that are set by percentages if the parent amount didnt change
            changedDeposits = changedDeposits.Where(deposit => deposit.Value != depositStore.GetBudgetDepositAmount(deposit.Key)).ToDictionary();

            foreach (KeyValuePair<int, decimal> deposit in changedDeposits)
            {
                depositStore.BudgetDeposits[deposit.Key].DepositAmtPct.Amount = deposit.Value;

                if (budgetStore.GetBudget(deposit.Key).ChildBudgets.Count > 0)
                {
                    Allocate(deposit.Key, depositStore.BudgetDeposits[deposit.Key].DepositAmtPct.Amount, budgetStore.Budgets[deposit.Key].ChildBudgets.ToList());
                }
            }

            if (parentDeposit != -1 && !depositStore.BudgetDeposits[parentDeposit].UnallocatedIsUserSet)
                depositStore.BudgetDeposits[parentDeposit].UnallocatedAmtPct.Amount = allocationAmount;


            return changedDeposits.Keys.ToList();

        }

        private decimal RefAmt(int depositID) => depositStore.GetBudgetReferenceAmount(depositID); //Gets either Takehome amount or Deposit Amount depending on if budget is pre-tax


        private Dictionary<int, decimal> DistributeAmtAmongDeposits(List<int> deposits, decimal amt, Func<int, decimal> ratioWeight, Func<int, decimal> preAllocatedAmt, Func<int, decimal> maxAllocationAmt)
        {
            Dictionary<int, decimal> distributedAmts = deposits.ToDictionary(b => b, b => 0m);

            bool finishedAllocation = false;
            decimal totalUsed = 0;

            while (finishedAllocation == false)
            {
                finishedAllocation = true;
                decimal ratioTotal = deposits.Sum(id => ratioWeight(id));

                foreach (int deposit in deposits)
                {
                    decimal calculatedAmt = (amt - totalUsed) * (ratioWeight(deposit) / ratioTotal);

                    if (calculatedAmt + preAllocatedAmt(deposit) >= maxAllocationAmt(deposit))
                    {
                        decimal maxDepositAmount = maxAllocationAmt(deposit) - preAllocatedAmt(deposit);
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
