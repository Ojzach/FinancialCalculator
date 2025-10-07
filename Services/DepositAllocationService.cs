using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
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

        public List<int> Allocate(int parentDeposit, float allocationAmount, List<int> budgetsToAllocate)
        {  

            if (parentDeposit != -1) depositStore.SetBudgetDepositAmt(parentDeposit, 0);

            List<int> changedDeposits = new List<int>();

            //Find the sum of the budgets that are set by the user. These are not editable so they will be subtracted out of rebalance
            int[] userSetDeposits = budgetsToAllocate.Where(id => depositStore.BudgetDeposits[id].DepositIsUserSet).ToArray();
            foreach (int depositID in userSetDeposits)
            {
                allocationAmount = allocationAmount - depositStore.GetBudgetDepositAmount(depositID);
                budgetsToAllocate.Remove(depositID);
            }

            if (allocationAmount < 0)
            {
                AllocationError();
                allocationAmount = 0;
            }



            if(budgetsToAllocate.Sum(id => budgetStore.Budgets[id].MinDepositAmount(depositStore.GetBudgetReferenceAmount(id))) <= allocationAmount)
            {

                foreach(Budget budget in budgetStore.Budgets.Values.Where(budget => budgetsToAllocate.Contains(budget.ID)))
                {
                    float budgetMinDeposit = budget.MinDepositAmount(depositStore.GetBudgetReferenceAmount(budget.ID));

                    depositStore.SetBudgetDepositAmt(budget.ID, budgetMinDeposit);
                    allocationAmount = allocationAmount - budgetMinDeposit;
                }

                changedDeposits.AddRange(budgetsToAllocate.Where(id => budgetStore.Budgets[id].MinDepositAmount(depositStore.GetBudgetReferenceAmount(id)) == budgetStore.Budgets[id].MaxDepositAmount(depositStore.GetBudgetReferenceAmount(id))));
                budgetsToAllocate.RemoveAll(id => budgetStore.Budgets[id].MinDepositAmount(depositStore.GetBudgetReferenceAmount(id)) == budgetStore.Budgets[id].MaxDepositAmount(depositStore.GetBudgetReferenceAmount(id)));
            }
            else
            {
                AllocationError();
                allocationAmount = 0;
            }






            if (parentDeposit != -1) depositStore.SetBudgetDepositAmt(parentDeposit, allocationAmount);


            //Allocates The Children
            foreach(int budget in changedDeposits)
            {
                if (budgetStore.Budgets[budget].ChildBudgets.Count > 0)
                {
                    Allocate(budget , depositStore.BudgetDeposits[budget].DepositAmtPct.Amount, budgetStore.Budgets[budget].ChildBudgets.ToList());
                }
            }


            return changedDeposits;
            /*
            //Find the sum of the budgets that are fixed amounts. These are not editable so they will be subtracted out of rebalance
            //Where function does not add fixed cost budgets that are also user set as to not double subtract
            if (availableSum >= unAssignedBudgets.Where(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget).Sum(item => item.BudgetRecommendedAmtPerMonth))
            {
                foreach (BudgetDepositViewModel budget in unAssignedBudgets.Where(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget))
                {
                    budget.DepositAmt = budget.BudgetRecommendedAmtPerMonth;
                    availableSum = availableSum - budget.DepositAmt;
                }
                unAssignedBudgets.RemoveAll(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget);

            }
            else
            {
                BudgetError();
                foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget).ToList(), availableSum, (budget) => 1, (budget) => 0))
                {
                    keyValue.Key.DepositAmt = keyValue.Value;
                    availableSum = availableSum - keyValue.Value;
                    unAssignedBudgets.Remove(keyValue.Key);
                }
                availableSum = 0;
            }


            //Distribute Felexible Budgets

            List<BudgetDepositViewModel> savingBudgets = unAssignedBudgets.Where(item => item._budget is SavingsBudget).ToList();
            List<BudgetDepositViewModel> flexibleBudgets = unAssignedBudgets.Where(item => item._budget is FlexibleBudget).ToList();


            if (availableSum >= savingBudgets.Sum(item => item.MaxAmt) + flexibleBudgets.Sum(item => item.MaxAmt))
            {
                availableSum = availableSum - savingBudgets.Sum(item => item.MaxAmt) + flexibleBudgets.Sum(item => item.MaxAmt);
                foreach (BudgetDepositViewModel budget in savingBudgets) { budget.DepositAmt = budget.MaxAmt; unAssignedBudgets.Remove(budget); }
                foreach (BudgetDepositViewModel budget in flexibleBudgets) { budget.DepositAmt = budget.MaxAmt; unAssignedBudgets.Remove(budget); }
            }
            else
            {


                if (availableSum >= savingBudgets.Sum(item => item.MinAmt) + flexibleBudgets.Sum(item => item.MinAmt))
                {

                    availableSum = availableSum - savingBudgets.Sum(item => item.MinAmt) - flexibleBudgets.Sum(item => item.MinAmt);

                    if (availableSum >= 0)
                    {

                        foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(budget => budget._budget is SavingsBudget).ToList(), availableSum, (budget) => 1, (budget) => budget.MinAmt))
                        {
                            keyValue.Key.DepositAmt = keyValue.Key.MinAmt + keyValue.Value;
                            availableSum = availableSum - keyValue.Value;
                            unAssignedBudgets.Remove(keyValue.Key);
                        }
                    }

                    if (availableSum >= 0)
                    {
                        foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(budget => budget._budget is FlexibleBudget).ToList(), availableSum, (budget) => 1, (budget) => budget.MinAmt))
                        {
                            keyValue.Key.DepositAmt = keyValue.Key.MinAmt + keyValue.Value;
                            availableSum = availableSum - keyValue.Value;
                            unAssignedBudgets.Remove(keyValue.Key);
                        }
                    }

                }
                else
                {
                    BudgetError();
                    availableSum = 0;
                }



            }

            if (!UnallocattedBudget.IsUsrSet)
            {
                UnallocattedBudget.DepositAmt = availableSum;
                unAssignedBudgets.Remove(UnallocattedBudget);

                if (_budget is FixedBudget && UnallocattedBudget.DepositAmt == 0) UnallocattedBudget.isVisible = false;
                else UnallocattedBudget.isVisible = true;
            }


            foreach (BudgetDepositViewModel budget in unAssignedBudgets) budget.DepositAmt = 0;


            if (MathF.Round(SubItems.Sum(item => item.DepositAmt), 2) != MathF.Round(DepositAmt, 2))
            {
                string t = "";
                //foreach (BudgetDepositViewModel item in SubItems) t += "\n\t" + item.BudgetName + " " + item.DepositAmt;
                Debug.Print(BudgetName + " " + SubItems.Sum(item => item.DepositAmt) + " " + DepositAmt + " " + t);
                BudgetError();
            }*/
        }
        /*
        private Dictionary<BudgetDepositViewModel, float> DistributeAmtAmongBudgets(List<BudgetDepositViewModel> budgets, float amt, Func<BudgetDepositViewModel, float> ratioWeight, Func<BudgetDepositViewModel, float> preAllocatedAmt)
        {
            Dictionary<BudgetDepositViewModel, float> distributedAmts = budgets.ToDictionary(b => b, b => 0f);
            bool finishedAllocation = false;
            float totalUsed = 0;

            while (finishedAllocation == false)
            {
                finishedAllocation = true;
                float ratioTotal = budgets.Count();

                foreach (BudgetDepositViewModel budget in budgets)
                {
                    float calculatedAmt = (amt - totalUsed) * (ratioWeight(budget) / ratioTotal);

                    if (calculatedAmt + preAllocatedAmt(budget) >= budget.MaxAmt)
                    {
                        distributedAmts[budget] = budget.MaxAmt - preAllocatedAmt(budget);
                        totalUsed += budget.MaxAmt - preAllocatedAmt(budget);
                        budgets.Remove(budget);
                        finishedAllocation = false;
                        break;
                    }
                    else distributedAmts[budget] = calculatedAmt;
                }
            }

            return distributedAmts;
        }*/


        public void AllocationError()
        {
            Debug.Print("Allocation Error");
        }
        

    }
}
