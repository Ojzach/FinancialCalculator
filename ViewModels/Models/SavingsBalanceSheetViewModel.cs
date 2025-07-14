using FinancialCalculator.Model;
using FinancialCalculator.Stores;


namespace FinancialCalculator.ViewModels
{
    class SavingsBalanceSheetViewModel : BalanceSheetBaseViewModel
    {

        public string MaxTotalSavingsPercentStr
        {
            get => (balanceSheet.maxUsagePercent * 100).ToString("0.00") + "%";
            set
            {
                float p = float.Parse(value.Trim(new Char[] { '%' }));
                balanceSheet.maxUsagePercent = p <= 100 ? p / 100 : 1;
                OnPropertyChanged("MaxTotalSavingsPercentStr");
                BalanceItemChanged();
            }
        }

        public SavingsBalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName) : base(paycheck, balanceSheetName)
        {
            MaxTotalSavingsPercentStr = "60";
        
        }

        public override void CreateBalanceSheetItem()
        {
            base.AddBalanceSheetItem(new SavingsBalanceItem(_paycheck, AddBalanceItemName, new Models.FinancialAccount(AddBalanceItemName, Models.BankAccountType.Checking)));
            ToggleAddBalanceItemBox();
            BalanceItemChanged();
        }


        protected override void BalanceItemChanged()
        {

            float amountLeft = _paycheck.TakeHomeAmount * balanceSheet.maxUsagePercent;

            SavingsBalanceItem[] savingsBalanceItems = balanceSheet.BalanceItems.Cast<SavingsBalanceItem>().ToArray();

            List<List<SavingsBalanceItem>> balanceItemsByPriorityTier = new List<List<SavingsBalanceItem>>()
            {
                savingsBalanceItems.Where(item => item.SavingsPriority == SavingsBalanceItemPriority.High).OrderBy(item => item.MonthsTillGoalDate).ToList(),
                savingsBalanceItems.Where(item => (item.SavingsPriority == SavingsBalanceItemPriority.Medium || item.SavingsPriority == SavingsBalanceItemPriority.Low)).OrderBy(item => item.SavingsPriority).ToList(),
                savingsBalanceItems.Where(item => item.SavingsPriority == SavingsBalanceItemPriority.None).ToList()
            };

            foreach(List<SavingsBalanceItem> tierItems in balanceItemsByPriorityTier)
            {

                if (tierItems.Count > 0)
                {
                    float totalUsed = 0;

                    if (amountLeft <= 0)
                    {
                        foreach (SavingsBalanceItem item in tierItems) item.SetRecommendedMonthly(0);

                    }
                    else if (tierItems.Sum(item => item.MonthlySavingsToReachGoal) < amountLeft)
                    {

                        foreach (SavingsBalanceItem item in tierItems)
                        {
                            item.SetRecommendedMonthly(item.MonthlySavingsToReachGoal);
                            totalUsed += item.MonthlySavingsToReachGoal;
                        }

                    }
                    else
                    {

                        bool finishedAllocation = false;


                        while (finishedAllocation == false)
                        {
                            finishedAllocation = true;

                            float ratioTotal = tierItems.Sum(item => (float)item.SavingsPriority / (float)item.MonthsTillGoalDate);

                            foreach (SavingsBalanceItem item in tierItems)
                            {
                                float calculatedAmt = (amountLeft - totalUsed) * ((float)item.SavingsPriority / (float)item.MonthsTillGoalDate / ratioTotal);

                                if (calculatedAmt >= item.MonthlySavingsToReachGoal)
                                {
                                    item.SetRecommendedMonthly(item.MonthlySavingsToReachGoal);
                                    totalUsed += item.MonthlySavingsToReachGoal;
                                    tierItems.Remove(item);
                                    finishedAllocation = false;
                                    break;
                                }
                                else item.SetRecommendedMonthly(calculatedAmt);
                            }

                        }

                        foreach(SavingsBalanceItem item in tierItems)
                        {
                            totalUsed += item.MonthlyAmt;
                        }
                    }

                    amountLeft = amountLeft - totalUsed;
                }
            }

            base.BalanceItemChanged();
        }

        public override void AddBalanceSheetItem(BalanceItem item)
        {
            base.AddBalanceSheetItem(item);
            BalanceItemChanged();
        }
    }
}
