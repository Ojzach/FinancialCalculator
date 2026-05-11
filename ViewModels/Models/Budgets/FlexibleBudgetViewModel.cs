using FinancialCalculator.Models;

namespace FinancialCalculator.ViewModels
{
    public class FlexibleBudgetViewModel : BudgetViewModel
    {

        public override string BudgetType => "Flexible Budget";
        private FlexibleBudget flexibleBudget => (FlexibleBudget)_budget;

        private bool isMinAmountEnabled;
        private bool isMinPercentEnabled;
        private bool isMaxAmountEnabled;
        private bool isMaxPercentEnabled;

        private decimal cachedMinAmount;
        private decimal cachedMinPercent;
        private decimal cachedMaxAmount;
        private decimal cachedMaxPercent;

        public bool IsMinAmountEnabled
        {
            get => isMinAmountEnabled;
            set
            {
                if (isMinAmountEnabled != value)
                {
                    isMinAmountEnabled = value;

                    if (!isMinAmountEnabled)
                    {
                        cachedMinAmount = flexibleBudget.MinMonthlyAmt;
                        flexibleBudget.MinMonthlyAmt = 0m;
                    }
                    else
                    {
                        flexibleBudget.MinMonthlyAmt = cachedMinAmount;
                    }

                    OnPropertyChanged(nameof(IsMinAmountEnabled));
                    OnPropertyChanged(nameof(MinAmount));
                }
            }
        }

        public bool IsMinPercentEnabled
        {
            get => isMinPercentEnabled;
            set
            {
                if (isMinPercentEnabled != value)
                {
                    isMinPercentEnabled = value;

                    if (!isMinPercentEnabled)
                    {
                        cachedMinPercent = flexibleBudget.MinMonthlyPct;
                        flexibleBudget.MinMonthlyPct = 0m;
                    }
                    else
                    {
                        flexibleBudget.MinMonthlyPct = cachedMinPercent;
                    }

                    OnPropertyChanged(nameof(IsMinPercentEnabled));
                    OnPropertyChanged(nameof(MinPercent));
                }
            }
        }

        public bool IsMaxAmountEnabled
        {
            get => isMaxAmountEnabled;
            set
            {
                if (isMaxAmountEnabled != value)
                {
                    isMaxAmountEnabled = value;

                    if (!isMaxAmountEnabled)
                    {
                        cachedMaxAmount = flexibleBudget.MaxMonthlyAmt == decimal.MaxValue ? 0m : flexibleBudget.MaxMonthlyAmt;
                        flexibleBudget.MaxMonthlyAmt = decimal.MaxValue;
                    }
                    else
                    {
                        flexibleBudget.MaxMonthlyAmt = cachedMaxAmount;
                    }

                    OnPropertyChanged(nameof(IsMaxAmountEnabled));
                    OnPropertyChanged(nameof(MaxAmount));
                }
            }
        }

        public bool IsMaxPercentEnabled
        {
            get => isMaxPercentEnabled;
            set
            {
                if (isMaxPercentEnabled != value)
                {
                    isMaxPercentEnabled = value;

                    if (!isMaxPercentEnabled)
                    {
                        cachedMaxPercent = flexibleBudget.MaxMonthlyPct == 1m ? 0m : flexibleBudget.MaxMonthlyPct;
                        flexibleBudget.MaxMonthlyPct = 1m;
                    }
                    else
                    {
                        flexibleBudget.MaxMonthlyPct = cachedMaxPercent;
                    }

                    OnPropertyChanged(nameof(IsMaxPercentEnabled));
                    OnPropertyChanged(nameof(MaxPercent));
                }
            }
        }

        public decimal MinAmount
        {
            get => flexibleBudget.MinMonthlyAmt;
            set
            {
                decimal sanitizedValue = Math.Max(0m, value);

                if (flexibleBudget.MinMonthlyAmt != sanitizedValue)
                {
                    flexibleBudget.MinMonthlyAmt = sanitizedValue;
                }

                cachedMinAmount = sanitizedValue;

                if (!IsMinAmountEnabled && sanitizedValue > 0)
                {
                    isMinAmountEnabled = true;
                    OnPropertyChanged(nameof(IsMinAmountEnabled));
                }

                OnPropertyChanged(nameof(MinAmount));
            }
        }

        public decimal MinPercent
        {
            get => flexibleBudget.MinMonthlyPct;
            set
            {
                decimal sanitizedValue = Math.Clamp(value, 0m, 1m);

                if (flexibleBudget.MinMonthlyPct != sanitizedValue)
                {
                    flexibleBudget.MinMonthlyPct = sanitizedValue;
                }

                cachedMinPercent = sanitizedValue;

                if (!IsMinPercentEnabled && sanitizedValue > 0)
                {
                    isMinPercentEnabled = true;
                    OnPropertyChanged(nameof(IsMinPercentEnabled));
                }

                OnPropertyChanged(nameof(MinPercent));
            }
        }

        public decimal MaxAmount
        {
            get => flexibleBudget.MaxMonthlyAmt == decimal.MaxValue ? cachedMaxAmount : flexibleBudget.MaxMonthlyAmt;
            set
            {
                decimal sanitizedValue = value;

                if (!IsMaxAmountEnabled && value > 0)
                {
                    isMaxAmountEnabled = true;
                    OnPropertyChanged(nameof(IsMaxAmountEnabled));
                }

                if (sanitizedValue <= 0)
                {
                    sanitizedValue = 0m;
                }

                if (flexibleBudget.MaxMonthlyAmt != sanitizedValue)
                {
                    flexibleBudget.MaxMonthlyAmt = sanitizedValue;
                }

                cachedMaxAmount = sanitizedValue;
                OnPropertyChanged(nameof(MaxAmount));
            }
        }

        public decimal MaxPercent
        {
            get => flexibleBudget.MaxMonthlyPct == 1m ? cachedMaxPercent : flexibleBudget.MaxMonthlyPct;
            set
            {
                decimal sanitizedValue = Math.Clamp(value, 0m, 1m);

                if (!IsMaxPercentEnabled && sanitizedValue > 0)
                {
                    isMaxPercentEnabled = true;
                    OnPropertyChanged(nameof(IsMaxPercentEnabled));
                }

                if (flexibleBudget.MaxMonthlyPct != sanitizedValue)
                {
                    flexibleBudget.MaxMonthlyPct = sanitizedValue;
                }

                cachedMaxPercent = sanitizedValue;
                OnPropertyChanged(nameof(MaxPercent));
            }
        }

        public FlexibleBudgetViewModel(FlexibleBudget budget) : base(budget)
        {
            cachedMinAmount = budget.MinMonthlyAmt;
            cachedMinPercent = budget.MinMonthlyPct;
            cachedMaxAmount = budget.MaxMonthlyAmt;
            cachedMaxPercent = budget.MaxMonthlyPct;

            isMinAmountEnabled = budget.MinMonthlyAmt > 0m;
            isMinPercentEnabled = budget.MinMonthlyPct > 0m;
            isMaxAmountEnabled = budget.MaxMonthlyAmt < decimal.MaxValue;
            isMaxPercentEnabled = budget.MaxMonthlyPct < 1m;

            if (!isMaxAmountEnabled) cachedMaxAmount = 0m;
            if (!isMaxPercentEnabled) cachedMaxPercent = 0m;
        }
    }
}
