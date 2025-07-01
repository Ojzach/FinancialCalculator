using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    class SavingsBalanceSheetViewModel : BalanceSheetBaseViewModel
    {

        public string MaxTotalSavingsPercentStr
        {
            get => (balanceSheet.maxTotalSavingsPercent * 100).ToString("0.00") + "%";
            set
            {
                float p = float.Parse(value.Trim(new Char[] { '%' }));
                balanceSheet.maxTotalSavingsPercent = p <= 100 ? p / 100 : 1;
                OnPropertyChanged("MaxTotalSavingsPercentStr");
                BalanceItemChanged();
            }
        }

        public SavingsBalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName) : base(paycheck, balanceSheetName)
        {

            balanceSheet = new BalanceSheet();
            MaxTotalSavingsPercentStr = "60";

            
        }

        public override void AddBalanceSheetItem(BalanceItem item)
        {
            base.AddBalanceSheetItem(item);
            BalanceItemChanged();
        }


        protected override void BalanceItemChanged()
        {

            float maxAmount = _paycheck.TakeHomeAmount * balanceSheet.maxTotalSavingsPercent;

            SavingsBalanceItem[] savingsBalanceItems = balanceSheet.BalanceItems.Cast<SavingsBalanceItem>().ToArray();

            float totalUsedAmt = 0;


            SavingsBalanceItem[] highPriorityItems = savingsBalanceItems.Where(item => item.SavingsPriority == SavingsBalanceItemPriority.High).OrderBy(item => item.MonthsTillGoalDate).ToArray();
            SavingsBalanceItem[] mediumLowPriorityItems = savingsBalanceItems.Where(item => (item.SavingsPriority == SavingsBalanceItemPriority.Medium || item.SavingsPriority == SavingsBalanceItemPriority.Low)).OrderBy(item => item.SavingsPriority).ToArray();
            SavingsBalanceItem[] nonePriorityItems = savingsBalanceItems.Where(item => item.SavingsPriority == SavingsBalanceItemPriority.None).ToArray();

            Debug.Print("\nHigh: " + highPriorityItems.Count().ToString() +
                " Med/Low: " + mediumLowPriorityItems.Count().ToString() +
                " None: " + nonePriorityItems.Count().ToString());

            Debug.Print("Max Amount Towards Savings: " + maxAmount.ToString());

            if (highPriorityItems.Count() > 0)
            {

                if (highPriorityItems.Sum(item => item.MonthlySavingsToReachGoal) > maxAmount)
                {
                    Debug.Print("High Priority Items Are More Then Max Amount");

                    float ratio = highPriorityItems.Sum(item => (1f / (float)item.MonthsTillGoalDate));

                    foreach (SavingsBalanceItem item in highPriorityItems)
                    {

                        float amt = MathF.Min(maxAmount * ((1f / (float)item.MonthsTillGoalDate) / ratio), item.MonthlySavingsToReachGoal);
                        Debug.Print("Monthly Amount: " + item.MonthlySavingsToReachGoal + " Amount: " + amt);

                        item.SetRecommendedMonthly(amt);
                        totalUsedAmt = totalUsedAmt + amt;
                    }
                }
                else
                {
                    foreach (SavingsBalanceItem item in highPriorityItems)
                    {
                        item.SetRecommendedMonthly(item.MonthlySavingsToReachGoal);
                        totalUsedAmt = totalUsedAmt + item.MonthlySavingsToReachGoal;
                    }

                }


            }

            float highAmt = totalUsedAmt;
            maxAmount = maxAmount - totalUsedAmt;
            Debug.Print("High Priority Used: " + highAmt + "  Amount Left: " + maxAmount);

            if (maxAmount - totalUsedAmt == 0) foreach(SavingsBalanceItem item in mediumLowPriorityItems) item.SetRecommendedMonthly(0);
            else if(mediumLowPriorityItems.Count() > 0)
            {
                if (mediumLowPriorityItems.Sum(item => item.MonthlySavingsToReachGoal) > maxAmount)
                {
                    Debug.Print("Medium/Low Priority Items Are More Then Max Amount");

                    float ratio = mediumLowPriorityItems.Where(item => item.SavingsPriority == SavingsBalanceItemPriority.Medium).Sum(item => (2f / (float)item.MonthsTillGoalDate))
                        + mediumLowPriorityItems.Where(item => item.SavingsPriority == SavingsBalanceItemPriority.Low).Sum(item => (1f / (float)item.MonthsTillGoalDate));
                    

                    foreach (SavingsBalanceItem item in mediumLowPriorityItems)
                    {

                        float amt = item.SavingsPriority == SavingsBalanceItemPriority.Low ?
                            MathF.Min(maxAmount * ((1f / (float)item.MonthsTillGoalDate) / ratio), item.MonthlySavingsToReachGoal) :
                            MathF.Min(maxAmount * ((2f / (float)item.MonthsTillGoalDate) / ratio), item.MonthlySavingsToReachGoal);
                        
                        Debug.Print("Monthly Amount: " + item.MonthlySavingsToReachGoal + " Amount: " + amt);

                        item.SetRecommendedMonthly(amt);
                        totalUsedAmt = totalUsedAmt + amt;
                    }


                }
                else
                {
                    foreach (SavingsBalanceItem item in mediumLowPriorityItems)
                    {
                        item.SetRecommendedMonthly(item.MonthlySavingsToReachGoal);
                        totalUsedAmt = totalUsedAmt + item.MonthlySavingsToReachGoal;
                    }

                }
            }

            float medLowAmt = totalUsedAmt - highAmt;
            maxAmount = maxAmount - totalUsedAmt;
            Debug.Print("Medium/Low Priority Used: " + medLowAmt + "  Amount Left: " + maxAmount);


            if (maxAmount - totalUsedAmt == 0) foreach (SavingsBalanceItem item in nonePriorityItems) item.SetRecommendedMonthly(0);
            else if (nonePriorityItems.Count() > 0)
            {

                if (nonePriorityItems.Sum(item => item.MonthlySavingsToReachGoal) > maxAmount)
                {
                    Debug.Print("High Priority Items Are More Then Max Amount");

                    float ratio = nonePriorityItems.Sum(item => (1f / (float)item.MonthsTillGoalDate));

                    foreach (SavingsBalanceItem item in nonePriorityItems)
                    {

                        float amt = MathF.Min(maxAmount * ((1f / (float)item.MonthsTillGoalDate) / ratio), item.MonthlySavingsToReachGoal);
                        Debug.Print("Monthly Amount: " + item.MonthlySavingsToReachGoal + " Amount: " + amt);

                        item.SetRecommendedMonthly(amt);
                        totalUsedAmt = totalUsedAmt + amt;
                    }

                }
                else
                {
                    foreach (SavingsBalanceItem item in nonePriorityItems)
                    {
                        item.SetRecommendedMonthly(item.MonthlySavingsToReachGoal);
                        totalUsedAmt = totalUsedAmt + item.MonthlySavingsToReachGoal;
                    }

                }


            }


            Debug.Print("None Priority Used: " + (totalUsedAmt - highAmt - medLowAmt) + "  Amount Left: " + (maxAmount - totalUsedAmt));


            base.BalanceItemChanged();
        }
    }
}
