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

        private float cachedMinAmount;
        private float cachedMinPercent;
        private float cachedMaxAmount;
        private float cachedMaxPercent;

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
                        flexibleBudget.MinMonthlyAmt = 0f;
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
                        flexibleBudget.MinMonthlyPct = 0f;
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
                        cachedMaxAmount = flexibleBudget.MaxMonthlyAmt == float.MaxValue ? 0f : flexibleBudget.MaxMonthlyAmt;
                        flexibleBudget.MaxMonthlyAmt = float.MaxValue;
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
                        cachedMaxPercent = flexibleBudget.MaxMonthlyPct == 1f ? 0f : flexibleBudget.MaxMonthlyPct;
                        flexibleBudget.MaxMonthlyPct = 1f;
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

        public float MinAmount
        {
            get => flexibleBudget.MinMonthlyAmt;
            set
            {
                float sanitizedValue = Math.Max(0f, value);

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

        public float MinPercent
        {
            get => flexibleBudget.MinMonthlyPct;
            set
            {
                float sanitizedValue = Math.Clamp(value, 0f, 1f);

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

        public float MaxAmount
        {
            get => flexibleBudget.MaxMonthlyAmt == float.MaxValue ? cachedMaxAmount : flexibleBudget.MaxMonthlyAmt;
            set
            {
                float sanitizedValue = value;

                if (!IsMaxAmountEnabled && value > 0)
                {
                    isMaxAmountEnabled = true;
                    OnPropertyChanged(nameof(IsMaxAmountEnabled));
                }

                if (sanitizedValue <= 0)
                {
                    sanitizedValue = 0f;
                }

                if (flexibleBudget.MaxMonthlyAmt != sanitizedValue)
                {
                    flexibleBudget.MaxMonthlyAmt = sanitizedValue;
                }

                cachedMaxAmount = sanitizedValue;
                OnPropertyChanged(nameof(MaxAmount));
            }
        }

        public float MaxPercent
        {
            get => flexibleBudget.MaxMonthlyPct == 1f ? cachedMaxPercent : flexibleBudget.MaxMonthlyPct;
            set
            {
                float sanitizedValue = Math.Clamp(value, 0f, 1f);

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

            isMinAmountEnabled = budget.MinMonthlyAmt > 0f;
            isMinPercentEnabled = budget.MinMonthlyPct > 0f;
            isMaxAmountEnabled = budget.MaxMonthlyAmt < float.MaxValue;
            isMaxPercentEnabled = budget.MaxMonthlyPct < 1f;

            if (!isMaxAmountEnabled) cachedMaxAmount = 0f;
            if (!isMaxPercentEnabled) cachedMaxPercent = 0f;
        }
    }
}
